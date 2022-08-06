using System;
using System.Collections;

using UnityEditor;
using UnityEngine;

using Codice.Client.Common;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using PlasticGui;
using PlasticGui.Configuration.CloudEdition;
using PlasticGui.SwitcherWindow.Repositories;
using PlasticGui.SwitcherWindow.Workspaces;
using Unity.PlasticSCM.Editor.UI.Progress;

namespace Unity.PlasticSCM.Editor.Views.CreateWorkspace
{
    internal class CreateWorkspaceView :
        IPlasticDialogCloser,
        IWorkspacesRefreshableView
    {
        internal interface ICreateWorkspaceListener
        {
            void OnWorkspaceCreated(WorkspaceInfo wkInfo, bool isGluonMode);
        }

        internal CreateWorkspaceView(
            EditorWindow parentWindow,
            ICreateWorkspaceListener listener,
            PlasticAPI plasticApi,
            string workspacePath)
        {
            mParentWindow = parentWindow;
            mCreateWorkspaceListener = listener;
            mWorkspacePath = workspacePath;

            mProgressControls = new ProgressControlsForViews();
            mWorkspaceOperations = new WorkspaceOperations(this, mProgressControls);
            mCreateWorkspaceState = CreateWorkspaceViewState.BuildForProjectDefaults();

            Initialize(plasticApi);
        }

        internal void Update()
        {
            mProgressControls.UpdateProgress(mParentWindow);
        }

        internal void OnGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                mProgressControls.ProgressData.CopyInto(
                    mCreateWorkspaceState.ProgressData);
            }

            string repositoryName = mCreateWorkspaceState.RepositoryName;

            DrawCreateWorkspace.ForState(
                CreateRepository,
                ValidateAndCreateWorkspace,
                mParentWindow,
                mDefaultServer,
                ref mCreateWorkspaceState);

            if (repositoryName == mCreateWorkspaceState.RepositoryName)
                return;

            OnRepositoryChanged(
                mDialogUserAssistant,
                mCreateWorkspaceState,
                mWorkspacePath);
        }

        void Initialize(PlasticAPI plasticApi)
        {
            ((IProgressControls)mProgressControls).ShowProgress(string.Empty);

            WorkspaceInfo[] allWorkspaces = null;
            IList allRepositories = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    mDefaultServer = GetDefaultServer.ToCreateWorkspace();

                    allWorkspaces = plasticApi.GetAllWorkspacesArray();

                    allRepositories = plasticApi.GetAllRepositories(
                        mDefaultServer, true);
                },
                /*afterOperationDelegate*/ delegate
                {
                    ((IProgressControls)mProgressControls).HideProgress();

                    if (waiter.Exception != null)
                    {
                        DisplayException(mProgressControls, waiter.Exception);
                        return;
                    }

                    string serverSpecPart = string.Format("@{0}",
                        mDefaultServer);

                    mCreateWorkspaceState.RepositoryName = ValidRepositoryName.Get(
                        string.Format("{0}{1}",
                            mCreateWorkspaceState.RepositoryName,
                            serverSpecPart),
                        allRepositories);

                    mCreateWorkspaceState.WorkspaceName =
                        mCreateWorkspaceState.RepositoryName.Replace(
                            serverSpecPart, string.Empty);

                    mDialogUserAssistant = new CreateWorkspaceDialogUserAssistant(
                        mWorkspacePath,
                        allWorkspaces);

                    OnRepositoryChanged(
                        mDialogUserAssistant,
                        mCreateWorkspaceState,
                        mWorkspacePath);
                });
        }

        static void OnRepositoryChanged(
            CreateWorkspaceDialogUserAssistant dialogUserAssistant,
            CreateWorkspaceViewState createWorkspaceState,
            string workspacePath)
        {
            if (dialogUserAssistant == null)
                return;

            dialogUserAssistant.RepositoryChanged(
                createWorkspaceState.RepositoryName,
                createWorkspaceState.WorkspaceName,
                workspacePath);

            createWorkspaceState.WorkspaceName =
                dialogUserAssistant.GetProposedWorkspaceName();
        }

        void CreateRepository(
            RepositoryCreationData data)
        {
            if (!data.Result)
                return;

            ((IProgressControls)mProgressControls).ShowProgress(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.CreatingRepository,
                    data.RepName));

            RepositoryInfo createdRepository = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    createdRepository = Plastic.API.CreateRepository(
                        data.ServerName, data.RepName);
                },
                /*afterOperationDelegate*/ delegate
                {
                    ((IProgressControls)mProgressControls).HideProgress();

                    if (waiter.Exception != null)
                    {
                        DisplayException(mProgressControls, waiter.Exception);
                        return;
                    }

                    if (createdRepository == null)
                        return;

                    mCreateWorkspaceState.RepositoryName =
                        createdRepository.GetRepSpec().ToString();
                });
        }

        void ValidateAndCreateWorkspace(
            CreateWorkspaceViewState state)
        {
            mWkCreationData = BuildCreationDataFromState(
                state, mWorkspacePath);

            WorkspaceCreationValidation.AsyncValidation(
                mWkCreationData, this, mProgressControls);
            // validation calls IPlasticDialogCloser.CloseDialog()
            // when the validation is ok
        }

        void IPlasticDialogCloser.CloseDialog()
        {
            ((IProgressControls)mProgressControls).ShowProgress(string.Empty);

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    RepositorySpec repSpec = new SpecGenerator().GenRepositorySpec(
                        false, mWkCreationData.Repository);

                    bool repositoryExist = Plastic.API.CheckRepositoryExists(
                        repSpec.Server, repSpec.Name);

                    if (!repositoryExist)
                        Plastic.API.CreateRepository(repSpec.Server, repSpec.Name);
                },
                /*afterOperationDelegate*/ delegate
                {
                    ((IProgressControls)mProgressControls).HideProgress();

                    if (waiter.Exception != null)
                    {
                        DisplayException(mProgressControls, waiter.Exception);
                        return;
                    }

                    mWkCreationData.Result = true;
                    mWorkspaceOperations.CreateWorkspace(mWkCreationData);
                    // the operation calls IWorkspacesRefreshableView.RefreshAndSelect
                    // when the workspace is created
                });
        }

        void IWorkspacesRefreshableView.RefreshAndSelect(WorkspaceInfo wkInfo)
        {
            mCreateWorkspaceListener.OnWorkspaceCreated(
                wkInfo, mWkCreationData.IsGluonWorkspace);
        }

        static WorkspaceCreationData BuildCreationDataFromState(
            CreateWorkspaceViewState state,
            string workspacePath)
        {
            return new WorkspaceCreationData(
                state.WorkspaceName,
                workspacePath,
                state.RepositoryName,
                state.WorkspaceMode ==
                    CreateWorkspaceViewState.WorkspaceModes.Gluon,
                false);
        }

        static void DisplayException(
            IProgressControls progressControls,
            Exception ex)
        {
            ExceptionsHandler.LogException(
                "CreateWorkspaceView", ex);

            progressControls.ShowError(
                ExceptionsHandler.GetCorrectExceptionMessage(ex));
        }

        class GetDefaultServer
        {
            internal static string ToCreateWorkspace()
            {
                string clientConfServer = Plastic.ConfigAPI.GetClientConfServer();

                if (!EditionToken.IsCloudEdition())
                    return clientConfServer;

                string organizationName = PlasticGuiConfig.Get().
                    Configuration.CloudOrganization;

                if (!string.IsNullOrEmpty(organizationName))
                    return GetCloudServer(organizationName);

                CloudEditionCreds.Data config =
                    CloudEditionCreds.GetFromClientConf();

                organizationName = CloudOrganizationRetriever.
                    GetOrganization(config.Email, config.Password);

                if (string.IsNullOrEmpty(organizationName))
                    return clientConfServer;

                SaveCloudOrganization.ToPlasticGuiConfig(organizationName);

                return GetCloudServer(organizationName);
            }

            static string GetCloudServer(string organizationName)
            {
                return string.Format("{0}@{1}",
                    organizationName, CloudServer.Alias);
            }
        }

        WorkspaceCreationData mWkCreationData;
        CreateWorkspaceViewState mCreateWorkspaceState;

        CreateWorkspaceDialogUserAssistant mDialogUserAssistant;

        string mDefaultServer;

        readonly WorkspaceOperations mWorkspaceOperations;
        readonly ProgressControlsForViews mProgressControls;
        readonly string mWorkspacePath;
        readonly ICreateWorkspaceListener mCreateWorkspaceListener;
        readonly EditorWindow mParentWindow;
    }
}

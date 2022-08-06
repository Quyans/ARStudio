using System;
using System.Threading;

using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.Commands;
using Codice.Client.Common;
using Codice.CM.Common;
using Codice.LogWrapper;
using PlasticGui;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WorkspaceWindow.Update;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.ProjectDownloader
{
    internal class DownloadRepositoryOperation
    {
        internal void DownloadRepositoryToPathIfNeeded(
            string cloudRepository,
            string cloudOrganization,
            string projectPath)
        {
            var allowAutoRefreshFeature = new AssetDatabase().GetType().GetMethod("DisallowAutoRefresh") != null;
            
            #if allowAutoRefreshFeature
                AssetDatabase.DisallowAutoRefresh();
            #endif

            try
            {
                BuildProgressSpeedAndRemainingTime.ProgressData progressData =
                    new BuildProgressSpeedAndRemainingTime.ProgressData(DateTime.Now);

                ThreadPool.QueueUserWorkItem(
                    DownloadRepository,
                    new DownloadRepositoryParameters()
                    {
                        CloudOrganization = cloudOrganization,
                        CloudRepository = cloudRepository,
                        ProjectPath = projectPath,
                        AccessToken = CloudProjectSettings.accessToken
                    });

                while (!mOperationFinished)
                {
                    if (mDisplayProgress)
                    {
                        DisplayProgress(
                            mUpdateNotifier.GetUpdateStatus(),
                            progressData,
                            cloudRepository);
                    }

                    Thread.Sleep(150);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();

                #if allowAutoRefreshFeature
                    AssetDatabase.AllowAutoRefresh();
                #endif

                DisableCollabIfEnabled(projectPath);

                RefreshAsset.UnityAssetDatabase();

                if (!mOperationFailed)
                {
                    // we just download a cloud project,
                    // so let's assume we're going to use Cloud Edition
                    SetupUnityEditionToken.ToCloud();

                    ShowWindow.Plastic();
                }
            }
        }

        static void DisplayProgress(
            UpdateOperationStatus status,
            BuildProgressSpeedAndRemainingTime.ProgressData progressData,
            string cloudRepository)
        {
            string totalProgressMessage = UpdateProgressRender.
                GetProgressString(status, progressData);

            float totalProgressPercent = GetProgressBarPercent.
                ForTransfer(status.UpdatedSize, status.TotalSize) / 100f;

            EditorUtility.DisplayProgressBar(
               string.Format("Downloading {0}", cloudRepository),
               totalProgressMessage, totalProgressPercent);
        }

        static void DisableCollabIfEnabled(string projectPath)
        {
            if (!CollabPlugin.IsEnabled())
                return;

            CollabPlugin.Disable();

            mLog.DebugFormat("Disabled Collab Plugin on Project: {0}",
                projectPath);
        }

        void DownloadRepository(object state)
        {
            DownloadRepositoryParameters parameters = (DownloadRepositoryParameters)state;

            try
            {
                RepositorySpec repSpec = BuildRepSpec(
                    parameters.CloudRepository,
                    parameters.CloudOrganization);

                if (!ClientConfig.IsConfigured())
                {
                    AutoConfigClientConf.FromUnityAccessToken(
                        parameters.AccessToken,
                        repSpec,
                        parameters.ProjectPath);
                }

                if (WorkspaceExists(parameters.ProjectPath))
                {
                    // each domain reload, the package is reloaded.
                    // way need to check if we already downloaded it
                    return;
                }

                if (MonoRuntime.IsRunningUnder35RuntimeOrOlder())
                {
                    Debug.LogWarning(
                        "Plastic SCM: Detected an unsupported Runtime Version. " +
                        "Download Cloud Projects feature is not available for this Runtime Version. " +
                        "Please upgrade your Unity installation to the last LTS version and try again.");
                    return;
                }

                mDisplayProgress = true;

                WorkspaceInfo wkInfo = CreateWorkspace(
                    repSpec,
                   parameters.ProjectPath);

                mLog.DebugFormat("Created workspace {0} on {1}",
                    wkInfo.Name,
                    wkInfo.ClientPath);

                Plastic.API.Update(
                    wkInfo.ClientPath,
                    UpdateFlags.None,
                    null,
                    mUpdateNotifier);
            }
            catch(Exception ex)
            {
                LogException(ex);

                UnityEngine.Debug.LogErrorFormat(
                    "Error downloading Cloud Project: {0}",
                    ex.Message);

                mOperationFailed = true;
            }
            finally
            {
                mOperationFinished = true;
            }
        }

        static WorkspaceInfo CreateWorkspace(
            RepositorySpec repositorySpec,
            string projectPath)
        {
            CreateWorkspaceDialogUserAssistant assistant = new CreateWorkspaceDialogUserAssistant(
                PlasticGuiConfig.Get().Configuration.DefaultWorkspaceRoot,
                Plastic.API.GetAllWorkspacesArray());

            assistant.RepositoryChanged(
                repositorySpec.ToString(),
                string.Empty,
                string.Empty);

            return Plastic.API.CreateWorkspace(
                projectPath,
                assistant.GetProposedWorkspaceName(),
                repositorySpec.ToString());
        }

        static RepositorySpec BuildRepSpec(
            string cloudRepository,
            string cloudOrganization)
        {
            return new RepositorySpec()
            {
                Name = cloudRepository,
                Server = string.Format("{0}@{1}",
                    cloudOrganization,
                    CloudServer.Alias)
            };
        }

        static void LogException(Exception ex)
        {
            mLog.WarnFormat("Message: {0}", ex.Message);

            mLog.DebugFormat(
                "StackTrace:{0}{1}",
                Environment.NewLine, ex.StackTrace);
        }

        static bool WorkspaceExists(string projectPath)
        {
            return Plastic.API.GetWorkspaceFromPath(projectPath) != null;
        }

        class DownloadRepositoryParameters
        {
            internal string CloudRepository;
            internal string CloudOrganization;
            internal string ProjectPath;
            internal string AccessToken;
        }

        volatile bool mOperationFinished = false;
        volatile bool mOperationFailed = false;
        volatile bool mDisplayProgress;

        UpdateNotifier mUpdateNotifier = new UpdateNotifier();

        static readonly ILog mLog = LogManager.GetLogger("DownloadRepositoryOperation");
    }
}

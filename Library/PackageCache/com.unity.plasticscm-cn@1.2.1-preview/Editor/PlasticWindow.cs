using System;
using System.IO;
using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.Common;
using Codice.Client.Common.Encryption;
using Codice.Client.Common.EventTracking;
using Codice.Client.Common.FsNodeReaders;
using Codice.Client.Common.Threading;
using Codice.Client.Common.WebApi;
using Codice.CM.Common;
using Codice.LogWrapper;
using CodiceApp.EventTracking;
using GluonGui;
using PlasticGui;
using PlasticGui.Gluon;
using PlasticGui.WorkspaceWindow;
using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetUtils.Processor;
using Unity.PlasticSCM.Editor.AutoSetup;
using Unity.PlasticSCM.Editor.Configuration;
using Unity.PlasticSCM.Editor.Inspector;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Avatar;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.Views.CreateWorkspace;
using Unity.PlasticSCM.Editor.Views.Welcome;

using GluonCheckIncomingChanges = PlasticGui.Gluon.WorkspaceWindow.CheckIncomingChanges;
using GluonNewIncomingChangesUpdater = PlasticGui.Gluon.WorkspaceWindow.NewIncomingChangesUpdater;
using EventTracking = PlasticGui.EventTracking.EventTracking;

namespace Unity.PlasticSCM.Editor
{
    internal class PlasticWindow : EditorWindow,
        PlasticGui.WorkspaceWindow.CheckIncomingChanges.IAutoRefreshIncomingChangesView,
        GluonCheckIncomingChanges.IAutoRefreshIncomingChangesView,
        CreateWorkspaceView.ICreateWorkspaceListener
    {
        internal PlasticGUIClient PlasticClientForTesting { get { return mPlasticClient; } }
        internal ViewSwitcher ViewSwitcherForTesting { get { return mViewSwitcher; } }
        internal IPlasticAPI PlasticApiForTesting { get { return mPlasticAPI; } }

        void PlasticGui.WorkspaceWindow.CheckIncomingChanges.IAutoRefreshIncomingChangesView.IfVisible()
        {
            mViewSwitcher.AutoRefreshIncomingChangesView();
        }

        void GluonCheckIncomingChanges.IAutoRefreshIncomingChangesView.IfVisible()
        {
            mViewSwitcher.AutoRefreshIncomingChangesView();
        }

        void CreateWorkspaceView.ICreateWorkspaceListener.OnWorkspaceCreated(
            WorkspaceInfo wkInfo, bool isGluonMode)
        {
            mWkInfo = wkInfo;
            mIsGluonMode = isGluonMode;
            mWelcomeView = null;

            if (mIsGluonMode)
                ConfigurePartialWorkspace.AsFullyChecked(mWkInfo);

            InitializePlastic();
            Repaint();
        }

        void OnEnable()
        {
            wantsMouseMove = true;

            if (mException != null)
                return;

            GuiMessage.Initialize(new UnityPlasticGuiMessage(this));

            PlasticApp.Initialize();

            RegisterApplicationFocusHandlers(this);

            CredentialsUIRegistrar.RegisterCredentialsUI(
                new CredentialsUiImpl(this));
            ClientEncryptionServiceProvider.SetEncryptionPasswordProvider(
                new MissingEncryptionPasswordPromptHandler(this));

            mPlasticAPI = new PlasticAPI();

            mPingEventLoop = new PingEventLoop();
            mEventSenderRestApi = new SimpleEventSenderRestApi(
                PlasticWebApiUris.GetBaseUri());
            mEventSenderScheduler = EventTracking.Configure(
                mEventSenderRestApi,
                ApplicationIdentifier.UnityPackage,
                IdentifyEventPlatform.Get());

            if (mEventSenderScheduler != null)
                mPingEventLoop.Start();

            InitializePlastic();

            EditorApplication.update += AutoCommitOperation.AutoCommit;
        }

        void OnDisable()
        {
            AssetsProcessors.Disable();
            
            EditorApplication.update -= AutoCommitOperation.AutoCommit;
            
            if (mWkInfo != null)
                WorkspaceFsNodeReaderCachesCleaner.CleanWorkspaceFsNodeReader(mWkInfo);

            if (mException != null)
                return;

            if (mWkInfo == null)
            {
                ClosePlasticWindow(this);
                return;
            }

            mViewSwitcher.OnDisable();

            ClosePlasticWindow(this);
        }

        void OnDestroy()
        {
            if (mException != null)
                return;

            if (mWkInfo == null)
                return;

            if (!mPlasticClient.IsOperationInProgress())
                return;

            bool bCloseWindow = GuiMessage.ShowQuestion(
                PlasticLocalization.GetString(PlasticLocalization.Name.OperationRunning),
                PlasticLocalization.GetString(PlasticLocalization.Name.ConfirmClosingRunningOperation),
                PlasticLocalization.GetString(PlasticLocalization.Name.YesButton));

            if (bCloseWindow)
                return;

            mForceToOpen = true;
            ShowPlasticWindow(this);
        }

        void OnFocus()
        {
            if (mException != null)
                return;

            if (mWkInfo == null)
                return;

            mViewSwitcher.AutoRefreshPendingChangesView();
            mViewSwitcher.AutoRefreshIncomingChangesView();
        }

        void OnGUI()
        {
            if (mException != null)
            {
                DoExceptionErrorArea();
                return;
            }

            try
            {
                bool isPlasticExeAvailable = IsExeAvailable.ForMode(mIsGluonMode);
                bool clientNeedsConfiguration = UnityConfigurationChecker.NeedsConfiguration();

                if (NeedsToDisplayWelcomeView(
                        isPlasticExeAvailable,
                        clientNeedsConfiguration,
                        mWkInfo))
                {
                    GetWelcomeView().OnGUI(
                        isPlasticExeAvailable,
                        clientNeedsConfiguration,
                        mIsGluonMode);
                    return;
                }

                DoHeader(
                    mWkInfo,
                    mPlasticClient,
                    mViewSwitcher,
                    mViewSwitcher,
                    mIsGluonMode,
                    mIncomingChangesNotificationPanel,
                    mIncomingChangesFromMainNotificationPanel);
                DoTabToolbar(
                    mWkInfo,
                    mPlasticClient,
                    mViewSwitcher,
                    mIsGluonMode);

                mViewSwitcher.TabViewGUI();

                if (mPlasticClient.IsOperationInProgress())
                    DrawProgressForOperations.For(
                        mPlasticClient, mPlasticClient.Progress,
                        position.width);
            }
            catch (Exception ex)
            {
                if (IsExitGUIException(ex))
                    throw;

                GUI.enabled = true;

                if (IsIMGUIPaintException(ex))
                {
                    ExceptionsHandler.LogException("PlasticWindow", ex);
                    return;
                }

                mException = ex;

                DoExceptionErrorArea();

                ExceptionsHandler.HandleException("OnGUI", ex);
            }
        }

        void Update()
        {
            if (mException != null)
                return;

            if (mWkInfo == null)
                return;

            try
            {
                double currentUpdateTime = EditorApplication.timeSinceStartup;
                double elapsedSeconds = currentUpdateTime - mLastUpdateTime;

                mViewSwitcher.Update();
                mPlasticClient.OnParentUpdated(elapsedSeconds);

                if (mWelcomeView != null)
                    mWelcomeView.Update();

                mLastUpdateTime = currentUpdateTime;
            }
            catch (Exception ex)
            {
                mException = ex;

                ExceptionsHandler.HandleException("Update", ex);
            }
        }

        void InitializePlastic()
        {
            if (mForceToOpen)
            {
                mForceToOpen = false;
                return;
            }

            try
            {
                if (UnityConfigurationChecker.NeedsConfiguration())
                    return;

                mWkInfo = FindWorkspace.InfoForApplicationPath(
                    Application.dataPath, mPlasticAPI);

                if (mWkInfo == null)
                {
                    AssetMenuItems.Disable();
                    return;
                }
                
                // SetupCloudProjectId.ForWorkspace(mWkInfo, mPlasticAPI);

                DisableVCSIfEnabled(mWkInfo.ClientPath);

                mIsGluonMode = mPlasticAPI.IsGluonWorkspace(mWkInfo);

                IAssetStatusCache assetStatusCache =
                    new AssetStatusCache(
                        mWkInfo,
                        mIsGluonMode,
                        RepaintProjectWindow);

                AssetsProcessors.Enable(
                    mPlasticAPI,
                    assetStatusCache);

                mPingEventLoop.SetWorkspace(mWkInfo);
                mEventSenderRestApi.SetToken(BuildToken.FromServerProfile(
                    ClientConfig.Get().GetDefaultProfile()));

                InitializeNewIncomingChanges(mWkInfo, mIsGluonMode);

                ViewHost viewHost = new ViewHost();

                PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                    new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(mWkInfo);

                mViewSwitcher = new ViewSwitcher(
                    mWkInfo,
                    viewHost,
                    mIsGluonMode,
                    pendingChanges,
                    mDeveloperNewIncomingChangesUpdater,
                    mGluonNewIncomingChangesUpdater,
                    mIncomingChangesNotificationPanel,
                    assetStatusCache,
                    this);

                mPlasticClient = new PlasticGUIClient(
                    mWkInfo,
                    mViewSwitcher,
                    mViewSwitcher,
                    viewHost,
                    pendingChanges,
                    mDeveloperNewIncomingChangesUpdater,
                    mGluonNewIncomingChangesUpdater,
                    this,
                    new UnityPlasticGuiMessage(this));

                mViewSwitcher.SetPlasticGUIClient(mPlasticClient);
                mViewSwitcher.ShowInitialView();

                UnityStyles.Initialize(Repaint);

                AssetOperations.IAssetSelection inspectorAssetSelection =
                    new InspectorAssetSelection();

                AssetOperations.IAssetSelection projectViewAssetSelection =
                    new ProjectViewAssetSelection();

                AssetOperations inspectorAssetOperations =
                    new AssetOperations(
                        mWkInfo,
                        mPlasticClient,
                        mViewSwitcher,
                        mViewSwitcher,
                        viewHost,
                        mDeveloperNewIncomingChangesUpdater,
                        assetStatusCache,
                        mViewSwitcher,
                        mViewSwitcher,
                        this,
                        inspectorAssetSelection,
                        mIsGluonMode);

                AssetOperations projectViewAssetOperations =
                    new AssetOperations(
                        mWkInfo,
                        mPlasticClient,
                        mViewSwitcher,
                        mViewSwitcher,
                        viewHost,
                        mDeveloperNewIncomingChangesUpdater,
                        assetStatusCache,
                        mViewSwitcher,
                        mViewSwitcher,
                        this,
                        projectViewAssetSelection,
                        mIsGluonMode);

                AssetMenuItems.Enable(
                    projectViewAssetOperations,
                    assetStatusCache,
                    projectViewAssetSelection);

                DrawInspectorOperations.Enable(
                    inspectorAssetOperations,
                    assetStatusCache,
                    inspectorAssetSelection);

                DrawAssetOverlay.Initialize(
                    assetStatusCache,
                    RepaintProjectWindow);

                AutoCommitOperation.SetPlasticGUIClient(mPlasticClient);
                
                mLastUpdateTime = EditorApplication.timeSinceStartup;
            }
            catch (Exception ex)
            {
                mException = ex;

                ExceptionsHandler.HandleException("InitializePlastic", ex);
            }
        }
        
        void InitializeNewIncomingChanges(
            WorkspaceInfo wkInfo,
            bool bIsGluonMode)
        {
            if (bIsGluonMode)
            {
                Gluon.IncomingChangesNotificationPanel gluonPanel =
                    new Gluon.IncomingChangesNotificationPanel(this);
                mGluonNewIncomingChangesUpdater =
                    NewIncomingChanges.BuildUpdaterForGluon(
                        wkInfo,
                        this,
                        gluonPanel,
                        new GluonCheckIncomingChanges.CalculateIncomingChanges());
                mIncomingChangesNotificationPanel = gluonPanel;
                return;
            }

            Developer.IncomingChangesNotificationPanel developerPanel =
                new Developer.IncomingChangesNotificationPanel(this);
            mDeveloperNewIncomingChangesUpdater =
                NewIncomingChanges.BuildUpdaterForDeveloper(
                    wkInfo, this, developerPanel);
            mIncomingChangesNotificationPanel = developerPanel;
            
            Developer.IncomingChangesNotificationPanel developerFromMainPanel =
                new Developer.IncomingChangesNotificationPanel(this);
            mDeveloperNewIncomingChangesFromMainUpdater =
                NewIncomingChanges.BuildUpdaterFromMainForDeveloper(
                    wkInfo, developerFromMainPanel);
            mIncomingChangesFromMainNotificationPanel = developerFromMainPanel;
            
        }

        void OnApplicationActivated()
        {
            if (mException != null)
                return;

            Reload.IfWorkspaceConfigChanged(
                mPlasticAPI, mWkInfo, mIsGluonMode,
                ExecuteFullReload);

            if (mWkInfo == null)
                return;

            ((IWorkspaceWindow)mPlasticClient).UpdateTitle();

            NewIncomingChanges.LaunchUpdater(
                mDeveloperNewIncomingChangesUpdater,
                mGluonNewIncomingChangesUpdater,
                mDeveloperNewIncomingChangesFromMainUpdater);

            mViewSwitcher.AutoRefreshPendingChangesView();
            mViewSwitcher.AutoRefreshIncomingChangesView();
        }

        void OnApplicationDeactivated()
        {
            if (mException != null)
                return;

            if (mWkInfo == null)
                return;

            NewIncomingChanges.StopUpdater(
                mDeveloperNewIncomingChangesUpdater,
                mGluonNewIncomingChangesUpdater,
                mDeveloperNewIncomingChangesFromMainUpdater);
        }

        void ExecuteFullReload()
        {
            mException = null;

            DisposeNewIncomingChanges(this);

            InitializePlastic();
        }

        void DoExceptionErrorArea()
        {
            string labelText = PlasticLocalization.GetString(
                PlasticLocalization.Name.UnexpectedError);

            string buttonText = PlasticLocalization.GetString(
                PlasticLocalization.Name.ReloadButton);

            DrawActionHelpBox.For(
                Images.GetErrorDialogIcon(), labelText, buttonText,
                ExecuteFullReload);
        }

        WelcomeView GetWelcomeView()
        {
            if (mWelcomeView != null)
                return mWelcomeView;

            mWelcomeView = new WelcomeView(
                this, this, mPlasticAPI);

            return mWelcomeView;
        }

        static void DoHeader(
            WorkspaceInfo workspaceInfo,
            PlasticGUIClient plasticClient,
            IMergeViewLauncher mergeViewLauncher,
            IGluonViewSwitcher gluonSwitcher,
            bool isGluonMode,
            IIncomingChangesNotificationPanel incomingChangesNotificationPanel,
            IIncomingChangesNotificationPanel incomingChangesFromMainNotificationPanel)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Label(
                plasticClient.HeaderTitle,
                UnityStyles.PlasticWindow.HeaderTitleLabel);

            GUILayout.FlexibleSpace();

            DrawIncomingChangesFromMainNotificationPanel.ForMode(
                workspaceInfo,
                isGluonMode,
                incomingChangesNotificationPanel.IsVisible,
                incomingChangesFromMainNotificationPanel.IsVisible,
                incomingChangesFromMainNotificationPanel.Data);

            DrawIncomingChangesNotificationPanel.ForMode(
                workspaceInfo, plasticClient,
                mergeViewLauncher, gluonSwitcher, isGluonMode,
                incomingChangesNotificationPanel.IsVisible,
                incomingChangesNotificationPanel.Data);

            //TODO: Codice - beta: hide the switcher until the update dialog is implemented
            //DrawGuiModeSwitcher.ForMode(
            //    isGluonMode, plasticClient, changesTreeView, editorWindow);

            DoLaunchPlasticHubButton(workspaceInfo);
            
            EditorGUILayout.EndHorizontal();
        }

        static void DoTabToolbar(
            WorkspaceInfo workspaceInfo,
            PlasticGUIClient plasticClient,
            ViewSwitcher viewSwitcher,
            bool isGluonMode)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            viewSwitcher.TabButtonsGUI();

            GUILayout.FlexibleSpace();

            AutoCommitOperation.DoAutoCommitCheckBox();
            
            DoLaunchButtons(workspaceInfo, isGluonMode);

            EditorGUILayout.EndHorizontal();
        }

        static void DoLaunchButtons(
            WorkspaceInfo wkInfo,
            bool isGluonMode)
        {
            //TODO: Codice - beta: hide the diff button until the behavior is implemented
            /*GUILayout.Button(PlasticLocalization.GetString(
                PlasticLocalization.Name.DiffWindowMenuItemDiff),
                EditorStyles.toolbarButton,
                GUILayout.Width(UnityConstants.REGULAR_BUTTON_WIDTH));*/

            if (isGluonMode)
            {
                if (DrawActionButton.For("Configure Gluon"))
                    LaunchTool.OpenWorkspaceConfiguration(wkInfo);
            }
            else
            {
                if (DrawActionButton.For("Launch branch explorer"))
                    LaunchTool.OpenBranchExplorer(wkInfo);
            }

            string openToolText = isGluonMode ?
                "Launch Gluon" : "Launch Plastic";

            if (DrawActionButton.For(openToolText))
                LaunchTool.OpenGUIForMode(wkInfo, isGluonMode);
        }

        static void DoLaunchPlasticHubButton(WorkspaceInfo wkInfo)
        {
            string repoURL = PlasticHubUtil.GetRepoURLFromWorkspace(wkInfo);
            if (string.IsNullOrEmpty(repoURL))
            {
                return;
            }

            string buttonText = "Open in PlasticHub";
            
            GUIContent buttonContent = new GUIContent(buttonText);
            GUIStyle buttonStyle = EditorStyles.miniButton;
            
            Rect rt = GUILayoutUtility.GetRect(
                buttonContent, buttonStyle,
                GUILayout.MinWidth(UnityConstants.REGULAR_BUTTON_WIDTH));

            if (GUI.Button(rt, buttonText, buttonStyle))
            {
                Application.OpenURL(repoURL);
            }
        }

        static void DisableVCSIfEnabled(string projectPath)
        {
            if (!VCSPlugin.IsEnabled())
                return;

            VCSPlugin.Disable();

            mLog.DebugFormat("Disabled VCS Plugin on Project: {0}",
                projectPath);
        }

        static void DisposeNewIncomingChanges(PlasticWindow window)
        {
            NewIncomingChanges.DisposeUpdater(
                window.mDeveloperNewIncomingChangesUpdater,
                window.mGluonNewIncomingChangesUpdater,
                window.mDeveloperNewIncomingChangesFromMainUpdater);

            window.mDeveloperNewIncomingChangesUpdater = null;
            window.mGluonNewIncomingChangesUpdater = null;
            window.mDeveloperNewIncomingChangesFromMainUpdater = null;
        }

        static void RegisterApplicationFocusHandlers(PlasticWindow window)
        {
            EditorWindowFocus.OnApplicationActivated += window.OnApplicationActivated;
            EditorWindowFocus.OnApplicationDeactivated += window.OnApplicationDeactivated;
        }

        static void UnRegisterApplicationFocusHandlers(PlasticWindow window)
        {
            EditorWindowFocus.OnApplicationActivated -= window.OnApplicationActivated;
            EditorWindowFocus.OnApplicationDeactivated -= window.OnApplicationDeactivated;
        }

        static bool IsExitGUIException(Exception ex)
        {
            return ex is ExitGUIException;
        }

        static bool IsIMGUIPaintException(Exception ex)
        {
            if (!(ex is ArgumentException))
                return false;

            return ex.Message.StartsWith("Getting control") &&
                   ex.Message.Contains("controls when doing repaint");
        }

        static void ClosePlasticWindow(PlasticWindow window)
        {
            UnRegisterApplicationFocusHandlers(window);

            PlasticApp.Dispose();

            AssetMenuItems.Disable();

            DrawInspectorOperations.Disable();

            DrawAssetOverlay.Dispose();

            if (window.mEventSenderScheduler != null)
            {
                window.mPingEventLoop.Stop();
                window.mEventSenderScheduler.End();
            }

            DisposeNewIncomingChanges(window);

            AvatarImages.Dispose();
        }

        static void ShowPlasticWindow(PlasticWindow window)
        {
            EditorWindow dockWindow = FindEditorWindow.ToDock<PlasticWindow>();

            PlasticWindow newPlasticWindow = InstantiateFrom(window);

            if (DockEditorWindow.IsAvailable())
                DockEditorWindow.To(dockWindow, newPlasticWindow);

            newPlasticWindow.Show();

            newPlasticWindow.Focus();
        }

        static bool NeedsToDisplayWelcomeView(
            bool isPlasticExeAvailable,
            bool clientNeedsConfiguration,
            WorkspaceInfo wkInfo)
        {
            if (!isPlasticExeAvailable)
                return true;

            if (clientNeedsConfiguration)
                return true;

            if (wkInfo == null)
                return true;

            return false;
        }

        static void RepaintProjectWindow()
        {
            EditorWindow projectWindow = FindEditorWindow.ProjectWindow();

            if (projectWindow == null)
                return;

            projectWindow.Repaint();
        }

        static PlasticWindow InstantiateFrom(PlasticWindow window)
        {
            PlasticWindow result = Instantiate(window);
            result.mWkInfo = window.mWkInfo;
            result.mPlasticClient = window.mPlasticClient;
            result.mViewSwitcher = window.mViewSwitcher;
            result.mDeveloperNewIncomingChangesUpdater = window.mDeveloperNewIncomingChangesUpdater;
            result.mDeveloperNewIncomingChangesFromMainUpdater = window.mDeveloperNewIncomingChangesFromMainUpdater;
            result.mGluonNewIncomingChangesUpdater = window.mGluonNewIncomingChangesUpdater;
            
            result.mException = window.mException;
            result.mLastUpdateTime = window.mLastUpdateTime;
            result.mIsGluonMode = window.mIsGluonMode;
            result.mIncomingChangesNotificationPanel = window.mIncomingChangesNotificationPanel;
            result.mIncomingChangesFromMainNotificationPanel = window.mIncomingChangesFromMainNotificationPanel;
            result.mWelcomeView = window.mWelcomeView;
            result.mPlasticAPI = window.mPlasticAPI;
            result.mEventSenderRestApi = window.mEventSenderRestApi;
            result.mEventSenderScheduler = window.mEventSenderScheduler;
            result.mPingEventLoop = window.mPingEventLoop;
            return result;
        }

        static class Reload
        {
            internal static void IfWorkspaceConfigChanged(
                IPlasticAPI plasticApi,
                WorkspaceInfo lastWkInfo,
                bool lastIsGluonMode,
                Action reloadAction)
            {
                string applicationPath = Application.dataPath;

                bool isGluonMode = false;
                WorkspaceInfo wkInfo = null;

                IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
                waiter.Execute(
                    /*threadOperationDelegate*/ delegate
                    {
                        wkInfo = FindWorkspace.
                            InfoForApplicationPath(applicationPath, plasticApi);

                        if (wkInfo != null)
                            isGluonMode = plasticApi.IsGluonWorkspace(wkInfo);
                    },
                    /*afterOperationDelegate*/ delegate
                    {
                        if (waiter.Exception != null)
                            return;

                        if (!IsWorkspaceConfigChanged(
                                lastWkInfo, wkInfo,
                                lastIsGluonMode, isGluonMode))
                            return;

                        reloadAction();
                });
            }

            static bool IsWorkspaceConfigChanged(
                WorkspaceInfo lastWkInfo,
                WorkspaceInfo currentWkInfo,
                bool lastIsGluonMode,
                bool currentIsGluonMode)
            {
                if (lastIsGluonMode != currentIsGluonMode)
                    return true;

                if (lastWkInfo == null || currentWkInfo == null)
                    return true;

                return !lastWkInfo.Equals(currentWkInfo);
            }
        }

        [SerializeField]
        bool mForceToOpen;



        [NonSerialized]
        WorkspaceInfo mWkInfo;

        Exception mException;

        IIncomingChangesNotificationPanel mIncomingChangesNotificationPanel;
        IIncomingChangesNotificationPanel mIncomingChangesFromMainNotificationPanel;


        double mLastUpdateTime = 0f;

        ViewSwitcher mViewSwitcher;
        WelcomeView mWelcomeView;

        PlasticGui.WorkspaceWindow.NewIncomingChangesUpdater mDeveloperNewIncomingChangesUpdater;
        AutoSetup.NewIncomingChangesUpdater mDeveloperNewIncomingChangesFromMainUpdater;
        GluonNewIncomingChangesUpdater mGluonNewIncomingChangesUpdater;

        PlasticGUIClient mPlasticClient;

        bool mIsGluonMode;

        PlasticAPI mPlasticAPI;
        EventSender.IRestApi mEventSenderRestApi;
        EventSenderScheduler mEventSenderScheduler;
        PingEventLoop mPingEventLoop;
        
        static readonly ILog mLog = LogManager.GetLogger("PlasticWindow");
    }
}
 
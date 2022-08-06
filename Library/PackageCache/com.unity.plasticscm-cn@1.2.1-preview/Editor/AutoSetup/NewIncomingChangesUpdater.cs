using System;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.LogWrapper;
using Codice.Utils;
using PlasticGui.WorkspaceWindow;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.AutoSetup
{
    public class NewIncomingChangesUpdater: INewIncomingChangesUpdater
    {
        public NewIncomingChangesUpdater(
            IPlasticTimerBuilder timerBuilder,
            PlasticGui.WorkspaceWindow.CheckIncomingChanges.IUpdateIncomingChanges updateIncomingChanges)
        {
            mTimerBuilder = timerBuilder;
            mUpdateIncomingChanges = updateIncomingChanges;
        }
        public void SetWorkspace(WorkspaceInfo wkInfo)
        {
            lock (mLock)
            {
                mWkInfo = wkInfo;
            }

            mUpdateIncomingChanges.Hide();
            Update();
        }

        public void Start()
        {
            lock (mLock)
            {
                if (mTimer != null)
                    return;
                
                // every 1 minute
                mTimer = mTimerBuilder.Get(false, 60 * 1000, Update);
                mTimer.Start();
            }
        }

        public void Stop()
        {
            lock (mLock)
            {
                if (mTimer == null)
                    return;

                mTimer.Stop();
                mTimer = null;
            }
        }

        public void Dispose()
        {
            lock (mLock)
            {
                mCurrentCancelToken.Cancel();
            }

            Stop();
        }

        public void Update()
        {
            if (mIsDisabledOnce)
            {
                mIsDisabledOnce = false;
                return;
            }

            WorkspaceInfo currentWkInfo = null;
            CancelToken cancelToken = new CancelToken();

            lock (mLock)
            {
                currentWkInfo = mWkInfo;

                if (currentWkInfo == null)
                    return;

                mCurrentCancelToken.Cancel();
                mCurrentCancelToken = cancelToken;
            }

            try
            {
                CheckIncomingChanges.ForWorkspace(
                    currentWkInfo, cancelToken,
                    mUpdateIncomingChanges);
            }
            catch (Exception ex)
            {
                mLog.ErrorFormat(
                    "Error checking new incoming changes from main for workspace '{0}': {1} ",
                    currentWkInfo.Name , ex.Message);
                mLog.DebugFormat(
                    "Stack trace:{0}{1}",
                    Environment.NewLine, ex.StackTrace);
            }
        }
        
        public void DisableOnce()
        {
            mIsDisabledOnce = true;
        }
        
        bool mIsDisabledOnce = false;
        CancelToken mCurrentCancelToken = new CancelToken();

        WorkspaceInfo mWkInfo;
        IPlasticTimer mTimer;

        readonly PlasticGui.WorkspaceWindow.CheckIncomingChanges.IUpdateIncomingChanges mUpdateIncomingChanges;
        readonly IPlasticTimerBuilder mTimerBuilder;
        readonly object mLock = new object();

        static ILog mLog = LogManager.GetLogger("NewIncomingChangesFromMainUpdater");
    }
}
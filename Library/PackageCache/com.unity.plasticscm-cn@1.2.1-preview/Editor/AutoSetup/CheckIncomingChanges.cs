using System;
using Codice.Client.Common;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.Utils;
using PlasticGui;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.AutoSetup
{
    public class CheckIncomingChanges
    {
        public static void ForWorkspace(
            WorkspaceInfo wkInfo,
            CancelToken cancelToken,
            PlasticGui.WorkspaceWindow.CheckIncomingChanges.IUpdateIncomingChanges updateIncomingChanges)
        {
            ForWorkspace(
                wkInfo,
                cancelToken,
                new CalculateIncomingChanges(),
                updateIncomingChanges);
        }
        
         public static void ForWorkspace(
            WorkspaceInfo wkInfo,
            CancelToken cancelToken,
            PlasticGui.WorkspaceWindow.CheckIncomingChanges.ICalculateIncomingChanges calculateIncomingChanges,
            PlasticGui.WorkspaceWindow.CheckIncomingChanges.IUpdateIncomingChanges updateIncomingChanges)
        {
            bool areNewChangesAvailable = false;
            RepositoryInfo repInfo = null;
            BranchInfo workingBranchInfo = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    if (wkInfo.IsDynamic)
                    {
                        CmConnection.Get()
                            .GetWorkspaceHandler()
                            .WaitUntilDynamicWorkspaceIsMounted(
                                wkInfo, TimeSpan.FromSeconds(30));
                    }

                    workingBranchInfo = Plastic.API.GetWorkingBranch(wkInfo);

                    if (cancelToken.IsCancelled())
                        return;

                    if (workingBranchInfo == null)
                    {
                        return;
                    }

                    repInfo = Plastic.API.GetRootRepositoryInfo(wkInfo.ClientPath);

                    if (repInfo == null)
                        return;

                    if (cancelToken.IsCancelled())
                        return;
                    
                    areNewChangesAvailable = calculateIncomingChanges.AreNewChangesAvailable(
                       wkInfo, workingBranchInfo);
                },
                /*afterOperationDelegate*/ delegate
                {
                    if (cancelToken.IsCancelled())
                        return;

                    if (waiter.Exception != null)
                    {
                        updateIncomingChanges.Hide();
                        return;
                    }
                    
                    if (!areNewChangesAvailable)
                    {
                        updateIncomingChanges.Hide();
                        return;
                    }
                    
                    updateIncomingChanges.Show(
                        mInfoText,
                        mActionText,
                        mToolTips,
                        PlasticGui.WorkspaceWindow.CheckIncomingChanges.Severity.Info,
                        PlasticGui.WorkspaceWindow.CheckIncomingChanges.Action.ShowIncomingChanges);
                });
        }

         private const string mInfoText = "New incoming changes from main branch";
         private const string mActionText = "Merge";
         private const string mToolTips = "merge from main";
    }
}
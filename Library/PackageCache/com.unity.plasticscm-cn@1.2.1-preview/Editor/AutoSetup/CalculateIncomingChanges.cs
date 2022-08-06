using System.Collections;
using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow;

namespace Unity.PlasticSCM.Editor.AutoSetup
{
    public class CalculateIncomingChanges: PlasticGui.WorkspaceWindow.CheckIncomingChanges.ICalculateIncomingChanges
    {
        public bool AreNewChangesAvailable(WorkspaceInfo wkInfo, BranchInfo workingBranchInfo)
        {
            return CheckIncomingChanges(wkInfo, workingBranchInfo);
        }

        public int GetChangesetsCount(RepositoryInfo repInfo, BranchInfo workingBranchInfo, ChangesetInfo loadedChangeset)
        {
            throw new System.NotImplementedException();
        }

        public bool AreConflictsInvolved(WorkspaceInfo wkInfo, RepositoryInfo repInfo, BranchInfo workingBranchInfo)
        {
            throw new System.NotImplementedException();
        }
        
        bool CheckIncomingChanges(WorkspaceInfo mWkInfo, BranchInfo workingBranchInfo)
        {
            if (mWkInfo == null)
            {
                return false;
            }

            RepositorySpec repositorySpec = Plastic.API.GetRepositorySpec(mWkInfo);
            BranchInfo mainBranchInfo = Plastic.API.GetMainBranch(repositorySpec);
           
            if (workingBranchInfo.BranchId == mainBranchInfo.BranchId)
            {
                return false;
            }

            string mergesQueryString = "find merge where srcbranch = 'br:{0}' and dstbranch = 'br:{1}'";
            string mergesQuery = string.Format(mergesQueryString, mainBranchInfo.BranchName, workingBranchInfo.BranchName);
            string reverseMergesQuery = string.Format(mergesQueryString, workingBranchInfo.BranchName, mainBranchInfo.BranchName);
            
            IList mergesList = Plastic.API.FindQuery(mWkInfo, mergesQuery).Result[0];
            IList reverseMergesList = Plastic.API.FindQuery(mWkInfo, reverseMergesQuery).Result[0];

            MergeLinkRealizationInfo latestMergeInfo = null;
            if (mergesList.Count > 0)
            {
                latestMergeInfo = (MergeLinkRealizationInfo) mergesList[mergesList.Count - 1];
            }

            if (reverseMergesList.Count > 0)
            {
                MergeLinkRealizationInfo reverseLatestMergeInfo = (MergeLinkRealizationInfo) reverseMergesList[reverseMergesList.Count - 1];
                if (latestMergeInfo == null || reverseLatestMergeInfo.UtcTimeStamp > latestMergeInfo.UtcTimeStamp)
                {
                    latestMergeInfo = reverseLatestMergeInfo;
                }
            }

            if (latestMergeInfo != null)
            {
                ChangesetInfo mainBranchChangeset = latestMergeInfo.SourceObject as ChangesetInfo;

                if (mainBranchChangeset.BranchId != mainBranchInfo.BranchId)
                {
                    mainBranchChangeset = latestMergeInfo.DestinationObject as ChangesetInfo;
                }
                
                if (mainBranchChangeset.ChangesetId == mainBranchInfo.Changeset)
                {
                    return false;
                }

                return true;
            }

            string csetsQuery = string.Format("find changesets where changesetid = {0}", mainBranchInfo.Changeset);
            IList changesets = Plastic.API.FindQuery(mWkInfo, csetsQuery).Result[0];
            ChangesetInfo changesetInfo = (ChangesetInfo)changesets[0];
            
            if (changesetInfo.UtcTimeStamp > workingBranchInfo.UtcTimeStamp)
            {
                return true;
            }
            
            return false;
        }
    }
}
using System;
using System.Collections;
using System.IO;
using Codice.Client.BaseCommands.Merge;
using Codice.Client.Commands;
using Codice.Client.Common;
using Codice.CM.Common;
using PlasticGui;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.UI;
using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.AutoSetup
{
    class AutoCommitOperation
    {
        public static void AutoCommit()
        {
            if (EditorApplication.isCompiling)
            {
                return;
            }

            if (EditorApplication.isPlaying)
            {
                return;
            }
            
            if (!EditorPrefs.GetBool(mAutoCommitKey, true))
            {
                return;
            }
            
            double currentUpdateTime = EditorApplication.timeSinceStartup;
            double elapsedSeconds = currentUpdateTime - mLastAutoCommitTime;
            double fixedTimeDuration = mFixedAutoCommitTime;
            string plasticIniFilePah = Path.Combine(Application.dataPath, "../plastic.ini");

            Action checkinEndOperation = null;
                
            if (File.Exists(plasticIniFilePah))
            {
                fixedTimeDuration = mFixedInitAutoCommitTime;
                checkinEndOperation = MergeToMain;
            }
            
            if (elapsedSeconds < fixedTimeDuration)
            {
                return;
            }

            mLastAutoCommitTime = currentUpdateTime;

            mPlasticClient?.FullCheckIn(checkinEndOperation);
        }

        static void MergeToMain()
        {
            try
            {
                WorkspaceInfo mWkInfo = FindWorkspace.InfoForApplicationPath(Application.dataPath, Plastic.API);
            
                if (mWkInfo == null)
                {
                    return;
                }
            
                RepositorySpec repSpec = Plastic.API.GetRepositorySpec(mWkInfo);
                RepositoryInfo repInfo = Plastic.API.GetRepositoryInfo(repSpec);
            
                BranchInfo workingBranchInfo = Plastic.API.GetWorkingBranch(mWkInfo);
                BranchInfo mainBranchInfo = Plastic.API.GetMainBranch(repSpec);
                
                if (workingBranchInfo.BranchId == mainBranchInfo.BranchId)
                {
                    return;
                }
            
                SpecGenerator specGenerator = new SpecGenerator(repInfo);
                BranchSpec sourceSpec = specGenerator.GenBranchSpec(false, workingBranchInfo.BranchName);
                BranchSpec destinationSpec = specGenerator.GenBranchSpec(false, mainBranchInfo.BranchName);
            
                MergeSource mergeSource = MergeSourceBuilder.BuildMergeSource(repInfo,
                    null, sourceSpec, destinationSpec, new MergeParameters());
                BuildMerge.ApplyMergeForMerge(mWkInfo, null).MergeTo(mergeSource, mMergeComments);
            }
            finally
            {
                string plasticIniFilePah = Path.Combine(Application.dataPath, "../plastic.ini");
                File.Delete(plasticIniFilePah);
            }
        }

        public static void DoAutoCommitCheckBox()
        {
            if (!EditorPrefs.HasKey(mAutoCommitKey))
            {
                EditorPrefs.SetBool(mAutoCommitKey, true);
            }
            
            var layoutOptions = GUILayout.Height(18);
            bool autoCommit = EditorPrefs.GetBool(mAutoCommitKey);           
            bool newAutocommit = GUILayout.Toggle(autoCommit, new GUIContent("", mToolTip), layoutOptions);
            
            GUILayout.Label(new GUIContent(mCheckboxLabel, mToolTip), layoutOptions);
            
            EditorPrefs.SetBool(mAutoCommitKey, newAutocommit);
        }

        public static void SetPlasticGUIClient(PlasticGUIClient plasticGUIClient)
        {
            mPlasticClient = plasticGUIClient;
        }

        static double mLastAutoCommitTime = 0f;
        const double mFixedAutoCommitTime = 300;
        const double mFixedInitAutoCommitTime = 5;
        const string mAutoCommitKey = "auto_commit";
        const string mMergeComments = "Merge to main";
        const string mCheckboxLabel = "Auto Save";
        const string mToolTip = "Auto save every five minutes";
        static PlasticGUIClient mPlasticClient;
    }
    
}
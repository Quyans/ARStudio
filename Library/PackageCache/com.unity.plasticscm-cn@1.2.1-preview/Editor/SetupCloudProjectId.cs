using System;
using System.Reflection;

using UnityEditor;
using UnityEngine;

using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.LogWrapper;
using PlasticGui;

namespace Unity.PlasticSCM.Editor
{
    static class SetupCloudProjectId
    {
        internal static void ForWorkspace(
            WorkspaceInfo wkInfo,
            IPlasticAPI plasticApi)
        {
            if (HasCloudProjectId())
                return;

            string repGuid = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    RepositorySpec repSpec = plasticApi.GetRepositorySpec(wkInfo);
                    RepositoryInfo repInfo = plasticApi.GetRepositoryInfo(repSpec);

                    repGuid = repInfo.GUID.ToString();
                },
                /*afterOperationDelegate*/ delegate
                {
                    if (waiter.Exception != null)
                    {
                        ExceptionsHandler.LogException(
                            "SetupCloudProjectId",
                            waiter.Exception);
                    }

                    ConfigureCloudProjectId(repGuid);
                });
        }

        static bool HasCloudProjectId()
        {
            //disable Warning CS0618  'PlayerSettings.cloudProjectId' is obsolete: 'cloudProjectId is deprecated
#pragma warning disable 0618
            return !string.IsNullOrEmpty(PlayerSettings.cloudProjectId);
        }

        static void ConfigureCloudProjectId(string projectId)
        {
            // Invokes PlayerSettings.SetCloudProjectId(projectId)
            SetCloudProjectId(projectId);

            AssetDatabase.SaveAssets();
        }

        static void SetCloudProjectId(string projectId)
        {
            MethodInfo InternalSetCloudProjectId = PlayerSettingsType.GetMethod(
                "SetCloudProjectId",
                BindingFlags.NonPublic | BindingFlags.Static);

            if (InternalSetCloudProjectId == null)
            {
                Debug.LogWarning(
                    "Cannot write cloudProjectId: " + 
                    "Method PlayerSettings.SetCloudProjectId not found");
                return;
            }

            InternalSetCloudProjectId.Invoke(
                null, new object[] { projectId });
        }

        static readonly Type PlayerSettingsType =
            typeof(UnityEditor.PlayerSettings);

        static readonly ILog mLog = LogManager.GetLogger("SetupCloudProjectId");
    }
}

using Unity.PlasticSCM.Editor.UI;
using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.AutoSetup
{
    [InitializeOnLoad]
    internal class PlasticWindowAutoOpen
    {
        static PlasticWindowAutoOpen()
        {
            EditorApplication.update += RunOnce;
        }

        static void RunOnce()
        {
            EditorApplication.update -= RunOnce;
            Execute();
        }
        
        static void Execute()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return;
            }

            ShowWindow.Plastic(false);
        }
    }
}
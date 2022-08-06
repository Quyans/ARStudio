using System.IO;
using UnityEditor;

namespace Unity.PlasticSCM.Editor.AssetUtils
{
    internal static class AssetPath
    {
        internal static string GetFullPath(UnityEngine.Object obj)
        {
            string relativePath = AssetDatabase.GetAssetPath(obj);

            if (string.IsNullOrEmpty(relativePath))
                return null;

            return Path.GetFullPath(relativePath);
        }
    }
}

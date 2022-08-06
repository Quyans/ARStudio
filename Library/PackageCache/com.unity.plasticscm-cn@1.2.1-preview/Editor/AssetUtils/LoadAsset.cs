using System;
using System.IO;

using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.Common;

namespace Unity.PlasticSCM.Editor.AssetUtils
{
    internal static class LoadAsset
    {
        internal static UnityEngine.Object FromChangeInfo(ChangeInfo changeInfo)
        {
            string changeFullPath = changeInfo.GetFullPath();

            if (MetaPath.IsMetaPath(changeFullPath))
                changeFullPath = MetaPath.GetPathFromMetaPath(changeFullPath);

            return FromFullPath(changeFullPath);
        }

        static UnityEngine.Object FromFullPath(string fullPath)
        {
            if (!IsProjectPath(fullPath))
                return null;

            string assetPath = PathHelper.GetRelativePath(
                mProjectFullPath, fullPath).Substring(1);

            return AssetDatabase.LoadMainAssetAtPath(assetPath);
        }

        static bool IsProjectPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            var fullPath = Path.GetFullPath(path).Replace('\\', '/');

            return fullPath.StartsWith(
                mProjectRelativePath,
                StringComparison.OrdinalIgnoreCase);
        }

        static string mProjectRelativePath = Directory.GetCurrentDirectory().Replace('\\', '/') + '/';
        static string mProjectFullPath = Path.GetDirectoryName(Path.GetFullPath(Application.dataPath));
    }
}

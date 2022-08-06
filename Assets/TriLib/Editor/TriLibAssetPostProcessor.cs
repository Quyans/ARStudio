using UnityEditor;
using UnityEngine;
using System.IO;
using TriLib;

namespace TriLibEditor
{
    public class TriLibAssetPostProcessor : AssetPostprocessor
    {
        private static readonly string[] UnityExtensions = { ".fbx", ".dae", ".3ds", ".dxf", ".obj", ".skp", ".ma", ".mb", ".max", ".c4d", ".blend", ".bmp", ".xml", ".raw" };

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!TriLibCheckPlugins.PluginsLoaded)
            {
                return;
            }
            foreach (var str in importedAssets)
            {
                CheckForAssimpAsset(str);
            }
            foreach (var str in movedAssets)
            {
                CheckForAssimpAsset(str);
            }
        }

        private static void CheckForAssimpAsset(string str)
        {
            if (!TriLibCheckPlugins.PluginsLoaded)
            {
                return;
            }
            var extension = Path.GetExtension(str);
            if (extension == null)
            {
                return;
            }
            foreach (var unityExtension in UnityExtensions)
            {
                if (unityExtension == extension.ToLower())
                {
                    return;
                }
            }
            if (AssimpInterop.ai_IsExtensionSupported(extension))
            {
                TriLibAssetImporter.Import(str);
                #if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                Debug.Log("Asset imported: " + str);
                #endif
            }
        }
    }
}
using System.IO;
using UnityEngine;
using UnityEditor;
using TriLib;

namespace TriLibEditor
{
    public static class TriLibAssetImporter
    {
        public static void Import(string assetPath)
        {
            var assimpLoaderOptions = AssetLoaderOptions.CreateInstance();
            var assetImporter = AssetImporter.GetAtPath(assetPath);
            var userData = assetImporter.userData;
            if (!string.IsNullOrEmpty(userData))
            {
                assimpLoaderOptions.Deserialize(userData);
            }
            var folderPath = Path.GetDirectoryName(assetPath);
            var filename = Path.GetFileName(assetPath);
            var filePath = folderPath + "/" + filename;
            var prefabPath = filePath + ".prefab";
            using (var assetLoader = new AssetLoader())
            {
                assetLoader.OnMeshCreated +=
                    (meshIndex, mesh) => ReplaceOldAsset(mesh, prefabPath);
                assetLoader.OnMaterialCreated +=
                    (materialIndex, isOverriden, material) => ReplaceOldAsset(material, prefabPath);
                assetLoader.OnTextureLoaded +=
                    (sourcePath, material, propertyName, texture) => ReplaceOldAsset(texture, prefabPath);
                assetLoader.OnAnimationClipCreated +=
                    (animationClipIndex, animationClip) => ReplaceOldAsset(animationClip, prefabPath);
                assetLoader.OnObjectLoaded += delegate(GameObject loadedGameObject)
                {
                    var existingPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                    if (existingPrefab == null)
                    {
                        existingPrefab = PrefabUtility.CreatePrefab(prefabPath, loadedGameObject);
                    }
                    else
                    {
                        existingPrefab = PrefabUtility.ReplacePrefab(loadedGameObject, existingPrefab,
                            ReplacePrefabOptions.ReplaceNameBased);
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Object.DestroyImmediate(loadedGameObject);
                    var activeEditor = TriLibAssetEditor.Active;
                    if (activeEditor != null && activeEditor.AssetPath == assetPath)
                    {
                        activeEditor.OnPrefabCreated((GameObject) existingPrefab);
                    }
                };
                assetLoader.LoadFromFile(assetPath, assimpLoaderOptions);
            }
        }
        private static void ReplaceOldAsset(Object asset, string prefabPath)
        {
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(prefabPath);
            foreach (var subAsset in subAssets)
            {
                if (subAsset.name == asset.name && asset.GetType() == subAsset.GetType())
                {
                    Object.DestroyImmediate(subAsset, true);
                }
            }
            AssetDatabase.AddObjectToAsset(asset, prefabPath);
        }
    }
}
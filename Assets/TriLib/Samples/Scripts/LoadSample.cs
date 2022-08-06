using System;
using UnityEngine;

namespace TriLib
{
    namespace Samples
    {
        /// <summary>
        /// Represents a simple model loading sample.
        /// </summary>
        public class LoadSample : MonoBehaviour
        {
#if !UNITY_WINRT
            /// <summary>
            /// Tries to load "Bouncing.fbx" model
            /// </summary>
            protected void Start()
            {
                using (var assetLoader = new AssetLoader())
                {
                    try
                    {
                        var assetLoaderOptions = AssetLoaderOptions.CreateInstance();
                        assetLoaderOptions.RotationAngles = new Vector3(90f, 180f, 0f);
                        assetLoaderOptions.AutoPlayAnimations = true;
                        var loadedGameObject = assetLoader.LoadFromFile(Application.dataPath + "/TriLib/TriLib/Samples/Models/Bouncing.fbx", assetLoaderOptions);
                        loadedGameObject.transform.position = new Vector3(128f, 0f, 0f);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }
            }
#endif
        }
    }
}

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace TriLib
{
    /// <summary>
    /// Represents an asset unloader.
    /// This class is added to your loaded GameObject if your <see cref="AssetLoaderOptions.AddAssetUnloader"/> flag is set to true.
    /// This class destroy every asset (textures, materials, meshes) used by your GameObjects if they are not being used anymore.
    /// </summary>
    public class AssetUnloader : MonoBehaviour
    {
        /// <inheritdoc/>
        protected virtual void OnDestroy()
        {
            if (Application.isPlaying)
            {
                Resources.UnloadUnusedAssets();
            }
            #if UNITY_EDITOR
            else
            {
                EditorUtility.UnloadUnusedAssetsImmediate(); 
            }
            #endif
        }
    }
}

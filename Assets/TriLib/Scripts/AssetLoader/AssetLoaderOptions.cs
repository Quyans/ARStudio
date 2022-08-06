using UnityEngine;
using System;
using System.Collections.Generic;

namespace TriLib
{   
    /// <summary>
    /// Represents a series of asset loading options.
    /// </summary>
    [Serializable]
    public class AssetLoaderOptions : ScriptableObject
    { 
        /// <summary>
        /// Returns a new AssetLoaderOptions instance.
        /// </summary>
        /// <returns>The instance.</returns>
        public static AssetLoaderOptions CreateInstance()
        {
            return CreateInstance<AssetLoaderOptions>();
        }

        /// <summary>
        /// Turn on this field to add <see cref="AssetUnloader"/> behaviour to loaded <see cref="UnityEngine.GameObject"/> and automatically free resources when it's destroyed.
        /// </summary>
        public bool AddAssetUnloader;

        /// <summary>                              
        /// Turn on this field to disable animations loading.
        /// </summary>                              
        public bool DontLoadAnimations;

        /// <summary>
        /// Turn on this field to disable applying loaded animations.
        /// </summary>
        public bool DontApplyAnimations;

        /// <summary>                              
        /// Turn on this field to disable lights loading.
        /// </summary>                              
        public bool DontLoadLights = true;

        /// <summary>                              
        /// Turn on this field to disable cameras loading.
        /// </summary>                              
        public bool DontLoadCameras = true;

        /// <summary>
        /// Turn on this field to automatically play the first loaded animation.
        /// @note Only for legacy animations.
        /// </summary>
        public bool AutoPlayAnimations = true;

        /// <summary>
        /// Use this field to change default animations wrap mode.
        /// </summary>
        public WrapMode AnimationWrapMode = WrapMode.Loop;    

        /// <summary>
        /// Turn on this field to use legacy <see cref="UnityEngine.Animation"/> component.
        /// </summary>
        public bool UseLegacyAnimations = true;
        
        /// <summary>
        /// Turn on this field to realigns quaternion keys to ensure shortest interpolation paths.
        /// </summary>
        /// <remarks>This can cause glitches on some animations.</remarks>
        public bool EnsureQuaternionContinuity = true;

        /// <summary>
        /// If you don´t wish to use legacy animations, use this field to specify a <see cref=" UnityEngine.RuntimeAnimatorController"/>.
        /// </summary>
        public RuntimeAnimatorController AnimatorController;

        /// <summary>
        /// If you don´t wish to use legacy animations, use this field to specify an existing <see cref=" UnityEngine.Avatar"/> for using.
        /// </summary>
        public Avatar Avatar;

        /// <summary>
        /// Turn on this field if you gonna generate your own <see cref="UnityEngine.Avatar"/> later.
        /// </summary>
        public bool DontGenerateAvatar;

        /// <summary>
        /// Turn on this field to disable metadata loading.
        /// </summary>
        public bool DontLoadMetadata;

        /// <summary>
        /// Turn on this field to disable materials loading.
        /// </summary>                             
        public bool DontLoadMaterials;

		/// <summary>
		/// Turn on this field to automatically scan and apply alpha channel materials per pixel (may decrease performance).
		/// </summary>
		public bool ApplyAlphaMaterials;

        /// <summary>
        /// Turn on this field to disable transparent materials creation.
        /// </summary>
        public bool DisableAlphaMaterials;

        /// <summary>
        /// Turn on this field to use cutout materials instead of alpha-blended materials.
        /// </summary>
        public bool UseCutoutMaterials = true;

        /// <summary>
        /// Turn on this field to use the Unity default specular material.
        /// </summary>
        public bool UseStandardSpecularMaterial = false;

        /// <summary>
        /// Turn on this field to disable meshes loading.
        /// </summary>   
        public bool DontLoadMeshes;

        /// <summary>
        /// Turn on this field to combine loaded meshes.
        /// </summary>
        public bool CombineMeshes = true;

#if UNITY_2017_3_OR_NEWER
/// <summary>
/// Turn ON this field to use 32 bits mesh vertex index format.
/// </summary>
		public bool Use32BitsIndexFormat = true;
#endif

        /// <summary>
        /// Turn on this field to enable mesh collider generation 
        /// @note Only for non-skinned mesh renderers.
        /// </summary>
        public bool GenerateMeshColliders;

        /// <summary>
        /// Turn on this field to indicate that generated mesh collider will be convex.
        /// </summary>
        public bool ConvexMeshColliders;

        /// <summary>
        /// Use this field to override materials with your own.
        /// If this array is not empty, each mesh material will be replaced by the material with the same index from this array.
        /// </summary>
        [Obsolete("MaterialsOverride is not used anymore. If you want to change loaded materials source, override LoadMaterial method from AssetLoaderBase.")]
        public List<Material> MaterialsOverride = new List<Material>();

        /// <summary>
        /// Use this field to override object rotation angles.
        /// </summary>
        public Vector3 RotationAngles = new Vector3(0f, 180f,0f);

        /// <summary>
        /// Use this field to override object scale.
        /// </summary>
        public float Scale = 1f;

        /// <summary>
        /// Use this field to set-up advanced object loading options. <see cref="AssimpPostProcessSteps"/>
        /// </summary>
        public AssimpPostProcessSteps PostProcessSteps = AssimpPostProcessSteps.FlipWindingOrder | AssimpPostProcessSteps.MakeLeftHanded | AssimpProcessPreset.TargetRealtimeMaxQuality;

        /// <summary>
        /// Use this field to override the object textures searching path.
        /// </summary>
        [Obsolete("TexturesPathOverride is not used anymore. If you want to change loaded textures path, override LoadTextureFromFile method from AssetLoaderBase.")]
        public string TexturesPathOverride;

        /// <summary>
        /// Use this field to set loaded textures compression level.
        /// </summary>
        public TextureCompression TextureCompression = TextureCompression.NormalQuality;

        /// <summary>
        /// Enable this field to generate textures mipmaps.
        /// </summary>
        public bool GenerateMipMaps = true;

        /// <summary>
        /// Use this field to define asset loading advanced configs.
        /// </summary>
        public List<AssetAdvancedConfig> AdvancedConfigs = new List<AssetAdvancedConfig>
        {
            AssetAdvancedConfig.CreateConfig(AssetAdvancedPropertyClassNames.SplitLargeMeshesVertexLimit, 65000),
            AssetAdvancedConfig.CreateConfig(AssetAdvancedPropertyClassNames.FBXImportReadLights, false),
            AssetAdvancedConfig.CreateConfig(AssetAdvancedPropertyClassNames.FBXImportReadCameras, false)
        };
        
        /// @private
        /// <summary>
        /// Deserialize the specified JSON representation into this class.
        /// </summary>
        /// <param name="json">Json.</param>
        public void Deserialize(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

        /// @private
        /// <summary>
        /// Serializes this instance to a JSON representation.
        /// </summary>
        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }
    }  
}

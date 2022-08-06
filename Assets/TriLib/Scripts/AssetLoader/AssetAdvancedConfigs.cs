using System;

namespace TriLib
{
    /// <summary>
    /// Represents an asset advanced config type.
    /// </summary>
    public enum AssetAdvancedConfigType
    {
        /// <summary>
        /// Not a valid config type.
        /// </summary>
        None,

        /// <summary>
        /// Boolean config type.
        /// </summary>
        Bool,

        /// <summary>
        /// Integer config type.
        /// </summary>
        Integer,

        /// <summary>
        /// Float config type.
        /// </summary>
        Float,

        /// <summary>
        /// String config type.
        /// </summary>
        String,

        /// <summary>
        /// AiComponent config type.
        /// </summary>
        AiComponent,

        /// <summary>
        /// AiPrimitiveType config type.
        /// </summary>
        AiPrimitiveType,

        /// <summary>
        /// AiUVTransform config type.
        /// </summary>
        AiUVTransform,

		/// <summary>
		/// AiMatrix config type.
		/// </summary>
		AiMatrix
    }

    /// <summary>
    /// Represents a primitive type flags.
    /// </summary>
    [Flags]
    public enum AiPrimitiveType
    {
        /// <summary>
        /// Point primitive type.
        /// </summary>
        Point = 0x1,

        /// <summary>
        /// Line primitive type.
        /// </summary>
        Line = 0x2,

        /// <summary>
        /// Triangle primitive type.
        /// </summary>
        Triangle = 0x4,

        /// <summary>
        /// Polygon primitive type.
        /// </summary>
        Polygon = 0x8
    }

    /// <summary>
    /// Represents a component flags.
    /// </summary>
    [Flags]
    public enum AiComponent
    {
        /// <summary>
        /// Normal components.
        /// </summary>
        Normals = 0x2,

        /// <summary>
        /// Tangents and Bitangents components.
        /// </summary>
        TangentsAndBitangents = 0x4,

        /// <summary>
        /// Colors components.
        /// </summary>
        Colors = 0x8,

        /// <summary>
        /// Texture coordinates components.
        /// </summary>
        TexCoords = 0x10,

        /// <summary>
        /// Bone weights components.
        /// </summary>
        BoneWeights = 0x20,

        /// <summary>
        /// Animations components.
        /// </summary>
        Animations = 0x40,

        /// <summary>
        /// Texture components.
        /// </summary>
        Textures = 0x80,

        /// <summary>
        /// Light components.
        /// </summary>
        Lights = 0x100,

        /// <summary>
        /// Camera components.
        /// </summary>
        Cameras = 0x200,

        /// <summary>
        /// Mesh components.
        /// </summary>
        Meshes = 0x400,

        /// <summary>
        /// Material components.
        /// </summary>
        Materials = 0x800
    }

    /// <summary>
    /// Represents an UV transform flags.
    /// </summary>
    [Flags]
    public enum AiUVTransform
    {
        /// <summary>
        /// Apply rotation.
        /// </summary>
        Rotation = 0x2,

        /// <summary>
        /// Apply scaling.
        /// </summary>
        Scaling = 0x1,

        /// <summary>
        /// Apply translation.
        /// </summary>
        Translation = 0x4
    }

    /// <summary>
    /// Represents the asset advanced configs class names.
    /// </summary>
    public enum AssetAdvancedPropertyClassNames
    {
        /// <summary>
        /// Configures whether the AC loader evaluates subdivision surfaces (indicated by the presence
        /// of the 'subdiv' attribute in the file). By default, TriLib performs
        /// the subdivision using the standard Catmull-Clark algorithm.
        /// </summary>
        ACImportEvalSubdivision,

        /// <summary>
        /// Configures the AC loader to collect all surfaces which have the "Backface cull" flag set in separate
        /// meshes.
        /// </summary>
        ACImportSeparateBackfaceCull,

        /// <summary>
        /// Configures the ASE loader to always reconstruct normal vectors basing on the smoothing groups
        /// loaded from the file. Some ASE files carry invalid normals, others don't.
        /// </summary>
        ASEImportReconstructNormals,

        /// <summary>
        /// Specifies the maximum angle that may be between two face normals at the same vertex position that
        /// their normals will be smoothed together during the calculate smooth normals step. This is commonly
        /// called the "crease angle". The angle is specified in degrees.
        /// </summary>
        CalculateNormalsMaxSmoothingAngle,

        /// <summary>
        /// Specifies the maximum angle that may be between two vertex tangents that their tangents
        /// and bitangents are smoothed during the step to calculate the tangent basis. The angle specified 
        /// is in degrees.
        /// </summary>
        CalculateTangentsMaxSmoothingAngle,

        /// <summary>
        /// Source UV channel for tangent space computation. The specified channel must exist or an error will be raised.
        /// </summary>
        CalculateTangentsTextureChannelIndex,

        /// <summary>
        /// Specifies whether the collada loader will ignore the up direction.
        /// </summary>
        ColladaImportIgnoreUpDirection,

        /// <summary>
        /// Threshold used to determine if a bone is kept or removed during the <see cref="TriLib.AssimpPostProcessSteps.Debone"/> step.
        /// </summary>
        DeboneThreshold,

        /// <summary>
        /// Require all bones to qualify for de-boning before any are removed.
        /// </summary>
        DeboneAllOrNone,

        /// <summary>
        /// Specifies whether the FBX importer will read all materials present in the source file or take only the referenced materials, if the importer
        /// will read materials, otherwise this has no effect.
        /// </summary>
        FBXImportReadAllMaterials,

        /// <summary>
        /// Specifies whether the importer will drop empty animation curves or animation curves which match the bind pose
        /// </summary>
        FBXImportOptimizeEmptyAnimationCurves,

        /// <summary>
		/// Specifies whether the FBX importer will use legacy embedded texture naming
        /// </summary>
		FBXImportEmbeddedTextureLegacyNaming,

        /// <summary>
        /// Specifies whether the FBX importer will act in strict mode in which only the FBX 2013<br/>format 
        /// is supported and any other sub formats are rejected. FBX 2013 is the primary target for the importer, 
        /// so this<br/>format is best supported and well-tested.
        /// </summary>
        FBXImportStrictMode,

        /// <summary>
        /// Specifies whether the FBX importer will read light sources.
        /// </summary>
        FBXImportReadLights,

        /// <summary>
        /// Specifies whether the FBX importer will read materials.
        /// </summary>
        FBXImportReadMaterials,

        /// <summary>
        /// Specifies whether the FBX importer will preserve pivot points for transformations (as extra nodes). If set to false, pivots
        /// and offsets will be evaluated whenever possible.
        /// </summary>
        FBXImportPreservePivots,

        /// <summary>
        /// Specifies whether the FBX importer will merge all geometry layers present in the source file or take only the first.
        /// </summary>
        FBXImportReadAllGeometryLayers,

        /// <summary>
        /// Specifies whether the FBX importer will read embedded textures.
        /// </summary>
        FBXImportReadTextures,

        /// <summary>
        /// Specifies whether the FBX importer will read cameras.
        /// </summary>
        FBXImportReadCameras,

        /// <summary>
        /// Specifies whether the FBX importer will read animations.
        /// </summary>
        FBXImportReadAnimations,

        /// <summary>
        /// Configures the <see cref="TriLib.AssimpPostProcessSteps.FindDegenerates"/> step
        /// to remove degenerated primitives from the import immediately.
        /// <para>The default behavior converts degenerated triangles to lines and
        /// degenerated lines to points.</para>
        /// </summary>
        FindDegeneratesRemove,

        /// <summary>
        /// Input parameter to the <see cref="TriLib.AssimpPostProcessSteps.FindInvalidData"/> step.
        /// It specifies the floating point accuracy for animation values, specifically the epsilon
        /// during the comparison. The step checks for animation tracks where all frame values are absolutely equal
        /// and removes them. Two floats are considered equal if the invariant <c>abs(n0-n1) > epislon</c> holds
        /// true for all vector/quaternion components.
        /// </summary>
        FindInvalidDataAnimAccuracy,

        /// <summary>
        /// Specifies whether the IFC loader skips over shape representations of type 'Curve2D'. A lot of files contain
        /// both a faceted mesh representation and a outline with a presentation type of 'Curve2D'. Currently TriLib 
        /// does not convert those, so turning this option off just clutters the log with errors.
        /// </summary>
        IFCImportSkipSpaceRepresentations,

        /// <summary>
        /// This is used by the IFC importer to determine the tessellation parameter
        /// for cylindrical shapes, i.e. the number of segments used to approximate a circle.
        /// </summary>
        IFCImportCylindricalTesselation,

        /// <summary>
        /// This is used by the IFC importer to determine the tessellation parameter
        /// for smoothing curves.
        /// </summary>
        IFCImportSmoothingAngle,

        /// <summary>
        /// Specifies whether the IFC loader will use its own, custom triangulation algorithm to triangulate wall and floor
        /// meshes. If this is set to false, walls will be either triangulated by the post process triangulation or will be 
        /// passed through as huge polygons with faked holes (e.g. holes that are connected with the outer boundary using a 
        /// dummy edge). It is highly recommended to leave this property set to true as the default post process has some known
        /// issues with these kind of polygons.
        /// </summary>
        IFCImportCustomTriangulation,

        /// <summary>
        /// Global setting to disable generation of skeleton dummy meshes. These are generated as a visualization aid
        /// in cases which the input data contains no geometry, but only animation data. So the geometry are visualizing
        /// the bones.
        /// </summary>
        ImportNoSkeletonMeshes,

        /// <summary>
        /// A hint to TriLib to favour speed against import quality. Enabling this option
        /// may result in faster loading, or it may not. It is just a hint to loaders and post-processing
        /// steps to use faster code paths if possible. A value not equal to zero stands
        /// for true.
        /// </summary>
        ImportFavourSpeed,

        /// <summary>
        /// Sets the vertex animation keyframe to be imported. TriLib does not support vertex animation.
        /// </summary>
        ImportGlobalKeyframe,

        /// <summary>
        /// Sets the size of the post-transform vertex cache to optimize vertices for. This is
        /// for the <see cref="TriLib.AssimpPostProcessSteps.ImproveCacheLocality"/> step. The size
        /// is given in vertices. Of course you can't know how the vertex format will exactly look
        /// like after the import returns, but you can still guess what your meshes will
        /// probably have. The default value *has* resulted in slight performance improvements
        /// for most Nvidia/AMD cards since 2002.
        /// </summary>
        ImproveCacheLocalityPostTransformCacheSize,

        /// <summary>
        /// Defines the output frame rate of the IRR loader.
        /// <para>IRR animations are difficult to convert for TriLib and there will always be
        /// a loss of quality. This setting defines how many keys per second are returned by the converter.</para>
        /// </summary>
        IRRImportAnimFPS,

        /// <summary>
        /// Sets the maximum number of bones that can affect a single vertex. This is used
        /// by the <see cref="TriLib.AssimpPostProcessSteps.LimitBoneWeights"/> step.
        /// </summary>
        LimitBoneWeightsMaxWeights,

        /// <summary>
        /// Configures the LWO loader to load just one layer from the model.
        /// <para>LWO files consist of layers and in some cases it could be useful to load only one of them.
        /// This property can be either a string - which specifies the name of the layer - or an integer - the index
        /// of the layer. If the property is not set then the whole LWO model is loaded. Loading fails
        /// if the requested layer is not available. The layer index is zero-based and the layer name may not be empty</para>
        /// </summary>
        LWOImportOneLayerOnly,

        /// <summary>
        /// Defines the beginning of the time range for which the LWS loader evaluates animations and computes
        /// AiNodeAnim's.<para>TriLib provides full conversion of Lightwave's envelope system, including pre and post
        /// conditions. The loader computes linearly subsampled animation channels with the frame rate
        /// given in the LWS file. This property defines the start time.</para>
        /// <para>Animation channels are only generated if a node has at least one envelope with more than one key
        /// assigned. This property is given in frames where '0' is the first. By default,
        /// if this property is not set, the importer takes the animation start from the input LWS
        /// file ('FirstFrame' line)</para>
        /// </summary>
        LWSImportAnimStart,

        /// <summary>
        /// Defines the ending of the time range for which the LWS loader evaluates animations and computes
        /// AiNodeAnim's.
        /// <para>TriLib provides full conversion of Lightwave's envelope system, including pre and post
        /// conditions. The loader computes linearly subsampled animation channels with the frame rate
        /// given in the LWS file. This property defines the end time.</para>
        /// <para>Animation channels are only generated if a node has at least one envelope with more than one key
        /// assigned. This property is given in frames where '0' is the first. By default,
        /// if this property is not set, the importer takes the animation end from the input LWS
        /// file.</para>
        /// </summary>
        LWSImportAnimEnd,

        /// <summary>
        /// Sets the vertex animation keyframe to be imported. TriLib does not support vertex animation.
        /// </summary>
        MD2ImportKeyframe,

        /// <summary>
        /// Specifies the Quake 3 shader file to be used for a particular MD3 file. This can be a full path or
        /// relative to where all MD3 shaders reside.
        /// </summary>
        MD3ImportShaderSource,

        /// <summary>
        /// Sets the vertex animation keyframe to be imported. TriLib does not support vertex animation.
        /// </summary>
        MD3ImportKeyframe,

        /// <summary>
        /// Tells the MD3 loader which skin files to load. When loading MD3 files, TriLib checks
        /// whether a file named "md3_file_name"_"skin_name".skin exists. These files are used by
        /// Quake III to be able to assign different skins (e.g. red and blue team) to models. 'default', 'red', 'blue'
        /// are typical skin names.
        /// </summary>
        MD3ImportSkinName,

        /// <summary>
        /// Configures the MD3 loader to detect and process multi-part Quake player models. These models
        /// usually consit of three files, lower.md3, upper.md3 and head.md3. If this propery is
        /// set to true, TriLib will try to load and combine all three files if one of them is loaded.
        /// </summary>
        MD3ImportHandleMultiPart,

        /// <summary>
        /// Configures the MD5 loader to not load the MD5ANIM file for a MD5MESH file automatically.
        /// <para>The default strategy is to look for a file with the same name but with the MD5ANIm extension
        /// in the same directory. If it is found it is loaded and combined with the MD5MESH file. This configuration
        /// option can be used to disable this behavior.</para>
        /// </summary>
        MD5ImportNoAnimAutoLoad,

        /// <summary>
        /// Sets the vertex animation key-frame to be imported. TriLib does not support vertex animation.
        /// </summary>
        MDCImportKeyframe,

        /// <summary>
        /// Sets the vertex animation key-frame to be imported. TriLib does not support vertex animation.
        /// </summary>
        MDLmportKeyframe,

        /// <summary>
        /// Sets the color-map (= palette) to be used to decode embedded textures in MDL (Quake or 3DG5) files.
        /// This must be a valid path to a file. The file is 768 (256 * 3) bytes large and contains
        /// RGB triplets for each of the 256 palette entries. If the file is not found, a default
        /// palette (from Quake 1) is used.
        /// </summary>
        MDLImportColormap,

        /// <summary>
        /// The Ogre importer will try to load this MaterialFile. Ogre meshes reference with material names, this does not tell TriLib
        /// where the file is located. TriLib will try to find the source file in the following order: [material-name].material, 
        /// [mesh-filename-base].material, and lastly the material name defined by this config property.
        /// </summary>
        OgreImportMaterialFile,

		///<summary>
		///	The Ogre importer will detect the texture usage from the filename. Normally a texture is loaded as a color map, if no target is specified
		///	in the material file. If this is enabled, then TriLib will try to detect the type from the texture filename postfix:
		///	<list type="bullet"><item><description>Normal Maps: _n, _nrm, _nrml, _normal, _normals, _normalmap</description></item><item><description>Specular Maps: _s, _spec, _specular, _specularmap</description></item><item><description>Light Maps: _l, _light, _lightmap, _occ, _occlusion</description></item><item><description>Displacement Maps: _dis, _displacement</description></item></list>The matching is case insensitive. Postfix is taken between the last "_" and last ".". The default behavior is to detect type from lower cased
		///	texture unit name by matching against: normalmap, specular-map, lightmap, and displacement-map. For both cases if no match is found then,
		///	<see cref="TextureType.Diffuse"/> is used.
		///</summary>
        OgreImportTextureTypeFromFilename,

        /// <summary>
        /// Configures the <see cref="TriLib.AssimpPostProcessSteps.OptimizeGraph"/> step
        /// to preserve nodes matching a name in a given list. This is a list of 1 to n strings, whitespace ' ' serves as a delimter character.
        /// Identifiers containing whitespaces must be enclosed in *single* quotation marks. Carriage returns
        /// and tabs are treated as white space.
        /// <para>If a node matches one of these names, it will not be modified or removed by the
        /// post-processing step.</para>
        /// </summary>
        OptimizeGraphExcludeList,

        /// <summary>
        /// Configures the <see cref="TriLib.PostProcessSteps.PreTransformVertices"/> step
        /// to normalize all vertex components into the -1...1 range.
        /// </summary>
        PreTransformVerticesNormalize,

        /// <summary>
        /// Configures the <see cref="TriLib.AssimpPostProcessSteps.PreTransformVertices"/> step
        /// to keep the scene hierarchy. Meshes are moved to world-space, but no optimization
        /// is performed where meshes with the same materials are not joined.
        /// <para>This option could be of used if you have a scene hierarchy that contains
        /// important additional information which you intend to parse.</para>
        /// </summary>
        PreTransformVerticesKeepHierarchy,

        /// <summary>
        /// Configures the <see cref="TriLib.AssimpPostProcessSteps.PreTransformVertices"/> step to use a user defined matrix as the scene root node transformation
        /// before transforming vertices.
        /// </summary>
        PreTransformVerticesRootTransformation,

        /// <summary>
        /// Configures the <see cref="TriLib.AssimpPostProcessSteps.PreTransformVertices"/> step to use a user defined matrix as the scene root node
        /// transformation before transforming vertices.
        /// </summary>
        PreTransformVerticesAddRootTransformation,

        /// <summary>
        /// Input parameter to the <see cref="TriLib.AssimpPostProcessSteps.RemoveComponent"/> step.
        /// It specifies the parts of the data structure to be removed.
        /// <para>This is a bitwise combination of the <see cref="TriLib.AiComponent"/> flag. If no valid mesh is remaining after
        /// the step is executed, the import FAILS.</para>
        /// </summary>
        RemoveComponentFlags,

        /// <summary>
        /// Configures the <see cref="TriLib.AssimpPostProcessSteps.RemoveRedundantMaterials"/> step to
        /// keep materials matching a name in a given list. This is a list of
        /// 1 to n strings where whitespace ' ' serves as a delimiter character. Identifiers
        /// containing whitespaces must be enclosed in *single* quotation marks. Tabs or
        /// carriage returns are treated as whitespace.
        /// <para>If a material matches one of these names, it will not be modified
        /// or removed by the post processing step nor will other materials be replaced
        /// by a reference to it.</para>
        /// </summary>
        RemoveRedundantMaterialsExcludeList,

        /// <summary>
        /// Sets the vertex animation key-frame to be imported. TriLib does not support vertex animation.
        /// </summary>
        SMDImportKeyframe,

        /// <summary>
        /// Input parameter to the <see cref="TriLib.AssimpPostProcessSteps.SortByPrimitiveType"/> step.
        /// It specifies which primitive types are to be removed by the step.
        /// <para>This is a bitwise combination of the <see cref="TriLib.AiPrimitiveType"/> flag.
        /// Specifying ALL types is illegal.</para>
        /// </summary>
        SortByPrimitiveTypeRemove,

        /// <summary>
        /// Maximum bone cone per mesh for the <see cref="TriLib.AssimpPostProcessSteps.SplitByBoneCount"/> step.
        /// </summary>
        SplitByBoneCountMaxBones,

        /// <summary>
        /// Sets the maximum number of triangles a mesh can contain. This is used by the
        /// <see cref="TriLib.AssimpPostProcessSteps.SplitLargeMeshes"/> step to determine
        /// whether a mesh must be split or not.
        /// </summary>
        SplitLargeMeshesTriangleLimit,

        /// <summary>
        /// Sets the maximum number of vertices in a mesh. This is used by the
        /// <see cref="TriLib.AssimpPostProcessSteps.SplitLargeMeshes"/> step to determine
        /// whether a mesh must be split or not.
        /// </summary>
        SplitLargeMeshesVertexLimit,

        /// <summary>
        /// Configures the terragen import plugin to compute UV's for terrains, if
        /// they are not given. Furthermore, a default texture is assigned.
        /// <para>UV coordinates for terrains are so simple to compute that you'll usually 
        /// want to compute them on your own, if you need them. This option is intended for model viewers which
        /// want to offer an easy way to apply textures to terrains.</para>
        /// </summary>
        TerImportMakeUVs,

        /// <summary>
        /// Input parameter to the <see cref="TriLib.AssimpPostProcessSteps.TransformUVCoords"/> step.
        /// It specifies which UV transformations are to be evaluated.
        /// <para>This is bitwise combination of the <see cref="TriLib.AiUVTransform"/> flag.</para>
        /// </summary>
        TransformUVCoordsEvaluate,

        /// <summary>
        /// Sets the vertex animation keyframe to be imported. TriLib does not support vertex animation.
        /// </summary>
        UnrealImportKeyframe,

        /// <summary>
        /// Configures the UNREAL 3D loader to separate faces with different surface flags (e.g. two-sided vs single-sided).
        /// </summary>
        UnrealHandleFlags,

        /// <summary>
        /// If this property is set to true, the Collada names will be used as the
        /// * node name. The default is to use the id tag (resp. sid tag, if no id tag is present)
        /// * instead.
        /// </summary>
        ColladaImportUseColladaNames
    }

    /// <summary>
    /// Represents all asset advanced configuration-metadata.
    /// </summary>
    public static class AssetAdvancedPropertyMetadata
    {
        /// <summary>
        /// Asset advanced configs total group count.
        /// </summary>
        public const int GroupCount = 35;

        /// <summary>
        /// Asset advanced config keys.
        /// </summary>
        public static readonly string[] ConfigKeys =
        {
            "IMPORT_AC_EVAL_SUBDIVISION",
            "IMPORT_AC_SEPARATE_BFCULL",
            "IMPORT_ASE_RECONSTRUCT_NORMALS",
            "PP_GSN_MAX_SMOOTHING_ANGLE",
            "PP_CT_MAX_SMOOTHING_ANGLE",
            "PP_CT_TEXTURE_CHANNEL_INDEX",
            "IMPORT_COLLADA_IGNORE_UP_DIRECTION",
            "PP_DB_THRESHOLD",
            "PP_DB_ALL_OR_NONE",
            "IMPORT_FBX_READ_ALL_MATERIALS",
            "IMPORT_FBX_OPTIMIZE_EMPTY_ANIMATION_CURVES",
			"IMPORT_FBX_EMBEDDED_TEXTURES_LEGACY_NAMING",
            "IMPORT_FBX_STRICT_MODE",
            "IMPORT_FBX_READ_LIGHTS",
            "IMPORT_FBX_READ_MATERIALS",
            "IMPORT_FBX_PRESERVE_PIVOTS",
            "IMPORT_FBX_READ_ALL_GEOMETRY_LAYERS",
            "IMPORT_FBX_READ_TEXTURES",
            "IMPORT_FBX_READ_CAMERAS",
            "IMPORT_FBX_READ_ANIMATIONS",
            "PP_FD_REMOVE",
            "PP_FID_ANIM_ACCURACY",
            "IMPORT_IFC_SKIP_SPACE_REPRESENTATIONS",
            "IMPORT_IFC_CYLINDRICAL_TESSELLATION",
            "IMPORT_IFC_SMOOTHING_ANGLE",
            "IMPORT_IFC_CUSTOM_TRIANGULATION",
            "IMPORT_NO_SKELETON_MESHES",
            "FAVOUR_SPEED",
            "IMPORT_GLOBAL_KEYFRAME",
            "PP_ICL_PTCACHE_SIZE",
            "IMPORT_IRR_ANIM_FPS",
            "PP_LBW_MAX_WEIGHTS",
            "IMPORT_LWO_ONE_LAYER_ONLY",
            "IMPORT_LWS_ANIM_START",
            "IMPORT_LWS_ANIM_END",
            "IMPORT_MD2_KEYFRAME",
            "IMPORT_MD3_SHADER_SRC",
            "IMPORT_MD3_KEYFRAME",
            "IMPORT_MD3_SKIN_NAME",
            "IMPORT_MD3_HANDLE_MULTIPART",
            "IMPORT_MD5_NO_ANIM_AUTOLOAD",
            "IMPORT_MDC_KEYFRAME",
            "IMPORT_MDL_KEYFRAME",
            "IMPORT_MDL_COLORMAP",
            "IMPORT_OGRE_MATERIAL_FILE",
            "IMPORT_OGRE_TEXTURETYPE_FROM_FILENAME",
            "PP_OG_EXCLUDE_LIST",
            "PP_PTV_NORMALIZE",
            "PP_PTV_KEEP_HIERARCHY",
            "PP_PTV_ROOT_TRANSFORMATION",
            "PP_PTV_ADD_ROOT_TRANSFORMATION",
            "PP_RVC_FLAGS",
            "PP_RRM_EXCLUDE_LIST",
            "IMPORT_SMD_KEYFRAME",
            "PP_SBP_REMOVE",
            "PP_SBBC_MAX_BONES",
            "PP_SLM_TRIANGLE_LIMIT",
            "PP_SLM_VERTEX_LIMIT",
            "IMPORT_TER_MAKE_UVS",
            "PP_TUV_EVALUATE",
            "IMPORT_UNREAL_KEYFRAME",
            "UNREAL_HANDLE_FLAGS",
            "IMPORT_COLLADA_USE_COLLADA_NAMES"
        };

        /// <summary>
        /// Gets the given class asset advanced config key.
        /// </summary>
        /// <returns>The config key.</returns>
        /// <param name="className">The class name to get the key.</param>
        public static string GetConfigKey(AssetAdvancedPropertyClassNames className)
        {
            return ConfigKeys[(int)className];
        }

        /// <summary>
        /// Gets an asset advanced config metadata.
        /// </summary>
        /// <param name="key">Asset advanced config key.</param>
        /// <param name="assetAdvancedConfigType">(out) Asset advanced config type.</param>
        /// <param name="className">(out) Asset advanced config class name.</param>
        /// <param name="description">(out) Asset advanced config  description.</param>
        /// <param name="group">(out) Asset advanced config  group.</param>
        /// <param name="defaultValue">(out) Asset advanced config default value.</param>
        /// <param name="minValue">(out) Asset advanced config minimum value.</param>
        /// <param name="maxValue">(out) Asset advanced config max value.</param>
        /// <param name="hasDefaultValue">(out) Asset advanced config has default value?</param>
        /// <param name="hasMinValue">(out) Asset advanced config has minimum value?</param>
        /// <param name="hasMaxValue">(out) Asset advanced config has max value?</param>
        public static void GetOptionMetadata(string key, out AssetAdvancedConfigType assetAdvancedConfigType, out string className, out string description, out string group, out object defaultValue, out object minValue, out object maxValue, out bool hasDefaultValue, out bool hasMinValue, out bool hasMaxValue)
        {
            switch (key)
            {
                #region Configs Metadata
                case "IMPORT_AC_EVAL_SUBDIVISION":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "ACImportEvalSubdivision";
                    description = "Configures whether the AC loader evaluates subdivision surfaces (indicated by the presence\nof the 'subdiv' attribute in the file). By default, TriLib performs\nthe subdivision using the standard Catmull-Clark algorithm.";
                    group = "ACImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_AC_SEPARATE_BFCULL":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "ACImportSeparateBackfaceCull";
                    description = "Configures the AC loader to collect all surfaces which have the \"Backface cull\" flag set in separate\nmeshes.";
                    group = "ACImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_ASE_RECONSTRUCT_NORMALS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "ASEImportReconstructNormals";
                    description = "Configures the ASE loader to always reconstruct normal vectors basing on the smoothing groups\nloaded from the file. Some ASE files carry invalid normals, others don't.";
                    group = "ASEImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_GSN_MAX_SMOOTHING_ANGLE":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Float;
                    className = "CalculateNormalsMaxSmoothingAngle";
                    description = "Specifies the maximum angle that may be between two face normals at the same vertex position that\ntheir normals will be smoothed together during the calculate smooth normals step. This is commonly\ncalled the \"crease angle\". The angle is specified in degrees.";
                    group = "CalculateNormals";
                    hasDefaultValue = true;
                    hasMinValue = true;
                    hasMaxValue = true;
                    defaultValue = 175f;
                    minValue = 0f;
                    maxValue = 175f;
                    break;
                case "PP_CT_MAX_SMOOTHING_ANGLE":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Float;
                    className = "CalculateTangentsMaxSmoothingAngle";
                    description = "Specifies the maximum angle that may be between two vertex tangents that their tangents\nand bitangents are smoothed during the step to calculate the tangent basis. The angle specified \nis in degrees.";
                    group = "CalculateTangents";
                    hasDefaultValue = true;
                    hasMinValue = true;
                    hasMaxValue = true;
                    defaultValue = 45f;
                    minValue = 0f;
                    maxValue = 175f;
                    break;
                case "PP_CT_TEXTURE_CHANNEL_INDEX":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "CalculateTangentsTextureChannelIndex";
                    description = "Source UV channel for tangent space computation. The specified channel must exist or an error will be raised.";
                    group = "CalculateTangents";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_COLLADA_IGNORE_UP_DIRECTION":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "ColladaImportIgnoreUpDirection";
                    description = "Specifies whether the collada loader will ignore the up direction.";
                    group = "ColladaImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_DB_THRESHOLD":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Float;
                    className = "DeboneThreshold";
                    description = "Threshold used to determine if a bone is kept or removed during the TriLib.AssimpPostProcessSteps.Debone step.";
                    group = "Debone";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 1f;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_DB_ALL_OR_NONE":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "DeboneAllOrNone";
                    description = "Require all bones to qualify for deboning before any are removed.";
                    group = "Debone";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_FBX_READ_ALL_MATERIALS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "FBXImportReadAllMaterials";
                    description = "Specifies whether the FBX importer will read all materials present in the source file or take only the referenced materials, if the importer\nwill read materials, otherwise this has no effect.";
                    group = "FBXImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_FBX_OPTIMIZE_EMPTY_ANIMATION_CURVES":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "FBXImportOptimizeEmptyAnimationCurves";
                    description = "Specifies whether the importer will drop empty animation curves or animation curves which match the bind pose";
                    group = "FBXImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
				case "IMPORT_FBX_EMBEDDED_TEXTURES_LEGACY_NAMING":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "FBXImportEmbeddedTextureLegacyNaming";
                    description = "Specifies whether the FBX importer will use legacy embedded texture naming.";
                    group = "FBXImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_FBX_STRICT_MODE":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "FBXImportStrictMode";
                    description = "Specifies whether the FBX importer will act in strict mode in which only the FBX 2013format is supported and any other sub formats are rejected. FBX 2013 is the primary target for the importer, so thisformat is best supported and well-tested.";
                    group = "FBXImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_FBX_READ_LIGHTS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "FBXImportReadLights";
                    description = "Specifies whether the FBX importer will read light sources.";
                    group = "FBXImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_FBX_READ_MATERIALS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "FBXImportReadMaterials";
                    description = "Specifies whether the FBX importer will read materials.";
                    group = "FBXImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_FBX_PRESERVE_PIVOTS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "FBXImportPreservePivots";
                    description = "Specifies whether the FBX importer will preserve pivot points for transformations (as extra nodes). If set to false, pivots\nand offsets will be evaluated whenever possible.";
                    group = "FBXImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_FBX_READ_ALL_GEOMETRY_LAYERS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "FBXImportReadAllGeometryLayers";
                    description = "Specifies whether the FBX importer will merge all geometry layers present in the source file or take only the first.";
                    group = "FBXImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_FBX_READ_TEXTURES":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "FBXImportReadTextures";
                    description = "Specifies whether the FBX importer will read embedded textures.";
                    group = "FBXImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_FBX_READ_CAMERAS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "FBXImportReadCameras";
                    description = "Specifies whether the FBX importer will read cameras.";
                    group = "FBXImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_FBX_READ_ANIMATIONS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "FBXImportReadAnimations";
                    description = "Specifies whether the FBX importer will read animations.";
                    group = "FBXImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_FD_REMOVE":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "FindDegeneratesRemove";
                    description = "Configures the TriLib.AssimpPostProcessSteps.FindDegenerates step\nto remove degenerated primitives from the import immediately.\nThe default behavior converts degenerated triangles to lines and\ndegenerated lines to points.";
                    group = "FindDegenerates";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_FID_ANIM_ACCURACY":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Float;
                    className = "FindInvalidDataAnimAccuracy";
                    description = "Input parameter to the TriLib.AssimpPostProcessSteps.FindInvalidData step.\nIt specifies the floating point accuracy for animation values, specifically the epislon\nduring the comparison. The step checks for animation tracks where all frame values are absolutely equal\nand removes them. Two floats are considered equal if the invariant abs(n0-n1) > epislon holds\ntrue for all vector/quaternion components.";
                    group = "FindInvalidData";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0f;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_IFC_SKIP_SPACE_REPRESENTATIONS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "IFCImportSkipSpaceRepresentations";
                    description = "Specifies whether the IFC loader skips over shape representations of type 'Curve2D'. A lot of files contain both a faceted mesh representation and a outline\nwith a presentation type of 'Curve2D'. Currently TriLib does not convert those, so turning this option off just clutters the log with errors.";
                    group = "IFCImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_IFC_CYLINDRICAL_TESSELLATION":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "IFCImportCylindricalTesselation";
                    description = "This is used by the IFC importer to determine the tessellation parameter\nfor cylindrical shapes, i.e. the number of segments used to aproximate a circle.";
                    group = "IFCImport";
                    hasDefaultValue = true;
                    hasMinValue = true;
                    hasMaxValue = true;
                    defaultValue = 32;
                    minValue = 3;
                    maxValue = 180;
                    break;
                case "IMPORT_IFC_SMOOTHING_ANGLE":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Float;
                    className = "IFCImportSmoothingAngle";
                    description = "This is used by the IFC importer to determine the tessellation parameter\nfor smoothing curves.\n";
                    group = "IFCImport";
                    hasDefaultValue = true;
                    hasMinValue = true;
                    hasMaxValue = true;
                    defaultValue = 10f;
                    minValue = 5f;
                    maxValue = 120f;
                    break;
                case "IMPORT_IFC_CUSTOM_TRIANGULATION":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "IFCImportCustomTriangulation";
                    description = "Specifies whether the IFC loader will use its own, custom triangulation algorithm to triangulate wall and floor meshes. If this is set to false,\nwalls will be either triangulated by the post process triangulation or will be passed through as huge polygons with faked holes (e.g. holes that are connected\nwith the outer boundary using a dummy edge). It is highly recommended to leave this property set to true as the default post process has some known\nissues with these kind of polygons.";
                    group = "IFCImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_NO_SKELETON_MESHES":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "ImportNoSkeletonMeshes";
                    description = "Global setting to disable generation of skeleton dummy meshes. These are generated as a visualization aid\nin cases which the input data contains no geometry, but only animation data. So the geometry are visualizing\nthe bones.";
                    group = "Import";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "FAVOUR_SPEED":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "ImportFavourSpeed";
                    description = "A hint to TriLib to favour speed against import quality. Enabling this option\nmay result in faster loading, or it may not. It is just a hint to loaders and post-processing\nsteps to use faster code paths if possible. A value not equal to zero stands\nfor true.";
                    group = "Import";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_GLOBAL_KEYFRAME":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "ImportGlobalKeyframe";
                    description = "Sets the vertex animation keyframe to be imported. TriLib does not support vertex animation.";
                    group = "Import";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_ICL_PTCACHE_SIZE":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "ImproveCacheLocalityPostTransformCacheSize";
                    description = "Sets the size of the post-transform vertex cache to optimize vertices for. This is\nfor the TriLib.AssimpPostProcessSteps.ImproveCacheLocality step. The size\nis given in vertices. Of course you can't know how the vertex format will exactly look\nlike after the import returns, but you can still guess what your meshes will\nprobably have. The default value *has* resulted in slight performance improvements\nfor most Nvidia/AMD cards since 2002.";
                    group = "ImproveCacheLocality";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_IRR_ANIM_FPS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "IRRImportAnimFPS";
                    description = "Defines the output frame rate of the IRR loader.\nIRR animations are difficult to convert for TriLib and there will always be\na loss of quality. This setting defines how many keys per second are returned by the converter.";
                    group = "IRRImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 100;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_LBW_MAX_WEIGHTS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "LimitBoneWeightsMaxWeights";
                    description = "Sets the maximum number of bones that can affect a single vertex. This is used\nby the TriLib.AssimpPostProcessSteps.LimitBoneWeights step.";
                    group = "LimitBoneWeights";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0x4;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_LWO_ONE_LAYER_ONLY":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "LWOImportOneLayerOnly";
                    description = "Configures the LWO loader to load just one layer from the model.\nLWO files consist of layers and in some cases it could be useful to load only one of them.\nThis property can be either a string - which specifies the name of the layer - or an integer - the index\nof the layer. If the property is not set then the whole LWO model is loaded. Loading fails\nif the requested layer is not vailable. The layer index is zero-based and the layer name may not be empty";
                    group = "LWOImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_LWS_ANIM_START":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "LWSImportAnimStart";
                    description = "Defines the beginning of the time range for which the LWS loader evaluates animations and computes\nAiNodeAnim's.TriLib provides full conversion of Lightwave's envelope system, including pre and post\nconditions. The loader computes linearly subsampled animation channels with the frame rate\ngiven in the LWS file. This property defines the start time.\nAnimation channels are only generated if a node has at least one envelope with more than one key\nassigned. This property is given in frames where '0' is the first. By default,\nif this property is not set, the importer takes the animation start from the input LWS\nfile ('FirstFrame' line)";
                    group = "LWSImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_LWS_ANIM_END":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "LWSImportAnimEnd";
                    description = "Defines the ending of the time range for which the LWS loader evaluates animations and computes\nAiNodeAnim's.\nTriLib provides full conversion of Lightwave's envelope system, including pre and post\nconditions. The loader computes linearly subsampled animation channels with the frame rate\ngiven in the LWS file. This property defines the end time.\nAnimation channels are only generated if a node has at least one envelope with more than one key\nassigned. This property is given in frames where '0' is the first. By default,\nif this property is not set, the importer takes the animation end from the input LWS\nfile.";
                    group = "LWSImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_MD2_KEYFRAME":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "MD2ImportKeyframe";
                    description = "Sets the vertex animation keyframe to be imported. TriLib does not support vertex animation.";
                    group = "MD2Import";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_MD3_SHADER_SRC":
                    assetAdvancedConfigType = AssetAdvancedConfigType.String;
                    className = "MD3ImportShaderSource";
                    description = "Specifies the Quake 3 shader file to be used for a particular MD3 file. This can be a full path or\nrelative to where all MD3 shaders reside.";
                    group = "MD3Import";
                    hasDefaultValue = false;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = null;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_MD3_KEYFRAME":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "MD3ImportKeyframe";
                    description = "Sets the vertex animation keyframe to be imported. TriLib does not support vertex animation.";
                    group = "MD3Import";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_MD3_SKIN_NAME":
                    assetAdvancedConfigType = AssetAdvancedConfigType.String;
                    className = "MD3ImportSkinName";
                    description = "Tells the MD3 loader which skin files to load. When loading MD3 files, TriLib checks\nwhether a file named \"md3_file_name\"_\"skin_name\".skin exists. These files are used by\nQuake III to be able to assign different skins (e.g. red and blue team) to models. 'default', 'red', 'blue'\nare typical skin names.";
                    group = "MD3Import";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = "default";
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_MD3_HANDLE_MULTIPART":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "MD3ImportHandleMultiPart";
                    description = "Configures the MD3 loader to detect and process multi-part Quake player models. These models\nusually consit of three files, lower.md3, upper.md3 and head.md3. If this propery is\nset to true, TriLib will try to load and combine all three files if one of them is loaded.";
                    group = "MD3Import";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_MD5_NO_ANIM_AUTOLOAD":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "MD5ImportNoAnimAutoLoad";
                    description = "Configures the MD5 loader to not load the MD5ANIM file for a MD5MESH file automatically.\nThe default strategy is to look for a file with the same name but with the MD5ANIm extension\nin the same directory. If it is found it is loaded and combined with the MD5MESH file. This configuration\noption can be used to disable this behavior.";
                    group = "MD5Import";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_MDC_KEYFRAME":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "MDCImportKeyframe";
                    description = "Sets the vertex animation keyframe to be imported. TriLib does not support vertex animation.";
                    group = "MDCImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_MDL_KEYFRAME":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "MDLmportKeyframe";
                    description = "Sets the vertex animation keyframe to be imported. TriLib does not support vertex animation.";
                    group = "MDLImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_MDL_COLORMAP":
                    assetAdvancedConfigType = AssetAdvancedConfigType.String;
                    className = "MDLImportColormap";
                    description = "Sets the colormap(= palette) to be used to decode embedded textures in MDL (Quake or 3DG5) files.\nThis must be a valid path to a file. The file is 768 (256 * 3) bytes large and contains\nRGB triplets for each of the 256 palette entries. If the file is not found, a default\npalette (from Quake 1) is used.";
                    group = "MDLImport";
                    hasDefaultValue = false;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = null;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_OGRE_MATERIAL_FILE":
                    assetAdvancedConfigType = AssetAdvancedConfigType.String;
                    className = "OgreImportMaterialFile";
                    description = "The Ogre importer will try to load this MaterialFile. Ogre meshes reference with material names, this does not tell TriLib\nwhere the file is located. TriLib will try to find the source file in the following order: [material-name].material, [mesh-filename-base].material,\nand lastly the material name defined by this config property.";
                    group = "OgreImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = "Scene.Material";
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_OGRE_TEXTURETYPE_FROM_FILENAME":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "OgreImportTextureTypeFromFilename";
                    description = "The Ogre importer will detect the texture usage from the filename. Normally a texture is loaded as a color map, if no target is specified\nin the material file. If this is enabled, then TriLib will try to detect the type from the texture filename postfix:\nNormal Maps: _n, _nrm, _nrml, _normal, _normals, _normalmapSpecular Maps: _s, _spec, _specular, _specularmapLight Maps: _l, _light, _lightmap, _occ, _occlusionDisplacement Maps: _dis, _displacementThe matching is case insensitive. Postfix is taken between the last \"_\" and last \".\". The default behavior is to detect type from lower cased\ntexture unit name by matching against: normalmap, specularmap, lightmap, and displacementmap. For both cases if no match is found then,\nTextureType.Diffuse is used.";
                    group = "OgreImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_OG_EXCLUDE_LIST":
                    assetAdvancedConfigType = AssetAdvancedConfigType.String;
                    className = "OptimizeGraphExcludeList";
                    description = "Configures the TriLib.AssimpPostProcessSteps.OptimizeGraph step\nto preserve nodes matching a name in a given list. This is a list of 1 to n strings, whitespace ' ' serves as a delimter character.\nIdentifiers containing whitespaces must be enclosed in *single* quotation marks. Carriage returns\nand tabs are treated as white space.\nIf a node matches one of these names, it will not be modified or removed by the\npostprocessing step.";
                    group = "OptimizeGraph";
                    hasDefaultValue = false;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = null;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_PTV_NORMALIZE":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "PreTransformVerticesNormalize";
                    description = "Configures the TriLib.PostProcessSteps.PreTransformVertices step\nto normalize all vertex components into the -1...1 range.";
                    group = "PreTransformVertices";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_PTV_KEEP_HIERARCHY":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "PreTransformVerticesKeepHierarchy";
                    description = "Configures the TriLib.AssimpPostProcessSteps.PreTransformVertices step\nto keep the scene hierarchy. Meshes are moved to worldspace, but no optimization\nis performed where meshes with the same materials are not joined.\nThis option could be of used if you have a scene hierarchy that contains\nimportant additional information which you intend to parse.";
                    group = "PreTransformVertices";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_PTV_ROOT_TRANSFORMATION": 
                    assetAdvancedConfigType = AssetAdvancedConfigType.AiMatrix;
                    className = "PreTransformVerticesRootTransformation";
                    description = "Configures the TriLib.AssimpPostProcessSteps.PreTransformVertices step to use a user defined matrix as the scene root node transformation\nbefore transforming vertices.";
                    group = "PreTransformVertices";
                    hasDefaultValue = false;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = null;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_PTV_ADD_ROOT_TRANSFORMATION":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "PreTransformVerticesAddRootTransformation";
                    description = "Configures the TriLib.AssimpPostProcessSteps.PreTransformVertices step to use a user defined matrix as the scene root node\ntransformation before transforming vertices.";
                    group = "PreTransformVertices";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_RVC_FLAGS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.AiComponent;
                    className = "RemoveComponentFlags";
                    description = "Input parameter to the TriLib.AssimpPostProcessSteps.RemoveComponent step.\nIt specifies the parts of the data structure to be removed.\nThis is a bitwise combination of the TriLib.AiComponent flag. If no valid mesh is remaining after\nthe step is executed, the import FAILS.";
                    group = "RemoveComponent";
                    hasDefaultValue = false;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = null;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_RRM_EXCLUDE_LIST":
                    assetAdvancedConfigType = AssetAdvancedConfigType.String;
                    className = "RemoveRedundantMaterialsExcludeList";
                    description = "Configures the TriLib.AssimpPostProcessSteps.RemoveRedundantMaterials step to\nkeep materials matching a name in a given list. This is a list of\n1 to n strings where whitespace ' ' serves as a delimiter character. Identifiers\ncontaining whitespaces must be enclosed in *single* quotation marks. Tabs or\ncarriage returns are treated as whitespace.\nIf a material matches one of these names, it will not be modified\nor removed by the post processing step nor will other materials be replaced\nby a reference to it.";
                    group = "RemoveRedundantMaterials";
                    hasDefaultValue = false;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = null;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_SMD_KEYFRAME":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "SMDImportKeyframe";
                    description = "Sets the vertex animation keyframe to be imported. TriLib does not support vertex animation.";
                    group = "SMDImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_SBP_REMOVE":
                    assetAdvancedConfigType = AssetAdvancedConfigType.AiPrimitiveType;
                    className = "SortByPrimitiveTypeRemove";
                    description = "Input parameter to the TriLib.AssimpPostProcessSteps.SortByPrimitiveType step.\nIt specifies which primitive types are to be removed by the step.\nThis is a bitwise combination of the TriLib.AiPrimitiveType flag.\nSpecifying ALL types is illegal.";
                    group = "SortByPrimitiveType";
                    hasDefaultValue = false;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = null;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_SBBC_MAX_BONES":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "SplitByBoneCountMaxBones";
                    description = "Maximum bone cone per mesh for the TriLib.AssimpPostProcessSteps.SplitByBoneCount step.";
                    group = "SplitByBoneCount";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 60;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_SLM_TRIANGLE_LIMIT":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "SplitLargeMeshesTriangleLimit";
                    description = "Sets the maximum number of triangles a mesh can contain. This is used by the\nTriLib.AssimpPostProcessSteps.SplitLargeMeshes step to determine\nwhether a mesh must be split or not.";
                    group = "SplitLargeMeshes";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 1000000;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_SLM_VERTEX_LIMIT":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "SplitLargeMeshesVertexLimit";
                    description = "Sets the maximum number of vertices in a mesh. This is used by the\nTriLib.AssimpPostProcessSteps.SplitLargeMeshes step to determine\nwhether a mesh must be split or not.";
                    group = "SplitLargeMeshes";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 1000000;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_TER_MAKE_UVS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "TerImportMakeUVs";
                    description = "Configures the terragen import plugin to compute UV's for terrains, if\nthey are not given. Furthermore, a default texture is assigned.\nUV coordinates for terrains are so simple to compute that you'll usually \nwant to compute them on your own, if you need them. This option is intended for model viewers which\nwant to offer an easy way to apply textures to terrains.";
                    group = "TERImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                case "PP_TUV_EVALUATE":
                    assetAdvancedConfigType = AssetAdvancedConfigType.AiUVTransform;
                    className = "TransformUVCoordsEvaluate";
                    description = "Input parameter to the TriLib.AssimpPostProcessSteps.TransformUVCoords step.\nIt specifies which UV transformations are to be evaluated.\nThis is bitwise combination of the TriLib.AiUVTransform flag.";
                    group = "TransformUVCoords";
                    hasDefaultValue = false;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = null;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_UNREAL_KEYFRAME":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Integer;
                    className = "UnrealImportKeyframe";
                    description = "Sets the vertex animation keyframe to be imported. TriLib does not support vertex animation.";
                    group = "UnrealImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = 0;
                    minValue = null;
                    maxValue = null;
                    break;
                case "UNREAL_HANDLE_FLAGS":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "UnrealHandleFlags";
                    description = "Configures the UNREAL 3D loader to separate faces with different surface flags (e.g. two-sided vs single-sided).";
                    group = "UnrealImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = true;
                    minValue = null;
                    maxValue = null;
                    break;
                case "IMPORT_COLLADA_USE_COLLADA_NAMES":
                    assetAdvancedConfigType = AssetAdvancedConfigType.Bool;
                    className = "ColladaImportUseColladaNames";
                    description = "Specifies whether the Collada loader should use Collada names as node names.\n\n\nIf this property is set to true, the Collada names will be used as the\nnode name. The default is to use the id tag (resp. sid tag, if no id tag is present)\ninstead.";
                    group = "ColladaImport";
                    hasDefaultValue = true;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                default:
                    assetAdvancedConfigType = AssetAdvancedConfigType.None;
                    className = string.Empty;
                    description = string.Empty;
                    group = string.Empty;
                    hasDefaultValue = false;
                    hasMinValue = false;
                    hasMaxValue = false;
                    defaultValue = false;
                    minValue = null;
                    maxValue = null;
                    break;
                #endregion
            }
        }
    }

    /// <summary>
    /// Represents an asset advanced config.
    /// </summary>
    [Serializable]
    public class AssetAdvancedConfig
    {
        /// <summary>
        /// Config Key.
        /// </summary>
        public string Key;

        /// <summary>
        /// Config Integer value.
        /// </summary>
        public int IntValue;

        /// <summary>
        /// Config Float value.
        /// </summary>
        public float FloatValue;

        /// <summary>
        /// Config Bool value.
        /// </summary>
        public bool BoolValue;

        /// <summary>
        /// Config String value.
        /// </summary>
        public string StringValue;

		/// <summary>
		/// Config Translation value.
		/// </summary>
		public UnityEngine.Vector3 TranslationValue;

		/// <summary>
		/// Config Rotation value.
		/// </summary>
		public UnityEngine.Vector3 RotationValue;

		/// <summary>
		/// Config Scale value.
		/// </summary>
		public UnityEngine.Vector3 ScaleValue;

        /// <summary>
        /// Creates a new integer advanced config.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="className">Config class name.</param>
        /// <param name="value">Config value.</param>
        public static AssetAdvancedConfig CreateConfig(AssetAdvancedPropertyClassNames className, int value) {
            return new AssetAdvancedConfig(AssetAdvancedPropertyMetadata.GetConfigKey(className), value);
        }

        /// <summary>
        /// Creates a new float advanced config.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="className">Config class name.</param>
        /// <param name="value">Config value.</param>
        public static AssetAdvancedConfig CreateConfig(AssetAdvancedPropertyClassNames className, float value) {
            return new AssetAdvancedConfig(AssetAdvancedPropertyMetadata.GetConfigKey(className), value);
        }

        /// <summary>
        /// Creates a new bool advanced config.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="className">Config class name.</param>
        /// <param name="value">Config value.</param>
        public static AssetAdvancedConfig CreateConfig(AssetAdvancedPropertyClassNames className, bool value) {
            return new AssetAdvancedConfig(AssetAdvancedPropertyMetadata.GetConfigKey(className), value);
        }

        /// <summary>
        /// Creates a new string advanced config.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="className">Config class name.</param>
        /// <param name="value">Config value.</param>
        public static AssetAdvancedConfig CreateConfig(AssetAdvancedPropertyClassNames className, string value) {
            return new AssetAdvancedConfig(AssetAdvancedPropertyMetadata.GetConfigKey(className), value);
        }

        /// <summary>
        /// Creates a new <see cref="AiComponent"/> advanced config.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="className">Config class name.</param>
        /// <param name="value">Config value.</param>
        public static AssetAdvancedConfig CreateConfig(AssetAdvancedPropertyClassNames className, AiComponent value) {
            return new AssetAdvancedConfig(AssetAdvancedPropertyMetadata.GetConfigKey(className), value);
        }

        /// <summary>
        /// Creates a new <see cref="AiPrimitiveType"/> advanced config.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="className">Config class name.</param>
        /// <param name="value">Config value.</param>
        public static AssetAdvancedConfig CreateConfig(AssetAdvancedPropertyClassNames className, AiPrimitiveType value) {
            return new AssetAdvancedConfig(AssetAdvancedPropertyMetadata.GetConfigKey(className), value);
        }

        /// <summary>
        /// Creates a new <see cref="AiUVTransform"/> advanced config.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="className">Config class name.</param>
        /// <param name="value">Config value.</param>
        public static AssetAdvancedConfig CreateConfig(AssetAdvancedPropertyClassNames className, AiUVTransform value) {
            return new AssetAdvancedConfig(AssetAdvancedPropertyMetadata.GetConfigKey(className), value);
        }

        /// <summary>
        /// Creates a new matrix advanced config.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="className">Config class name.</param>
        /// <param name="translation">Translation value.</param>
        /// <param name="rotation">Rotation value.</param>
        /// <param name="scale">Scale value.</param>
        public static AssetAdvancedConfig CreateConfig(AssetAdvancedPropertyClassNames className, UnityEngine.Vector3 translation, UnityEngine.Vector3 rotation, UnityEngine.Vector3 scale) {
            return new AssetAdvancedConfig(AssetAdvancedPropertyMetadata.GetConfigKey(className), translation, rotation, scale);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriLib.AssetAdvancedConfig"/> class.
        /// </summary>
        public AssetAdvancedConfig()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriLib.AssetAdvancedConfig"/> class.
        /// </summary>
        /// <param name="key">Config Key.</param>
        public AssetAdvancedConfig(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriLib.AssetAdvancedConfig"/> class.
        /// </summary>
        /// <param name="key">Config Key.</param>
        /// <param name="defaultValue">Config default value.</param>
        public AssetAdvancedConfig(string key, int defaultValue)
        {
            Key = key;
            IntValue = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriLib.AssetAdvancedConfig"/> class.
        /// </summary>
        /// <param name="key">Config Key.</param>
        /// <param name="defaultValue">Config default value.</param>
        public AssetAdvancedConfig(string key, float defaultValue)
        {
            Key = key;
            FloatValue = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriLib.AssetAdvancedConfig"/> class.
        /// </summary>
        /// <param name="key">Config Key.</param>
        /// <param name="defaultValue">Config default value.</param>
        public AssetAdvancedConfig(string key, bool defaultValue)
        {
            Key = key;
            BoolValue = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriLib.AssetAdvancedConfig"/> class.
        /// </summary>
        /// <param name="key">Config Key.</param>
        /// <param name="defaultValue">Config default value.</param>
        public AssetAdvancedConfig(string key, string defaultValue)
        {
            Key = key;
            StringValue = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriLib.AssetAdvancedConfig"/> class.
        /// </summary>
        /// <param name="key">Config Key.</param>
        /// <param name="defaultValue">Config default value.</param>
        public AssetAdvancedConfig(string key, AiComponent defaultValue)
        {
            Key = key;
            IntValue = (int)defaultValue >> 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriLib.AssetAdvancedConfig"/> class.
        /// </summary>
        /// <param name="key">Config Key.</param>
        /// <param name="defaultValue">Config default value.</param>
        public AssetAdvancedConfig(string key, AiPrimitiveType defaultValue)
        {
            Key = key;
            IntValue = (int)defaultValue >> 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriLib.AssetAdvancedConfig"/> class.
        /// </summary>
        /// <param name="key">Config Key.</param>
        /// <param name="defaultValue">Config default value.</param>
        public AssetAdvancedConfig(string key, AiUVTransform defaultValue)
        {
            Key = key;
            IntValue = (int)defaultValue >> 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriLib.AssetAdvancedConfig"/> class.
        /// </summary>
        /// <param name="key">Config Key.</param>
        /// <param name="translation">Translation default value.</param>
        /// <param name="rotation">Rotation default value.</param>
        /// <param name="scale">Scale default value.</param>
        public AssetAdvancedConfig(string key, UnityEngine.Vector3 translation, UnityEngine.Vector3 rotation, UnityEngine.Vector3 scale)
		{
			Key = key;
			TranslationValue = translation;
			RotationValue = rotation;
			ScaleValue = scale;
		}
    }
}

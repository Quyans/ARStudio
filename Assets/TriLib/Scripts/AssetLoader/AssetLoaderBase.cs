using System;
using System.Collections.Generic;
using System.IO;
#if !TRILIB_USE_UNITY_TEXTURE_LOADER
using STB;
#endif
using UnityEngine;
using UnityEngine.Rendering;
#if !UNITY_EDITOR && !NET_4_6 &&! NETFX_CORE
using AOT;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
#if TRILIB_USE_ZIP
#if !UNITY_EDITOR && (NET_4_6 || NETFX_CORE)
using System.IO.Compression;
#else
using ICSharpCode.SharpZipLib.Zip;
#endif
#endif
namespace TriLib
{
    /// <summary>
    /// Represents an <see cref="AssetLoader"/> asset loaded event handler.
    /// </summary>
    public delegate void ObjectLoadedHandle(GameObject loadedGameObject);

    /// <summary>
    /// Represents an <see cref="AssetLoader"/> mesh creation event handler.
    /// </summary>
    public delegate void MeshCreatedHandle(uint meshIndex, Mesh mesh);

    /// <summary>
    /// Represents an <see cref="AssetLoader"/> material created event handler.
    /// </summary>
    public delegate void MaterialCreatedHandle(uint materialIndex, bool isOverriden, Material material);

    /// <summary>
    /// Represents an <see cref="AssetLoader"/> animation created event handler.
    /// </summary>
    public delegate void AnimationClipCreatedHandle(uint animationClipIndex, AnimationClip animationClip);

    /// <summary>
    /// Represents an <see cref="AssetLoader"/> metadata processed event handler.
    /// </summary>
    /// <param name="metadataType">The <see cref="AssimpMetadataType"/> of the metadata.</param>
    /// <param name="metadataIndex">The index of the metadata</param>
    /// <param name="metadataKey">The key of the metadata</param>
    /// <param name="metadataValue">The value of the metadata</param>
    public delegate void MetadataProcessedHandle(AssimpMetadataType metadataType, uint metadataIndex, string metadataKey, object metadataValue);

    /// <summary>
    /// Event used to pass a custom texture data when processing a texture entry.
    /// </summary>
    public delegate EmbeddedTextureData EmbeddedTextureLoadCallback(string path);

    /// <summary>
    /// Callback used to retrieve custom embedded teture data.
    /// </summary>
    /// <param name="path">Texture path.</param>
    /// <param name="basePath">Texture base path.</param>
    /// <returns>A new <see cref="EmbeddedTextureData"/></returns>
    public delegate EmbeddedTextureData LoadTextureDataCallback(string path, string basePath);

    /// <summary>
    /// Base asset loader.
    /// </summary>
    public class AssetLoaderBase : IDisposable
    {
        /// <summary>
        /// Assimp uses this suffix when creating extra transformation nodes to handle FBX files correctly.
        /// </summary>
        protected const string AssimpFbxMagicString = "_$AssimpFbx$";

        /// <summary>
        /// Assimp uses this preffix when loading files from memory.
        /// </summary>
        protected const string AssimpFilenameMagicString = "$$$___magic___$$$";

        /// <summary>
        /// Main scene <see cref="TriLib.NodeData"/>.
        /// </summary>
        protected NodeData RootNodeData;

        /// <summary>
        /// Processed <see cref="TriLib.MaterialData"/> list.
        /// </summary>
        protected MaterialData[] MaterialData;

        /// <summary>
        /// Processed <see cref="TriLib.MeshData"/> list.
        /// </summary>
        protected MeshData[] MeshData;

        /// <summary>
        /// Processed <see cref="TriLib.AnimationData"/> list.
        /// </summary>
        protected AnimationData[] AnimationData;

        /// <summary>
        /// Processed <see cref="TriLib.CameraData"/> list.
        /// </summary>
        protected CameraData[] CameraData;

        /// <summary>
        /// Processed <see cref="TriLib.AssimpMetadata"/> list.
        /// </summary>
        protected AssimpMetadata[] Metadata;

        /// <summary>
        /// Processed nodes path dictionary.
        /// </summary>
        protected Dictionary<string, string> NodesPath;

        /// <summary>
        /// Loaded <see cref="UnityEngine.Material"/> for a given name dictionary.
        /// </summary>
        protected Dictionary<string, Material> LoadedMaterials;

        /// <summary>
        /// Loaded <see cref="UnityEngine.Texture2D"/> for a given name dictionary.
        /// </summary>
        protected Dictionary<string, Texture2D> LoadedTextures;

        /// <summary>
        /// Loaded bone names for a given <see cref="UnityEngine.SkinnedMeshRenderer"/> dictionary.
        /// </summary>
        protected Dictionary<SkinnedMeshRenderer, IList<string>> LoadedBoneNames;

        /// <summary>
        /// Base Diffuse <see cref="UnityEngine.Material"/> used to load materials.
        /// </summary>
        protected static Material StandardBaseMaterial;

        /// <summary>
        /// Base Specular <see cref="UnityEngine.Material"/> used to load materials.
        /// </summary>
        protected static Material StandardSpecularMaterial;

        /// <summary>
        /// Base Diffuse Alpha <see cref="UnityEngine.Material"/> used to load materials.
        /// </summary>
        protected static Material StandardBaseAlphaMaterial;

        /// <summary>
        /// Base Specular Alpha <see cref="UnityEngine.Material"/> used to load materials.
        /// </summary>
        protected static Material StandardSpecularAlphaMaterial;

        /// <summary>
        /// Base Diffuse Cutout <see cref="UnityEngine.Material"/> used to load materials.
        /// </summary>
        protected static Material StandardBaseCutoutMaterial;

        /// <summary>
        /// Base Specular Cutout <see cref="UnityEngine.Material"/> used to load materials.
        /// </summary>
        protected static Material StandardSpecularCutoutMaterial;

        /// <summary>
        /// <see cref="UnityEngine.Texture"/> used to show when no texture is found.
        /// </summary>
        protected static Texture2D NotFoundTexture;

        /// <summary>
        /// <see cref="UnityEngine.Texture"/> used as base to create normal maps.
        /// </summary>
        public static Texture2D NormalBaseTexture
        {
            get;
            private set;
        }

        /// <summary>
        /// Used to temporarily store nodes id.
        /// </summary>
        protected uint NodeId;

        /// <summary>
        /// Used to temporarily indicate if there are any bones assigned to loaded meshes.
        /// </summary>
        protected bool HasBoneInfo;

        /// <summary>
        /// Use this field to assign the callback that will be triggered when a texture looks up for embedded data.
        /// </summary>
        public event EmbeddedTextureLoadCallback EmbeddedTextureLoad;

        /// <summary>
        /// Use this field to assign the event that occurs when a mesh is loaded.
        /// </summary>
        public event MeshCreatedHandle OnMeshCreated;

        /// <summary>
        /// Gets a value indicating whether this instance has on mesh created event.
        /// </summary>
        /// <value><c>true</c> if this instance has on mesh created event; otherwise, <c>false</c>.</value>
        protected bool HasOnMeshCreated
        {
            get
            {
                return OnMeshCreated != null;
            }
        }

        /// <summary>
        /// Triggers the on mesh created event.
        /// </summary>
        /// <param name="meshIndex">Mesh index.</param>
        /// <param name="mesh">Mesh.</param>
        protected void TriggerOnMeshCreated(uint meshIndex, Mesh mesh)
        {
            if (OnMeshCreated != null)
            {
                OnMeshCreated(meshIndex, mesh);
            }
        }

        /// <summary>
        /// Use this field to assign the event that occurs when a material is created.
        /// </summary>
        public event MaterialCreatedHandle OnMaterialCreated;

        /// <summary>
        /// Gets a value indicating whether this instance has on material created event.
        /// </summary>
        /// <value><c>true</c> if this instance has on material created event; otherwise, <c>false</c>.</value>
        protected bool HasOnMaterialCreated
        {
            get
            {
                return OnMaterialCreated != null;
            }
        }

        /// <summary>
        /// Triggers the on material created event.
        /// </summary>
        /// <param name="materialIndex">Material index.</param>
        /// <param name="isOverriden">If set to <c>true</c> is overriden.</param>
        /// <param name="material">Material.</param>
        protected void TriggerOnMaterialCreated(uint materialIndex, bool isOverriden, Material material)
        {
            if (OnMaterialCreated != null)
            {
                OnMaterialCreated(materialIndex, isOverriden, material);
            }
        }

        /// <summary>
        /// Use this field to assign the event that occurs when a texture is loaded.
        /// </summary>
        public event TextureLoadHandle OnTextureLoaded;

        /// <summary>
        /// Gets a value indicating whether this instance has on texture loaded event.
        /// </summary>
        /// <value><c>true</c> if this instance has on texture loaded event; otherwise, <c>false</c>.</value>
        protected bool HasOnTextureLoaded
        {
            get
            {
                return OnTextureLoaded != null;
            }
        }

        /// <summary>
        /// Triggers the on texture loaded event.
        /// </summary>
        /// <param name="sourcePath">Source path.</param>
        /// <param name="material">Material.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="texture">Texture.</param>
        protected void TriggerOnTextureLoaded(string sourcePath, Material material, string propertyName, Texture2D texture)
        {
            if (OnTextureLoaded != null)
            {
                OnTextureLoaded(sourcePath, material, propertyName, texture);
            }
        }

        /// <summary>
        /// Gets the get on texture loaded event.
        /// </summary>
        /// <value>Texture loaded event.</value>
        [Obsolete("GetOnTextureLoaded is not used anymore. If you want to change loaded textures, override LoadTextureFromFile method.")]
        protected TextureLoadHandle GetOnTextureLoaded
        {
            get
            {
                return OnTextureLoaded;
            }
        }

        /// <summary>
        /// Use this field to assign the event that occurs when a texture is about to load, to replace the loading routine.
        /// </summary>
        [Obsolete("OnTexturePreLoad is not used anymore. If you want to change loaded textures, override LoadTextureFromFile method.")]
        public event TexturePreLoadHandle OnTexturePreLoad;

        /// <summary>
        /// Gets a value indicating whether this instance has on texture pre-load event.
        /// </summary>
        /// <value><c>true</c> if this instance has on texture pre-load event; otherwise, <c>false</c>.</value>
        [Obsolete("HasOnTexturePreLoad is not used anymore. If you want to change loaded textures, override LoadTextureFromFile method.")]
        protected bool HasOnTexturePreLoad
        {
            get
            {
                return OnTexturePreLoad != null;
            }
        }

        /// <summary>
        /// Triggers the on texture pre-load event.
        /// </summary>
        /// <param name="scene">Scene.</param>
        /// <param name="path">Path.</param>
        /// <param name="name">Name.</param>
        /// <param name="material">Material.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="checkAlphaChannel">Check alpha channel.</param>
        /// <param name="textureWrapMode">Texture wrap mode.</param>
        /// <param name="basePath">Base path.</param>
        /// <param name="onTextureLoaded">On texture loaded.</param>
        /// <param name="textureCompression">Texture compression.</param>
        /// <param name="isNormalMap">If set to <c>true</c> is normal map.</param>
        [Obsolete("TriggerOnTexturePreLoad is not used anymore. If you want to change loaded textures, override LoadTextureFromFile method.")]
        protected void TriggerOnTexturePreLoad(IntPtr scene, string path, string name, Material material, string propertyName, ref bool checkAlphaChannel, TextureWrapMode textureWrapMode = TextureWrapMode.Repeat, string basePath = null, TextureLoadHandle onTextureLoaded = null, TextureCompression textureCompression = TextureCompression.None, bool isNormalMap = false)
        {
            if (OnTexturePreLoad != null)
            {
                OnTexturePreLoad(scene, path, name, material, propertyName, ref checkAlphaChannel, textureWrapMode,
                    basePath, onTextureLoaded, textureCompression, isNormalMap);
            }
        }

        /// <summary>
        /// Use this field to assign the event that occurs when an animation is created.
        /// </summary>
        public event AnimationClipCreatedHandle OnAnimationClipCreated;

        /// <summary>
        /// Gets a value indicating whether this instance has on animation clip created event.
        /// </summary>
        /// <value><c>true</c> if this instance has on animation clip created event; otherwise, <c>false</c>.</value>
        protected bool HasOnAnimationClipCreated
        {
            get
            {
                return OnAnimationClipCreated != null;
            }
        }

        /// <summary>
        /// Triggers the on animation clip created event.
        /// </summary>
        /// <param name="animationClipIndex">Animation clip index.</param>
        /// <param name="animationClip">Animation clip.</param>
        protected void TriggerOnAnimationClipCreated(uint animationClipIndex, AnimationClip animationClip)
        {
            if (OnAnimationClipCreated != null)
            {
                OnAnimationClipCreated(animationClipIndex, animationClip);
            }
        }

        /// <summary>
        /// Use this field to assign the event that occurs when the asset is loaded.
        /// </summary>
        public event ObjectLoadedHandle OnObjectLoaded;

        /// <summary>
        /// Gets a value indicating whether this instance has on object loaded event.
        /// </summary>
        /// <value><c>true</c> if this instance has on object loaded event; otherwise, <c>false</c>.</value>
        protected bool HasOnObjectLoaded
        {
            get
            {
                return OnObjectLoaded != null;
            }
        }

        /// <summary>
        /// Triggers the on object loaded event.
        /// </summary>
        /// <param name="loadedGameObject">Created <see cref="UnityEngine.GameObject"/>.</param>
        protected void TriggerOnObjectLoaded(GameObject loadedGameObject)
        {
            if (OnObjectLoaded != null)
            {
                OnObjectLoaded(loadedGameObject);
            }
        }

        /// <summary>
        /// Use this field to assign the event that will occurs when each file metadata is processed.
        /// </summary>
        public event MetadataProcessedHandle OnMetadataProcessed;

        /// <summary>
        /// Gets a value indicating whether this instance has on metadata processed event.
        /// </summary>
        /// <value><c>true</c> if this instance has on metadata processed event; otherwise, <c>false</c>.</value>
        protected bool HasOnMetadataProcessed
        {
            get
            {
                return OnMetadataProcessed != null;
            }
        }

        /// <summary>
        /// Stores a <see cref="FileLoadData"></see> reference list for callbacks.
        /// </summary>
        public static ConcurrentList<FileLoadData> FilesLoadData = new ConcurrentList<FileLoadData>();

        /// <summary>
        /// Triggers the on metadata processed event.
        /// </summary>
        /// <param name="metadataType">Metadata type.</param>
        /// <param name="metadataIndex">Metadata index.</param>
        /// <param name="metadataKey">Metadata key.</param>
        /// <param name="metadataValue">Metadata value.</param>
        protected void TriggerOnMetadataProcessed(AssimpMetadataType metadataType, uint metadataIndex, string metadataKey, object metadataValue)
        {
            if (OnMetadataProcessed != null)
            {
                OnMetadataProcessed(metadataType, metadataIndex, metadataKey, metadataValue);
            }
        }

        /// <summary>
        /// Checks whether the given file extension is supported.
        /// </summary>
        /// <returns><c>true</c>, if the extension is supported. Otherwise, <c>false</c>.</returns>
        public static bool IsExtensionSupported(string extension)
        {
            return AssimpInterop.ai_IsExtensionSupported(extension);
        }

        /// <summary>
        /// Returns a string of supported file extensions.
        /// </summary>
        /// <returns>Supported file extensions.</returns>
        public static string GetSupportedFileExtensions()
        {
            string supportedFileExtensions;
            AssimpInterop.ai_GetExtensionList(out supportedFileExtensions);
            return supportedFileExtensions;
        }

        /// <summary>
        /// Ensure all materials are loaded when calling AssetLoader statically.
        /// </summary>
        static AssetLoaderBase()
        {
            LoadAllStandardMaterials();
        }

        /// <summary>
        /// Tries to load all TriLib base resources.
        /// @warning To ensure TriLib works properly, don't forget to import TriLib 'Resources' folder to the project.
        /// </summary>
        private static void LoadAllStandardMaterials()
        {
            if (!LoadNotFoundTexture())
            {
#if UNITY_EDITOR
                BuildNotFoundTexture();
#else
                throw new Exception("Please import 'NotFound' asset from TriLib package 'TriLib\\Resources' to the project.");
#endif
            }
            if (!LoadNormalBaseTexture())
            {
                throw new Exception("Please import 'NormalBase.png' asset from TriLib package 'TriLib\\Resources' to the project.");
            }
            if (!LoadStandardMaterials())
            {
#if UNITY_EDITOR
                BuildDefaultMaterials();
#else
                throw new Exception("Please import all material assets from TriLib package 'TriLib\\Resources' to the project.");
#endif
            }
        }

        /// <summary>
        /// Tries to load all TriLib standard base materials.
        /// </summary>
        /// <returns><c>true</c> if all materials have been found. Otherwise, <c>false</c></returns>
        private static bool LoadStandardMaterials()
        {
            if (StandardBaseMaterial == null)
            {
                StandardBaseMaterial = Resources.Load("StandardMaterial") as Material;
            }
            if (StandardSpecularMaterial == null)
            {
                StandardSpecularMaterial = Resources.Load("StandardSpecularMaterial") as Material;
            }
            if (StandardBaseAlphaMaterial == null)
            {
                StandardBaseAlphaMaterial = Resources.Load("StandardBaseAlphaMaterial") as Material;
            }
            if (StandardSpecularAlphaMaterial == null)
            {
                StandardSpecularAlphaMaterial = Resources.Load("StandardSpecularAlphaMaterial") as Material;
            }
            if (StandardBaseCutoutMaterial == null)
            {
                StandardBaseCutoutMaterial = Resources.Load("StandardBaseCutoutMaterial") as Material;
            }
            if (StandardSpecularCutoutMaterial == null)
            {
                StandardSpecularCutoutMaterial = Resources.Load("StandardSpecularCutoutMaterial") as Material;
            }
            return StandardBaseMaterial != null && StandardSpecularMaterial != null && StandardBaseAlphaMaterial != null && StandardSpecularAlphaMaterial != null && StandardBaseCutoutMaterial != null && StandardSpecularCutoutMaterial != null;
        }

        /// <summary>
        /// Loads the <see cref="UnityEngine.Texture"/> resource to show in case of unknown textures.
        /// @warning Don´t remove the __NotFound.asset_ included in the package.
        /// </summary>
        private static bool LoadNotFoundTexture()
        {
            if (NotFoundTexture == null)
            {
                NotFoundTexture = Resources.Load("NotFound") as Texture2D;
            }
            return NotFoundTexture != null;
        }

        /// <summary>
        /// Loads the <see cref="UnityEngine.Texture"/> used as base for normal map loading.
        /// @warning Don´t remove the __NormalBase.png_ included in the package.
        /// </summary>
        private static bool LoadNormalBaseTexture()
        {
            if (NormalBaseTexture == null)
            {
                NormalBaseTexture = Resources.Load("NormalBase") as Texture2D;
            }
            return NormalBaseTexture != null;
        }

        /// <summary>
        /// Internally loads a file from memory into its data representation.
        /// </summary>
        /// <param name="fileBytes">File data to load.</param>
        /// <param name="filename">Filename, in case it doesn't exist, the file extension should be used (eg: ".FBX").</param>
        /// <param name="basePath">Base path from the loaded file.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions" /> used to load the file.</param>
        /// <param name="usesWrapperGameObject">Pass <c>true</c> when using a wrapper <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="dataCallback">Custom resource data retrieval callback. Pass this parameter when you need to load external data while loading from memory.</param>
        /// <param name="existsCallback">Custom resource size retrieval callback. Pass this parameter when you need to load external data while loading from memory.</param>
        /// <param name="loadTextureDataCallback">Pass this callback to load texture data from custom sources.</param>
        protected void InternalLoadFromMemory(byte[] fileBytes, string filename, string basePath, AssetLoaderOptions options = null, bool usesWrapperGameObject = false, AssimpInterop.DataCallback dataCallback = null, AssimpInterop.ExistsCallback existsCallback = null, LoadTextureDataCallback loadTextureDataCallback = null)
        {
            var fileLoadData = new GCFileLoadData()
            {
                Filename = filename,
                BasePath = basePath
            };
            var fileId = FilesLoadData.Count;
            FilesLoadData.Add(fileLoadData);
            IntPtr scene;
            try
            {
                var extension = File.Exists(filename) ? Path.GetExtension(filename) : filename;
                scene = ImportFileFromMemory(fileBytes, extension, options, dataCallback ?? DefaultDataCallback, existsCallback ?? DefaultExistsCallback, fileId);
            }
            catch (Exception exception)
            {
                throw new Exception("Error parsing file.", exception);
            }
            if (scene == IntPtr.Zero)
            {
                var error = AssimpInterop.ai_GetErrorString();
                throw new Exception(string.Format("Error loading asset. Assimp returns: [{0}]", error));
            }
            LoadInternal(scene, basePath, options, usesWrapperGameObject, loadTextureDataCallback);            
            FilesLoadData[fileId] = null;
            AssimpInterop.ai_ReleaseImport(scene);
        }

        /// <summary>
        /// Internally loads a model from memory with custom embedded texture loading for ZIP files.
        /// </summary>
        /// <param name="data">Loaded file data.</param>
        /// <param name="assetExtension">Loaded file extension.</param>
        /// <param name="basePath">Base path from the loaded file.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to load the file.</param>
        /// <param name="usesWrapperGameObject">Pass <c>true</c> when using a wrapper <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="dataCallback">Custom resource data retrieval callback. Pass this parameter when you need to load external data while loading from memory.</param>
        /// <param name="existsCallback">Custom resource size retrieval callback. Pass this parameter when you need to load external data while loading from memory.</param>
        /// <param name="loadTextureDataCallback">Pass this callback to load texture data from custom sources.</param>
        /// <returns></returns>
        protected void InternalLoadFromMemoryAndZip(byte[] data, string assetExtension, string basePath, AssetLoaderOptions options = null, bool usesWrapperGameObject = false, AssimpInterop.DataCallback dataCallback = null, AssimpInterop.ExistsCallback existsCallback = null, LoadTextureDataCallback loadTextureDataCallback = null)
        {
#if TRILIB_USE_ZIP
#if !UNITY_EDITOR && (NET_4_6 || NETFX_CORE)
            ZipArchive zipFile = null;
#else
            ZipFile zipFile = null;
#endif
#endif
            if (assetExtension.ToLowerInvariant() == ".zip")
            {
#if TRILIB_USE_ZIP
#if UNITY_EDITOR || !UNITY_WINRT
                ZipConstants.DefaultCodePage = 0;
#endif
                var supportedExtensions = GetSupportedFileExtensions();
                var memoryStream = new MemoryStream(data);
#if !UNITY_EDITOR && (NET_4_6 || NETFX_CORE)
                    zipFile = new ZipArchive(memoryStream, ZipArchiveMode.Read);
                    foreach (ZipArchiveEntry zipEntry in zipFile.Entries)
                    {
#else
                zipFile = new ZipFile(memoryStream);
                foreach (ZipEntry zipEntry in zipFile)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;
                    }
#endif
                    var entryExtension = FileUtils.GetFileExtension(zipEntry.Name);
                    if (supportedExtensions.Contains("*" + entryExtension + ";"))
                    {
#if TRILIB_USE_ZIP
#if !UNITY_EDITOR && (NET_4_6 || NETFX_CORE)
                        var zipStream = zipEntry.Open();
#else
                        var zipStream = zipFile.GetInputStream(zipEntry);
#endif
#endif
                        data = StreamUtils.ReadFullStream(zipStream);
                        assetExtension = entryExtension;
                    }
                }
#else
                throw new Exception("Please enable TRILIB_USE_ZIP to load zip files");
#endif
            }
            EmbeddedTextureLoad += delegate (string path)
            {
#if TRILIB_USE_ZIP
                if (zipFile != null)
                {
                    var fileShortName = FileUtils.GetShortFilename(path).ToLowerInvariant();
#if TRILIB_USE_ZIP
#if !UNITY_EDITOR && (NET_4_6 || NETFX_CORE)
                        foreach (ZipArchiveEntry zipEntry in zipFile.Entries)
                        {
#else
                    foreach (ZipEntry zipEntry in zipFile)
                    {
                        if (!zipEntry.IsFile)
                        {
                            continue;
                        }
#endif
#endif
                        if (FileUtils.GetShortFilename(zipEntry.Name).ToLowerInvariant() == fileShortName)
                        {
#if TRILIB_USE_ZIP
#if !UNITY_EDITOR && (NET_4_6 || NETFX_CORE)
                            var zipStream = zipEntry.Open();
#else
                            var zipStream = zipFile.GetInputStream(zipEntry);
#endif
#endif
                            var uncompressedData = StreamUtils.ReadFullStream(zipStream);
                            var embeddedTextureData = new EmbeddedTextureData();
#if !TRILIB_USE_UNITY_TEXTURE_LOADER
                            embeddedTextureData.Data = STBImageLoader.LoadTextureDataFromByteArray(uncompressedData, out embeddedTextureData.Width, out embeddedTextureData.Height, out embeddedTextureData.NumChannels);
                            embeddedTextureData.IsRawData = true;
#else
                            embeddedTextureData.Data = uncompressedData;
#endif
                            return embeddedTextureData;
                        }
                    }
                }
#endif
                return null;
            };
            InternalLoadFromMemory(data, assetExtension, basePath, options, usesWrapperGameObject, dataCallback, existsCallback, loadTextureDataCallback);
        }

        /// <summary>
        /// Internally loads a file from file system into it's data representation.
        /// </summary>
        /// <param name="filename">Filename to load.</param>
        /// <param name="basePath">Base path from the loaded file.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions" /> used to load the file.</param>
        /// <param name="usesWrapperGameObject">Pass <c>true</c> when using a wrapper <see cref="UnityEngine.GameObject"/>.</param>
        protected void InternalLoadFromFile(string filename, string basePath, AssetLoaderOptions options = null, bool usesWrapperGameObject = false)
        {
            IntPtr scene;
            try
            {
                scene = ImportFile(filename, options);
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Error parsing file: {0}", filename), exception);
            }
            if (scene == IntPtr.Zero)
            {
                var error = AssimpInterop.ai_GetErrorString();
                throw new Exception(string.Format("Error loading asset. Assimp returns: [{0}]", error));
            }
            LoadInternal(scene, basePath, options, usesWrapperGameObject);
            AssimpInterop.ai_ReleaseImport(scene);
        }

        /// <summary>
        /// Builds the <see cref="UnityEngine.GameObject"/> from pre-loaded data.
        /// </summary>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to build the <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="basePath">Base path from the loaded file.</param>
        /// <param name="wrapperGameObject">Wrapper <see cref="UnityEngine.GameObject"/> to build the object into.</param>
        /// <returns>The built <see cref="UnityEngine.GameObject"/>.</returns>
        protected GameObject BuildGameObject(AssetLoaderOptions options, string basePath = null, GameObject wrapperGameObject = null)
        {
            if (HasOnMetadataProcessed && Metadata != null && (options == null || !options.DontLoadMetadata))
            {
                foreach (var metadata in Metadata)
                {
                    ProcessMetadata(metadata);
                }
            }
            if (MaterialData != null && (options == null || !options.DontLoadMaterials))
            {
                LoadAllStandardMaterials();
                LoadedMaterials = new Dictionary<string, Material>();
                LoadedTextures = new Dictionary<string, Texture2D>();
                foreach (var materialData in MaterialData)
                {
                    TransformMaterialData(materialData, options, basePath);
                }
            }
            if (MeshData != null && (options == null || !options.DontLoadMeshes))
            {
                foreach (var meshData in MeshData)
                {
                    TransformMeshData(meshData, options);
                }
            }
            GameObject gameObject = null;
            if (RootNodeData != null)
            {
                gameObject = TransformNodeData(RootNodeData, options, wrapperGameObject);
                if (gameObject != null)
                {
                    if (LoadedBoneNames != null && LoadedBoneNames.Count > 0)
                    {
                        SetupSkinnedMeshRendererTransforms(gameObject);
                    }
                    if (options != null)
                    {
                        LoadContextOptions(gameObject, options);
                    }
                }
            }
            if (AnimationData != null && (options == null || !options.DontLoadAnimations))
            {
                foreach (var animationData in AnimationData)
                {
                    TransformAnimationData(animationData, options);
                }
            }
            if (gameObject != null)
            {
                if (AnimationData != null && (options == null || !options.DontApplyAnimations))
                {
                    SetupAnimations(wrapperGameObject ?? gameObject, options);
                }
                if (CameraData != null && (options == null || !options.DontLoadCameras))
                {
                    foreach (var cameraData in CameraData)
                    {
                        TransformCameraData(gameObject, cameraData, options);
                    }
                }
                if (options != null && options.AddAssetUnloader)
                {
                    gameObject.AddComponent<AssetUnloader>();
                }
                if (HasOnObjectLoaded)
                {
                    TriggerOnObjectLoaded(gameObject);
                }
            }
            return gameObject;
        }

        /// <summary>
        /// Setups the <see cref="UnityEngine.SkinnedMeshRenderer"/> bone transforms.
        /// </summary>
        /// <param name="gameObject"><see cref="UnityEngine.GameObject"/> where the bones will be searched.</param>
        protected virtual void SetupSkinnedMeshRendererTransforms(GameObject gameObject)
        {
            foreach (var loadedSkinnedMeshRenderer in LoadedBoneNames)
            {
                var skinnedMeshRenderer = loadedSkinnedMeshRenderer.Key;
                var boneNames = loadedSkinnedMeshRenderer.Value;
                var boneCount = boneNames.Count;
                var transforms = new List<Transform>(boneCount);
                for (var i = 0; i < boneCount; i++)
                {
                    var transform = gameObject.transform.FindDeepChild(boneNames[i]);
                    if (transform == null)
                    {
                        continue;
                    }
                    transforms.Add(transform);
                }
                skinnedMeshRenderer.bones = transforms.ToArray();
            }
        }

        /// <summary>
        /// Applies transform from <see cref="AssetLoaderOptions"/> into given <see cref="UnityEngine.GameObject" />.
        /// </summary>
        /// <param name="gameobject"><see cref="UnityEngine.GameObject" /> to transform.</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the transform.</param>
        private static void LoadContextOptions(GameObject gameobject, AssetLoaderOptions options)
        {
            gameobject.transform.rotation = Quaternion.Euler(options.RotationAngles);
            gameobject.transform.localScale = Vector3.one * options.Scale;
        }

        /// <summary>
        /// Processes the given metadata by triggering the OnMetadataProcessed event.
        /// </summary>
        /// <param name="metadata"><see cref="TriLib.AssimpMetadata"/> to proccess.</param>
        protected virtual void ProcessMetadata(AssimpMetadata metadata)
        {
            TriggerOnMetadataProcessed(metadata.MetadataType, metadata.MetadataIndex, metadata.MetadataKey, metadata.MetadataValue);
        }

        /// <summary>
        /// Setups animation components and <see cref="UnityEngine.AnimationClip"/> clips into the given <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        /// <param name="gameObject"><see cref="UnityEngine.GameObject"/> to add the component to.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to process the components.</param>
        protected virtual void SetupAnimations(GameObject gameObject, AssetLoaderOptions options)
        {
            if (options == null || options.UseLegacyAnimations)
            {
                var unityAnimation = gameObject.GetComponent<Animation>();
                if (unityAnimation == null)
                {
                    unityAnimation = gameObject.AddComponent<Animation>();
                }
                AnimationClip defaultClip = null;
                for (var c = 0; c < AnimationData.Length; c++)
                {
                    var unityAnimationClip = AnimationData[c].AnimationClip;
                    if (unityAnimationClip == null)
                    {
                        continue;
                    }
                    unityAnimation.AddClip(unityAnimationClip, unityAnimationClip.name);
                    if (c == 0)
                    {
                        defaultClip = unityAnimationClip;
                    }
                }
                unityAnimation.clip = defaultClip;
                if (options == null || options.AutoPlayAnimations)
                {
                    unityAnimation.Play();
                }
            }
            else
            {
                var unityAnimator = gameObject.GetComponent<Animator>();
                if (unityAnimator == null)
                {
                    unityAnimator = gameObject.AddComponent<Animator>();
                }
                if (options.AnimatorController != null)
                {
                    unityAnimator.runtimeAnimatorController = options.AnimatorController;
                }
                if (!options.DontGenerateAvatar)
                {
                    unityAnimator.avatar = options.Avatar != null ? options.Avatar : AvatarBuilder.BuildGenericAvatar(gameObject, "");
                }
            }
        }

        /// <summary>
        /// Transforms given <see cref="TriLib.NodeData"/> into a <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        /// <param name="nodeData"><see cref="TriLib.NodeData"/> to be transformed.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to transform the data.</param>
        /// <param name="existingGameObject"><see cref="UnityEngine.GameObject"> used to add the components to instead of adding to a new object.</see></param>
        /// <returns></returns>
        protected virtual GameObject TransformNodeData(NodeData nodeData, AssetLoaderOptions options, GameObject existingGameObject = null)
        {
            GameObject gameObject = new GameObject { name = nodeData.Name };
           // GameObject gameObject = new GameObject();
            var parentGameObject = existingGameObject != null ? existingGameObject : (nodeData.Parent == null ? null : nodeData.Parent.GameObject);
            if (parentGameObject != null)
            {
                gameObject.transform.SetParent(parentGameObject.transform, false);
            }
            gameObject.transform.LoadMatrix(nodeData.Matrix);
            if (nodeData.Meshes != null && nodeData.Meshes.Length > 0 && MeshData != null && MeshData.Length > 0)
            {
                var vertexCount = 0;
                foreach (var meshIndex in nodeData.Meshes)
                {
                    var meshData = MeshData[meshIndex];
                    vertexCount += meshData.Vertices.Length;
                }
#if UNITY_2017_3_OR_NEWER
                var useCombineInstances = options == null || options.Use32BitsIndexFormat && options.CombineMeshes || vertexCount < 65536 && options.CombineMeshes;
#else
                var useCombineInstances = vertexCount < 65536 && (options == null || options.CombineMeshes);
#endif
                if (useCombineInstances)
                {
                    Material lastMaterial = null;
                    List<string> combinedNodeNames = null;
                    var singleMaterial = true;
                    var combineInstances = new CombineInstance[nodeData.Meshes.Length];
                    var combinedMaterials = new Material[nodeData.Meshes.Length];
                    for (var i = 0; i < nodeData.Meshes.Length; i++)
                    {
                        var meshIndex = nodeData.Meshes[i];
                        if (meshIndex >= MeshData.Length)
                        {
                            continue;
                        }
                        var meshData = MeshData[meshIndex];
                        if (meshData.HasBoneInfo && meshData.BoneNames.Length > 0)
                        {
                            if (combinedNodeNames == null)
                            {
                                combinedNodeNames = new List<string>();
                            }
                            combinedNodeNames.AddRange(meshData.BoneNames);
                        }
                        var combineInstance = new CombineInstance
                        {
                            mesh = meshData.Mesh,
                            transform = Matrix4x4.identity
                        };
                        combineInstances[i] = combineInstance;
                        if (MaterialData == null || MaterialData.Length == 0 || meshData.MaterialIndex >= MaterialData.Length)
                        {
                            continue;
                        }
                        var materialData = MaterialData[meshData.MaterialIndex];
                        var material = materialData.Material;
                        if (lastMaterial != null && material != lastMaterial)
                        {
                            singleMaterial = false;
                        }
                        combinedMaterials[i] = material;
                        lastMaterial = material;
                    }
                    var combinedMesh = new Mesh();
#if UNITY_2017_3_OR_NEWER
                    if (options == null || options.Use32BitsIndexFormat)
                    {
                        combinedMesh.indexFormat = IndexFormat.UInt32;
                    }
#endif
                    combinedMesh.CombineMeshes(combineInstances, singleMaterial);
                    combinedMesh.name = FixName(nodeData.Name) + "_Mesh";
                    CreateMeshComponents(gameObject, options, combinedMesh, HasBoneInfo, combinedMaterials, combinedNodeNames, singleMaterial ? combinedMaterials[0] : null);
                }
                else
                {
                    for (var i = 0; i < nodeData.Meshes.Length; i++)
                    {
                        var meshIndex = nodeData.Meshes[i];
                        var meshData = MeshData[meshIndex];
                        var material = MaterialData == null ? null : MaterialData[meshData.MaterialIndex].Material;
                        var subGameObject = new GameObject { name = "SubMesh_" + i };
                        subGameObject.transform.SetParent(gameObject.transform, false);
                        CreateMeshComponents(subGameObject, options, meshData.Mesh, HasBoneInfo, null, meshData.BoneNames, material);
                    }
                }

            }
            nodeData.GameObject = gameObject;
            if (nodeData.Children != null)
            {
                foreach (var childNodeData in nodeData.Children)
                {
                    TransformNodeData(childNodeData, options);
                }
            }
            return gameObject;
        }

        /// <summary>
        /// Creates mesh rendering components into given <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        /// <param name="gameObject"><see cref="UnityEngine.GameObject"/> to create the components at.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to create the components.</param>
        /// <param name="mesh"><see cref="UnityEngine.Mesh"/> to add to the created component.</param>
        /// <param name="hasBoneInfo">If <c>true</c>, creates a <see cref="UnityEngine.SkinnedMeshRenderer"/>, otherwise, creates a <see cref="UnityEngine.MeshRenderer"/>.</param>
        /// <param name="combinedMaterials"><see cref="UnityEngine.Material"/> list to assign to the created component.</param>
        /// <param name="boneNames">Bone names loaded for the given component.</param>
        /// <param name="singleMaterial">Single <see cref="UnityEngine.Material"/> to assign to the component.</param>
        protected virtual void CreateMeshComponents(GameObject gameObject, AssetLoaderOptions options, Mesh mesh,
            bool hasBoneInfo, Material[] combinedMaterials, IList<string> boneNames = null, Material singleMaterial = null)
        {
            if (hasBoneInfo)
            {
                var skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
                skinnedMeshRenderer.sharedMesh = mesh;
                skinnedMeshRenderer.quality = SkinQuality.Bone4;
                if (boneNames != null)
                {
                    if (LoadedBoneNames == null)
                    {
                        LoadedBoneNames = new Dictionary<SkinnedMeshRenderer, IList<string>>();
                    }
                    LoadedBoneNames.Add(skinnedMeshRenderer, boneNames);
                }
                if (singleMaterial != null)
                {
                    skinnedMeshRenderer.sharedMaterial = singleMaterial;
                }
                else
                {
                    skinnedMeshRenderer.sharedMaterials = combinedMaterials;
                }
            }
            else
            {
                var meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                var meshRenderer = gameObject.AddComponent<MeshRenderer>();
                if (singleMaterial != null)
                {
                    meshRenderer.sharedMaterial = singleMaterial;
                }
                else
                {
                    meshRenderer.sharedMaterials = combinedMaterials;
                }

                if (options != null && options.GenerateMeshColliders)
                {
                    var meshCollider = gameObject.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = mesh;
                    meshCollider.convex = options.ConvexMeshColliders;
                }
            }
            if (HasOnMeshCreated)
            {
                TriggerOnMeshCreated(0, mesh);
            }
        }

        /// <summary>
        /// Transforms the given <see cref="TriLib.CameraData"/> into a <see cref="UnityEngine.Camera"/>.
        /// </summary>
        /// <param name="gameObject"><see cref="UnityEngine.GameObject"/> to add the <see cref="UnityEngine.Camera"/> component into.</param>
        /// <param name="cameraData"><see cref="TriLib.CameraData"/> to be transformed.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to transform the camera.</param>
        protected virtual void TransformCameraData(GameObject gameObject, CameraData cameraData, AssetLoaderOptions options)
        {
            var cameraTransform = gameObject.transform.FindDeepChild(cameraData.Name);
            if (cameraTransform == null)
            {
                return;
            }
            var camera = cameraTransform.gameObject.AddComponent<Camera>();
            camera.aspect = cameraData.Aspect;
            camera.nearClipPlane = cameraData.NearClipPlane;
            camera.farClipPlane = cameraData.FarClipPlane;
            camera.fieldOfView = cameraData.FieldOfView;
            camera.transform.localPosition = cameraData.LocalPosition;
            camera.transform.LookAt(cameraData.Forward, cameraData.Up);
            cameraData.Camera = camera;
        }

        /// <summary>
        /// Fixes animation curve length issues (curves containing only one key or with length too small).
        /// </summary>
        /// <param name="animationLength">Final animation length.</param>
        /// <param name="curve">Curve to fix.</param>
        /// <returns>Fixed curve.</returns>
        private static AnimationCurve FixCurve(float animationLength, AnimationCurve curve)
        {
            if (Mathf.Approximately(animationLength, 0f))
            {
                animationLength = 1f;
            }
            if (curve.keys.Length == 1)
            {
                curve.AddKey(new Keyframe(animationLength, curve.keys[0].value));
            }
            return curve;
        }

        /// <summary>
        /// Transforms the given <see cref="TriLib.AnimationData"/> into a <see cref="UnityEngine.AnimationClip"/>.
        /// </summary>
        /// <param name="animationData"><see cref="TriLib.AnimationData"/> to be transformed.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to transform the animation.</param>
        protected virtual void TransformAnimationData(AnimationData animationData, AssetLoaderOptions options)
        {
            var animationClip = new AnimationClip
            {
                name = animationData.Name,
                frameRate = animationData.FrameRate,
                wrapMode = animationData.WrapMode,
                legacy = animationData.Legacy
            };
            foreach (var animationChannelData in animationData.ChannelData)
            {
                if (!NodesPath.ContainsKey(animationChannelData.NodeName))
                {
                    continue;
                }
                var nodePath = NodesPath[animationChannelData.NodeName];
                foreach (var animationCurveData in animationChannelData.CurveData)
                {
                    var propertyName = animationCurveData.Key;
                    var curveData = animationCurveData.Value;
                    var animationCurve = FixCurve(animationData.Length, new AnimationCurve { keys = curveData.Keyframes });
                    curveData.AnimationCurve = animationCurve;
                    animationClip.SetCurve(nodePath, typeof(Transform), propertyName, animationCurve);
                }
            }
            if (options != null && options.EnsureQuaternionContinuity)
            {
                animationClip.EnsureQuaternionContinuity();
            }
            if (HasOnAnimationClipCreated)
            {
                TriggerOnAnimationClipCreated(0, animationClip);
            }
            animationData.AnimationClip = animationClip;
        }

        /// <summary>
        /// Transforms the given <see cref="TriLib.MeshData"/> into a <see cref="UnityEngine.Mesh"/>.
        /// </summary>
        /// <param name="meshData"><see cref="TriLib.MeshData"/> to be transformed.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to transform the mesh.</param>
        protected virtual void TransformMeshData(MeshData meshData, AssetLoaderOptions options)
        {
            var mesh = new Mesh();
#if UNITY_2017_3_OR_NEWER
            if (options == null || options.Use32BitsIndexFormat)
            {
                mesh.indexFormat = IndexFormat.UInt32;
            }
#endif
            mesh.name = meshData.Name;
            mesh.vertices = meshData.Vertices;
            mesh.normals = meshData.Normals;
            mesh.uv4 = meshData.Uv3;
            mesh.uv3 = meshData.Uv2;
            mesh.uv2 = meshData.Uv1;
            mesh.uv = meshData.Uv;
            mesh.tangents = meshData.Tangents;
            mesh.colors = meshData.Colors;
            mesh.boneWeights = meshData.BoneWeights;
            mesh.bindposes = meshData.BindPoses;
            mesh.triangles = meshData.Triangles;
            meshData.Mesh = mesh;
        }

        /// <summary>
        /// Transforms the given <see cref="TriLib.MaterialData"/> into a <see cref="UnityEngine.Material"/>.
        /// </summary>
        /// <param name="materialData"><see cref="TriLib.MaterialData"/> to be transformed.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to transform the material.</param>
        /// <param name="basePath">Loaded asset base path.</param>
        protected virtual void TransformMaterialData(MaterialData materialData, AssetLoaderOptions options, string basePath = null)
        {
            var dummy = false;
            var hasAlphaChannelOnTextures = false;

            var diffuseTexture = materialData.DiffuseInfoLoaded ? LoadTextureFromFile(materialData.DiffusePath, materialData.Name, options, materialData.DiffuseEmbeddedTextureData, materialData.DiffuseWrapMode, ref hasAlphaChannelOnTextures, false, basePath, options != null && options.ApplyAlphaMaterials) : null;
            var emissionTexture = materialData.EmissionInfoLoaded ? LoadTextureFromFile(materialData.EmissionPath, materialData.Name, options, materialData.EmissionEmbeddedTextureData, materialData.EmissionWrapMode, ref dummy, false, basePath) : null;
            var specularTexture = materialData.SpecularInfoLoaded ? LoadTextureFromFile(materialData.SpecularPath, materialData.Name, options, materialData.SpecularEmbeddedTextureData, materialData.SpecularWrapMode, ref dummy, false, basePath) : null;
            var normalTexture = materialData.NormalInfoLoaded ? LoadTextureFromFile(materialData.NormalPath, materialData.Name, options, materialData.NormalEmbeddedTextureData, materialData.NormalWrapMode, ref dummy, true, basePath) : null;
            var heightTexture = materialData.HeightInfoLoaded ? LoadTextureFromFile(materialData.HeightPath, materialData.Name, options, materialData.HeightEmbeddedTextureData, materialData.HeightWrapMode, ref dummy, false, basePath) : null;

            var hasAlpha = hasAlphaChannelOnTextures || materialData.AlphaLoaded && materialData.Alpha != 1f;
            var hasSpecular = materialData.SpecularColorLoaded || !string.IsNullOrEmpty(materialData.SpecularPath);

            var material = LoadMaterial(materialData.Name, options, hasAlpha, hasSpecular);
            material.SetTexture("_MainTex", diffuseTexture);
            if (materialData.DiffuseColorLoaded)
            {
                var color = materialData.DiffuseColor;
                if (materialData.AlphaLoaded)
                {
                    color.a = materialData.Alpha;
                }
                material.SetColor("_Color", color);
            }
            material.SetTexture("_EmissionMap", emissionTexture);
            if (materialData.EmissionColorLoaded)
            {
                material.SetColor("_EmissionColor", materialData.EmissionColor);
            }
            material.SetTexture("_SpecGlossMap", specularTexture);
            if (materialData.SpecularColorLoaded)
            {
                material.SetColor("_SpecColor", materialData.SpecularColor);
            }
            material.SetTexture("_BumpMap", normalTexture);
            material.SetTexture("_Displacement", heightTexture);
            if (materialData.BumpScaleLoaded)
            {
                material.SetFloat("_BumpScale", materialData.BumpScale);
            }
            if (materialData.GlossinessLoaded)
            {
                material.SetFloat("_Glossiness", materialData.Glossiness);
            }
            if (materialData.GlossMapScaleLoaded)
            {
                material.SetFloat("_GlossMapScale", materialData.GlossMapScale);
            }
            materialData.Material = material;
        }

        /// <summary>
        /// Creates a new <see cref="UnityEngine.Material"/> or loads an existing <see cref="UnityEngine.Material"/>  with the given name.
        /// </summary>
        /// <param name="name">Material name.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to load the material.</param>
        /// <param name="hasAlpha">If <c>true</c>, creates/loads an alpha material.</param>
        /// <param name="hasSpecular">If <c>true</c>, creates/loads a specular material.</param>
        /// <returns>The created/loaded <see cref="UnityEngine.Material"/>.</returns>
        protected virtual Material LoadMaterial(string name, AssetLoaderOptions options, bool hasAlpha, bool hasSpecular)
        {
            Material material;
            if (LoadedMaterials.ContainsKey(name))
            {
                material = LoadedMaterials[name];
            }
            else
            {
                if (options != null && !options.DisableAlphaMaterials && hasAlpha)
                {
                    material = options.UseCutoutMaterials
                        ? new Material(options.UseStandardSpecularMaterial && hasSpecular
                            ? StandardSpecularCutoutMaterial
                            : StandardBaseCutoutMaterial)
                        : new Material(options.UseStandardSpecularMaterial && hasSpecular
                            ? StandardSpecularAlphaMaterial
                            : StandardBaseAlphaMaterial);
                }
                else
                {
                    material = new Material(options != null && options.UseStandardSpecularMaterial && hasSpecular
                        ? StandardSpecularMaterial
                        : StandardBaseMaterial);
                }
                material.name = name;
                LoadedMaterials.Add(name, material);
            }
            if (HasOnMaterialCreated)
            {
                TriggerOnMaterialCreated(0, false, material);
            }
            return material;
        }

        /// <summary>
        /// Creates a new <see cref="UnityEngine.Texture2D"/> or loads an existing <see cref="UnityEngine.Texture2D"/> with the given path.
        /// </summary>
        /// <param name="path">Path to load the texture from.</param>
        /// <param name="name">Texture name.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to load the texture.</param>
        /// <param name="embeddedTextureData"><see cref="TriLib.EmbeddedTextureData"/> to load the texture from.</param>
        /// <param name="textureWrapMode"><see cref="UnityEngine.TextureWrapMode"/> to assign to the texture.</param>
        /// <param name="hasAlphaChannel">Setted to <c>true</c> when the texture has alpha pixels.</param>
        /// <param name="isNormalMap">If <c>true</c>, applies special processing to the texture and treat it as a normal map.</param>
        /// <param name="basePath">Base path to lookup the texture file.</param>
        /// <param name="checkAlphaChannel">If <c>true</c>, checks for any alpha pixel on loaded texture and assigns the value back to this variable.</param>
        /// <returns>The created/loaded <see cref="UnityEngine.Texture2D"/></returns>
        protected virtual Texture2D LoadTextureFromFile(string path, string name, AssetLoaderOptions options, EmbeddedTextureData embeddedTextureData, TextureWrapMode textureWrapMode, ref bool hasAlphaChannel, bool isNormalMap, string basePath, bool checkAlphaChannel = false)
        {
            Texture2D texture;
            if (LoadedTextures.ContainsKey(path))
            {
                texture = LoadedTextures[path];
            }
            else
            {
                if (!checkAlphaChannel && embeddedTextureData != null)
                {
                    hasAlphaChannel = embeddedTextureData.NumChannels == 4;
                }
                texture = Texture2DUtils.ProcessTexture(embeddedTextureData != null ? embeddedTextureData.Width : 0,
                    embeddedTextureData != null ? embeddedTextureData.Height : 0, name,
                    ref hasAlphaChannel, embeddedTextureData != null ? embeddedTextureData.Data : null,
                    embeddedTextureData != null && embeddedTextureData.IsRawData, isNormalMap, textureWrapMode, options != null ? options.TextureCompression : TextureCompression.NormalQuality, checkAlphaChannel, options == null || options.GenerateMipMaps);
                if (texture != null)
                {
                    texture.name = name;
                    LoadedTextures.Add(path, texture);
                }
            }
            if (texture != null && HasOnTextureLoaded)
            {
                TriggerOnTextureLoaded(path, null, null, texture);
            }
            return texture;
        }

        /// <summary>
        /// Gets the default post process steps.
        /// </summary>
        /// <returns>The default post process steps.</returns>
        private static uint GetDefaultPostProcessSteps()
        {
            return (uint)(AssimpPostProcessSteps.FlipWindingOrder | AssimpPostProcessSteps.MakeLeftHanded | AssimpProcessPreset.TargetRealtimeMaxQuality);
        }

        /// <summary>
        /// Builds a property store used to pass advanced configs to native plugins.
        /// </summary>
        /// <param name="options">Input options.</param>
        /// <returns>The property store native pointer.</returns>
        private static IntPtr BuildPropertyStore(AssetLoaderOptions options)
        {
            var propertyStore = AssimpInterop.ai_CreatePropertyStore();
            foreach (var advancedConfig in options.AdvancedConfigs)
            {
                AssetAdvancedConfigType assetAdvancedConfigType;
                string className;
                string description;
                string group;
                bool hasDefaultValue;
                bool hasMinValue;
                bool hasMaxValue;
                object defaultValue;
                object minValue;
                object maxValue;
                AssetAdvancedPropertyMetadata.GetOptionMetadata(advancedConfig.Key, out assetAdvancedConfigType, out className, out description, out group, out defaultValue, out minValue, out maxValue, out hasDefaultValue, out hasMinValue, out hasMaxValue);
                switch (assetAdvancedConfigType)
                {
                    case AssetAdvancedConfigType.AiComponent:
                        AssimpInterop.ai_SetImportPropertyInteger(propertyStore, advancedConfig.Key, advancedConfig.IntValue << 1);
                        break;
                    case AssetAdvancedConfigType.AiPrimitiveType:
                        AssimpInterop.ai_SetImportPropertyInteger(propertyStore, advancedConfig.Key, advancedConfig.IntValue << 1);
                        break;
                    case AssetAdvancedConfigType.AiUVTransform:
                        AssimpInterop.ai_SetImportPropertyInteger(propertyStore, advancedConfig.Key, advancedConfig.IntValue << 1);
                        break;
                    case AssetAdvancedConfigType.Bool:
                        AssimpInterop.ai_SetImportPropertyInteger(propertyStore, advancedConfig.Key, advancedConfig.BoolValue ? 1 : 0);
                        break;
                    case AssetAdvancedConfigType.Integer:
                        AssimpInterop.ai_SetImportPropertyInteger(propertyStore, advancedConfig.Key, advancedConfig.IntValue);
                        break;
                    case AssetAdvancedConfigType.Float:
                        AssimpInterop.ai_SetImportPropertyFloat(propertyStore, advancedConfig.Key, advancedConfig.FloatValue);
                        break;
                    case AssetAdvancedConfigType.String:
                        AssimpInterop.ai_SetImportPropertyString(propertyStore, advancedConfig.Key, advancedConfig.StringValue);
                        break;
                    case AssetAdvancedConfigType.AiMatrix:
                        AssimpInterop.ai_SetImportPropertyMatrix(propertyStore, advancedConfig.Key, advancedConfig.TranslationValue, advancedConfig.RotationValue, advancedConfig.ScaleValue);
                        break;
                }
            }
            return propertyStore;
        }

        /// <summary>
        /// Imports the file based on given options and returns the Assimp scene native pointer.
        /// </summary>
        /// <param name="fileBytes">File data used to load the <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="fileHint">File format hint. Eg: ".fbx".</param>
        /// <param name="dataCallback">Callback used to retieve file data from C#.</param>
        /// <param name="existsCallback">Callback used to retrieve file size from C#.</param>
        /// <param name="fileId">Generated <see cref="FileLoadData"/> id.</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the object.</param>
        /// <returns>Assimp scene pointer.</returns>
        private static IntPtr ImportFileFromMemory(byte[] fileBytes, string fileHint, AssetLoaderOptions options, AssimpInterop.DataCallback dataCallback, AssimpInterop.ExistsCallback existsCallback, int fileId)
        {
            IntPtr scene;
            if (options != null && options.AdvancedConfigs != null)
            {
                var propertyStore = BuildPropertyStore(options);
                scene = AssimpInterop.ai_ImportFileFromMemoryWithProperties(fileBytes, (uint)options.PostProcessSteps, fileHint, propertyStore, dataCallback, existsCallback, fileId);
                AssimpInterop.ai_CreateReleasePropertyStore(propertyStore);
            }
            else
            {
                scene = AssimpInterop.ai_ImportFileFromMemory(fileBytes, options == null ? GetDefaultPostProcessSteps() : (uint)options.PostProcessSteps, fileHint, dataCallback, existsCallback, fileId);
            }
            return scene;
        }

        /// <summary>
        /// Imports the file based on given options and returns the Assimp scene native pointer.
        /// </summary>
        /// <param name="filename">Filename used to load the <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the object.</param>
        /// <returns>Assimp scene pointer.</returns>
        private static IntPtr ImportFile(string filename, AssetLoaderOptions options)
        {
            IntPtr scene;
            if (options != null && options.AdvancedConfigs != null)
            {
                var propertyStore = BuildPropertyStore(options);
                scene = AssimpInterop.ai_ImportFileEx(filename, (uint)options.PostProcessSteps, IntPtr.Zero, propertyStore);
                AssimpInterop.ai_CreateReleasePropertyStore(propertyStore);
            }
            else
            {
                scene = AssimpInterop.ai_ImportFile(filename, options == null ? GetDefaultPostProcessSteps() : (uint)options.PostProcessSteps);
            }
            return scene;
        }

        /// <summary>
        /// Processes the importing and creates the internal data representation.
        /// </summary>
        /// <param name="scene">Assimp scene pointer.</param>
        /// <param name="basePath">Base model path.</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the object.</param>
        /// <param name="usesWrapperGameObject">Pass <c>true</c> when using a wrapper <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="loadTextureDataCallback">Pass this callback to load texture data from custom sources.</param>
        private void LoadInternal(IntPtr scene, string basePath, AssetLoaderOptions options, bool usesWrapperGameObject = false, LoadTextureDataCallback loadTextureDataCallback = null)
        {
            Dispose();
            BuildMetadata(scene);
            if (AssimpInterop.aiScene_HasMaterials(scene) && (options == null || !options.DontLoadMaterials))
            {
                MaterialData = new MaterialData[AssimpInterop.aiScene_GetNumMaterials(scene)];
                BuildMaterials(scene, basePath, loadTextureDataCallback);
            }
            if (AssimpInterop.aiScene_HasMeshes(scene) && (options == null || !options.DontLoadMeshes))
            {
                MeshData = new MeshData[AssimpInterop.aiScene_GetNumMeshes(scene)];
                BuildMeshes(scene);
                BuildBones(scene);
            }
            if (AssimpInterop.aiScene_HasAnimation(scene) && (options == null || !options.DontLoadAnimations))
            {
                AnimationData = new AnimationData[AssimpInterop.aiScene_GetNumAnimations(scene)];
                BuildAnimations(scene, options);
            }
            if (AssimpInterop.aiScene_HasCameras(scene) && (options == null || !options.DontLoadCameras))
            {
                CameraData = new CameraData[AssimpInterop.aiScene_GetNumCameras(scene)];
                BuildCameras(scene);
            }
            BuildObjects(scene, options, usesWrapperGameObject);
        }

        /// <summary>
        /// Builds the root <see cref="TriLib.NodeData"/>.
        /// </summary>
        /// <param name="scene">Assimp scene pointer.</param>
		/// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to proccess the <see cref="TriLib.NodeData"/>.</param>
        /// <param name="usesWrapperGameObject">Pass <c>true</c> when using a wrapper <see cref="UnityEngine.GameObject"/>.</param>
        private void BuildObjects(IntPtr scene, AssetLoaderOptions options, bool usesWrapperGameObject = false)
        {
            NodesPath = new Dictionary<string, string>();
            var rootNode = AssimpInterop.aiScene_GetRootNode(scene);
            RootNodeData = BuildObject(RootNodeData, rootNode, options, usesWrapperGameObject);
        }

        /// <summary>
        /// Builds the a new <see cref="TriLib.NodeData"/> from the given Assimp node pointer.
        /// </summary>
        /// <param name="parentNodeData">Parent <see cref="TriLib.NodeData"/>, if exists.</param>
        /// <param name="node">Assimp node pointer.</param>
        /// <param name="options"><see cref="TriLib.AssetLoaderOptions"/> used to proccess the <see cref="TriLib.NodeData"/>.</param>
        /// <param name="usesWrapperGameObject">Pass <c>true</c> when using a wrapper <see cref="UnityEngine.GameObject"/>.</param>
        /// <returns>The built <see cref="TriLib.NodeData"/>.</returns>
        private NodeData BuildObject(NodeData parentNodeData, IntPtr node, AssetLoaderOptions options, bool usesWrapperGameObject = false)
        {
            var nodeId = NodeId++;
            var nodeName = AssimpInterop.aiNode_GetName(node);
            var fixedNodeName = FixNodeName(nodeName, nodeId);
            var matrix = AssimpInterop.aiNode_GetTransformation(node);
            var nodePath = parentNodeData != null ? string.Format(parentNodeData.Path != null ? "{0}/{1}" : "{1}", parentNodeData.Path, nodeName) : usesWrapperGameObject ? string.Format("{0}", nodeName) : null;
            NodesPath.Add(fixedNodeName, nodePath);
            var meshCount = AssimpInterop.aiNode_GetNumMeshes(node);
            var nodeData = new NodeData
            {
                Name = fixedNodeName,
                Path = nodePath,
                Matrix = matrix,
                Meshes = new uint[meshCount],
            };
            for (uint m = 0; m < meshCount; m++)
            {
                var meshIndex = AssimpInterop.aiNode_GetMeshIndex(node, m);
                nodeData.Meshes[m] = meshIndex;
            }
            var childrenCount = AssimpInterop.aiNode_GetNumChildren(node);
            if (childrenCount > 0)
            {
                nodeData.Children = new NodeData[childrenCount];
                for (uint c = 0; c < childrenCount; c++)
                {
                    var childNode = AssimpInterop.aiNode_GetChildren(node, c);
                    var childNodeData = BuildObject(nodeData, childNode, options, usesWrapperGameObject);
                    childNodeData.Parent = nodeData;
                    nodeData.Children[c] = childNodeData;
                }
            }
            return nodeData;
        }

        /// <summary>
        /// Builds the <see cref="TriLib.AssimpMetadata"/> list for given scene.
        /// </summary>
        /// <param name="scene">Assimp scene pointer.</param>
        private void BuildMetadata(IntPtr scene)
        {
            if (!HasOnMetadataProcessed)
            {
                return;
            }
            var metadataCount = AssimpInterop.aiScene_GetMetadataCount(scene);
            Metadata = new AssimpMetadata[metadataCount];
            for (uint i = 0; i < metadataCount; i++)
            {
                var metadataKey = AssimpInterop.aiScene_GetMetadataKey(scene, i);
                var metadataType = AssimpInterop.aiScene_GetMetadataType(scene, i);
                object metadataValue;
                switch (metadataType)
                {
                    case AssimpMetadataType.AI_BOOL:
                        metadataValue = AssimpInterop.aiScene_GetMetadataBoolValue(scene, i);
                        break;
                    case AssimpMetadataType.AI_INT32:
                        metadataValue = AssimpInterop.aiScene_GetMetadataInt32Value(scene, i);
                        break;
                    case AssimpMetadataType.AI_UINT64:
                        metadataValue = AssimpInterop.aiScene_GetMetadataInt64Value(scene, i);
                        break;
                    case AssimpMetadataType.AI_FLOAT:
                        metadataValue = AssimpInterop.aiScene_GetMetadataFloatValue(scene, i);
                        break;
                    case AssimpMetadataType.AI_DOUBLE:
                        metadataValue = AssimpInterop.aiScene_GetMetadataDoubleValue(scene, i);
                        break;
                    case AssimpMetadataType.AI_AIVECTOR3D:
                        metadataValue = AssimpInterop.aiScene_GetMetadataVectorValue(scene, i);
                        break;
                    default:
                        metadataValue = AssimpInterop.aiScene_GetMetadataStringValue(scene, i);
                        break;
                }
                Metadata[i] = new AssimpMetadata(metadataType, i, metadataKey, metadataValue);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Builds the not found texture resource.
        /// </summary>
        private static void BuildNotFoundTexture()
        {
            const string notFoundTextureData = "iVBORw0KGgoAAAANSUhEUgAAAAgAAAAICAIAAABLbSncAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAtSURBVBhXY/gPBvYeZ+AIIgKSQObD2Xgl4ABZBUICWRQIUHQgAyJ0oEj8/w8AyzKd+YE5HWsAAAAASUVORK5CYII=";
            NotFoundTexture = new Texture2D(2, 2);
            NotFoundTexture.LoadImage(Convert.FromBase64String(notFoundTextureData));
            AssetDatabase.CreateAsset(NotFoundTexture, "Assets/TriLib/TriLib/Resources/NotFound.asset");
        }

        /// <summary>
        /// Builds the default material resources.
        /// </summary>
        private static void BuildDefaultMaterials()
        {
            //Standard Diffuse & Specular
            StandardBaseMaterial = new Material(Shader.Find("Standard"));
            StandardBaseMaterial.EnableKeyword("_EMISSION");
            StandardBaseMaterial.EnableKeyword("_SPECGLOSSMAP");
            StandardBaseMaterial.EnableKeyword("_NORMALMAP");
            StandardBaseMaterial.SetTexture("_MainTex", NotFoundTexture);
            StandardBaseMaterial.SetTexture("_EmissionMap", NotFoundTexture);
            StandardBaseMaterial.SetTexture("_BumpMap", NormalBaseTexture);
            StandardBaseMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            AssetDatabase.CreateAsset(StandardBaseMaterial, "Assets/TriLib/TriLib/Resources/StandardMaterial.mat");

            StandardSpecularMaterial = new Material(Shader.Find("Standard (Specular setup)"));
            StandardSpecularMaterial.EnableKeyword("_EMISSION");
            StandardSpecularMaterial.EnableKeyword("_SPECGLOSSMAP");
            StandardSpecularMaterial.EnableKeyword("_NORMALMAP");
            StandardSpecularMaterial.SetTexture("_MainTex", NotFoundTexture);
            StandardSpecularMaterial.SetTexture("_EmissionMap", NotFoundTexture);
            StandardSpecularMaterial.SetTexture("_SpecGlossMap", NotFoundTexture);
            StandardSpecularMaterial.SetTexture("_BumpMap", NormalBaseTexture);
            StandardSpecularMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            AssetDatabase.CreateAsset(StandardSpecularMaterial, "Assets/TriLib/TriLib/Resources/StandardSpecularMaterial.mat");

            //Alpha Diffuse & Specular
            StandardBaseAlphaMaterial = new Material(Shader.Find("Standard"));
            StandardBaseAlphaMaterial.SetFloat("_Mode", 3f);
            StandardBaseAlphaMaterial.SetOverrideTag("RenderType", "Transparent");
            StandardBaseAlphaMaterial.SetInt("_SrcBlend", (int)BlendMode.One);
            StandardBaseAlphaMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            StandardBaseAlphaMaterial.SetInt("_ZWrite", 0);
            StandardBaseAlphaMaterial.DisableKeyword("_ALPHATEST_ON");
            StandardBaseAlphaMaterial.DisableKeyword("_ALPHABLEND_ON");
            StandardBaseAlphaMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            StandardBaseAlphaMaterial.renderQueue = (int)RenderQueue.Transparent;
            StandardBaseAlphaMaterial.EnableKeyword("_EMISSION");
            StandardBaseAlphaMaterial.EnableKeyword("_SPECGLOSSMAP");
            StandardBaseAlphaMaterial.EnableKeyword("_NORMALMAP");
            StandardBaseAlphaMaterial.SetTexture("_MainTex", NotFoundTexture);
            StandardBaseAlphaMaterial.SetTexture("_EmissionMap", NotFoundTexture);
            StandardBaseAlphaMaterial.SetTexture("_BumpMap", NormalBaseTexture);
            StandardBaseAlphaMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            AssetDatabase.CreateAsset(StandardBaseAlphaMaterial, "Assets/TriLib/TriLib/Resources/StandardBaseAlphaMaterial.mat");

            StandardSpecularAlphaMaterial = new Material(Shader.Find("Standard (Specular setup)"));
            StandardSpecularAlphaMaterial.SetFloat("_Mode", 3f);
            StandardSpecularAlphaMaterial.SetOverrideTag("RenderType", "Transparent");
            StandardSpecularAlphaMaterial.SetInt("_SrcBlend", (int)BlendMode.One);
            StandardSpecularAlphaMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            StandardSpecularAlphaMaterial.SetInt("_ZWrite", 0);
            StandardSpecularAlphaMaterial.DisableKeyword("_ALPHATEST_ON");
            StandardSpecularAlphaMaterial.DisableKeyword("_ALPHABLEND_ON");
            StandardSpecularAlphaMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            StandardSpecularAlphaMaterial.renderQueue = (int)RenderQueue.Transparent;
            StandardSpecularAlphaMaterial.EnableKeyword("_EMISSION");
            StandardSpecularAlphaMaterial.EnableKeyword("_SPECGLOSSMAP");
            StandardSpecularAlphaMaterial.EnableKeyword("_NORMALMAP");
            StandardSpecularAlphaMaterial.SetTexture("_MainTex", NotFoundTexture);
            StandardSpecularAlphaMaterial.SetTexture("_EmissionMap", NotFoundTexture);
            StandardSpecularAlphaMaterial.SetTexture("_SpecGlossMap", NotFoundTexture);
            StandardSpecularAlphaMaterial.SetTexture("_BumpMap", NormalBaseTexture);
            StandardSpecularAlphaMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            AssetDatabase.CreateAsset(StandardSpecularAlphaMaterial, "Assets/TriLib/TriLib/Resources/StandardSpecularAlphaMaterial.mat");

            //Cutout Diffuse & Specular
            StandardBaseCutoutMaterial = new Material(Shader.Find("Standard"));
            StandardBaseCutoutMaterial.SetFloat("_Mode", 1f);
            StandardBaseCutoutMaterial.SetOverrideTag("RenderType", "TransparentCutout");
            StandardBaseCutoutMaterial.SetInt("_SrcBlend", (int)BlendMode.One);
            StandardBaseCutoutMaterial.SetInt("_DstBlend", (int)BlendMode.Zero);
            StandardBaseCutoutMaterial.SetInt("_ZWrite", 1);
            StandardBaseCutoutMaterial.EnableKeyword("_ALPHATEST_ON");
            StandardBaseCutoutMaterial.DisableKeyword("_ALPHABLEND_ON");
            StandardBaseCutoutMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            StandardBaseCutoutMaterial.renderQueue = (int)RenderQueue.AlphaTest;
            StandardBaseCutoutMaterial.EnableKeyword("_EMISSION");
            StandardBaseCutoutMaterial.EnableKeyword("_SPECGLOSSMAP");
            StandardBaseCutoutMaterial.EnableKeyword("_NORMALMAP");
            StandardBaseCutoutMaterial.SetTexture("_MainTex", NotFoundTexture);
            StandardBaseCutoutMaterial.SetTexture("_EmissionMap", NotFoundTexture);
            StandardBaseCutoutMaterial.SetTexture("_BumpMap", NormalBaseTexture);
            StandardBaseCutoutMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            AssetDatabase.CreateAsset(StandardBaseCutoutMaterial, "Assets/TriLib/TriLib/Resources/StandardBaseCutoutMaterial.mat");

            StandardSpecularCutoutMaterial = new Material(Shader.Find("Standard (Specular setup)"));
            StandardSpecularCutoutMaterial.SetFloat("_Mode", 1f);
            StandardSpecularCutoutMaterial.SetOverrideTag("RenderType", "TransparentCutout");
            StandardSpecularCutoutMaterial.SetInt("_SrcBlend", (int)BlendMode.One);
            StandardSpecularCutoutMaterial.SetInt("_DstBlend", (int)BlendMode.Zero);
            StandardSpecularCutoutMaterial.SetInt("_ZWrite", 1);
            StandardSpecularCutoutMaterial.EnableKeyword("_ALPHATEST_ON");
            StandardSpecularCutoutMaterial.DisableKeyword("_ALPHABLEND_ON");
            StandardSpecularCutoutMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            StandardSpecularCutoutMaterial.renderQueue = (int)RenderQueue.AlphaTest;
            StandardSpecularCutoutMaterial.EnableKeyword("_EMISSION");
            StandardSpecularCutoutMaterial.EnableKeyword("_SPECGLOSSMAP");
            StandardSpecularCutoutMaterial.EnableKeyword("_NORMALMAP");
            StandardSpecularCutoutMaterial.SetTexture("_MainTex", NotFoundTexture);
            StandardSpecularCutoutMaterial.SetTexture("_EmissionMap", NotFoundTexture);
            StandardSpecularCutoutMaterial.SetTexture("_SpecGlossMap", NotFoundTexture);
            StandardSpecularCutoutMaterial.SetTexture("_BumpMap", NormalBaseTexture);
            StandardSpecularCutoutMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            AssetDatabase.CreateAsset(StandardSpecularCutoutMaterial, "Assets/TriLib/TriLib/Resources/StandardSpecularCutoutMaterial.mat");

            AssetDatabase.SaveAssets();
        }
#endif

        /// <summary>
        /// Builds the <see cref="TriLib.MeshData"/> list for given scene.
        /// </summary>
        /// <param name="scene">Assimp scene pointer.</param>
        private void BuildMeshes(IntPtr scene)
        {
            var meshCount = AssimpInterop.aiScene_GetNumMeshes(scene);
            for (uint m = 0; m < meshCount; m++)
            {
                var meshData = new MeshData();
                var mesh = AssimpInterop.aiScene_GetMesh(scene, m);
                var meshName = AssimpInterop.aiMesh_GetName(mesh);
                meshData.Name = FixName(meshName, m);
                var materialIndex = AssimpInterop.aiMesh_GetMatrialIndex(mesh);
                meshData.MaterialIndex = materialIndex;
                var vertexCount = AssimpInterop.aiMesh_VertexCount(mesh);
                var hasNormals = AssimpInterop.aiMesh_HasNormals(mesh);
                if (hasNormals)
                {
                    meshData.Normals = new Vector3[vertexCount];
                }
                var hasTangentsAndBitangents = AssimpInterop.aiMesh_HasTangentsAndBitangents(mesh);
                if (hasTangentsAndBitangents)
                {
                    meshData.Tangents = new Vector4[vertexCount];
                    meshData.BiTangents = new Vector4[vertexCount];
                }
                var hasTextureCoords0 = AssimpInterop.aiMesh_HasTextureCoords(mesh, 0);
                if (hasTextureCoords0)
                {
                    meshData.Uv = new Vector2[vertexCount];
                }
                var hasTextureCoords1 = AssimpInterop.aiMesh_HasTextureCoords(mesh, 1);
                if (hasTextureCoords1)
                {
                    meshData.Uv1 = new Vector2[vertexCount];
                }
                var hasTextureCoords2 = AssimpInterop.aiMesh_HasTextureCoords(mesh, 2);
                if (hasTextureCoords2)
                {
                    meshData.Uv2 = new Vector2[vertexCount];
                }
                var hasTextureCoords3 = AssimpInterop.aiMesh_HasTextureCoords(mesh, 3);
                if (hasTextureCoords3)
                {
                    meshData.Uv3 = new Vector2[vertexCount];
                }
                var hasVertexColors = AssimpInterop.aiMesh_HasVertexColors(mesh, 0);
                if (hasVertexColors)
                {
                    meshData.Colors = new Color[vertexCount];
                }
                meshData.Vertices = new Vector3[vertexCount];
                for (uint v = 0; v < vertexCount; v++)
                {
                    meshData.Vertices[v] = AssimpInterop.aiMesh_GetVertex(mesh, v);
                    if (hasNormals)
                    {
                        meshData.Normals[v] = AssimpInterop.aiMesh_GetNormal(mesh, v);
                    }
                    if (hasTangentsAndBitangents)
                    {
                        meshData.Tangents[v] = AssimpInterop.aiMesh_GetTangent(mesh, v);
                        meshData.BiTangents[v] = AssimpInterop.aiMesh_GetBitangent(mesh, v);
                    }
                    if (hasTextureCoords0)
                    {
                        meshData.Uv[v] = AssimpInterop.aiMesh_GetTextureCoord(mesh, 0, v);
                    }
                    if (hasTextureCoords1)
                    {
                        meshData.Uv1[v] = AssimpInterop.aiMesh_GetTextureCoord(mesh, 1, v);
                    }
                    if (hasTextureCoords2)
                    {
                        meshData.Uv2[v] = AssimpInterop.aiMesh_GetTextureCoord(mesh, 2, v);
                    }
                    if (hasTextureCoords3)
                    {
                        meshData.Uv3[v] = AssimpInterop.aiMesh_GetTextureCoord(mesh, 3, v);
                    }
                    if (hasVertexColors)
                    {
                        meshData.Colors[v] = AssimpInterop.aiMesh_GetVertexColor(mesh, 0, v);
                    }
                }
                if (AssimpInterop.aiMesh_HasFaces(mesh))
                {
                    var facesCount = AssimpInterop.aiMesh_GetNumFaces(mesh);
                    meshData.Triangles = new int[facesCount * 3];
                    for (uint f = 0; f < facesCount; f++)
                    {
                        var face = AssimpInterop.aiMesh_GetFace(mesh, f);
                        var indexCount = AssimpInterop.aiFace_GetNumIndices(face);
                        if (indexCount > 3)
                        {
                            throw new UnityException("More than three face indices is not supported. Please enable \"Triangulate\" in your \"AssetLoaderOptions\" \"PostProcessSteps\" field");
                        }
                        for (uint i = 0; i < indexCount; i++)
                        {
                            meshData.Triangles[f * 3 + i] = (int)AssimpInterop.aiFace_GetIndex(face, i);
                        }
                    }
                }
                MeshData[m] = meshData;
            }
        }

        /// <summary>
        /// Builds the <see cref="TriLib.CameraData"/> list for given scene.
        /// </summary>
        /// <param name="scene">Assimp scene pointer.</param>
        private void BuildCameras(IntPtr scene)
        {
            for (uint c = 0; c < AssimpInterop.aiScene_GetNumCameras(scene); c++)
            {
                var camera = AssimpInterop.aiScene_GetCamera(scene, c);
                var cameraName = AssimpInterop.aiCamera_GetName(camera);
                var cameraData = new CameraData
                {
                    Name = cameraName,
                    Aspect = AssimpInterop.aiCamera_GetAspect(camera),
                    NearClipPlane = AssimpInterop.aiCamera_GetClipPlaneNear(camera),
                    FarClipPlane = AssimpInterop.aiCamera_GetClipPlaneFar(camera),
                    FieldOfView = AssimpInterop.aiCamera_GetHorizontalFOV(camera),
                    LocalPosition = AssimpInterop.aiCamera_GetPosition(camera),
                    Forward = AssimpInterop.aiCamera_GetLookAt(camera),
                    Up = AssimpInterop.aiCamera_GetUp(camera)
                };
                CameraData[c] = cameraData;
            }
        }

        /// <summary>
        /// Builds the <see cref="TriLib.MaterialData"/> list for given scene.
        /// </summary>
        /// <param name="scene">Assimp scene pointer.</param>
        /// <param name="basePath">Base model path.</param>
        /// <param name="loadTextureDataCallback">Pass this callback to load texture data from custom sources.</param>
        private void BuildMaterials(IntPtr scene, string basePath, LoadTextureDataCallback loadTextureDataCallback = null)
        {
            for (uint m = 0; m < AssimpInterop.aiScene_GetNumMaterials(scene); m++)
            {
                var materialData = new MaterialData();
                var material = AssimpInterop.aiScene_GetMaterial(scene, m);
                string materialName;
                if (AssimpInterop.aiMaterial_HasName(material))
                {
                    if (!AssimpInterop.aiMaterial_GetName(material, out materialName))
                    {
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                        Debug.LogWarning("Error loading material name");
#endif
                        materialName = "Material_" + StringUtils.GenerateUniqueName(m);
                    }
                }
                else
                {
                    materialName = "Material_" + StringUtils.GenerateUniqueName(m);
                }
                materialData.Name = materialName;
                var alphaLoaded = false;
                if (AssimpInterop.aiMaterial_HasOpacity(material))
                {
                    float tmpAlpha;
                    if (AssimpInterop.aiMaterial_GetOpacity(material, out tmpAlpha))
                    {
                        materialData.Alpha = tmpAlpha;
                        alphaLoaded = true;
                    }
                }
                materialData.AlphaLoaded = alphaLoaded;

                var diffuseInfoLoaded = false;
                var numDiffuse = AssimpInterop.aiMaterial_GetNumTextureDiffuse(material);
                if (numDiffuse > 0)
                {
                    string path;
                    uint textureMapping;
                    uint uvIndex;
                    float blendMode;
                    uint op;
                    uint mapMode;
                    if (AssimpInterop.aiMaterial_GetTextureDiffuse(material, 0, out path, out textureMapping,
                        out uvIndex,
                        out blendMode, out op, out mapMode))
                    {
                        var wrapMode = mapMode == (uint)TextureWrapMode.Clamp
                            ? TextureWrapMode.Clamp
                            : TextureWrapMode.Repeat;
                        var textureName = StringUtils.GenerateUniqueName(path);
                        materialData.DiffusePath = FixName(path);
                        materialData.DiffuseWrapMode = wrapMode;
                        materialData.DiffuseName = textureName;
                        materialData.DiffuseBlendMode = blendMode;
                        materialData.DiffuseOp = op;
                        diffuseInfoLoaded = true;
                        EmbeddedTextureData embeddedTextureData = null;
                        if (EmbeddedTextureLoad != null)
                        {
                            embeddedTextureData = EmbeddedTextureLoad(path);
                        }
                        if (embeddedTextureData == null)
                        {
                            var texture = AssimpInterop.aiScene_GetEmbeddedTexture(scene, path);
                            if (texture != IntPtr.Zero)
                            {
                                embeddedTextureData = LoadEmbeddedTextureData(scene, texture);
                            }
                            else
                            {
                                embeddedTextureData = loadTextureDataCallback == null ? LoadTextureData(path, basePath) : loadTextureDataCallback(path, basePath);
                            }
                        }
                        materialData.DiffuseEmbeddedTextureData = embeddedTextureData;
                    }
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                    else
                    {
                        Debug.LogWarning("Error loading diffuse texture " + m);
                    }
#endif
                }
                materialData.DiffuseInfoLoaded = diffuseInfoLoaded;

                var diffuseColorLoaded = false;
                if (AssimpInterop.aiMaterial_HasDiffuse(material))
                {
                    Color colorDiffuse;
                    if (AssimpInterop.aiMaterial_GetDiffuse(material, out colorDiffuse))
                    {
                        materialData.DiffuseColor = colorDiffuse;
                        diffuseColorLoaded = true;
                    }
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                    else
                    {
                        Debug.LogWarning("Error loading diffuse color");
                    }
#endif
                }
                materialData.DiffuseColorLoaded = diffuseColorLoaded;

                var emissionColorLoaded = false;
                var hasEmissive = AssimpInterop.aiMaterial_HasEmissive(material);
                if (hasEmissive)
                {
                    Color colorEmissive;
                    if (AssimpInterop.aiMaterial_GetEmissive(material, out colorEmissive))
                    {
                        materialData.EmissionColor = colorEmissive;
                        emissionColorLoaded = true;
                    }
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                    else
                    {
                        Debug.LogWarning("Error loading emissive color");
                    }
#endif
                }
                materialData.EmissionColorLoaded = emissionColorLoaded;

                var emissiveInfoLoaded = false;
                var numEmissive = AssimpInterop.aiMaterial_GetNumTextureEmissive(material);
                if (numEmissive > 0)
                {
                    string path;
                    uint textureMapping;
                    uint uvIndex;
                    float blendMode;
                    uint op;
                    uint mapMode;
                    if (AssimpInterop.aiMaterial_GetTextureEmissive(material, 0, out path, out textureMapping,
                        out uvIndex,
                        out blendMode, out op, out mapMode))
                    {
                        var wrapMode = mapMode == (uint)TextureWrapMode.Clamp
                            ? TextureWrapMode.Clamp
                            : TextureWrapMode.Repeat;
                        var textureName = StringUtils.GenerateUniqueName(path);
                        materialData.EmissionPath = FixName(path);
                        materialData.EmissionWrapMode = wrapMode;
                        materialData.EmissionName = textureName;
                        materialData.EmissionBlendMode = blendMode;
                        materialData.EmissionOp = op;
                        emissiveInfoLoaded = true;
                        EmbeddedTextureData embeddedTextureData = null;
                        if (EmbeddedTextureLoad != null)
                        {
                            embeddedTextureData = EmbeddedTextureLoad(path);
                        }
                        if (embeddedTextureData == null)
                        {
                            var texture = AssimpInterop.aiScene_GetEmbeddedTexture(scene, path);
                            if (texture != IntPtr.Zero)
                            {
                                embeddedTextureData = LoadEmbeddedTextureData(scene, texture);
                            }
                            else
                            {
                                embeddedTextureData = loadTextureDataCallback == null ? LoadTextureData(path, basePath) : loadTextureDataCallback(path, basePath);
                            }
                        }
                        materialData.EmissionEmbeddedTextureData = embeddedTextureData;
                    }
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                    else
                    {
                        Debug.LogWarning("Error loading emissive texture");
                    }
#endif
                }
                materialData.EmissionInfoLoaded = emissiveInfoLoaded;

                var specColorLoaded = false;
                var hasSpecular = AssimpInterop.aiMaterial_HasSpecular(material);
                if (hasSpecular)
                {
                    Color colorSpecular;
                    if (AssimpInterop.aiMaterial_GetSpecular(material, out colorSpecular))
                    {
                        materialData.SpecularColor = colorSpecular;
                        specColorLoaded = true;
                    }
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                    else
                    {
                        Debug.LogWarning("Error loading specular color");
                    }
#endif
                }
                materialData.SpecularColorLoaded = specColorLoaded;

                var specularInfoLoaded = false;
                var numSpecular = AssimpInterop.aiMaterial_GetNumTextureSpecular(material);
                if (numSpecular > 0)
                {
                    string path;
                    uint textureMapping;
                    uint uvIndex;
                    float blendMode;
                    uint op;
                    uint mapMode;
                    if (AssimpInterop.aiMaterial_GetTextureSpecular(material, 0, out path, out textureMapping,
                        out uvIndex,
                        out blendMode, out op, out mapMode))
                    {
                        var wrapMode = mapMode == (uint)TextureWrapMode.Clamp
                            ? TextureWrapMode.Clamp
                            : TextureWrapMode.Repeat;
                        var textureName = StringUtils.GenerateUniqueName(path);
                        materialData.SpecularPath = FixName(path);
                        materialData.SpecularWrapMode = wrapMode;
                        materialData.SpecularName = textureName;
                        materialData.SpecularBlendMode = blendMode;
                        materialData.SpecularOp = op;
                        specularInfoLoaded = true;
                        EmbeddedTextureData embeddedTextureData = null;
                        if (EmbeddedTextureLoad != null)
                        {
                            embeddedTextureData = EmbeddedTextureLoad(path);
                        }
                        if (embeddedTextureData == null)
                        {
                            var texture = AssimpInterop.aiScene_GetEmbeddedTexture(scene, path);
                            if (texture != IntPtr.Zero)
                            {
                                embeddedTextureData = LoadEmbeddedTextureData(scene, texture);
                            }
                            else
                            {
                                embeddedTextureData = loadTextureDataCallback == null ? LoadTextureData(path, basePath) : loadTextureDataCallback(path, basePath);
                            }
                        }
                        materialData.SpecularEmbeddedTextureData = embeddedTextureData;
                    }
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                    else
                    {
                        Debug.LogWarning("Error loading specular texture");
                    }
#endif
                }
                materialData.SpecularInfoLoaded = specularInfoLoaded;

                var normalInfoLoaded = false;
                var numNormals = AssimpInterop.aiMaterial_GetNumTextureNormals(material);
                if (numNormals > 0)
                {
                    string path;
                    uint textureMapping;
                    uint uvIndex;
                    float blendMode;
                    uint op;
                    uint mapMode;
                    if (AssimpInterop.aiMaterial_GetTextureNormals(material, 0, out path, out textureMapping,
                        out uvIndex,
                        out blendMode, out op, out mapMode))
                    {
                        var wrapMode = mapMode == (uint)TextureWrapMode.Clamp
                            ? TextureWrapMode.Clamp
                            : TextureWrapMode.Repeat;
                        var textureName = StringUtils.GenerateUniqueName(path);
                        materialData.NormalPath = FixName(path);
                        materialData.NormalWrapMode = wrapMode;
                        materialData.NormalName = textureName;
                        materialData.NormalBlendMode = blendMode;
                        materialData.NormalOp = op;
                        normalInfoLoaded = true;
                        EmbeddedTextureData embeddedTextureData = null;
                        if (EmbeddedTextureLoad != null)
                        {
                            embeddedTextureData = EmbeddedTextureLoad(path);
                        }
                        if (embeddedTextureData == null)
                        {
                            var texture = AssimpInterop.aiScene_GetEmbeddedTexture(scene, path);
                            if (texture != IntPtr.Zero)
                            {
                                embeddedTextureData = LoadEmbeddedTextureData(scene, texture);
                            }
                            else
                            {
                                embeddedTextureData = loadTextureDataCallback == null ? LoadTextureData(path, basePath) : loadTextureDataCallback(path, basePath);
                            }
                        }
                        materialData.NormalEmbeddedTextureData = embeddedTextureData;
                    }
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                    else
                    {
                        Debug.LogWarning("Error loading normals texture");
                    }
#endif
                }
                materialData.NormalInfoLoaded = normalInfoLoaded;

                var heightInfoLoaded = false;
                var numHeight = AssimpInterop.aiMaterial_GetNumTextureHeight(material);
                if (numHeight > 0)
                {
                    string path;
                    uint textureMapping;
                    uint uvIndex;
                    float blendMode;
                    uint op;
                    uint mapMode;
                    if (AssimpInterop.aiMaterial_GetTextureHeight(material, 0, out path, out textureMapping,
                        out uvIndex,
                        out blendMode, out op, out mapMode))
                    {
                        var wrapMode = mapMode == (uint)TextureWrapMode.Clamp
                            ? TextureWrapMode.Clamp
                            : TextureWrapMode.Repeat;
                        var textureName = StringUtils.GenerateUniqueName(path);
                        materialData.HeightPath = FixName(path);
                        materialData.HeightWrapMode = wrapMode;
                        materialData.HeightName = textureName;
                        materialData.HeightBlendMode = blendMode;
                        materialData.HeightOp = op;
                        heightInfoLoaded = true;
                        EmbeddedTextureData embeddedTextureData = null;
                        if (EmbeddedTextureLoad != null)
                        {
                            embeddedTextureData = EmbeddedTextureLoad(path);
                        }
                        if (embeddedTextureData == null)
                        {
                            var texture = AssimpInterop.aiScene_GetEmbeddedTexture(scene, path);
                            if (texture != IntPtr.Zero)
                            {
                                embeddedTextureData = LoadEmbeddedTextureData(scene, texture);
                            }
                            else
                            {
                                embeddedTextureData = loadTextureDataCallback == null ? LoadTextureData(path, basePath) : loadTextureDataCallback(path, basePath);
                            }
                        }
                        materialData.HeightEmbeddedTextureData = embeddedTextureData;
                    }
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                    else
                    {
                        Debug.LogWarning("Error loading normals texture");
                    }
#endif
                }
                materialData.HeightInfoLoaded = heightInfoLoaded;

                var bumpScaleLoaded = false;
                if (AssimpInterop.aiMaterial_HasBumpScaling(material))
                {
                    float bumpScaling;
                    if (AssimpInterop.aiMaterial_GetBumpScaling(material, out bumpScaling))
                    {
                        if (Mathf.Approximately(bumpScaling, 0f))
                        {
                            bumpScaling = 1f;
                        }
                        materialData.BumpScale = bumpScaling;
                        bumpScaleLoaded = true;
                    }
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                    else
                    {
                        Debug.LogWarning("Error loading bump scaling");
                    }
#endif
                }
                materialData.BumpScaleLoaded = bumpScaleLoaded;

                var shininessLoaded = false;
                if (AssimpInterop.aiMaterial_HasShininess(material))
                {
                    float shininess;
                    if (AssimpInterop.aiMaterial_GetShininess(material, out shininess))
                    {
                        materialData.Glossiness = shininess;
                        shininessLoaded = true;
                    }
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                    else
                    {
                        Debug.LogWarning("Error loading shininess");
                    }
#endif
                }
                materialData.GlossinessLoaded = shininessLoaded;

                var shininessStrengthLoaded = false;
                if (AssimpInterop.aiMaterial_HasShininessStrength(material))
                {
                    float shininessStrength;
                    if (AssimpInterop.aiMaterial_GetShininessStrength(material, out shininessStrength))
                    {
                        materialData.GlossMapScale = shininessStrength;
                        shininessStrengthLoaded = true;
                    }
                    else
                    {
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                        Debug.LogWarning("Error loading shininess strength");
#endif
                    }
                }
                materialData.GlossMapScaleLoaded = shininessStrengthLoaded;

                MaterialData[m] = materialData;
            }
        }

        /// <summary>
        /// Tries to load texture data from the given path, searching from basePath.
        /// </summary>
        /// <param name="path">Texture relative path.</param>
        /// <param name="basePath">Model absolute path.</param>
        /// <returns>A new <see cref="TriLib.EmbeddedTextureData"/>.</returns>
        public static EmbeddedTextureData LoadTextureData(string path, string basePath)
        {
            string filename = null;
            var finalPath = path;
            var data = FileUtils.LoadFileData(finalPath);
            if (data.Length == 0 && basePath != null)
            {
                finalPath = Path.Combine(basePath, path);
                data = FileUtils.LoadFileData(finalPath);
            }
            if (data.Length == 0)
            {
                filename = FileUtils.GetFilename(path);
                finalPath = filename;
                data = FileUtils.LoadFileData(finalPath);
            }
            if (data.Length == 0 && basePath != null && filename != null)
            {
                finalPath = Path.Combine(basePath, filename);
                data = FileUtils.LoadFileData(finalPath);
            }
            if (data.Length == 0)
            {
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                    Debug.LogWarningFormat("Texture '{0}' not found", path);
#endif
                return null;
            }
            var embeddedTextureData = new EmbeddedTextureData();
#if !TRILIB_USE_UNITY_TEXTURE_LOADER
            embeddedTextureData.Data = STBImageLoader.LoadTextureDataFromByteArray(data, out embeddedTextureData.Width, out embeddedTextureData.Height, out embeddedTextureData.NumChannels);
            embeddedTextureData.IsRawData = true;
#else
            embeddedTextureData.Data = data;
#endif
            return embeddedTextureData;
        }

        /// <summary>
        /// Loads an embedded texture data.
        /// </summary>
        /// <param name="scene">Assimp scene pointer.</param>
        /// <param name="texture">Assimp texture pointer.</param>
        /// <returns>A new <see cref="TriLib.EmbeddedTextureData"/>.</returns>
        private EmbeddedTextureData LoadEmbeddedTextureData(IntPtr scene, IntPtr texture)
        {
            var embeddedTextureData = new EmbeddedTextureData();
            var isRawData = !AssimpInterop.aiMaterial_IsEmbeddedTextureCompressed(scene, texture);
            var dataLength = AssimpInterop.aiMaterial_GetEmbeddedTextureDataSize(scene, texture, !isRawData);
            var data = AssimpInterop.aiMaterial_GetEmbeddedTextureData(scene, texture, dataLength);
            if (!isRawData)
            {
#if !TRILIB_USE_UNITY_TEXTURE_LOADER
                embeddedTextureData.Data = STBImageLoader.LoadTextureDataFromByteArray(data, out embeddedTextureData.Width, out embeddedTextureData.Height, out embeddedTextureData.NumChannels);
                embeddedTextureData.IsRawData = true;
#else
                embeddedTextureData.Data = data;
                embeddedTextureData.IsRawData = false;
                embeddedTextureData.Width = AssimpInterop.aiMaterial_GetEmbeddedTextureWidth(texture);
                embeddedTextureData.Height = AssimpInterop.aiMaterial_GetEmbeddedTextureHeight(texture);
#endif
            }
            else
            {
                embeddedTextureData.Data = data;
                embeddedTextureData.IsRawData = true;
                embeddedTextureData.Width = AssimpInterop.aiMaterial_GetEmbeddedTextureWidth(texture);
                embeddedTextureData.Height = AssimpInterop.aiMaterial_GetEmbeddedTextureHeight(texture);
            }
            return embeddedTextureData;
        }

        /// <summary>
        /// Builds the bones and binding poses and assigns to the <see cref="TriLib.MeshData"/> list.
        /// </summary>
        /// <param name="scene">Assimp scene pointer.</param>
        private void BuildBones(IntPtr scene)
        {
            var meshCount = AssimpInterop.aiScene_GetNumMeshes(scene);
            for (uint m = 0; m < meshCount; m++)
            {
                var meshData = MeshData[m];
                var mesh = AssimpInterop.aiScene_GetMesh(scene, m);
                var hasBoneInfo = AssimpInterop.aiMesh_HasBones(mesh);
                meshData.HasBoneInfo = hasBoneInfo;
                if (hasBoneInfo)
                {
                    HasBoneInfo = true;
                }
                if (hasBoneInfo)
                {
                    var vertexCount = AssimpInterop.aiMesh_VertexCount(mesh);
                    var boneCount = AssimpInterop.aiMesh_GetNumBones(mesh);
                    meshData.BindPoses = new Matrix4x4[boneCount];
                    meshData.BoneNames = new string[boneCount];
                    meshData.BoneWeights = new BoneWeight[vertexCount];
                    var unityBonesInVertices = new int[vertexCount];
                    for (uint b = 0; b < boneCount; b++)
                    {
                        var bone = AssimpInterop.aiMesh_GetBone(mesh, b);
                        var boneName = AssimpInterop.aiBone_GetName(bone);
                        meshData.BoneNames[b] = boneName;
                        var unityBindPose = AssimpInterop.aiBone_GetOffsetMatrix(bone);
                        meshData.BindPoses[b] = unityBindPose;
                        var vertexWeightCount = AssimpInterop.aiBone_GetNumWeights(bone);
                        for (uint w = 0; w < vertexWeightCount; w++)
                        {
                            var wInt = (int)b;
                            var vertexWeight = AssimpInterop.aiBone_GetWeights(bone, w);
                            var weightValue = AssimpInterop.aiVertexWeight_GetWeight(vertexWeight);
                            var weightVertexId = AssimpInterop.aiVertexWeight_GetVertexId(vertexWeight);
                            BoneWeight unityBoneWeight;
                            var unityCurrentBonesInVertex = unityBonesInVertices[weightVertexId];
                            switch (unityCurrentBonesInVertex)
                            {
                                case 0:
                                    unityBoneWeight = new BoneWeight
                                    {
                                        boneIndex0 = wInt,
                                        weight0 = weightValue
                                    };
                                    meshData.BoneWeights[weightVertexId] = unityBoneWeight;
                                    break;
                                case 1:
                                    unityBoneWeight = meshData.BoneWeights[weightVertexId];
                                    unityBoneWeight.boneIndex1 = wInt;
                                    unityBoneWeight.weight1 = weightValue;
                                    meshData.BoneWeights[weightVertexId] = unityBoneWeight;
                                    break;
                                case 2:
                                    unityBoneWeight = meshData.BoneWeights[weightVertexId];
                                    unityBoneWeight.boneIndex2 = wInt;
                                    unityBoneWeight.weight2 = weightValue;
                                    meshData.BoneWeights[weightVertexId] = unityBoneWeight;
                                    break;
                                case 3:
                                    unityBoneWeight = meshData.BoneWeights[weightVertexId];
                                    unityBoneWeight.boneIndex3 = wInt;
                                    unityBoneWeight.weight3 = weightValue;
                                    meshData.BoneWeights[weightVertexId] = unityBoneWeight;
                                    break;
                                default:
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                                    Debug.LogWarningFormat("Vertex {0} has more than 4 bone weights. This is not supported", weightVertexId);
#endif
                                    unityBoneWeight = meshData.BoneWeights[weightVertexId];
                                    unityBoneWeight.boneIndex3 = wInt;
                                    unityBoneWeight.weight3 = weightValue;
                                    meshData.BoneWeights[weightVertexId] = unityBoneWeight;
                                    break;
                            }
                            unityBonesInVertices[weightVertexId]++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Builds the <see cref="TriLib.AnimationData"/> list for given scene.
        /// </summary>
        /// <param name="scene">Assimp scene pointer.</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the object.</param>
        private void BuildAnimations(IntPtr scene, AssetLoaderOptions options)
        {
            var animationCount = AssimpInterop.aiScene_GetNumAnimations(scene);
            for (uint a = 0; a < animationCount; a++)
            {
                var sceneAnimation = AssimpInterop.aiScene_GetAnimation(scene, a);
                var ticksPerSecond = AssimpInterop.aiAnimation_GetTicksPerSecond(sceneAnimation);
                if (ticksPerSecond <= 0)
                {
                    ticksPerSecond = 60f;
                }
                var durationInTicks = AssimpInterop.aiAnimation_GetDuraction(sceneAnimation);
                var totalTime = durationInTicks / ticksPerSecond;
                var animationChannelCount = AssimpInterop.aiAnimation_GetNumChannels(sceneAnimation);
                var sceneAnimationName = AssimpInterop.aiAnimation_GetName(sceneAnimation);
                var animationData = new AnimationData
                {
                    Name =
                        string.IsNullOrEmpty(sceneAnimationName)
                            ? "Animation_" + StringUtils.GenerateUniqueName(a)
                            : sceneAnimationName,
                    Legacy = options == null || options.UseLegacyAnimations,
                    FrameRate = ticksPerSecond,
                    Length = totalTime,
                    ChannelData = new AnimationChannelData[animationChannelCount]
                };
                for (uint n = 0; n < animationChannelCount; n++)
                {
                    var nodeAnimationChannel = AssimpInterop.aiAnimation_GetAnimationChannel(sceneAnimation, n);
                    var nodeName = AssimpInterop.aiNodeAnim_GetNodeName(nodeAnimationChannel);
                    AnimationChannelData channelData =

                            new AnimationChannelData
                            {
                                CurveData = new Dictionary<string, AnimationCurveData>(),
                                NodeName = nodeName
                            };
                    var numPositionKeys = AssimpInterop.aiNodeAnim_GetNumPositionKeys(nodeAnimationChannel);
                    if (numPositionKeys > 0)
                    {
                        var unityPositionCurveX = new AnimationCurveData(numPositionKeys);
                        var unityPositionCurveY = new AnimationCurveData(numPositionKeys);
                        var unityPositionCurveZ = new AnimationCurveData(numPositionKeys);
                        for (uint p = 0; p < numPositionKeys; p++)
                        {
                            var positionKey = AssimpInterop.aiNodeAnim_GetPositionKey(nodeAnimationChannel, p);
                            var time = AssimpInterop.aiVectorKey_GetTime(positionKey) / ticksPerSecond;
                            var unityVector3 = AssimpInterop.aiVectorKey_GetValue(positionKey);
                            unityPositionCurveX.AddKey(time, unityVector3[0]);
                            unityPositionCurveY.AddKey(time, unityVector3[1]);
                            unityPositionCurveZ.AddKey(time, unityVector3[2]);
                        }
                        channelData.SetCurve("localPosition.x", unityPositionCurveX);
                        channelData.SetCurve("localPosition.y", unityPositionCurveY);
                        channelData.SetCurve("localPosition.z", unityPositionCurveZ);
                    }
                    var numRotationKeys = AssimpInterop.aiNodeAnim_GetNumRotationKeys(nodeAnimationChannel);
                    if (numRotationKeys > 0)
                    {
                        var unityRotationCurveX = new AnimationCurveData(numRotationKeys);
                        var unityRotationCurveY = new AnimationCurveData(numRotationKeys);
                        var unityRotationCurveZ = new AnimationCurveData(numRotationKeys);
                        var unityRotationCurveW = new AnimationCurveData(numRotationKeys);
                        for (uint r = 0; r < numRotationKeys; r++)
                        {
                            var rotationKey = AssimpInterop.aiNodeAnim_GetRotationKey(nodeAnimationChannel, r);
                            var time = AssimpInterop.aiQuatKey_GetTime(rotationKey) / ticksPerSecond;
                            var unityQuaternion = AssimpInterop.aiQuatKey_GetValue(rotationKey);
                            unityRotationCurveX.AddKey(time, unityQuaternion[1]);
                            unityRotationCurveY.AddKey(time, unityQuaternion[2]);
                            unityRotationCurveZ.AddKey(time, unityQuaternion[3]);
                            unityRotationCurveW.AddKey(time, unityQuaternion[0]);
                        }
                        channelData.SetCurve("localRotation.x", unityRotationCurveX);
                        channelData.SetCurve("localRotation.y", unityRotationCurveY);
                        channelData.SetCurve("localRotation.z", unityRotationCurveZ);
                        channelData.SetCurve("localRotation.w", unityRotationCurveW);
                    }
                    var numScalingKeys = AssimpInterop.aiNodeAnim_GetNumScalingKeys(nodeAnimationChannel);
                    if (numScalingKeys > 0)
                    {

                        var unityScaleCurveX = new AnimationCurveData(numScalingKeys);
                        var unityScaleCurveY = new AnimationCurveData(numScalingKeys);
                        var unityScaleCurveZ = new AnimationCurveData(numScalingKeys);
                        for (uint s = 0; s < numScalingKeys; s++)
                        {
                            var scaleKey = AssimpInterop.aiNodeAnim_GetScalingKey(nodeAnimationChannel, s);
                            var time = AssimpInterop.aiVectorKey_GetTime(scaleKey) / ticksPerSecond;
                            var unityVector3 = AssimpInterop.aiVectorKey_GetValue(scaleKey);
                            unityScaleCurveX.AddKey(time, unityVector3[0]);
                            unityScaleCurveY.AddKey(time, unityVector3[1]);
                            unityScaleCurveZ.AddKey(time, unityVector3[2]);
                        }
                        channelData.SetCurve("localScale.x", unityScaleCurveX);
                        channelData.SetCurve("localScale.y", unityScaleCurveY);
                        channelData.SetCurve("localScale.z", unityScaleCurveZ);
                    }
                    animationData.ChannelData[n] = channelData;
                }
                animationData.WrapMode = options != null ? options.AnimationWrapMode : WrapMode.Loop;
                AnimationData[a] = animationData;
            }
        }

        /// <summary>
        /// Generates a unique node name, if the given name is empty.
        /// </summary>
        /// <param name="name">Node name to check.</param>
        /// <param name="nodeId">Node id to use when the node name is empty or when it already exists.</param>
        /// <returns>Generated name if given name is empty. Otherwise, returns the given name.</returns>
        protected virtual string FixNodeName(string name, uint nodeId)
        {
            if (string.IsNullOrEmpty(name))
            {
                return nodeId.ToString();
            }
            if (NodesPath != null && NodesPath.ContainsKey(name))
            {
                return name + nodeId;
            }
            return name;
        }

        /// <summary>
        /// Generates a unique name, if the given name is empty.
        /// </summary>
        /// <param name="name">Name to check.</param>
        /// <param name="id">Id to generate the unique name.</param>
        /// <returns>Generated name if given name is empty. Otherwise, returns the given name.</returns>
        protected virtual string FixName(string name, uint id)
        {
            return string.IsNullOrEmpty(name) ? StringUtils.GenerateUniqueName(id) : name;
        }

        /// <summary>
        /// Generates a unique name using GUIDs, if the given name is empty.
        /// </summary>
        /// <param name="name">Name to check.</param>
        /// <returns>Generated name if given name is empty. Otherwise, returns the given name.</returns>
        protected virtual string FixName(string name)
        {
            return string.IsNullOrEmpty(name) ? new Guid().ToString() : name;
        }

        /// <summary>
        /// The standard resource data retrieval callback.
        /// </summary>
        /// <param name="resourceFilename">Requested resource filename.</param>
        /// <param name="filename">Requested resource generated ID.</param>
        /// <param name="fileSize">Requested resource output fiele size.</param>
        /// <returns>The resource data <see cref="System.IntPtr"/>, if exists.</returns>
#if !UNITY_EDITOR && !NET_4_6 && !NETFX_CORE
        [MonoPInvokeCallback(typeof(AssimpInterop.DataCallback))]
#endif
        private static IntPtr DefaultDataCallback(string resourceFilename, int resourceId, ref int fileSize)
        {
            var fileLoadData = FilesLoadData[resourceId];
            if (resourceFilename == fileLoadData.Filename || resourceFilename.StartsWith(AssimpFilenameMagicString) || fileLoadData.BasePath == null)
            {
                return IntPtr.Zero;
            }
            var resourceShortFilename = FileUtils.GetFilename(resourceFilename);
            var resourceFileBytes = FileUtils.LoadFileData(Path.Combine(fileLoadData.BasePath, resourceShortFilename));
            if (resourceFileBytes.Length == 0)
            {
                return IntPtr.Zero;
            }
            fileSize = resourceFileBytes.Length;
            var dataBuffer = AssimpInterop.LockGc(resourceFileBytes);
            ((GCFileLoadData)fileLoadData).LockedBuffers.Add(dataBuffer);
            return dataBuffer.AddrOfPinnedObject();
        }

        /// <summary>
        /// The standard callback to defines if a resource exists.
        /// </summary>
        /// <param name="resourceFilename">Requested resource filename.</param>
        /// <param name="filename">Requested resource generated ID.</param>
        /// <returns><c>true</c> if resource exists, otherwise <c>false</c>.</returns>
#if !UNITY_EDITOR && !NET_4_6 && !NETFX_CORE
        [MonoPInvokeCallback(typeof(AssimpInterop.ExistsCallback))]
#endif
        private static bool DefaultExistsCallback(string resourceFilename, int resourceId)
        {
            var fileLoadData = FilesLoadData[resourceId];
            if (resourceFilename == fileLoadData.Filename || resourceFilename.StartsWith(AssimpFilenameMagicString) || fileLoadData.BasePath == null)
            {
                return false;
            }
            var resourceShortFilename = FileUtils.GetFilename(resourceFilename);
            return File.Exists(Path.Combine(fileLoadData.BasePath, resourceShortFilename));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            RootNodeData = null;
            MaterialData = null;
            MeshData = null;
            AnimationData = null;
            CameraData = null;
            Metadata = null;
            NodesPath = null;
            LoadedMaterials = null;
            LoadedTextures = null;
            LoadedBoneNames = null;
            NodeId = 0;
            HasBoneInfo = false;
        }
    }
}

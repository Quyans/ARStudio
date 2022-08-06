#if (NET_4_6 || NETFX_CORE)
using System.Threading.Tasks;
#else
using System.Threading;
#endif
using UnityEngine;

namespace TriLib
{    /// <summary>
     /// Represents an asynchronous asset loader.
     /// </summary>
    public class AssetLoaderAsync : AssetLoaderBase
    {
        /// <summary>
        /// Asynchronously loads a <see cref="UnityEngine.GameObject"/> from input filename with defined options.
        /// @warning To ensure your materials will be loaded, don´t remove the material files included in the package.
        /// </summary>
        /// <param name="filename">Filename used to load the <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the object.</param>
        /// <param name="wrapperGameObject">Use this field to load the new <see cref="UnityEngine.GameObject"/> into referenced <see cref="UnityEngine.GameObject"/>.</param> 
        /// <param name="onAssetLoaded">The action that will be executed when the <see cref="UnityEngine.GameObject"/> be loaded</param>
        /// <param name="basePath">Base path from the loaded file.</param>
        /// <returns>The created Thread on NET 2.0, otherwise returns the created Task.</returns>
        /// <example>
        /// @code
        /// protected void Awake() {
        ///     GameObject myGameObject;
        ///     try {
        ///         using (var assetLoader = new AssetLoader()) {
        ///             assetLoader.LoadFromFile("mymodel.fbx", null, null, delegate(GameObject loadedGameObject) {
        ///                 Debug.Log("My object '" + loadedGameObject.name +  "' has been loaded!");
        ///             });
        ///         }
        ///     } catch (Exception e) {
        ///         Debug.LogFormat("Unable to load mymodel.fbx. The loader returned: {0}", e);
        ///     }
        /// }
        /// @endcode
        /// </example>
#if (NET_4_6 || NETFX_CORE)
        public Task LoadFromFile(string filename, AssetLoaderOptions options, GameObject wrapperGameObject,
            ObjectLoadedHandle onAssetLoaded, string basePath = null)
#else
        public Thread LoadFromFile(string filename, AssetLoaderOptions options, GameObject wrapperGameObject,
            ObjectLoadedHandle onAssetLoaded, string basePath = null)
#endif
        {
            if (basePath == null)
            {
                basePath = FileUtils.GetFileDirectory(filename);
            }
            var usesWrapperGameObject = wrapperGameObject != null;
            return ThreadUtils.RunThread(delegate
            {
                InternalLoadFromFile(filename, basePath, options, usesWrapperGameObject);
            },
                delegate
                {
                    var loadedGameObject = BuildGameObject(options, basePath, wrapperGameObject);
                    if (onAssetLoaded != null)
                    {
                        onAssetLoaded(loadedGameObject);
                    }
                }
            );
        }

        /// <summary>
        /// Asynchronously loads a <see cref="UnityEngine.GameObject"/> from input byte array with defined options.
        /// @warning To ensure your materials will be loaded, don´t remove material files included in the package.
        /// </summary>
        /// <param name="fileBytes">Data used to load the <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="filename">Original file name, if you know it. Otherwise, use the original file extension instead. (Eg: ".FBX")</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the object.</param>
        /// <param name="wrapperGameObject">Use this field to load the new <see cref="UnityEngine.GameObject"/> into referenced <see cref="UnityEngine.GameObject"/>.</param> 
        /// <param name="onAssetLoaded">The action that will be executed when the <see cref="UnityEngine.GameObject"/> be loaded</param>
        /// <param name="basePath">Base path from the loaded file.</param>        /// 
        /// <param name="dataCallback">Custom resource data retrieval callback. Pass this parameter when you need to load external data while loading from memory.</param>
        /// <param name="existsCallback">Custom resource size retrieval callback. Pass this parameter when you need to load external data while loading from memory.</param>
        /// <param name="loadTextureDataCallback">Pass this callback to load texture data from custom sources.</param>
        /// <returns>The created Thread on NET 2.0, otherwise returns the created Task.</returns>
        /// <example>
        /// @code
        /// protected void Awake() {
        ///     GameObject myGameObject;
        ///     try {
        ///         using (var assetLoader = new AssetLoader()) {
        ///             //In case you don't have a valid filename, set this to the file extension
        ///             //to help TriLib assigining a file loader to this file
        ///             //example value: ".FBX"
        /// 			var filename = "c:/models/mymodel.fbx";
        /// 			var fileData = File.ReadAllBytes(filename);
        ///             assetLoader.LoadFromMemory(fleData, filename, delegate(GameObject loadedGameObject) {
        ///                 Debug.Log("My object '" + loadedGameObject.name +  "' has been loaded!");
        ///             });
        ///         }
        ///     } catch (Exception e) {
        ///         Debug.LogFormat("Unable to load mymodel.fbx. The loader returned: {0}", e);
        ///     }
        /// }
        /// @endcode
        /// </example>
#if (NET_4_6 || NETFX_CORE)
        public Task LoadFromMemory(byte[] fileBytes, string filename, AssetLoaderOptions options,
            GameObject wrapperGameObject, ObjectLoadedHandle onAssetLoaded, string basePath = null, AssimpInterop.DataCallback dataCallback = null, AssimpInterop.ExistsCallback existsCallback = null, LoadTextureDataCallback loadTextureDataCallback = null)
#else
        public Thread LoadFromMemory(byte[] fileBytes, string filename, AssetLoaderOptions options,
            GameObject wrapperGameObject, ObjectLoadedHandle onAssetLoaded, string basePath = null, AssimpInterop.DataCallback dataCallback = null, AssimpInterop.ExistsCallback existsCallback = null, LoadTextureDataCallback loadTextureDataCallback = null)
#endif
        {
            if (basePath == null)
            {
                basePath = FileUtils.GetFileDirectory(filename);
            }
            var usesWrapperGameObject = wrapperGameObject != null;
            return ThreadUtils.RunThread(delegate
                {
                    InternalLoadFromMemory(fileBytes, filename, basePath, options, usesWrapperGameObject, dataCallback, existsCallback, loadTextureDataCallback);
                },
                delegate
                {
                    var loadedGameObject = BuildGameObject(options, filename, wrapperGameObject);
                    if (onAssetLoaded != null)
                    {
                        onAssetLoaded(loadedGameObject);
                    }
                }
            );
        }

        /// <summary>
        /// Asynchronously loads a <see cref="UnityEngine.GameObject"/> from file (Accept ZIP files).
        /// </summary>
        /// <param name="filename">Filename used to load the <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the object.</param>
        /// <param name="wrapperGameObject">Use this field to load the new <see cref="UnityEngine.GameObject"/> into referenced <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="onAssetLoaded">The action that will be executed when the <see cref="UnityEngine.GameObject"/> be loaded</param>
        /// <param name="basePath">Base path from the loaded file.</param>
        /// <returns>The created Thread on NET 2.0, otherwise returns the created Task.</returns>
#if (NET_4_6 || NETFX_CORE)
        public Task LoadFromFileWithTextures(string filename, AssetLoaderOptions options, GameObject wrapperGameObject, ObjectLoadedHandle onAssetLoaded, string basePath = null)
#else
        public Thread LoadFromFileWithTextures(string filename, AssetLoaderOptions options, GameObject wrapperGameObject, ObjectLoadedHandle onAssetLoaded, string basePath = null)
#endif
        {
            var extension = FileUtils.GetFileExtension(filename);
            if (basePath == null)
            {
                basePath = FileUtils.GetFileDirectory(filename);
            }
            var usesWrapperGameObject = wrapperGameObject != null;
            return ThreadUtils.RunThread(delegate
            {
                var fileData = FileUtils.LoadFileData(filename);
                InternalLoadFromMemoryAndZip(fileData, extension, basePath, options, usesWrapperGameObject);
            },
                delegate
                {
                    var loadedGameObject = BuildGameObject(options, extension, wrapperGameObject);
                    if (onAssetLoaded != null)
                    {
                        onAssetLoaded(loadedGameObject);
                    }
                }
            );
        }

        /// <summary>
        /// Asynchronously loads a <see cref="UnityEngine.GameObject"/> from input byte array (Accept ZIP files).
        /// </summary>
        /// <param name="fileData">File data.</param>
        /// <param name="assetExtension">Asset extension.</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the object.</param>
        /// <param name="wrapperGameObject">Use this field to load the new <see cref="UnityEngine.GameObject"/> into referenced <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="onAssetLoaded">The action that will be executed when the <see cref="UnityEngine.GameObject"/> be loaded</param>
        /// <param name="basePath">Base path from the loaded file.</param>
        /// <param name="dataCallback">Custom resource data retrieval callback. Pass this parameter when you need to load external data while loading from memory.</param>
        /// <param name="existsCallback">Custom resource size retrieval callback. Pass this parameter when you need to load external data while loading from memory.</param>
        /// <returns>The created Thread on NET 2.0, otherwise returns the created Task.</returns>
#if (NET_4_6 || NETFX_CORE)
        public Task LoadFromMemoryWithTextures(byte[] fileData, string assetExtension, AssetLoaderOptions options, GameObject wrapperGameObject, ObjectLoadedHandle onAssetLoaded, string basePath = null, AssimpInterop.DataCallback dataCallback = null, AssimpInterop.ExistsCallback existsCallback = null)
#else
        public Thread LoadFromMemoryWithTextures(byte[] fileData, string assetExtension, AssetLoaderOptions options, GameObject wrapperGameObject, ObjectLoadedHandle onAssetLoaded, string basePath = null, AssimpInterop.DataCallback dataCallback = null, AssimpInterop.ExistsCallback existsCallback = null)
#endif
        {
            if (basePath == null)
            {
                basePath = FileUtils.GetFileDirectory(assetExtension);
            }
            var usesWrapperGameObject = wrapperGameObject != null;
            return ThreadUtils.RunThread(delegate
                {
                    InternalLoadFromMemoryAndZip(fileData, assetExtension, basePath, options, usesWrapperGameObject, dataCallback, existsCallback);
                },
                delegate
                {
                    var loadedGameObject = BuildGameObject(options, assetExtension, wrapperGameObject);
                    if (onAssetLoaded != null)
                    {
                        onAssetLoaded(loadedGameObject);
                    }
                }
            );
        }
    }
}

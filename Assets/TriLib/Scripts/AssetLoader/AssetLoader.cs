using UnityEngine;

namespace TriLib
{
    /// <summary>
    /// Represents a synchronous asset loader.
    /// </summary>
    public class AssetLoader : AssetLoaderBase
    {
        /// <summary>
        /// Loads a <see cref="UnityEngine.GameObject"/> from input filename with defined options.
        /// @warning To ensure your materials will be loaded, don´t remove the material files included in the package.
        /// </summary>
        /// <param name="filename">Filename used to load the <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the object.</param>
        /// <param name="wrapperGameObject">Use this field to load the new <see cref="UnityEngine.GameObject"/> into referenced <see cref="UnityEngine.GameObject"/>.</param> 
        /// <param name="basePath">Base path from the loaded file.</param>
        /// <returns>A new <see cref="UnityEngine.GameObject"/>.</returns>
        /// <example>
        /// @code
        /// protected void Awake() {
        ///     GameObject myGameObject;
        ///     try {
        ///         using (var assetLoader = new AssetLoader()) {
        ///             gameObject = assetLoader.LoadFromFile("mymodel.fbx");
        ///         }
        ///     } catch (Exception e) {
        ///         Debug.LogFormat("Unable to load mymodel.fbx. The loader returned: {0}", e);
        ///     }
        /// }
        /// @endcode
        /// </example>
        public GameObject LoadFromFile(string filename, AssetLoaderOptions options = null, GameObject wrapperGameObject = null, string basePath = null)
        {
            if (basePath == null)
            {
                basePath = FileUtils.GetFileDirectory(filename);
            }
            InternalLoadFromFile(filename, basePath, options, wrapperGameObject != null);
            return BuildGameObject(options, basePath, wrapperGameObject);
        }

        /// <summary>
        /// Loads a <see cref="UnityEngine.GameObject"/> from input byte array with defined options.
        /// @warning To ensure your materials will be loaded, don´t remove material files included in the package.
        /// </summary>
        /// <param name="fileBytes">Data used to load the <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="filename">Original file name, if you know it. Otherwise, use the original file extension instead. (Eg: ".FBX")</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the object.</param>
        /// <param name="wrapperGameObject">Use this field to load the new <see cref="UnityEngine.GameObject"/> into referenced <see cref="UnityEngine.GameObject"/>.</param> 
        /// <param name="basePath">Base path from the loaded file.</param>
        /// <param name="dataCallback">Custom resource data retrieval callback. Pass this parameter when you need to load external data while loading from memory.</param>
        /// <param name="existsCallback">Custom resource size retrieval callback. Pass this parameter when you need to load external data while loading from memory.</param>
        ///<param name="loadTextureDataCallback">Pass this callback to load texture data from custom sources.</param>
        /// <returns>A new <see cref="UnityEngine.GameObject"/>.</returns>
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
        ///             gameObject = assetLoader.LoadFromMemory(fleData, filename);
        ///         }
        ///     } catch (Exception e) {
        ///         Debug.LogFormat("Unable to load mymodel.fbx. The loader returned: {0}", e);
        ///     }
        /// }
        /// @endcode
        /// </example>
        public GameObject LoadFromMemory(byte[] fileBytes, string filename, AssetLoaderOptions options = null, GameObject wrapperGameObject = null, string basePath = null, AssimpInterop.DataCallback dataCallback = null, AssimpInterop.ExistsCallback existsCallback = null, LoadTextureDataCallback loadTextureDataCallback = null)
        {
            if (basePath == null)
            {
                basePath = FileUtils.GetFileDirectory(filename);
            }
            InternalLoadFromMemory(fileBytes, filename, basePath, options, wrapperGameObject != null, dataCallback, existsCallback, loadTextureDataCallback);
            return BuildGameObject(options, basePath, wrapperGameObject);
        }

        /// <summary>
        /// Loads a <see cref="UnityEngine.GameObject"/> from file (Accept ZIP files).
        /// </summary>
        /// <param name="filename">Filename used to load the <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the object.</param>
        /// <param name="wrapperGameObject">Use this field to load the new <see cref="UnityEngine.GameObject"/> into referenced <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="basePath">Base path from the loaded file.</param>
        /// <returns><c>true</c> if model was loaded successfuly, otherwise <c>false</c>.</returns>
        public GameObject LoadFromFileWithTextures(string filename, AssetLoaderOptions options, GameObject wrapperGameObject = null, string basePath = null)
        {
            var fileData = FileUtils.LoadFileData(filename);
            var extension = FileUtils.GetFileExtension(filename);
            if (basePath == null)
            {
                basePath = FileUtils.GetFileDirectory(filename);
            }
            InternalLoadFromMemoryAndZip(fileData, extension, basePath, options, wrapperGameObject != null);
            return BuildGameObject(options, extension, wrapperGameObject);
        }

        /// <summary>
        /// Loads a <see cref="UnityEngine.GameObject"/> from input byte array (Accept ZIP files).
        /// </summary>
        /// <param name="fileData">File data.</param>
        /// <param name="assetExtension">Asset extension.</param>
        /// <param name="options"><see cref="AssetLoaderOptions"/> used to load the object.</param>
        /// <param name="wrapperGameObject">Use this field to load the new <see cref="UnityEngine.GameObject"/> into referenced <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="basePath">Base path from the loaded file.</param>
        /// <param name="dataCallback">Custom resource data retrieval callback. Pass this parameter when you need to load external data while loading from memory.</param>
        /// <param name="existsCallback">Custom resource size retrieval callback. Pass this parameter when you need to load external data while loading from memory.</param>
        /// <returns><c>true</c> if model was loaded successfuly, otherwise <c>false</c>.</returns>
        public GameObject LoadFromMemoryWithTextures(byte[] fileData, string assetExtension, AssetLoaderOptions options, GameObject wrapperGameObject = null, string basePath = null, AssimpInterop.DataCallback dataCallback = null, AssimpInterop.ExistsCallback existsCallback = null)
        {
            if (basePath == null)
            {
                basePath = FileUtils.GetFileDirectory(assetExtension);
            }
            InternalLoadFromMemoryAndZip(fileData, assetExtension, basePath, options, wrapperGameObject != null, dataCallback, existsCallback);
            return BuildGameObject(options, assetExtension, wrapperGameObject);
        }
    }
}

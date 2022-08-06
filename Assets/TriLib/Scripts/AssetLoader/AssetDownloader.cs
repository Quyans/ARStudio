using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
namespace TriLib
{
    /// <summary>
    /// Represents an asset downloader.
    /// Add this MonoBehaviour to any GameObject and set AssetURI and AssetExtension to automatically download the referenced model.
    /// </summary>
    /// <remarks>
    /// Enable TRILIB_USE_ZIP compiler symbol manually or tick "Enable Zip loading" on TriLib menu to enable ZIP files processing.
    /// </remarks>
    public class AssetDownloader : MonoBehaviour
    {
        /// <summary>
        /// Use this field to set the asset URI.
        /// </summary>
        public string AssetURI;

        /// <summary>
        /// Use this field to set the download timeout in seconds.
        /// </summary>
        public int Timeout = 2000;

        /// <summary>
        /// Use this field to set the asset extension. (Eg: ".FBX")
        /// </summary>
        /// <remarks>
        /// Use ".ZIP" for ZIP files.
        /// </remarks>
        public string AssetExtension;

        /// <summary>
        /// Use this field to set the <see cref="UnityEngine.GameObject"/> to load the asset into.
        /// </summary>
        public GameObject WrapperGameObject;

        /// <summary>
        /// Enable this field to show loading progress on GUI.
        /// </summary>
        public bool ShowProgress;

        /// <summary>
        /// Enable this field to load models asynchronously.
        /// </summary>
        public bool Async;

        /// <summary>
        /// The <see cref="UnityWebRequest"/> used to download the asset.
        /// </summary>
        private UnityWebRequest _unityWebRequest;

        /// <summary>
        /// The centered <see cref="GUIStyle"/> used to display the progress.
        /// </summary>
        private GUIStyle _centeredStyle;

        /// <summary>
        /// The last error message.
        /// </summary>
        private string _error;

        /// <summary>
        /// Gets a value indicating whether the download has started.
        /// </summary>
        /// <value><c>true</c> if this download has started; otherwise, <c>false</c>.</value>
        public bool HasStarted
        {
            get
            {
                return _unityWebRequest != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the download is done.
        /// </summary>
        /// <value><c>true</c> if this download is done; otherwise, <c>false</c>.</value>
        public bool IsDone
        {
            get
            {
                return _unityWebRequest != null && _unityWebRequest.isDone;
            }
        }

        /// <summary>
        /// Gets the last error message.
        /// </summary>
        /// <value>The error message.</value>
        public string Error
        {
            get
            {
                if (HasStarted)
                {
                    return _unityWebRequest.error ?? _error;
                }
                return _error;
            }
        }

        /// <summary>
        /// Gets the download progress.
        /// </summary>
        /// <value>The download progress.</value>
        public float Progress
        {
            get
            {
                if (!HasStarted)
                {
                    return 0f;
                }
                return _unityWebRequest.downloadProgress;
            }
        }

        /// <summary>
        /// Checks the AssetURI and AssetExtension and starts downloading the asset, if both are avaliable.
        /// </summary>
        protected void Start()
        {
            if (!string.IsNullOrEmpty(AssetURI) && !string.IsNullOrEmpty(AssetExtension))
            {
                DownloadAsset(AssetURI, AssetExtension, null, null, null, WrapperGameObject);
            }
        }

        /// <summary>
        /// Shows the progress on screen, if ShowProgress is set to <c>true</c>.
        /// </summary>
        protected void OnGUI()
        {
            if (!ShowProgress || !HasStarted || IsDone)
            {
                return;
            }
            if (_centeredStyle == null)
            {
                _centeredStyle = GUI.skin.GetStyle("Label");
                _centeredStyle.alignment = TextAnchor.UpperCenter;
            }
            var centeredRect = new Rect(Screen.width / 2f - 100f, Screen.height / 2f - 25f, 200f, 50f);
            GUI.Label(centeredRect, string.Format("Downloaded {0:P2}", Progress), _centeredStyle);
        }

        /// <summary>
        /// Downloads an asset.
        /// </summary>
        /// <returns><c>true</c>, if asset was downloaded, <c>false</c> otherwise.</returns>
        /// <param name="assetURI">Asset URI.</param>
        /// <param name="assetExtension">Asset extension.</param>
        /// <param name="onAssetLoaded">On asset loaded event.</param>
        /// <param name="onTexturePreLoad">On texture pre load event.</param>
        /// <param name="options">Asset loading options.</param>
        /// <param name="wrapperGameObject">Wrapper <see cref="UnityEngine.GameObject"/> to load the asset into.</param>
        public bool DownloadAsset(string assetURI, string assetExtension, ObjectLoadedHandle onAssetLoaded = null, TexturePreLoadHandle onTexturePreLoad = null, AssetLoaderOptions options = null, GameObject wrapperGameObject = null)
        {
            if (HasStarted && !IsDone)
            {
                return false;
            }
            AssetURI = assetURI;
            AssetExtension = assetExtension;
            WrapperGameObject = wrapperGameObject;
            StartCoroutine(DoDownloadAsset(assetURI, assetExtension, onAssetLoaded, options, wrapperGameObject));
            return true;
        }

        /// <summary>
        /// Internal asset download coroutine.
        /// </summary>
        /// <returns>The coroutine IEnumerator.</returns>
        /// <param name="assetUri">Asset URI.</param>
        /// <param name="assetExtension">Asset extension.</param>
        /// <param name="onAssetLoaded">On asset loaded event.</param>
        /// <param name="options">Asset loading options.</param>
        /// <param name="wrapperGameObject">Wrapper <see cref="UnityEngine.GameObject"/> to load the asset into.</param>
        private IEnumerator DoDownloadAsset(string assetUri, string assetExtension, ObjectLoadedHandle onAssetLoaded, AssetLoaderOptions options = null, GameObject wrapperGameObject = null)
        {
            _unityWebRequest = UnityWebRequest.Get(assetUri);
            _unityWebRequest.timeout = Timeout;
#if UNITY_2017_3_OR_NEWER
            yield return _unityWebRequest.SendWebRequest();
#else
            yield return _unityWebRequest.Send();
#endif
            if (string.IsNullOrEmpty(_unityWebRequest.error))
            {
                var data = _unityWebRequest.downloadHandler.data;
                try
                {
                    if (Async)
                    {
                        using (var assetLoaderAsync = new AssetLoaderAsync())
                        {
                            assetLoaderAsync.LoadFromMemoryWithTextures(data, assetExtension, options, wrapperGameObject,
                                onAssetLoaded);
                        }
                    }
                    else
                    {
                        using (var assetLoader = new AssetLoader())
                        {
                            assetLoader.OnObjectLoaded += onAssetLoaded;
                            assetLoader.LoadFromMemoryWithTextures(data, assetExtension, options, wrapperGameObject);
                        }
                    }
                }
                catch (Exception exception)
                {
                    _error = exception.ToString();
                    if (onAssetLoaded != null)
                    {
                        onAssetLoaded(null);
                    }
                }
            }
            else
            {
                onAssetLoaded(null);
            }
            _unityWebRequest.Dispose();
            _unityWebRequest = null;
        }
    }
}


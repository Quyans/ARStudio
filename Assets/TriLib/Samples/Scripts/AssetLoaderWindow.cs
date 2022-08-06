using UnityEngine;

namespace TriLib
{
    namespace Samples
    {
        /// <summary>
        /// Represents the asset loader UI component.
        /// </summary>
        [RequireComponent(typeof(AssetDownloader))]
        public class AssetLoaderWindow : MonoBehaviour
        {
            /// <summary>
            /// Class singleton.
            /// </summary>
            public static AssetLoaderWindow Instance { get; private set; }

            /// <summary>
            /// Turn on this field to enable async loading.
            /// </summary>
            public bool Async;

            /// <summary>
            /// "Load local asset button" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Button _loadLocalAssetButton;
            /// <summary>
            /// "Load remote button" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Button _loadRemoteAssetButton;
            /// <summary>
            /// "Spinning text" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Text _spinningText;
            /// <summary>
            /// "Cutout toggle" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Toggle _cutoutToggle;
            /// <summary>
            /// "Spin X toggle" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Toggle _spinXToggle;
            /// <summary>
            /// "Spin Y toggle" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Toggle _spinYToggle;
            /// <summary>
            /// "Reset rotation button" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Button _resetRotationButton;
            /// <summary>
            /// "Stop animation button" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Button _stopAnimationButton;
            /// <summary>
            /// "Animations text" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Text _animationsText;
            /// <summary>
            /// "Animations scroll rect "reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.ScrollRect _animationsScrollRect;
            /// <summary>
            /// "Animations scroll rect container" reference.
            /// </summary>
            [SerializeField]
            private Transform _containerTransform;
            /// <summary>
            /// <see cref="AnimationText"/> prefab reference.
            /// </summary>
            [SerializeField]
            private AnimationText _animationTextPrefab;
            /// <summary>
            /// "Background (gradient) canvas" reference.
            /// </summary>
            [SerializeField]
            private Canvas _backgroundCanvas;
            /// <summary>
            /// Loaded Game Object reference.
            /// </summary>
            private GameObject _rootGameObject;
            
            /// <summary>
            /// Handles events from <see cref="AnimationText"/>.
            /// </summary>
            /// <param name="animationName">Choosen animation name.</param>
            public void HandleEvent(string animationName)
            {
                _rootGameObject.GetComponent<Animation>().Play(animationName);
                _stopAnimationButton.interactable = true;
            }

            /// <summary>
            /// Destroys all objects in the container.
            /// </summary>
            public void DestroyItems()
            {
                foreach (Transform innerTransform in _containerTransform)
                {
                    Destroy(innerTransform.gameObject);
                }
            }

            /// <summary>
            /// Initializes variables.
            /// </summary>
            protected void Awake()
            {
                _loadLocalAssetButton.onClick.AddListener(LoadLocalAssetButtonClick);
                _loadRemoteAssetButton.onClick.AddListener(LoadRemoteAssetButtonClick);
                _stopAnimationButton.onClick.AddListener(StopAnimationButtonClick);
                _resetRotationButton.onClick.AddListener(ResetRotationButtonClick);
                HideControls();
                Instance = this;
            }

            /// <summary>
            /// Spins the loaded Game Object if options are enabled.
            /// </summary>
            protected void Update()
            {
                if (_rootGameObject != null)
                {
                    _rootGameObject.transform.Rotate(_spinXToggle.isOn ? 20f * Time.deltaTime : 0f,
                        _spinYToggle.isOn ? -20f * Time.deltaTime : 0f, 0f, Space.World);
                }
            }

            /// <summary>
            /// Hides user controls.
            /// </summary>
            private void HideControls()
            {
                _loadLocalAssetButton.interactable = true;
                _loadRemoteAssetButton.interactable = true;
                _spinningText.gameObject.SetActive(false);
                _spinXToggle.gameObject.SetActive(false);
                _spinYToggle.gameObject.SetActive(false);
                _resetRotationButton.gameObject.SetActive(false);
                _stopAnimationButton.gameObject.SetActive(false);
                _animationsText.gameObject.SetActive(false);
                _animationsScrollRect.gameObject.SetActive(false);
            }

            /// <summary>
            /// Handles "Load local asset button" click event and tries to load an asset at chosen path.
            /// </summary>
            private void LoadLocalAssetButtonClick()
            {
                var fileOpenDialog = FileOpenDialog.Instance;
                fileOpenDialog.Title = "Please select a File";
                fileOpenDialog.Filter = AssetLoaderBase.GetSupportedFileExtensions() + "*.zip;";
#if (UNITY_WINRT && !UNITY_EDITOR_WIN)
                fileOpenDialog.ShowFileOpenDialog(delegate (byte[] fileBytes, string filename) 
                {
                    LoadInternal(filename, fileBytes);
#else
                fileOpenDialog.ShowFileOpenDialog(delegate (string filename)
                {
#endif
                    LoadInternal(filename, null);
                }
                );
            }

            /// <summary>
            /// Loads a model from the given filename or given file bytes.
            /// </summary>
            /// <param name="filename">Model filename.</param>
            /// <param name="fileBytes">Model file bytes.</param>
            private void LoadInternal(string filename, byte[] fileBytes = null)
            {
                PreLoadSetup();
                var assetLoaderOptions = GetAssetLoaderOptions();
                if (!Async)
                {
                    using (var assetLoader = new AssetLoader())
                    {
                        assetLoader.OnMetadataProcessed += AssetLoader_OnMetadataProcessed;
                        try
                        {
#if (UNITY_WINRT && !UNITY_EDITOR_WIN)
                        var extension = FileUtils.GetFileExtension(filename);
                        _rootGameObject = assetLoader.LoadFromMemoryWithTextures(fileBytes, extension, assetLoaderOptions, _rootGameObject);
#else
                            if (fileBytes != null && fileBytes.Length > 0)
                            {
                                var extension = FileUtils.GetFileExtension(filename);
                                _rootGameObject = assetLoader.LoadFromMemoryWithTextures(fileBytes, extension, assetLoaderOptions, _rootGameObject);
                            }
                            else if (!string.IsNullOrEmpty(filename))
                            {
                                _rootGameObject = assetLoader.LoadFromFileWithTextures(filename, assetLoaderOptions);
                            }
                            else
                            {
                                throw new System.Exception("File not selected");
                            }
#endif
                        }
                        catch (System.Exception exception)
                        {
                            ErrorDialog.Instance.ShowDialog(exception.ToString());
                        }
                    }
                    if (_rootGameObject != null)
                    {
                        PostLoadSetup();
                    }
                }
                else
                {
                    using (var assetLoader = new AssetLoaderAsync())
                    {
                        assetLoader.OnMetadataProcessed += AssetLoader_OnMetadataProcessed;
                        try
                        {
                            if (fileBytes != null && fileBytes.Length > 0)
                            {
                                var extension = FileUtils.GetFileExtension(filename);
                                assetLoader.LoadFromMemoryWithTextures(fileBytes, extension, assetLoaderOptions, null, delegate (GameObject loadedGameObject)
                                {
                                    _rootGameObject = loadedGameObject;
                                    if (_rootGameObject != null)
                                    {
                                        PostLoadSetup();
                                    }
                                });
                            }
                            else if (!string.IsNullOrEmpty(filename))
                            {
                                assetLoader.LoadFromFileWithTextures(filename, assetLoaderOptions, null, delegate (GameObject loadedGameObject)
                                {
                                    _rootGameObject = loadedGameObject;
                                    if (_rootGameObject != null)
                                    {
                                        PostLoadSetup();
                                    }
                                });
                            }
                            else
                            {
                                throw new System.Exception("File not selected");
                            }
                        }
                        catch (System.Exception exception)
                        {
                            ErrorDialog.Instance.ShowDialog(exception.ToString());
                        }
                    }
                }
            }

            /// <summary>
            /// Event assigned to FBX metadata loading. Editor debug purposes only.
            /// </summary>
            /// <param name="metadataType">Type of loaded metadata</param>
            /// <param name="metadataIndex">Index of loaded metadata</param>
            /// <param name="metadataKey">Key of loaded metadata</param>
            /// <param name="metadataValue">Value of loaded metadata</param>
            private void AssetLoader_OnMetadataProcessed(AssimpMetadataType metadataType, uint metadataIndex, string metadataKey, object metadataValue)
            {
                Debug.Log("Found metadata of type [" + metadataType + "] at index [" + metadataIndex + "] and key [" + metadataKey + "] with value [" + metadataValue + "]");
            }

            /// <summary>
            /// Gets the asset loader options.
            /// </summary>
            /// <returns>The asset loader options.</returns>
            private AssetLoaderOptions GetAssetLoaderOptions()
            {
                var assetLoaderOptions = AssetLoaderOptions.CreateInstance();
                assetLoaderOptions.DontLoadCameras = false;
                assetLoaderOptions.DontLoadLights = false;
                assetLoaderOptions.UseCutoutMaterials = _cutoutToggle.isOn;
                assetLoaderOptions.AddAssetUnloader = true;
                return assetLoaderOptions;
            }

            /// <summary>
            /// Pre Load setup.
            /// </summary>
            private void PreLoadSetup()
            {
                HideControls();
                if (_rootGameObject != null)
                {
                    Destroy(_rootGameObject);
                    _rootGameObject = null;
                }
            }

            /// <summary>
            /// Post load setup.
            /// </summary>
            private void PostLoadSetup()
            {
                var mainCamera = Camera.main;
                mainCamera.FitToBounds(_rootGameObject.transform, 3f);
                _backgroundCanvas.planeDistance = mainCamera.farClipPlane * 0.99f;
                _spinningText.gameObject.SetActive(true);
                _spinXToggle.isOn = false;
                _spinXToggle.gameObject.SetActive(true);
                _spinYToggle.isOn = false;
                _spinYToggle.gameObject.SetActive(true);
                _resetRotationButton.gameObject.SetActive(true);
                DestroyItems();
                var rootAnimation = _rootGameObject.GetComponent<Animation>();
                if (rootAnimation != null)
                {
                    _animationsText.gameObject.SetActive(true);
                    _animationsScrollRect.gameObject.SetActive(true);
                    _stopAnimationButton.gameObject.SetActive(true);
                    _stopAnimationButton.interactable = false;
                    foreach (AnimationState animationState in rootAnimation)
                    {
                        CreateItem(animationState.name);
                    }
                }
            }

            /// <summary>
            /// Handles "Load remote asset button" click event and tries to load the asset at chosen URI.
            /// </summary>
            private void LoadRemoteAssetButtonClick()
            {
                URIDialog.Instance.ShowDialog(delegate (string assetURI, string assetExtension)
                {
                    var assetDownloader = GetComponent<AssetDownloader>();
                    assetDownloader.DownloadAsset(assetURI, assetExtension, LoadDownloadedAsset, null, GetAssetLoaderOptions(), null);
                    _loadLocalAssetButton.interactable = false;
                    _loadRemoteAssetButton.interactable = false;
                });
            }

            /// <summary>
            /// Loads the downloaded asset.
            /// </summary>
            /// <param name="loadedGameObject">Loaded game object.</param>
            private void LoadDownloadedAsset(GameObject loadedGameObject)
            {
                PreLoadSetup();
                if (loadedGameObject != null)
                {
                    _rootGameObject = loadedGameObject;
                    PostLoadSetup();
                }
                else
                {
                    var assetDownloader = GetComponent<AssetDownloader>();
                    ErrorDialog.Instance.ShowDialog(assetDownloader.Error);
                }
            }

            /// <summary>
            /// Creates a <see cref="AnimationText"/> item in the container.
            /// </summary>
            /// <param name="text">Text of the <see cref="AnimationText"/> item.</param>
            private void CreateItem(string text)
            {
                var instantiated = Instantiate(_animationTextPrefab, _containerTransform);
                instantiated.Text = text;
            }

            /// <summary>
            /// Handles the "Reset Rotation button" click event and stops the loaded Game Object spinning. 
            /// </summary>
            private void ResetRotationButtonClick()
            {
                _spinXToggle.isOn = false;
                _spinYToggle.isOn = false;
                _rootGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }

            /// <summary>
            /// Handles the "Stop Animation button" click event and stops the loaded Game Object animation.
            /// </summary>
            private void StopAnimationButtonClick()
            {
                _rootGameObject.GetComponent<Animation>().Stop();
                _stopAnimationButton.interactable = false;
            }
        }
    }
}

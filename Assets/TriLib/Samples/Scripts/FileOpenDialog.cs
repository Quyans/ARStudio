using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace TriLib
{
    namespace Samples
    {
        #if (UNITY_WINRT && !UNITY_EDITOR_WIN)
        /// <summary>
        /// Represents a <see cref="FileOpenDialog"/> file opening event.
        /// </summary>
        /// <param name="fileBytes">File data bytes.</param>
        /// <param name="filename">Original filename.</param>
        public delegate void FileOpenEventHandle(byte[] fileBytes, string filename);
        #else
        /// <summary>
        /// Represents a <see cref="FileOpenDialog"/> file opening event.
        /// </summary>
        /// <param name="filename">Choosen filename.</param>
        public delegate void FileOpenEventHandle(string filename);
        #endif
        /// <summary>
        /// File dialog item type enumeration for event handling.
        /// </summary>
        public enum ItemType
        {
            ParentDirectory,
            Directory,
            File
        }
        /// <summary>
        /// Represents a file loader UI component.
        /// </summary>
        public class FileOpenDialog : MonoBehaviour
        {
            /// <summary>
            /// Class singleton.
            /// </summary>
            public static FileOpenDialog Instance { get; private set; }
            /// <summary>
            /// File dialog filter.
            /// </summary>
            public string Filter = "*.*";
            /// <summary>
            /// Gets/Sets the file dialog title.
            /// </summary>
            public string Title
            {
                get
                {
                    return _headerText.text;
                }
                set
                {
                    _headerText.text = value;
                }
            }
            /// <summary>
            /// Event that occurs when user choose a file.
            /// </summary>
            private event FileOpenEventHandle OnFileOpen;
            /// <summary>
            /// "Container Transform" reference.
            /// </summary>
            [SerializeField]
            private Transform _containerTransform;
            /// <summary>
            /// "<see cref="FileText"/> prefab" reference.
            /// </summary>
            [SerializeField]
            private FileText _fileTextPrefab;
            /// <summary>
            /// "Inner Game Object" reference.
            /// </summary>
            [SerializeField]
            private GameObject _fileLoaderRenderer;
            /// <summary>
            /// "Close button" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Button _closeButton;
            /// <summary>
            /// "Header text" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Text _headerText;
            /// <summary>
            /// Working directory.
            /// </summary>
            private string _directory;

#if (UNITY_WINRT && !UNITY_EDITOR_WIN)
			/// <summary>
			/// File open event handler.
			/// </summary>
            private FileOpenEventHandle _onFileOpen;

			/// <summary>
			/// Pending file data.
			/// </summary>
            private byte[] _pendingFile;

			/// <summary>
			/// Pendinf file filename.
			/// </summary>
            private string _pendingFilename;

            /// <summary>
            /// Initializes variables.
            /// </summary>
            protected void Awake()
            {
                Instance = this;
            }

			/// <summary>
			/// Check for pending file.
			/// </summary>
            protected void Update() {
                if (_pendingFile != null && _onFileOpen != null && _pendingFilename != null) {
                    _onFileOpen(_pendingFile, _pendingFilename);
                    _pendingFile = null;
                    _onFileOpen = null;
                    _pendingFilename = null;
                }
            }

			/// <summary>
			/// Shows the file open dialog.
			/// </summary>
			/// <param name="onFileOpen">Event that occurs when user choose a file.</param>
			public void ShowFileOpenDialog(FileOpenEventHandle onFileOpen)
			{
                UnityEngine.WSA.Application.InvokeOnUIThread(async () => {
                    var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
                    filePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Objects3D;
                    filePicker.FileTypeFilter.Add("*");        
                    var file = await filePicker.PickSingleFileAsync();
                    if (file != null) {
                        _onFileOpen = onFileOpen;
                        _pendingFile = await UWPUtils.ReadStorageFileIntoBuffer(file);
                        _pendingFilename = file.Path;
                    }
                }, false);
			}

			/// <summary>
			/// Handles events from <see cref="FileText"/>.
			/// </summary>
			/// <param name="itemType">Clicked item type.</param>
			/// <param name="filename">Clicked item filename, if exists.</param>
			public void HandleEvent(ItemType itemType, string filename)
			{
			
			}
#else

            /// <summary>
            /// Shows the file open dialog.
            /// </summary>
            /// <param name="onFileOpen">Event that occurs when user choose a file.</param>
            public void ShowFileOpenDialog(FileOpenEventHandle onFileOpen)
            {
                OnFileOpen = onFileOpen;
                ReloadItemNames();
                _fileLoaderRenderer.SetActive(true);
            }

            /// <summary>
            /// Hides the file open dialog.
            /// </summary>
            public void HideFileOpenDialog()
            {
                DestroyItems();
                _fileLoaderRenderer.SetActive(false);
            }

            /// <summary>
            /// Handles events from <see cref="FileText"/>.
            /// </summary>
            /// <param name="itemType">Clicked item type.</param>
            /// <param name="filename">Clicked item filename, if exists.</param>
            public void HandleEvent(ItemType itemType, string filename)
            {
                switch (itemType)
                {
				case ItemType.ParentDirectory:
					var parentDirectory = Directory.GetParent (_directory);
					if (parentDirectory != null) {
						_directory = parentDirectory.FullName;
						ReloadItemNames ();
					} else {
						ShowDirectoryNames ();
					}
                        break;
                    case ItemType.Directory:
                        _directory = filename;
                        ReloadItemNames();
                        break;
                    default:
                        OnFileOpen(Path.Combine(_directory, filename));
                        HideFileOpenDialog();
                        break;
                }
            }

            /// <summary>
            /// Destroys all game objects in the container.
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
                _directory = Path.GetDirectoryName(Application.dataPath);
                _closeButton.onClick.AddListener(HideFileOpenDialog);
                Instance = this;
            }

            /// <summary>
            /// Reloads directory info and creates container items.
            /// </summary>
            private void ReloadItemNames()
            {
                DestroyItems();
                CreateItem(ItemType.ParentDirectory, "[Parent Directory]");
                var directories = Directory.GetDirectories(_directory);
                foreach (var directory in directories)
                {
                    CreateItem(ItemType.Directory, directory);
                }
                var files = Directory.GetFiles(_directory, "*.*");
                if (!string.IsNullOrEmpty(Filter) && Filter != "*.*")
                {
                    files = files.Where(x => Filter.Contains(Path.GetExtension(x).ToLower())).ToArray();
                }
                foreach (var file in files)
                {
                    CreateItem(ItemType.File, Path.GetFileName(file));
                }
            }

			/// <summary>
			/// Shows the directory names.
			/// </summary>
			private void ShowDirectoryNames() {
				DestroyItems();
				var driveInfos = Directory.GetLogicalDrives();
				foreach (var driveInfo in driveInfos)
				{
					CreateItem(ItemType.Directory, driveInfo);
				}
			}

            /// <summary>
            /// Creates a <see cref="FileText"/> item in the container.
            /// </summary>
            /// <param name="itemType">Type of the item to be created.</param>
            /// <param name="text">Text of the item to be created.</param>
            private void CreateItem(ItemType itemType, string text)
            {
                var instantiated = Instantiate(_fileTextPrefab, _containerTransform);
                instantiated.ItemType = itemType;
                instantiated.Text = text;
			}
			#endif
        }
    }
}
using UnityEngine;
using System;

namespace TriLib
{
    namespace Samples
    {
        /// <summary>
        /// Represents the asset loader URI input dialog UI component.
        /// </summary>
        public class URIDialog : MonoBehaviour
        {
            /// <summary>
            /// Class singleton.
            /// </summary>
            public static URIDialog Instance { get; private set; }

            /// <summary>
            /// "OK button" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Button _okButton;

            /// <summary>
            /// "Cancel Button" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Button _cancelButton;

            /// <summary>
            /// "URI text" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.InputField _uriText;

            /// <summary>
            /// "Extension text" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.InputField _extensionText;

            /// <summary>
            /// "URI dialog inner Game Object" reference.
            /// </summary>
            [SerializeField]
            private GameObject _rendererGameObject;

            /// <summary>
            /// Gets or sets the filename.
            /// </summary>
            /// <value>The filename.</value>
            public string Filename
            {
                get { return _uriText.text; }
                set { _uriText.text = value; }
            }

            /// <summary>
            /// Gets or sets the extension.
            /// </summary>
            /// <value>The text.</value>
            public string Extension
            {
                get { return _extensionText.text; }
                set { _extensionText.text = value; }
            }

            /// <summary>
            /// Initializes variables.
            /// </summary>
            protected void Awake()
            {
                _cancelButton.onClick.AddListener(HideDialog);
                _uriText.onValueChanged.AddListener(UpdateExtension);
                Instance = this;
            }

            /// <summary>
            /// Shows the dialog.
            /// </summary>
            public void ShowDialog(Action<string, string> onOk)
            {
                _okButton.onClick.RemoveAllListeners();
                _okButton.onClick.AddListener(delegate
                {
                    if (onOk != null)
                    {
                        onOk(Filename, Extension);
                    }
                    HideDialog();
                });
                _rendererGameObject.SetActive(true);
            }

            /// <summary>
            /// Hides the dialog.
            /// </summary>
            public void HideDialog()
            {
                _rendererGameObject.SetActive(false);
            }

            /// <summary>
            /// Updates the extension text.
            /// </summary>
            private void UpdateExtension(string text)
            {
                _extensionText.text = FileUtils.GetFileExtension(text);
            }
        }
    }
}
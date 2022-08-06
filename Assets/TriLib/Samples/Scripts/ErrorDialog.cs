using UnityEngine;

namespace TriLib
{
    namespace Samples
    {
        /// <summary>
        /// Represents the asset loader error dialog UI component.
        /// </summary>
        public class ErrorDialog : MonoBehaviour
        {
            /// <summary>
            /// Class singleton.
            /// </summary>
            public static ErrorDialog Instance { get; private set; }

            /// <summary>
            /// "OK button" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Button _okButton;

            /// <summary>
            /// "Error text" reference.
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.InputField _errorText;

            /// <summary>
            /// "Error dialog inner Game Object" reference.
            /// </summary>
            [SerializeField]
            private GameObject _rendererGameObject;

            public string Text
            {
                get { return _errorText.text; }
                set { _errorText.text = value; }
            }

            /// <summary>
            /// Initializes variables.
            /// </summary>
            protected void Awake()
            {
                _okButton.onClick.AddListener(HideDialog);
                Instance = this;
            }

            /// <summary>
            /// Shows the error dialog.
            /// </summary>
            /// <param name="text">Error text to display.</param>
            public void ShowDialog(string text)
            {
                Text = text;
                _rendererGameObject.SetActive(true);
            }

            /// <summary>
            /// Hides the error dialog.
            /// </summary>
            public void HideDialog()
            {
                _rendererGameObject.SetActive(false);
            }
        }
    }
}
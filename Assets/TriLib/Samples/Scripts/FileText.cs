using UnityEngine;
using UnityEngine.EventSystems;

namespace TriLib
{
    namespace Samples
    {
        /// <summary>
        /// Represents a file, folder or "go to parent" item for <see cref="FileOpenDialog"/> class.
        /// </summary>
        public class FileText : MonoBehaviour, ISelectHandler
        {
            /// <summary>
            /// Gets/Sets the related UI component text.
            /// </summary>
            public string Text
            {
                get
                {
                    return GetComponent<UnityEngine.UI.Text>().text;
                }
                set
                {
                    GetComponent<UnityEngine.UI.Text>().text = value;
                }
            }
            /// <summary>
            /// Item type (file, folder or "go to parent").
            /// </summary>
            public ItemType ItemType { get; set; }
            /// <summary>
            /// Calls <see cref="FileOpenDialog"/> HandleEvent function, indicating that user clicked this item.
            /// </summary>
            /// <param name="eventData">Contains the base event data that is common to all event types in the new Unity EventSystem.</param>
            public void OnSelect(BaseEventData eventData)
            {
                FileOpenDialog.Instance.HandleEvent(ItemType, Text);
            }
        }
    }
}
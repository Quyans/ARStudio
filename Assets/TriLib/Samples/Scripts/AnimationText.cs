using UnityEngine;
using UnityEngine.EventSystems;

namespace TriLib
{
    namespace Samples
    {
        /// <summary>
        /// Represents an animation entry UI component.
        /// </summary>
        public class AnimationText : MonoBehaviour, ISelectHandler
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
            /// Calls <see cref="AssetLoaderWindow"/> HandleEvent function, indicating that user clicked this item.
            /// </summary>
            /// <param name="eventData">Contains the base event data that is common to all event types in the new Unity EventSystem.</param>
            public void OnSelect(BaseEventData eventData)
            {
                AssetLoaderWindow.Instance.HandleEvent(Text);
            }
        }
    }
}

using UnityEngine;

namespace TriLib {
    namespace Samples {
        /// <summary>
        /// A simple rotator class (rotates the object).
        /// </summary>
        public class SimpleRotator : MonoBehaviour {
            /// <summary>
            /// Rotates the object.
            /// </summary>
            protected void Update () {
                transform.Rotate(-10f * Time.deltaTime, -10f * Time.deltaTime, 0f);
            }
        }
    }
}


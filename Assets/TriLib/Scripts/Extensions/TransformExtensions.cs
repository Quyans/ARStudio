using UnityEngine;

namespace TriLib
{
    /// <summary>
    /// Represents a series of <see cref="UnityEngine.Transform"/> extension methods.
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Assigns a <see cref="UnityEngine.Matrix4x4"/> into <see cref="UnityEngine.Transform"/> components.
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform"/> to assign the <see cref="UnityEngine.Matrix4x4"/>.</param>
        /// <param name="matrix"><see cref="UnityEngine.Matrix4x4"/> to assign.</param>
        /// <param name="local">
        /// If true, copies the <see cref="UnityEngine.Matrix4x4"/> components to <see cref="UnityEngine.Transform"/> local components.
        /// Otherwise, copies the <see cref="UnityEngine.Matrix4x4"/> components to <see cref="UnityEngine.Transform"/> world components.
        /// </param>
        /// <example>
        /// @code
        ///     //Creates a matrix that moves 100 units forward at world space, then rotates 90 units on y-axis at local space
        ///     var myMatrix = Matrix4x4.TRS(new Vector3(0f, 0f, 100f), Quaternion.Euler(0f, 90f, 0f), Vector3.one);
        ///     //Assigns the position, rotation and local scale contained in the "myMatrix" into "myGameObject"
        ///     myGameObject.transform.LoadMatrix(myMatrix);
        /// @endcode
        /// </example>
        public static void LoadMatrix(this Transform transform, Matrix4x4 matrix, bool local = true)
        {
            if (local)
            {
                transform.localScale = matrix.ExtractScale();
                transform.localRotation = matrix.ExtractRotation();
                transform.localPosition = matrix.ExtractPosition();
            }
            else
            {
                transform.rotation = matrix.ExtractRotation();
                transform.position = matrix.ExtractPosition();
            }
        }

        /// <summary>
        /// Encapsulates the <see cref="UnityEngine.Transform"/> childs bounds on new bounds.
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform"/> to encapsulate.</param>
        /// <returns>Encapsulated <see cref="UnityEngine.Bounds"/>.</returns>
        /// <example>
        /// @code
        ///    //Gets the bounds that represents all children extents
        ///    var bounds = myTransform.EncapsulateBounds();
        ///    //Gets the bounds radius
        ///    var boundRadius = bounds.extents.magnitude;
        ///    //Calculates a distance from "myTransform" for camera framing
        ///    var distance = (boundRadius / (2.0f * Mathf.Tan(0.5f * camera.fieldOfView * Mathf.Deg2Rad))) * radius;
        ///    //Returns if the distance isn´t numeric (caused by empty bounds)
        ///    if (System.Single.IsNaN(distance))
        ///    {
        ///        return;
        ///    }
        ///    //Sets the camera far clip plane to the double of the acquired distance 
        ///    camera.farClipPlane = distance * 2f;
        ///    //Puts the camera at the acquired distance relative to the bounds
        ///    camera.transform.position = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z + distance);
        ///    //Looks at the bounds center with the camera
        ///    camera.transform.LookAt(bounds.center);
        /// @endcode
        /// </example>
        public static Bounds EncapsulateBounds(this Transform transform)
        {
            Bounds bounds;
            var renderers = transform.GetComponentsInChildren<Renderer>();
            if (renderers != null && renderers.Length > 0)
            {
                bounds = renderers[0].bounds;
                for (var i = 1; i < renderers.Length; i++)
                {
                    var renderer = renderers[i];
                    bounds.Encapsulate(renderer.bounds);
                }
            } else
            {
                bounds = new Bounds();
            }
            return bounds;
        }

        /// <summary>
        //  Finds a <see cref="UnityEngine.Transform"/> in a deep hierarchy level.
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform"/> to search in.</param>
        /// <param name="name">Name of the object to search for.</param>
        /// <param name="endsWith">When true, search for transforms that ends with 'name' value.</param>
        /// <returns>The <see cref="UnityEngine.Transform"/> if found, otherwise, null.</returns>
        public static Transform FindDeepChild(this Transform transform, string name, bool endsWith = false)
        {
            if (endsWith ? transform.name == name : transform.name.EndsWith(name))
            {
                return transform;
            }
            foreach (Transform child in transform)
            {
                var result = child.FindDeepChild(name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Destroys the children <see cref="UnityEngine.GameObject"/> of this <see cref="UnityEngine.Transform"/>.
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform"/> to destroy children from.</param>
        /// <param name="destroyImmediate">When <c>true</c> destroy children immediately.</param> 
        public static void DestroyChildren(this Transform transform, bool destroyImmediate = false)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (destroyImmediate)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
                else
                {
                    Object.Destroy(child.gameObject);
                }
            }
        }
    }
}


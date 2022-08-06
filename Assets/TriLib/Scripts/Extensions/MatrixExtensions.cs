using UnityEngine;

namespace TriLib
{
    /// <summary>
    /// Represents a series of <see cref="UnityEngine.Matrix4x4"/> extension methods.
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Extracts the rotation from a <see cref="UnityEngine.Matrix4x4"/>.
        /// </summary>
        /// <param name="matrix"><see cref="UnityEngine.Matrix4x4"/> to extract from.</param>
        /// <returns>Extracted rotation <see cref="UnityEngine.Quaternion"/>.</returns>
        /// <example>
        /// @code
        ///     //Creates a Matrix4x4 that moves 100 units forward in World Space, then rotate 90 units on y-Axis in Local Space
        ///     var myMatrix = Matrix4x4.TRS(new Vector3(0f, 0f, 100f), Quaternion.Euler(0f, 90f, 0f), Vector3.one);
        ///     //Assigns the rotation contained in the "myMatrix" to "myGameObject"
        ///     myGameObject.transform.rotation = myMatrix.ExtractRotation(); 
        /// @endcode
        /// </example>
        public static Quaternion ExtractRotation(this Matrix4x4 matrix)
        {
            return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
        }

        /// <summary>
        /// Extracts the Position from a <see cref="UnityEngine.Matrix4x4"/>.
        /// </summary>
        /// <param name="matrix"><see cref="UnityEngine.Matrix4x4"/> to extract from.</param>
        /// <returns>Extracted position <see cref="UnityEngine.Vector3"/>.</returns>
        /// <example>
        /// @code
        ///     //Creates a Matrix4x4 that moves 100 units forward in World Space, then rotate 90 units on y-Axis in Local Space
        ///     var myMatrix = Matrix4x4.TRS(new Vector3(0f, 0f, 100f), Quaternion.Euler(0f, 90f, 0f), Vector3.one);
        ///     //Assigns the position contained in the "myMatrix" to "myGameObject"
        ///     myGameObject.transform.position = myMatrix.ExtractRotation(); 
        /// @endcode
        /// </example>
        public static Vector3 ExtractPosition(this Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        /// <summary>
        /// Extracts the scale from a <see cref="UnityEngine.Matrix4x4"/>.
        /// </summary>
        /// <param name="matrix"><see cref="UnityEngine.Matrix4x4"/> to extract from.</param>
        /// <returns>Extracted scale <see cref="UnityEngine.Vector3"/>.</returns>
        /// <example>
        /// @code
        ///     //Creates a matrix that moves 100 units forward at world space, then rotates 90 units on y-axis at local space
        ///     var myMatrix = Matrix4x4.TRS(new Vector3(0f, 0f, 100f), Quaternion.Euler(0f, 90f, 0f), Vector3.one);
        ///     //Assigns the local scale contained in the "myMatrix" to "myGameObject"
        ///     myGameObject.transform.localScale = myMatrix.ExtractScale(); 
        /// @endcode
        /// </example>
        public static Vector3 ExtractScale(this Matrix4x4 matrix)
        {
            return new Vector3(matrix.GetColumn(0).magnitude, matrix.GetColumn(1).magnitude, matrix.GetColumn(2).magnitude);
        }
    }
}

using UnityEngine;

namespace TriLib
{
    /// <summary>
    /// Represents a series of <see cref="UnityEngine.Camera"/> extension methods.
    /// </summary>
    public static class CameraExtensions
    {
        /// <summary>
        /// Frames the <see cref="UnityEngine.Camera"/> to encapsulate the <see cref="UnityEngine.Transform"/> child bounds.
        /// </summary>
        /// <param name="camera"><see cref="UnityEngine.Camera"/> that will frame the transform.</param>
        /// <param name="transform"><see cref="UnityEngine.Transform"/> that will be framed.</param>
        /// <param name="distance">Distance from the transform to place the camera.</param>
        /// <example>
        /// @code
        /// protected void Awake() {
        ///     GameObject myGameObject;
        ///     try {
        ///         using (var assetLoader = new AssetLoader()) {
        ///             gameObject = assetLoader.LoadFromFile("mymodel.fbx");
        ///         }
        ///     } catch (Exception e) {
        ///         Debug.LogFormat("Unable to load myModel.fbx. The loader returned: {0}", e);
        ///     }
        ///     //Frames "myGameObject" with the main camera at 2 units of distance
        ///     Camera.Main.FitToBounds(myGameObject.transform, 2f);
        /// }
        /// @endcode
        /// </example>
        public static void FitToBounds(this Camera camera, Transform transform, float distance)
        {
            var bounds = transform.EncapsulateBounds();
            var boundRadius = bounds.extents.magnitude;
            var finalDistance = boundRadius/(2.0f*Mathf.Tan(0.5f*camera.fieldOfView*Mathf.Deg2Rad))*distance;
            if (float.IsNaN(finalDistance))
            {
                return;
            }
            camera.farClipPlane = finalDistance*2f;
            camera.transform.position = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z + finalDistance);
            camera.transform.LookAt(bounds.center);
        }
    }

}
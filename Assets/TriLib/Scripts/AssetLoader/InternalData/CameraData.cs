using UnityEngine;

namespace TriLib
{
    /// <summary>
    /// Internally represents a Unity <see cref="UnityEngine.Camera"/>.
    /// </summary>
    public class CameraData
    {
        public string Name;
        public float Aspect;
        public float NearClipPlane;
        public float FarClipPlane;
        public float FieldOfView;
        public Vector3 LocalPosition;
        public Vector3 Forward;
        public Vector3 Up;

        public Camera Camera;
    }
}
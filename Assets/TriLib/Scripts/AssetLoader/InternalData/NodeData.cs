using UnityEngine;

namespace TriLib
{
    /// <summary>
    /// Internally represents a Unity <see cref="UnityEngine.GameObject"/>.
    /// </summary>
    public class NodeData
    {
        public string Name;
        public string Path;
        public Matrix4x4 Matrix;
        public uint[] Meshes;
        public NodeData Parent;
        public NodeData[] Children;

        public GameObject GameObject;
    }
}
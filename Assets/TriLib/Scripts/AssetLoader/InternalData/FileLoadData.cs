using System;
using System.Collections.Generic;

namespace TriLib
{
    /// <summary>
    /// Represents a model path information, with GC buffers to store resources data.
    /// </summary>
    public class GCFileLoadData : FileLoadData
    {
        /// <summary>
        /// GC buffers used to store resources data.
        /// </summary>
        public List<System.Runtime.InteropServices.GCHandle> LockedBuffers = new List<System.Runtime.InteropServices.GCHandle>();

        ///<inheritdoc/>
        public override void Dispose()
        {
            foreach (var lockedBuffer in LockedBuffers)
            {
                lockedBuffer.Free();
            }
        }
    }

    /// <summary>
    /// Represents a model path information.
    /// This class is used to retrieve models path information when calling <see cref="AssimpInterop.DataCallback"/> and <see cref="AssimpInterop.ExistsCallback"/> callbacks.
    /// </summary>
    public class FileLoadData : IDisposable
    {
        /// <summary>
        /// Model filename.
        /// </summary>
        public string Filename;

        /// <summary>
        /// Model base path.
        /// </summary>
        public string BasePath;

        ///<inheritdoc/>
        public virtual void Dispose()
        {

        }
    }
}

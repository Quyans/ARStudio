using UnityEngine;
using System.IO;

namespace TriLib {
    /// <summary>
    /// Contains stream helper functions.
    /// </summary>
    public static class StreamUtils
    {
        /// <summary>
        /// Reads a full stream content into a byte array.
        /// </summary>
        /// <returns>The stream content into a byte array.</returns>
        /// <param name="stream">Input stream.</param>
        public static byte[] ReadFullStream(Stream stream)
        {
            var buffer = new byte[4096];
            using (var memoryStream = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, read);
                }
                return memoryStream.ToArray();
            }
        }
    }
}


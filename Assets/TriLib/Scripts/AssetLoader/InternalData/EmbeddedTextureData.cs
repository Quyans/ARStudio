namespace TriLib
{
    /// <summary>
    /// Represents a texture.
    /// </summary>
    public class EmbeddedTextureData
    {
        /// <summary>
        /// Texture pixel data.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Texture width.
        /// </summary>
        public int Width;

        /// <summary>
        /// Texture height.
        /// </summary>
        public int Height;

        /// <summary>
        /// Texture bits-per-pixel.
        /// </summary>
        public int NumChannels;

        /// <summary>
        /// <c>true</c> when the texture contains raw pixel data, <c>false</c> when it contains the file data.
        /// </summary>
        public bool IsRawData;
    }
}

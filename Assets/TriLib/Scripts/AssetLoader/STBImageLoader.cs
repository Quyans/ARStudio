#if !TRILIB_USE_UNITY_TEXTURE_LOADER
namespace STB
{
    public static class STBImageLoader
    {
        public static byte[] LoadTextureDataFromByteArray(byte[] bytes, out int width, out int height, out int channelsInFile)
        {
            var data = STBImage.STBImageInterop.LoadFromMemory(bytes, out width, out height, out channelsInFile, 4);
            return data;
        }
    }
}
#endif
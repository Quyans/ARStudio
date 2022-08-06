using System;
using System.Runtime.InteropServices;
#if !TRILIB_USE_UNITY_TEXTURE_LOADER
namespace STBImage
{
    public static class STBImageInterop
    {
#region DllPath
        /// <summary>
        /// Specifies the stb_image native library used in the bindings
        /// </summary>
#if (UNITY_WINRT && !UNITY_EDITOR_WIN)
		private const string DllPath = "stb_image-uwp";
#elif (!UNITY_WINRT && UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        private const string DllPath = "stb_image";
#elif (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
		private const string DllPath = "libstb_image";
#elif (UNITY_IOS && !UNITY_EDITOR)
		private const string DllPath = "__Internal";
#elif (UNITY_WEBGL && !UNITY_EDITOR)
		private const string DllPath = "__Internal";
#elif (UNITY_STANDALONE_LINUX)
		private const string DllPath = "stb_image";
#else
		private const string DllPath = "libstb_image";
#endif
#endregion DllPath

        [DllImport(DllPath, EntryPoint = "loadFromMemory")]
        private static extern IntPtr _loadFromMemory(IntPtr buffer, int len, ref int x, ref int y, ref int channelsInFile, int desiredChannels);

        [DllImport(DllPath, EntryPoint = "imageFree")]
        private static extern void _imageFree(IntPtr buffer);

        public static byte[] LoadFromMemory(byte[] data, out int outX, out int outY, out int outChannelsInFile, int desiredChannels)
        {
            var dataHandle = LockGc(data);
            outX = 0;
            var xHandle = LockGc(outX);
            outY = 0;
            var yHandle = LockGc(outY);
            outChannelsInFile = 0;
            var channelsInFileHandle = LockGc(outChannelsInFile);
            var dataPtr = _loadFromMemory(dataHandle.AddrOfPinnedObject(), data.Length, ref outX, ref outY, ref outChannelsInFile, desiredChannels);
            dataHandle.Free();
            xHandle.Free();
            yHandle.Free();
            channelsInFileHandle.Free();
            var dataLength = outX * outY * desiredChannels;
            var outData = new byte[dataLength];
            Marshal.Copy(dataPtr, outData, 0, dataLength);
            _imageFree(dataPtr);
            return outData;
        }

        private static GCHandle LockGc(object value)
        {
            return GCHandle.Alloc(value, GCHandleType.Pinned);
        }
    }
}
#endif
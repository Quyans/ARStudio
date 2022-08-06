using UnityEngine;
using System;

namespace TriLib
{
    /// <summary>
    /// Represents a texture compression parameter.
    /// </summary>
    public enum TextureCompression
    {
        /// <summary>
        /// No texture compression will be applied.
        /// </summary>
        None,

        /// <summary>
        /// Normal-quality texture compression will be applied.
        /// </summary>
        NormalQuality,

        /// <summary>
        /// High-quality texture compression will be applied.
        /// </summary>
        HighQuality
    }

    /// <summary>
    /// Represents a <see cref="UnityEngine.Texture2D"/> post-loading event handle.
    /// </summary>
    public delegate void TextureLoadHandle(string sourcePath, Material material, string propertyName, Texture2D texture);

    /// <summary>
    /// Represents a  <see cref="UnityEngine.Texture2D"/> pre-loading event handle.
    /// </summary>
    public delegate void TexturePreLoadHandle(IntPtr scene, string path, string name, Material material, string propertyName, ref bool checkAlphaChannel, TextureWrapMode textureWrapMode = TextureWrapMode.Repeat, string basePath = null, TextureLoadHandle onTextureLoaded = null, TextureCompression textureCompression = TextureCompression.None, bool isNormalMap = false);

    /// <summary>
    /// Represents a class to load external textures.
    /// </summary>
    public static class Texture2DUtils
    {
        public static Texture2D ProcessTexture(int width,
            int height,
            string name,
            ref bool hasAlphaChannel,
            byte[] data,
            bool isRawData = false,
            bool isNormalMap = false,
            TextureWrapMode textureWrapMode = TextureWrapMode.Repeat,
            TextureCompression textureCompression = TextureCompression.None,
            bool checkAlphaChannel = false,
            bool generateMipMaps = true
        )
        {
            if (data == null || data.Length == 0)
            {
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                    Debug.LogWarningFormat("Texture '{0}' not found", path);
#endif
                return null;
            }
            Texture2D tempTexture2D;
            if (ApplyTextureData(data, isRawData, out tempTexture2D, width, height, generateMipMaps))
            {
                return ProccessTextureData(tempTexture2D, name, ref hasAlphaChannel, textureWrapMode, textureCompression, isNormalMap, checkAlphaChannel);
            }
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
            Debug.LogErrorFormat("Unable to load texture '{0}'", path);
#endif
            return null;
        }

        private static bool ApplyTextureData(byte[] data, bool isRawData, out Texture2D outputTexture2D, int width, int height, bool generateMipMaps)
        {
            if (data.Length == 0)
            {
                outputTexture2D = null;
                return false;
            }
            if (isRawData)
            {
                try
                {
#if !TRILIB_USE_UNITY_TEXTURE_LOADER
                    outputTexture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
#else
                    outputTexture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
#endif
                    outputTexture2D.LoadRawTextureData(data);
                    outputTexture2D.Apply();
                    if (generateMipMaps)
                    {
                        var tempTexture2D = new Texture2D(width, height, TextureFormat.ARGB32, true);
                        tempTexture2D.SetPixels(outputTexture2D.GetPixels());
                        tempTexture2D.Apply(true);
                        outputTexture2D = tempTexture2D;
                    }
                    return true;
                }
#if TRILIB_OUTPUT_MESSAGES || ASSIMP_OUTPUT_MESSAGES
                catch (Exception e)
                {
                    Debug.LogError("Invalid embedded texture data" + e);
                }
#else
                catch
                {

                }
#endif
            }
            outputTexture2D = new Texture2D(2, 2, TextureFormat.RGBA32, true);
            return outputTexture2D.LoadImage(data);
        }

        private static Texture2D ProccessTextureData(Texture2D inputTexture2D, string name, ref bool hasAlphaChannel, TextureWrapMode textureWrapMode, TextureCompression textureCompression, bool isNormalMap, bool checkAlphaChannel = false)
        {
            if (inputTexture2D == null)
            {
                return null;
            }
            inputTexture2D.name = name;
            inputTexture2D.wrapMode = textureWrapMode;
            if (isNormalMap)
            {
                var colors = inputTexture2D.GetPixels32();
#if UNITY_5
                Texture2D outputTexture2D = new Texture2D(inputTexture2D.width, inputTexture2D.height, TextureFormat.ARGB32, true);
                for (var i = 0; i < colors.Length; i++)
                {
                    var color = colors[i];
                    color.a = color.r;
                    color.r = 0;
                    color.b = 0;
                    colors[i] = color;
                }
                outputTexture2D.SetPixels32(colors);
                outputTexture2D.Apply();
#else
                Texture2D outputTexture2D = UnityEngine.Object.Instantiate(AssetLoaderBase.NormalBaseTexture);
                outputTexture2D.filterMode = inputTexture2D.filterMode;
                outputTexture2D.wrapMode = inputTexture2D.wrapMode;
                outputTexture2D.Resize(inputTexture2D.width, inputTexture2D.height);
                outputTexture2D.SetPixels32(colors);
                outputTexture2D.Apply();
#endif
                return outputTexture2D;
            }
            if (textureCompression != TextureCompression.None)
            {
                inputTexture2D.Compress(textureCompression == TextureCompression.HighQuality);
            }
            if (checkAlphaChannel)
            {
                hasAlphaChannel = false;
                var colors = inputTexture2D.GetPixels32();
                foreach (var color in colors)
                {
                    if (color.a == 255) continue;
                    hasAlphaChannel = true;
                    break;
                }
            }
            return inputTexture2D;
        }
    }
}


using System;
using UnityEngine;
using TriLib;
using System.Text;
#if !TRILIB_USE_UNITY_TEXTURE_LOADER
using STB;
#endif
#if !UNITY_EDITOR && !NET_4_6 && !NETFX_CORE
using AOT;
#endif
/// <summary>
/// Represents a custom IO system file loading.
/// You can load data from any source that returns a byte array with TriLib.
/// This sample loads a series of data from embedded data strings.
/// </summary>
public class CustomIOLoadSample : MonoBehaviour
{
    /// <summary>
    /// Obj file data.
    /// </summary>
    private const string objData = "mtllib cube.mtl\n\nv -1.000000 -1.000000 1.000000\nv 1.000000 -1.000000 1.000000\nv -1.000000 1.000000 1.000000\nv 1.000000 1.000000 1.000000\nv -1.000000 1.000000 -1.000000\nv 1.000000 1.000000 -1.000000\nv -1.000000 -1.000000 -1.000000\nv 1.000000 -1.000000 -1.000000\n\nvt 0.000000 0.000000\nvt 1.000000 0.000000\nvt 0.000000 1.000000\nvt 1.000000 1.000000\n\nvn 0.000000 0.000000 1.000000\nvn 0.000000 1.000000 0.000000\nvn 0.000000 0.000000 -1.000000\nvn 0.000000 -1.000000 0.000000\nvn 1.000000 0.000000 0.000000\nvn -1.000000 0.000000 0.000000\n\ng cube\nusemtl cube\ns 1\nf 1/1/1 2/2/1 3/3/1\nf 3/3/1 2/2/1 4/4/1\ns 2\nf 3/1/2 4/2/2 5/3/2\nf 5/3/2 4/2/2 6/4/2\ns 3\nf 5/4/3 6/3/3 7/2/3\nf 7/2/3 6/3/3 8/1/3\ns 4\nf 7/1/4 8/2/4 1/3/4\nf 1/3/4 8/2/4 2/4/4\ns 5\nf 2/1/5 8/2/5 4/3/5\nf 4/3/5 8/2/5 6/4/5\ns 6\nf 7/1/6 1/2/6 5/3/6\nf 5/3/6 1/2/6 3/4/6\n";

    /// <summary>
    /// MTL file name.
    /// </summary>
    private const string mtlFilename = "cube.mtl";
    
    /// <summary>
    /// MTL file data.
    /// </summary>
    private const string mtlData = "newmtl cube\n  Ns 10.0000\n  Ni 1.5000\n  d 1.0000\n  Tr 0.0000\n  Tf 1.0000 1.0000 1.0000 \n  illum 2\n  Ka 0.0000 0.0000 0.0000\n  Kd 0.5880 0.5880 0.5880\n  Ks 0.0000 0.0000 0.0000\n  Ke 0.0000 0.0000 0.0000\n  map_Ka cube.png\n  map_Kd cube.png";

    /// <summary>
    /// Texture file name.
    /// </summary>
    private const string textureFilename = "cube.png";

    /// <summary>
    /// Texture file data.
    /// </summary>
    private const string textureData = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAIAAACQkWg2AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABTSURBVDhPrYrJDcAwDMO8/7hdoAegKAbtJo+W4EdH/MB5TDfka7YHJ0gwt05QOdZGIDvWRjhvFWgXCgcPMH8e6pDFQTTV4HXy0NqDk12B6+03Ii7udAgYy29ORgAAAABJRU5ErkJggg==";

    /// <summary>
    /// Load our custom model from embedded data strings.
    /// </summary>
    private void Start()
    {
        ///Creates a new AssetLoader instance to load our model.
        using (var assetLoader = new AssetLoader())
        {
            var objBytes = Encoding.UTF8.GetBytes(objData); //Transforms the OBJ file data into a char array.

            //Loads the model contained in objBytes, passing:
            //CustomDataCallback to load our mtlData contents;
            //CustomExistsCallback to tell our mtlData contents exists;
            //CustomTextureDataCallback to load our textureData;
            assetLoader.LoadFromMemory(objBytes, ".obj", null, gameObject, null, CustomDataCallback, CustomExistsCallback, CustomTextureDataCallback);
        }
    }

    /// <summary>
    /// Checks if our custom texture has been requested and returns a new <see cref="EmbeddedTextureData"></see>.
    /// </summary>
    /// <param name="path">Requested texture path.</param>
    /// <param name="basePath">Requested texture base path.</param>
    /// <returns>If our custom texture has been requested, returns a new <see cref="EmbeddedTextureData"></see>, otherwise, returns <c>null</c></returns>
    private static EmbeddedTextureData CustomTextureDataCallback(string path, string basePath)
    {
        //Checks if our custom texture has been requested.
        if (path == textureFilename)
        {
            var embeddedTextureData = new EmbeddedTextureData(); //Creates a new EmbeddedTextureData instance.
            var textureBytes = Convert.FromBase64String(textureData); //Transforms texture data from a Base64 string into a byte array.
#if !TRILIB_USE_UNITY_TEXTURE_LOADER
            embeddedTextureData.Data = STBImageLoader.LoadTextureDataFromByteArray(textureBytes, out embeddedTextureData.Width, out embeddedTextureData.Height, out embeddedTextureData.NumChannels); //Loads our custom texture data from the given byte array.
            embeddedTextureData.IsRawData = true; //Tells this texture contains raw data (True for all textures loaded with STBImageLoader).
#else
            embeddedTextureData.Data = textureBytes; //Set the texture data to the raw PNG texture contents.
#endif
            return embeddedTextureData;
        }
        //Returns null if our custom texture hasn't been requested.
        return null;
    }

    /// <summary>
    /// Checks if our custom mtl file has been requested and returns a new GC pointer handle for it.
    /// @warning Always include the <see cref="AOT.MonoPInvokeCallbackAttribute"/> attribute to this callback when using Mono or IL2CPP.
    /// </summary>
    /// <param name="resourceFilename">Requested resource filename.</param>
    /// <param name="resourceId">Requested resource ID.</param>
    /// <param name="fileSize">Output requested resource file size.</param>
    /// <returns>A new <see cref="IntPtr"/> if our custom mtl file has been requested, otherwise, returns a null <see cref="IntPtr"/>.</returns>
#if !UNITY_EDITOR && !NET_4_6 && !NETFX_CORE
	[MonoPInvokeCallback(typeof(AssimpInterop.DataCallback))]
#endif
    private static IntPtr CustomDataCallback(string resourceFilename, int resourceId, ref int fileSize)
    {
        //Checks if our custom mtl file has been requested.
        if (resourceFilename == mtlFilename)
        {
            var mtlBytes = Encoding.UTF8.GetBytes(mtlData); //Transforms our mtlData into a byte array.
            fileSize = mtlBytes.Length; //Important: sets the fileSize output to our data array length.
            var dataBuffer = AssimpInterop.LockGc(mtlBytes); //Lock the GC to don't destroy our array while being used.
            var fileLoadData = AssetLoaderBase.FilesLoadData[resourceId]; //Retrieves the fileLoadData created for this request.
            ((GCFileLoadData)fileLoadData).LockedBuffers.Add(dataBuffer); //Add the locked GC buffer to the request buffers list.
            return dataBuffer.AddrOfPinnedObject(); //Returns our locked buffer pointer.
        }
        //Returns a null IntPtr if our custom mtl file hasn't been requested.
        return IntPtr.Zero;
    }

    /// <summary>
    /// Checks if it's our custom mtl file being requested and returns <c>true</c> to indicate the requested resource exists.
    /// @warning Always include the <see cref="AOT.MonoPInvokeCallbackAttribute"/> attribute to this callback when using Mono or IL2CPP.
    /// </summary>
    /// <param name="resourceFilename">Requested resource filename.</param>
    /// <param name="resourceId">Requested resource ID.</param>
    /// <returns><c>true</c> if our custom mtl file has been requested, otherwise <c>false</c>.</returns>
#if !UNITY_EDITOR && !NET_4_6 && !NETFX_CORE
	[MonoPInvokeCallback(typeof(AssimpInterop.DataCallback))]
#endif
    private static bool CustomExistsCallback(string resourceFilename, int resourceId)
    {
        return resourceFilename == mtlFilename;
    }
}
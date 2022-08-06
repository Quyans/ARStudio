using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace TriLib
{
    /// <summary>
    ///     Represents the internal Assimp library functions and helpers.
    ///     @warning Do not modify!
    /// </summary>
    public static class AssimpInterop
    {
        #region DllImport

#if (UNITY_WINRT && !UNITY_EDITOR_WIN)
		private const string DllPath = "assimp-uwp";
#elif (!UNITY_WINRT && UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        private const string DllPath = "assimp";
#elif (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
		private const string DllPath = "libassimp";
#elif (UNITY_IOS && !UNITY_EDITOR)
		private const string DllPath = "__Internal";
#elif (UNITY_WEBGL && !UNITY_EDITOR)
		private const string DllPath = "__Internal";
#elif (UNITY_STANDALONE_LINUX)
		private const string DllPath = "assimp";
#else
		private const string DllPath = "libassimp";
#endif
        #endregion

        #region Generated

        private const int MaxStringLength = 1024;
        private const int MaxInputStringLength = 2048;
        private static readonly bool Is32Bits = IntPtr.Size == 4;
        private static readonly int IntSize = Is32Bits ? 4 : 8;

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiCreatePropertyStore")]
        private static extern IntPtr _aiCreatePropertyStore();

        public static IntPtr ai_CreatePropertyStore()
        {
            var result = _aiCreatePropertyStore();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiReleasePropertyStore")]
        private static extern void _aiReleasePropertyStore(IntPtr ptrPropertyStore);

        public static void ai_CreateReleasePropertyStore(IntPtr ptrPropertyStore)
        {
            _aiReleasePropertyStore(ptrPropertyStore);
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiSetImportPropertyInteger")]
        private static extern IntPtr _aiSetImportPropertyInteger(IntPtr ptrStore, IntPtr name, int value);

        public static IntPtr ai_SetImportPropertyInteger(IntPtr ptrStore, string name, int value)
        {
            var stringBuffer = GetStringBuffer(name);
            var result = _aiSetImportPropertyInteger(ptrStore, stringBuffer.AddrOfPinnedObject(), value);
            stringBuffer.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiSetImportPropertyFloat")]
        private static extern IntPtr _aiSetImportPropertyFloat(IntPtr ptrStore, IntPtr name, float value);

        public static IntPtr ai_SetImportPropertyFloat(IntPtr ptrStore, string name, float value)
        {
            var stringBuffer = GetStringBuffer(name);
            var result = _aiSetImportPropertyFloat(ptrStore, stringBuffer.AddrOfPinnedObject(), value);
            stringBuffer.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiSetImportPropertyString")]
        private static extern IntPtr _aiSetImportPropertyString(IntPtr ptrStore, IntPtr name, IntPtr ptrValue);

        public static IntPtr ai_SetImportPropertyString(IntPtr ptrStore, string name, string value)
        {
            var stringBuffer = GetStringBuffer(name);
            var assimpStringBuffer = GetAssimpStringBuffer(value);
            var result = _aiSetImportPropertyString(ptrStore, stringBuffer.AddrOfPinnedObject(), assimpStringBuffer);
            stringBuffer.Free();
            Marshal.FreeHGlobal(assimpStringBuffer);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiSetImportPropertyMatrix")]
        private static extern IntPtr _aiSetImportPropertyMatrix(IntPtr ptrStore, IntPtr name, IntPtr ptrValue);

        public static IntPtr ai_SetImportPropertyMatrix(IntPtr ptrStore, string name, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            var stringBuffer = GetStringBuffer(name);
            var buffer = Matrix4x4ToAssimp(translation, rotation, scale);
            var result = _aiSetImportPropertyMatrix(ptrStore, stringBuffer.AddrOfPinnedObject(), buffer.AddrOfPinnedObject());
            stringBuffer.Free();
            buffer.Free();
            return result;
        }

        public delegate IntPtr DataCallback(string pFile, int fileId, ref int fileSize);
        public delegate bool ExistsCallback(string pFile, int fileId);

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiImportFileFromMemory")]
        private static extern IntPtr _aiImportFileFromMemory(IntPtr ptrBuffer, uint uintLength, uint uintFlags, int fileId, string strHint, [MarshalAs(UnmanagedType.FunctionPtr)] DataCallback dataCallback, [MarshalAs(UnmanagedType.FunctionPtr)] ExistsCallback existsCallback);

        public static IntPtr ai_ImportFileFromMemory(Byte[] fileBytes, uint uintFlags, string strHint, DataCallback dataCallback, ExistsCallback existsCallback, int fileId)
        {
            var dataBuffer = LockGc(fileBytes);
            var result = _aiImportFileFromMemory(dataBuffer.AddrOfPinnedObject(), (uint)fileBytes.Length, uintFlags, fileId, strHint, dataCallback, existsCallback);
            dataBuffer.Free();
            return result;
        }


        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiImportFileFromMemoryWithProperties")]
        private static extern IntPtr _aiImportFileFromMemoryWithProperties(IntPtr ptrBuffer, uint uintLength, uint uintFlags, int fileId, string strHint, IntPtr ptrProps, [MarshalAs(UnmanagedType.FunctionPtr)] DataCallback dataCallback, [MarshalAs(UnmanagedType.FunctionPtr)] ExistsCallback existsCallback);

        public static IntPtr ai_ImportFileFromMemoryWithProperties(Byte[] fileBytes, uint uintFlags, string strHint, IntPtr ptrProps, DataCallback dataCallback, ExistsCallback existsCallback, int fileId)
        {
            var dataBuffer = LockGc(fileBytes);
            var result = _aiImportFileFromMemoryWithProperties(dataBuffer.AddrOfPinnedObject(), (uint)fileBytes.Length, uintFlags, fileId, strHint, ptrProps, dataCallback, existsCallback);
            dataBuffer.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiImportFile")]
        private static extern IntPtr _aiImportFile(string filename, uint flags);

        public static IntPtr ai_ImportFile(string filename, uint flags)
        {
            var result = _aiImportFile(filename, flags);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiImportFileExWithProperties")]
        private static extern IntPtr _aiImportFileEx(string filename, uint flags, IntPtr ptrFS, IntPtr ptrProps);

        public static IntPtr ai_ImportFileEx(string filename, uint flags, IntPtr ptrFS, IntPtr ptrProp)
        {
            var result = _aiImportFileEx(filename, flags, ptrFS, ptrProp);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiReleaseImport")]
        private static extern void _aiReleaseImport(IntPtr scene);

        public static void ai_ReleaseImport(IntPtr scene)
        {
            //TODO: code crashing on WINRT, commented for now
#if !UNITY_WINRT
            _aiReleaseImport(scene);
#endif
        }

        //TODO: New interface
        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiGetExtensionList")]
        private static extern void _aiGetExtensionList(IntPtr ptrExtensionList);

        public static void ai_GetExtensionList(out string strExtensionList)
        {
            byte[] byteArray;
            var stringBuffer = GetNewStringBuffer(out byteArray);
            _aiGetExtensionList(stringBuffer.AddrOfPinnedObject());
            stringBuffer.Free();
            var length = Is32Bits ? BitConverter.ToInt32(byteArray, 0) : BitConverter.ToInt64(byteArray, 0);
            strExtensionList = Encoding.UTF8.GetString(byteArray, IntSize, (int)length);
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiGetErrorString")]
        private static extern IntPtr _aiGetErrorString();

        public static string ai_GetErrorString()
        {
            var result = _aiGetErrorString();
            return Marshal.PtrToStringAnsi(result);
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiIsExtensionSupported")]
        private static extern bool _aiIsExtensionSupported(IntPtr strExtension);

        public static bool ai_IsExtensionSupported(string strExtension)
        {
            var stringBuffer = GetStringBuffer(strExtension);
            var result = _aiIsExtensionSupported(stringBuffer.AddrOfPinnedObject());
            stringBuffer.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_HasMaterials")]
        private static extern bool _aiScene_HasMaterials(IntPtr ptrScene);

        public static bool aiScene_HasMaterials(IntPtr ptrScene)
        {
            var result = _aiScene_HasMaterials(ptrScene);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_GetNumMaterials")]
        private static extern uint _aiScene_GetNumMaterials(IntPtr ptrScene);

        public static uint aiScene_GetNumMaterials(IntPtr ptrScene)
        {
            var result = _aiScene_GetNumMaterials(ptrScene);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_GetNumMeshes")]
        private static extern uint _aiScene_GetNumMeshes(IntPtr ptrScene);

        public static uint aiScene_GetNumMeshes(IntPtr ptrScene)
        {
            var result = _aiScene_GetNumMeshes(ptrScene);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_GetNumAnimations")]
        private static extern uint _aiScene_GetNumAnimations(IntPtr ptrScene);

        public static uint aiScene_GetNumAnimations(IntPtr ptrScene)
        {
            var result = _aiScene_GetNumAnimations(ptrScene);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_GetNumCameras")]
        private static extern uint _aiScene_GetNumCameras(IntPtr ptrScene);

        public static uint aiScene_GetNumCameras(IntPtr ptrScene)
        {
            var result = _aiScene_GetNumCameras(ptrScene);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_GetNumLights")]
        private static extern uint _aiScene_GetNumLights(IntPtr ptrScene);

        public static uint aiScene_GetNumLights(IntPtr ptrScene)
        {
            var result = _aiScene_GetNumLights(ptrScene);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_HasMeshes")]
        private static extern bool _aiScene_HasMeshes(IntPtr ptrScene);

        public static bool aiScene_HasMeshes(IntPtr ptrScene)
        {
            var result = _aiScene_HasMeshes(ptrScene);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_HasAnimation")]
        private static extern bool _aiScene_HasAnimation(IntPtr ptrScene);

        public static bool aiScene_HasAnimation(IntPtr ptrScene)
        {
            var result = _aiScene_HasAnimation(ptrScene);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_HasCameras")]
        private static extern bool _aiScene_HasCameras(IntPtr ptrScene);

        public static bool aiScene_HasCameras(IntPtr ptrScene)
        {
            var result = _aiScene_HasCameras(ptrScene);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_HasLights")]
        private static extern bool _aiScene_HasLights(IntPtr ptrScene);

        public static bool aiScene_HasLights(IntPtr ptrScene)
        {
            var result = _aiScene_HasLights(ptrScene);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_GetRootNode")]
        private static extern IntPtr _aiScene_GetRootNode(IntPtr ptrScene);

        public static IntPtr aiScene_GetRootNode(IntPtr ptrScene)
        {
            var result = _aiScene_GetRootNode(ptrScene);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_GetMaterial")]
        private static extern IntPtr _aiScene_GetMaterial(IntPtr ptrScene, uint uintIndex);

        public static IntPtr aiScene_GetMaterial(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetMaterial(ptrScene, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_GetMesh")]
        private static extern IntPtr _aiScene_GetMesh(IntPtr ptrScene, uint uintIndex);

        public static IntPtr aiScene_GetMesh(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetMesh(ptrScene, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_GetAnimation")]
        private static extern IntPtr _aiScene_GetAnimation(IntPtr ptrScene, uint uintIndex);

        public static IntPtr aiScene_GetAnimation(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetAnimation(ptrScene, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_GetCamera")]
        private static extern IntPtr _aiScene_GetCamera(IntPtr ptrScene, uint uintIndex);

        public static IntPtr aiScene_GetCamera(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetCamera(ptrScene, uintIndex);
            return result;
        }


        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_GetLight")]
        private static extern IntPtr _aiScene_GetLight(IntPtr ptrScene, uint uintIndex);

        public static IntPtr aiScene_GetLight(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetLight(ptrScene, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNode_GetName")]
        private static extern IntPtr _aiNode_GetName(IntPtr ptrNode);

        public static string aiNode_GetName(IntPtr ptrNode)
        {
            var result = _aiNode_GetName(ptrNode);
            var resultConverted = ReadStringFromPointer(result);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNode_GetNumChildren")]
        private static extern uint _aiNode_GetNumChildren(IntPtr ptrNode);

        public static uint aiNode_GetNumChildren(IntPtr ptrNode)
        {
            var result = _aiNode_GetNumChildren(ptrNode);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNode_GetNumMeshes")]
        private static extern uint _aiNode_GetNumMeshes(IntPtr ptrNode);

        public static uint aiNode_GetNumMeshes(IntPtr ptrNode)
        {
            var result = _aiNode_GetNumMeshes(ptrNode);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNode_GetChildren")]
        private static extern IntPtr _aiNode_GetChildren(IntPtr ptrNode, uint uintIndex);

        public static IntPtr aiNode_GetChildren(IntPtr ptrNode, uint uintIndex)
        {
            var result = _aiNode_GetChildren(ptrNode, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNode_GetMeshIndex")]
        private static extern uint _aiNode_GetMeshIndex(IntPtr ptrNode, uint uintIndex);

        public static uint aiNode_GetMeshIndex(IntPtr ptrNode, uint uintIndex)
        {
            var result = _aiNode_GetMeshIndex(ptrNode, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNode_GetParent")]
        private static extern IntPtr _aiNode_GetParent(IntPtr ptrNode);

        public static IntPtr aiNode_GetParent(IntPtr ptrNode)
        {
            var result = _aiNode_GetParent(ptrNode);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNode_GetTransformation")]
        private static extern IntPtr _aiNode_GetTransformation(IntPtr ptrNode);

        public static Matrix4x4 aiNode_GetTransformation(IntPtr ptrNode)
        {
            var result = _aiNode_GetTransformation(ptrNode);
            var resultConverted = GetNewMatrix4x4(result);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_IsEmbeddedTextureCompressed")]
        private static extern bool _aiMaterial_IsEmbeddedTextureCompressed(IntPtr ptrScene, IntPtr ptrTexture);

        public static bool aiMaterial_IsEmbeddedTextureCompressed(IntPtr ptrScene, IntPtr ptrTexture)
        {
            var result = _aiMaterial_IsEmbeddedTextureCompressed(ptrScene, ptrTexture);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetEmbeddedTextureDataSize")]
        private static extern uint _aiMaterial_GetEmbeddedTextureDataSize(IntPtr ptrScene, IntPtr ptrTexture,
                                                                          bool boolCompressed);

        public static uint aiMaterial_GetEmbeddedTextureDataSize(IntPtr ptrScene, IntPtr ptrTexture, bool boolCompressed)
        {
            var result = _aiMaterial_GetEmbeddedTextureDataSize(ptrScene, ptrTexture, boolCompressed);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetEmbeddedTextureData")]
        private static extern void _aiMaterial_GetEmbeddedTextureData(IntPtr ptrScene, IntPtr ptrData, IntPtr ptrTexture,
                                                                      uint uintSize);

        public static byte[] aiMaterial_GetEmbeddedTextureData(IntPtr ptrScene, IntPtr ptrTexture, uint uintSize)
        {
            var data = new byte[uintSize];
            var dataBuffer = LockGc(data);
            _aiMaterial_GetEmbeddedTextureData(ptrScene, dataBuffer.AddrOfPinnedObject(), ptrTexture, uintSize);
            dataBuffer.Free();
            return data;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetEmbeddedTextureWidth")]
        private static extern int _aiMaterial_GetEmbeddedTextureWidth(IntPtr ptrTexture);

        public static int aiMaterial_GetEmbeddedTextureWidth(IntPtr ptrTexture)
        {
            var result = _aiMaterial_GetEmbeddedTextureWidth(ptrTexture);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetEmbeddedTextureHeight")]
        private static extern int _aiMaterial_GetEmbeddedTextureHeight(IntPtr ptrTexture);

        public static int aiMaterial_GetEmbeddedTextureHeight(IntPtr ptrTexture)
        {
            var result = _aiMaterial_GetEmbeddedTextureHeight(ptrTexture);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiScene_GetEmbeddedTexture")]
        private static extern IntPtr _aiScene_GetEmbeddedTexture(IntPtr ptrScene, string strFilename);

        public static IntPtr aiScene_GetEmbeddedTexture(IntPtr ptrScene, string strFilename)
        {
            return _aiScene_GetEmbeddedTexture(ptrScene, strFilename);
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetTextureCount")]
        private static extern uint _aiMaterial_GetTextureCount(IntPtr ptrMat, uint uintType);

        public static uint aiMaterial_GetTextureCount(IntPtr ptrMat, uint uintType)
        {
            var result = _aiMaterial_GetTextureCount(ptrMat, uintType);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasTextureDiffuse")]
        private static extern bool _aiMaterial_HasTextureDiffuse(IntPtr ptrMat, uint uintType);

        public static bool aiMaterial_HasTextureDiffuse(IntPtr ptrMat, uint uintType)
        {
            var result = _aiMaterial_HasTextureDiffuse(ptrMat, uintType);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetTextureDiffuse")]
        private static extern bool _aiMaterial_GetTextureDiffuse(IntPtr ptrMat, uint uintType,
                                                                 IntPtr strPath, IntPtr uintMapping, IntPtr uintUvIndex, IntPtr floatBlend, IntPtr uintOp,
                                                                 IntPtr uintMapMode);

        public static bool aiMaterial_GetTextureDiffuse(IntPtr ptrMat, uint uintType,
                                                        out string strPath, out uint uintMapping, out uint uintUvIndex,
                                                        out float floatBlend, out uint uintOp, out uint uintMapMode)
        {
            byte[] strPathByteArray;
            var strPathBufferHandle = GetNewStringBuffer(out strPathByteArray);
            uintMapping = 0;
            var uintMappingBufferHandle = LockGc(uintMapping);
            uintUvIndex = 0;
            var uintUvIndexBufferHandle = LockGc(uintUvIndex);
            floatBlend = 0;
            var floatBlendBufferHandle = LockGc(floatBlend);
            uintOp = 0;
            var uintOpBufferHandle = LockGc(uintOp);
            uintMapMode = 0;
            var uintMapModeBufferHandle = LockGc(uintMapMode);
            var result = _aiMaterial_GetTextureDiffuse(ptrMat, uintType, strPathBufferHandle.AddrOfPinnedObject(),
                             uintMappingBufferHandle.AddrOfPinnedObject(), uintUvIndexBufferHandle.AddrOfPinnedObject(),
                             floatBlendBufferHandle.AddrOfPinnedObject(), uintOpBufferHandle.AddrOfPinnedObject(),
                             uintMapModeBufferHandle.AddrOfPinnedObject());
            strPath = ByteArrayToString(strPathByteArray);
            strPathBufferHandle.Free();
            uintMappingBufferHandle.Free();
            uintUvIndexBufferHandle.Free();
            floatBlendBufferHandle.Free();
            uintOpBufferHandle.Free();
            uintMapModeBufferHandle.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetNumTextureDiffuse")]
        private static extern uint _aiMaterial_GetNumTextureDiffuse(IntPtr ptrMat);

        public static uint aiMaterial_GetNumTextureDiffuse(IntPtr ptrMat)
        {
            var result = _aiMaterial_GetNumTextureDiffuse(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasTextureEmissive")]
        private static extern bool _aiMaterial_HasTextureEmissive(IntPtr ptrMat, uint uintIndex);

        public static bool aiMaterial_HasTextureEmissive(IntPtr ptrMat, uint uintIndex)
        {
            var result = _aiMaterial_HasTextureEmissive(ptrMat, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetTextureEmissive")]
        private static extern bool _aiMaterial_GetTextureEmissive(IntPtr ptrMat, uint uintIndex,
                                                                  IntPtr strPath, IntPtr uintMapping, IntPtr uintUvIndex, IntPtr floatBlend, IntPtr uintOp,
                                                                  IntPtr uintMapMode);

        public static bool aiMaterial_GetTextureEmissive(IntPtr ptrMat, uint uintIndex,
                                                         out string strPath, out uint uintMapping, out uint uintUvIndex,
                                                         out float floatBlend, out uint uintOp, out uint uintMapMode)
        {
            byte[] strPathByteArray;
            var strPathBufferHandle = GetNewStringBuffer(out strPathByteArray);
            uintMapping = 0;
            var uintMappingBufferHandle = LockGc(uintMapping);
            uintUvIndex = 0;
            var uintUvIndexBufferHandle = LockGc(uintUvIndex);
            floatBlend = 0;
            var floatBlendBufferHandle = LockGc(floatBlend);
            uintOp = 0;
            var uintOpBufferHandle = LockGc(uintOp);
            uintMapMode = 0;
            var uintMapModeBufferHandle = LockGc(uintMapMode);
            var result = _aiMaterial_GetTextureEmissive(ptrMat, uintIndex, strPathBufferHandle.AddrOfPinnedObject(),
                             uintMappingBufferHandle.AddrOfPinnedObject(), uintUvIndexBufferHandle.AddrOfPinnedObject(),
                             floatBlendBufferHandle.AddrOfPinnedObject(), uintOpBufferHandle.AddrOfPinnedObject(),
                             uintMapModeBufferHandle.AddrOfPinnedObject());
            strPath = ByteArrayToString(strPathByteArray);
            strPathBufferHandle.Free();
            uintMappingBufferHandle.Free();
            uintUvIndexBufferHandle.Free();
            floatBlendBufferHandle.Free();
            uintOpBufferHandle.Free();
            uintMapModeBufferHandle.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetNumTextureEmissive")]
        private static extern uint _aiMaterial_GetNumTextureEmissive(IntPtr ptrMat);

        public static uint aiMaterial_GetNumTextureEmissive(IntPtr ptrMat)
        {
            var result = _aiMaterial_GetNumTextureEmissive(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasTextureSpecular")]
        private static extern bool _aiMaterial_HasTextureSpecular(IntPtr ptrMat, uint uintIndex);

        public static bool aiMaterial_HasTextureSpecular(IntPtr ptrMat, uint uintIndex)
        {
            var result = _aiMaterial_HasTextureSpecular(ptrMat, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetTextureSpecular")]
        private static extern bool _aiMaterial_GetTextureSpecular(IntPtr ptrMat, uint uintIndex,
                                                                  IntPtr strPath, IntPtr uintMapping, IntPtr uintUvIndex, IntPtr floatBlend, IntPtr uintOp,
                                                                  IntPtr uintMapMode);

        public static bool aiMaterial_GetTextureSpecular(IntPtr ptrMat, uint uintIndex,
                                                         out string strPath, out uint uintMapping, out uint uintUvIndex,
                                                         out float floatBlend, out uint uintOp, out uint uintMapMode)
        {
            byte[] strPathByteArray;
            var strPathBufferHandle = GetNewStringBuffer(out strPathByteArray);
            uintMapping = 0;
            var uintMappingBufferHandle = LockGc(uintMapping);
            uintUvIndex = 0;
            var uintUvIndexBufferHandle = LockGc(uintUvIndex);
            floatBlend = 0;
            var floatBlendBufferHandle = LockGc(floatBlend);
            uintOp = 0;
            var uintOpBufferHandle = LockGc(uintOp);
            uintMapMode = 0;
            var uintMapModeBufferHandle = LockGc(uintMapMode);
            var result = _aiMaterial_GetTextureSpecular(ptrMat, uintIndex, strPathBufferHandle.AddrOfPinnedObject(),
                             uintMappingBufferHandle.AddrOfPinnedObject(), uintUvIndexBufferHandle.AddrOfPinnedObject(),
                             floatBlendBufferHandle.AddrOfPinnedObject(), uintOpBufferHandle.AddrOfPinnedObject(),
                             uintMapModeBufferHandle.AddrOfPinnedObject());
            strPath = ByteArrayToString(strPathByteArray);
            strPathBufferHandle.Free();
            uintMappingBufferHandle.Free();
            uintUvIndexBufferHandle.Free();
            floatBlendBufferHandle.Free();
            uintOpBufferHandle.Free();
            uintMapModeBufferHandle.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetNumTextureSpecular")]
        private static extern uint _aiMaterial_GetNumTextureSpecular(IntPtr ptrMat);

        public static uint aiMaterial_GetNumTextureSpecular(IntPtr ptrMat)
        {
            var result = _aiMaterial_GetNumTextureSpecular(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasTextureNormals")]
        private static extern bool _aiMaterial_HasTextureNormals(IntPtr ptrMat, uint uintIndex);

        public static bool aiMaterial_HasTextureNormals(IntPtr ptrMat, uint uintIndex)
        {
            var result = _aiMaterial_HasTextureNormals(ptrMat, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetTextureNormals")]
        private static extern bool _aiMaterial_GetTextureNormals(IntPtr ptrMat, uint uintIndex,
                                                                 IntPtr strPath, IntPtr uintMapping, IntPtr uintUvIndex, IntPtr floatBlend, IntPtr uintOp,
                                                                 IntPtr uintMapMode);

        public static bool aiMaterial_GetTextureNormals(IntPtr ptrMat, uint uintIndex,
                                                        out string strPath, out uint uintMapping, out uint uintUvIndex,
                                                        out float floatBlend, out uint uintOp, out uint uintMapMode)
        {
            byte[] strPathByteArray;
            var strPathBufferHandle = GetNewStringBuffer(out strPathByteArray);
            uintMapping = 0;
            var uintMappingBufferHandle = LockGc(uintMapping);
            uintUvIndex = 0;
            var uintUvIndexBufferHandle = LockGc(uintUvIndex);
            floatBlend = 0;
            var floatBlendBufferHandle = LockGc(floatBlend);
            uintOp = 0;
            var uintOpBufferHandle = LockGc(uintOp);
            uintMapMode = 0;
            var uintMapModeBufferHandle = LockGc(uintMapMode);
            var result = _aiMaterial_GetTextureNormals(ptrMat, uintIndex, strPathBufferHandle.AddrOfPinnedObject(),
                             uintMappingBufferHandle.AddrOfPinnedObject(), uintUvIndexBufferHandle.AddrOfPinnedObject(),
                             floatBlendBufferHandle.AddrOfPinnedObject(), uintOpBufferHandle.AddrOfPinnedObject(),
                             uintMapModeBufferHandle.AddrOfPinnedObject());
            strPath = ByteArrayToString(strPathByteArray);
            strPathBufferHandle.Free();
            uintMappingBufferHandle.Free();
            uintUvIndexBufferHandle.Free();
            floatBlendBufferHandle.Free();
            uintOpBufferHandle.Free();
            uintMapModeBufferHandle.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetNumTextureNormals")]
        private static extern uint _aiMaterial_GetNumTextureNormals(IntPtr ptrMat);

        public static uint aiMaterial_GetNumTextureNormals(IntPtr ptrMat)
        {
            var result = _aiMaterial_GetNumTextureNormals(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasTextureHeight")]
        private static extern bool _aiMaterial_HasTextureHeight(IntPtr ptrMat, uint uintIndex);

        public static bool aiMaterial_HasTextureHeight(IntPtr ptrMat, uint uintIndex)
        {
            var result = _aiMaterial_HasTextureHeight(ptrMat, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetTextureHeight")]
        private static extern bool _aiMaterial_GetTextureHeight(IntPtr ptrMat, uint uintIndex,
                                                                IntPtr strPath, IntPtr uintMapping, IntPtr uintUvIndex, IntPtr floatBlend, IntPtr uintOp,
                                                                IntPtr uintMapMode);

        public static bool aiMaterial_GetTextureHeight(IntPtr ptrMat, uint uintIndex,
                                                       out string strPath, out uint uintMapping, out uint uintUvIndex,
                                                       out float floatBlend, out uint uintOp, out uint uintMapMode)
        {
            byte[] strPathByteArray;
            var strPathBufferHandle = GetNewStringBuffer(out strPathByteArray);
            uintMapping = 0;
            var uintMappingBufferHandle = LockGc(uintMapping);
            uintUvIndex = 0;
            var uintUvIndexBufferHandle = LockGc(uintUvIndex);
            floatBlend = 0;
            var floatBlendBufferHandle = LockGc(floatBlend);
            uintOp = 0;
            var uintOpBufferHandle = LockGc(uintOp);
            uintMapMode = 0;
            var uintMapModeBufferHandle = LockGc(uintMapMode);
            var result = _aiMaterial_GetTextureHeight(ptrMat, uintIndex, strPathBufferHandle.AddrOfPinnedObject(),
                             uintMappingBufferHandle.AddrOfPinnedObject(), uintUvIndexBufferHandle.AddrOfPinnedObject(),
                             floatBlendBufferHandle.AddrOfPinnedObject(), uintOpBufferHandle.AddrOfPinnedObject(),
                             uintMapModeBufferHandle.AddrOfPinnedObject());
            strPath = ByteArrayToString(strPathByteArray);
            strPathBufferHandle.Free();
            uintMappingBufferHandle.Free();
            uintUvIndexBufferHandle.Free();
            floatBlendBufferHandle.Free();
            uintOpBufferHandle.Free();
            uintMapModeBufferHandle.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetNumTextureHeight")]
        private static extern uint _aiMaterial_GetNumTextureHeight(IntPtr ptrMat);

        public static uint aiMaterial_GetNumTextureHeight(IntPtr ptrMat)
        {
            var result = _aiMaterial_GetNumTextureHeight(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasAmbient")]
        private static extern bool _aiMaterial_HasAmbient(IntPtr ptrMat);

        public static bool aiMaterial_HasAmbient(IntPtr ptrMat)
        {
            var result = _aiMaterial_HasAmbient(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetAmbient")]
        private static extern bool _aiMaterial_GetAmbient(IntPtr ptrMat, IntPtr colorOut);

        public static bool aiMaterial_GetAmbient(IntPtr ptrMat, out Color colorOut)
        {
            var colorOutBufferHandle = AllocHGlobal<Color>();
            var result = _aiMaterial_GetAmbient(ptrMat, colorOutBufferHandle);
            colorOut = ReadStruct<Color>(colorOutBufferHandle);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasDiffuse")]
        private static extern bool _aiMaterial_HasDiffuse(IntPtr ptrMat);

        public static bool aiMaterial_HasDiffuse(IntPtr ptrMat)
        {
            var result = _aiMaterial_HasDiffuse(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetDiffuse")]
        private static extern bool _aiMaterial_GetDiffuse(IntPtr ptrMat, IntPtr colorOut);

        public static bool aiMaterial_GetDiffuse(IntPtr ptrMat, out Color colorOut)
        {
            var colorOutBufferHandle = AllocHGlobal<Color>();
            var result = _aiMaterial_GetDiffuse(ptrMat, colorOutBufferHandle);
            colorOut = ReadStruct<Color>(colorOutBufferHandle);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasSpecular")]
        private static extern bool _aiMaterial_HasSpecular(IntPtr ptrMat);

        public static bool aiMaterial_HasSpecular(IntPtr ptrMat)
        {
            var result = _aiMaterial_HasSpecular(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetSpecular")]
        private static extern bool _aiMaterial_GetSpecular(IntPtr ptrMat, IntPtr colorOut);

        public static bool aiMaterial_GetSpecular(IntPtr ptrMat, out Color colorOut)
        {
            var colorOutBufferHandle = AllocHGlobal<Color>();
            var result = _aiMaterial_GetSpecular(ptrMat, colorOutBufferHandle);
            colorOut = ReadStruct<Color>(colorOutBufferHandle);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasEmissive")]
        private static extern bool _aiMaterial_HasEmissive(IntPtr ptrMat);

        public static bool aiMaterial_HasEmissive(IntPtr ptrMat)
        {
            var result = _aiMaterial_HasEmissive(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetEmissive")]
        private static extern bool _aiMaterial_GetEmissive(IntPtr ptrMat, IntPtr colorOut);

        public static bool aiMaterial_GetEmissive(IntPtr ptrMat, out Color colorOut)
        {
            var colorOutBufferHandle = AllocHGlobal<Color>();
            var result = _aiMaterial_GetEmissive(ptrMat, colorOutBufferHandle);
            colorOut = ReadStruct<Color>(colorOutBufferHandle);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasName")]
        private static extern bool _aiMaterial_HasName(IntPtr ptrMat);

        public static bool aiMaterial_HasName(IntPtr ptrMat)
        {
            var result = _aiMaterial_HasName(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetName")]
        private static extern bool _aiMaterial_GetName(IntPtr ptrMat, IntPtr strName);

        public static bool aiMaterial_GetName(IntPtr ptrMat, out string strName)
        {
            byte[] strNameByteArray;
            var strNameBufferHandle = GetNewStringBuffer(out strNameByteArray);
            var result = _aiMaterial_GetName(ptrMat, strNameBufferHandle.AddrOfPinnedObject());
            strName = ByteArrayToString(strNameByteArray);
            strNameBufferHandle.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasBumpScaling")]
        private static extern bool _aiMaterial_HasBumpScaling(IntPtr ptrMat);

        public static bool aiMaterial_HasBumpScaling(IntPtr ptrMat)
        {
            var result = _aiMaterial_HasBumpScaling(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetBumpScaling")]
        private static extern bool _aiMaterial_GetBumpScaling(IntPtr ptrMat, IntPtr floatOut);

        public static bool aiMaterial_GetBumpScaling(IntPtr ptrMat, out float floatOut)
        {
            floatOut = 1f;
            var floatOutBufferHandle = LockGc(floatOut);
            var result = _aiMaterial_GetBumpScaling(ptrMat, floatOutBufferHandle.AddrOfPinnedObject());
            floatOutBufferHandle.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasShininess")]
        private static extern bool _aiMaterial_HasShininess(IntPtr ptrMat);

        public static bool aiMaterial_HasShininess(IntPtr ptrMat)
        {
            var result = _aiMaterial_HasShininess(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetShininess")]
        private static extern bool _aiMaterial_GetShininess(IntPtr ptrMat, IntPtr floatOut);

        public static bool aiMaterial_GetShininess(IntPtr ptrMat, out float floatOut)
        {
            floatOut = 0f;
            var floatOutBufferHandle = LockGc(floatOut);
            var result = _aiMaterial_GetShininess(ptrMat, floatOutBufferHandle.AddrOfPinnedObject());
            floatOutBufferHandle.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasShininessStrength")]
        private static extern bool _aiMaterial_HasShininessStrength(IntPtr ptrMat);

        public static bool aiMaterial_HasShininessStrength(IntPtr ptrMat)
        {
            var result = _aiMaterial_HasShininessStrength(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetShininessStrength")]
        private static extern bool _aiMaterial_GetShininessStrength(IntPtr ptrMat, IntPtr floatOut);

        public static bool aiMaterial_GetShininessStrength(IntPtr ptrMat, out float floatOut)
        {
            floatOut = 0f;
            var floatOutBufferHandle = LockGc(floatOut);
            var result = _aiMaterial_GetShininessStrength(ptrMat, floatOutBufferHandle.AddrOfPinnedObject());
            floatOutBufferHandle.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_HasOpacity")]
        private static extern bool _aiMaterial_HasOpacity(IntPtr ptrMat);

        public static bool aiMaterial_HasOpacity(IntPtr ptrMat)
        {
            var result = _aiMaterial_HasOpacity(ptrMat);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMaterial_GetOpacity")]
        private static extern bool _aiMaterial_GetOpacity(IntPtr ptrMat, IntPtr floatOut);

        public static bool aiMaterial_GetOpacity(IntPtr ptrMat, out float floatOut)
        {
            floatOut = 1f;
            var buffer = LockGc(floatOut);
            var result = _aiMaterial_GetOpacity(ptrMat, buffer.AddrOfPinnedObject());
            var floatBuffer = new float[1];
            Marshal.Copy(buffer.AddrOfPinnedObject(), floatBuffer, 0, 1);
            floatOut = floatBuffer[0];
            buffer.Free();
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_VertexCount")]
        private static extern uint _aiMesh_VertexCount(IntPtr ptrMesh);

        public static uint aiMesh_VertexCount(IntPtr ptrMesh)
        {
            var result = _aiMesh_VertexCount(ptrMesh);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_HasNormals")]
        private static extern bool _aiMesh_HasNormals(IntPtr ptrMesh);

        public static bool aiMesh_HasNormals(IntPtr ptrMesh)
        {
            var result = _aiMesh_HasNormals(ptrMesh);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_HasTangentsAndBitangents")]
        private static extern bool _aiMesh_HasTangentsAndBitangents(IntPtr ptrMesh);

        public static bool aiMesh_HasTangentsAndBitangents(IntPtr ptrMesh)
        {
            var result = _aiMesh_HasTangentsAndBitangents(ptrMesh);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_HasTextureCoords")]
        private static extern bool _aiMesh_HasTextureCoords(IntPtr ptrMesh, uint uintIndex);

        public static bool aiMesh_HasTextureCoords(IntPtr ptrMesh, uint uintIndex)
        {
            var result = _aiMesh_HasTextureCoords(ptrMesh, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_HasVertexColors")]
        private static extern bool _aiMesh_HasVertexColors(IntPtr ptrMesh, uint uintIndex);

        public static bool aiMesh_HasVertexColors(IntPtr ptrMesh, uint uintIndex)
        {
            var result = _aiMesh_HasVertexColors(ptrMesh, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_GetVertex")]
        private static extern IntPtr _aiMesh_GetVertex(IntPtr ptrMesh, uint uintIndex);

        public static Vector3 aiMesh_GetVertex(IntPtr ptrMesh, uint uintIndex)
        {
            var result = _aiMesh_GetVertex(ptrMesh, uintIndex);
            var resultConverted = ReadStruct<Vector3>(result, false);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_GetNormal")]
        private static extern IntPtr _aiMesh_GetNormal(IntPtr ptrMesh, uint uintIndex);

        public static Vector3 aiMesh_GetNormal(IntPtr ptrMesh, uint uintIndex)
        {
            var result = _aiMesh_GetNormal(ptrMesh, uintIndex);
            var resultConverted = ReadStruct<Vector3>(result, false);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_GetTangent")]
        private static extern IntPtr _aiMesh_GetTangent(IntPtr ptrMesh, uint uintIndex);

        public static Vector3 aiMesh_GetTangent(IntPtr ptrMesh, uint uintIndex)
        {
            var result = _aiMesh_GetTangent(ptrMesh, uintIndex);
            var resultConverted = ReadStruct<Vector3>(result, false);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_GetBitangent")]
        private static extern IntPtr _aiMesh_GetBitangent(IntPtr ptrMesh, uint uintIndex);

        public static Vector3 aiMesh_GetBitangent(IntPtr ptrMesh, uint uintIndex)
        {
            var result = _aiMesh_GetBitangent(ptrMesh, uintIndex);
            var resultConverted = ReadStruct<Vector3>(result, false);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_GetTextureCoord")]
        private static extern IntPtr _aiMesh_GetTextureCoord(IntPtr ptrMesh, uint uintChannel,
                                                             uint uintIndex);

        public static Vector2 aiMesh_GetTextureCoord(IntPtr ptrMesh, uint uintChannel,
                                                     uint uintIndex)
        {
            var result = _aiMesh_GetTextureCoord(ptrMesh, uintChannel, uintIndex);
            var resultConverted = ReadStruct<Vector2>(result, false);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_GetVertexColor")]
        private static extern IntPtr _aiMesh_GetVertexColor(IntPtr ptrMesh, uint uintChannel,
                                                            uint uintIndex);

        public static Color aiMesh_GetVertexColor(IntPtr ptrMesh, uint uintChannel,
                                                  uint uintIndex)
        {
            var result = _aiMesh_GetVertexColor(ptrMesh, uintChannel, uintIndex);
            var resultConverted = ReadStruct<Color>(result, false);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_GetMatrialIndex")]
        private static extern uint _aiMesh_GetMatrialIndex(IntPtr ptrMesh);

        public static uint aiMesh_GetMatrialIndex(IntPtr ptrMesh)
        {
            var result = _aiMesh_GetMatrialIndex(ptrMesh);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_GetName")]
        private static extern IntPtr _aiMesh_GetName(IntPtr ptrMesh);

        public static string aiMesh_GetName(IntPtr ptrMesh)
        {
            var result = _aiMesh_GetName(ptrMesh);
            var resultConverted = ReadStringFromPointer(result);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_HasFaces")]
        private static extern bool _aiMesh_HasFaces(IntPtr ptrMesh);

        public static bool aiMesh_HasFaces(IntPtr ptrMesh)
        {
            var result = _aiMesh_HasFaces(ptrMesh);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_GetNumFaces")]
        private static extern uint _aiMesh_GetNumFaces(IntPtr ptrMesh);

        public static uint aiMesh_GetNumFaces(IntPtr ptrMesh)
        {
            var result = _aiMesh_GetNumFaces(ptrMesh);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_GetFace")]
        private static extern IntPtr _aiMesh_GetFace(IntPtr ptrMesh, uint uintIndex);

        public static IntPtr aiMesh_GetFace(IntPtr ptrMesh, uint uintIndex)
        {
            var result = _aiMesh_GetFace(ptrMesh, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_HasBones")]
        private static extern bool _aiMesh_HasBones(IntPtr ptrMesh);

        public static bool aiMesh_HasBones(IntPtr ptrMesh)
        {
            var result = _aiMesh_HasBones(ptrMesh);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_GetNumBones")]
        private static extern uint _aiMesh_GetNumBones(IntPtr ptrMesh);

        public static uint aiMesh_GetNumBones(IntPtr ptrMesh)
        {
            var result = _aiMesh_GetNumBones(ptrMesh);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiMesh_GetBone")]
        private static extern IntPtr _aiMesh_GetBone(IntPtr ptrMesh, uint uintIndex);

        public static IntPtr aiMesh_GetBone(IntPtr ptrMesh, uint uintIndex)
        {
            var result = _aiMesh_GetBone(ptrMesh, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiFace_GetNumIndices")]
        private static extern uint _aiFace_GetNumIndices(IntPtr ptrFace);

        public static uint aiFace_GetNumIndices(IntPtr ptrFace)
        {
            var result = _aiFace_GetNumIndices(ptrFace);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiFace_GetIndex")]
        private static extern uint _aiFace_GetIndex(IntPtr ptrFace, uint uintIndex);

        public static uint aiFace_GetIndex(IntPtr ptrFace, uint uintIndex)
        {
            var result = _aiFace_GetIndex(ptrFace, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiBone_GetName")]
        private static extern IntPtr _aiBone_GetName(IntPtr ptrBone);

        public static string aiBone_GetName(IntPtr ptrBone)
        {
            var result = _aiBone_GetName(ptrBone);
            var resultConverted = ReadStringFromPointer(result);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiBone_GetNumWeights")]
        private static extern uint _aiBone_GetNumWeights(IntPtr ptrBone);

        public static uint aiBone_GetNumWeights(IntPtr ptrBone)
        {
            var result = _aiBone_GetNumWeights(ptrBone);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiBone_GetWeights")]
        private static extern IntPtr _aiBone_GetWeights(IntPtr ptrBone, uint uintIndex);

        public static IntPtr aiBone_GetWeights(IntPtr ptrBone, uint uintIndex)
        {
            var result = _aiBone_GetWeights(ptrBone, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiBone_GetOffsetMatrix")]
        private static extern IntPtr _aiBone_GetOffsetMatrix(IntPtr ptrBone);

        public static Matrix4x4 aiBone_GetOffsetMatrix(IntPtr ptrBone)
        {
            var result = _aiBone_GetOffsetMatrix(ptrBone);
            var resultConverted = GetNewMatrix4x4(result);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiVertexWeight_GetWeight")]
        private static extern float _aiVertexWeight_GetWeight(IntPtr ptrVweight);

        public static float aiVertexWeight_GetWeight(IntPtr ptrVweight)
        {
            var result = _aiVertexWeight_GetWeight(ptrVweight);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiVertexWeight_GetVertexId")]
        private static extern uint _aiVertexWeight_GetVertexId(IntPtr ptrVweight);

        public static uint aiVertexWeight_GetVertexId(IntPtr ptrVweight)
        {
            var result = _aiVertexWeight_GetVertexId(ptrVweight);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiAnimation_GetName")]
        private static extern IntPtr _aiAnimation_GetName(IntPtr ptrAnimation);

        public static string aiAnimation_GetName(IntPtr ptrAnimation)
        {
            var result = _aiAnimation_GetName(ptrAnimation);
            var resultConverted = ReadStringFromPointer(result);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiAnimation_GetDuraction")]
        private static extern float _aiAnimation_GetDuraction(IntPtr ptrAnimation);

        public static float aiAnimation_GetDuraction(IntPtr ptrAnimation)
        {
            var result = _aiAnimation_GetDuraction(ptrAnimation);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiAnimation_GetTicksPerSecond")]
        private static extern float _aiAnimation_GetTicksPerSecond(IntPtr ptrAnimation);

        public static float aiAnimation_GetTicksPerSecond(IntPtr ptrAnimation)
        {
            var result = _aiAnimation_GetTicksPerSecond(ptrAnimation);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiAnimation_GetNumChannels")]
        private static extern uint _aiAnimation_GetNumChannels(IntPtr ptrAnimation);

        public static uint aiAnimation_GetNumChannels(IntPtr ptrAnimation)
        {
            var result = _aiAnimation_GetNumChannels(ptrAnimation);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiAnimation_GetNumMorphChannels")]
        private static extern uint _aiAnimation_GetNumMorphChannels(IntPtr ptrAnimation);

        public static uint aiAnimation_GetNumMorphChannels(IntPtr ptrAnimation)
        {
            var result = _aiAnimation_GetNumMorphChannels(ptrAnimation);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiAnimation_GetNumMeshChannels")]
        private static extern uint _aiAnimation_GetNumMeshChannels(IntPtr ptrAnimation);

        public static uint aiAnimation_GetNumMeshChannels(IntPtr ptrAnimation)
        {
            var result = _aiAnimation_GetNumMeshChannels(ptrAnimation);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiAnimation_GetAnimationChannel")]
        private static extern IntPtr _aiAnimation_GetAnimationChannel(IntPtr ptrAnimation, uint uintIndex);

        public static IntPtr aiAnimation_GetAnimationChannel(IntPtr ptrAnimation, uint uintIndex)
        {
            var result = _aiAnimation_GetAnimationChannel(ptrAnimation, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNodeAnim_GetNodeName")]
        private static extern IntPtr _aiNodeAnim_GetNodeName(IntPtr ptrNodeAnim);

        public static string aiNodeAnim_GetNodeName(IntPtr ptrNodeAnim)
        {
            var result = _aiNodeAnim_GetNodeName(ptrNodeAnim);
            var resultConverted = ReadStringFromPointer(result);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNodeAnim_GetNumPositionKeys")]
        private static extern uint _aiNodeAnim_GetNumPositionKeys(IntPtr ptrNodeAnim);

        public static uint aiNodeAnim_GetNumPositionKeys(IntPtr ptrNodeAnim)
        {
            var result = _aiNodeAnim_GetNumPositionKeys(ptrNodeAnim);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNodeAnim_GetNumRotationKeys")]
        private static extern uint _aiNodeAnim_GetNumRotationKeys(IntPtr ptrNodeAnim);

        public static uint aiNodeAnim_GetNumRotationKeys(IntPtr ptrNodeAnim)
        {
            var result = _aiNodeAnim_GetNumRotationKeys(ptrNodeAnim);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNodeAnim_GetNumScalingKeys")]
        private static extern uint _aiNodeAnim_GetNumScalingKeys(IntPtr ptrNodeAnim);

        public static uint aiNodeAnim_GetNumScalingKeys(IntPtr ptrNodeAnim)
        {
            var result = _aiNodeAnim_GetNumScalingKeys(ptrNodeAnim);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNodeAnim_GetPostState")]
        private static extern uint _aiNodeAnim_GetPostState(IntPtr ptrNodeAnim);

        public static uint aiNodeAnim_GetPostState(IntPtr ptrNodeAnim)
        {
            var result = _aiNodeAnim_GetPostState(ptrNodeAnim);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNodeAnim_GetPreState")]
        private static extern uint _aiNodeAnim_GetPreState(IntPtr ptrNodeAnim);

        public static uint aiNodeAnim_GetPreState(IntPtr ptrNodeAnim)
        {
            var result = _aiNodeAnim_GetPreState(ptrNodeAnim);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNodeAnim_GetPositionKey")]
        private static extern IntPtr _aiNodeAnim_GetPositionKey(IntPtr ptrNodeAnim, uint uintIndex);

        public static IntPtr aiNodeAnim_GetPositionKey(IntPtr ptrNodeAnim, uint uintIndex)
        {
            var result = _aiNodeAnim_GetPositionKey(ptrNodeAnim, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNodeAnim_GetRotationKey")]
        private static extern IntPtr _aiNodeAnim_GetRotationKey(IntPtr ptrNodeAnim, uint uintIndex);

        public static IntPtr aiNodeAnim_GetRotationKey(IntPtr ptrNodeAnim, uint uintIndex)
        {
            var result = _aiNodeAnim_GetRotationKey(ptrNodeAnim, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiNodeAnim_GetScalingKey")]
        private static extern IntPtr _aiNodeAnim_GetScalingKey(IntPtr ptrNodeAnim, uint uintIndex);

        public static IntPtr aiNodeAnim_GetScalingKey(IntPtr ptrNodeAnim, uint uintIndex)
        {
            var result = _aiNodeAnim_GetScalingKey(ptrNodeAnim, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiVectorKey_GetTime")]
        private static extern float _aiVectorKey_GetTime(IntPtr ptrVectorKey);

        public static float aiVectorKey_GetTime(IntPtr ptrVectorKey)
        {
            var result = _aiVectorKey_GetTime(ptrVectorKey);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiVectorKey_GetValue")]
        private static extern IntPtr _aiVectorKey_GetValue(IntPtr ptrVectorKey);

        public static float[] aiVectorKey_GetValue(IntPtr ptrVectorKey)
        {
            var result = _aiVectorKey_GetValue(ptrVectorKey);
            var resultConverted = ReadFloatArray(result, 3);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiQuatKey_GetTime")]
        private static extern float _aiQuatKey_GetTime(IntPtr ptrQuatKey);

        public static float aiQuatKey_GetTime(IntPtr ptrQuatKey)
        {
            var result = _aiQuatKey_GetTime(ptrQuatKey);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiQuatKey_GetValue")]
        private static extern IntPtr _aiQuatKey_GetValue(IntPtr ptrQuatKey);

        public static float[] aiQuatKey_GetValue(IntPtr ptrQuatKey)
        {
            var result = _aiQuatKey_GetValue(ptrQuatKey);
            var resultConverted = ReadFloatArray(result, 4);
            return resultConverted;
        }

        //CAMERA
        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiCamera_GetAspect")]
        private static extern float _aiCamera_GetAspect(IntPtr ptrCamera);

        public static float aiCamera_GetAspect(IntPtr ptrCamera)
        {
            var result = _aiCamera_GetAspect(ptrCamera);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiCamera_GetClipPlaneFar")]
        private static extern float _aiCamera_GetClipPlaneFar(IntPtr ptrCamera);

        public static float aiCamera_GetClipPlaneFar(IntPtr ptrCamera)
        {
            var result = _aiCamera_GetClipPlaneFar(ptrCamera);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiCamera_GetClipPlaneNear")]
        private static extern float _aiCamera_GetClipPlaneNear(IntPtr ptrCamera);

        public static float aiCamera_GetClipPlaneNear(IntPtr ptrCamera)
        {
            var result = _aiCamera_GetClipPlaneNear(ptrCamera);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiCamera_GetHorizontalFOV")]
        private static extern float _aiCamera_GetHorizontalFOV(IntPtr ptrCamera);

        public static float aiCamera_GetHorizontalFOV(IntPtr ptrCamera)
        {
            var result = _aiCamera_GetHorizontalFOV(ptrCamera);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiCamera_GetLookAt")]
        private static extern IntPtr _aiCamera_GetLookAt(IntPtr ptrCamera);

        public static Vector3 aiCamera_GetLookAt(IntPtr ptrCamera)
        {
            var result = _aiCamera_GetLookAt(ptrCamera);
            var resultConverted = ReadStruct<Vector3>(result, false);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiCamera_GetName")]
        private static extern IntPtr _aiCamera_GetName(IntPtr ptrCamera);

        public static string aiCamera_GetName(IntPtr ptrCamera)
        {
            var result = _aiCamera_GetName(ptrCamera);
            var resultConverted = ReadStringFromPointer(result);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiCamera_GetPosition")]
        private static extern IntPtr _aiCamera_GetPosition(IntPtr ptrCamera);

        public static Vector3 aiCamera_GetPosition(IntPtr ptrCamera)
        {
            var result = _aiCamera_GetPosition(ptrCamera);
            var resultConverted = ReadStruct<Vector3>(result, false);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiCamera_GetUp")]
        private static extern IntPtr _aiCamera_GetUp(IntPtr ptrCamera);

        public static Vector3 aiCamera_GetUp(IntPtr ptrCamera)
        {
            var result = _aiCamera_GetUp(ptrCamera);
            var resultConverted = ReadStruct<Vector3>(result, false);
            return resultConverted;
        }

        //LIGHT
        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiLight_GetAngleInnerCone")]
        private static extern float _aiLight_GetAngleInnerCone(IntPtr ptrLight);

        public static float aiLight_GetAngleInnerCone(IntPtr ptrLight)
        {
            var result = _aiLight_GetAngleInnerCone(ptrLight);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiLight_GetAngleOuterCone")]
        private static extern float _aiLight_GetAngleOuterCone(IntPtr ptrLight);

        public static float aiLight_GetAngleOuterCone(IntPtr ptrLight)
        {
            var result = _aiLight_GetAngleOuterCone(ptrLight);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiLight_GetAttenuationConstant")]
        private static extern float _aiLight_GetAttenuationConstant(IntPtr ptrLight);

        public static float aiLight_GetAttenuationConstant(IntPtr ptrLight)
        {
            var result = _aiLight_GetAttenuationConstant(ptrLight);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiLight_GetAttenuationLinear")]
        private static extern float _aiLight_GetAttenuationLinear(IntPtr ptrLight);

        public static float aiLight_GetAttenuationLinear(IntPtr ptrLight)
        {
            var result = _aiLight_GetAttenuationLinear(ptrLight);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiLight_GetAttenuationQuadratic")]
        private static extern float _aiLight_GetAttenuationQuadratic(IntPtr ptrLight);

        public static float aiLight_GetAttenuationQuadratic(IntPtr ptrLight)
        {
            var result = _aiLight_GetAttenuationQuadratic(ptrLight);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiLight_GetColorAmbient")]
        private static extern IntPtr _aiLight_GetColorAmbient(IntPtr ptrLight);

        public static Color aiLight_GetColorAmbient(IntPtr ptrLight)
        {
            var result = _aiLight_GetColorAmbient(ptrLight);
            var resultConverted = ReadStruct<Color>(result, false);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiLight_GetColorDiffuse")]
        private static extern IntPtr _aiLight_GetColorDiffuse(IntPtr ptrLight);

        public static Color aiLight_GetColorDiffuse(IntPtr ptrLight)
        {
            var result = _aiLight_GetColorDiffuse(ptrLight);
            var resultConverted = ReadStruct<Color>(result, false);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiLight_GetColorSpecular")]
        private static extern IntPtr _aiLight_GetColorSpecular(IntPtr ptrLight);

        public static Color aiLight_GetColorSpecular(IntPtr ptrLight)
        {
            var result = _aiLight_GetColorSpecular(ptrLight);
            var resultConverted = ReadStruct<Color>(result, false);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiLight_GetDirection")]
        private static extern IntPtr _aiLight_GetDirection(IntPtr ptrLight);

        public static Vector3 aiLight_GetDirection(IntPtr ptrLight)
        {
            var result = _aiLight_GetDirection(ptrLight);
            var resultConverted = ReadStruct<Vector3>(result, false);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "aiLight_GetName")]
        private static extern IntPtr _aiLight_GetName(IntPtr ptrLight);

        public static string aiLight_GetName(IntPtr ptrLight)
        {
            var result = _aiLight_GetName(ptrLight);
            var resultConverted = ReadStringFromPointer(result);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "aiScene_GetMetadataCount")]
        private static extern uint _aiScene_GetMetadataCount(IntPtr ptrScene);

        public static uint aiScene_GetMetadataCount(IntPtr ptrScene)
        {
            var result = _aiScene_GetMetadataCount(ptrScene);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "aiScene_GetMetadataKey")]
        private static extern IntPtr _aiScene_GetMetadataKey(IntPtr ptrScene, uint uintIndex);

        public static string aiScene_GetMetadataKey(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetMetadataKey(ptrScene, uintIndex);
            var resultConverted = ReadStringFromPointer(result);
            return resultConverted;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "aiScene_GetMetadataType")]
        private static extern int _aiScene_GetMetadataType(IntPtr ptrScene, uint uintIndex);

        public static AssimpMetadataType aiScene_GetMetadataType(IntPtr ptrScene, uint uintIndex)
        {
            var result = (AssimpMetadataType)_aiScene_GetMetadataType(ptrScene, uintIndex);
            return result;
        }

        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "aiScene_GetMetadataValue")]
        private static extern IntPtr _aiScene_GetMetadataValue(IntPtr ptrScene, uint uintIndex);

        public static bool aiScene_GetMetadataBoolValue(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetMetadataValue(ptrScene, uintIndex);
            return GetNewBool(result);
        }

        public static int aiScene_GetMetadataInt32Value(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetMetadataValue(ptrScene, uintIndex);
            return GetNewInt32(result);
        }

        public static long aiScene_GetMetadataInt64Value(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetMetadataValue(ptrScene, uintIndex);
            return GetNewInt64(result);
        }

        public static float aiScene_GetMetadataFloatValue(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetMetadataValue(ptrScene, uintIndex);
            return GetNewFloat(result);
        }

        public static double aiScene_GetMetadataDoubleValue(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetMetadataValue(ptrScene, uintIndex);
            return GetNewDouble(result);
        }

        public static string aiScene_GetMetadataStringValue(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetMetadataValue(ptrScene, uintIndex);
            return ReadStringFromPointer(result);
        }

        public static Vector3 aiScene_GetMetadataVectorValue(IntPtr ptrScene, uint uintIndex)
        {
            var result = _aiScene_GetMetadataValue(ptrScene, uintIndex);
            var resultConverted = ReadStruct<Vector3>(result, false);
            return resultConverted;
        }

        #endregion

        #region Helpers

        public static GCHandle LockGc(object value)
        {
            return GCHandle.Alloc(value, GCHandleType.Pinned);
        }

        private static IntPtr AllocHGlobal<T>()
        {
#if NETFX_CORE || NET_4_6
            var locked = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
#else
            var locked = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
#endif
            return locked;
        }

        private static T ReadStruct<T>(IntPtr pointer, bool dealloc = true)
        {
#if NETFX_CORE || NET_4_6
            T structure = (T)Marshal.PtrToStructure<T>(pointer);
#else
            T structure = (T)Marshal.PtrToStructure(pointer, typeof(T));
#endif
            if (dealloc)
            {
                Marshal.FreeHGlobal(pointer);
            }
            return structure;
        }

        private static byte[] StringToByteArray(string str, int length)
        {
            return Encoding.ASCII.GetBytes(str.PadRight(length, '\0'));
        }

        private static string ByteArrayToString(byte[] value)
        {
            var count = Array.IndexOf<byte>(value, 0, 0);
            if (count < 0)
                count = value.Length;
            return Encoding.ASCII.GetString(value, 0, count);
        }

        private static GCHandle GetStringBuffer(string value)
        {
            var buffer = StringToByteArray(value, MaxStringLength);
            return LockGc(buffer);
        }

        private static IntPtr GetAssimpStringBuffer(string value)
        {
            var offset = Is32Bits ? 4 : 8;
            var buffer = Marshal.AllocHGlobal(offset + value.Length);
            if (Is32Bits)
                Marshal.WriteInt32(buffer, value.Length);
            {
                Marshal.WriteInt64(buffer, value.Length);
            }
            var bytes = Encoding.ASCII.GetBytes(value);
            Marshal.Copy(bytes, 0, new IntPtr(Is32Bits ? buffer.ToInt32() : buffer.ToInt64() + offset), value.Length);
            return buffer;
        }

        private static GCHandle GetNewStringBuffer(out byte[] byteArray)
        {
            byteArray = new byte[MaxInputStringLength];
            return LockGc(byteArray);
        }

        private static string ReadStringFromPointer(IntPtr pointer)
        {
            return Marshal.PtrToStringAnsi(pointer);
        }

        private static bool GetNewBool(IntPtr pointer)
        {
            return Marshal.ReadByte(pointer) == 1;
        }

        private static int GetNewInt32(IntPtr pointer)
        {
            return Marshal.ReadInt32(pointer);
        }

        private static long GetNewInt64(IntPtr pointer)
        {
            return Marshal.ReadInt64(pointer);
        }

        private static float GetNewFloat(IntPtr pointer)
        {
            var result = new float[1];
            Marshal.Copy(pointer, result, 0, 1);
            return result[0];
        }

        private static double GetNewDouble(IntPtr pointer)
        {
            var result = new double[1];
            Marshal.Copy(pointer, result, 0, 1);
            return result[0];
        }

        private static Matrix4x4 GetNewMatrix4x4(IntPtr pointer)
        {
            var matrix = new Matrix4x4();
            var data = new float[16];
            Marshal.Copy(pointer, data, 0, 16);
            matrix[0] = data[0];
            matrix[4] = data[1];
            matrix[8] = data[2];
            matrix[12] = data[3];
            matrix[1] = data[4];
            matrix[5] = data[5];
            matrix[9] = data[6];
            matrix[13] = data[7];
            matrix[2] = data[8];
            matrix[6] = data[9];
            matrix[10] = data[10];
            matrix[14] = data[11];
            matrix[3] = data[12];
            matrix[7] = data[13];
            matrix[11] = data[14];
            matrix[15] = data[15];
            return matrix;
        }

        private static GCHandle Matrix4x4ToAssimp(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            var matrix = Matrix4x4.TRS(translation, Quaternion.Euler(rotation), scale);
            var array = new float[16];
            array[0] = matrix[0];
            array[4] = matrix[1];
            array[8] = matrix[2];
            array[12] = matrix[3];
            array[1] = matrix[4];
            array[5] = matrix[5];
            array[9] = matrix[6];
            array[13] = matrix[7];
            array[2] = matrix[8];
            array[6] = matrix[9];
            array[10] = matrix[10];
            array[14] = matrix[11];
            array[3] = matrix[12];
            array[7] = matrix[13];
            array[11] = matrix[14];
            array[15] = matrix[15];
            return LockGc(array);
        }

        private static float[] ReadFloatArray(IntPtr pointer, int size)
        {
            var result = new float[size];
            Marshal.Copy(pointer, result, 0, size);
            return result;
        }
        #endregion
    }
}

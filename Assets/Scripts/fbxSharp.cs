using TriLib;
using UnityEngine;

public static class FbxLoaderUtils
{
    public static GameObject LoadFile(string fileName) =>
        new AssetLoader().LoadFromFileWithTextures(fileName, AssetLoaderOptions.CreateInstance());
}

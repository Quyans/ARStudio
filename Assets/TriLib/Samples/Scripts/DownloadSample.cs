using System;
using System.IO;
using System.Collections;
using UnityEngine;
#if TRILIB_USE_ZIP
#if !UNITY_EDITOR && UNITY_WINRT
using System.IO.Compression;
#else
using ICSharpCode.SharpZipLib.Zip;
#endif
#endif
namespace TriLib
{
    public class DownloadSample : MonoBehaviour
    {
        //Here are our asset URLs
        private string[] urls = {
            "http://ricardoreis.net/trilib/test1.zip",
            "http://ricardoreis.net/trilib/test2.zip",
            "http://ricardoreis.net/trilib/test3.zip",
            "http://ricardoreis.net/trilib/test1.3ds"
        };

        //Stores a reference for file downloaders
        private WWW[] fileDownloaders;

        //Reference for the latest loaded GameObject
        private GameObject _loadedGameObject;

        //Start logic
        private void Start()
        {
            //For each asset on the list, we create a slot for a new WWW instance
            fileDownloaders = new WWW[urls.Length];
        }

        //GUI logic
        private void OnGUI()
        {
            //Loop thru all avaliable URLs
            for (var i = 0; i < urls.Length; i++)
            {
                //Gets current URL
                var url = urls[i];

                //Gets current WWW instance (if avaliable)
                var fileDownloader = fileDownloaders[i];

                GUILayout.BeginHorizontal();

                //Shows the URL on GUI
                GUILayout.Label(url);

                //Checks if current file downloader exists
                if (fileDownloader == null)
                {
                    //When clicking the "Load" button
                    if (GUILayout.Button("Load"))
                    {
                        //Destroys the latest loaded GameObject, if avaliable
                        if (_loadedGameObject != null)
                        {
                            Destroy(_loadedGameObject);
                        }

                        //Gets current file name, extension, local path and local filename
                        var fileName = FileUtils.GetFilenameWithoutExtension(url);
                        var fileExtension = FileUtils.GetFileExtension(url);
                        var localFilePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);
                        var localFilename = string.Format("{0}/{1}{2}", localFilePath, fileName, fileExtension);

                        //Checks if local path exists, which indicates the file has been downloaded
                        if (Directory.Exists(localFilePath))
                        {
                            //Loads local file
                            LoadFile(fileExtension, localFilename);
                        }
                        else
                        {
                            //If local path doesn't exists, download the file and create the local folder
                            StartCoroutine(DownloadFile(url, i, fileExtension, localFilePath, localFilename));
                        }
                    }
                }
                else
                {
                    //Shows the current file download progress
                    GUILayout.Label(string.Format("Downloaded {0:P2}", fileDownloader.bytesDownloaded == 0 ? 0f : fileDownloader.progress));
                }
                GUILayout.EndHorizontal();
            }
        }

        //Searches inside a path and returns the first path of an asset loadable by TriLib
        private string GetReadableAssetPath(string path)
        {
            var supportedExtensions = AssetLoaderBase.GetSupportedFileExtensions();
            foreach (var file in Directory.GetFiles(path))
            {
                var fileExtension = FileUtils.GetFileExtension(file);
                if (supportedExtensions.Contains(fileExtension))
                {
                    return file;
                }
            }
            foreach (var directory in Directory.GetDirectories(path))
            {
                var assetPath = GetReadableAssetPath(directory);
                if (assetPath != null)
                {
                    return assetPath;
                }
            }
            return null;
        }

        //Loads an existing local file
        private void LoadFile(string fileExtension, string localFilename)
        {
            //Creates a new AssetLoader instance
            using (var assetLoader = new AssetLoader())
            {
                //Checks if the URL is a ZIP file
                if (fileExtension == ".zip")
                {
#if TRILIB_USE_ZIP
                    var localFilePath = FileUtils.GetFileDirectory(localFilename);

                    //Gets the first asset loadable by TriLib on the folder
                    var assetPath = GetReadableAssetPath(localFilePath);
                    if (assetPath == null)
                    {
                        Debug.LogError("No TriLib readable file could be found on the given directory");
                        return;
                    }

                    //Loads the found asset
                    _loadedGameObject = assetLoader.LoadFromFile(assetPath);
#else
                    throw new Exception("Please enable TriLib ZIP loading");
#endif
                }
                else
                {
                    //If the URL is not a ZIP file, loads the file inside the folder
                    _loadedGameObject = assetLoader.LoadFromFile(localFilename);
                }

                //Move camera away to fit the loaded object in view
                CameraExtensions.FitToBounds(Camera.main, _loadedGameObject.transform, 3f);
            }
        }

        //Downloads a file to a local path or extract all ZIP file contents to the local path in case of ZIP files, then loads the file
        private IEnumerator DownloadFile(string url, int index, string fileExtension, string localFilePath, string localFilename)
        {
            fileDownloaders[index] = new WWW(url);
            yield return fileDownloaders[index];
            if (fileExtension == ".zip")
            {
#if TRILIB_USE_ZIP
                using (var memoryStream = new MemoryStream(fileDownloaders[index].bytes))
                {
                    UnzipFromStream(memoryStream, localFilePath);
                }
#else
                throw new Exception("Please enable TriLib ZIP loading");
#endif
            }
            Directory.CreateDirectory(localFilePath);
            File.WriteAllBytes(localFilename, fileDownloaders[index].bytes);
            LoadFile(fileExtension, localFilename);
            fileDownloaders[index] = null;
        }

#if TRILIB_USE_ZIP
        //Helper function to extract all ZIP file contents to a local folder
        private void UnzipFromStream(Stream zipStream, string outFolder)
        {
#if !UNITY_EDITOR && (NET_4_6 || NETFX_CORE)
            var zipFile = new ZipArchive(zipStream, ZipArchiveMode.Read);
            foreach (ZipArchiveEntry zipEntry in zipFile.Entries)
            {
                var zipFileStream = zipEntry.Open();
#else
            var zipFile = new ZipFile(zipStream);
            foreach (ZipEntry zipEntry in zipFile)
            {
                if (!zipEntry.IsFile)
                {
                    continue;
                }
                var zipFileStream = zipFile.GetInputStream(zipEntry);
#endif
                var entryFileName = zipEntry.Name;
                var buffer = new byte[4096];
                var fullZipToPath = Path.Combine(outFolder, entryFileName);
                var directoryName = Path.GetDirectoryName(fullZipToPath);
                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(directoryName);
                }
                var fileName = Path.GetFileName(fullZipToPath);
                if (fileName.Length == 0)
                {
                    continue;
                }
                using (var streamWriter = File.Create(fullZipToPath))
                {
#if !UNITY_EDITOR && (NET_4_6 || NETFX_CORE)
                    zipFileStream.CopyTo(streamWriter);
#else
                    ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(zipFileStream, streamWriter, buffer);
#endif
                }
            }
        }
#endif
    }
}
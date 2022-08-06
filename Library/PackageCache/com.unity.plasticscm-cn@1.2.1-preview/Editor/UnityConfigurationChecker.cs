using System;
using System.IO;

using Codice.Client.Common;
using Codice.Utils;
using Unity.PlasticSCM.Editor.Tool;

namespace Unity.PlasticSCM.Editor
{
    internal static class UnityConfigurationChecker
    {
        internal static bool NeedsConfiguration()
        {
            string plasticClientBinDir = PlasticInstallPath.GetClientBinDir();

            if (string.IsNullOrEmpty(plasticClientBinDir))
                return true;

            SetupUnityEditionToken.FromPlasticInstallation(plasticClientBinDir);

            return ConfigurationChecker.NeedConfiguration();
        }
    }

    internal static class SetupUnityEditionToken
    {
        internal static void ToCloud()
        {
            SetupTokenFiles(true, false);
        }

        internal static void FromPlasticInstallation(string plasticClientBinDir)
        {
            bool isCloudPlasticInstall = EditionToken.IsCloudEditionForPath(plasticClientBinDir);
            bool isDvcsPlasticInstall = EditionToken.IsDvcsEditionForPath(plasticClientBinDir);

            SetupTokenFiles(
                isCloudPlasticInstall,
                isDvcsPlasticInstall);
        }

        static void SetupTokenFiles(
            bool isCloudPlasticInstall,
            bool isDvcsPlasticInstall)
        {
            string unityCloudEditionTokenFile = Path.Combine(
                ApplicationLocation.GetAppPath(),
                EditionToken.CLOUD_EDITION_FILE_NAME);

            string unityDvcsEditionTokenFile = Path.Combine(
                ApplicationLocation.GetAppPath(),
                EditionToken.DVCS_EDITION_FILE_NAME);

            CreateOrDeleteTokenFile(isCloudPlasticInstall, unityCloudEditionTokenFile);
            CreateOrDeleteTokenFile(isDvcsPlasticInstall, unityDvcsEditionTokenFile);
        }

        static void CreateOrDeleteTokenFile(bool isEdition, string editionTokenFile)
        {
            if (isEdition && !File.Exists(editionTokenFile))
            {
                File.Create(editionTokenFile).Dispose();

                string metaPath = MetaPath.GetMetaPath(editionTokenFile);

                if (!File.Exists(metaPath))
                    File.WriteAllText(metaPath, CLOUD_EDITION_TOKEN_META_CONTENT);

                return;
            }

            if (!isEdition && File.Exists(editionTokenFile))
            {
                File.Delete(editionTokenFile);

                string metaPath = MetaPath.GetMetaPath(editionTokenFile);

                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }

                return;
            }
        }

        static string CLOUD_EDITION_TOKEN_META_CONTENT =
            "fileFormatVersion: 2" + Environment.NewLine +
            "guid: 5d56f0dbdd6197440973edc78adb7f7a" + Environment.NewLine +
            "DefaultImporter:" + Environment.NewLine +
            "  externalObjects: {}" + Environment.NewLine +
            "  userData: " + Environment.NewLine +
            "  assetBundleName: " + Environment.NewLine +
            "  assetBundleVariant: " + Environment.NewLine;
    }
}

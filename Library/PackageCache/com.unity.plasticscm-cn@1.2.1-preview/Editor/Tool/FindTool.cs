using System;
using System.Collections.Generic;
using System.IO;

namespace Unity.PlasticSCM.Editor.Tool
{
    internal static class FindTool
    {
        internal static string ObtainToolCommand(
            string toolName, List<string> installationPaths)
        {
            List<string> folderPaths = new List<string>();

            folderPaths.AddRange(GetPathEnvironmentVariableFromProcess());
            folderPaths.AddRange(GetPathEnvironmentVariableFromMachine());
            folderPaths.AddRange(installationPaths);

            foreach (string path in folderPaths)
            {
                if (path == null)
                    continue;

                if (path.Trim() == string.Empty)
                    continue;

                string filePath = CleanFolderPath(path);

                filePath = Path.Combine(filePath, toolName);

                if (File.Exists(filePath))
                    return Path.GetFullPath(filePath);
            }

            return null;
        }

        static string CleanFolderPath(string folderPath)
        {
            foreach (char c in Path.GetInvalidPathChars())
                folderPath = folderPath.Replace(c.ToString(), string.Empty);

            return folderPath;
        }

        static string[] GetPathEnvironmentVariableFromProcess()
        {
            string pathVariable = 
                Environment.GetEnvironmentVariable(
                    PATH_ENVIRONMENT_VARIABLE);

            return pathVariable.Split(Path.PathSeparator);
        }

        static string[] GetPathEnvironmentVariableFromMachine()
        {
            string pathVariable = 
                Environment.GetEnvironmentVariable(
                    PATH_ENVIRONMENT_VARIABLE,
                    EnvironmentVariableTarget.Machine);

            return pathVariable.Split(Path.PathSeparator);
        }

        const string PATH_ENVIRONMENT_VARIABLE = "PATH";
    }
}

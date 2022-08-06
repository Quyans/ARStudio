using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

using Codice.LogWrapper;

namespace Unity.PlasticSCM.Editor.ProjectDownloader
{
    [InitializeOnLoad]
    internal static class CloudProjectDownloader
    {
        static CloudProjectDownloader()
        {
            if (mIsAlreadyExecuted)
                return;
            
            // EditorApplication.update += RunOnce;
        }

        static void RunOnce()
        {
            EditorApplication.update -= RunOnce;

            mIsAlreadyExecuted = true;

            Execute();
        }

        static void Execute()
        {
            Dictionary<string, string> args = CommandLineArguments.Build(
                Environment.GetCommandLineArgs());

            mLog.DebugFormat(
                "Processing Unity arguments: {0}",
                string.Join(" ", Environment.GetCommandLineArgs()));

            string projectPath = ParseArguments.ProjectPath(args);
            string cloudRepository = ParseArguments.CloudProject(args);
            string cloudOrganization = ParseArguments.CloudOrganization(args);

            if (string.IsNullOrEmpty(projectPath) ||
                string.IsNullOrEmpty(cloudRepository) ||
                string.IsNullOrEmpty(cloudOrganization))
                return;

            PlasticApp.Initialize();

            DownloadRepositoryOperation downloadOperation = new DownloadRepositoryOperation();

            downloadOperation.DownloadRepositoryToPathIfNeeded(
                cloudRepository,
                cloudOrganization,
                Path.GetFullPath(projectPath));
        }

        [SerializeField]
        static bool mIsAlreadyExecuted;

        static readonly ILog mLog = LogManager.GetLogger("ProjectDownloader");
    }
}

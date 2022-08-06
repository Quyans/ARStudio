using Codice.CM.Common;
using PlasticGui.WorkspaceWindow;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.AutoSetup
{
    public class PlasticHubUtil
    {
        public static string GetRepoURLFromWorkspace(WorkspaceInfo wkInfo)
        {
            if (wkInfo == null)
            {
                return null;
            }

            WorkspaceStatusString.Data wkStatusData =
                WorkspaceStatusString.GetSelectorData(wkInfo);

            string repositoryName = wkStatusData.RepositoryName;
            string server = wkStatusData.Server;
            if (string.IsNullOrEmpty(repositoryName) || string.IsNullOrEmpty(server))
            {
                return null;
            }
            
            string orgName = GetOrgNameFromServer(server);
            string plasticHubURL = GetPlasticHubServer(server);
            if (string.IsNullOrEmpty(orgName) || string.IsNullOrEmpty(plasticHubURL))
            {
                return null;
            }

            return string.Format("{0}/{1}/{2}",
                plasticHubURL,
                orgName,
                repositoryName);
        }

        private static string GetOrgNameFromServer(string server)
        {
            
            string[] urlParts = server.Split('@');
            if (urlParts.Length != 2)
            {
                return null;
            }
            
            if (urlParts[0].StartsWith(mSSLPrefix))
            {
                return server.Substring(mSSLPrefix.Length, urlParts[0].Length - mSSLPrefix.Length);
            }
            
            return urlParts[0];
        }

        private static string GetPlasticHubServer(string server)
        {
            string[] urlParts = server.Split('@');
            if (urlParts.Length != 2)
            {
                return null;
            }

            if (!urlParts[1].Contains("plasticscm"))
            {
                return null;
            }

            if (urlParts[1].Contains("-plasticscm-int"))
            {
                return "https://plastichub-int.unity.cn";
            }

            return "https://plastichub.unity.cn";
        }

        private const string mSSLPrefix = "ssl://";
    }
}
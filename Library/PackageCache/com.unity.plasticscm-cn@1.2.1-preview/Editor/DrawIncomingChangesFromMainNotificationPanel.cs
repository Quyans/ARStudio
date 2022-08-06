using UnityEditor;
using UnityEngine;

using Codice.CM.Common;
using PlasticGui;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor
{
    internal static class DrawIncomingChangesFromMainNotificationPanel
    {
        internal static void ForMode(
            WorkspaceInfo workspaceInfo,
            bool isGluonMode, 
            bool isIncomingChangesVisible,
            bool isVisible,
            NotificationPanelData notificationPanelData)
        {
            if (!isVisible)
                return;

            if (isIncomingChangesVisible)
            {
                return;
            }

            if (isGluonMode)
            {
                return;
            }

            GUIContent labelContent = new GUIContent(
                notificationPanelData.InfoText, notificationPanelData.TooltipText);
            GUIContent buttonContent = new GUIContent(
                notificationPanelData.ActionText, notificationPanelData.TooltipText);

            float panelWidth = DrawIncomingChangesNotificationPanel.GetPanelWidth(
                labelContent, buttonContent,
                UnityStyles.Notification.Label, EditorStyles.miniButton);

            EditorGUILayout.BeginHorizontal(
                UnityStyles.Notification.GreenNotification,
                GUILayout.Width(panelWidth));

            GUILayout.Label(labelContent, UnityStyles.Notification.Label);
                
            DoActionButton(workspaceInfo, buttonContent, EditorStyles.miniButton);
                
            EditorGUILayout.EndHorizontal();
        }

        static void DoActionButton(
            WorkspaceInfo workspaceInfo, 
            GUIContent buttonContent, 
            GUIStyle buttonStyle)
        {
            if (!GUILayout.Button(
                buttonContent, buttonStyle,
                GUILayout.ExpandHeight(true),
                GUILayout.MinWidth(40)))
                return;
            
            LaunchTool.OpenBranchExplorer(workspaceInfo);
        }
    }
}

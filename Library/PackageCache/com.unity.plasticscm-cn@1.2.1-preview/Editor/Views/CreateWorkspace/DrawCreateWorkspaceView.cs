using System;

using UnityEditor;
using UnityEngine;

using Codice.Client.Common;
using Codice.CM.Common;
using PlasticGui;
using PlasticGui.SwitcherWindow.Repositories;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.Views.CreateWorkspace.Dialogs;

namespace Unity.PlasticSCM.Editor.Views.CreateWorkspace
{
    internal static class DrawCreateWorkspace
    {
        internal static void ForState(
            Action<RepositoryCreationData> createRepositoryAction,
            Action<CreateWorkspaceViewState> createWorkspaceAction,
            EditorWindow parentWindow,
            string defaultServer,
            ref CreateWorkspaceViewState state)
        {
            DoTitle();

            GUILayout.Space(15);

            DoFieldsArea(
                createRepositoryAction,
                parentWindow,
                defaultServer,
                ref state);

            GUILayout.Space(10);

            DoRadioButtonsArea(ref state);

            GUILayout.Space(3);

            DoHelpLabel();

            GUILayout.Space(10);

            DoCreateWorkspaceButton(
                createWorkspaceAction,
                ref state);

            GUILayout.Space(5);

            DoNotificationArea(state.ProgressData);
        }

        static void DoTitle()
        {
            GUILayout.Label(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.NewWorkspace),
                UnityStyles.Dialog.MessageTitle);

            GUILayout.Label(
                "Workspaces are used to store the local copies of your project files that you can checkin to a repository.",
                EditorStyles.wordWrappedLabel);
        }

        static void DoFieldsArea(
            Action<RepositoryCreationData> createRepositoryAction,
            EditorWindow parentWindow,
            string defaultServer,
            ref CreateWorkspaceViewState state)
        {
            DoRepositoryField(
                createRepositoryAction,
                parentWindow,
                defaultServer,
                ref state);

            DoWorkspaceField(ref state);
        }

        static void DoRepositoryField(
            Action<RepositoryCreationData> createRepositoryAction,
            EditorWindow parentWindow,
            string defaultServer,
            ref CreateWorkspaceViewState state)
        {
            EditorGUILayout.BeginHorizontal();

            DoLabel("Repository name");

            state.RepositoryName = DoTextField(
                state.RepositoryName,
                !state.ProgressData.IsOperationRunning,
                LABEL_WIDTH,
                TEXTBOX_WIDTH - BROWSE_BUTTON_WIDTH);

            float browseButtonX =
                LABEL_WIDTH + TEXTBOX_WIDTH + BUTTON_MARGIN -
                BROWSE_BUTTON_WIDTH;
            float browseButtonWidth =
                BROWSE_BUTTON_WIDTH - BUTTON_MARGIN;

            if (DoButton(
                    "...",
                    !state.ProgressData.IsOperationRunning,
                    browseButtonX,
                    browseButtonWidth))
            {
                DoBrowseRepositoryButton(
                    parentWindow,
                    defaultServer,
                    ref state);
                EditorGUIUtility.ExitGUI();
            }

            float newButtonX =
                LABEL_WIDTH + TEXTBOX_WIDTH + BUTTON_MARGIN;
            float newButtonWidth =
                NEW_BUTTON_WIDTH - BUTTON_MARGIN;

            if (DoButton(
                    "New",
                    !state.ProgressData.IsOperationRunning,
                    newButtonX, newButtonWidth))
            {
                DoNewRepositoryButton(
                    createRepositoryAction,
                    parentWindow,
                    state.RepositoryName,
                    defaultServer);
                EditorGUIUtility.ExitGUI();
            }

            ValidationResult validationResult = ValidateRepositoryName(
                state.RepositoryName);

            if (!validationResult.IsValid)
                DoWarningLabel(validationResult.ErrorMessage,
                    LABEL_WIDTH + TEXTBOX_WIDTH + NEW_BUTTON_WIDTH + LABEL_MARGIN);

            EditorGUILayout.EndHorizontal();
        }

        static void DoWorkspaceField(
            ref CreateWorkspaceViewState state)
        {
            EditorGUILayout.BeginHorizontal();

            DoLabel("Workspace name");

            state.WorkspaceName = DoTextField(
                state.WorkspaceName,
                !state.ProgressData.IsOperationRunning,
                LABEL_WIDTH,
                TEXTBOX_WIDTH - BROWSE_BUTTON_WIDTH);

            ValidationResult validationResult = ValidateWorkspaceName(
                state.WorkspaceName);

            if (!validationResult.IsValid)
                DoWarningLabel(validationResult.ErrorMessage,
                    LABEL_WIDTH + TEXTBOX_WIDTH - BROWSE_BUTTON_WIDTH + LABEL_MARGIN);

            EditorGUILayout.EndHorizontal();
        }

        static void DoRadioButtonsArea(
            ref CreateWorkspaceViewState state)
        {
            EditorGUILayout.BeginVertical();
            DoLabel("How do you prefer to work?");

            if (DoRadioButton(
                "I need branching, merging and the ability to push/pull (Plastic workspace)",
                state.WorkspaceMode == CreateWorkspaceViewState.WorkspaceModes.Developer,
                !state.ProgressData.IsOperationRunning,
                RADIO_BUTTON_MARGIN))
                state.WorkspaceMode = CreateWorkspaceViewState.WorkspaceModes.Developer;

            if (DoRadioButton(
                "I simply need to checkin my work (Gluon workspace)",
                state.WorkspaceMode == CreateWorkspaceViewState.WorkspaceModes.Gluon,
                !state.ProgressData.IsOperationRunning,
                RADIO_BUTTON_MARGIN))
                state.WorkspaceMode = CreateWorkspaceViewState.WorkspaceModes.Gluon;

            EditorGUILayout.EndVertical();
        }

        static void DoCreateWorkspaceButton(
            Action<CreateWorkspaceViewState> createWorkspaceAction,
            ref CreateWorkspaceViewState state)
        {
            EditorGUILayout.BeginHorizontal();

            bool isButtonEnabled =
                IsValidState(state) &&
                !state.ProgressData.IsOperationRunning;

            if (DoButton("Create workspace", isButtonEnabled,
                    CREATE_WORKSPACE_BUTTON_MARGIN, CREATE_WORKSPACE_BUTTON_WIDTH))
            {
                createWorkspaceAction(state);
                return;
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
        }

        static void DoBrowseRepositoryButton(
            EditorWindow parentWindow,
            string defaultServer,
            ref CreateWorkspaceViewState state)
        {
            string result = RepositoryExplorerDialog.BrowseRepository(
                parentWindow, defaultServer);

            if (string.IsNullOrEmpty(result))
                return;

            state.RepositoryName = result;
        }

        static void DoNewRepositoryButton(
            Action<RepositoryCreationData> createRepositoryAction,
            EditorWindow parentWindow,
            string repositoryName,
            string defaultServer)
        {
            string proposedRepositoryName = string.Empty;
            string proposedServer = string.Empty;

            RepositorySpec repSpec = GetRepSpecFromName(repositoryName);

            if (repSpec != null)
            {
                proposedRepositoryName = repSpec.Name;
                proposedServer = repSpec.Server;
            }

            RepositoryCreationData creationData = CreateRepositoryDialog.CreateRepository(
                parentWindow,
                proposedRepositoryName,
                proposedServer,
                defaultServer,
                ClientConfig.Get().GetWorkspaceServer());

            createRepositoryAction(creationData);
        }

        static void DoHelpLabel()
        {
            string linkText = "here.";
            string labelText = string.Format(
                "Learn more about the differences between Plastic/Gluon workspaces <color=\"{0}\">{1}</color>",
                UnityStyles.HexColors.LINK_COLOR,
                linkText);

            EditorGUILayout.BeginHorizontal();

            if (DoLinkLabel(labelText, linkText, RADIO_BUTTON_MARGIN))
                Application.OpenURL(HELP_URL);

            EditorGUILayout.EndHorizontal();
        }

        static void DoNotificationArea(ProgressControlsForViews.Data progressData)
        {
            if (string.IsNullOrEmpty(progressData.NotificationMessage))
                return;

            DrawProgressForViews.ForNotificationArea(progressData);
        }

        static void DoLabel(string labelText)
        {
            GUIStyle labelStyle = EditorStyles.label;

            Rect rect = GUILayoutUtility.GetRect(
                new GUIContent(labelText),
                labelStyle);

            GUI.Label(rect, labelText, labelStyle);
        }

        static string DoTextField(
            string entryValue,
            bool enabled,
            float textBoxLeft,
            float textBoxWidth)
        {
            GUI.enabled = enabled;

            var rect = GUILayoutUtility.GetRect(
                new GUIContent(entryValue),
                UnityStyles.Dialog.EntryLabel);
            rect.width = textBoxWidth;
            rect.x = textBoxLeft;

            string result = GUI.TextField(rect, entryValue);

            GUI.enabled = true;

            return result;
        }

        static bool DoButton(
            string text,
            bool isEnabled,
            float buttonLeft,
            float buttonWidth)
        {
            GUI.enabled = isEnabled;

            var rect = GUILayoutUtility.GetRect(
                new GUIContent(text),
                UnityStyles.Dialog.EntryLabel);

            rect.width = buttonWidth;
            rect.x = buttonLeft;

            bool result = GUI.Button(rect, text);
            GUI.enabled = true;
            return result;
        }

        static bool DoRadioButton(
            string text,
            bool isChecked,
            bool isEnabled,
            float buttonLeft)
        {
            GUI.enabled = isEnabled;

            GUIStyle radioButtonStyle =
                EditorStyles.radioButton;

            var rect = GUILayoutUtility.GetRect(
                new GUIContent(text),
                radioButtonStyle);

            rect.x = buttonLeft;

            bool result = GUI.Toggle(
                rect,
                isChecked,
                text,
                radioButtonStyle);

            GUI.enabled = true;

            return result;
        }

        static void DoWarningLabel(
            string labelText,
            float labelLeft)
        {
            Rect rect = GUILayoutUtility.GetRect(
                new GUIContent(labelText),
                EditorStyles.label);

            rect.x = labelLeft;

            GUI.Label(rect,
                new GUIContent(labelText, Images.GetWarnIcon()),
                UnityStyles.IncomingChangesTab.HeaderWarningLabel);
        }

        static bool DoLinkLabel(
            string labelText,
            string linkText,
            float labelLeft)
        {
            GUIContent labelContent = new GUIContent(labelText);
            GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel);
            labelStyle.richText = true;

            Rect rect = GUILayoutUtility.GetRect(
                labelContent,
                labelStyle);

            rect.x = labelLeft;

            Rect linkRect = GetLinkRect(
                labelText,
                linkText,
                labelContent,
                labelStyle,
                rect);

            EditorGUIUtility.AddCursorRect(linkRect, MouseCursor.Link);

            GUI.Label(rect, labelText, labelStyle);

            return Mouse.IsLeftMouseButtonPressed(Event.current)
                && linkRect.Contains(Event.current.mousePosition);
        }

        static Rect GetLinkRect(
            string labelText,
            string linkText,
            GUIContent labelContent,
            GUIStyle labelStyle,
            Rect rect)
        {
            int beginLinkChar = labelText.IndexOf(linkText);
            int endLinkChar = beginLinkChar + linkText.Length;

            Vector2 beginPos = labelStyle.GetCursorPixelPosition(
               rect, labelContent, beginLinkChar);
            Vector2 endPos = labelStyle.GetCursorPixelPosition(
               rect, labelContent, endLinkChar);

            Rect linkRect = new Rect(
                beginPos.x,
                beginPos.y,
                endPos.x - beginPos.x,
                labelStyle.lineHeight * 1.2f);

            return linkRect;
        }

        static bool IsValidState(
            CreateWorkspaceViewState state)
        {
            if (!ValidateRepositoryName(state.RepositoryName).IsValid)
                return false;

            if (!ValidateWorkspaceName(state.WorkspaceName).IsValid)
                return false;

            return true;
        }

        static ValidationResult ValidateRepositoryName(string repositoryName)
        {
            ValidationResult result = new ValidationResult();

            if (string.IsNullOrEmpty(repositoryName))
            {
                result.ErrorMessage = "Repository name cannot be empty";
                result.IsValid = false;
                return result;
            }

            result.IsValid = true;
            return result;
        }

        static ValidationResult ValidateWorkspaceName(string workspaceName)
        {
            ValidationResult result = new ValidationResult();

            if (string.IsNullOrEmpty(workspaceName))
            {
                result.ErrorMessage = "Workspace name cannot be empty";
                result.IsValid = false;
                return result;
            }

            result.IsValid = true;
            return result;
        }

        static RepositorySpec GetRepSpecFromName(string repositoryName)
        {
            if (string.IsNullOrEmpty(repositoryName))
                return null;

            return new SpecGenerator().
                GenRepositorySpec(false, repositoryName);
        }

        class ValidationResult
        {
            internal string ErrorMessage;
            internal bool IsValid;
        }

        const float LABEL_WIDTH = 150;
        const float TEXTBOX_WIDTH = 400;
        const float BROWSE_BUTTON_WIDTH = 25;
        const float NEW_BUTTON_WIDTH = 60;
        const float BUTTON_MARGIN = 2;
        const float LABEL_MARGIN = 2;
        const float RADIO_BUTTON_MARGIN = 38;
        const float CREATE_WORKSPACE_BUTTON_MARGIN = 32;
        const float CREATE_WORKSPACE_BUTTON_WIDTH = 160;

        const string HELP_URL = @"https://www.plasticscm.com/book/#_full_workspaces_vs_partial_workspaces";
    }
}

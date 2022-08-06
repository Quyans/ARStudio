using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using PlasticGui;

namespace Unity.PlasticSCM.Editor.UI
{
    [InitializeOnLoad]
    internal abstract class PlasticDialog : EditorWindow, IPlasticDialogCloser
    {
        protected virtual Rect DefaultRect
        {
            get
            {
                int pixelWidth = Screen.currentResolution.width;
                float x = (pixelWidth - DEFAULT_WIDTH) / 2;
                return new Rect(x, 200, DEFAULT_WIDTH, DEFAULT_HEIGHT);
            }
        }

        protected virtual bool IsResizable { get; set; }

        internal void OkButtonAction()
        {
            CompleteModal(ResponseType.Ok);
        }

        internal void CancelButtonAction()
        {
            CompleteModal(ResponseType.Cancel);
        }

        internal void CloseButtonAction()
        {
            CompleteModal(ResponseType.None);
        }

        internal void ApplyButtonAction()
        {
            CompleteModal(ResponseType.Apply);
        }

        internal ResponseType RunModal(EditorWindow parentWindow)
        {
            InitializeVars(parentWindow);

            if (!IsResizable)
                MakeNonResizable();

            if (UI.RunModal.IsAvailable())
            {
                UI.RunModal.Dialog(this);
                return mAnswer;
            }

            EditorUtility.DisplayDialog(
                "Plastic SCM",
                "The modal dialog is not available in this version and it is required for the Plastic SCM plugin works fine.\n" +
                "Please contact support@codicesoftware.com for further info.",
                PlasticLocalization.GetString(PlasticLocalization.Name.CloseButton));
            return ResponseType.None;
        }

        protected void OnGUI()
        {
            try
            {
                // If the Dialog has been saved into the Unity editor layout and persisted between restarts, the methods
                // to configure the dialogs will be skipped. Simple fix here is to close it when this state is detected.
                // Fixes a NPE loop when the state mentioned above is occuring. 
                if (!mIsConfigured)
                {
                    mIsClosed = true;
                    Close();
                    return;
                }

                if (Event.current.type == EventType.Layout)
                {
                    EditorDispatcher.Update();
                }

                if (!mFocusedOnce)
                {
                    // Somehow the prevents the dialog from jumping when dragged
                    // NOTE(rafa): We cannot do every frame because the modal kidnaps focus for all processes (in mac at least) 
                    Focus();
                    mFocusedOnce = true;
                }

                ProcessKeyActions();

                if (mIsClosed)
                    return;

                GUI.Box(new Rect(0, 0, position.width, position.height), GUIContent.none);

                float margin = 25;
                float marginTop = 25;
                using (new EditorGUILayout.HorizontalScope(GUILayout.Height(position.height)))
                {
                    GUILayout.Space(margin);
                    using (new EditorGUILayout.VerticalScope(GUILayout.Height(position.height)))
                    {
                        GUILayout.Space(marginTop);
                        OnModalGUI();
                        GUILayout.Space(margin);
                    }
                    GUILayout.Space(margin);
                }

                var lastRect = GUILayoutUtility.GetLastRect();
                float desiredHeight = lastRect.yMax;
                Rect newPos = position;
                newPos.height = desiredHeight;
                if (position.height < desiredHeight)
                    position = newPos;

                if (Event.current.type == EventType.Repaint)
                {
                    if (mIsCompleted)
                    {
                        mIsClosed = true;
                        Close();
                    }
                }
            }
            finally
            {
                if (mIsClosed)
                    EditorGUIUtility.ExitGUI();
            }
        }

        void OnDestroy()
        {
            if (!mIsConfigured)
                return;

            SaveSettings();
            mParentWindow.Focus();
        }

        protected virtual void SaveSettings() { }
        protected abstract void OnModalGUI();
        protected abstract string GetTitle();

        protected void Paragraph(string text)
        {
            Paragraph(text, DEFAULT_LINE_SPACING);
        }

        protected void Paragraph(string text, float spacing)
        {
            string[] lines = GetLines(text, UnityStyles.Paragraph);

            if (lines != null)
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        GUILayout.Label(lines[i], UnityStyles.Paragraph);
                        GUILayout.Space(spacing);
                    }
                    GUILayout.Space(DEFAULT_PARAGRAPH_SPACING);
                }
            }
        }

        protected bool TextBlockWithLink(
            string url, string formattedExplanation,
            GUIStyle textblockStyle)
        {
            string explanationColored = string.Format(
                formattedExplanation, url);

            var lines = GetLines(explanationColored, textblockStyle);

            if (lines == null)
                return false;

            Rect linkHitbox = Rect.zero;
            foreach (string rawLine in lines)
            {
                string line = ReplaceUrlWithRichTextUrl(rawLine, url);

                GUILayout.Label(line, textblockStyle);
                Rect lineRect = GUILayoutUtility.GetLastRect();
                GUILayout.Space(DEFAULT_LINE_SPACING);

                if (Event.current.type == EventType.Layout)
                    continue;

                int beginChar = line.IndexOf(url);
                if (beginChar == -1)
                    continue;

                int lastChar = beginChar + url.Length;
                GUIContent lineContent = new GUIContent(line);

                Vector2 beginPos = textblockStyle.GetCursorPixelPosition(
                    lineRect, lineContent, beginChar);
                Vector2 endPos = textblockStyle.GetCursorPixelPosition(
                    lineRect, lineContent, lastChar);

                linkHitbox = new Rect(
                    beginPos.x,
                    beginPos.y,
                    endPos.x - beginPos.x,
                    textblockStyle.lineHeight * 1.2f);
            }

            EditorGUIUtility.AddCursorRect(linkHitbox, MouseCursor.Link);

            return Mouse.IsLeftMouseButtonPressed(Event.current)
                && linkHitbox.Contains(Event.current.mousePosition);
        }

        protected static void Title(string text)
        {
            GUILayout.Label(text, UnityStyles.Dialog.Toggle);
        }

        protected static bool TitleToggle(string text, bool isOn)
        {
            return EditorGUILayout.ToggleLeft(text, isOn, UnityStyles.Dialog.Toggle);
        }

        protected static bool TitleToggle(string text, bool isOn, GUIStyle style)
        {
            return EditorGUILayout.ToggleLeft(text, isOn, style);
        }

        protected static string TextEntry(
            string label,
            string value,
            float width,
            float x)
        {
            return TextEntry(
                label,
                value,
                null,
                width,
                x);
        }

        protected static string TextEntry(
            string label, string value, string controlName, float width, float x)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EntryLabel(label);

                GUILayout.FlexibleSpace();

                var rt = GUILayoutUtility.GetRect(
                    new GUIContent(value), UnityStyles.Dialog.EntryLabel);
                rt.width = width;
                rt.x = x;

                if (!string.IsNullOrEmpty(controlName))
                    GUI.SetNextControlName(controlName);

                return GUI.TextField(rt, value);
            }
        }

        protected static string ComboBox(
            string label,
            string value,
            string controlName,
            List<string> dropDownOptions,
            GenericMenu.MenuFunction2 optionSelected,
            float width,
            float x)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EntryLabel(label);

                GUILayout.FlexibleSpace();

                var rt = GUILayoutUtility.GetRect(
                    new GUIContent(value), UnityStyles.Dialog.EntryLabel);
                rt.width = width;
                rt.x = x;

                return DropDownTextField.DoDropDownTextField(
                    value,
                    label,
                    dropDownOptions,
                    optionSelected,
                    rt);
            }
        }

        protected static string PasswordEntry(
            string label, string value, float width, float x)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EntryLabel(label);

                GUILayout.FlexibleSpace();

                var rt = GUILayoutUtility.GetRect(
                    new GUIContent(value), UnityStyles.Dialog.EntryLabel);
                rt.width = width;
                rt.x = x;

                return GUI.PasswordField(rt, value, '*');
            }
        }

        protected static bool ToggleEntry(
            string label, bool value, float width, float x)
        {
            var rt = GUILayoutUtility.GetRect(
                new GUIContent(label), UnityStyles.Dialog.EntryLabel);
            rt.width = width;
            rt.x = x;

            return GUI.Toggle(rt, value, label);
        }

        protected static bool NormalButton(string text)
        {
            return GUILayout.Button(
                text, UnityStyles.Dialog.NormalButton,
                GUILayout.MinWidth(80),
                GUILayout.Height(25));
        }

        void IPlasticDialogCloser.CloseDialog()
        {
            OkButtonAction();
        }

        void ProcessKeyActions()
        {
            Event e = Event.current;

            if (mEnterKeyAction != null &&
                Keyboard.IsReturnOrEnterKeyPressed(e))
            {
                mEnterKeyAction.Invoke();
                e.Use();
                return;
            }

            if (mEscapeKeyAction != null &&
                Keyboard.IsKeyPressed(e, KeyCode.Escape))
            {
                mEscapeKeyAction.Invoke();
                e.Use();
                return;
            }
        }

        protected static bool AcceptButton(string text)
        {
            GUI.color = new Color(0.098f, 0.502f, 0.965f, .8f);

            int textWidth = (int)((GUIStyle)UnityStyles.Dialog.AcceptButtonText)
                .CalcSize(new GUIContent(text)).x;

            bool pressed = GUILayout.Button(
                string.Empty, GetEditorSkin().button,
                GUILayout.MinWidth(Math.Max(80, textWidth + 10)),
                GUILayout.Height(25));

            GUI.color = Color.white;

            Rect buttonRect = GUILayoutUtility.GetLastRect();
            GUI.Label(buttonRect, text, UnityStyles.Dialog.AcceptButtonText);

            return pressed;
        }

        void CompleteModal(ResponseType answer)
        {
            mIsCompleted = true;
            mAnswer = answer;
        }

        string[] GetLines(string text, GUIStyle textblockStyle)
        {
            string[] lines;
            if (!mWrappedTextLines.TryGetValue(text, out lines))
            {
                var content = new GUIContent(text);

                var textRect = GUILayoutUtility.GetRect(content, textblockStyle);
                var lineHeight = textblockStyle.lineHeight;
                int lineCount = (int)(textRect.height / lineHeight);

                int[] lineEnds = new int[lineCount];
                lines = new string[lineCount];
                for (int i = 0; i < lineCount; i++)
                {
                    int endLineIdx = 0;
                    for (float x = 0; x < textRect.width; x += 6)
                    {
                        Vector2 hitTestPos = textRect.position + new Vector2(textRect.width - x, i * lineHeight + lineHeight / 2);
                        endLineIdx = textblockStyle.GetCursorStringIndex(
                            textRect,
                            content,
                            hitTestPos);

                        if (endLineIdx > 0)
                            break;
                    }

                    lineEnds[i] = endLineIdx;

                    if (i == lines.Length - 1 && lines.Length > 1)
                        lines[i] = BuildLine.ForIndex(text, lineEnds[i - 1] + 1);
                    else if (i > 0)
                        lines[i] = BuildLine.ForIndexAndLenght(text, lineEnds[i - 1] + 1, endLineIdx - lineEnds[i - 1] - 1);
                    else
                        lines[0] = BuildLine.ForIndexAndLenght(text, 0, Mathf.Min(text.Length, endLineIdx + 1)).Trim();
                }

                bool validLines = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                    {
                        validLines = true;
                        break;
                    }
                }

                if (validLines)
                {
                    mWrappedTextLines.Add(text, lines);
                    Repaint();
                    return null; // The frame they are generated they shouldn't be drawn, for layout error reasons
                }
            }
            else
                return lines;

            return null;
        }

        void InitializeVars(EditorWindow parentWindow)
        {
            mIsConfigured = true;
            mIsCompleted = false;
            mIsClosed = false;
            mAnswer = ResponseType.Cancel;

            titleContent = new GUIContent(GetTitle());

            mFocusedOnce = false;

            position = DefaultRect;
            mParentWindow = parentWindow;
        }

        void MakeNonResizable()
        {
            maxSize = DefaultRect.size;
            minSize = maxSize;
        }

        static void EntryLabel(string labelText)
        {
            GUIContent labelContent = new GUIContent(labelText);
            GUIStyle labelStyle = UnityStyles.Dialog.EntryLabel;

            Rect rt = GUILayoutUtility.GetRect(labelContent, labelStyle);

            GUI.Label(rt, labelText, EditorStyles.label);
        }

        static string ReplaceUrlWithRichTextUrl(string line, string url)
        {
            if (!line.Contains(url))
                return line;

            string coloredUrl = string.Format(
                "<color=\"{0}\">{1}</color>",
                UnityStyles.HexColors.LINK_COLOR,
                url);

            return line.Replace(url, coloredUrl);
        }

        static GUISkin GetEditorSkin()
        {
            return EditorGUIUtility.isProSkin ?
                EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene) :
                EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
        }

        bool mIsConfigured;
        bool mIsCompleted;
        bool mIsClosed;
        ResponseType mAnswer;

        protected Action mEnterKeyAction = null;
        protected Action mEscapeKeyAction = null;

        bool mFocusedOnce;

        Dictionary<string, string[]> mWrappedTextLines =
            new Dictionary<string, string[]>();

        EditorWindow mParentWindow;

        protected const float DEFAULT_LINE_SPACING = -5;
        const float DEFAULT_WIDTH = 500;
        const float DEFAULT_HEIGHT = 180;
        const float DEFAULT_PARAGRAPH_SPACING = 10f;

        static class BuildLine
        {
            internal static string ForIndex(string text, int index)
            {
                if (index < 0 || index > text.Length)
                    return string.Empty;

                return text.Substring(index).Trim();
            }

            internal static string ForIndexAndLenght(string text, int index, int lenght)
            {
                if (index < 0 || index > text.Length)
                    return string.Empty;

                return text.Substring(index, lenght);
            }
        }
    }
}

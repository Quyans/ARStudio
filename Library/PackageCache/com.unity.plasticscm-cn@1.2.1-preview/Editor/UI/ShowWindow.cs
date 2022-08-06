using System;

using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class ShowWindow
    {
        internal static PlasticWindow Plastic(bool focus = true)
        {
            PlasticWindow window = EditorWindow.GetWindow<PlasticWindow>(
                UnityConstants.PLASTIC_WINDOW_TITLE,
                focus,
                mConsoleWindowType,
                mProjectBrowserType);

            SetIcon(
                window,
                Images.GetImage(Images.Name.IconPlasticView));

            return window;
        }

        static void SetIcon(EditorWindow window, Texture2D icon)
        {
            window.titleContent = new GUIContent(
                window.titleContent.text,
                icon);
        }

        static Type mConsoleWindowType = typeof(EditorWindow).
            Assembly.GetType("UnityEditor.ConsoleWindow");
        static Type mProjectBrowserType = typeof(EditorWindow).
            Assembly.GetType("UnityEditor.ProjectBrowser");
    }
}

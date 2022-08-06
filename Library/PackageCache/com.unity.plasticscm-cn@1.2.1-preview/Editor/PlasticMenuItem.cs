using UnityEditor;

using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor
{
    class PlasticMenuItem
    {
        [MenuItem(MENU_ITEM_NAME, false)]
        internal static void ShowPanel()
        {
            ShowWindow.Plastic();
        }
        
        [MenuItem(MENU_ITEM_NAME, true)]
        static bool ValidateMenu()
        {
            return !CollabPlugin.IsEnabled();
        }

        const string MENU_ITEM_NAME = "Window/" + UnityConstants.PLASTIC_WINDOW_TITLE;
    }
}

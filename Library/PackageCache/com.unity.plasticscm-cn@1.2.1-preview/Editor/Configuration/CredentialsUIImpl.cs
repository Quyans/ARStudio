using UnityEditor;

using Codice.Client.Common;
using PlasticGui;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Configuration
{
    internal class CredentialsUiImpl : ICredentialsUI
    {
        internal CredentialsUiImpl(EditorWindow parentWindow)
        {
            mParentWindow = parentWindow;
        }

        CredentialsDialogData ICredentialsUI.AskUserForCredentials(string servername)
        {
            CredentialsDialogData result = null;

            GUIActionRunner.RunGUIAction(delegate
            {
                result = CredentialsDialog.RequestCredentials(
                    servername, mParentWindow);
            });

            return result;
        }

        void ICredentialsUI.ShowSaveProfileErrorMessage(string message)
        {
            GUIActionRunner.RunGUIAction(delegate
            {
                GuiMessage.ShowError(string.Format(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.CredentialsErrorSavingProfile),
                    message));
            });
        }

        EditorWindow mParentWindow;
    }
}

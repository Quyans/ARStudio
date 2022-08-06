using Unity.PlasticSCM.Editor.Views.IncomingChanges;
using Unity.PlasticSCM.Editor.Views.PendingChanges;

namespace Unity.PlasticSCM.Editor
{
    internal static class AutoRefresh
    {
        internal static void PendingChangesView(PendingChangesTab pendingChangesTab)
        {
            if (DisableFsWatcher.MustDisableFsWatcher())
                return;

            if (pendingChangesTab == null)
                return;

            pendingChangesTab.AutoRefresh();
        }

        internal static void IncomingChangesView(IIncomingChangesTab incomingChangesTab)
        {
            if (DisableFsWatcher.MustDisableFsWatcher())
                return;

            if (incomingChangesTab == null)
                return;

            if (!incomingChangesTab.IsVisible)
                return;

            incomingChangesTab.AutoRefresh();
        }
    }
}

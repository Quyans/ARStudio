using UnityEditor;

using Codice.Utils;

namespace Unity.PlasticSCM.Editor
{
    internal static class DisableFsWatcher
    {
        internal static bool MustDisableFsWatcher()
        {
            return PlatformIdentifier.IsWindows() &&
                   MonoRuntime.IsRunningUnder35RuntimeOrOlder();
        }
    }
}

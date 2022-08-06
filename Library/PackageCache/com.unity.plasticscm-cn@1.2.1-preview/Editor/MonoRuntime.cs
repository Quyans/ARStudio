using UnityEditor;

namespace Unity.PlasticSCM.Editor
{
    internal static class MonoRuntime
    {
        internal static bool IsRunningUnder35RuntimeOrOlder()
        {
#if !UNITY_2019_2_OR_NEWER
            return PlayerSettings.scriptingRuntimeVersion ==
                ScriptingRuntimeVersion.Legacy;
#else
            return false;
#endif
        }
    }
}

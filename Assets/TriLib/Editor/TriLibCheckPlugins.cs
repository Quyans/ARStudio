using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
#if UNITY_EDITOR_OSX && UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif
using TriLib;
using System;
using System.IO;
[InitializeOnLoad]
public class TriLibCheckPlugins
{
    public const string DebugSymbol = "TRILIB_OUTPUT_MESSAGES";
    public const string UnityTextureLoaderSymbol = "TRILIB_USE_UNITY_TEXTURE_LOADER";
    public const string ZipSymbol = "TRILIB_USE_ZIP";
    public const string DebugEnabledMenuPath = "TriLib/Enable Debug";
    public const string UnityTextureLoaderMenuPath = "TriLib/Use Unity default texture loader";
    public const string ZipEnabledMenuPath = "TriLib/Enable Zip loading";
#if UNITY_EDITOR_OSX && UNITY_IOS
	public const string IOSSimulatorSymbol = "USE_IOS_SIMULATOR";
	public const string IOSEnableFileSharingSymbol = "USE_IOS_FILES_SHARING";
	public const string IOSSimulatorEnabledMenuPath = "TriLib/iOS Simulator Enabled";
	public const string IOSEnableFileSharingMenuPath = "TriLib/iOS File Sharing Enabled";
	public const string XCodeProjectPath = "Libraries/TriLib/TriLib/Plugins/iOS";
#endif
    public static bool PluginsLoaded { get; private set; }

    static TriLibCheckPlugins()
    {
        try
        {
            AssimpInterop.ai_IsExtensionSupported(".3ds");
            PluginsLoaded = true;
        }
        catch (Exception exception)
        {
            if (exception is DllNotFoundException)
            {
                if (EditorUtility.DisplayDialog("TriLib plugins not found", "TriLib was unable to find the native plugins.\n\nIf you just imported the package, you will have to restart Unity editor.\n\nIf you click \"Ask to save changes and restart\", you will be prompted to save your changes (if there is any) then Unity editor will restart.\n\nOtherwise, you will have to save your changes and restart Unity editor manually.", "Ask to save changes and restart", "I will do it manually"))
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    var projectPath = Directory.GetParent(Application.dataPath);
                    EditorApplication.OpenProject(projectPath.FullName);
                }
            }
        }
    }

    [MenuItem(DebugEnabledMenuPath)]
    public static void DebugEnabled()
    {
        GenerateSymbolsAndUpdateMenu(DebugEnabledMenuPath, DebugSymbol, true);
    }

    [MenuItem(DebugEnabledMenuPath, true)]
    public static bool DebugEnabledValidate()
    {
        GenerateSymbolsAndUpdateMenu(DebugEnabledMenuPath, DebugSymbol, false);
        return true;
    }

    [MenuItem(UnityTextureLoaderMenuPath)]
    public static void UnityTextureLoaderEnabled()
    {
        GenerateSymbolsAndUpdateMenu(UnityTextureLoaderMenuPath, UnityTextureLoaderSymbol, true);
    }

    [MenuItem(UnityTextureLoaderMenuPath, true)]
    public static bool UnityTextureLoaderEnabledValidate()
    {
        GenerateSymbolsAndUpdateMenu(UnityTextureLoaderMenuPath, UnityTextureLoaderSymbol, false);
        return true;
    }

    [MenuItem(ZipEnabledMenuPath)]
    public static void ZipEnabled()
    {
        GenerateSymbolsAndUpdateMenu(ZipEnabledMenuPath, ZipSymbol, true);
    }

    [MenuItem(ZipEnabledMenuPath, true)]
    public static bool ZipEnabledValidate()
    {
        GenerateSymbolsAndUpdateMenu(ZipEnabledMenuPath, ZipSymbol, false);
        return true;
    }

    public static void GenerateSymbolsAndUpdateMenu(string menuPath, string checkingDefineSymbol, bool generateSymbols, bool forceDefinition = false)
    {
        var isDefined = false;
        var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        var defineSymbolsArray = defineSymbols.Split(';');
        var newDefineSymbols = generateSymbols ? string.Empty : null;
        foreach (var defineSymbol in defineSymbolsArray)
        {
            var trimmedDefineSymbol = defineSymbol.Trim();
            if (trimmedDefineSymbol == checkingDefineSymbol)
            {
                isDefined = true;
                if (!generateSymbols)
                {
                    break;
                }
                continue;
            }
            if (generateSymbols)
            {
                newDefineSymbols += string.Format("{0};", trimmedDefineSymbol);
            }
        }
        if (generateSymbols)
        {
            if (!isDefined || forceDefinition)
            {
                newDefineSymbols += string.Format("{0};", checkingDefineSymbol);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefineSymbols);
        }
        Menu.SetChecked(menuPath, generateSymbols ? !isDefined : isDefined);
    }

#if UNITY_EDITOR_OSX && UNITY_IOS
	[MenuItem (IOSSimulatorEnabledMenuPath)]
	public static void IOSSimulatorEnabled ()
	{
		GenerateSymbolsAndUpdateMenu (IOSSimulatorEnabledMenuPath, IOSSimulatorSymbol, true);
	}

	[MenuItem (IOSSimulatorEnabledMenuPath, true)]
	public static bool IOSSimulatorEnabledValidate ()
	{
		GenerateSymbolsAndUpdateMenu (IOSSimulatorEnabledMenuPath, IOSSimulatorSymbol, false);
		return true;
	}

	[MenuItem (IOSEnableFileSharingMenuPath)]
	public static void IOSEnableFileSharingEnabled ()
	{
		GenerateSymbolsAndUpdateMenu (IOSEnableFileSharingMenuPath, IOSEnableFileSharingSymbol, true);
	}

	[MenuItem (IOSEnableFileSharingMenuPath, true)]
	public static bool IOSEnableFileSharingValidate ()
	{
		GenerateSymbolsAndUpdateMenu (IOSEnableFileSharingMenuPath, IOSEnableFileSharingSymbol, false);
		return true;
	}

	[PostProcessBuildAttribute (1000)]
	public static void OnPreProcessBuild (BuildTarget target, string pathToBuiltProject)
	{
		if (target == BuildTarget.iOS) {
			var path = PBXProject.GetPBXProjectPath (pathToBuiltProject);
			var pbxProject = new PBXProject ();
			pbxProject.ReadFromFile (path);
			var targetGuid = pbxProject.TargetGuidByName (PBXProject.GetUnityTargetName ());
#if USE_IOS_SIMULATOR
    		RemoveFileFromProject(pbxProject, targetGuid, "libassimp.release.a");
			RemoveFileFromProject(pbxProject, targetGuid, "libirrxml.release.a");
			RemoveFileFromProject (pbxProject, targetGuid, "libminizip.release.a");
			RemoveFileFromProject(pbxProject, targetGuid, "libstb_image.release.a");
#else
			RemoveFileFromProject (pbxProject, targetGuid, "libassimp.debug.a");
			RemoveFileFromProject (pbxProject, targetGuid, "libirrxml.debug.a");
			RemoveFileFromProject (pbxProject, targetGuid, "libminizip.debug.a");
			RemoveFileFromProject(pbxProject, targetGuid, "libstb_image.debug.a");
			pbxProject.SetBuildProperty (targetGuid, "ENABLE_BITCODE", "NO");
#endif
			pbxProject.AddFrameworkToProject (targetGuid, "libz.dylib", true);
			pbxProject.AddFrameworkToProject (targetGuid, "libz.tbd", true);
			pbxProject.WriteToFile (path);
#if USE_IOS_FILES_SHARING
			var plistPath = pathToBuiltProject + "/info.plist";
			var plist = new PlistDocument ();
			plist.ReadFromFile (plistPath);
			var dict = plist.root.AsDict ();
			dict.SetBoolean ("UIFileSharingEnabled", true);
			plist.WriteToFile (plistPath);
#endif
		}
	}

	private static void RemoveFileFromProject (PBXProject pbxProject, string targetGuid, string filename)
	{
		var path = Path.Combine (XCodeProjectPath, filename);
		var fileGuid = pbxProject.FindFileGuidByProjectPath (path);
		if (fileGuid != null) {
			pbxProject.RemoveFileFromBuild (targetGuid, fileGuid);
			pbxProject.RemoveFile (fileGuid);
		}
	}
#endif
}

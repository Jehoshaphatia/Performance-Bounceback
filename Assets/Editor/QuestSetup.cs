using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

public static class QuestSetup
{
    public static void Run()
    {
        ConfigureXr();
        ConfigurePlayer();
        AssetDatabase.SaveAssets();
        Log("complete");
    }

    static void ConfigureXr()
    {
        if (!EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey,
                out XRGeneralSettingsPerBuildTarget perTarget) || perTarget == null)
        {
            perTarget = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
            Directory.CreateDirectory("Assets/XR");
            AssetDatabase.CreateAsset(perTarget, "Assets/XR/XRGeneralSettingsPerBuildTarget.asset");
            EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey, perTarget, true);
            Log("created XRGeneralSettingsPerBuildTarget");
        }

        perTarget.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Android);
        var settings = perTarget.SettingsForBuildTarget(BuildTargetGroup.Android);
        if (settings == null || settings.Manager == null) { Log("ERROR: no XRManagerSettings for Android"); return; }

        foreach (var name in new[] { "UnityEngine.XR.OpenXR.OpenXRLoader", "OpenXRLoader" })
        {
            if (settings.Manager.activeLoaders.Any(l => l != null && l.GetType().Name == "OpenXRLoader"))
            { Log("OpenXR loader already active"); break; }
            Log($"AssignLoader('{name}') -> {XRPackageMetadataStore.AssignLoader(settings.Manager, name, BuildTargetGroup.Android)}");
        }
        Log("active loaders: " + string.Join(", ", settings.Manager.activeLoaders.Select(l => l == null ? "null" : l.GetType().Name)));

        var oxr = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
        if (oxr == null) { Log("ERROR: no OpenXRSettings for Android"); return; }

        foreach (var f in oxr.GetFeatures<OpenXRFeature>())
        {
            var t = f.GetType().Name;
            // Quest device support + the Touch controller interaction profile.
            if (t.Contains("MetaQuest") || t.Contains("OculusTouchController"))
            {
                f.enabled = true;
                EditorUtility.SetDirty(f);
                Log($"enabled feature: {t}");
            }
        }
        Log("enabled features now: " + string.Join(", ",
            oxr.GetFeatures<OpenXRFeature>().Where(f => f.enabled).Select(f => f.GetType().Name)));
        EditorUtility.SetDirty(oxr);
    }

    static void ConfigurePlayer()
    {
        var android = NamedBuildTarget.Android;
        PlayerSettings.SetScriptingBackend(android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel32;
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan });
        PlayerSettings.colorSpace = ColorSpace.Linear;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.SetApplicationIdentifier(android, "com.jehoshaphatia.performancebounceback");
        PlayerSettings.productName = "Performance Bounceback";

        Log($"scripting={PlayerSettings.GetScriptingBackend(android)} arch={PlayerSettings.Android.targetArchitectures} " +
            $"minSdk={PlayerSettings.Android.minSdkVersion} gfx={string.Join(",", PlayerSettings.GetGraphicsAPIs(BuildTarget.Android))} " +
            $"colorSpace={PlayerSettings.colorSpace} id={PlayerSettings.GetApplicationIdentifier(android)}");
    }

    static void Log(string m) => Debug.Log($"QUESTSETUP: {m}");
}

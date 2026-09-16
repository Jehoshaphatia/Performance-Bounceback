using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildQuest
{
    // Deliberately NOT "Build/": Unity's Android Gradle step writes its own
    // lowercase build/ folder at the project root, which collides with the
    // `build` branch on a case-insensitive volume. Keeping the APK out of it
    // means that folder holds only disposable logs and is safe to delete.
    const string OutputPath = "Builds/PerformanceBounceback.apk";

    public static void Run()
    {
        var opts = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/PerformanceBounceback/Scenes/Scene0.unity" },
            locationPathName = OutputPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.Development,
        };

        var report = BuildPipeline.BuildPlayer(opts);
        var s = report.summary;
        Debug.Log($"BUILD: result={s.result} errors={s.totalErrors} warnings={s.totalWarnings} " +
                  $"size={s.totalSize / (1024 * 1024)}MB time={s.totalTime}");

        foreach (var m in report.steps.SelectMany(st => st.messages)
                     .Where(m => m.type == LogType.Error || m.type == LogType.Exception).Take(15))
            Debug.Log($"BUILD_ERR: {m.content}");
    }
}

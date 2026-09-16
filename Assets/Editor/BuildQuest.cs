using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildQuest
{
    // Deliberately NOT "Build/": Unity's Android Gradle step owns that folder
    // and rewrites it on every build. Keeping the artifact out of it means
    // build/ holds only disposable logs and can be cleared at any time.
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

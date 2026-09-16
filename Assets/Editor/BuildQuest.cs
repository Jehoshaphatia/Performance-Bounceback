using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildQuest
{
    public static void Run()
    {
        var scenes = new[] { "Assets/PerformanceBounceback/Scenes/Scene0.unity" };
        var opts = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "Build/PerformanceBounceback.apk",
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.Development,
        };

        var report = BuildPipeline.BuildPlayer(opts);
        var s = report.summary;
        Debug.Log($"BUILD: result={s.result} errors={s.totalErrors} warnings={s.totalWarnings} " +
                  $"size={s.totalSize / (1024 * 1024)}MB time={s.totalTime}");

        foreach (var step in report.steps.SelectMany(st => st.messages)
                     .Where(m => m.type == LogType.Error || m.type == LogType.Exception).Take(15))
            Debug.Log($"BUILD_ERR: {step.content}");
    }
}

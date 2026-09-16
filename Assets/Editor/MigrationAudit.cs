using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MigrationAudit
{
    const string ScenePath = "Assets/PerformanceBounceback/Scenes/Scene0.unity";

    public static void Run()
    {
        var sb = new StringBuilder();
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        sb.AppendLine($"SCENE {scene.name}: loaded={scene.isLoaded} roots={scene.rootCount}");

        sb.AppendLine("\n--- ROOT OBJECTS ---");
        foreach (var root in scene.GetRootGameObjects())
            sb.AppendLine($"  {root.name}  (children={root.transform.childCount}, static={root.isStatic}, active={root.activeSelf})");

        int missing = 0;
        var tally = new Dictionary<string, int>();
        var missingPaths = new List<string>();

        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            foreach (var c in go.GetComponents<Component>())
            {
                if (c == null) { missing++; if (missingPaths.Count < 25) missingPaths.Add(Path(go)); }
                else { var n = c.GetType().Name; tally[n] = tally.TryGetValue(n, out var v) ? v + 1 : 1; }
            }
        }

        sb.AppendLine($"\n--- MISSING SCRIPTS: {missing} ---");
        foreach (var p in missingPaths) sb.AppendLine($"  {p}");

        sb.AppendLine("\n--- COMPONENT TALLY ---");
        foreach (var kv in tally.OrderByDescending(k => k.Value).Take(30))
            sb.AppendLine($"  {kv.Value,6}  {kv.Key}");

        sb.AppendLine("\n--- BALL PREFAB ---");
        var ball = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PerformanceBounceback/Prefabs/Ball.prefab");
        if (ball == null) sb.AppendLine("  <failed to load>");
        else foreach (var c in ball.GetComponents<Component>())
            sb.AppendLine($"  {(c == null ? "<MISSING SCRIPT>" : c.GetType().Name)}  tag={ball.tag}");

        System.IO.File.WriteAllText("migration-audit.txt", sb.ToString());
        Debug.Log(sb.ToString());
    }

    static string Path(GameObject go)
    {
        var p = go.name; var t = go.transform.parent;
        while (t != null) { p = t.name + "/" + p; t = t.parent; }
        return p;
    }
}

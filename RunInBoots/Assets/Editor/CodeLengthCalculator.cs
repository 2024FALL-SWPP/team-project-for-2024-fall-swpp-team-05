using UnityEditor;
using UnityEngine;
using System.IO;

public class CodeLengthCalculator : EditorWindow
{
    [MenuItem("Tools/Calculate Code Length")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CodeLengthCalculator));
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Calculate Code Length"))
        {
            string[] scripts = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
            int totalLines = 0;
            string toLog = "";
            foreach (var script in scripts)
            {
                string[] lines = File.ReadAllLines(script);
                totalLines += lines.Length;
                toLog += $"{Path.GetFileName(script)}: {lines.Length} lines\n";
            }
            Debug.Log(toLog);
            Debug.Log($"Total Lines of Code: {totalLines}");
        }
    }
}

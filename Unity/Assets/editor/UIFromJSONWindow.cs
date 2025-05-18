using UnityEditor;
using UnityEngine;
using System.IO;

public class UIFromJSONWindow : EditorWindow
{
    private TextAsset jsonFile;

    [MenuItem("Tools/UI Generator From JSON")]
    public static void ShowWindow()
    {
        GetWindow<UIFromJSONWindow>("UI JSON Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("Select a UI JSON file", EditorStyles.boldLabel);
        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);

        if (jsonFile != null && GUILayout.Button("Generate UI"))
        {
            UILayoutData layout = JsonUtility.FromJson<UILayoutData>(jsonFile.text);
            UIFromJSONBuilder.BuildUI(layout);
        }
    }
}

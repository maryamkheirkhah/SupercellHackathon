using UnityEditor;
using UnityEngine;

public class UIFromPromptWindow : EditorWindow
{
    private string prompt = "Create a main menu with buttons: Start, Settings, Quit";

    //[MenuItem("Tools/UI Builder")]
    public static void ShowWindow()
    {
        GetWindow<UIFromPromptWindow>("UI Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("UI Prompt", EditorStyles.boldLabel);
        prompt = EditorGUILayout.TextField("Prompt", prompt);

        if (GUILayout.Button("Generate UI"))
        {
            UIBuilder.BuildUIFromPrompt(prompt);
        }
    }
}

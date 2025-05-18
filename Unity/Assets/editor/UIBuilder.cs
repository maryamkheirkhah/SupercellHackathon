using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public static class UIBuilder
{
    public static void BuildUIFromPrompt(string prompt)
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("⚠️ Use the UI Builder outside of Play Mode.");
            return;
        }

        // 1. Extract number of buttons and labels
        string[] buttonLabels = ExtractButtonLabels(prompt);

        // 2. Create a top-level Canvas (once)
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (!canvas)
        {
            GameObject canvasGO = new GameObject("GeneratedCanvas");
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");

            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // 3. Create a NEW root panel each time
        string containerName = "MainMenu_" + System.DateTime.Now.ToString("HHmmss");
        GameObject panelGO = new GameObject(containerName, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(panelGO, "Create Panel");
        panelGO.transform.SetParent(canvas.transform, false);

        var panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.sizeDelta = new Vector2(400, 600);
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.anchoredPosition = Vector2.zero;

        // 4. Add layout for stacking
        var layout = panelGO.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 20;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;

        var fitter = panelGO.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 5. Optional header
        if (prompt.ToLower().Contains("main menu"))
        {
            CreateHeader(panelGO.transform, "Main Menu");
        }

        // 6. Create buttons (default to 1)
        if (buttonLabels.Length == 0)
            buttonLabels = new[] { "Button" };

        foreach (string label in buttonLabels)
        {
            CreateButton(panelGO.transform, label.Trim());
        }

        UnityEngine.Debug.Log($"✅ UI Generated: {containerName}");
    }

    private static string[] ExtractButtonLabels(string prompt)
    {
        Match match = Regex.Match(prompt, @"buttons?:?\s*(.+)", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Groups[1].Value.Split(',');
        }

        return new string[0];
    }

    private static void CreateButton(Transform parent, string label)
    {
        GameObject buttonGO = new GameObject(label + "Button");
        Undo.RegisterCreatedObjectUndo(buttonGO, "Create Button");

        var rt = buttonGO.AddComponent<RectTransform>();
        buttonGO.AddComponent<UnityEngine.UI.Image>();
        var btn = buttonGO.AddComponent<Button>();
        buttonGO.transform.SetParent(parent, false);

        rt.sizeDelta = new Vector2(200, 50);

        GameObject textGO = new GameObject("Text", typeof(UnityEngine.UI.Text));
        textGO.transform.SetParent(buttonGO.transform, false);

        var text = textGO.GetComponent<UnityEngine.UI.Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 18;
        text.alignment = TextAnchor.MiddleCenter;

        var textRT = text.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
    }

    private static void CreateHeader(Transform parent, string content)
    {
        GameObject headerGO = new GameObject("Header", typeof(UnityEngine.UI.Text));
        Undo.RegisterCreatedObjectUndo(headerGO, "Create Header");

        headerGO.transform.SetParent(parent, false);
        var text = headerGO.GetComponent<UnityEngine.UI.Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 28;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;

        var rt = headerGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 80);
    }
}
    
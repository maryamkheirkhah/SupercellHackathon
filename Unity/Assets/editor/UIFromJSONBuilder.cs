
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UIFromJSONBuilder
{
    public static void BuildUI(UILayoutData layout)
    {
        // ───────────────── Canvas ─────────────────
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (!canvas)
        {
            GameObject canvasGO = new("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // ───────────────── Panel (green) ─────────────────
        GameObject panelGO = new("InventoryPanel", typeof(RectTransform), typeof(Image));
        panelGO.transform.SetParent(canvas.transform, false);

        RectTransform panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.sizeDelta = new Vector2(1200, 900);
        panelRT.anchorMin =
        panelRT.anchorMax =
        panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.anchoredPosition = Vector2.zero;

        Image panelBG = panelGO.GetComponent<Image>();
        panelBG.color = Color.green;

        // ───────────────── Title ─────────────────
        GameObject titleGO = new("InventoryTitle", typeof(RectTransform), typeof(Text));
        titleGO.transform.SetParent(panelGO.transform, false);

        RectTransform titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = titleRT.anchorMax = new Vector2(0.5f, 1f);
        titleRT.pivot = new Vector2(0.5f, 1f);
        titleRT.anchoredPosition = new Vector2(0, -40);
        titleRT.sizeDelta = new Vector2(500, 60);

        Text title = titleGO.GetComponent<Text>();
        title.text = layout?.title ?? "Inventory";
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        title.fontSize = 34;
        title.alignment = TextAnchor.MiddleCenter;
        title.color = Color.white;

        // ───────────────── Grid background (blue 50 %) ─────────────────
        GameObject gridGO = new("InventoryGrid", typeof(RectTransform), typeof(Image));
        gridGO.transform.SetParent(panelGO.transform, false);

        RectTransform gridRT = gridGO.GetComponent<RectTransform>();
        gridRT.sizeDelta = new Vector2(1000, 700);
        gridRT.anchorMin =
        gridRT.anchorMax =
        gridRT.pivot = new Vector2(0.5f, 0.5f);
        gridRT.anchoredPosition = new Vector2(0, -60);

        Image gridBG = gridGO.GetComponent<Image>();
        gridBG.color = new Color(0f, 0f, 1f, 0.5f); // semi-transparent blue

        // ───────────────── Manual slot placement ─────────────────
        int cols = 4;   
        int rows = 2;   
        Vector2 cell = new(90, 90);
        float spacing = 10f;

        float gridW = cols * cell.x + (cols - 1) * spacing;
        float gridH = rows * cell.y + (rows - 1) * spacing;

        Vector2 origin = new(-gridW * .5f + cell.x * .5f,
                              gridH * .5f - cell.y * .5f);

        // default sprite that ships with every Unity install
        Sprite slotSprite = Resources.GetBuiltinResource<Sprite>("UISprite.psd");

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                Vector2 pos = origin + new Vector2(
                    c * (cell.x + spacing),
                   -r * (cell.y + spacing));

                GameObject slotGO = new($"Slot_{r}_{c}",
                                        typeof(RectTransform),
                                        typeof(Image));
                slotGO.transform.SetParent(gridGO.transform, false);

                RectTransform rt = slotGO.GetComponent<RectTransform>();
                rt.sizeDelta = cell;
                rt.anchorMin =
                rt.anchorMax =
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = pos;

                Image img = slotGO.GetComponent<Image>();
                img.sprite = slotSprite;
                img.type = Image.Type.Sliced;
                img.color = Color.red;

                var outline = slotGO.AddComponent<Outline>();
                outline.effectColor = Color.white;
                outline.effectDistance = new Vector2(1, -1);
            }

        Debug.Log("✅ Inventory generated (green panel, blue grid, red slots).");
    }
}

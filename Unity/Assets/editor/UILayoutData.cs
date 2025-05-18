using UnityEngine;

[System.Serializable]
public class UIGrid
{
    public int columns;
    public int rows;
    public Vector2 cell_size;
}

[System.Serializable]
public class UISlot
{
    public string id;
}

[System.Serializable]
public class UILayoutData
{
    public string type;       // e.g., "inventory", "main_menu"
    public string title;      // optional header
    public UIGrid grid;       // inventory layout info
    public UISlot[] slots;    // item slots
}

# SupercellHackathon

# Unity Prompt-to-UI Generator

A Unity Editor tool that allows you to **generate user interfaces from JSON files** — designed to be compatible with LLM (Large Language Model) output. This project enables you to go from:

**Prompt → JSON → Functional UI Layout**

All without manually placing elements in Unity's Editor.

---

## What It Does

With this tool, a designer or developer can:

1. Prompt an LLM:  
   _“Create a Diablo-style inventory with 5 rows and 5 columns.”_

2. Receive JSON output that describes the UI layout.

3. Load the JSON file into Unity via the tool.

4. Automatically generate a **visible**, **editable**, and **correctly structured UI** in the Scene.

---

## Features

- **JSON-based UI generation**  
  Describe UIs using structured text instead of manual placement.

- **Manual RectTransform layout**  
  No Unity layout groups — every element is placed with precision and clarity.

- **LLM compatibility**  
  Designed for integration with ChatGPT, Claude, or custom prompt pipelines.

- **Inventory grid support**  
  Automatically create slot-based layouts from JSON.

- **Fully visible elements**  
  Each slot is forced into the visible layer with proper styling and hierarchy.

- **Expandable JSON structure**  
  Add buttons, sliders, HUDs, shops, menus, and more.

---

## Use Cases

This tool can power:

- **Rapid UI prototyping**  
  Perfect for small teams and hackathons where time is limited.

- **LLM-driven UI generation**  
  Automate layout creation via AI.

- **Modding tools**  
  Let players or designers define UI layouts via external files.

- **Dynamic runtime UIs**  
  Load UI from saved configs, backend instructions, or user preferences.

- **In-game editors**  
  Combine this system with in-game scripting to allow player-created menus or HUDs.

---


---

## How to Use

1. Write or generate a JSON file that describes your UI layout.

2. In Unity, open the **UI Generator From JSON** tool.

3. Select your JSON file (as a `TextAsset`).

4. Click **Generate UI**.

The tool will create:
- A canvas (if none exists)
- A background panel
- A title label
- A grid area
- Individual slots as UI `Image` objects

All objects will be visible, manually placed, and editable in the scene hierarchy.

---

## Example JSON

```json
{
"type": "inventory",
"title": "Backpack",
"grid": {
 "columns": 5,
 "rows": 5,
 "cell_size": [100, 100]
},
"slots": [
 { "id": "slot_0_0" }, { "id": "slot_0_1" }, { "id": "slot_0_2" }, { "id": "slot_0_3" }, { "id": "slot_0_4" },
 { "id": "slot_1_0" }, { "id": "slot_1_1" }, { "id": "slot_1_2" }, { "id": "slot_1_3" }, { "id": "slot_1_4" },
 { "id": "slot_2_0" }, { "id": "slot_2_1" }, { "id": "slot_2_2" }, { "id": "slot_2_3" }, { "id": "slot_2_4" },
 { "id": "slot_3_0" }, { "id": "slot_3_1" }, { "id": "slot_3_2" }, { "id": "slot_3_3" }, { "id": "slot_3_4" },
 { "id": "slot_4_0" }, { "id": "slot_4_1" }, { "id": "slot_4_2" }, { "id": "slot_4_3" }, { "id": "slot_4_4" }
]
}

```

<img width="844" alt="Screenshot 2025-05-18 at 9 30 58" src="https://github.com/user-attachments/assets/0730bf7c-529b-42b1-8bc3-e119059ef625" />


## Roadmap
 Add main_menu, HUD, shop support

 Support Text, Button, Icon elements

 Save generated layout as Unity prefab

 Enable runtime JSON loading (e.g., from a server)

 Integrate drag-and-drop between slots

 Add hover/tooltip support

 Theming support (dark mode, sci-fi, retro)

## Credits

Created by An Phan, Thanh Bui, Cephas Ofori, Maryam Kheirkhah, Oskar Valkamo 
Built during a hackathon hosted by Supercell



## Installation

1. Clone or download this repository into your Unity project (e.g. into `Assets/Editor/UIBuilder/`).

2. Ensure you're using **Unity 2023.x or later**.

3. Open Unity. In the top menu bar, go to:


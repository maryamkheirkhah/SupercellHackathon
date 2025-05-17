# main.py
import os
from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, Field, ValidationError
from openai import OpenAI, OpenAIError
from dotenv import load_dotenv
load_dotenv()

# ──────────────────────────────────────
# FastAPI bootstrap + CORS
# ──────────────────────────────────────
app = FastAPI(title="Smart-UI Generator")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Wide-open for hackathon ease
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ──────────────────────────────────────
# 1. Pydantic data-models (JSON contract)
# ──────────────────────────────────────

class GridData(BaseModel):
    rows: int
    columns: int
    cell_size: tuple[int, int]
    spacing: int
    bg_color: str

class Element(BaseModel):
    id: str | None = None                       # optional unique key
    type: str                                   # ItemGrid, EquipSlot, …
    position: str                               # center, top_left …
    rows: int | None = Field(None, ge=1, le=20)
    columns: int | None = Field(None, ge=1, le=20)
    equipType: str | None = None                # inventory-specific
    categoryFilters: list[str] | None = None
    sortable: bool | None = None
    maxWeight: int | None = None
    showText: bool | None = None
    label: str | None = None
    title: str | None = None

class LayoutJSON(BaseModel):
    # new fields
    prompt: str
    layout: str
    title: str
    panel_color: str
    grid: GridData
    slot_color: str

    # leave these as optional so older layouts still pass
    style: str | None = None
    meta: dict | None = None
    elements: list[Element] | None = None    # ← NOW OPTIONAL

class PromptIn(BaseModel):
    prompt: str

# ──────────────────────────────────────
# 2.  OpenAI / GPT-4(o) client
# ──────────────────────────────────────
client = OpenAI(api_key=os.getenv("OPENAI_API_KEY"))
SYSTEM_PROMPT = """
★
You are Smart-UI GPT: your job is to turn a designer’s sentence into a JSON layout for a Unity game inventory UI.

‼️  IMPORTANT OUTPUT RULES
– Respond with **raw JSON only** (no markdown fences, no extra keys, no prose).  
– The JSON **must validate** against the schema below (same key order).  
– Echo the user’s prompt in the top-level `"prompt"` key for traceability.  
– Provide a final newline.

—————————————————————————————————————
SCHEMA (comments explain each field)

{
  // original designer prompt (for humans / debug)
  "prompt": "<string>",

  // always the literal string "inventory_screen"
  "layout": "inventory_screen",

  // panel header text; default "Inventory"
  "title": "<string>",

  // hex color (#RRGGBB or #RRGGBBAA) for panel background; default #228B22 (green)
  "panel_color": "<hex>",

  // grid definition block
  "grid": {
    // number of rows (1-12). default 8
    "rows": <int>,

    // number of columns (1-12). default 10
    "columns": <int>,

    // slot width & height in pixels. default [80,80]
    "cell_size": [ <int>, <int> ],

    // pixel gap between slots. default 10
    "spacing": <int>,

    // hex color for grid backdrop (can include alpha). default #0000FF80 (50 % blue)
    "bg_color": "<hex>"
  },

  // hex fill color for slot sprites; default #FF0000 (red)
  "slot_color": "<hex>"
}
—————————————————————————————————————
DEFAULTS & AUTO-RULES
1. If user omits rows/columns → use 10×8.
2. Keywords in prompt:
   • “big cells / bigger slots”  → cell_size [100,100]  
   • “tiny / small”              → cell_size [40,40]
3. Simple color words (red, blue, olive, pink) convert to matching #RRGGBB.
4. Always include trailing newline.

EXAMPLE

USER:
I want a sci-fi inventory: 6×5 grid, neon blue panel, cyan slots.

ASSISTANT (what you should output):
{"prompt":"I want a sci-fi inventory: 6×5 grid, neon blue panel, cyan slots.","layout":"inventory_screen","title":"Inventory","panel_color":"#1B03A3","grid":{"rows":6,"columns":5,"cell_size":[80,80],"spacing":10,"bg_color":"#1720FF80"},"slot_color":"#00FFFF"}
★
"""


# ──────────────────────────────────────
# 3.  Routes
# ──────────────────────────────────────
@app.get("/")
def read_root():
    return {"hello": "world"}

@app.post("/prompt", response_model=LayoutJSON)
async def generate_json_design(payload: PromptIn):
    """
    POST JSON: { "prompt": "I need a retro 5×8 inventory..." }
    ↳ returns validated LayoutJSON or raises 422 / 502
    """
    # 1️⃣  Call GPT-4(o)
    try:
        completion = client.chat.completions.create(
            model="gpt-4o-mini",
            messages = [
                {"role": "system", "content": SYSTEM_PROMPT},
                {"role": "user",   "content": payload.prompt}    #  ← correct
            ],
            response_format={"type": "json_object"},
            temperature=0.2,
        )
    except OpenAIError as e:
        raise HTTPException(status_code=502, detail=f"OpenAI error: {e}")

    raw_json = completion.choices[0].message.content

    # 2️⃣  Validate against our strict schema
    try:
        layout = LayoutJSON.model_validate_json(raw_json)
    except ValidationError as ve:
        raise HTTPException(
            status_code=422,
            detail={"msg": "LLM produced invalid JSON", "errors": ve.errors()},
        )

    # 3️⃣  (Optional) push to Figma here:
    # figma_helper.push_layout(layout)

    return layout

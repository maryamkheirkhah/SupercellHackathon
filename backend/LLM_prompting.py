# backend.py
import os
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel, Field, ValidationError
from openai import OpenAI, OpenAIError

# ────────────────────────────────
# 1.  Data-model that DEFINES the JSON we expect from GPT
# ────────────────────────────────
class Element(BaseModel):
    type: str                    # e.g. "ItemGrid"
    position: str                # e.g. "center"
    rows: int | None = Field(None, ge=1, le=10)
    columns: int | None = Field(None, ge=1, le=10)

class LayoutJSON(BaseModel):
    layout: str                  # e.g. "inventory_screen"
    style: str | None = "default"
    elements: list[Element]

# ────────────────────────────────
# 2.  FastAPI plumbing
# ────────────────────────────────
app = FastAPI(title="Smart UI Generator API")

class PromptIn(BaseModel):
    prompt: str

client = OpenAI(
    api_key=os.environ.get("OPENAI_API_KEY"),
)

SYSTEM_PROMPT = """
You are a game-UI generator.
OUTPUT STRICT JSON ONLY — no extra keys, no markdown.
Schema:
{
  "layout": "<string>",
  "style":  "<string>",
  "elements": [
    { "type": "<string>",
      "position": "<string>",
      "rows": <int?>,
      "columns": <int?> }
  ]
}
Valid types: ItemGrid, HealthBar, AmmoCounter, Minimap,
             ButtonPrimary, CharacterPortrait, WeightMeter.
Valid positions: top_left, top_right, center, left, right,
                 bottom, bottom_left, bottom_right.
If rows/columns are omitted, default = 4.
"""

# ────────────────────────────────
# 3.  Endpoint: POST /generate-layout
# ────────────────────────────────
@app.post("/generate-layout", response_model=LayoutJSON)
async def generate_layout(payload: PromptIn):
    """
    1. Sends prompt to GPT-4(o).
    2. Parses response as JSON.
    3. Validates with Pydantic.
    """
    try:
        completion = client.chat.completions.create(
            model="gpt-4o",             # or "gpt-4o" / "gpt-4-turbo"
            messages=[
                {"role": "system", "content": SYSTEM_PROMPT},
                {"role": "user", "content": payload.prompt},
            ],
            response_format={"type": "json_object"},
            temperature=0.2,
        )
    except OpenAIError as e:
        raise HTTPException(status_code=502, detail=f"OpenAI error: {e}")

    raw_json = completion.choices[0].message.content

    # ── 4. Validate the response against our schema
    try:
        layout = LayoutJSON.model_validate_json(raw_json)
    except ValidationError as ve:
        # GPT returned invalid structure — let caller know clearly
        raise HTTPException(
            status_code=422,
            detail={"msg": "LLM produced invalid JSON", "errors": ve.errors()},
        )

    # TODO: call your Figma-API helper here and forward `layout`
    # figma_helper.push_layout(layout)

    return layout        # FastAPI auto-serialises to JSON

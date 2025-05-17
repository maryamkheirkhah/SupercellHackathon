from typing import Union

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

class PromptText(BaseModel):
    prompt: str

@app.get("/")
def read_root():
    return {"Hello": "World"}


@app.post("/prompt")
def generate_json_design(prompt: PromptText):
    print(prompt.prompt)
    return {"prompt": prompt.prompt}

@app.post("/figma_plugin")
def figma_plugin(prompt: PromptText):
    print(f"prompt text: {prompt.prompt}")
    return {
        "frame": {
            "name": "test",
            "x": 0,
            "y": 0,
            "width": 250,
            "height": 500,
            "fills": [{"type": 'SOLID', "color": {"r": 1, "g": 1, "b": 1}}],
            "children": [
                {
                    "type": "rectangle",
                    "name": "test",
                    "width": 50,
                    "height": 50,
                    "x": 0,
                    "y": 0,
                    "fills": [{"type": 'SOLID', "color": {"r": 1, "g": 0.2, "b": 0.9}}],
                },
                {
                    "type": "rectangle",
                    "name": "test",
                    "width": 50,
                    "height": 150,
                    "x": 100,
                    "y": 80,
                    "fills": [{"type": 'SOLID', "color": {"r": 0.5, "g": 1, "b": 1}}],
                }
            ]
        }
    }
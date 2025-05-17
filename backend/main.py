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

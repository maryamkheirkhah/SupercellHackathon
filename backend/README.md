Python version: 3.13.2

To activate your virtual environment and install dependencies, follow these steps:

1. **Create and activate a virtual environment** (if you haven't already):

   On **Unix/macOS**:
   ```
   python3 -m venv .venv
   source .venv/bin/activate
   ```


2. **Install dependencies** (for this project, FastAPI is required):

   ```
   pip3 install -r requirements.txt
   ```

3. **Start dev server**

    ```
    fastapi dev main.py
    ```

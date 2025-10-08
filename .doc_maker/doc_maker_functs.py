import os
import pathlib
import google.generativeai as genai
from typing import Optional
import sys


# Lee la clave desde una variable de entorno
API_KEY = os.getenv("GEMINI_API_KEY")

SCAN_DIR = "Assets"  # Directorio a escanear
EXCLUDED_DIRS = {
    ".doc_maker",
    ".git",
    ".vscode",
    "Packages",
    "Library",
    "TutorialInfo",
    "Outline",
}  # Directorios a excluir
OUTPUT_DIR = "Docs"
MODEL_NAME = "gemini-2.5-flash"

PROMPT_PATH = pathlib.Path(".doc_maker") / "doc_maker_prompt.md"

# Configura la llave API
if not API_KEY:
    print("❌ Error: La variable de entorno GEMINI_API_KEY no está configurada.")
    print("Por favor, configúrala con tu clave de API de Google Generative AI.")
    sys.exit(1)  # Termina el script si la clave no existe

genai.configure(api_key=API_KEY)
model = genai.GenerativeModel(MODEL_NAME)


def _read_file(file_path: pathlib.Path) -> Optional[str]:
    """
    Función auxiliar para leer un archivo de forma segura.
    Devuelve el contenido del archivo o None si ocurre un error.
    """
    try:
        with open(file_path, "r", encoding="utf-8") as f:
            return f.read()
    except Exception as e:
        print(f"❌ Error leyendo {file_path}: {e}")
        return None


def get_scripts(scan_root: str):
    for dirpath, dirnames, filenames in os.walk(scan_root):
        # Elimina carpetas excluidas del recorrido
        dirnames[:] = [d for d in dirnames if d not in EXCLUDED_DIRS]
        for file in filenames:
            ruta = pathlib.Path(dirpath) / file
            if ruta.suffix == ".cs":
                yield ruta


def document_script(
    script_path: pathlib.Path, base_dir: pathlib.Path, readme_context: str
):
    """
    Genera la documentación para un único script de C#.
    """
    script_content = _read_file(script_path)
    if script_content is None:
        return

    prompt_template = _read_file(PROMPT_PATH)
    if prompt_template is None:
        return

    # Construye el prompt final
    context_section = ""
    if readme_context:
        context_section = f"**Contexto General del Proyecto (extraído del README.md):**\n\n---\n\n{readme_context}"

    final_prompt = f"{context_section}\n{prompt_template}\n{script_content}\n```"

    # Intenta generar el contenido con el modelo
    try:
        response = model.generate_content(final_prompt)
        response_text = response.text
    except Exception as e:
        print(f"❌ Error inesperado con Gemini al procesar {script_path}: {e}")
        return

    # Guarda el archivo de documentación
    output_path = pathlib.Path(OUTPUT_DIR) / script_path.relative_to(base_dir)
    output_path = output_path.with_suffix(".md")
    output_path.parent.mkdir(parents=True, exist_ok=True)

    try:
        with open(output_path, "w", encoding="utf-8") as f:
            f.write(response_text)
        print(f"✅ Documentado: {output_path}")
    except Exception as e:
        print(f"❌ Error guardando archivo {output_path}: {e}")


def main():
    repo_root = pathlib.Path(".").resolve()
    scan_path = repo_root / SCAN_DIR
    readme_path = repo_root / "README.md"

    # Carga el contenido del README en la variable global
    readme_context = ""
    if readme_path.is_file():
        content = _read_file(readme_path)
        if content:
            readme_context = content
            print("ℹ️  Contexto del README.md cargado.")
    else:
        print("ℹ️  No se encontró README.md, continuando sin contexto adicional.")

    if not scan_path.is_dir():
        print(f"❌ El directorio a escanear no existe: {scan_path}")
        return

    print(f"📁 Escaneando: {scan_path}")
    for archivo in get_scripts(scan_path):
        document_script(archivo, repo_root, readme_context)

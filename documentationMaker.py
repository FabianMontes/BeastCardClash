import os
import pathlib
import google.generativeai as genai

import sys


# === CONFIGURACIÓN ===
API_KEY = os.getenv("GEMINI_API_KEY")  # Lee la clave desde una variable de entorno
EXTENSIONS = {".cs"}  # Solo archivos C#
SCAN_DIR = "Assets"  # Directorio a escanear
EXCLUDED_DIRS = {
    ".git",
    ".vscode",
    "Packages",
    "Library",
    "TutorialInfo",
}  # Directorios a excluir
OUTPUT_DIR = "Docs"
MODEL_NAME = "gemini-2.5-flash"

# Variable global para el contexto del README
README_CONTEXT = ""

# === INICIALIZACIÓN ===
if not API_KEY:
    print("❌ Error: La variable de entorno GEMINI_API_KEY no está configurada.")
    print("Por favor, configúrala con tu clave de API de Google Generative AI.")
    sys.exit(1)  # Termina el script si la clave no existe

genai.configure(api_key=API_KEY)
model = genai.GenerativeModel(MODEL_NAME)

# === UTILIDADES ===


def es_codigo_valido(path: pathlib.Path) -> bool:
    return path.suffix in EXTENSIONS


def obtener_archivos_codigo(scan_root: str):
    for dirpath, dirnames, filenames in os.walk(scan_root):
        # Elimina carpetas excluidas del recorrido
        dirnames[:] = [d for d in dirnames if d not in EXCLUDED_DIRS]
        for file in filenames:
            ruta = pathlib.Path(dirpath) / file
            if es_codigo_valido(ruta):
                yield ruta


def generar_documentacion_para_archivo(archivo: pathlib.Path, base_dir: pathlib.Path):
    try:
        with open(archivo, "r", encoding="utf-8") as f:
            contenido = f.read()
    except Exception as e:
        print(f"❌ Error leyendo {archivo}: {e}")
        return

    # Construye la sección de contexto si el README fue cargado
    contexto_readme_str = ""
    if README_CONTEXT:
        contexto_readme_str = f"""**Contexto General del Proyecto (extraído del README.md):**
---
{README_CONTEXT}
---

"""
    prompt = f"""{contexto_readme_str}

Actúa como un ingeniero de software senior documentando código para un nuevo miembro del equipo en un proyecto de Unity llamado "BeastCardClash".

Tu tarea es generar una documentación técnica clara, concisa y bien estructurada en formato Markdown (GFM) para el siguiente archivo de código C#.

Sigue esta estructura estrictamente:

# `{archivo.name}`

## 1. Propósito General
Describe en 1-2 frases el rol principal de este script dentro del juego. ¿Qué gestiona? ¿Con qué otros sistemas principales interactúa?

## 2. Componentes Clave
Enumera y explica las clases, structs o enums más importantes definidos en el archivo. Para cada clase principal:

### [Nombre de la Clase]
- **Descripción:** Una explicación detallada de lo que hace la clase. Si hereda de `MonoBehaviour`, menciónalo.
- **Variables Públicas / Serializadas:** Lista las variables más importantes que son visibles en el Inspector de Unity (`[SerializeField]`) o públicas. Explica para qué se usa cada una.
- **Métodos Principales:**
    - `void MetodoImportante(parametro)`: Describe qué hace el método, qué significan sus parámetros y qué devuelve (si aplica). Presta especial atención a los métodos del ciclo de vida de Unity (`Awake`, `Start`, `Update`, `FixedUpdate`, etc.).
- **Lógica Clave:** Si hay algún algoritmo, máquina de estados o flujo de trabajo complejo, explícalo aquí de forma sencilla.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Menciona si este script usa `[RequireComponent(typeof(UnComponente))]`.
- **Eventos (Entrada):** ¿A qué eventos se suscribe este script? (Ej: `button.onClick.AddListener(...)`).
- **Eventos (Salida):** ¿Este script invoca algún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas?

---

**Instrucciones Adicionales:**
- **Precisión:** Basa tu documentación únicamente en el código proporcionado. No inventes funcionalidades.
- **Claridad:** Usa un lenguaje claro y directo. Evita la jerga innecesaria.
- **Fragmentos de Código:** Incluye pequeños fragmentos de código relevantes para ilustrar tus explicaciones, pero no copies funciones enteras a menos que sea crucial.
- **Llano**: Usa un formato simple, con menos listas y más párrafos explicativos.

Aquí está el archivo a documentar:

```{archivo.suffix[1:]}
{contenido}
```
"""

    try:
        respuesta = model.generate_content(prompt)
        texto = respuesta.text
    except Exception as e:
        print(f"❌ Error inesperado con Gemini al procesar {archivo}: {e}")
        return

    # Ruta de salida
    salida = pathlib.Path(OUTPUT_DIR) / archivo.relative_to(base_dir)
    salida = salida.with_suffix(salida.suffix + ".md")
    salida.parent.mkdir(parents=True, exist_ok=True)

    try:
        with open(salida, "w", encoding="utf-8") as f:
            f.write(texto)
        print(f"✅ Documentado: {archivo} → {salida}")
    except Exception as e:
        print(f"❌ Error guardando archivo {salida}: {e}")


# === PROGRAMA PRINCIPAL ===


def main():
    global README_CONTEXT  # Indica que vamos a modificar la variable global
    repo_root = pathlib.Path(".").resolve()
    scan_path = repo_root / SCAN_DIR
    readme_path = repo_root / "README.md"

    # Carga el contenido del README en la variable global
    if readme_path.is_file():
        try:
            with open(readme_path, "r", encoding="utf-8") as f:
                README_CONTEXT = f.read()
            print("ℹ️  Contexto del README.md cargado.")
        except Exception as e:
            print(f"⚠️  No se pudo leer el archivo README.md: {e}")
    else:
        print("ℹ️  No se encontró README.md, continuando sin contexto adicional.")

    if not scan_path.is_dir():
        print(f"❌ El directorio a escanear no existe: {scan_path}")
        return

    print(f"📁 Escaneando: {scan_path}")
    for archivo in obtener_archivos_codigo(scan_path):
        generar_documentacion_para_archivo(
            archivo, repo_root
        )  # La llamada es más simple


if __name__ == "__main__":
    main()

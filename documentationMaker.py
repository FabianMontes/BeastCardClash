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
    prompt = f"""
## Resumen de documentación
Eres un programador con experiencia en el desarrollo de videojuegos con Unity y C#, así como también en el proceso de documentación y cumplimiento de buenas prácticas en los proyectos que manejas.

Con el contenido del archivo del script que tienes adjunto, y teniendo en cuenta los detalles del proyecto del archivo README que también esta adjunto, crea un texto de documentación que explique de forma clara y exhaustiva su estructura, su funcionamiento y la forma en la que parece interactúar con el resto de los componentes del proyecto.

Los archivos que crees están orientados a ser leídas por los demás miembros del proyecto, por lo que el nivel técnico y contenido debe estar al nivel de sus necesidades y no contener obviedades o explicaciones inútiles, así como tampoco ignorar información.

El proyecto, en complemento del README y lo que explica, consiste en un videojuego indie, cuyo enfoque no es el seguimiento de reglas precisas ni buenas prácticas perfectas, sino el desarrollo de una buena experiencia de jugador y sobre todo, de desarrollo para los programadores. Tenlo en cuenta.

----

Para el archivo a crear sigue la siguiente estructura

```Markdown
# [Nombre del script sin la extensión]
[Resumen completo y explicado de la función del script y su funcionamiento]

# Métodos

## Métodos de Unity

### [Nombre del método (Awake, Start, Update)]
[Explicación completa del funcionamiento del método]

## Otros métodos

### [Nombre y tipado del método]
[Explicación completa del funcionamiento del método]

[Repite este bloque de título + descripción por cada método que haya]

## Getters y Setters

1. [Nombre y tipado del método, sin los paréntesis de los parámetros]: [Explica el dato que pide o establece.]

[Repite este bloque numerado de método + dato por cada getter o setter que haya]
```

---

Algunos detalles extra para el contenido del documento:

1. No incluyas información ajena al contenido buscado: no incluyas descripciones iniciales ("Aquí tienes tu documento..."), preguntas de seguimiento o formato Markdown fuera de la estructura planeada.
2. Mantén un tono neutral y no dirigido, al estilo de la documentación real de un proyecto... tus documentos tambien lo serán.
3. Usa formato Markdown GFM o Obsidian Markdown en su defecto. Sé consistente y aprovecha caracteristicas como callouts o bloques de código en el texto.
4. Incluye secciones reales del código para explicar partes críticas y mejorar la comprensión del documento. Los scripts están en C# siempre. Usa igualmente nombres de métodos, variables o componentes para incorporar las explicaciones al flujo del código.
5. Mantén el contexto autocontenido. Como se supone que no tienes acceso a otros scripts, actúa dentro del contexto del que tienes adjunto solamente y no hagas suposiciones riesgosas sobre lo que sea externo.

---

Adjunto tienes el archivo a documentar:

---

{archivo.suffix[1:]}

```csharp
{contenido}
```

Y el README.md del proyecto:

```markdown
{contexto_readme_str}
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
        print(f"✅ Documentado: {salida}")
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

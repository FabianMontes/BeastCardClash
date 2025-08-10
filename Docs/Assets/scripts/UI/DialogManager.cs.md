Aquí tienes la documentación técnica para el script `DialogManager.cs`:

---

# `DialogManager.cs`

## 1. Propósito General
Este script es responsable de gestionar y mostrar los diálogos interactivos en el juego. Controla la visibilidad del panel de diálogo, la navegación a través de las líneas de texto, y la selección del conjunto de diálogos apropiado según el estado actual del juego. Interactúa con la UI para presentar el texto y los nombres de los personajes, y con el sistema de movimiento del jugador para pausarlo durante los diálogos.

## 2. Componentes Clave

### `GameStates`
- **Descripción:** Un `enum` que define los diferentes estados o contextos posibles del juego que influyen en qué conjunto de diálogos debe mostrarse.
- **Valores:**
    - `normal`: El estado predeterminado del juego.
    - `preGame`: Indica el momento antes de que el juego comience formalmente, típicamente para diálogos introductorios.
    - `win`: El estado cuando el jugador ha ganado.
    - `lose`: El estado cuando el jugador ha perdido.

### `DialogFile`
- **Descripción:** Una clase serializable (`[System.Serializable]`) utilizada para estructurar y cargar el contenido de los archivos JSON que contienen todos los diálogos del juego. Actúa como el contenedor principal para diferentes secuencias de diálogos basadas en el estado del juego.
- **Variables Públicas / Serializadas:**
    - `Dialogs[] Dialogs`: Un array de objetos `Dialogs` que contiene la secuencia de diálogos para el estado `normal`.
    - `Dialogs[] PreGameDialogs`: Un array para los diálogos del estado `preGame`.
    - `Dialogs[] WinDialogs`: Un array para los diálogos del estado `win`.
    - `Dialogs[] LoseDialogs`: Un array para los diálogos del estado `lose`.

### `Dialogs`
- **Descripción:** Una clase serializable (`[System.Serializable]`) que representa una única entrada de diálogo. Contiene la información de un personaje específico y el texto que pronuncia.
- **Variables Públicas / Serializadas:**
    - `string character`: El nombre del personaje que habla la línea de diálogo.
    - `string text`: El contenido del texto del diálogo.

### `DialogManager`
- **Descripción:** El componente principal que gestiona la lógica de los diálogos. Hereda de `MonoBehaviour`, lo que permite que sea adjuntado a un GameObject en la escena de Unity y que utilice los métodos del ciclo de vida de Unity.
- **Variables Públicas / Serializadas:**
    - `GameObject dialogPanel`: Referencia al objeto UI (`GameObject`) que actúa como el contenedor visual de todos los elementos del diálogo. Se activa o desactiva para mostrar u ocultar el diálogo.
    - `TextMeshProUGUI namePanel`: Referencia al componente de texto de TextMeshPro que muestra el nombre del personaje que está hablando.
    - `TextMeshProUGUI textPanel`: Referencia al componente de texto de TextMeshPro que muestra el contenido del diálogo.
    - `string characterName`: Una variable de cadena para un nombre de personaje, aunque en la implementación actual los nombres son extraídos del archivo JSON. Podría ser un valor predeterminado o un identificador para el propio objeto que contiene este `DialogManager`.
    - `Transform target`: Referencia al `Transform` del jugador o del objeto con el que este `DialogManager` interactuará para iniciar diálogos.
    - `float maxDistance`: La distancia máxima entre este GameObject y el `target` para que se pueda iniciar un diálogo.
    - `GameStates gameState`: El estado actual del juego, que determina qué conjunto de diálogos se mostrará.
    - `TextAsset dialogFile`: El archivo de texto (JSON) que contiene todos los datos de los diálogos. Se debe arrastrar desde el Editor de Unity.

- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se ejecuta una vez al inicio.
        - Inicializa la referencia al script `Target` del objeto `target`.
        - Desactiva el `dialogPanel` para que no sea visible al inicio.
        - Carga y deserializa el contenido del `dialogFile` (JSON) en el objeto `dialogFileContent` utilizando `JsonUtility.FromJson`.
    - `void Update()`: Este método del ciclo de vida de Unity se ejecuta una vez por frame.
        - Calcula la distancia entre el `DialogManager` y el `target`.
        - Verifica si el `target` está dentro del `maxDistance` y si el jugador presiona la tecla `Z` mientras no hay un diálogo activo. Si ambas condiciones se cumplen, llama a `ShowDialogPanel()`.
        - Si un diálogo está activo (`inDialog` es `true`):
            - Si el jugador presiona `Z`, avanza al siguiente diálogo llamando a `DisplayDialog()`.
            - Si el jugador presiona `C`, finaliza el diálogo inmediatamente llamando a `EndDialog()`.
    - `void ShowDialogPanel()`: Inicia la secuencia de diálogo.
        - Establece `inDialog` a `true` para indicar que un diálogo está activo.
        - Deshabilita el script `Target` del jugador (`targetScript.enabled = false`) para evitar el movimiento durante el diálogo.
        - Activa el `dialogPanel` para hacerlo visible.
        - Reinicia el `currentDialogIndex` a `0` y llama a `DisplayDialog()` para mostrar la primera línea del diálogo.
    - `Dialogs[] GetDialogsForState()`: Un método auxiliar que devuelve el array de diálogos adecuado basándose en el valor actual de `gameState`. Utiliza un `switch` para seleccionar entre `Dialogs`, `PreGameDialogs`, `WinDialogs`, o `LoseDialogs` de `dialogFileContent`.
    - `void DisplayDialog()`: Muestra la línea de diálogo actual en la UI.
        - Obtiene el array de diálogos relevante para el `gameState` actual.
        - Si `currentDialogIndex` es menor que la longitud del array de diálogos, actualiza `namePanel.text` y `textPanel.text` con la información del diálogo correspondiente.
        - Si no hay más diálogos en la secuencia (es decir, `currentDialogIndex` ha superado la longitud del array), llama a `EndDialog()`.
    - `void EndDialog()`: Finaliza la secuencia de diálogo.
        - Establece `inDialog` a `false`.
        - Desactiva el `dialogPanel`.
        - Habilita el script `Target` del jugador (`targetScript.enabled = true`) para permitir el movimiento nuevamente.
        - Reinicia `currentDialogIndex` a `0` para que el próximo diálogo comience desde el principio.
    - `public void SetGameState(GameStates newState)`: Un método público que permite a otros scripts cambiar el estado del juego, lo que a su vez afectará qué secuencias de diálogo se reproducen.

- **Lógica Clave:**
    La lógica de este script se centra en una máquina de estados implícita para la gestión de diálogos:
    1.  **Activación:** El diálogo se inicia cuando el `target` está dentro de un rango (`maxDistance`) y el jugador presiona la tecla `Z`, siempre y cuando no haya ya un diálogo activo.
    2.  **Interacción:** Durante un diálogo activo (`inDialog` es `true`), el movimiento del jugador se desactiva.
        *   Presionar `Z` avanza a la siguiente línea de diálogo.
        *   Presionar `C` salta y finaliza el diálogo inmediatamente.
    3.  **Finalización:** Un diálogo termina automáticamente cuando se han mostrado todas sus líneas o cuando el jugador lo salta. Al finalizar, el panel de diálogo se oculta y el movimiento del jugador se restaura.
    4.  **Contexto:** La selección de los diálogos a mostrar depende del `GameStates` actual, lo que permite diferentes narrativas para el pre-juego, el juego normal, la victoria o la derrota.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Este script no utiliza `[RequireComponent]`, pero asume que el `target` (Transform) tiene un componente llamado `Target` que maneja su movimiento, y que el `dialogPanel`, `namePanel`, y `textPanel` existen y están configurados correctamente en la UI.
- **Eventos (Entrada):**
    - Escucha la entrada del teclado directamente a través de `Input.GetKeyDown(KeyCode.Z)` para iniciar/avanzar diálogos.
    - Escucha `Input.GetKeyDown(KeyCode.C)` para saltar/terminar diálogos.
- **Eventos (Salida):**
    - Este script no invoca eventos de `UnityEvent` o `Action` para notificar a otros sistemas. Su interacción con el script `Target` es directa al habilitar/deshabilitar su componente.
    - El método `public void SetGameState(GameStates newState)` actúa como un punto de entrada público para que otros sistemas modifiquen el comportamiento de los diálogos.

---
# `DialogManager.cs`

## 1. Propósito General

Este script es responsable de gestionar y mostrar los diálogos narrativos dentro del juego. Controla la visibilidad del panel de diálogo, la progresión de las líneas de texto y la interacción del jugador para iniciar o avanzar a través de las conversaciones, integrándose con el sistema de estado del juego (`GameState`).

## 2. Componentes Clave

### `DialogManager`

-   **Descripción:**
    Esta clase es un `MonoBehaviour`, lo que significa que puede adjuntarse a un GameObject en la escena de Unity. Su función principal es orquestar la aparición, el contenido y la interacción de los diálogos en el juego. Determina cuándo un diálogo debe comenzar (basado en la proximidad a un `target` y la entrada del jugador), cómo avanza y cuándo termina. También se encarga de cargar el conjunto correcto de diálogos basándose en el estado actual del juego.

-   **Variables Públicas / Serializadas:**
    Las siguientes variables se exponen en el Inspector de Unity, permitiendo configurar el comportamiento del manager desde el editor sin modificar el código:
    -   `dialogPanel` (GameObject): Es el `GameObject` principal que contiene todos los elementos visuales del diálogo (nombre, texto). Su activación/desactivación controla la visibilidad de todo el sistema de diálogo.
    -   `namePanel` (TextMeshProUGUI): Componente de texto donde se mostrará el nombre del personaje que habla.
    -   `textPanel` (TextMeshProUGUI): Componente de texto donde se mostrará el contenido de la línea de diálogo actual.
    -   `target` (Transform): El `Transform` de un objeto en la escena (e.g., un NPC) con el que el jugador puede interactuar para iniciar un diálogo.
    -   `maxDistance` (float): La distancia máxima entre el GameObject que contiene este `DialogManager` y el `target` para que un diálogo pueda ser activado.
    -   `targetScript` (Target): Una referencia al script `Target` que se espera esté adjunto al `target` GameObject. Se utiliza para deshabilitar o habilitar la funcionalidad de ese script mientras un diálogo está activo o inactivo, respectivamente.
    -   `currentDialogIndex` (int): Un índice interno que lleva un seguimiento de la línea de diálogo actual dentro de una secuencia.
    -   `inDialog` (bool): Una bandera interna que indica si un diálogo está activo en este momento.

-   **Métodos Principales:**

    -   `void Start()`:
        Método del ciclo de vida de Unity, llamado una vez antes del primer `Update`.
        Se utiliza para la inicialización. Aquí se obtiene una referencia al componente `Target` del `target` asignado y se asegura de que el `dialogPanel` esté inicialmente oculto.

        ```csharp
        targetScript = target.GetComponent<Target>();
        dialogPanel?.SetActive(false);
        ```

    -   `void Update()`:
        Método del ciclo de vida de Unity, llamado una vez por cada frame.
        Contiene la lógica principal para detectar la entrada del jugador y la proximidad al `target`.
        -   Calcula la distancia al `target`.
        -   Si no hay un diálogo activo, el `target` está en rango y el jugador presiona `KeyCode.Z`, inicia el diálogo llamando a `ShowDialogPanel()`.
        -   Si un diálogo ya está activo y el jugador presiona `KeyCode.Z`, avanza a la siguiente línea del diálogo.
        -   Si un diálogo ya está activo y el jugador presiona `KeyCode.C`, termina el diálogo.

    -   `void ShowDialogPanel()`:
        Gestiona el inicio de una secuencia de diálogo.
        Activa el `dialogPanel`, establece la bandera `inDialog` a `true` y deshabilita el `targetScript` (posiblemente para detener el movimiento del jugador o la interacción con el `target`). Luego, inicializa el `currentDialogIndex` y llama a `DisplayNextDialog()` para mostrar la primera línea.

    -   `Dialogs[] GetDialogsForState()`:
        Este método es crucial para la lógica de contenido dinámico.
        Accede al `GameState.singleton` para obtener el `DialogFile` actual y devuelve un array de objetos `Dialogs` específico para el `CurrentGameState` (e.g., `begin`, `preGame`, `win`, `lose`, `repeat`). Esto permite que el diálogo cambie según el progreso o el resultado del juego.

    -   `void DisplayNextDialog()`:
        Muestra la siguiente línea de diálogo en la UI.
        Obtiene el array de diálogos relevante del `GetDialogsForState()`. Si aún quedan diálogos en la secuencia (`currentDialogIndex` es menor que la longitud del array), actualiza `namePanel` y `textPanel` con el contenido del diálogo actual. Si no quedan más líneas, llama a `EndDialog()`.

    -   `void EndDialog()`:
        Finaliza la secuencia de diálogo actual.
        Desactiva el `dialogPanel`, restablece la bandera `inDialog` a `false` y re-habilita el `targetScript`. También reinicia el `currentDialogIndex` y notifica al `GameState` llamando a `GameState.singleton.NextGameState()`, lo que puede desencadenar un cambio en el estado general del juego.

-   **Lógica Clave:**
    La gestión del diálogo se basa en una máquina de estados simple:
    1.  **Fuera de Diálogo:** El script está atento a la distancia al `target` y a la pulsación de la tecla `Z`. Si ambas condiciones se cumplen, se pasa al estado "En Diálogo".
    2.  **En Diálogo:** El script ignora la distancia al `target` y en su lugar procesa las pulsaciones de `Z` para avanzar la conversación y `C` para terminarla prematuramente.
    3.  **Transición de Estados del Juego:** Los diálogos que se muestran están directamente ligados al `GameState` actual del juego. Al finalizar un diálogo, se notifica al `GameState` para una posible transición, lo que es vital para la progresión narrativa del juego.

## 3. Dependencias y Eventos

-   **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere:
    -   Un `Transform` para la variable `target`.
    -   Un script `Target` adjunto al `GameObject` del `target`.
    -   Componentes `TextMeshProUGUI` para `namePanel` y `textPanel` en la jerarquía del `dialogPanel`.
    -   La existencia de una instancia `GameState.singleton` en la escena para acceder a la configuración de los diálogos y gestionar el estado del juego.

-   **Eventos (Entrada):**
    El script se suscribe directamente a la entrada del teclado a través de `Input.GetKeyDown()` para las teclas `KeyCode.Z` (para iniciar/avanzar diálogo) y `KeyCode.C` (para finalizar diálogo).

-   **Eventos (Salida):**
    Este script no emite `UnityEvent`s o `Action`s públicos. Sin embargo, interactúa directamente con el `GameState.singleton` invocando `GameState.singleton.NextGameState(GameState.singleton.CurrentGameState)` al finalizar un diálogo. Esto actúa como un mecanismo de notificación a otro sistema central del juego, indicando que una secuencia de diálogo ha concluido y el juego puede avanzar a un nuevo estado o fase.
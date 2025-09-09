# DialogManager
El script `DialogManager` es el componente central para la gestión y visualización de diálogos en el proyecto `Beast Card Clash`. Su función principal es controlar la interfaz de usuario de los diálogos, determinar cuándo deben activarse basándose en la proximidad del jugador y la interacción, y gestionar el flujo de las líneas de diálogo en función del estado actual del juego.

Este componente es crucial para la narrativa del juego, permitiendo la presentación de historias, información o interacciones con los personajes (animales autóctonos/facultades) que enriquecen la experiencia del jugador. Durante un diálogo, el script se encarga de pausar temporalmente el movimiento o la interacción del jugador para asegurar una inmersión completa en la conversación. Su diseño prioriza una implementación directa y fácil de mantener para los desarrolladores, alineándose con el enfoque del proyecto en una buena experiencia de desarrollo.

La interacción con otros componentes se realiza principalmente con:
*   **Elementos de UI**: Gestiona los `GameObject` y `TextMeshProUGUI` configurados para mostrar el panel de diálogo, el nombre del personaje y el texto del diálogo.
*   **Componente `Target`**: Requiere una referencia al `Transform` del "target" (presumiblemente el jugador o una entidad interactuable) para calcular distancias y su script asociado (`Target`) para habilitar o deshabilitar su funcionalidad (como el movimiento).
*   **Singleton `GameState`**: Depende del patrón Singleton `GameState` para acceder al archivo de diálogos (`DialogFile`) y determinar el estado actual del juego (`GameStates`), lo que permite cargar los diálogos pertinentes para cada momento de la partida. También notifica al `GameState` el final de un diálogo para una posible transición de estado.

# Métodos

## Métodos de Unity

### Awake, Start, Update

### Start()
Este método se invoca una vez al inicio del ciclo de vida del script, antes de la primera actualización de frame. Su propósito es inicializar las referencias y el estado inicial del `DialogManager`.

*   **Inicialización de `targetScript`**: Obtiene una referencia al componente `Target` del `GameObject` asignado a la variable `target`. Este componente es esencial para controlar el movimiento o las interacciones del jugador durante los diálogos.
*   **Desactivación del panel de diálogo**: Asegura que el `dialogPanel` se encuentre oculto al inicio del juego, proporcionando una interfaz limpia hasta que se active un diálogo.

```csharp
void Start()
{
    targetScript = target.GetComponent<Target>();
    dialogPanel?.SetActive(false);
}
```

### Update()
Este método se invoca una vez por frame y contiene la lógica principal para la detección de interacciones y el manejo del flujo de los diálogos.

*   **Detección de proximidad y activación de diálogo**:
    *   Calcula la distancia euclidiana entre la posición del `DialogManager` (donde probablemente se encuentre el trigger del diálogo) y la posición del `target`.
    *   Determina si el `target` se encuentra dentro del `maxDistance` configurado.
    *   Si el juego no está en un diálogo (`!inDialog`), el `target` está en rango (`targetInRange`) y el jugador presiona la tecla `Z`, se llama al método `ShowDialogPanel()` para iniciar un nuevo diálogo.

*   **Avance y finalización de diálogo**:
    *   Si ya se encuentra en un diálogo (`inDialog` es `true`):
        *   Detecta la pulsación de la tecla `Z` para avanzar al siguiente segmento del diálogo, incrementando `currentDialogIndex` y llamando a `DisplayNextDialog()`.
        *   Detecta la pulsación de la tecla `C` para terminar el diálogo de forma anticipada, llamando a `EndDialog()`.

    > [!NOTE] Importante
    > La estructura `if (!inDialog && ...)` seguido de `else if (inDialog)` es una elección deliberada para evitar el "bug del segundo panel". Esta lógica asegura que una única pulsación de la tecla `Z` solo inicie un diálogo o avance uno existente, pero no ambas cosas en el mismo frame, mejorando la experiencia de interacción del jugador.

```csharp
void Update()
{
    float distance = Vector3.Distance(transform.position, target.position);
    bool targetInRange = distance <= maxDistance;

    if (!inDialog && targetInRange && Input.GetKeyDown(KeyCode.Z))
    {
        ShowDialogPanel();
    }
    else if (inDialog)
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            currentDialogIndex++;
            DisplayNextDialog();
        }
        if (Input.GetKeyDown(KeyCode.C)) EndDialog();
    }
}
```

## Otros métodos

### ShowDialogPanel() : void
Este método se encarga de iniciar el proceso de un nuevo diálogo.

*   **Control de reentrada**: Primero verifica si ya hay un diálogo activo (`inDialog`). Si es así, el método retorna inmediatamente para evitar la superposición o reinicio accidental de diálogos.
*   **Activación de diálogo**: Establece `inDialog` a `true` e inmediatamente activa el `dialogPanel` para hacerlo visible al jugador.
*   **Control del jugador**: Deshabilita el script `targetScript` (probablemente responsable del movimiento o interacción del jugador) para pausar la acción y enfocar la atención en el diálogo.
*   **Inicio del flujo**: Reinicia el `currentDialogIndex` a `0` para comenzar desde la primera línea de diálogo y llama a `DisplayNextDialog()` para mostrar el primer texto.

```csharp
void ShowDialogPanel()
{
    if (inDialog) return;

    inDialog = true;
    dialogPanel.SetActive(true);
    targetScript.enabled = false;

    currentDialogIndex = 0;
    DisplayNextDialog();
}
```

### GetDialogsForState() : Dialogs[]
Este método es responsable de obtener el conjunto de diálogos apropiado para el estado actual del juego.

*   **Carga de contenido**: Intenta cargar el `dialogFileContent` desde el singleton `GameState`. Incluye verificaciones de nulidad para asegurar la robustez en caso de que el archivo de diálogos no esté asignado.
*   **Selección de diálogos**: Utiliza una sentencia `switch` para evaluar el `CurrentGameState` del `GameState.singleton`. Dependiendo del estado (ej. `begin`, `preGame`, `win`, `lose`, `repeat`), devuelve el array de `Dialogs` correspondiente desde el `dialogFileContent`.
*   **Manejo por defecto**: Si el estado actual no coincide con ninguno de los casos definidos o si no se puede cargar el archivo de diálogos, devuelve un array vacío de `Dialogs` para evitar errores.

```csharp
Dialogs[] GetDialogsForState()
{
    DialogFile dialogFileContent = GameState.singleton.dialogFileContent;
    // La doble verificación de nulidad puede ser redundante pero asegura la referencia.
    if (dialogFileContent == null) dialogFileContent = GameState.singleton.dialogFileContent;
    if (dialogFileContent == null) return new Dialogs[0];

    switch (GameState.singleton.CurrentGameState)
    {
        case GameStates.begin:
            return dialogFileContent.BeginDialogs;
        case GameStates.preGame:
            return dialogFileContent.PreGameDialogs;
        case GameStates.win:
            return dialogFileContent.WinDialogs;
        case GameStates.lose:
            return dialogFileContent.LoseDialogs;
        case GameStates.repeat:
            return dialogFileContent.RepeatDialogs;
        default:
            return new Dialogs[0];
    }
}
```

### DisplayNextDialog() : void
Este método se encarga de actualizar la interfaz de usuario con la siguiente línea de diálogo.

*   **Obtención de diálogos**: Llama a `GetDialogsForState()` para obtener el array de diálogos relevante para el estado actual del juego.
*   **Avance de línea**:
    *   Verifica si el `currentDialogIndex` es menor que la longitud total del array de diálogos.
    *   Si hay más diálogos disponibles, actualiza el `namePanel.text` con el nombre del personaje (`character`) y el `textPanel.text` con el contenido de la línea de diálogo (`text`) en la posición actual del índice.
    *   Si el `currentDialogIndex` es igual o mayor que la longitud del array, significa que no hay más líneas de diálogo en la secuencia actual, por lo que llama a `EndDialog()` para finalizar la conversación.

```csharp
void DisplayNextDialog()
{
    var dialogs = GetDialogsForState();

    if (currentDialogIndex < dialogs.Length)
    {
        namePanel.text = dialogs[currentDialogIndex].character;
        textPanel.text = dialogs[currentDialogIndex].text;
    }
    else
    {
        EndDialog();
    }
}
```

### EndDialog() : void
Este método se encarga de finalizar un diálogo activo y restaurar el estado normal del juego.

*   **Desactivación de diálogo**: Establece `inDialog` a `false` y desactiva el `dialogPanel`, ocultándolo de la vista del jugador.
*   **Restauración del jugador**: Habilita el `targetScript` (probablemente el script de movimiento del jugador) para que el jugador pueda reanudar sus acciones.
*   **Reinicio de índice**: Reinicia el `currentDialogIndex` a `0`, preparando el sistema para la próxima interacción de diálogo.
*   **Transición de estado**: Notifica al `GameState.singleton` para que avance al siguiente estado de juego, pasando el estado actual como referencia. Esto permite que el sistema de juego reaccione a la finalización del diálogo, lo cual es fundamental para la progresión narrativa de `Beast Card Clash`.

```csharp
void EndDialog()
{
    inDialog = false;
    dialogPanel.SetActive(false);
    targetScript.enabled = true;

    currentDialogIndex = 0;

    GameState.singleton.NextGameState(GameState.singleton.CurrentGameState);
}
```

## Getters y Setters
En el script `DialogManager`, no se han definido métodos públicos explícitos que funcionen como *getters* o *setters* en el sentido tradicional (ej. `public int GetCurrentIndex()` o `public void SetCurrentIndex(int value)`). La gestión del estado interno, como `currentDialogIndex` o `inDialog`, se realiza a través de las lógicas internas de los métodos privados y los métodos de Unity (`Start`, `Update`). Todas las variables relevantes para la configuración externa (`dialogPanel`, `namePanel`, `textPanel`, `target`, `maxDistance`) son variables serializadas que se configuran directamente desde el editor de Unity.
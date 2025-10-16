# `DialogManager`
Este script se encarga de la gestión y visualización de los diálogos en el juego. Su función principal es controlar la interacción del jugador con puntos de diálogo (`target`), activar un panel de interfaz de usuario para mostrar el nombre del personaje y el texto del diálogo, y permitir al jugador avanzar o finalizar estas conversaciones. Se integra con el sistema global del juego (`GameState`) para determinar qué diálogos deben mostrarse en función del estado actual del juego y para notificar cuándo una secuencia de diálogo ha concluido, lo que puede desencadenar una transición al siguiente estado del juego.

El `DialogManager` es fundamental para la narrativa y la progresión del juego, ya que es el componente que facilita la comunicación con el jugador a través de los personajes y sus intervenciones. Dada la naturaleza del juego, enfocada en la experiencia de jugador y la narrativa educativa, este script es un pilar para presentar la personalidad de los animales y las facultades de la UNAL.

# Métodos

## Métodos de Unity

### `Start()`
Este método se invoca una vez al inicio, antes de la primera actualización del frame, después de que el objeto que contiene este script ha sido instanciado.
Su propósito es inicializar el script:
1.  Obtiene una referencia al componente `Target` adjunto al `Transform` asignado a la variable `target`. Esta referencia es crucial para poder activar o desactivar el comportamiento del `target` (por ejemplo, el movimiento del jugador o la interacción de un NPC) durante un diálogo.
    ```csharp
    targetScript = target.GetComponent<Target>();
    ```
2.  Desactiva inicialmente el panel de diálogo (`dialogPanel`) para asegurar que no se muestre al comienzo del juego si no hay un diálogo activo.
    ```csharp
    dialogPanel?.SetActive(false);
    ```

### `Update()`
Este método se llama una vez por cada frame del juego. Es el corazón de la lógica de detección de interacción y manejo de entrada del jugador para los diálogos.
1.  **Detección de distancia:** Calcula la distancia entre la posición del objeto que tiene este `DialogManager` y la posición del `target`. Luego, verifica si esta distancia es menor o igual a `maxDistance`.
    ```csharp
    float distance = Vector3.Distance(transform.position, target.position);
    bool targetInRange = distance <= maxDistance;
    ```
2.  **Lógica de inicio y avance de diálogo:** Utiliza una estructura `if-else if` para manejar dos escenarios principales con la tecla `Z`:
    *   **Iniciar un diálogo:** Si el juego no está en un diálogo (`!inDialog`), el `target` está en el rango (`targetInRange`) y el jugador presiona la tecla `Z` (`Input.GetKeyDown(KeyCode.Z)`), el método `ShowDialogPanel()` es llamado para iniciar la conversación.
        ```csharp
        if (!inDialog && targetInRange && Input.GetKeyDown(KeyCode.Z))
        {
            ShowDialogPanel();
        }
        ```
    *   **Avanzar diálogo:** Si ya está en un diálogo (`inDialog`) y el jugador presiona `Z`, incrementa `currentDialogIndex` para pasar a la siguiente línea y llama a `DisplayNextDialog()` para mostrarla.
        ```csharp
        else if (inDialog)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                currentDialogIndex++;
                DisplayNextDialog();
            }
        }
        ```
3.  **Finalizar diálogo prematuramente:** También, si el juego está en un diálogo (`inDialog`) y el jugador presiona la tecla `C` (`Input.GetKeyDown(KeyCode.C)`), el método `EndDialog()` es invocado para terminar la conversación de forma anticipada.
    ```csharp
    if (Input.GetKeyDown(KeyCode.C)) EndDialog();
    ```
    Esta separación en `if` e `else if` para las interacciones con `Z` es importante para evitar que, al presionar `Z`, se intente iniciar un diálogo y avanzar una línea en el mismo frame si las condiciones lo permitieran, lo que podría generar comportamientos inesperados (el "bug del segundo panel" mencionado en los comentarios del código).

## Otros métodos

### `ShowDialogPanel()`
`void ShowDialogPanel()`
Este método se encarga de preparar y activar la interfaz de usuario de diálogo.
1.  Verifica si ya hay un diálogo activo (`inDialog`). Si es así, el método retorna para evitar reiniciar el diálogo accidentalmente.
    ```csharp
    if (inDialog) return;
    ```
2.  Establece `inDialog` a `true`, indica que un diálogo ha comenzado.
3.  Activa el `dialogPanel` para hacerlo visible en la pantalla.
4.  Desactiva el `targetScript`, lo que probablemente detiene el movimiento del jugador o la interacción del `target` mientras el diálogo está activo.
    ```csharp
    inDialog = true;
    dialogPanel.SetActive(true);
    targetScript.enabled = false;
    ```
5.  Inicializa `currentDialogIndex` a `0` para asegurar que el diálogo comience desde la primera línea.
6.  Llama a `DisplayNextDialog()` para mostrar la primera línea del diálogo.
    ```csharp
    currentDialogIndex = 0;
    DisplayNextDialog();
    ```

### `GetDialogsForState()`
`Dialogs[] GetDialogsForState()`
Este método es responsable de obtener el conjunto de diálogos apropiado para el estado actual del juego.
1.  Intenta cargar el contenido del archivo de diálogos desde `GameState.Singleton.dialogFileContent`. Se realizan dos comprobaciones para asegurar que el contenido esté disponible. Si no se puede obtener, devuelve un array vacío de `Dialogs` para evitar errores.
    ```csharp
    DialogFile dialogFileContent = GameState.Singleton.dialogFileContent;
    if (dialogFileContent == null) dialogFileContent = GameState.Singleton.dialogFileContent; // Posible reintento o redundancia
    if (dialogFileContent == null) return new Dialogs[0];
    ```
2.  Utiliza una sentencia `switch` para determinar qué array de `Dialogs` debe devolver, basándose en el valor de `GameState.Singleton.CurrentGameState`. Esto permite al `DialogManager` mostrar diálogos contextuales para diferentes fases del juego (Inicio, PreJuego, Victoria, Derrota, Repetir).
    ```csharp
    switch (GameState.Singleton.CurrentGameState)
    {
        case GameStates.Begin:
            return dialogFileContent.BeginDialogs;
        case GameStates.PreGame:
            return dialogFileContent.PreGameDialogs;
        // ... otros casos ...
        default:
            return new Dialogs[0];
    }
    ```
    Este diseño es flexible y permite una fácil adición de nuevos estados de juego y sus correspondientes diálogos, sin modificar la lógica principal de manejo de diálogos.

### `DisplayNextDialog()`
`void DisplayNextDialog()`
Este método se encarga de mostrar la siguiente línea de diálogo en la interfaz de usuario.
1.  Obtiene la lista de diálogos relevantes para el estado actual del juego llamando a `GetDialogsForState()`.
    ```csharp
    var dialogs = GetDialogsForState();
    ```
2.  Verifica si `currentDialogIndex` es menor que la longitud del array `dialogs`. Si hay más líneas de diálogo para mostrar:
    *   Actualiza el `namePanel.text` con el nombre del personaje (`dialogs[currentDialogIndex].character`).
    *   Actualiza el `textPanel.text` con el texto de la línea de diálogo (`dialogs[currentDialogIndex].text`).
    ```csharp
    if (currentDialogIndex < dialogs.Length)
    {
        namePanel.text = dialogs[currentDialogIndex].character;
        textPanel.text = dialogs[currentDialogIndex].text;
    }
    ```
3.  Si `currentDialogIndex` es igual o mayor que la longitud del array, significa que se han mostrado todas las líneas de diálogo para el estado actual. En este caso, llama a `EndDialog()` para finalizar la secuencia.
    ```csharp
    else
    {
        EndDialog();
    }
    ```

### `EndDialog()`
`void EndDialog()`
Este método se invoca para finalizar una secuencia de diálogo y restaurar el estado normal del juego.
1.  Establece `inDialog` a `false`, indicando que ya no hay un diálogo activo.
2.  Desactiva el `dialogPanel` para ocultarlo de la pantalla.
3.  Rehabilita el `targetScript` para permitir el movimiento del jugador o la interacción del `target` nuevamente.
    ```csharp
    inDialog = false;
    dialogPanel.SetActive(false);
    targetScript.enabled = true;
    ```
4.  Reinicia `currentDialogIndex` a `0`, dejando el script listo para una nueva secuencia de diálogo.
5.  Notifica al `GameState.Singleton` que debe avanzar al siguiente estado del juego, pasando el estado actual como parámetro. Esto es crucial para la progresión del juego después de que un diálogo importante ha finalizado.
    ```csharp
    GameState.Singleton.NextGameState(GameState.Singleton.CurrentGameState);
    ```

## Getters y Setters
Aunque este script no expone propiedades públicas con `get` y `set` explícitos, los campos `[SerializeField]` actúan como "setters" configurables desde el Inspector de Unity, mientras que las variables privadas gestionan el estado interno.

1.  `dialogPanel` (GameObject): Establece el objeto `GameObject` que se usa como contenedor para toda la interfaz de usuario del diálogo.
2.  `namePanel` (TextMeshProUGUI): Establece el componente `TextMeshProUGUI` donde se mostrará el nombre del personaje que habla.
3.  `textPanel` (TextMeshProUGUI): Establece el componente `TextMeshProUGUI` donde se mostrará el texto del diálogo.
4.  `target` (Transform): Establece el objeto `Transform` (normalmente el jugador o un NPC) cuya distancia se monitoriza para activar el diálogo y cuyo script `Target` se deshabilita/habilita.
5.  `maxDistance` (float): Establece la distancia máxima a la que el jugador debe estar del `target` para que se pueda iniciar un diálogo.
6.  `currentDialogIndex` (int): Almacena y gestiona el índice de la línea de diálogo actual dentro de una secuencia, avanzando internamente por el script.
7.  `inDialog` (bool): Almacena y gestiona un indicador booleano que determina si el juego está actualmente en un estado de diálogo activo.
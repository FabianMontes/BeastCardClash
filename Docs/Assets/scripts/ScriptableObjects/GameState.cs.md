# GameState
El script `GameState` es un componente fundamental en el proyecto `Beast Card Clash`, actuando como el **administrador global del estado del juego**. Implementa el patrón Singleton para asegurar que solo exista una instancia activa en todo momento, persistiendo entre escenas. Su función principal es **centralizar y controlar aspectos clave** como el estado actual del juego (`GameStates`), el idioma seleccionado (`Languages`), los datos del jugador (nombre, skin, equipo) y la gestión de los diálogos del juego.

Este script es el punto de control para las transiciones entre los diferentes estados y escenas del juego, asegurando una experiencia coherente y permitiendo a otros componentes consultar y, en algunos casos, modificar el estado global de forma controlada. Además, es responsable de cargar dinámicamente los archivos de diálogos del juego en el idioma seleccionado, procesándolos desde archivos `TextAsset` en formato JSON.

El `GameState` es esencial para mantener la consistencia en la narrativa y la progresión, adaptándose a las decisiones del jugador y gestionando los recursos necesarios para cada fase del juego, desde el inicio hasta el fin de una partida, pasando por las secuencias de victoria o derrota. Su diseño busca simplificar la gestión de estados complejos y datos globales, priorizando la facilidad de uso para los desarrolladores en un proyecto indie donde la agilidad es clave para resaltar elementos culturales y naturales de Colombia, como los animales autóctonos y las facultades de la UNAL que inspiran a los personajes.

# Métodos

## Métodos de Unity

### OnEnable
Este método se ejecuta cuando el objeto que contiene el script se habilita (justo antes de `Start` y `Awake` si el objeto ya está activo al iniciar la escena). Su propósito principal es asegurar que `GameState` funcione como un **Singleton** y realizar la inicialización inicial del estado del juego y los recursos asociados.

```csharp
private void OnEnable()
{
    // Mantiene la instancia única del GameState
    if (singleton == null)
    {
        singleton = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
    }

    // Establece el equipo (por defecto: Ingeniosos)
    team = Team.ingeniosos;

    // Carga el archivo de diálogos
    LoadDialogFile();

    // Establece el estado actual del juego
    CurrentGameState = gameState;
    language = Languages.spanish;
}
```

**Funcionamiento detallado:**
1.  **Implementación del Singleton:** Verifica si la instancia `singleton` ya existe. Si no, asigna la instancia actual (`this`) y usa `DontDestroyOnLoad(gameObject)` para que el objeto persista entre cargas de escena. Si ya existe otra instancia, destruye el objeto actual para garantizar una única instancia global en todo el ciclo de vida del juego.
2.  **Inicialización de `team`:** Establece el `team` del jugador a `Team.ingeniosos` por defecto. Se asume que `Team` es una enumeración definida en otro script y `ingeniosos` es uno de sus valores. Esta configuración inicial puede ser sobrescrita más adelante por la selección del jugador.
3.  **Carga de diálogos:** Llama al método `LoadDialogFile()` para cargar el archivo de diálogos correspondiente al idioma inicial (que se establece en el siguiente paso). Esto asegura que los diálogos estén disponibles desde el primer momento.
4.  **Establecimiento del estado inicial:** Inicializa la propiedad `CurrentGameState` con el valor de la variable `gameState`, que puede ser configurada desde el Inspector para pruebas o para iniciar el juego en un estado específico.
5.  **Establecimiento del idioma inicial:** Fuerza el idioma actual (`language`) a `Languages.spanish`. Esto significa que, aunque la variable `language` pueda tener un valor por defecto en el Inspector, en tiempo de ejecución siempre comenzará en español.

## Otros métodos

### LoadDialogFile()
`private void LoadDialogFile()`

Este método es responsable de seleccionar y cargar el archivo de diálogos adecuado según el `CurrentLanguage` y de parsear su contenido para que sea accesible en el juego a través de la propiedad `dialogFileContent`.

```csharp
private void LoadDialogFile()
{
    // Carga el idioma elegido
    selectedFile = (language == Languages.spanish) ? spanishFile : englishFile;

    // Intenta cargar el archivo de diálogos correspondiente
    // Si no hay, alerta del error en consola y establece el contenido en null
    if (selectedFile != null)
    {
        dialogFileContent = JsonUtility.FromJson<DialogFile>(selectedFile.text);
    }
    else
    {
        dialogFileContent = null;
        Debug.LogError("¡Error en GameState! No se ha asignado un archivo de diálogos en el Inspector.");
    }
}
```

**Funcionamiento detallado:**
1.  **Selección del archivo:** Evalúa el valor de la variable `language` (el idioma actual). Si es `Languages.spanish`, asigna `spanishFile` a `selectedFile`; de lo contrario, asigna `englishFile`. Estos `TextAsset` (`spanishFile` y `englishFile`) deben estar asignados previamente en el Inspector de Unity y contener datos en formato JSON.
2.  **Deserialización JSON:** Si `selectedFile` no es nulo (es decir, se ha asignado un archivo en el Inspector), utiliza `JsonUtility.FromJson<DialogFile>(selectedFile.text)` para parsear el contenido de texto del `TextAsset` (que se espera sea un JSON válido) en una instancia del objeto `DialogFile`. Este objeto `dialogFileContent` contendrá arrays de `Dialogs` clasificados por `GameStates`, permitiendo un acceso estructurado a los diálogos.
3.  **Manejo de errores:** Si `selectedFile` es nulo, significa que no se ha asignado un archivo de diálogos para el idioma correspondiente en el Inspector. En este caso, `dialogFileContent` se establece en `null` y se registra un error en la consola de Unity. Esta alerta es crítica para identificar configuraciones faltantes que podrían detener la progresión narrativa del juego.

### NextGameState(GameStates newState)
`public void NextGameState(GameStates newState)`

Este método público permite a otros componentes del juego solicitar un cambio en el `CurrentGameState` del juego. Dependiendo del nuevo estado solicitado, el método realiza acciones específicas, principalmente transiciones de escena y actualizaciones del `CurrentGameState`, orquestando la progresión del juego.

```csharp
public void NextGameState(GameStates newState)
{
    switch (newState)
    {
        // begin: pasa a preGame
        case GameStates.begin:
            CurrentGameState = GameStates.preGame;
            break;
        // preGame: salta a la escena de batalla
        case GameStates.preGame:
            SceneManager.LoadScene(3);
            break;
        // Win: pasa a la escena del mundo y establece el estado en repeat
        case GameStates.win:
            SceneManager.LoadScene(2);
            CurrentGameState = GameStates.repeat;
            break;
        // Lose: pasa a la escena de mundo y establece el estado en repeat
        case GameStates.lose:
            SceneManager.LoadScene(2);
            CurrentGameState = GameStates.repeat;
            break;
        // repeat: vuelve a la escena de batalla
        case GameStates.repeat:
            SceneManager.LoadScene(3);
            break;
        // Por defecto: no hace nada
        default:
            break;
    }
}
```

**Funcionamiento detallado:**
Utiliza una estructura `switch` para gestionar diferentes transiciones de estado, permitiendo una lógica clara para la navegación entre las fases del juego:
*   **`GameStates.begin`**: Cambia el `CurrentGameState` a `GameStates.preGame` sin cargar una nueva escena. Esto sugiere que esta transición puede ser para configurar la partida o mostrar una secuencia inicial antes de la acción principal del juego, como la selección de personajes o tutoriales.
*   **`GameStates.preGame`**: Carga la escena con el índice `3`. Basado en el contexto de `Beast Card Clash`, esto probablemente corresponde a la escena de "batalla" o jugabilidad principal donde los jugadores se enfrentan usando sus cartas.
*   **`GameStates.win`**: Carga la escena con el índice `2` (posiblemente la escena del "mundo", un resumen post-partida o una pantalla de victoria) y luego establece el `CurrentGameState` a `GameStates.repeat`, indicando que el juego ha finalizado una partida pero está listo para una repetición o un nuevo ciclo de juego.
*   **`GameStates.lose`**: Similar a `GameStates.win`, carga la escena `2` y luego establece el `CurrentGameState` a `GameStates.repeat`. Esta lógica compartida simplifica la gestión de los estados finales de una partida.
*   **`GameStates.repeat`**: Carga la escena con el índice `3`, lo que implica que desde el estado "repeat" (después de una victoria o derrota) se vuelve directamente a la escena de batalla. Esto facilita reintentar partidas o pasar a la siguiente sin pasos intermedios, alineándose con la agilidad del desarrollo indie.
*   **`default`**: No realiza ninguna acción para cualquier otro estado no especificado, evitando errores si se pasa un `GameStates` no contemplado y manteniendo la estabilidad del sistema.

### SetLanguage(Languages newLanguage)
`public void SetLanguage(Languages newLanguage)`

Permite cambiar el idioma del juego en tiempo de ejecución, actualizando tanto la preferencia de idioma como los diálogos cargados.

```csharp
public void SetLanguage(Languages newLanguage)
{
    language = newLanguage;

    // Recargamos los diálogos con el nuevo idioma
    LoadDialogFile();
}
```

**Funcionamiento detallado:**
1.  Actualiza la variable privada `language` con el `newLanguage` proporcionado.
2.  Llama inmediatamente a `LoadDialogFile()` para recargar los diálogos. Esto asegura que el contenido de `dialogFileContent` refleje ahora el idioma recién seleccionado, lo que es crucial para la interfaz de usuario y la narrativa del juego.

### SetSkin(int newSkin)
`public void SetSkin(int newSkin)`

Establece el valor entero que representa la skin del jugador.

```csharp
public void SetSkin(int newSkin)
{
    skin = newSkin;
}
```

**Funcionamiento detallado:**
Actualiza la propiedad `skin` del jugador con el valor `newSkin`. Este valor `int` probablemente se utiliza para referenciar un asset de skin o un índice en una lista de skins disponibles para los personajes basados en animales autóctonos.

### SetTeam(Team team)
`public void SetTeam(Team team)`

Establece el equipo al que pertenece el jugador.

```csharp
public void SetTeam(Team team)
{
    this.team = team;
}
```

**Funcionamiento detallado:**
Actualiza la propiedad `team` del jugador con el valor de la enumeración `Team` proporcionado. Esto es relevante para las mecánicas de juego que dependen de la afiliación a un equipo, como las facultades de la UNAL que inspiran el proyecto.

### SetPlayer(string name)
`public void SetPlayer(string name)`

Establece el nombre del jugador.

```csharp
public void SetPlayer(string name)
{
    this.playerName = name;
}
```

**Funcionamiento detallado:**
Actualiza la propiedad `playerName` del jugador con el `name` (string) proporcionado. Este nombre puede ser utilizado en diálogos, marcadores o elementos de la interfaz de usuario para personalizar la experiencia del jugador.

## Getters y Setters

1.  `CurrentGameState` (GameStates): Proporciona el estado actual del juego. Es de solo lectura público, permitiendo a otros scripts consultar la fase actual del juego, y su valor se gestiona internamente por el script `GameState`.
2.  `CurrentLanguage` (Languages): Proporciona el idioma actualmente seleccionado para el juego. Es de solo lectura público y se obtiene de la variable `language`, lo que permite a los componentes de UI o texto adaptarse al idioma correcto.
3.  `singleton` (GameState): Proporciona la instancia única global del `GameState`. Permite a otros scripts acceder a las funcionalidades y datos gestionados por este Singleton sin necesidad de referencias directas en la jerarquía.
4.  `playerName` (string): Proporciona el nombre del jugador. Es de solo lectura público y su valor se establece externamente mediante el método `SetPlayer`.
5.  `skin` (int): Proporciona el identificador numérico de la skin del jugador. Es de solo lectura público y su valor se establece externamente mediante el método `SetSkin`.
6.  `team` (Team): Proporciona el equipo al que pertenece el jugador. Es de solo lectura público y su valor se establece externamente mediante el método `SetTeam`.
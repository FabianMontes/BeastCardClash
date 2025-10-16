# `GameState`
`GameState` es un componente de Unity fundamental para la gestión centralizada del estado del juego, la configuración global y los datos del jugador en "Beast Card Clash". Implementa el patrón Singleton, asegurando que solo exista una instancia de sí mismo a lo largo de todas las escenas, lo que permite un acceso fácil y global desde cualquier otro script.

Este script es el custodio de información crítica como el idioma actual del juego, el nombre y las características seleccionadas por el jugador (como la `Skin` y el `Team`), y lo más importante, el `CurrentGameState` del juego (Inicio, Previo a la Batalla, Victoria, Derrota, o Repetición). También se encarga de cargar y gestionar los archivos de diálogos del juego, adaptándose al idioma seleccionado.

El funcionamiento de `GameState` se basa en:
*   **Gestión del Singleton:** Asegura una única instancia global que persiste entre escenas.
*   **Definición de Enums:** Establece enumeraciones clave (`Languages`, `GameStates`) para tipificar los estados y configuraciones del juego de forma clara.
*   **Almacenamiento de Diálogos:** Utiliza clases serializables (`DialogFile`, `Dialogs`) para estructurar y cargar el contenido de los diálogos desde archivos JSON (TextAsset), permitiendo una fácil localización.
*   **Control del Flujo del Juego:** Proporciona un método (`NextGameState`) para cambiar el estado del juego, que a su vez puede desencadenar la carga de nuevas escenas.
*   **Configuración del Jugador:** Ofrece métodos para establecer el nombre, la apariencia (`Skin`) y el equipo (`Team`) del jugador, datos que se mantendrán disponibles globalmente.

De esta manera, `GameState` actúa como un "cerebro" central que coordina aspectos clave de la experiencia de jugador y el flujo del juego, facilitando que otros sistemas consulten o modifiquen estos datos de manera consistente. Su diseño con propiedades públicas y métodos de mutación (`Set...`) busca optimizar la experiencia de desarrollo, permitiendo una interacción directa y comprensible con los datos del juego.

# Métodos

## Métodos de Unity

### `OnEnable()`
Este método se ejecuta cuando el objeto `GameObject` al que está adjunto el script `GameState` se activa. Su propósito principal es inicializar la instancia Singleton y configurar el estado inicial del juego.

```csharp
private void OnEnable()
{
    // Mantiene la instancia única del GameState
    if (Singleton == null)
    {
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
    }

    // Establece el equipo (por defecto: Ingeniosos)
    Team = Team.Ingeniosos;

    // Carga el archivo de diálogos
    LoadDialogFile();

    // Establece el estado actual del juego
    CurrentGameState = gameState;
    language = Languages.Spanish;
}
```

**Funcionamiento:**
1.  **Inicialización Singleton:** Comprueba si ya existe una instancia de `GameState`. Si no existe (`Singleton == null`), establece la instancia actual (`this`) como `Singleton` y utiliza `DontDestroyOnLoad(gameObject)` para asegurar que este `GameObject` persista a través de las cargas de escenas, evitando que se destruya. Si ya existe una instancia, destruye el `GameObject` actual para mantener una única instancia del `GameState`.
2.  **Configuración del Equipo por Defecto:** Establece el `Team` del jugador en `Team.Ingeniosos` como valor predeterminado al inicio del juego.
3.  **Carga de Diálogos:** Llama a `LoadDialogFile()` para cargar el archivo de diálogos correspondiente al idioma inicial (por defecto, Español).
4.  **Establecimiento del Estado y Lenguaje Inicial:** `CurrentGameState` se inicializa con el valor `gameState` (que puede ser configurado en el Inspector de Unity para pruebas), y `language` se fuerza a `Languages.Spanish` como idioma inicial.

## Otros métodos

### `LoadDialogFile()`
Este método es responsable de cargar el archivo de diálogos adecuado según el `CurrentLanguage` seleccionado y deserializar su contenido del formato JSON a la estructura de datos `DialogFile`.

```csharp
private void LoadDialogFile()
{
    // Carga el idioma elegido
    _selectedFile = language == Languages.Spanish ? spanishFile : englishFile;

    // Intenta cargar el archivo de diálogos correspondiente. Si no hay, alerta del error en consola y establece el contenido en null
    if (_selectedFile != null)
    {
        dialogFileContent = JsonUtility.FromJson<DialogFile>(_selectedFile.text);
    }
    else
    {
        dialogFileContent = null;
        Debug.LogError("¡Error en GameState! No se ha asignado un archivo de diálogos en el Inspector.");
    }
}
```

**Funcionamiento:**
1.  **Selección de Archivo:** Utiliza un operador ternario para asignar `_selectedFile` al `spanishFile` o `englishFile` según el valor actual de `language`.
2.  **Deserialización JSON:** Si el `_selectedFile` no es nulo (es decir, se ha asignado un `TextAsset` en el Inspector), utiliza `JsonUtility.FromJson<DialogFile>(_selectedFile.text)` para parsear el contenido JSON del `TextAsset` y guardarlo en la variable pública `dialogFileContent`. Esto permite que otros scripts accedan fácilmente a todos los diálogos del juego.
3.  **Manejo de Errores:** Si `_selectedFile` es nulo, indica que no se ha asignado un archivo de diálogos en el Inspector, establece `dialogFileContent` a `null` y emite un mensaje de error en la consola para notificar al desarrollador.

### `NextGameState(GameStates newState)`
Este método es la interfaz principal para cambiar el estado del juego. Permite a otros scripts solicitar una transición a un nuevo `GameStates`, y `GameState` se encarga de la lógica asociada, incluyendo la carga de escenas.

```csharp
public void NextGameState(GameStates newState)
{
    switch (newState)
    {
        // begin: pasa a preGame
        case GameStates.Begin:
            CurrentGameState = GameStates.PreGame;
            break;
        // preGame: salta a la escena de batalla
        case GameStates.PreGame:
            SceneManager.LoadScene(3);
            break;
        // Win y Lose: pasa a la escena del mundo y establece el estado en repeat
        case GameStates.Win:
        case GameStates.Lose:
            SceneManager.LoadScene(2);
            CurrentGameState = GameStates.Repeat;
            break;
        // repeat: vuelve a la escena de batalla
        case GameStates.Repeat:
            SceneManager.LoadScene(3);
            break;
        // Por defecto (error hipotético): carga la escena de mundo y pasa a repeat
        default:
            SceneManager.LoadScene(2);
            CurrentGameState = GameStates.Repeat;
            break;
    }
}
```

**Funcionamiento:**
El método utiliza una estructura `switch` para manejar las transiciones entre los diferentes `GameStates`:
*   Si `newState` es `Begin`, el juego transiciona internamente a `PreGame`.
*   Si `newState` es `PreGame`, se carga la `Scene` con índice `3` (presumiblemente la escena de batalla).
*   Si `newState` es `Win` o `Lose`, se carga la `Scene` con índice `2` (probablemente la escena del mapa/mundo) y el estado interno se establece en `Repeat`.
*   Si `newState` es `Repeat`, se vuelve a cargar la `Scene` con índice `3` (escena de batalla), lo que permite reintentar una partida.
*   En caso de un `default` o estado inesperado, se asume un retorno a la escena del mundo (`Scene 2`) y se establece el estado en `Repeat`.

Este método es crucial para orquestar el flujo general del juego, permitiendo que la narrativa y la jugabilidad avancen de manera controlada.

### `SetLanguage(Languages newLanguage)`
Permite cambiar el idioma del juego en tiempo de ejecución.

```csharp
public void SetLanguage(Languages newLanguage)
{
    language = newLanguage;

    // Recargamos los diálogos con el nuevo idioma
    LoadDialogFile();
}
```

**Funcionamiento:**
Actualiza la variable `language` con el `newLanguage` proporcionado y luego llama a `LoadDialogFile()` para recargar el `TextAsset` de diálogos correspondiente al nuevo idioma, asegurando que los diálogos mostrados al jugador estén en el idioma correcto.

### `SetSkin(int newSkin)`
Establece el identificador de la `skin` (apariencia) seleccionada por el jugador.

```csharp
public void SetSkin(int newSkin)
{
    Skin = newSkin;
}
```

**Funcionamiento:**
Asigna el valor `newSkin` a la propiedad `Skin`, que luego puede ser utilizada por otros componentes del juego para representar visualmente al jugador con la apariencia elegida.

### `SetTeam(Team team)`
Establece el `Team` (equipo) al que pertenece el jugador.

```csharp
public void SetTeam(Team team)
{
    this.Team = team;
}
```

**Funcionamiento:**
Asigna el `Team` proporcionado a la propiedad `Team` del jugador. Aunque en `OnEnable` se establece un equipo por defecto, este método permite cambiarlo, por ejemplo, a través de una selección en la interfaz de usuario. Es importante notar que `Team` es una enumeración o clase definida externamente a este script.

### `SetPlayer(string newName)`
Establece el nombre del jugador.

```csharp
public void SetPlayer(string newName)
{
    this.PlayerName = newName;
}
```

**Funcionamiento:**
Asigna el `newName` proporcionado a la propiedad `PlayerName`. Esto es útil para personalizar la experiencia de juego, permitiendo que el nombre del jugador se muestre en diálogos, marcadores, etc.

## Getters y Setters

1.  `CurrentGameState`: Proporciona el estado actual del juego (`GameStates`). Es de solo lectura público, su valor se modifica internamente a través de `NextGameState()`.
2.  `CurrentLanguage`: Proporciona el idioma actualmente seleccionado en el juego (`Languages`). Es de solo lectura público, su valor se modifica a través de `SetLanguage()`.
3.  `PlayerName`: Proporciona el nombre elegido por el jugador (`string`). Es de solo lectura público, su valor se modifica a través de `SetPlayer()`.
4.  `Skin`: Proporciona el identificador de la skin (apariencia) seleccionada por el jugador (`int`). Es de solo lectura público, su valor se modifica a través de `SetSkin()`.
5.  `Team`: Proporciona el equipo al que pertenece el jugador (`Team`). Es de solo lectura público, su valor se modifica a través de `SetTeam()`.
6.  `dialogFileContent`: Proporciona el contenido completo de los diálogos cargados, estructurado en un objeto `DialogFile`. Contiene arrays de `Dialogs` para cada etapa del juego.
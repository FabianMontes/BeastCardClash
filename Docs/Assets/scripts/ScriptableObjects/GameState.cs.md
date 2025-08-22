# `GameState.cs`

## 1. Propósito General
Este script es el gestor central del estado global del juego "Beast Card Clash". Su rol principal es controlar el flujo de la partida, manejar el idioma actual, almacenar datos persistentes del jugador (nombre, skin, equipo) y cargar los diálogos apropiados. Interactúa directamente con el sistema de escenas de Unity para transiciones entre diferentes partes del juego.

## 2. Componentes Clave

### `Languages`
- **Descripción:** Una enumeración simple que define los idiomas disponibles en el juego. Sus valores corresponden a las opciones de localización para los diálogos.

### `GameStates`
- **Descripción:** Una enumeración que define los diferentes estados lógicos por los que puede pasar el juego. Estos estados son utilizados por el gestor para controlar el flujo y las acciones a realizar, como la carga de escenas o la visualización de diálogos específicos.

### `DialogFile`
- **Descripción:** Una clase serializable que actúa como un contenedor para arrays de objetos `Dialogs`. Está diseñada para mapear la estructura de un archivo JSON que contiene todos los diálogos del juego, clasificándolos por el estado del juego en el que deben aparecer. Esto permite cargar fácilmente todos los diálogos relevantes para un idioma y estado específicos.
- **Variables Públicas / Serializadas:**
    - `BeginDialogs`: Un array de objetos `Dialogs` que contiene los diálogos correspondientes al estado `GameStates.begin`.
    - `PreGameDialogs`: Un array de objetos `Dialogs` que contiene los diálogos correspondientes al estado `GameStates.preGame`.
    - `WinDialogs`: Un array de objetos `Dialogs` que contiene los diálogos correspondientes al estado `GameStates.win`.
    - `LoseDialogs`: Un array de objetos `Dialogs` que contiene los diálogos correspondientes al estado `GameStates.lose`.
    - `RepeatDialogs`: Un array de objetos `Dialogs` que contiene los diálogos correspondientes al estado `GameStates.repeat`.

### `Dialogs`
- **Descripción:** Una clase serializable que representa una única línea o entrada de diálogo. Cada instancia de esta clase define quién está hablando y el contenido textual de lo que se dice.
- **Variables Públicas / Serializadas:**
    - `character`: Un `string` que indica el nombre del personaje que pronuncia esta línea de diálogo.
    - `text`: Un `string` que contiene el contenido textual del diálogo en sí.

### `GameState`
- **Descripción:** Esta es la clase principal del archivo y hereda de `MonoBehaviour`, lo que significa que puede ser adjuntada a un `GameObject` en la escena de Unity. Implementa un patrón Singleton para asegurar que solo exista una instancia de `GameState` a lo largo de toda la aplicación, y que esta persista entre las cargas de escenas. Su responsabilidad abarca la gestión del estado global del juego, la selección del idioma, el almacenamiento de datos del jugador y la orquestación de la carga de diálogos y escenas.
- **Variables Públicas / Serializadas:**
    - `CurrentGameState` (`GameStates`): Propiedad pública de solo lectura (`get`) que proporciona el estado actual del juego. Su valor se actualiza internamente a través del método `NextGameState`.
    - `CurrentLanguage` (`Languages`): Propiedad pública de solo lectura que permite a otros scripts conocer el idioma que está actualmente seleccionado para el juego.
    - `singleton` (`static GameState`): Una referencia estática a la única instancia de la clase `GameState`. Esta es la forma principal de acceder a este gestor desde cualquier otro script en el proyecto.
    - `playerName` (`string`): Propiedad pública de solo lectura que almacena el nombre que el jugador ha introducido.
    - `skin` (`int`): Propiedad pública de solo lectura que guarda un identificador numérico para la skin (apariencia) que el jugador ha seleccionado para su personaje.
    - `team` (`Team`): Propiedad pública de solo lectura que almacena el equipo al que pertenece el jugador. (La definición del enumerador `Team` no se encuentra en este archivo).
    - `gameState` (`[SerializeField] private GameStates`): Una variable privada que se serializa para ser visible y configurable en el Inspector de Unity. Se usa para establecer el estado inicial del juego, principalmente para propósitos de depuración o pruebas en el editor.
    - `language` (`[SerializeField] private Languages`): Similar a `gameState`, esta variable privada es serializada y configurable en el Inspector, permitiendo preestablecer el idioma inicial del juego.
    - `spanishFile` (`[SerializeField] private TextAsset`): Una referencia a un archivo `TextAsset` (que debería contener JSON) donde se almacenan todos los diálogos en español del juego. Debe ser asignada desde el Inspector de Unity.
    - `englishFile` (`[SerializeField] private TextAsset`): Una referencia a un archivo `TextAsset` similar al anterior, pero para los diálogos en inglés. También debe ser asignada en el Inspector.
    - `dialogFileContent` (`public DialogFile`): Un objeto público que contiene todos los diálogos del juego, parseados del archivo `TextAsset` seleccionado (`spanishFile` o `englishFile`) en la estructura de la clase `DialogFile`.

- **Métodos Principales:**
    - `void OnEnable()`: Este es un método de ciclo de vida de Unity que se llama cuando el objeto se activa. Contiene la lógica central del patrón Singleton: si no hay ninguna instancia de `GameState` (`singleton` es nulo), esta instancia actual se convierte en el `singleton` y se utiliza `DontDestroyOnLoad(gameObject)` para asegurar que persista entre las cargas de escena. Si ya existe un `singleton`, esta nueva instancia se destruye para evitar duplicados. También inicializa el equipo por defecto (`Team.ingeniosos`), invoca `LoadDialogFile()` para cargar los diálogos iniciales, y establece el `CurrentGameState` y `language` a sus valores por defecto o serializados.
    ```csharp
    private void OnEnable()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // ... otras inicializaciones ...
    }
    ```
    - `private void LoadDialogFile()`: Método privado que se encarga de cargar el contenido de los diálogos. Selecciona el `TextAsset` apropiado (`spanishFile` o `englishFile`) basándose en el `CurrentLanguage`. Luego, utiliza `JsonUtility.FromJson<DialogFile>(selectedFile.text)` para deserializar el contenido de texto del `TextAsset` a un objeto `DialogFile` y lo asigna a `dialogFileContent`. Si no se ha asignado un archivo en el Inspector, registra un error en la consola.
    - `public void NextGameState(GameStates newState)`: Método público que permite a otros scripts solicitar un cambio en el estado del juego. Contiene una sentencia `switch` que define las acciones a realizar para cada `GameStates`. Las acciones incluyen actualizar `CurrentGameState` y, crucialmente, cargar nuevas escenas de Unity utilizando `SceneManager.LoadScene` para transicionar entre diferentes partes del juego (ej., de la pantalla de inicio a la batalla, o de la batalla al mundo principal).
    ```csharp
    public void NextGameState(GameStates newState)
    {
        switch (newState)
        {
            case GameStates.begin: // ...
            case GameStates.preGame: // ...
            // ... otros casos de estado y carga de escenas
        }
    }
    ```
    - `public void SetLanguage(Languages newLanguage)`: Un método público que permite cambiar el idioma actual del juego a `newLanguage`. Después de actualizar la variable `language`, invoca `LoadDialogFile()` para recargar los diálogos con el nuevo idioma seleccionado.
    - `public void SetSkin(int newSkin)`: Un método público que establece el valor de la `skin` del jugador al entero `newSkin`.
    - `public void SetTeam(Team team)`: Un método público que establece el equipo del jugador al valor `team` proporcionado.
    - `public void SetPlayer(string name)`: Un método público que establece el nombre del jugador a la cadena `name`.

- **Lógica Clave:**
    La clase `GameState` es fundamental por su implementación del patrón Singleton, garantizando una única instancia global accesible que centraliza la información crucial del juego. El flujo del juego se gestiona a través del método `NextGameState`, que actúa como una máquina de estados implícita. Este método no solo actualiza el estado interno, sino que también orquesta las transiciones de escena mediante `SceneManager.LoadScene`, llevando al jugador a la siguiente fase del juego. La carga de diálogos es dinámica y sensible al idioma, utilizando `JsonUtility` para parsear los datos de `TextAsset`, lo que facilita la localización del juego.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Para su funcionamiento, simplemente debe estar adjunto a un `GameObject` en la primera escena del juego que se mantenga activo y persista a través de las cargas de escena, lo cual es manejado internamente por su lógica Singleton (`DontDestroyOnLoad`).
- **Eventos (Entrada):** Este script no se suscribe explícitamente a eventos (`UnityEvent`) o delegados (`Action`) de otros sistemas. Sus funcionalidades se exponen a través de métodos públicos que deben ser llamados directamente por otros scripts que necesiten interactuar con el estado global o la configuración del juego (ej. `GameState.singleton.NextGameState(...)`).
- **Eventos (Salida):** Este script no invoca sus propios eventos personalizados (`UnityEvent` o `Action`) para notificar a otros sistemas sobre cambios en su estado o en la carga de datos. Sin embargo, sí realiza llamadas directas a `UnityEngine.SceneManagement.SceneManager.LoadScene()`, lo que desencadena el ciclo de vida de los scripts en las nuevas escenas cargadas por Unity.
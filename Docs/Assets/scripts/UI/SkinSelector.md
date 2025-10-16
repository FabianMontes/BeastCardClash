# `SkinSelector`
Este script `SkinSelector` es un componente fundamental para la interacción del jugador con la selección de personajes o "skins" dentro del juego **Beast Card Clash**. Su función principal es permitir a los jugadores elegir una skin específica, representativa de un animal autóctono y una facultad, mediante la interacción visual y de clic en la interfaz. También proporciona retroalimentación visual al jugador a través de efectos de contorno (outline) al pasar el ratón por encima del personaje.

El script se adhiere a un patrón de diseño sencillo y directo, común en proyectos de desarrollo ágil de videojuegos indie, donde la claridad y la inmediatez en el flujo de trabajo para el programador son prioritarias.

## `Variables`

*   `skinIndex` (int):
    Este campo serializado (`[SerializeField]`) representa el índice numérico único asociado a la skin (personaje) que este componente `SkinSelector` representa. Su valor se configura directamente desde el Inspector de Unity, lo que facilita la asignación de diferentes skins a distintos GameObjects sin necesidad de modificar el código. Este índice es crucial para identificar qué personaje ha sido seleccionado por el jugador y comunicarlo al sistema global del juego.
    ```csharp
    [SerializeField] private int skinIndex; // Índice de la skin necesaria
    ```

*   `outline` (`OutlineFx.Outline`):
    Esta variable almacena una referencia al componente `Outline` del paquete OutlineFx. Este componente es responsable de renderizar un contorno visual alrededor del personaje cuando el mouse se posiciona sobre él, proporcionando una señal clara de interacción al jugador. Se inicializa en el método `Start` buscando el componente en los hijos del GameObject actual, lo que permite que el `Outline` pueda estar en un modelo hijo del objeto principal que tiene el `SkinSelector`.
    ```csharp
    Outline outline; // Componente de contorno para el personaje
    ```

# Métodos

## Métodos de Unity

### `Start()`
Este método se llama una vez al inicio cuando el script se activa. Se encarga de la inicialización de los componentes necesarios para el funcionamiento del `SkinSelector`.

1.  **Obtención e inicialización del Outline:**
    Se busca el componente `Outline` en los GameObjects hijos del GameObject actual y se almacena en la variable `outline`. Inmediatamente después, se desactiva este componente (`outline.enabled = false`), asegurando que el contorno no sea visible por defecto al inicio del juego, sino solo cuando el jugador interactúe con el elemento.
    ```csharp
    outline = GetComponentInChildren<Outline>();
    outline.enabled = false;
    ```

2.  **Configuración del Animator:**
    Se obtiene una referencia al componente `Animator` en el mismo GameObject y se establece el parámetro booleano `"isFigthing"` a `true`. Esto sugiere que los modelos de personaje utilizados en la pantalla de selección de skins podrían estar configurados para mostrar una animación de "lucha" o pose de combate por defecto. Esto contribuye a la "experiencia de jugador" al dar vida a los personajes incluso antes de que empiece la partida.
    ```csharp
    GetComponent<Animator>().SetBool("isFigthing", true);
    ```

### `OnMouseDown()`
Este método de evento de Unity se invoca cuando el usuario presiona el botón del ratón mientras el cursor está sobre este GameObject. Su propósito principal es registrar la selección de la skin.

*   **Selección de la skin:**
    Cuando el jugador hace clic sobre el personaje, este método llama a `SetSkin()` del Singleton `GameState`. Le pasa el `skinIndex` que este `SkinSelector` representa. `GameState.Singleton` es un patrón de diseño común para gestionar información global del juego, como la skin actualmente seleccionada, lo que simplifica la comunicación entre diferentes partes del sistema.
    ```csharp
    GameState.Singleton.SetSkin(skinIndex);
    ```

### `OnMouseEnter()`
Este método de evento de Unity se llama cuando el puntero del ratón entra en el *collider* asociado a este GameObject.

*   **Activación del Outline:**
    Para proporcionar una retroalimentación visual al jugador, este método activa el componente `outline` (`outline.enabled = true`), haciendo que el contorno del personaje sea visible mientras el ratón está encima.
    ```csharp
    outline.enabled = true;
    ```

### `OnMouseExit()`
Este método de evento de Unity se llama cuando el puntero del ratón deja de estar sobre el *collider* asociado a este GameObject.

*   **Desactivación del Outline:**
    Complementando a `OnMouseEnter`, este método desactiva el componente `outline` (`outline.enabled = false`), ocultando el contorno del personaje una vez que el puntero del ratón se aleja.
    ```csharp
    outline.enabled = false;
    ```

## Otros métodos
No hay métodos adicionales definidos explícitamente en esta clase que no sean los métodos de evento y ciclo de vida de Unity.

## Getters y Setters
No hay métodos getters o setters definidos explícitamente en esta clase. El campo `skinIndex` se expone para edición a través del Inspector de Unity (`[SerializeField]`) y se utiliza internamente o se pasa a otros sistemas como `GameState`.
# `UIManager.cs`

## 1. Propósito General
Este script tiene como propósito principal gestionar la visibilidad y el flujo entre los diferentes menús de la interfaz de usuario (UI) del juego, como el menú de inicio, créditos y tutorial. Actúa como el controlador central para la navegación entre estas pantallas principales, asegurando que solo un menú esté activo en un momento dado.

## 2. Componentes Clave

### `UIManager`
-   **Descripción:** La clase `UIManager` es un `MonoBehaviour`, lo que significa que puede ser adjuntada a un GameObject en Unity y participar en el ciclo de vida del motor. Su función principal es controlar qué `Canvas` de menú está visible para el jugador en un momento dado, facilitando la navegación a través de la interfaz de usuario inicial del juego.
-   **Variables Públicas / Serializadas:**
    -   `[SerializeField] private Canvas StartMenu;`: Referencia al componente `Canvas` que representa el menú principal del juego. Este campo debe ser asignado en el Inspector de Unity.
    -   `[SerializeField] private Canvas CreditsMenu;`: Referencia al componente `Canvas` que muestra la pantalla de créditos. Al igual que el anterior, se asigna en el Inspector.
    -   `[SerializeField] private Canvas TutorialMenu;`: Referencia al componente `Canvas` que contiene la información del tutorial. También se asigna en el Inspector.

-   **Métodos Principales:**
    -   `void Start()`: Este es un método del ciclo de vida de Unity, invocado una vez al inicio del juego, antes de la primera actualización del frame. Su propósito es inicializar la vista de la UI, asegurando que solo el menú de inicio sea visible cuando el juego arranca. Internamente, llama a `ShowStartMenu()`.

        ```csharp
        void Start()
        {
            ShowStartMenu();
        }
        ```

    -   `void Update()`: Otro método del ciclo de vida de Unity, llamado una vez por frame. En la implementación actual, este método está vacío y no contiene lógica. Esto indica que la gestión de menús se basa principalmente en interacciones de un solo disparo (como clics de botón) en lugar de una lógica continua por frame.

    -   `public void ShowStartMenu()`: Este método es responsable de activar el `Canvas` del menú de inicio y, simultáneamente, desactivar los `Canvas` de los menús de créditos y tutorial. Está diseñado para ser invocado por eventos externos, como el `OnClick` de un botón en la UI.

        ```csharp
        public void ShowStartMenu()
        {
            StartMenu.gameObject.SetActive(true);
            CreditsMenu.gameObject.SetActive(false);
            TutorialMenu.gameObject.SetActive(false);
        }
        ```

    -   `public void ShowCreditsMenu()`: Similar a `ShowStartMenu()`, este método activa el `Canvas` del menú de créditos y desactiva los otros menús. Se espera que sea llamado por un evento de UI.

    -   `public void ShowTutorialMenu()`: Activa el `Canvas` del menú de tutorial y desactiva los otros dos. También está diseñado para la asignación a eventos de UI.

-   **Lógica Clave:**
    La lógica principal de `UIManager` se basa en un mecanismo de "un único menú activo". Cada vez que uno de los métodos `ShowXMenu()` es invocado, el script asegura que el `Canvas` correspondiente a ese menú se active (`SetActive(true)`) mientras que todos los demás `Canvas` gestionados por este script se desactiven (`SetActive(false)`). Esto garantiza que el jugador siempre vea una única pantalla de menú a la vez, evitando superposiciones o estados de UI ambiguos.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere que se le asignen referencias a tres componentes `Canvas` en el Inspector de Unity para poder operar correctamente.

-   **Eventos (Entrada):**
    Los métodos públicos `ShowStartMenu()`, `ShowCreditsMenu()` y `ShowTutorialMenu()` están diseñados para ser suscritos y llamados por eventos de interfaz de usuario de Unity, típicamente el `OnClick()` de botones UI. Esto se configura directamente en el Inspector de Unity, arrastrando el GameObject que contiene el `UIManager` al campo de evento del botón y seleccionando el método deseado.

-   **Eventos (Salida):**
    Este script no invoca ningún `UnityEvent` o `Action` personalizado para notificar a otros sistemas sobre cambios en el estado de la UI. Su interacción se limita a la manipulación directa de la visibilidad de los `Canvas` asignados.
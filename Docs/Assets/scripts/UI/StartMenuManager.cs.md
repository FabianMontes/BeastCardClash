# `StartMenuManager.cs`

## 1. Propósito General
Este script gestiona la interfaz de usuario del menú de inicio, específicamente los textos de los botones y etiquetas. Su rol principal es asegurar que estos elementos de la UI se muestren en el idioma correcto, interactuando directamente con los sistemas de gestión de estado del juego y de textos localizados.

## 2. Componentes Clave

### `StartMenuManager`
-   **Descripción:** `StartMenuManager` es una clase que hereda de `MonoBehaviour`, lo que significa que se adjunta a un GameObject en la escena de Unity y ejecuta lógica durante el ciclo de vida del juego. Su función principal es controlar los elementos textuales del menú inicial, como los botones de "Iniciar" y "Créditos", y la etiqueta de selección de idioma, asegurando que sus textos se actualicen según el idioma seleccionado por el usuario o el idioma predefinido del juego.

-   **Variables Públicas / Serializadas:**
    -   `[SerializeField] private GameState gameState;`: Una referencia al singleton `GameState`. Este objeto es crucial para determinar y almacenar el idioma actual del juego (`CurrentLanguage`). `StartMenuManager` consulta `GameState` para inicializar el idioma al inicio y le notifica cuando el idioma debe cambiar.
    -   `[SerializeField] private MenuTexts menuTexts;`: Una referencia a un `ScriptableObject` de tipo `MenuTexts`. Este activo contiene todas las cadenas de texto traducidas necesarias para el menú de inicio (ej. textos en español e inglés para los botones). `StartMenuManager` utiliza este objeto para obtener las traducciones correctas.
    -   `[SerializeField] private TextMeshProUGUI startButtonText;`: Una referencia al componente `TextMeshProUGUI` que muestra el texto del botón de inicio.
    -   `[SerializeField] private TextMeshProUGUI creditsButtonText;`: Una referencia al componente `TextMeshProProUGUI` que muestra el texto del botón de créditos.
    -   `[SerializeField] private TextMeshProUGUI languagesLabelText;`: Una referencia al componente `TextMeshProProUGUI` que muestra el texto de la etiqueta de idiomas.

-   **Métodos Principales:**
    -   `void Start()`: Este es un método del ciclo de vida de Unity, invocado una vez al comienzo cuando el script está habilitado por primera vez. Su propósito es inicializar los textos de la UI del menú de inicio. Llama a `UpdateUIText` para asegurarse de que los textos se muestren en el `CurrentLanguage` almacenado en `gameState` desde el principio.
        ```csharp
        void Start()
        {
            if (gameState != null) UpdateUIText(gameState.CurrentLanguage);
        }
        ```
    -   `public void SetLanguage(int languageIndex)`: Este método público se expone para ser llamado por eventos externos, como un botón de la UI. Recibe un `languageIndex` (típicamente 0 para español, 1 para inglés) que se convierte al tipo `Languages` (presumiblemente una enumeración). Luego, actualiza el idioma en el `GameState` y procede a refrescar todos los textos de la UI llamando a `UpdateUIText`.
        ```csharp
        public void SetLanguage(int languageIndex)
        {
            Languages newLanguage = (Languages)languageIndex;
            gameState.SetLanguage(newLanguage);
            UpdateUIText(newLanguage);
        }
        ```
    -   `private void UpdateUIText(Languages language)`: Este método privado es la lógica central para la localización de la UI del menú. Toma un valor del `enum Languages` y, basándose en él, asigna las cadenas de texto correspondientes obtenidas de `menuTexts` a los componentes `TextMeshProUGUI` del menú. Incluye una verificación de seguridad para asegurar que el `menuTexts` esté asignado antes de intentar acceder a sus propiedades.
        ```csharp
        private void UpdateUIText(Languages language)
        {
            if (menuTexts == null)
            {
                Debug.LogError("El asset 'MenuTexts' no está asignado en el StartMenuManager.");
                return;
            }
            // Logic to update texts based on 'language'
        }
        ```

-   **Lógica Clave:** La lógica principal del `StartMenuManager` radica en su capacidad para coordinar los cambios de idioma en la interfaz del menú. Al iniciar, carga el idioma guardado. Cuando el usuario interactúa con los controles de idioma, el método `SetLanguage` es invocado, el cual actualiza el estado global del idioma a través de `GameState` y luego dispara una actualización visual de todos los textos del menú usando el método privado `UpdateUIText`. Este método `UpdateUIText` utiliza las cadenas de texto predefinidas en el `ScriptableObject` `MenuTexts` para aplicar la traducción correcta.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere que los componentes `TextMeshProUGUI` referenciados (`startButtonText`, `creditsButtonText`, `languagesLabelText`) existan en la jerarquía de la escena y estén asignados en el Inspector de Unity.
-   **Eventos (Entrada):** Este script espera ser invocado por eventos de UI. Específicamente, el método público `SetLanguage(int languageIndex)` está diseñado para ser conectado a un `UnityEvent` de botones o dropdowns en el Inspector de Unity (ej. `onClick` de un botón de cambio de idioma).
-   **Eventos (Salida):** `StartMenuManager` no invoca ni emite eventos (como `UnityEvent` o `Action`) para notificar a otros sistemas sobre cambios o acciones realizadas. Su interacción con `GameState` es directa a través de la llamada a `gameState.SetLanguage()`.
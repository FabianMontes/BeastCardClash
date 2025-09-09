# StartMenuManager
Este script, `StartMenuManager`, es el encargado de gestionar la interfaz de usuario (UI) del menú de inicio del juego **Beast Card Clash**, centrándose principalmente en la funcionalidad de localización (cambio de idioma). Actúa como un puente entre la configuración global del juego (gestionada por `GameState`), los textos traducidos (gestionados por `MenuTexts`) y los elementos visuales de la UI del menú (botones y etiquetas).

Su función principal es asegurar que los textos de los elementos clave del menú de inicio, como los botones "Inicio" y "Créditos" y la etiqueta de selección de idioma, se muestren correctamente en el idioma seleccionado por el jugador. Permite cambiar el idioma en tiempo de ejecución y actualiza la UI de forma reactiva para reflejar esta selección, contribuyendo a una experiencia de usuario fluida y accesible para una audiencia diversa, en línea con el enfoque cultural y educativo del proyecto.

El script se apoya en dos Singletons o managers globales: `GameState`, para conocer el idioma actual del juego y para almacenar cualquier cambio de idioma, y `MenuTexts`, un ScriptableObject (o similar) que contiene las cadenas de texto para cada idioma.

# Métodos

## Métodos de Unity

### Start
Este método se ejecuta una vez al inicio del ciclo de vida del script, cuando el objeto al que está asociado se activa en la escena.

Su propósito en `StartMenuManager` es inicializar los textos de la UI del menú de inicio basándose en el idioma que esté configurado en ese momento en el `GameState` global del juego. Esto asegura que, al cargar el menú de inicio, los elementos de texto se presenten inmediatamente en el idioma correcto, evitando que se muestren en un idioma por defecto incorrecto o con placeholders.

```csharp
void Start()
{
    // Al iniciar, actualizamos el texto con el idioma que ya está guardado en GameState
    if (gameState != null) UpdateUIText(gameState.CurrentLanguage);
}
```
Como se puede ver en el código, el método realiza una comprobación sencilla para asegurar que la referencia a `gameState` no sea `null` antes de intentar acceder a su propiedad `CurrentLanguage` y llamar a `UpdateUIText`.

## Otros métodos

### SetLanguage (public void SetLanguage(int languageIndex))
Este método es público y se expone para ser llamado desde otros componentes, típicamente desde elementos de la UI como botones o desplegables de selección de idioma.

Recibe un entero (`languageIndex`) que representa el índice del idioma deseado (por ejemplo, `0` para español, `1` para inglés). Su funcionalidad principal es la siguiente:

1.  **Conversión de Índice a Idioma:** Convierte el `int` recibido a un tipo `Languages` (presumiblemente un `enum` definido en otro lugar del proyecto), lo que permite trabajar con nombres de idiomas más claros y tipado fuerte.
    ```csharp
    Languages newLanguage = (Languages)languageIndex;
    ```
2.  **Actualización de `GameState`:** Notifica al `gameState` global sobre el cambio de idioma. Esto es crucial porque `gameState` es la fuente de verdad del idioma actual del juego y otros componentes pueden depender de esta información.
    ```csharp
    gameState.SetLanguage(newLanguage);
    ```
3.  **Actualización Inmediata de UI:** Después de actualizar el estado global, llama inmediatamente al método privado `UpdateUIText` para que los textos de la UI del menú de inicio se refresquen y muestren las cadenas correspondientes al nuevo idioma.
    ```csharp
    UpdateUIText(newLanguage);
    ```
Este método encapsula la lógica para cambiar el idioma del juego y actualizar la UI de manera consistente.

### UpdateUIText (private void UpdateUIText(Languages language))
Este es un método auxiliar privado, lo que significa que solo puede ser llamado desde dentro de la clase `StartMenuManager`. Su responsabilidad es aplicar las cadenas de texto traducidas a los componentes de `TextMeshProUGUI` del menú de inicio.

Recibe el idioma deseado como un parámetro `Languages`. El flujo de funcionamiento es el siguiente:

1.  **Validación de Dependencia:** Primero, verifica si el componente `menuTexts` (que contiene las traducciones) ha sido asignado en el Inspector de Unity. Si no lo está, emite un error en la consola para alertar a los desarrolladores y detiene la ejecución del método para evitar errores de `NullReferenceException`.
    ```csharp
    if (menuTexts == null)
    {
        Debug.LogError("El asset 'MenuTexts' no está asignado en el StartMenuManager.");
        return;
    }
    ```
2.  **Lógica de Traducción:** Utiliza una estructura condicional (`if-else`) para determinar qué conjunto de textos debe usar:
    *   Si el `language` es `Languages.spanish`, asigna los textos con sufijo `_es` de `menuTexts` a los respectivos `TextMeshProUGUI` (startButtonText, creditsButtonText, languagesLabelText).
    *   Si no es español (asumiendo que `Languages.english` es la única otra opción o el idioma por defecto), asigna los textos con sufijo `_en`.
    ```csharp
    if (language == Languages.spanish)
    {
        startButtonText.text = menuTexts.startButton_es;
        creditsButtonText.text = menuTexts.creditsButton_es;
        languagesLabelText.text = menuTexts.languagesLabel_es;
    }
    // Inglés
    else
    {
        startButtonText.text = menuTexts.startButton_en;
        creditsButtonText.text = menuTexts.creditsButton_en;
        languagesLabelText.text = menuTexts.languagesLabel_en;
    }
    ```
Este método garantiza que los elementos de texto del menú de inicio siempre reflejen el idioma actual de manera correcta y centralizada.

## Getters y Setters

1.  `SetLanguage (void)`: Este método permite establecer el idioma actual del juego a través de un índice numérico, actualizando tanto el estado global (`GameState`) como la interfaz de usuario del menú.
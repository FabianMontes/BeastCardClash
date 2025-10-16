# `MenuTexts`
El script `MenuTexts` es una implementación de `ScriptableObject` diseñada para centralizar y gestionar los textos de la interfaz de usuario (UI) relacionados con el menú principal del juego, soportando múltiples idiomas. Al ser un `ScriptableObject`, permite crear activos (`.asset`) en el Editor de Unity que contienen estos textos, facilitando que sean configurados por diseñadores y accesibles por cualquier otro script sin necesidad de estar adjuntos a un GameObject en la escena.

Su propósito principal es almacenar strings predefinidos para elementos como botones y etiquetas, lo que es fundamental para el sistema de localización del juego. Actualmente, gestiona textos en español (`_es`) y en inglés (`_en`), permitiendo que la UI se adapte dinámicamente al idioma seleccionado por el jugador. Esto contribuye a una mejor experiencia de usuario y simplifica el mantenimiento de los textos del menú a medida que el juego evoluciona o se añaden nuevos idiomas.

### Ejemplo de uso en el Editor de Unity
El atributo `[CreateAssetMenu]` permite crear instancias de `MenuTexts` directamente desde el menú "Assets/Create" del Editor de Unity bajo la ruta "Localizations/Menu Texts". Una vez creado, el activo `MenuTexts.asset` puede ser configurado en el Inspector, como se muestra a continuación:

```csharp
[CreateAssetMenu(fileName = "MenuTexts", menuName = "Localizations/Menu Texts")]
public class MenuTexts : ScriptableObject
{
    // Textos en español
    [Header("Spanish")]
    public string startButton_es = "Iniciar";
    public string creditsButton_es = "Créditos";
    public string languagesLabel_es = "Idiomas";

    // Textos en inglés
    [Header("English")]
    public string startButton_en = "Start";
    public string creditsButton_en = "Credits";
    public string languagesLabel_en = "Languages";
}
```

Un script encargado de la UI del menú, por ejemplo, podría tener una referencia a este activo:

```csharp
// Ejemplo hipotético de un script UIController
public class MainMenuUIController : MonoBehaviour
{
    public MenuTexts menuTextsAsset; // Referencia al ScriptableObject
    public Text startButtonText; // Componente de texto del botón de inicio

    void Start()
    {
        // Supongamos que hay una lógica para determinar el idioma actual
        string currentLanguage = "es"; // O "en"

        if (currentLanguage == "es")
        {
            startButtonText.text = menuTextsAsset.startButton_es;
            // ... otros textos en español
        }
        else if (currentLanguage == "en")
        {
            startButtonText.text = menuTextsAsset.startButton_en;
            // ... otros textos en inglés
        }
    }
}
```

Este enfoque promueve una buena experiencia de desarrollo al desacoplar los datos de los textos de la lógica de la UI y permitir una fácil edición y gestión de los mismos.

# Métodos

## Métodos de Unity

`MenuTexts` hereda de `ScriptableObject` y está diseñado principalmente como un contenedor de datos. Por lo tanto, no utiliza ni implementa directamente los métodos de ciclo de vida comunes de `MonoBehaviour` como `Awake`, `Start` o `Update`. Su propósito es almacenar datos que pueden ser accedidos por otros componentes del juego.

## Otros métodos

Este script no contiene métodos personalizados o lógicos adicionales más allá de la definición de sus campos públicos. Su funcionalidad se limita a la exposición de datos de texto para su uso externo.

## Getters y Setters

El script `MenuTexts` expone sus datos a través de campos públicos, lo que permite el acceso directo a sus valores sin necesidad de métodos `get` o `set` explícitos. A continuación, se detallan los campos disponibles:

1.  `startButton_es`: Almacena el texto correspondiente al botón "Iniciar" cuando el idioma seleccionado es español.
2.  `creditsButton_es`: Almacena el texto correspondiente al botón "Créditos" cuando el idioma seleccionado es español.
3.  `languagesLabel_es`: Almacena el texto correspondiente a la etiqueta "Idiomas" cuando el idioma seleccionado es español.
4.  `startButton_en`: Almacena el texto correspondiente al botón "Start" cuando el idioma seleccionado es inglés.
5.  `creditsButton_en`: Almacena el texto correspondiente al botón "Credits" cuando el idioma seleccionado es inglés.
6.  `languagesLabel_en`: Almacena el texto correspondiente a la etiqueta "Languages" cuando el idioma seleccionado es inglés.
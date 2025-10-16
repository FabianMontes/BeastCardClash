# `StartMenuManager`
Este script, `StartMenuManager`, es el componente principal encargado de la gestión y visualización de la interfaz de usuario (UI) del menú de inicio del juego **Beast Card Clash**. Su función primordial es coordinar la interacción entre los datos del estado del juego (`GameState`) y los textos traducibles de la UI (`MenuTexts`) para asegurar que el menú se muestre correctamente y responda a los cambios de idioma del usuario.

El script mantiene referencias a dos "singletons" o managers globales (`GameState` y `MenuTexts`), que son cruciales para su operación:
*   `GameState`: Proporciona el idioma actual del juego y permite establecer uno nuevo.
*   `MenuTexts`: Contiene las cadenas de texto traducidas para los diferentes elementos de la UI del menú.

Al inicio, `StartMenuManager` carga el idioma guardado y actualiza los textos. Posteriormente, permite cambiar el idioma a través de un método público, refrescando inmediatamente la UI. Esto garantiza una experiencia de usuario fluida y localizada desde el primer momento de interacción con el juego.

# Métodos

## Métodos de Unity

### `Start`
Este método es parte del ciclo de vida de Unity y se invoca una vez al inicio, justo antes de que el primer frame del juego se actualice, siempre y cuando el script esté habilitado. En `StartMenuManager`, su propósito es inicializar los textos de la interfaz de usuario del menú de inicio según el idioma que esté configurado actualmente en el sistema de `GameState`.

```csharp
void Start()
{
    // Al iniciar, actualizamos el texto con el idioma que ya está guardado en GameState
    if (gameState != null) UpdateUIText(gameState.CurrentLanguage);
}
```

La condición `if (gameState != null)` asegura que la operación solo se realice si la referencia al `GameState` ha sido correctamente asignada en el Inspector de Unity, evitando posibles errores de referencia nula. Luego, llama al método `UpdateUIText` pasando el `CurrentLanguage` obtenido de `gameState` para que los elementos de texto se muestren con la localización correcta desde el arranque del menú.

## Otros métodos

### `SetLanguage(int languageIndex)`
Este método público es el principal punto de entrada para cambiar el idioma del juego desde la interfaz de usuario, por ejemplo, mediante la selección de un dropdown o un botón de idioma. Recibe un índice entero que se mapea directamente a un idioma específico.

```csharp
public void SetLanguage(int languageIndex)
{
    // Convertimos el índice (0 para español, 1 para inglés) al tipo Languages
    Languages newLanguage = (Languages)languageIndex;

    // Le decimos al manager que cambie el idioma en el GameState
    gameState.SetLanguage(newLanguage);

    // Actualizamos el texto de la UI inmediatamente
    UpdateUIText(newLanguage);
}
```

1.  **Conversión de índice a `Languages`**: El `languageIndex` (típicamente 0 para español, 1 para inglés, asumiendo la definición del enum `Languages`) se convierte explícitamente al tipo `Languages`. Esto refuerza la legibilidad y el tipo seguro del idioma.
2.  **Actualización del `GameState`**: Se invoca el método `SetLanguage` del `gameState` singleton para actualizar el idioma global del juego. Esto asegura que otros componentes que dependan del idioma también se ajusten.
3.  **Refresco de la UI**: Inmediatamente después de actualizar el `GameState`, se llama a `UpdateUIText` con el nuevo idioma para que todos los elementos de texto del menú de inicio reflejen el cambio de idioma en tiempo real.

### `UpdateUIText(Languages language)`
Este es un método auxiliar privado encargado de la lógica de actualización de los componentes de texto de la UI con las cadenas de texto localizadas. Se invoca tanto al inicio del juego como cada vez que el idioma es cambiado.

```csharp
private void UpdateUIText(Languages language)
{
    // Si no hay menú, retorna y advierte del error
    if (menuTexts == null)
    {
        Debug.LogError("El asset 'MenuTexts' no está asignado en el StartMenuManager.");
        return;
    }

    // Actualizamos el texto según el idioma
    // Español
    if (language == Languages.Spanish)
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
}
```

1.  **Verificación de `MenuTexts`**: Antes de intentar acceder a cualquier texto, se realiza una comprobación para asegurar que el asset `MenuTexts` ha sido correctamente asignado. Si no lo está, se registra un error en la consola de Unity y el método retorna, previniendo errores de referencia nula.
2.  **Actualización Condicional**: Utiliza una estructura `if-else` para determinar qué conjunto de textos debe usarse basándose en el parámetro `language` recibido:
    *   Si `language` es `Languages.Spanish`, asigna las cadenas de texto en español (`startButton_es`, `creditsButton_es`, `languagesLabel_es`) a los respectivos componentes `TextMeshProUGUI`.
    *   Si no es español (asumiendo que `Languages.English` es la otra opción), asigna las cadenas de texto en inglés (`startButton_en`, `creditsButton_en`, `languagesLabel_en`).

Este diseño permite una fácil expansión si se desean añadir más idiomas en el futuro, aunque requeriría modificar esta lógica condicional o adoptar un sistema de localización más robusto si el número de idiomas crece significativamente.

## Getters y Setters

Los siguientes campos están serializados (`[SerializeField]`) y actúan como puntos de configuración accesibles desde el Inspector de Unity. Aunque no son "getters" o "setters" en el sentido de propiedades C#, permiten establecer referencias y valores desde fuera del código, siendo cruciales para la configuración del script.

1.  `gameState`: Permite asignar la referencia al objeto `GameState` que gestiona el estado global del juego, incluyendo el idioma actual.
2.  `menuTexts`: Permite asignar la referencia al ScriptableObject `MenuTexts` que contiene todas las cadenas de texto traducibles para los elementos de la UI del menú de inicio.
3.  `startButtonText`: Permite enlazar el componente `TextMeshProUGUI` del botón "Inicio" en la UI para que su texto pueda ser actualizado por el script.
4.  `creditsButtonText`: Permite enlazar el componente `TextMeshProUGUI` del botón "Créditos" en la UI para que su texto pueda ser actualizado por el script.
5.  `languagesLabelText`: Permite enlazar el componente `TextMeshProUGUI` de la etiqueta de "Idiomas" en la UI para que su texto pueda ser actualizado por el script.
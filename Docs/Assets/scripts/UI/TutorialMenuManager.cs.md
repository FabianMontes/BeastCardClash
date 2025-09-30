# TutorialMenuManager
El script `TutorialMenuManager` es el componente central encargado de gestionar la interfaz de usuario del menú de tutorial en "Beast Card Clash". Su función principal es controlar la visibilidad y el avance a través de las diferentes páginas del tutorial, las cuales están disponibles en español e inglés.

Este script interactúa directamente con el componente `GameState` del proyecto para determinar el idioma actual del juego y, en base a ello, activa y desactiva los paneles de tutorial correspondientes. Proporciona métodos públicos para navegar secuencialmente entre los paneles (`Panel1`, `Panel2`, `Panel3`) y para saltarse el tutorial, cargando la escena de selección de _skins_ (`SkinSelector`).

La implementación actual prioriza la funcionalidad y la experiencia de desarrollo, como se evidencia en la nota `TODO` para una futura refactorización de la lógica de activación/desactivación de paneles. Esto permite que los desarrolladores puedan seguir iterando rápidamente sin comprometer la entrega de una buena experiencia de usuario inicial.

# Métodos

## Métodos de Unity

### Start
`void Start()`
Este método se invoca una vez al inicio del ciclo de vida del script. Su propósito es configurar el estado inicial del menú de tutorial. Primero, llama a `InitializePanels()` para asegurar que solo los paneles del idioma activo estén disponibles y que todos los paneles individuales estén inicialmente desactivados. Posteriormente, invoca a `ShowPanel1()` para mostrar la primera página del tutorial, garantizando que el jugador siempre comience desde el principio en el idioma correcto.

```csharp
void Start()
{
    // Desactiva la rama de páneles correspondientes al idioma no usado
    // Y al iniciar, mostramos solo el primer panel
    InitializePanels();
    ShowPanel1();
}
```

## Otros métodos

### InitializePanels
`void InitializePanels()`
Este método privado se encarga de preparar la interfaz del tutorial según el idioma seleccionado en el `GameState`. Su lógica se basa en el valor de `gameState.CurrentLanguage` para:

1.  **Activar** el `GameObject` padre que contiene todos los paneles del idioma correspondiente (`esPanels` para español o `enPanels` para inglés).
2.  **Desactivar** el `GameObject` padre del idioma no utilizado.
3.  **Desactivar individualmente** todos los paneles (`RawImage`) del idioma activo (es decir, `panel1Es`, `panel2Es`, `panel3Es` si es español, o sus equivalentes en inglés). Esto asegura que al inicio, ninguna página individual esté visible hasta que `ShowPanel1()` sea llamado explícitamente, evitando así superposiciones o estados inconsistentes en la UI.

```csharp
void InitializePanels()
{
    if (gameState.CurrentLanguage == Languages.spanish)
    {
        esPanels.gameObject.SetActive(true);
        enPanels.gameObject.SetActive(false);
        // Desactiva los paneles individuales de la rama activa
        panel1Es.gameObject.SetActive(false);
        panel2Es.gameObject.SetActive(false);
        panel3Es.gameObject.SetActive(false);
    }
    else if (gameState.CurrentLanguage == Languages.english)
    {
        enPanels.gameObject.SetActive(true);
        esPanels.gameObject.SetActive(false);
        // Desactiva los paneles individuales de la rama activa
        panel1En.gameObject.SetActive(false);
        panel2En.gameObject.SetActive(false);
        panel3En.gameObject.SetActive(false);
    }
}
```

[!NOTE]
El script incluye un `TODO` en la línea 12 (`// TODO: refactorizar este código, en las funciones de activación y desactivación del paneles`). Esto indica una mejora planificada para simplificar la lógica de gestión de paneles, aunque el código funciona correctamente en su estado actual. Se recomienda tenerlo en cuenta para futuras iteraciones del proyecto, buscando patrones más limpios o modularizados para el manejo de la activación/desactivación de elementos de UI.

### ShowPanel1
`public void ShowPanel1()`
Este método público es responsable de mostrar la primera página del tutorial. Evalúa el idioma actual del juego a través de `gameState.CurrentLanguage` y, según sea español o inglés, realiza lo siguiente:

1.  Activa el `RawImage` correspondiente al `Panel1` (ej. `panel1Es` para español, `panel1En` para inglés).
2.  Desactiva los `RawImage` de los `Panel2` y `Panel3` para el mismo idioma, asegurando que solo el primer panel sea visible.
3.  Actualiza la variable interna `CurrentPanel` a `CurrentPanel.Panel1`, registrando la página actual del tutorial.

Este método puede ser invocado programáticamente (como en `Start()`) o asignado a un botón en la interfaz de usuario para permitir la navegación directa a la primera página.

```csharp
public void ShowPanel1()
{
    // Español
    if (gameState.CurrentLanguage == Languages.spanish)
    {
        panel1Es.gameObject.SetActive(true);
        panel2Es.gameObject.SetActive(false);
        panel3Es.gameObject.SetActive(false);
    }
    // Inglés
    else
    {
        panel1En.gameObject.SetActive(true);
        panel2En.gameObject.SetActive(false);
        panel3En.gameObject.SetActive(false);
    }
    CurrentPanel = CurrentPanel.Panel1;
}
```

### ShowPanel2
`public void ShowPanel2()`
Similar a `ShowPanel1()`, este método muestra la segunda página del tutorial. Activa el `RawImage` del `Panel2` (ej. `panel2Es` o `panel2En`) y desactiva los otros dos paneles del mismo idioma. Actualiza la variable `CurrentPanel` a `CurrentPanel.Panel2`. Está diseñado para ser llamado como parte de la secuencia de navegación del tutorial.

### ShowPanel3
`public void ShowPanel3()`
Este método muestra la tercera y última página del tutorial. Activa el `RawImage` del `Panel3` (ej. `panel3Es` o `panel3En`) y desactiva los otros dos paneles del mismo idioma. Actualiza la variable `CurrentPanel` a `CurrentPanel.Panel3`. Una línea de código comentada (`NextButton.gameObject.SetActive(false);`) sugiere una posible futura funcionalidad para ocultar el botón "Siguiente" una vez que se llega a la última página, indicando el final del recorrido del tutorial.

### OnClick
`public void OnClick()`
Este método público es el principal controlador de la navegación secuencial a través de los paneles del tutorial. Está diseñado para ser invocado por el botón "Siguiente" (`NextButton`) en la interfaz de usuario. Utiliza una estructura `switch` basada en el valor de la variable `CurrentPanel` para determinar la acción a seguir:

*   Si `CurrentPanel` es `Panel1`, llama a `ShowPanel2()` para avanzar a la siguiente página.
*   Si `CurrentPanel` es `Panel2`, llama a `ShowPanel3()` para avanzar a la última página.
*   Si `CurrentPanel` es `Panel3` (la última página), invoca a `SkipButton()`. Esto significa que, al llegar al final del tutorial, el botón "Siguiente" se transforma funcionalmente en un botón para finalizar el tutorial y pasar a la siguiente fase del juego.

```csharp
public void OnClick()
{
    switch (CurrentPanel)
    {
        case CurrentPanel.Panel1:
            ShowPanel2();
            break;
        case CurrentPanel.Panel2:
            ShowPanel3();
            break;
        case CurrentPanel.Panel3:
            // Cuando llegamos al último panel, el botón hace la misma función de saltar
            SkipButton();
            break;
        default:
            break;
    }
}
```

### SkipButton
`public void SkipButton()`
Este método público tiene como única responsabilidad cargar la escena de Unity con el nombre "SkinSelector". Actúa como un punto de salida del tutorial, permitiendo a los jugadores omitir el resto de las explicaciones o finalizar el tutorial una vez que han llegado al final. Puede ser invocado por un botón "Saltar" dedicado en la UI o, como se ve en `OnClick()`, por el botón "Siguiente" cuando se encuentra en la última página del tutorial. La carga de la escena "SkinSelector" sugiere que el siguiente paso para el jugador es la personalización de su experiencia de juego (posiblemente de personajes o cartas) antes de entrar en la jugabilidad principal de "Beast Card Clash".

```csharp
public void SkipButton()
{
    SceneManager.LoadScene("SkinSelector");
}
```

## Getters y Setters
El script `TutorialMenuManager` no define explícitamente propiedades públicas con métodos `get` y `set` para exponer o modificar directamente sus campos privados. Los datos de configuración (`esPanels`, `panel1Es`, `enPanels`, etc., `gameState`, `NextButton`) se establecen a través del Inspector de Unity como campos `[SerializeField]`. La variable `CurrentPanel` es un campo privado que se gestiona y modifica internamente por los métodos `ShowPanel1()`, `ShowPanel2()` y `ShowPanel3()` para mantener el estado actual del tutorial.
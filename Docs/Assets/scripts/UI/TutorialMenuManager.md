# `TutorialMenuManager`
Este script de `MonoBehaviour` es responsable de gestionar la interfaz de usuario del menú de tutorial en el juego **Beast Card Clash**. Su función principal es controlar la visibilidad de los diferentes paneles del tutorial, permitiendo la navegación secuencial entre ellos y adaptándose al idioma seleccionado por el jugador (español o inglés). Además, maneja la lógica para avanzar a la siguiente escena del juego una vez que el tutorial ha sido completado o saltado. El script interactúa con elementos UI como `RawImage` para los paneles y `Button` para la navegación, y depende de un objeto `GameState` para determinar el idioma actual del juego.

La clase incluye un `TODO` explícito para refactorizar las funciones de activación y desactivación de paneles, lo que indica un área de mejora conocida para reducir la redundancia de código en los métodos `ShowPanelX()`.

# Métodos

## Métodos de Unity

### `Start()`
Este método se ejecuta una vez al inicio del ciclo de vida del script. Su propósito es inicializar el estado de los paneles del tutorial cuando la escena se carga.
1.  **`InitializePanels()`**: Llama a este método para asegurarse de que solo el conjunto de paneles correspondiente al idioma actual esté activo y que todos los paneles individuales dentro de ese conjunto estén inicialmente desactivados.
2.  **`ShowPanel1()`**: Después de la inicialización, este método se llama para mostrar el primer panel del tutorial, configurando el estado inicial de la navegación.

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

### `InitializePanels()`
Este método se encarga de configurar la visibilidad inicial de los grupos de paneles de tutorial según el idioma del juego.
*   **Verificación de idioma**: Comprueba el `CurrentLanguage` del objeto `gameState`.
*   **Activación/Desactivación de ramas**: Si el idioma es español (`Languages.Spanish`), activa el `GameObject` padre `esPanels` y desactiva `enPanels`. Si el idioma es inglés (`Languages.English`), hace lo contrario.
*   **Desactivación de paneles individuales**: Dentro de la rama activa (español o inglés), desactiva todos los `RawImage` individuales (`panel1Es`, `panel2Es`, `panel3Es`, o sus contrapartes en inglés) para asegurar que ningún panel esté visible antes de que `ShowPanel1()` se llame.

```csharp
void InitializePanels()
{
    // Espaañol: desactiva lo del inglés
    if (gameState.CurrentLanguage == Languages.Spanish)
    {
        // Activa el panel español y desactiva el inglés
        esPanels.gameObject.SetActive(true);
        enPanels.gameObject.SetActive(false);

        // Desactiva los paneles individuales de la rama activa
        panel1Es.gameObject.SetActive(false);
        panel2Es.gameObject.SetActive(false);
        panel3Es.gameObject.SetActive(false);
    }
    // Inglés: desactiva lo del español
    else if (gameState.CurrentLanguage == Languages.English)
    {
        // Activa el panel inglés y desactiva el español
        enPanels.gameObject.SetActive(true);
        esPanels.gameObject.SetActive(false);

        // Desactiva los paneles individuales de la rama activa
        panel1En.gameObject.SetActive(false);
        panel2En.gameObject.SetActive(false);
        panel3En.gameObject.SetActive(false);
    }
}
```

### `ShowPanel1()`
Este método público activa el primer panel del tutorial y desactiva los otros dos, actualizando la variable `CurrentPanel` a `CurrentPanel.Panel1`.
*   Comprueba el `gameState.CurrentLanguage` para determinar si debe manipular los paneles en español (`panel1Es`, `panel2Es`, `panel3Es`) o en inglés (`panel1En`, `panel2En`, `panel3En`).
*   Activa el `GameObject` del panel 1 correspondiente al idioma y desactiva los paneles 2 y 3.

### `ShowPanel2()`
Este método público activa el segundo panel del tutorial y desactiva los otros dos, actualizando la variable `CurrentPanel` a `CurrentPanel.Panel2`.
*   Similar a `ShowPanel1()`, comprueba el idioma y manipula los `GameObject` de los paneles.
*   Activa el `GameObject` del panel 2 correspondiente al idioma y desactiva los paneles 1 y 3.

### `ShowPanel3()`
Este método público activa el tercer y último panel del tutorial y desactiva los otros dos, actualizando la variable `CurrentPanel` a `CurrentPanel.Panel3`.
*   Similar a los métodos anteriores, comprueba el idioma y manipula los `GameObject` de los paneles.
*   Activa el `GameObject` del panel 3 correspondiente al idioma y desactiva los paneles 1 y 2.
*   Contiene una línea comentada `// NextButton.gameObject.SetActive(false);` que podría usarse para ocultar el botón "Siguiente" una vez que se llega al último panel, aunque actualmente no está activa.

> [!NOTE]
> Los métodos `ShowPanel1()`, `ShowPanel2()`, y `ShowPanel3()` son los que el `TODO` en la parte superior del script sugiere refactorizar debido a su estructura repetitiva. Podrían combinarse en un único método parametrizado.

### `OnClick()`
Este método público está diseñado para ser asignado al evento `onClick` de un botón (probablemente el `NextButton`). Su función es avanzar a través de los paneles del tutorial o finalizarlo, dependiendo del panel actual.
*   Utiliza una estructura `switch` para evaluar el valor de `CurrentPanel`:
    *   Si `CurrentPanel` es `Panel1`, llama a `ShowPanel2()`.
    *   Si `CurrentPanel` es `Panel2`, llama a `ShowPanel3()`.
    *   Si `CurrentPanel` es `Panel3` (el último panel), llama a `SkipButton()`, lo que significa que el botón "Siguiente" ahora actúa como un botón para saltar o finalizar el tutorial.

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

### `SkipButton()`
Este método público es invocado para finalizar el tutorial y cargar la siguiente escena del juego.
*   Utiliza `SceneManager.LoadScene("SkinSelector")` para cargar la escena llamada "SkinSelector". Esto implica que "SkinSelector" es la escena a la que se dirige el jugador después de completar o saltar el tutorial, probablemente para elegir un personaje o configuración inicial.
*   Este método puede ser invocado directamente por un botón "Saltar" en la UI o, como se ve en `OnClick()`, puede ser la acción final del botón "Siguiente" en el último panel del tutorial.

```csharp
public void SkipButton()
{
    SceneManager.LoadScene("SkinSelector");
}
```

## Getters y Setters

1.  `esPanels`: Un `GameObject` que actúa como contenedor padre para todos los paneles de tutorial en español. Su visibilidad se gestiona para activar/desactivar el grupo completo.
2.  `panel1Es`: Un `RawImage` que representa el primer panel del tutorial en español.
3.  `panel2Es`: Un `RawImage` que representa el segundo panel del tutorial en español.
4.  `panel3Es`: Un `RawImage` que representa el tercer panel del tutorial en español.
5.  `enPanels`: Un `GameObject` que actúa como contenedor padre para todos los paneles de tutorial en inglés. Su visibilidad se gestiona para activar/desactivar el grupo completo.
6.  `panel1En`: Un `RawImage` que representa el primer panel del tutorial en inglés.
7.  `panel2En`: Un `RawImage` que representa el segundo panel del tutorial en inglés.
8.  `panel3En`: Un `RawImage` que representa el tercer panel del tutorial en inglés.
9.  `gameState`: Una referencia a un script `GameState`, que se espera contenga información sobre el estado global del juego, como el idioma actual (`CurrentLanguage`). Es crucial para la lógica de localización de los paneles.
10. `NextButton`: Una referencia a un componente `Button` de la UI, que es el botón principal para avanzar en el tutorial. Sus eventos `onClick` se conectan al método `OnClick()`.
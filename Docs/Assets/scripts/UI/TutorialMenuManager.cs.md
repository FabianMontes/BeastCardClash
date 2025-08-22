# `TutorialMenuManager.cs`

## 1. Propósito General
Este script es responsable de gestionar la interfaz de usuario del menú de tutorial dentro del juego Beast Card Clash. Coordina la visualización de diferentes paneles de tutorial basados en el idioma seleccionado por el jugador y maneja la navegación entre estas páginas del tutorial, así como la transición a la siguiente escena del juego.

## 2. Componentes Clave

### `CurrentPanel` (Enum)
`CurrentPanel` es una enumeración simple que define los estados posibles del panel de tutorial actualmente visible. Se utiliza para rastrear qué página del tutorial se está mostrando en un momento dado y para controlar la lógica de navegación.

```csharp
public enum CurrentPanel
{
    Panel1, Panel2, Panel3
}
```

### `TutorialMenuManager` (Clase)
La clase `TutorialMenuManager` es un `MonoBehaviour` que orquesta la lógica del menú de tutorial. Se adjunta a un objeto de juego en la escena del menú de tutorial y gestiona los elementos de la UI para presentar las instrucciones del juego.

*   **Variables Públicas / Serializadas:**
    *   `esPanels` (GameObject): Un `GameObject` padre que agrupa todos los paneles de tutorial en español. Se utiliza para activar o desactivar rápidamente la rama completa de paneles en español.
    *   `panel1Es`, `panel2Es`, `panel3Es` (RawImage): Componentes `RawImage` individuales que representan las diferentes páginas del tutorial cuando el idioma es español. Solo uno de estos debe estar activo a la vez.
    *   `enPanels` (GameObject): Similar a `esPanels`, pero para los paneles de tutorial en inglés.
    *   `panel1En`, `panel2En`, `panel3En` (RawImage): Componentes `RawImage` individuales para las páginas del tutorial en inglés.
    *   `gameState` (GameState): Una referencia a un script `GameState`. Este objeto es crucial para determinar el idioma actual del juego (`gameState.CurrentLanguage`) y así mostrar los paneles correctos.
    *   `NextButton` (Button): El botón de la UI que permite al jugador avanzar a la siguiente página del tutorial o saltar el tutorial por completo al final.

*   **Métodos Principales:**
    *   `void Start()`: Este método se invoca al inicio de la escena. Su función principal es inicializar el estado de los paneles de tutorial. Primero, llama a `InitializePanels()` para asegurar que solo los paneles del idioma activo estén disponibles, y luego llama a `ShowPanel1()` para mostrar la primera página del tutorial.

    *   `void InitializePanels()`: Este método se encarga de preparar la UI de los tutoriales según el idioma seleccionado. Basándose en `gameState.CurrentLanguage`, activa el `GameObject` padre (`esPanels` o `enPanels`) correspondiente al idioma activo y desactiva el del idioma opuesto. Además, se asegura de que todos los paneles individuales dentro de la rama activa estén inicialmente desactivados, para que solo `ShowPanel1()` los active explícitamente.

    *   `public void ShowPanel1()`, `public void ShowPanel2()`, `public void ShowPanel3()`: Estos métodos son responsables de cambiar la página del tutorial visible. Cada método activa su panel correspondiente (por ejemplo, `panel1Es` o `panel1En`) y desactiva los otros dos paneles del mismo idioma. Después de activar el panel, actualizan la variable `CurrentPanel` para reflejar la página actualmente mostrada. Estos métodos están diseñados para ser asignados directamente a la interacción de botones o para ser llamados por la lógica de navegación.

    *   `public void OnClick()`: Este método está diseñado para ser invocado por el evento `OnClick()` del `NextButton`. Contiene una estructura `switch` que evalúa el valor de `CurrentPanel`. Si el panel actual es `Panel1`, llama a `ShowPanel2()`; si es `Panel2`, llama a `ShowPanel3()`. Cuando el jugador está en `Panel3` (la última página del tutorial), este método invoca `SkipButton()`, lo que significa que el botón "Siguiente" se convierte en un botón de "Saltar" el tutorial al llegar al final.

    *   `public void SkipButton()`: Este método carga la escena "SkinSelector". Está separado para permitir su uso directo (por ejemplo, desde un botón "Saltar Tutorial" dedicado) o como parte de la lógica de navegación del botón "Siguiente" cuando se llega al final del tutorial.

*   **Lógica Clave:**
    La lógica principal de este script reside en la gestión del estado bilingüe de los paneles y la navegación secuencial.
    1.  **Inicialización de Idioma:** Al inicio, `InitializePanels` determina qué conjunto de paneles (español o inglés) debe estar activo basándose en el estado del juego. Esto garantiza que solo los recursos de UI necesarios para el idioma actual se procesen y se muestren.
    2.  **Paginación del Tutorial:** Los métodos `ShowPanelX` (donde X es 1, 2 o 3) implementan una máquina de estados simple para la visualización de los paneles. Cada método asegura que solo una `RawImage` de tutorial esté activa para el idioma actual, desactivando las demás.
    3.  **Navegación del Botón Siguiente:** El método `OnClick` utiliza un `switch` para dirigir el flujo del tutorial. En los primeros dos paneles, el botón "Siguiente" avanza a la siguiente página. Una vez en el último panel (`Panel3`), el mismo botón cambia su función implícitamente para cargar la siguiente escena del juego (`SkinSelector`), simulando un "saltar" el tutorial una vez completado.

    La implementación de la activación/desactivación de paneles en `ShowPanel1`, `ShowPanel2`, `ShowPanel3` y `InitializePanels` se repite para cada idioma, lo que sugiere el `TODO` en el código para una posible refactorización que la haga más genérica y DRY (Don't Repeat Yourself).

## 3. Dependencias y Eventos

*   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, espera que los `GameObject`s asignados a sus variables serializadas tengan componentes `RawImage` o `Button` según corresponda, y que exista una instancia de `GameState` en la escena.

*   **Eventos (Entrada):**
    *   Este script se suscribe implícitamente al evento `onClick` del `NextButton` a través de la asignación directa en el Inspector de Unity del método `OnClick()`. Cuando el `NextButton` es clickeado, `OnClick()` es invocado.

*   **Eventos (Salida):**
    *   Este script no invoca ningún evento (`UnityEvent` o `Action`) para notificar a otros sistemas. Su interacción con otras partes del juego se limita a cargar una nueva escena (`SceneManager.LoadScene`) y consultar el estado del juego a través del objeto `gameState`.
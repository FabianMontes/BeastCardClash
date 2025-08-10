# `TutorialMenuManager.cs`

## 1. Propósito General
Este script es el controlador principal para la interfaz de usuario del menú de tutorial. Su función es gestionar la visualización secuencial de diferentes paneles (páginas) del tutorial y manejar la navegación del usuario entre ellos, culminando con la transición a la escena principal del juego.

## 2. Componentes Clave

### `CurrentPanel`
- **Descripción:** Un `enum` (enumeración) que define los posibles estados o páginas del tutorial: `Panel1`, `Panel2` y `Panel3`. Se utiliza internamente en la clase `TutorialMenuManager` para rastrear cuál es el panel actualmente visible.

### `TutorialMenuManager`
- **Descripción:** Esta es la clase principal del archivo, que hereda de `MonoBehaviour`. Es responsable de controlar la lógica y el flujo de la experiencia del tutorial dentro del juego. Manipula la visibilidad de los elementos de la interfaz de usuario (UI) que componen cada panel del tutorial y gestiona la carga de la siguiente escena al finalizar.

- **Variables Públicas / Serializadas:**
    - `panel1`, `panel2`, `panel3` (`RawImage`): Estas variables están marcadas con `[SerializeField]`, lo que permite asignarles referencias a los componentes `RawImage` de la interfaz de usuario directamente desde el Inspector de Unity. Cada una de estas `RawImage` representa una página visual distinta del tutorial.
    - `NextButton` (`Button`): También marcada con `[SerializeField]`, esta variable mantiene una referencia al objeto `Button` que probablemente actúa como el botón "Siguiente" o de navegación dentro del tutorial. Su propósito es permitir el control programático de este botón (por ejemplo, para ocultarlo al final del tutorial, aunque la línea de código para esto está comentada).

- **Métodos Principales:**
    - `void Start()`: Este es un método del ciclo de vida de Unity que se invoca una vez al inicio del juego, antes de la primera actualización del frame, después de que el objeto que contiene este script ha sido creado. Su propósito es inicializar el menú del tutorial, asegurándose de que solo el `panel1` sea visible al comienzo.
        ```csharp
        void Start()
        {
            ShowPanel1();
        }
        ```
    - `void ShowPanel1()`: Activa el `GameObject` del `panel1` (haciéndolo visible) y desactiva los `GameObject`s de `panel2` y `panel3`. También actualiza el estado interno `CurrentPanel` a `CurrentPanel.Panel1`.
    - `void ShowPanel2()`: Realiza una operación similar a `ShowPanel1()`, pero activando `panel2` y desactivando los otros, y estableciendo `CurrentPanel` a `CurrentPanel.Panel2`.
    - `void ShowPanel3()`: Activa `panel3`, desactiva los demás y establece `CurrentPanel` a `CurrentPanel.Panel3`. Contiene una línea de código comentada (`NextButton.gameObject.SetActive(false);`) que, si estuviera activa, ocultaría el botón "Siguiente" al llegar al último panel.
    - `void OnClick()`: Este método es el corazón de la navegación del tutorial. Se espera que esté asignado al evento `OnClick()` de un botón en la UI (probablemente el `NextButton`). Utiliza una declaración `switch` para determinar el panel actual y llama al método `ShowPanelX()` correspondiente para avanzar al siguiente panel en la secuencia. Si el panel actual es `Panel3` (el último), en lugar de avanzar, llama al método `SkipButton()`.
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
                    SkipButton();
                    break;
                default:
                    break;
            }
        }
        ```
    - `void SkipButton()`: Este método se encarga de la transición de la escena. Carga la escena de Unity con el nombre "World", asumiendo que "World" es la escena principal del juego a la que el jugador debe ir después de completar (o saltar) el tutorial.
        ```csharp
        public void SkipButton()
        {
            SceneManager.LoadScene("World");
        }
        ```

- **Lógica Clave:**
    La lógica principal de `TutorialMenuManager` se basa en una máquina de estados sencilla, donde cada estado corresponde a un panel del tutorial. Al inicio, el estado es `Panel1`. Cuando el usuario interactúa (ej. clic en el botón "Siguiente"), el método `OnClick()` avanza secuencialmente al siguiente estado (`Panel1` -> `Panel2` -> `Panel3`). Una vez que se llega a `Panel3` y se intenta "avanzar" de nuevo, el sistema interpreta esto como el final del tutorial y procede a cargar la escena del juego principal ("World") mediante el método `SkipButton()`. Los métodos `ShowPanelX()` gestionan directamente la visibilidad de los elementos `RawImage` en la UI para reflejar el panel activo.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, lo que significa que no impone la presencia de otros componentes en el mismo `GameObject` de Unity donde se adjunta.
-   **Eventos (Entrada):** Este script está diseñado para responder a eventos de interfaz de usuario. Específicamente, los métodos `OnClick()` y posiblemente `ShowPanelX()` (si se usan botones de navegación directa) se asignan típicamente a los eventos `OnClick` de los componentes `Button` en el Inspector de Unity.
-   **Eventos (Salida):** El script no invoca `UnityEvent`s ni `Action`s personalizados para notificar a otros sistemas del juego. Su principal forma de "salida" o interacción con el sistema más amplio es la carga de una nueva escena del juego (`SceneManager.LoadScene`).
# `TutorialMenuManager.cs`

## 1. Propósito General
Este script gestiona la interfaz de usuario para el menú de tutorial del juego. Su función principal es controlar la visibilidad de diferentes paneles de tutorial y permitir la navegación secuencial entre ellos, o saltar directamente a la escena principal del juego.

## 2. Componentes Clave

### `CurrentPanel`
- **Descripción:** Es un `enum` que define los posibles estados o páginas del tutorial. Se utiliza para rastrear qué panel de tutorial está actualmente visible, permitiendo al sistema reaccionar adecuadamente a la interacción del usuario.
- **Valores:** `Panel1`, `Panel2`, `Panel3`.

### `TutorialMenuManager`
- **Descripción:** Esta clase hereda de `MonoBehaviour` y es responsable de la lógica del menú de tutorial. Controla qué panel se muestra, cómo se avanza entre ellos y la transición final a la escena del juego.
- **Variables Públicas / Serializadas:**
    - `[SerializeField] private RawImage panel1`: Referencia al objeto `RawImage` que representa el primer panel del tutorial en la interfaz de usuario.
    - `[SerializeField] private RawImage panel2`: Referencia al objeto `RawImage` que representa el segundo panel del tutorial.
    - `[SerializeField] private RawImage panel3`: Referencia al objeto `RawImage` que representa el tercer panel del tutorial.
    - `private CurrentPanel CurrentPanel`: Una variable privada que almacena el panel de tutorial que está actualmente activo, utilizando los valores del `enum CurrentPanel`.
    - `[SerializeField] private Button NextButton`: Referencia al objeto `Button` que permite avanzar al siguiente panel. Se serializa para poder ocultarlo al llegar al último panel.
- **Métodos Principales:**
    - `void Start()`: Método de ciclo de vida de Unity que se llama una vez antes de la primera actualización del frame. Al inicio, activa solo el `panel1` para que sea la primera página visible del tutorial.
        ```csharp
        void Start()
        {
            ShowPanel1();
        }
        ```
    - `void Update()`: Método de ciclo de vida de Unity que se llama una vez por frame. En este script, se encuentra vacío, lo que indica que no hay lógica que deba ejecutarse continuamente en cada frame.
    - `public void ShowPanel1()`: Activa el `panel1` y desactiva los demás (`panel2`, `panel3`). Actualiza la variable `CurrentPanel` a `CurrentPanel.Panel1`.
    - `public void ShowPanel2()`: Activa el `panel2` y desactiva los demás (`panel1`, `panel3`). Actualiza la variable `CurrentPanel` a `CurrentPanel.Panel2`.
    - `public void ShowPanel3()`: Activa el `panel3` y desactiva los demás (`panel1`, `panel2`). Actualiza la variable `CurrentPanel` a `CurrentPanel.Panel3`. Además, desactiva el `NextButton` para evitar que el usuario intente avanzar más allá del último panel.
        ```csharp
        public void ShowPanel3()
        {
            // ... (activación/desactivación de paneles)
            CurrentPanel = CurrentPanel.Panel3;
            NextButton.gameObject.SetActive(false); // Ocultamos el botón de siguiente
        }
        ```
    - `public void OnClick()`: Este método se encarga de la lógica de "avanzar" en el tutorial. Utiliza una sentencia `switch` para determinar el `CurrentPanel` actual y, en función de este, invoca el método `ShowPanelX()` correspondiente para pasar al siguiente panel de forma secuencial (`Panel1` -> `Panel2`, `Panel2` -> `Panel3`). Si se intenta avanzar desde el `Panel3`, llama a `SkipButton()`, aunque el comentario en el código sugiere que esta rama nunca se ejecuta debido a que el botón es ocultado.
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
                // ...
            }
        }
        ```
    - `public void SkipButton()`: Este método carga la escena de Unity llamada "World". Se utiliza para salir del menú de tutorial y comenzar el juego principal.
        ```csharp
        public void SkipButton()
        {
            SceneManager.LoadScene("World");
        }
        ```
- **Lógica Clave:**
    La lógica central radica en la navegación secuencial de los paneles. Al inicio, solo el `panel1` es visible. El método `OnClick` (que probablemente está vinculado a un botón "Siguiente") determina el panel actual y llama al método `ShowPanelX()` apropiado para mostrar el siguiente en la secuencia. Cuando el `panel3` (el último) se activa, el botón "Siguiente" se desactiva para evitar clics adicionales. Existe también un método `SkipButton` para salir del tutorial en cualquier momento y cargar la escena "World".

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, para funcionar correctamente, requiere que se le asignen referencias a objetos `RawImage` para los paneles y un `Button` para el botón "Siguiente" en el Inspector de Unity.
- **Eventos (Entrada):**
    Los métodos públicos `ShowPanel1`, `ShowPanel2`, `ShowPanel3`, `OnClick` y `SkipButton` están diseñados para ser asignados a los eventos `OnClick()` de varios botones en la interfaz de usuario, típicamente a través del Editor de Unity.
- **Eventos (Salida):**
    Este script no invoca ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas del juego. Su única "salida" funcional es la carga de una nueva escena (`SceneManager.LoadScene`).
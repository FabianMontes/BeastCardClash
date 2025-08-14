# `TutorialMenuManager.cs`

## 1. Propósito General
Este script gestiona la navegación y visualización de los paneles del menú de tutorial dentro del juego. Controla la lógica para avanzar a través de las diferentes páginas del tutorial y para transicionar a la escena principal del juego ("World") una vez que el tutorial ha concluido o es omitido.

## 2. Componentes Clave

### `CurrentPanel`
- **Descripción:** Un `enum` simple que define los posibles estados o "páginas" del tutorial. Los valores `Panel1`, `Panel2` y `Panel3` corresponden a las distintas secciones informativas del tutorial que se muestran al jugador. Este `enum` se utiliza para seguir qué panel está actualmente activo.

### `TutorialMenuManager`
- **Descripción:** Esta clase es un `MonoBehaviour`, lo que significa que debe adjuntarse a un GameObject en la jerarquía de Unity. Su función principal es controlar la visibilidad de los diferentes paneles de la interfaz de usuario que componen el tutorial, permitiendo al jugador avanzar secuencialmente a través de ellos.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] private RawImage panel1`, `panel2`, `panel3`: Son referencias a los objetos `RawImage` de la UI que representan cada una de las tres páginas del tutorial. Son serializadas para poder ser asignadas desde el Inspector de Unity. El script activa o desactiva estos objetos para mostrar el panel correspondiente.
    - `private CurrentPanel CurrentPanel`: Almacena el estado actual del tutorial, indicando cuál de los `CurrentPanel` (Panel1, Panel2 o Panel3) está siendo mostrado al usuario.
    - `[SerializeField] private Button NextButton`: Una referencia al botón "Siguiente" o "Avanzar" en la interfaz del tutorial. Se utiliza para invocar la función `OnClick()` y avanzar al siguiente panel. Aunque existe un comentario sobre ocultarlo, la línea está deshabilitada en el código actual.

- **Métodos Principales:**
    - `void Start()`: Este método es parte del ciclo de vida de Unity y se llama una vez antes de la primera actualización del frame. Al inicio del juego o al cargar la escena del tutorial, este método se asegura de que el `Panel1` sea el primero en mostrarse.
        ```csharp
        void Start()
        {
            ShowPanel1();
        }
        ```
    - `public void ShowPanel1()`: Activa la visibilidad del `panel1` y desactiva los demás paneles (`panel2`, `panel3`). También actualiza la variable `CurrentPanel` a `CurrentPanel.Panel1`. Este método es público para poder ser invocado directamente (por ejemplo, por un botón en la UI o por el método `Start`).
    - `public void ShowPanel2()`: Similar a `ShowPanel1()`, pero activa el `panel2` y establece `CurrentPanel` a `CurrentPanel.Panel2`.
    - `public void ShowPanel3()`: Activa el `panel3` y establece `CurrentPanel` a `CurrentPanel.Panel3`. Contiene una línea comentada que sugiere la intención de ocultar el `NextButton` cuando se llega al último panel.
    - `public void OnClick()`: Este método está diseñado para ser invocado por el evento `OnClick()` de un botón de UI (probablemente el `NextButton`). Su lógica es la de una máquina de estados simple:
        - Si el `CurrentPanel` es `Panel1`, llama a `ShowPanel2()`.
        - Si el `CurrentPanel` es `Panel2`, llama a `ShowPanel3()`.
        - Si el `CurrentPanel` es `Panel3` (el último panel del tutorial), invoca el método `SkipButton()`, lo que efectivamente termina el tutorial y carga la escena del juego.
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
                    SkipButton(); // Cuando llegamos al tercer y último panel
                    break;
                default:
                    break;
            }
        }
        ```
    - `public void SkipButton()`: Este método carga la escena del juego principal, identificada por el nombre "World". Se utiliza para salir del tutorial, ya sea porque el jugador ha llegado al final de la secuencia de paneles, o si se añade un botón "Saltar" explícito en el futuro.
        ```csharp
        public void SkipButton()
        {
            SceneManager.LoadScene("World");
        }
        ```

- **Lógica Clave:**
    La lógica principal reside en la gestión secuencial de los paneles. Al iniciar, se muestra el primer panel. El método `OnClick()` actúa como un controlador de flujo, moviendo al jugador a través de los paneles de forma lineal. Una vez que se alcanza el último panel (`Panel3`), el mismo botón de "siguiente" (asociado a `OnClick()`) cambia su comportamiento para cargar la escena principal del juego, en lugar de intentar avanzar a un panel inexistente.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, para su correcto funcionamiento, se espera que el GameObject al que esté adjunto tenga asignadas en el Inspector las referencias a los objetos `RawImage` de los paneles y al `Button` de "Siguiente".
- **Eventos (Entrada):** Este script se suscribe implícitamente a los eventos `OnClick()` de los botones de UI en Unity Editor. Específicamente, se espera que el `NextButton` (y potencialmente otros botones para navegación directa o saltar) estén configurados para llamar a los métodos públicos `OnClick()`, `ShowPanel1()`, `ShowPanel2()`, `ShowPanel3()` o `SkipButton()`.
- **Eventos (Salida):** Este script no invoca eventos de Unity (`UnityEvent`, `Action`) para notificar a otros sistemas. Su principal efecto de "salida" es la carga de una nueva escena (`SceneManager.LoadScene`), lo que implica un cambio completo del contexto del juego.
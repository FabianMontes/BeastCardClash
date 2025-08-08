# `TutorialMenuManager.cs`

## 1. Propósito General
Este script gestiona la visualización y navegación de los paneles del tutorial dentro del juego. Se encarga de controlar qué página del tutorial se muestra al jugador y de manejar las transiciones entre ellas, así como la opción de finalizar el tutorial y cargar la escena principal del juego.

## 2. Componentes Clave

### `enum CurrentPanel`
- **Descripción:** Una enumeración que define los diferentes estados o páginas del menú de tutorial. Sirve para llevar un registro del panel de tutorial que se está mostrando actualmente al jugador. Los valores posibles son `Panel1`, `Panel2` y `Panel3`.

### `TutorialMenuManager`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, es la principal responsable de controlar la lógica y la interfaz de usuario del menú de tutorial. Gestiona la visibilidad de los paneles, la navegación entre ellos mediante un botón de "Siguiente", y la transición a la escena principal del juego.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] private RawImage panel1`, `panel2`, `panel3`: Tres referencias a componentes `RawImage` en la interfaz de usuario. Cada uno representa una página diferente del tutorial y su estado `gameObject.SetActive` controla su visibilidad.
    - `[SerializeField] private Button NextButton`: Una referencia al botón utilizado para avanzar a la siguiente página del tutorial. Se utiliza específicamente para ocultarlo una vez que el jugador llega a la última página del tutorial, indicando que no hay más páginas para avanzar.

- **Métodos Principales:**
    - `void Start()`: Este método se invoca una vez al inicio del ciclo de vida del script, antes de la primera actualización de frame. Su función principal es inicializar el tutorial mostrando el primer panel (`panel1`) al jugador.
        ```csharp
        void Start()
        {
            ShowPanel1();
        }
        ```
    - `void ShowPanel1()`: Activa el `panel1` y desactiva los `panel2` y `panel3`. Establece `CurrentPanel` a `CurrentPanel.Panel1`. Este método se usa para mostrar la primera página del tutorial.
    - `void ShowPanel2()`: Activa el `panel2` y desactiva los `panel1` y `panel3`. Establece `CurrentPanel` a `CurrentPanel.Panel2`. Utilizado para mostrar la segunda página.
    - `void ShowPanel3()`: Activa el `panel3` y desactiva los `panel1` y `panel2`. Establece `CurrentPanel` a `CurrentPanel.Panel3`. Además, este método oculta el `NextButton`, ya que no hay más páginas de tutorial a las que avanzar.
        ```csharp
        public void ShowPanel3()
        {
            // ... (activación/desactivación de paneles)
            NextButton.gameObject.SetActive(false); // Ocultamos el botón de siguiente
        }
        ```
    - `void OnClick()`: Este método está diseñado para ser invocado por el evento `OnClick` de un botón de UI (probablemente el `NextButton`). Determina la acción a tomar basándose en el valor actual de `CurrentPanel`:
        - Si el `CurrentPanel` es `Panel1`, llama a `ShowPanel2()`.
        - Si es `Panel2`, llama a `ShowPanel3()`.
        - Si es `Panel3`, teóricamente llamaría a `SkipButton()`, pero esta rama es inalcanzable en la práctica porque el `NextButton` se desactiva cuando se muestra `Panel3`.
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
                    // Cuando llegamos al tercer y último panel
                    // El botón desaparece, asi que esta parte nunca se ejecuta, pero puedes mantener este código si lo deseas
                    SkipButton();
                    break;
                default:
                    break;
            }
        }
        ```
    - `void SkipButton()`: Carga la escena de Unity con el nombre "World". Este método permite al jugador saltar el tutorial y comenzar el juego principal. Se asume que también estaría asignado a un botón "Saltar Tutorial" en la interfaz.
        ```csharp
        public void SkipButton()
        {
            SceneManager.LoadScene("World");
        }
        ```

- **Lógica Clave:**
    La lógica principal de este script gira en torno a una máquina de estados simple definida por la enumeración `CurrentPanel`. El método `OnClick()` actúa como el transicionador de estados, moviendo al jugador secuencialmente a través de las páginas del tutorial (`Panel1` -> `Panel2` -> `Panel3`). Una vez que el `Panel3` es mostrado, el botón para avanzar (`NextButton`) se desactiva, impidiendo más transiciones a través de `OnClick()` y señalando el final del tutorial de paginación. La navegación a la escena principal (`World`) se gestiona por el método `SkipButton()`.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, para su correcto funcionamiento en la escena, requiere que se le asignen referencias a tres componentes `RawImage` (para los paneles visuales) y un componente `Button` (para el botón de "Siguiente" o "Avanzar") en el Inspector de Unity.

- **Eventos (Entrada):**
    - El método `OnClick()` está diseñado para ser suscrito al evento `OnClick()` de un componente `Button` en la UI de Unity. Esto permite que el script reaccione a la interacción del usuario con el botón de navegación del tutorial.
    - El método `SkipButton()` también está destinado a ser suscrito al evento `OnClick()` de un botón de UI, típicamente un botón de "Saltar Tutorial", para permitir al jugador salir del tutorial en cualquier momento.

- **Eventos (Salida):**
    Este script no invoca explícitamente eventos (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas. Sus acciones se manifiestan directamente a través de la modificación de la interfaz de usuario (activar/desactivar paneles, ocultar botones) y la carga de escenas.
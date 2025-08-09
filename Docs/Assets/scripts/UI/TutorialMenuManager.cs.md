# `TutorialMenuManager.cs`

## 1. Propósito General
Este script es el controlador principal para la interfaz de usuario del tutorial del juego. Su función es gestionar la visualización secuencial de las diferentes páginas (paneles) del tutorial y manejar la transición final hacia la escena principal del juego una vez que el tutorial ha sido completado o saltado.

## 2. Componentes Clave

### `CurrentPanel`
`CurrentPanel` es un `enum` que define los estados posibles o las "páginas" dentro del menú de tutorial. Contiene tres valores: `Panel1`, `Panel2` y `Panel3`, cada uno representando una sección distinta del tutorial. Este `enum` se utiliza internamente para mantener un registro del panel que se está mostrando actualmente y para dictar el flujo de navegación.

### `TutorialMenuManager`
- **Descripción:** La clase `TutorialMenuManager` es un componente de Unity (`MonoBehaviour`) que debe adjuntarse a un `GameObject` en la escena del menú del tutorial. Su responsabilidad es orquestar la interacción del usuario con los paneles del tutorial, incluyendo su activación/desactivación y la lógica para avanzar a través de ellos hasta la carga de la siguiente escena.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] private RawImage panel1`, `panel2`, `panel3`: Estas variables son referencias a los componentes `RawImage` en la jerarquía de la UI de Unity, que visualmente representan cada uno de los paneles (o páginas) del tutorial. Se configuran desde el Inspector de Unity para que el script pueda controlar su visibilidad (activar/desactivar sus `GameObjects`).
    - `private CurrentPanel CurrentPanel`: Esta variable privada, del tipo `enum CurrentPanel`, almacena el estado actual del tutorial, indicando qué panel está siendo mostrado al jugador en un momento dado.
    - `[SerializeField] private Button NextButton`: Una referencia al componente `Button` de la interfaz de usuario que permite al jugador avanzar a la siguiente página del tutorial. Aunque el código para ocultar este botón en el último panel está comentado, la referencia existe para una posible implementación futura.

- **Métodos Principales:**
    - `void Start()`: Este es un método del ciclo de vida de Unity, ejecutado una vez cuando el script se inicializa. Al arrancar, su función es asegurar que el tutorial comience en la primera página, lo cual logra llamando al método `ShowPanel1()`.
    - `public void ShowPanel1()`, `public void ShowPanel2()`, `public void ShowPanel3()`: Estos tres métodos públicos son los encargados de gestionar la visibilidad de los paneles del tutorial. Cada método activa el `GameObject` de su panel correspondiente (`panel1`, `panel2`, o `panel3`) y desactiva los `GameObjects` de los otros dos paneles, garantizando que solo una página del tutorial sea visible a la vez. Adicionalmente, cada método actualiza la variable interna `CurrentPanel` para reflejar el panel que ha sido activado.
    - `public void OnClick()`: Este método es el controlador principal para la interacción del usuario con el botón de "siguiente". Está diseñado para ser asignado al evento `OnClick()` de un `Button` en el Inspector de Unity. Utiliza una estructura `switch` basada en el `CurrentPanel` actual para determinar el siguiente paso en la secuencia del tutorial:
        - Si el panel actual es `CurrentPanel.Panel1`, llama a `ShowPanel2()`.
        - Si el panel actual es `CurrentPanel.Panel2`, llama a `ShowPanel3()`.
        - Si el panel actual es `CurrentPanel.Panel3` (la última página del tutorial), en lugar de avanzar a otro panel, llama a `SkipButton()` para finalizar el tutorial.
    - `public void SkipButton()`: Este método es invocado cuando el tutorial ha sido completado (después de que el `OnClick()` se ejecuta en el `Panel3`). Su función principal es cargar la escena del juego principal, llamada "World", utilizando el `SceneManager` de Unity.
        ```csharp
        SceneManager.LoadScene("World");
        ```

- **Lógica Clave:**
    La lógica central de `TutorialMenuManager` se basa en una máquina de estados simple para la navegación del tutorial. El script mantiene un estado (`CurrentPanel`) que representa la página actual. Al inicio, se fuerza el estado al `Panel1`. Cada pulsación del botón "siguiente" (gestionada por `OnClick()`) avanza este estado secuencialmente (`Panel1` -> `Panel2` -> `Panel3`). Al alcanzar el `Panel3` y volver a pulsar "siguiente", la lógica bifurca para finalizar el tutorial, cargando directamente la escena "World". Esta aproximación garantiza un flujo lineal y predecible para el tutorial.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, lo que significa que no requiere que otros componentes específicos estén presentes en el mismo `GameObject` para funcionar correctamente.
- **Eventos (Entrada):** Este script actúa como un oyente para los eventos de clic de los botones de la interfaz de usuario. Específicamente, el método `public void OnClick()` está destinado a ser asignado al evento `OnClick()` del `Button` de "Siguiente" (`NextButton`) a través del Inspector de Unity.
- **Eventos (Salida):** El `TutorialMenuManager` no emite eventos personalizados (`UnityEvent` o `Action`) a otros sistemas. Su principal efecto de "salida" es la carga de una nueva escena de juego (`SceneManager.LoadScene("World")`), lo que representa una transición de estado global del juego.
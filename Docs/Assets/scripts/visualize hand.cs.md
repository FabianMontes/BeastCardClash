# `visualize hand.cs`

## 1. Propósito General
Este script controla la visibilidad y la interactividad de la mano de cartas del jugador en la interfaz de usuario. Su función principal es sincronizar el estado activo de la mano de cartas y sus componentes individuales con el estado de activación de un objeto "guía" específico en la jerarquía de la escena.

## 2. Componentes Clave

### `visualizehand`
-   **Descripción:** `visualizehand` es un script de tipo `MonoBehaviour` que gestiona la presentación visual de la mano de cartas del jugador. Se encarga de habilitar o deshabilitar los componentes `Image` y `Button` de la mano y sus cartas individuales, así como la activación de ciertos elementos internos de cada carta, basándose en la propiedad `activeSelf` de un `GameObject` de referencia. El atributo `[DefaultExecutionOrder(2)]` indica que este script se ejecutará después de aquellos con un orden de ejecución inferior (0 o 1).

-   **Variables Públicas / Serializadas:**
    -   `[SerializeField] Transform guide`: Esta variable es una referencia a un objeto `Transform` en la jerarquía de Unity. Se serializa para poder ser asignada a través del Inspector de Unity. El `GameObject` asociado a este `Transform` (`guide.gameObject`) actúa como un interruptor maestro: si está activo, la mano de cartas se muestra y se vuelve interactiva; si está inactivo, la mano se oculta y se deshabilita.

-   **Métodos Principales:**
    -   `void Start()`: Este método se invoca una vez al inicio del ciclo de vida del script. Su propósito es inicializar el estado de la mano de cartas a oculto y no interactivo. Desactiva los componentes `Image` tanto de la "mano" principal (el segundo hijo del `GameObject` al que está adjunto este script) como de cada una de sus cartas hijo.
    -   `void Update()`: Este método se ejecuta en cada frame del juego. En cada actualización, el script verifica el estado de activación (`activeSelf`) del `GameObject` referenciado por `guide`. Luego, utiliza este estado para:
        *   Controlar la propiedad `enabled` del componente `Image` de la mano principal.
        *   Controlar la propiedad `enabled` del componente `Image` de cada carta individual en la mano.
        *   Controlar la propiedad `enabled` del componente `Button` de cada carta individual, afectando su interactividad.
        *   Controlar la propiedad `activeSelf` del primer hijo de cada carta (presumiblemente un elemento visual o de texto dentro de la carta).

-   **Lógica Clave:**
    La lógica principal de `visualizehand` es la sincronización bidireccional entre el estado de activación del `guide` `GameObject` y la visibilidad/interactividad de la mano de cartas. El script asume que la estructura de la mano es la siguiente: el `GameObject` al que está adjunto este script tiene un segundo hijo (`transform.GetChild(1)`) que representa el contenedor de la mano, y los hijos de este contenedor son las cartas individuales. Al iterar sobre estos hijos (las cartas), el script ajusta sus componentes `Image` y `Button`, así como la activación de su primer sub-hijo, para reflejar el estado del `guide`.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere que el segundo hijo del `GameObject` al que está adjunto tenga un componente `Image`, y que los hijos de este (las "cartas") tengan componentes `Image` y `Button`. También asume que cada "carta" tiene al menos un hijo para poder activar/desactivar su primer sub-elemento.
-   **Eventos (Entrada):** La principal "entrada" de este script es el estado de activación del `GameObject` asociado a la variable `guide`, que se monitorea continuamente en el método `Update`. No se suscribe a eventos explícitos como clics de botones o cambios de estado de interfaz de usuario.
-   **Eventos (Salida):** Este script no invoca ni emite ningún tipo de evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios de estado. Su impacto se limita a la modificación directa de las propiedades de los componentes `Image`, `Button` y la activación de `GameObjects` dentro de la jerarquía de la mano de cartas.
# `PickerCard.cs`

## 1. Propósito General
Este script gestiona la visibilidad y el contenido de un componente de interfaz de usuario (UI) que muestra la carta actualmente "recogida" o seleccionada por un jugador. Su función principal es actualizar el texto con la ID de la carta y controlar su aparición/desaparición en función de las fases del combate del juego.

## 2. Componentes Clave

### `PickerCard`
-   **Descripción:** Esta clase es un componente de Unity (`MonoBehaviour`) que se encarga de la lógica visual y de datos de un elemento de UI que representa la carta activa del jugador. Se espera que este componente sea parte de la jerarquía de un objeto `Player`.
-   **Variables Públicas / Serializadas:**
    -   `[SerializeField] HolderPlay holderPlay`: Aunque está serializada y visible en el Inspector de Unity para asignación, esta variable no es utilizada en la lógica actual del script proporcionado. Su nombre sugiere que podría estar destinada a interactuar con un componente que maneja el área de juego (`HolderPlay`).
-   **Métodos Principales:**
    -   `void Start()`: Este método se invoca una vez al inicio del ciclo de vida del script. Su propósito es inicializar referencias a otros componentes:
        -   Obtiene una referencia al componente `TextMeshProUGUI` dentro de sus hijos, que se utilizará para mostrar el texto de la ID de la carta.
        -   Obtiene una referencia al componente `Player` de su padre, permitiendo al script acceder a información y acciones específicas del jugador.
        -   Inicializa `prevSetMoment` con un valor predeterminado de `SetMoments.PickDice`.
        -   Invoca `Visib(false)` para asegurar que el elemento de UI esté inicialmente oculto.
    -   `void Update()`: Este método se ejecuta en cada frame del juego. Su función principal es monitorear el estado actual del combate y actualizar la UI de la carta seleccionada:
        -   Obtiene la fase actual del juego (`SetMoments`) desde el sistema `Combatjudge`.
        -   Controla la visibilidad del UI de la carta: se hace visible si la fase es `SetMoments.PickCard` y el jugador está "luchando" (`IsFigthing()`). Se oculta si la fase es `SetMoments.Loop` o `SetMoments.End`.
        -   Obtiene la `Card` actualmente seleccionada por el jugador a través de `player.getPicked()`.
        -   Actualiza el texto del `TextMeshProUGUI` con la ID de la carta seleccionada. Si no hay ninguna carta seleccionada (`card == null`), el texto se vacía.
    -   `private void Visib(bool isVisible)`: Un método auxiliar que controla la visibilidad del elemento de UI. Desactiva/activa el primer hijo del transform (que se asume es el contenedor visual del texto y otros elementos) y habilita/deshabilita el componente `Image` adjunto al propio objeto `PickerCard`.

-   **Lógica Clave:**
    La lógica central de `PickerCard` gira en torno a la sincronización con el ciclo de combate y el estado del jugador. Durante la fase `PickCard` y solo si el jugador está activamente involucrado en el combate, el componente se hace visible para guiar al jugador. En otras fases relevantes (`Loop`, `End`), el componente se oculta. El texto que muestra la ID de la carta se actualiza dinámicamente cada frame para reflejar la carta que el jugador tiene actualmente "recogida", lo que proporciona una retroalimentación visual constante.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:**
    -   Este script requiere la presencia de un componente `TextMeshProUGUI` en uno de sus hijos para poder mostrar la ID de la carta.
    -   Requiere un componente `Image` adjunto al mismo `GameObject` que `PickerCard` para controlar su propia visibilidad visual.
    -   Depende de un componente `Player` presente en uno de sus objetos padre en la jerarquía de Unity para acceder a información específica del jugador.
-   **Eventos (Entrada):**
    Este script no se suscribe explícitamente a eventos de Unity (`UnityEvent`) o delegates de C# (`Action`). En su lugar, utiliza el método `Update` del ciclo de vida de Unity para realizar un "polling" (consulta constante) del estado del juego a través de `Combatjudge.combatjudge.GetSetMoments()` y la información del jugador (`player.IsFigthing()`, `player.getPicked()`) para determinar su comportamiento.
-   **Eventos (Salida):**
    Este script no invoca ningún evento (`UnityEvent` o `Action`) para notificar a otros sistemas. Su función es puramente de visualización y reacción a los cambios de estado internos del juego y del jugador.
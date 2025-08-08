# `PickerCard.cs`

## 1. Propósito General
Este script (`PickerCard`) es un componente de UI responsable de gestionar la visibilidad y el contenido visual del espacio donde se muestra la carta que un jugador (`Figther`) tiene actualmente seleccionada o "recogida". Su rol principal es sincronizar la UI con el estado del juego y la carta activa del jugador.

## 2. Componentes Clave

### `PickerCard`
- **Descripción:** La clase `PickerCard` es un `MonoBehaviour` que controla la representación visual de la carta activa de un jugador en la interfaz de usuario. Se encarga de mostrar u ocultar este elemento de UI y de actualizar el texto con la identificación de la carta actualmente en foco.
- **Variables Públicas / Serializadas:**
    - `[SerializeField] HolderPlay holderPlay`: Esta variable es una referencia a otro componente `HolderPlay`. Aunque está serializada y es visible en el Inspector de Unity para permitir su asignación, no se utiliza en ninguna lógica dentro del código actual de `PickerCard.cs`. Podría ser una dependencia para funcionalidades futuras o utilizada por otros scripts que interactúen con `PickerCard`.
- **Métodos Principales:**
    - `void Start()`: Este es un método del ciclo de vida de Unity que se ejecuta una vez al inicio, después de que el objeto es instanciado.
        - Se inicializa `textMeshPro` obteniendo el componente `TextMeshProUGUI` de uno de los objetos hijos del GameObject actual. Este componente es el encargado de mostrar el ID de la carta.
        - Se obtiene una referencia al componente `Figther` que se encuentra en un objeto padre en la jerarquía. Este `Figther` representa al jugador cuyas cartas se están gestionando.
        - La variable `prevSetMoment` se inicializa con `SetMoments.PickDice`; sin embargo, la lógica que la utilizaba para detectar cambios de estado está actualmente comentada.
        - Se invoca `Visib(false)` para asegurar que el elemento de la UI esté oculto por defecto al inicio del juego o escena.
    - `void Update()`: Este es un método del ciclo de vida de Unity que se ejecuta una vez por cada frame.
        - El script consulta el estado actual del juego (`SetMoments momo`) a través del sistema de combate global `Combatjudge.combatjudge.GetSetMoments()`. Esto indica una fuerte dependencia de un sistema centralizado de gestión del estado de la partida.
        - Basándose en el estado del juego:
            - Si el momento actual es `SetMoments.PickCard` y el jugador (`player`) está activamente en combate (`player.IsFigthing()`), el elemento de la UI se hace visible llamando a `Visib(true)`.
            - Si el momento actual es `SetMoments.Loop` o `SetMoments.End`, el elemento de la UI se oculta llamando a `Visib(false)`.
        - Se obtiene la referencia a la carta que el jugador tiene actualmente "recogida" o activa, utilizando `player.getPicked()`.
        - Finalmente, el texto mostrado en la UI (`textMeshPro.text`) se actualiza con el ID de la carta obtenida (`card.GetID()`). Si no hay ninguna carta seleccionada (es decir, `card` es `null`), el texto se establece como vacío.
    - `private void Visib(bool isVisible)`: Este es un método auxiliar privado utilizado para controlar la visibilidad del elemento de la UI que representa la carta.
        - Activa o desactiva el primer objeto hijo (`transform.GetChild(0).gameObject`) del GameObject al que está adjunto este script. Este hijo probablemente contiene el `TextMeshProUGUI` y otros elementos visuales internos de la carta.
        - Habilita o deshabilita el componente `Image` del propio GameObject `PickerCard`. Esto sugiere que el `PickerCard` en sí mismo tiene un componente `Image` que podría servir como fondo o contenedor visual para la carta.
- **Lógica Clave:**
    La lógica central de `PickerCard` reside en su método `Update`. En cada frame, el script monitorea el estado global de la partida a través del `Combatjudge` y el estado de la carta activa del jugador (`Figther`). Utilizando esta información, determina dinámicamente si el elemento de la UI debe ser visible y cuál es el texto (el ID de la carta) que debe mostrar. De este modo, `PickerCard` funciona como un visor adaptable para la carta actualmente en foco del jugador, ajustándose a las diferentes fases y eventos del combate.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]` para forzar la presencia de otros componentes en el mismo GameObject. Sin embargo, para su correcto funcionamiento visual y lógico, asume la existencia de:
    - Un componente `TextMeshProUGUI` en uno de sus objetos hijos (para `textMeshPro`).
    - Un componente `Image` en el propio GameObject `PickerCard` (para `transform.GetComponent<Image>()`).
    - Un componente `Figther` en uno de sus objetos padres en la jerarquía (para la variable `player`).
- **Eventos (Entrada):** `PickerCard` no se suscribe explícitamente a `UnityEvent`s ni `Action`s. Sin embargo, depende fundamentalmente de la información de estado que obtiene de forma síncrona mediante la llamada a `Combatjudge.combatjudge.GetSetMoments()`, lo que puede considerarse una forma de "entrada" de información sobre el flujo del juego.
- **Eventos (Salida):** Este script no invoca ningún evento (`UnityEvent`, `Action`, etc.) ni notifica a otros sistemas. Su función es puramente de presentación y actualización visual de la interfaz de usuario.
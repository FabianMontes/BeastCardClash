# `PickerCard.cs`

## 1. Propósito General
El script `PickerCard` es responsable de gestionar la visibilidad y el contenido visual de la interfaz de usuario que muestra la carta actualmente "recogida" o seleccionada por un jugador (`Figther`). Interactúa principalmente con el sistema de control de flujo del combate (`Combatjudge`) para determinar cuándo debe ser visible y actualiza su texto basándose en la carta seleccionada por el jugador asociado.

## 2. Componentes Clave

### `PickerCard`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, controla la representación visual de la carta que un jugador ha seleccionado o está a punto de seleccionar durante las fases específicas del combate. Se encarga de mostrar u ocultar la interfaz y de actualizar el texto con la identificación de la carta.
- **Variables Públicas / Serializadas:**
    - `TextMeshProUGUI textMeshPro;`: Una referencia al componente TextMeshProUGUI encontrado en los hijos de este GameObject. Se utiliza para mostrar la ID de la carta seleccionada.
    - `Figther player;`: Una referencia al componente `Figther` del GameObject padre. Representa al jugador cuya carta seleccionada se está mostrando.
    - `[SerializeField] HolderPlay holderPlay;`: Una referencia a un componente `HolderPlay`. Aunque está serializada y es visible en el Inspector de Unity, esta variable no es utilizada en la lógica del script proporcionado.
- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se ejecuta una vez al inicio del componente.
        - Inicializa `textMeshPro` obteniendo el componente `TextMeshProUGUI` de entre sus hijos.
        - Inicializa `player` obteniendo el componente `Figther` del GameObject padre.
        - Oculta la interfaz de la carta llamando a `Visib(false)`.
    - `void Update()`: Este método del ciclo de vida de Unity se ejecuta una vez por fotograma.
        - Obtiene el estado actual del juego (`SetMoments`) desde el `Combatjudge` singleton.
        - Controla la visibilidad de la interfaz: la muestra si el estado es `SetMoments.PickCard` y el jugador está "luchando" (`player.IsFigthing()`); la oculta si el estado es `SetMoments.Loop` o `SetMoments.End`.
        - Obtiene la carta actualmente "recogida" o seleccionada del objeto `player`.
        - Actualiza el texto de `textMeshPro` con la ID de la carta obtenida, o lo deja vacío si no hay ninguna carta seleccionada.
    - `private void Visib(bool isVisible)`: Este método privado controla la visibilidad de los elementos visuales del picker de cartas.
        - Activa o desactiva el primer hijo del GameObject (`transform.GetChild(0).gameObject.SetActive(isVisible)`). Este hijo probablemente contiene el diseño gráfico de la carta.
        - Habilita o deshabilita el componente `Image` adjunto al propio GameObject del `PickerCard` (`transform.GetComponent<Image>().enabled = isVisible`).
- **Lógica Clave:**
    La lógica principal de `PickerCard` reside en su método `Update`. Monitorea constantemente el estado actual del juego (`SetMoments`) a través del `Combatjudge`. En función de este estado y de si el `player` está activo en combate, la interfaz de la carta se hace visible o se oculta. Simultáneamente, el script recupera la carta que el `player` ha "recogido" y actualiza dinámicamente el texto mostrado en la UI con la identificación de esa carta, proporcionando retroalimentación visual al jugador.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, asume la presencia de un componente `TextMeshProUGUI` en uno de sus hijos y un componente `Figther` en su GameObject padre. También asume que el propio GameObject `PickerCard` tiene un componente `Image`.
- **Eventos (Entrada):** Este script no se suscribe explícitamente a eventos de Unity (`UnityEvent` o C# `Action`). En su lugar, sondea el estado del juego directamente desde el singleton `Combatjudge.combatjudge` en cada `Update`.
- **Eventos (Salida):** Este script no invoca ni emite ningún evento para notificar a otros sistemas. Sus acciones se limitan a actualizar su propia interfaz visual.
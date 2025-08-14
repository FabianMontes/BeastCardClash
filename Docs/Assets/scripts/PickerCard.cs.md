# `PickerCard.cs`

## 1. Propósito General
Este script gestiona la visualización de la carta que un jugador ha "seleccionado" (o "picado") durante las diferentes fases del combate. Su rol principal es mostrar u ocultar la representación visual de esta carta en la interfaz de usuario, actualizando su texto con la identificación de la carta actual. Interactúa directamente con el estado del `Figther` (jugador) al que pertenece y con el sistema global de fases de combate (`Combatjudge`).

## 2. Componentes Clave

### PickerCard
- **Descripción:** La clase `PickerCard` es un componente de `MonoBehaviour` que controla la visibilidad y el contenido de una carta en la interfaz de usuario, representando la carta actualmente seleccionada por un jugador (`Figther`). Se espera que este script esté adjunto a un objeto UI que visualiza la carta seleccionada.
- **Variables Públicas / Serializadas:**
    - `TextMeshProUGUI textMeshPro`: Una referencia al componente TextMeshProUGUI que se utiliza para mostrar el ID de la carta seleccionada. Se obtiene automáticamente de los hijos del GameObject al inicio.
    - `Figther player`: Una referencia al componente `Figther` del jugador propietario. Se obtiene automáticamente del componente padre del GameObject al inicio. Este `Figther` es quien "posee" la carta seleccionada que se muestra.
    - `[SerializeField] HolderPlay holderPlay`: Una referencia a un componente `HolderPlay`. Aunque está serializado para ser asignado desde el Inspector de Unity, actualmente no se utiliza en la lógica proporcionada en este script.
- **Métodos Principales:**
    - `void Start()`: Este método se ejecuta una vez al inicio, antes del primer `Update`.
        - Inicializa `textMeshPro` buscando un componente `TextMeshProUGUI` entre los hijos del GameObject.
        - Inicializa `player` buscando un componente `Figther` en los padres del GameObject.
        - Llama a `Visib(false)` para asegurar que la carta esté oculta al inicio del juego.
    - `void Update()`: Este método se ejecuta en cada frame.
        - Obtiene el momento (fase) actual del combate del sistema `Combatjudge`.
        - Basándose en la fase actual del combate (`SetMoments`), decide si la carta debe ser visible:
            - Se hace visible si la fase es `SetMoments.PickCard` y el jugador (`player`) está activamente en combate (`player.IsFigthing()`).
            - Se oculta si la fase es `SetMoments.Loop` o `SetMoments.End`.
        - Actualiza el texto de `textMeshPro` con el ID de la carta que el jugador tiene "picada" (`player.getPicked()`). Si no hay carta picada, el texto se establece en vacío.
    - `private void Visib(bool isVisible)`: Un método auxiliar privado que controla la visibilidad de los elementos visuales de la carta.
        - Desactiva/activa el primer hijo del GameObject (`transform.GetChild(0).gameObject`) y el componente `Image` adjunto al GameObject principal. El primer hijo probablemente contiene los detalles visuales de la carta, mientras que el `Image` podría ser el fondo o el contenedor de la carta.
- **Lógica Clave:**
    La lógica central de `PickerCard` reside en su método `Update`, donde de manera continua:
    1.  Consulta la fase actual del combate (`Combatjudge.GetSetMoments()`).
    2.  Ajusta la visibilidad de la carta en la UI (`Visib()`) dependiendo de si es la fase de selección de carta (`PickCard`) y si el jugador está activo, o si el combate ha terminado (`Loop`, `End`).
    3.  Obtiene la carta actualmente "picada" por el jugador (`player.getPicked()`) y actualiza el texto de la UI para reflejar su ID.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere que el GameObject al que está adjunto tenga un componente `Image` y que uno de sus hijos tenga un componente `TextMeshProUGUI` para la correcta visualización. También depende de un componente `Figther` en su jerarquía padre.
- **Eventos (Entrada):** Este script no se suscribe explícitamente a eventos de UI (como `onClick`) ni a `UnityEvent` o `Action`s. Su comportamiento se basa en la lectura del estado actual del sistema `Combatjudge` (`Combatjudge.combatjudge.GetSetMoments()`) y del `Figther` padre (`player.IsFigthing()`, `player.getPicked()`).
- **Eventos (Salida):** Este script no invoca ningún evento (`UnityEvent`, `Action`) para notificar a otros sistemas sobre cambios o acciones. Su rol es puramente de visualización y reacción al estado del juego.
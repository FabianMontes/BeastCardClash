# `PickerCard.cs`

## 1. Propósito General
Este script tiene como propósito principal gestionar la visualización de la carta que un jugador ha "elegido" o tiene activa en un momento dado del juego. Controla la visibilidad de esta representación visual de la carta y actualiza el texto que muestra su identificador, basándose en el estado actual del combate, que es determinado por el sistema `Combatjudge`.

## 2. Componentes Clave

### `PickerCard`
- **Descripción:** La clase `PickerCard` hereda de `MonoBehaviour`, lo que significa que puede ser adjuntada a un GameObject en la jerarquía de Unity. Su función principal es servir como una representación visual dinámica de la carta activa de un jugador, mostrando u ocultando la interfaz de usuario asociada y actualizando el texto con la información de la carta según el contexto del juego.

- **Variables Públicas / Serializadas:**
    - `TextMeshProUGUI textMeshPro`: Una referencia privada al componente TextMeshProUGUI que se encuentra como hijo del GameObject al que está adjunto este script. Se utiliza para mostrar el identificador (ID) de la carta actualmente elegida.
    - `Figther player`: Una referencia privada al script `Figther` (presumiblemente el jugador o entidad combatiente) que es padre del GameObject. Este `Figther` es quien posee la carta que `PickerCard` debe visualizar.
    - `[SerializeField] HolderPlay holderPlay`: Una referencia serializada a un componente `HolderPlay`. Aunque no se utiliza directamente en la lógica visible de este script, su serialización sugiere que es una dependencia o parte de un sistema más amplio donde este `PickerCard` podría estar anidado o con el que interactúa indirectamente. Su propósito exacto en relación con `PickerCard` no se detalla en este código específico.

- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se llama una vez antes de la primera actualización del frame. Se encarga de la inicialización de referencias clave:
        - Obtiene el componente `TextMeshProUGUI` de sus hijos.
        - Obtiene el componente `Figther` de su padre.
        - Inicializa la variable `prevSetMoment` con `SetMoments.PickDice` (aunque esta variable y su lógica asociada están comentadas en `Update`).
        - Llama a `Visib(false)` para asegurar que la visualización de la carta esté inicialmente oculta.
        ```csharp
        void Start()
        {
            textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
            player = GetComponentInParent<Figther>();
            prevSetMoment = SetMoments.PickDice; // Lógica comentada en Update
            Visib(false);
        }
        ```
    - `void Update()`: Este método se llama en cada frame y es el corazón de la lógica de actualización del `PickerCard`.
        - Consulta el `Combatjudge` (un singleton o una referencia estática `Combatjudge.combatjudge`) para obtener el `SetMoments` actual del juego.
        - Basándose en este momento, ajusta la visibilidad de la carta: si el momento es `SetMoments.PickCard` y el jugador (`player`) está activamente combatiendo (`IsFigthing()`), la carta se hace visible; si el momento es `SetMoments.Loop` o `SetMoments.End`, la carta se oculta.
        - Finalmente, recupera la carta actualmente "elegida" por el jugador (`player.getPicked()`) y actualiza el texto de `textMeshPro` para mostrar el ID de la carta, o un texto vacío si no hay carta elegida.
        ```csharp
        void Update()
        {
            SetMoments momo = Combatjudge.combatjudge.GetSetMoments();
            if (momo == SetMoments.PickCard && player.IsFigthing()) Visib(true);
            if (momo == SetMoments.Loop || momo == SetMoments.End) Visib(false);

            Card card = player.getPicked();
            textMeshPro.text = card == null ? "" : card.GetID();
        }
        ```
    - `private void Visib(bool isVisible)`: Un método auxiliar privado utilizado para controlar la visibilidad del elemento visual de la carta.
        - Oculta o muestra el primer hijo del GameObject (que presumiblemente contiene los elementos gráficos de la carta).
        - Habilita o deshabilita el componente `Image` adjunto al GameObject principal del `PickerCard`, afectando su renderizado visual.
        ```csharp
        private void Visib(bool isVisible)
        {
            transform.GetChild(0).gameObject.SetActive(isVisible);
            transform.GetComponent<Image>().enabled = isVisible;
        }
        ```

- **Lógica Clave:**
    La lógica central del `PickerCard` reside en su método `Update`, donde monitorea continuamente el estado del juego a través del `Combatjudge`. Esta supervisión constante le permite reaccionar a los cambios en el flujo del combate (específicamente, los `SetMoments`) para decidir cuándo la interfaz de usuario de la carta debe ser visible y cuándo no. Simultáneamente, mantiene actualizada la información textual de la carta mostrada, reflejando siempre la carta actual elegida por el `Figther` asociado.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, lo que significa que no impone la presencia obligatoria de otros componentes en el mismo GameObject en tiempo de edición. Sin embargo, para su correcto funcionamiento en tiempo de ejecución, asume la existencia de un componente `TextMeshProUGUI` en uno de sus hijos y un componente `Image` en el mismo GameObject. También requiere que su GameObject padre tenga un script `Figther` adjunto para poder obtener la referencia al jugador.

- **Eventos (Entrada):** `PickerCard` no se suscribe explícitamente a eventos tradicionales de Unity (como `UnityEvent` o acciones de UI como `onClick`). En su lugar, obtiene su entrada y reacciona a los cambios en el estado del juego mediante una consulta directa al sistema `Combatjudge` a través de su método `GetSetMoments()`. Esto es una forma de "pulling" de datos en lugar de un "push" de eventos.

- **Eventos (Salida):** Este script no invoca ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas. Su función es puramente observacional y de actualización de UI interna.
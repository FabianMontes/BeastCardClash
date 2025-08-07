Aquí tienes la documentación técnica para el script `PickerCard.cs`.

---

# `PickerCard.cs`

## 1. Propósito General
Este script `PickerCard` es un componente `MonoBehaviour` responsable de gestionar la interfaz de usuario (UI) que muestra la carta actualmente "recogida" o seleccionada por un jugador (`Figther`). Controla la visibilidad de este elemento UI y actualiza el texto con el identificador de la carta, basándose en la fase actual del combate.

## 2. Componentes Clave

### `PickerCard`
- **Descripción:** Esta clase `PickerCard` es un `MonoBehaviour` que se encarga de la visualización dinámica de la carta que un jugador ha "elegido" o tiene activa en un momento dado. Su función principal es mostrar u ocultar un elemento UI (que incluye un `TextMeshProUGUI` y un `Image`) y actualizar el texto con la información de la carta correspondiente.

- **Variables Públicas / Serializadas:**
    - `TextMeshProUGUI textMeshPro`: Una referencia al componente `TextMeshProUGUI` hijo, utilizado para mostrar el ID de la carta. Se obtiene dinámicamente en `Start()`.
    - `Figther player`: Una referencia al componente `Figther` del jugador asociado con este `PickerCard`. Se obtiene del padre en `Start()`.
    - `[SerializeField] HolderPlay holderPlay`: Una variable serializada en el Inspector de Unity. Aunque está presente, no se utiliza en la lógica actual de este script.

- **Métodos Principales:**
    - `void Start()`:
        Este método del ciclo de vida de Unity se llama una vez antes de la primera actualización del frame.
        *   Inicializa `textMeshPro` obteniendo el componente `TextMeshProUGUI` de los hijos del objeto de juego.
        *   Inicializa `player` obteniendo el componente `Figther` del padre del objeto de juego.
        *   Establece `prevSetMoment` a `SetMoments.PickDice` (aunque su uso posterior está comentado).
        *   Llama a `Visib(false)` para asegurarse de que el elemento UI esté oculto al inicio.

    - `void Update()`:
        Este método del ciclo de vida de Unity se llama una vez por frame. Es el corazón de la lógica de visibilidad y actualización de la carta.
        *   Obtiene la fase actual del combate (`SetMoments momo`) del sistema `Combatjudge`.
        *   Gestiona la visibilidad del UI:
            *   Si la fase es `SetMoments.PickCard` y el jugador (`player`) está en combate (`IsFigthing()`), el UI se hace visible.
            *   Si la fase es `SetMoments.Loop` o `SetMoments.End`, el UI se hace invisible.
            *   *Nota*: El bloque de código que compara `momo` con `prevSetMoment` está comentado, lo que significa que la lógica de visibilidad se evalúa en cada `Update` sin depender del cambio de fase.
        *   Obtiene la carta seleccionada por el jugador usando `player.getPicked()`.
        *   Actualiza el `textMeshPro.text`: si no hay carta seleccionada (`card == null`), el texto se establece en vacío; de lo contrario, se muestra el ID de la carta (`card.GetID()`).

    - `private void Visib(bool isVisible)`:
        Este método privado es una utilidad para controlar la visibilidad del elemento UI.
        *   Establece la actividad del primer hijo del `transform` (que se espera sea el elemento visual principal del UI, como un panel o el texto) según el valor de `isVisible`.
        *   Habilita o deshabilita el componente `Image` en el propio objeto de juego según `isVisible`.

- **Lógica Clave:**
La lógica central de `PickerCard` reside en su método `Update`. Este método monitorea continuamente la fase actual del combate (`SetMoments`) y el estado del jugador (`Figther`). Basado en estas condiciones, decide si el elemento UI de la carta "recogida" debe ser visible o no. Además, se encarga de mantener el texto del UI actualizado con el identificador de la carta que el `Figther` tiene actualmente seleccionada. La visibilidad es gestionada por el método auxiliar `Visib()`, que activa/desactiva el primer hijo y el componente `Image` del objeto.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, asume la presencia de un componente `TextMeshProUGUI` en uno de sus hijos y un componente `Image` en el mismo objeto de juego o en un objeto de juego con el que interactúa para controlar su visibilidad. También requiere un componente `Figther` en su objeto padre.

- **Eventos (Entrada):**
    El script `PickerCard` no se suscribe explícitamente a eventos de Unity (`UnityEvent`) o delegates (`Action`). En su lugar, "escucha" los cambios en el estado global del juego consultando directamente `Combatjudge.combatjudge.GetSetMoments()` en cada frame a través de su método `Update()`.

- **Eventos (Salida):**
    Este script no invoca ningún evento ni `UnityEvent` para notificar a otros sistemas o componentes sobre cambios en su estado o acciones. Su rol es puramente de visualización y actualización interna.

---
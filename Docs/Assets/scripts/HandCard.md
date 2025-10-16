# `HandCard`
El script `HandCard` es un componente fundamental en el desarrollo de la interfaz y la lógica de interacción de las cartas dentro del juego **Beast Card Clash**. Se encarga de gestionar la representación visual, la capacidad de interacción y la comunicación de acciones de una carta individual, ya sea que esta se encuentre en la mano del jugador lista para ser jugada o en un espacio de selección especial (`picker`). Al implementar las interfaces `IPointerEnterHandler` e `IPointerExitHandler`, también está preparado para responder a eventos de interacción del puntero, como el *hover*.

La clase `HandCard` se ejecuta con una ordenación de ejecución por defecto de `-4` (definida por `[DefaultExecutionOrder(-4)]`), lo que significa que se inicializa antes que la mayoría de los demás scripts en Unity. Esto es crucial para asegurar que la lógica de las cartas esté preparada y responda correctamente a los estados iniciales del juego.

Este script maneja dos roles principales a través de sus propiedades `playable` y `picker`:
*   **Carta jugable (`playable = true`):** Representa una carta en la mano del jugador que puede ser seleccionada para realizar una acción en el combate. Su interactividad está ligada a los `SetMoments` (momentos del juego) y al tipo de combate actual.
*   **Espacio de selección (`picker = true`):** Representa un *slot* o espacio para seleccionar una carta, que podría usarse para construir un mazo, elegir una recompensa o cualquier otra mecánica de selección que no implique jugar la carta directamente en el combate. Su visibilidad y comportamiento se adaptan a estos procesos de selección.

Interactúa principalmente con los siguientes componentes del proyecto:
*   `Card`: El objeto de datos que contiene la información real (elementos, habilidades, etc.) de la carta que este `HandCard` representa visualmente.
*   `Fighter`: El componente que representa al jugador o personaje que posee la mano de cartas. `HandCard` le notifica cuando se selecciona y juega una carta.
*   `CombatJudge`: Una instancia global que gestiona el estado actual del combate (`SetMoments`, `CombatType`), dictando cuándo una carta puede ser interactuable.
*   `Button`: El componente UI que permite la interactividad de la carta, gestionando los clics del usuario.
*   `Image`: Se utilizan varios componentes `Image` de los GameObjects hijos para controlar la apariencia visual y el feedback de interactividad de la carta.

# Métodos

## Métodos de Unity

### `Start`
Este método se ejecuta una vez al inicio, después de que el GameObject se activa. Se encarga de la inicialización de referencias y del estado visual y de interactividad de la carta:
*   Obtiene la referencia al componente `Fighter` que es el padre de este `HandCard` en la jerarquía, asumiendo que `HandCard` es un hijo de un GameObject que tiene un `Fighter`.
*   Inicializa `prevSetMoment` a `SetMoments.PickDice`, estableciendo un estado inicial para la lógica de actualización del juego.
*   Obtiene el componente `Button` adjunto al GameObject para controlar su interactividad.
*   Aplica lógica condicional basada en las propiedades `playable` y `picker`:
    *   Si `playable` es verdadero, llama a `clickable(false)` para asegurar que la carta no sea interactuable al inicio.
    *   Si `playable` es falso, oculta los elementos visuales principales de la carta llamando a `Visib(false)` y deshabilita explícitamente el `Button`. Esto sugiere que una carta no `playable` no debe ser vista ni interactuada como una opción de juego.
*   Llama a `SetCard(card)` para inicializar la carta con los datos definidos en el inspector y ajustar su visibilidad.
*   Activa o desactiva un GameObject hijo específico (`transform.GetChild(1).gameObject`) basado en si la carta está actuando como un `picker`. Este hijo parece ser un componente visual clave de la carta.

### `Update`
Este método se llama una vez por frame y es el corazón de la lógica de reacción a los cambios de estado del juego. Su función principal es determinar cuándo una carta en la mano (`playable = true`) puede ser seleccionada por el jugador, o cuándo un `picker` debe cambiar su visibilidad.

El método comienza obteniendo el momento actual del juego (`SetMoments`) desde el `CombatJudge` singleton:
```csharp
SetMoments momo = CombatJudge.Instance.GetSetMoments();
```

Posteriormente, bifurca su lógica según la propiedad `picker`:

#### Lógica para `picker` (`picker = true`)
Si `picker` es verdadero y el jugador está en combate (`player.IsFigthing()`) y el momento actual no es `SetMoments.SelectCombat`, el `HandCard` ajusta su visibilidad:
*   En `SetMoments.PickCard`, la carta se muestra con una visibilidad parcial (`halfVisible(true)`). Se ajusta la transparencia de una imagen hija (probablemente el arte de la carta) para reflejar si hay una `Card` asignada o no.
*   En `SetMoments.Reveal`, la carta se vuelve completamente visible (`Visib(true)`) y deja la visibilidad parcial (`halfVisible(false)`).
Esto sugiere que en su rol de `picker`, la carta tiene diferentes estados visuales durante el proceso de selección o revelación.

#### Lógica para cartas jugables (`picker = false`)
Para las cartas en la mano del jugador, la lógica se activa cuando el `SetMoments` cambia:
*   Si `momo` (el momento actual) es `SetMoments.PickCard` y el jugador está combatiendo (`player.IsFigthing()`):
    *   Verifica el tipo de combate actual (`CombatJudge.Instance.CombatType`) contra el elemento de la carta (`card.GetElement()`). Esto permite que solo las cartas con elementos compatibles (o cualquier carta si el tipo de combate es `Full`) sean jugables.
    *   Si las condiciones se cumplen, la carta se vuelve `clickable(true)` y el `Fighter` del jugador registra una carta disponible (`player.availableCard++`).
*   Si `momo` no es `SetMoments.PickCard`, la carta se vuelve `clickable(false)`, asegurando que solo se puedan jugar cartas durante la fase `PickCard`.
*   Finalmente, si el jugador ya ha seleccionado una carta (`player.GetPicked() != null`), todas las cartas jugables se deshabilitan (`clickable(false)`), evitando que se seleccionen múltiples cartas a la vez.

### `OnPointerEnter(PointerEventData eventData)`
Este método, parte de la interfaz `IPointerEnterHandler`, se invoca cuando el puntero del mouse entra en el área del `HandCard`. Actualmente, su implementación está vacía, pero podría usarse en el futuro para agregar efectos visuales o *tooltips* al pasar el mouse por encima de una carta.

### `OnPointerExit(PointerEventData eventData)`
Este método, parte de la interfaz `IPointerExitHandler`, se invoca cuando el puntero del mouse sale del área del `HandCard`. Al igual que `OnPointerEnter`, su implementación está vacía, pero podría usarse para revertir los efectos de *hover* o cerrar *tooltips*.

## Otros métodos

### `Visib(bool isVisible)`
Controla la visibilidad del principal componente visual de la carta. Se refiere al segundo hijo del GameObject (`transform.GetChild(1).gameObject`). El primer hijo (`transform.GetChild(0).gameObject`) está comentado, indicando una posible refactorización o un enfoque en un subconjunto específico de la UI de la carta.

### `ForceReveal()`
Un método público que fuerza a la carta a ser interactuable, llamando a `clickable(true)`. Esto podría ser útil para mecánicas de juego especiales, debugging, o para asegurar que una carta específica esté disponible bajo ciertas condiciones independientemente del `SetMoments` actual.

### `SetCard(Card card)`
Asigna un objeto `Card` a esta instancia de `HandCard`. Este método es crucial para vincular los datos de una carta con su representación visual y lógica de interacción:
*   Actualiza la referencia `this.card` con la nueva carta.
*   Ajusta la visibilidad de la carta:
    *   Si `card` es nulo (no hay carta asignada) o si la carta no es `playable` ni `picker`, la oculta (`Visib(false)`).
    *   De lo contrario, si es un `picker`, le da una visibilidad parcial (`halfVisible(true)`); si no es un `picker`, le da visibilidad completa (`Visib(true)`).

### `SelectedCard()`
Este método se invoca cuando el `Button` de la carta es presionado (lo que implica que la carta es interactuable). Representa la acción de jugar o seleccionar la carta:
*   Informa al componente `player` (el `Fighter` propietario) que se ha jugado la `card` actual, a través de `player.PlayCard(card)`.
*   Posteriormente, "vacía" el `slot` de la mano asignándole un valor nulo a la carta (`SetCard(null)`), lo que oculta la carta de ese espacio.

### `clickable(bool isClick)`
Gestiona la interactividad del `Button` de la carta y proporciona feedback visual:
*   Establece la propiedad `button.interactable` para habilitar o deshabilitar la interacción por clic.
*   Modifica el color de tres componentes `Image` hijos específicos (`transform.GetChild(1).GetChild(0)`, `.GetChild(1)`, `.GetChild(2)`) a blanco si la carta es interactuable, o a gris si no lo es. Esto proporciona una indicación visual clara al jugador sobre la disponibilidad de la carta. Estos hijos probablemente representan elementos gráficos del *frame* o del fondo de la carta.

### `halfVisible(bool visible)`
Controla la visibilidad de un GameObject hijo específico (`transform.GetChild(0).gameObject`). Este método parece diseñado para situaciones donde la carta necesita mostrarse, pero no con su apariencia completa o "activa", como durante una fase de selección donde las cartas pueden estar "bocabajo" o semi-transparentes. Otro `transform.GetChild(2)` está comentado, similar al método `Visib`.

## Getters y Setters

1.  `GetCard()`: Retorna el objeto `Card` que este `HandCard` está actualmente representando.
2.  `isClickable()`: Retorna un booleano indicando si el `Button` de la carta está interactuable (`true`) o no (`false`).
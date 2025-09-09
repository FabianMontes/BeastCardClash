# HolderPlay
Este script `HolderPlay` es un componente de Unity (`MonoBehaviour`) diseñado para gestionar una única instancia de una carta (`Card`) que ha sido "seleccionada" o "sostenida" en un momento dado. Su función principal es actuar como un registrador o un punto de control para la carta que actualmente está en el "estado de elección", ya sea porque el jugador la ha seleccionado para una acción, la está arrastrando, o se encuentra en una ranura temporal antes de ser jugada.

El script mantiene una referencia a la carta seleccionada a través de la variable `cardPicked`. Permite obtener la carta actual, "desseleccionar" la carta previamente elegida (lo que implica su reactivación visual), y seleccionar una nueva carta, asegurándose de que solo una carta esté "elegida" a la vez.

Dado el contexto de un juego de cartas por turnos como "Beast Card Clash", `HolderPlay` probablemente interactúa con sistemas de entrada del jugador, gestores de UI o el controlador principal del juego para manejar el flujo de selección y juego de cartas. Por ejemplo, cuando el jugador hace clic en una carta en su mano, otro script podría llamar a `PlayCard` en `HolderPlay` para registrar esa selección. Cuando la carta se juega o la selección se cancela, se podría llamar a `LosePick`. La reactivación del `GameObject` de la carta al llamar a `LosePick` sugiere que la carta se vuelve visualmente disponible de nuevo, quizás regresando a la mano del jugador o a una pila de cartas.

# Métodos

## Métodos de Unity

No hay métodos específicos de Unity (como `Awake`, `Start`, `Update`) definidos directamente en este script. Esto indica que su funcionalidad es principalmente reactiva, siendo invocada por otros scripts del proyecto según sea necesario.

## Otros métodos

### `public void LosePick()`
Este método se encarga de "desseleccionar" o "perder" la carta que actualmente está siendo sostenida. Si existe una carta en `cardPicked`, primero se asegura de que su `GameObject` se vuelva activo (`SetActive(true)`), lo que implica que la carta estaba previamente inactiva (quizás oculta o desactivada visualmente) mientras estaba "elegida". Después de reactivarla, la referencia `cardPicked` se establece en `null`, indicando que ya no hay ninguna carta seleccionada.

```csharp
public void LosePick()
{
    if (cardPicked != null) cardPicked.gameObject.SetActive(true);
    cardPicked = null;
}
```

### `public void PlayCard(Card card)`
Este método es el encargado de "seleccionar" una nueva carta. Recibe como parámetro un objeto `Card` que será la nueva carta a sostener. Antes de asignar la nueva carta, el método verifica si ya hay una carta en `cardPicked`. Si es así, invoca `LosePick()` para deselegir la carta anterior, asegurándose de que solo una carta esté en el estado "elegida" en cualquier momento. Finalmente, asigna la nueva carta al campo `cardPicked`. Es importante notar que este método no desactiva el `GameObject` de la carta que se acaba de asignar; esa lógica podría residir en el script que invoca `PlayCard` o en el propio componente `Card`.

```csharp
public void PlayCard(Card card)
{
    if (cardPicked != null) LosePick();
    cardPicked = card;
}
```

## Getters y Setters

1.  `public Card GetPicked()`: Retorna la instancia de `Card` que está siendo actualmente sostenida por este `HolderPlay`. Si no hay ninguna carta seleccionada, devolverá `null`.
# `DeckCardsList`
Este `enum` define el listado completo y canónico de todas las cartas disponibles en el juego "Beast Card Clash". Su propósito principal es estandarizar la identificación y el valor base de cada carta, facilitando su referencia a lo largo de toda la lógica del juego.

La estructura del `enum` se organiza en cuatro elementos principales: Fuego (`Fire`), Tierra (`Earth`), Agua (`Water`) y Aire (`Air`). Esta clasificación elemental es crucial para las mecánicas de "estrategia elemental" y "habilidades únicas" mencionadas en el `README.md` del proyecto.

Cada elemento contiene diez entradas de cartas, numeradas del 1 al 10 (e.g., `Fire1`, `Fire2`, ..., `Fire10`). La asignación explícita de un valor numérico a cada miembro del `enum` (e.g., `Fire1 = 1`, `Fire10 = 10`) indica que este número representa el "puntaje" o nivel de poder base de la carta. Esto sugiere que las cartas no solo tienen un identificador único, sino también un valor intrínseco que podría ser utilizado en cálculos de combate, desempates o habilidades.

Este `enum` actúa como una "tabla maestra" inmutable para la colección de cartas del juego, proveyendo un punto de referencia central para:
*   La generación de mazos.
*   La identificación de cartas en la mano de los jugadores o en el campo de batalla.
*   La implementación de la lógica de efectos de cartas que dependen de su elemento o puntaje base.

A continuación, se muestra un extracto de cómo se definen las cartas:

```csharp
public enum DeckCardsList
{
    // Fuego
    Fire1 = 1,
    Fire2 = 2,
    Fire3 = 3,
    Fire4 = 4,
    Fire5 = 5,
    Fire6 = 6,
    Fire7 = 7,
    Fire8 = 8,
    Fire9 = 9,
    Fire10 = 10,

    // ... otros elementos
}
```

La elección de un `enum` para este propósito es sencilla y directa, lo que se alinea con el enfoque del proyecto en "el desarrollo de una buena experiencia de jugador y sobre todo, de desarrollo para los programadores". Permite a los desarrolladores acceder y manipular la información de las cartas de manera tipada y explícita sin la necesidad de componentes de datos más complejos en las etapas iniciales del desarrollo.

Este `enum` no contiene métodos ni propiedades, ya que su función es puramente enumerativa y de definición de datos estáticos.
# Figther
El script `Figther` es el componente central que define y gestiona las características y el comportamiento de un personaje jugable en **Beast Card Clash**. Cada `Figther` representa una criatura que combina una especie de la biodiversidad colombiana con una facultad de la UNAL, encapsulando su salud, su identidad (nombre, equipo, especie), su apariencia visual, y su interacción con el sistema de cartas y el tablero de juego.

Este script actúa como el "cerebro" del personaje, controlando su estado vital, su representación gráfica en el juego (alternando entre diferentes modelos visuales), y orquestando acciones clave como el manejo de su mazo de cartas, el rellenado de su mano, el movimiento de su token en el tablero y la interacción con las fases de combate. También se encarga de gestionar la lógica de "skin" y "team" para permitir personalización y aleatorización de las propiedades del personaje.

Interactúa fuertemente con otros sistemas del juego, como `CombatJudge` para determinar el estado de combate y `PlayerToken` para el movimiento en el tablero, así como con componentes para la gestión de cartas como `Card`, `HandCard` y `HolderPlay`, aunque la lógica específica de estas interacciones se delega a dichos componentes.

A pesar de que el código contiene algunas convenciones de nombres que no siguen estrictamente PascalCase, el enfoque del proyecto prioriza la funcionalidad y la experiencia de desarrollo, asegurando que las interacciones del personaje sean claras y directas.

# Métodos

## Métodos de Unity

### Start
El método `Start` se ejecuta una vez cuando el `Figther` se inicializa en la escena. Es crucial para establecer el estado inicial del personaje en el juego:

1.  **Inicialización visual:** Desactiva todos los modelos visuales secundarios del `Figther` (presumiblemente representados por los hijos 1 al 4 del `GameObject` principal) y activa únicamente el modelo correspondiente a `visualFigther`. Esto permite cambiar dinámicamente la apariencia del personaje.
    ```csharp
    transform.GetChild(4).gameObject.SetActive(false);
    transform.GetChild(1).gameObject.SetActive(false);
    // ...
    transform.GetChild(visualFigther).gameObject.SetActive(true);
    ```
2.  **Creación del token de jugador:** Si el `playerToken` no ha sido asignado previamente en el Inspector, instancia un `tokenPrefab` en la posición inicial definida por `initialStone`. Luego, asigna este `Figther` como el `player` del `PlayerToken` y establece su `rocky` (la roca sobre la que se encuentra) a `initialStone`.
    ```csharp
    if (playerToken == null) playerToken = Instantiate(tokenPrefab, initialStone.transform.position + Vector3.up * 1, Quaternion.identity).transform.GetComponent<PlayerToken>();
    playerToken.player = this;
    playerToken.rocky = initialStone;
    ```
3.  **Creación del mazo provisional:** Instancia `deckSize` copias del `cardPrefab` como hijos de un objeto específico en la jerarquía del `Figther` (`transform.GetChild(0).GetChild(0)`). Cada carta creada recibe un `indexer` para su identificación. Este proceso simula la creación inicial del mazo de cartas del personaje.
    ```csharp
    for (int i = 0; i < deckSize; i++)
    {
        Card card = Instantiate(cardPrefab, transform.GetChild(0).GetChild(0)).GetComponent<Card>();
        card.indexer = i;
    }
    ```

### Update
El método `Update` se ejecuta en cada fotograma del juego, gestionando la lógica continua del `Figther`:

1.  **Actualización visual del luchador:** Comprueba si el valor de `visualFigther` ha cambiado desde el último fotograma (`lastVisualPlayer`). Si ha cambiado, desactiva el modelo visual anterior y activa el nuevo, actualizando `lastVisualPlayer` para el siguiente ciclo. Esto permite animar o cambiar la apariencia del personaje en tiempo real.
    ```csharp
    if (lastVisualPlayer != visualFigther)
    {
        transform.GetChild(lastVisualPlayer).gameObject.SetActive(false);
        transform.GetChild(visualFigther).gameObject.SetActive(true);
        lastVisualPlayer = visualFigther;
    }
    ```
2.  **Manejo de la fase de "PickCard":** Verifica si la fase actual del combate, según el `CombatJudge`, es `SetMoments.PickCard` y si este `Figther` está activamente luchando (`IsFigthing()`). Si es así y no hay cartas disponibles para recoger (`avalaibleCard == 0`), fuerza la revelación de todas las cartas en la mano del jugador. Esto es clave para la interacción del jugador durante la fase de selección de cartas.
    ```csharp
    if (CombatJudge.CombatJudgeInstance.GetSetMoments() == SetMoments.PickCard && IsFigthing())
    {
        if (avalaibleCard == 0)
        {
            Transform hand = transform.GetChild(0).GetChild(1);
            for (int i = 0; i < handSize; i++)
            {
                hand.GetChild(i).GetComponent<HandCard>().ForceReveal();
            }
        }
    }
    ```

## Otros métodos

### Specie GetSpecie()
Devuelve la `Specie` actual del `Figther`, que representa el animal asignado al personaje.

### void setTeam(Team team)
Establece el `Team` del `Figther` al valor proporcionado. Este método se utiliza para asignar al personaje a una facultad específica de la UNAL.

### void setSkin(int skin)
Establece el índice de `skin` del `Figther`. Este índice determina la variación visual específica del personaje dentro de su especie.

### void setNoSkin(int skin)
Asigna una `skin` aleatoria al `Figther`, asegurándose de que el índice de `skin` generado sea diferente al `skin` que se pasa como parámetro. Esto es útil para evitar skins repetidas o para forzar una variación.
El rango de skins aleatorias es entre 0 y 5.

### void setRSkin()
Asigna una `skin` completamente aleatoria al `Figther` dentro del rango de 0 a 5.

### void setNoTeam(Team team)
Asigna un `Team` aleatorio al `Figther`, garantizando que el equipo seleccionado sea diferente al `team` pasado como parámetro. Esto se utiliza para la asignación dinámica de equipos, posiblemente para balancear o diversificar los enfrentamientos.
El rango de equipos aleatorios es entre 0 y 8 (excluyendo el equipo actual).

### void FreeTeam()
Asigna un `Team` aleatorio al `Figther` sin restricciones, seleccionando cualquier equipo disponible entre 0 y 8.

### Team GetTeam()
Devuelve el `Team` actual del `Figther`.

### int GetPlayerLive()
Devuelve la cantidad actual de vida (`figtherLive`) del `Figther`.

### void setPlayerLive(int pL)
Establece la cantidad de vida (`figtherLive`) del `Figther` al valor `pL`.

### void addPlayerLive(int pL)
Ajusta la vida del `Figther` sumando `pL`. Este método también contiene lógica para:
*   Actualizar la propiedad `noHurt` a `true` si `pL` es positivo (curación) y `false` si es negativo (daño).
*   Clampear la vida para que no exceda la `initialLives` del `CombatJudge` ni sea menor que 0.
    ```csharp
    figtherLive += pL;
    if (figtherLive > CombatJudge.CombatJudgeInstance.initialLives)
    {
        figtherLive = CombatJudge.CombatJudgeInstance.initialLives;
    }
    if (figtherLive < 0)
    {
        figtherLive = 0;
    }
    ```

### void randomSpecie()
Asigna la `Specie` `bear` al `Figther` y luego establece una `skin` aleatoria (entre 0 y 1). Este método parece ser específico para una asignación particular o depuración, posiblemente para un prototipo inicial de la especie "oso".

### void movePlayer(RockBehavior rocker)
Actualiza la roca (`rocky`) sobre la que se encuentra el `playerToken` del `Figther` al `RockBehavior` proporcionado. Esto simula el movimiento del personaje en el tablero de juego.

### Card getPicked()
Obtiene y devuelve la `Card` que ha sido "seleccionada" (o preparada para jugar) por el componente `HolderPlay` asociado al `Figther`.

### void DrawCard(int index, int HandDex)
Dibuja una carta específica del mazo del `Figther` y la coloca en una posición específica de la mano.
*   `index`: El índice de la carta en el mazo (`transform.GetChild(0).GetChild(0)`).
*   `HandDex`: El índice de la posición en la mano (`transform.GetChild(0).GetChild(1)`) donde se colocará la carta.
La carta dibujada se desactiva en el mazo para simular que ha sido tomada.
```csharp
Transform deck = transform.GetChild(0).GetChild(0);
if (index >= deck.childCount || index < 0) return; // Validación básica
Card card = deck.GetChild(index).GetComponent<Card>();
deck.GetChild(index).gameObject.SetActive(false); // La carta ya no está en el mazo
Transform hand = transform.GetChild(0).GetChild(1);
hand.GetChild(HandDex).GetComponent<HandCard>().SetCard(card); // Se establece en la mano
```

### void PlayCard(Card card)
Delega la acción de jugar una `Card` al componente `HolderPlay` que se encuentre como hijo del `Figther`.

### private void DrawCard(int index)
Este es un método privado que dibuja una carta *aleatoria* del mazo activo (no desactivado) y la coloca en la posición `index` de la mano del `Figther`.
Recorre el mazo hasta encontrar una carta activa y la desactiva para moverla a la mano. Este método es utilizado internamente, por ejemplo, por `RefillHand()`.

### void RefillHand()
Rellena la mano del `Figther`. Itera por cada posición en la mano; si una posición está vacía (es decir, `GetCard()` devuelve `null`), llama a la sobrecarga privada `DrawCard(i)` para dibujar una carta aleatoria en esa posición. Al finalizar, `avalaibleCard` se resetea a 0. Esto asegura que el jugador siempre tenga la mano llena hasta `handSize`.

### bool IsFigthing()
Determina si este `Figther` está actualmente participando en un combate. Utiliza un sistema de banderas binarias (`CombatJudge.CombatJudgeInstance.GetPlayersFighting()`) donde cada bit representa el estado de combate de un jugador, identificado por `indexFigther`. Si el bit correspondiente a `indexFigther` es 1, el `Figther` está luchando.
```csharp
// Ejemplo de la lógica subyacente:
// Si GetPlayersFighting() devuelve 5 (binario 101) y indexFigther es 0 o 2,
// significa que el jugador está luchando. Si indexFigther es 1, no está luchando.
```

### void ThrowCard()
Delega la acción de "descartar" o "lanzar" una carta al componente `HolderPlay`, pasando `null` como argumento. Esto probablemente indica que se está descartando la carta actualmente seleccionada o una carta genérica.

## Getters y Setters

1.  `figtherLive`: Obtiene o establece la vida actual del luchador.
2.  `figtherName`: Obtiene o establece el nombre del luchador.
3.  `team`: Obtiene o establece el equipo (facultad) al que pertenece el luchador.
4.  `specie`: Obtiene la especie (animal) del luchador.
5.  `skin`: Obtiene el índice de la variación visual (skin) del luchador. Se establece internamente.
6.  `deckSize`: Obtiene el tamaño total del mazo del luchador.
7.  `handSize`: Obtiene el tamaño máximo de la mano del luchador.
8.  `avalaibleCard`: Obtiene o establece el número de cartas disponibles para una acción específica (ej. recoger).
9.  `tokenPrefab`: Obtiene o establece el prefab del token que representa al luchador en el tablero.
10. `cardPrefab`: Obtiene o establece el prefab de las cartas que se instanciarán para el mazo.
11. `playerToken`: Obtiene o establece la referencia al token del jugador en el tablero.
12. `visualFigther`: Obtiene o establece el índice del modelo visual activo del luchador.
13. `indexFigther`: Obtiene o establece el índice único del luchador para el sistema de combate.
14. `initialStone`: Obtiene o establece la roca inicial donde se posiciona el token del jugador.
15. `noHurt`: Obtiene un booleano que indica si la última modificación de vida fue una curación (`true`) o daño (`false`). Se establece internamente al llamar a `addPlayerLive`.
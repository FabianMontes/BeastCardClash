# CombatJudge
El script `CombatJudge` es el pilar central del sistema de gestión de partidas en Beast Card Clash. Implementado como un Singleton y ejecutándose con alta prioridad (`DefaultExecutionOrder(-1)`), se encarga de orquestar el flujo completo del juego, desde la inicialización de los jugadores y el entorno de batalla hasta la resolución de turnos, combates de cartas y la determinación de las condiciones de victoria o derrota. Su diseño se enfoca en proporcionar una estructura clara para el avance de la partida a través de una máquina de estados (`SetMoments`), permitiendo una gestión robusta de las interacciones entre jugadores (humanos y bots), las cartas, las rocas de la arena y los resultados de los enfrentamientos.

En esencia, `CombatJudge` inicializa el número de combatientes (generando bots y configurando el jugador humano), distribuye a estos en el área de juego, gestiona el avance de las rondas y turnos, coordina el lanzamiento de dados y el movimiento entre rocas, y lo más importante, procesa la lógica de combate entre cartas, incluyendo la aplicación de daño y curación. También maneja la eliminación de jugadores y el final de la partida.

La máquina de estados de `CombatJudge` es fundamental para el desarrollo de la partida:
```csharp
public enum SetMoments
{
    PickDice,     // Elegir dado
    RollDice,     // Tirar dado
    RevealDice,   // Revelar valor del dado
    GlowRock,     // Resaltar las rocas disponibles
    MoveToRock,   // Moverse a la roca elegida
    SelectCombat, // Seleccionar tipo de combate
    PickCard,     // Elegir carta
    Reveal,       // Revelar carta
    Result,       // Mostrar resultados
    End,          // Finalizar partida
    Loop,         // Reiniciar para la siguiente ronda
    Round,        // Muestra la ronda
    Rounded       // Fin de ronda
}
```
Estos estados dictan el comportamiento del juego en el método `Update()`, asegurando una progresión lógica y controlada de cada fase de la ronda.

# Métodos

## Métodos de Unity

### Start
El método `Start()` es crucial para la inicialización de la partida y se ejecuta una vez al inicio del ciclo de vida del script. Su función principal es preparar el `CombatJudge` y configurar el estado inicial del juego, incluyendo a los jugadores, la arena y las reglas básicas.

1.  **Configuración del Singleton:** Establece la instancia de `CombatJudgeInstance` si es la primera vez que se carga el script. Si ya existe una instancia, se destruye el `GameObject` actual para asegurar que solo haya un `CombatJudge` activo en la escena.
    ```csharp
    if (CombatJudgeInstance == null)
    {
        CombatJudgeInstance = this;
    }
    else
    {
        Destroy(gameObject);
    }
    ```
2.  **Inicialización de Variables de Juego:** Asigna valores iniciales a variables clave como `manyFighters` (cantidad de jugadores, entre 2 y 4 de forma aleatoria), `actualAction` (establecido en `SetMoments.Loop` para iniciar la primera ronda), `fighterTurn` (iniciado en -1 para forzar que el primer turno sea para el jugador humano), y `Round` (iniciado en 0).
3.  **Gestión de Jugadores Existentes:**
    *   Busca todos los componentes `Figther` en la escena.
    *   Ordena estos `Figther` extrayendo un dígito inicial de su nombre (`int.Parse(players[a].name[0].ToString())`) para garantizar un orden específico (e.g., "0_Player", "1_Bot").
    *   Destruye cualquier `Figther` adicional si `manyFighters` es menor que el número de `Figther` encontrados, ajustando la partida al número de jugadores deseado.
4.  **Referencias a la Escena:** Obtiene referencias a componentes clave de la UI y la arena, como `PlayZone` (el círculo de rocas) y `Canvas`.
5.  **Creación e Configuración de `_fighters`:**
    *   Crea el array `_fighters` con el tamaño determinado por `manyFighters`.
    *   Itera para poblar el array:
        *   Si `i == 0`, se asigna el prefab `player` (el humano). De lo contrario, se asigna el prefab `bots`.
        *   Se reutilizan `Figther` existentes si `i` es menor que `players.Length`. De lo contrario, se instancia un nuevo `Figther` (ya sea el jugador o un bot), se le asigna una especie aleatoria con `randomSpecie()` y se le establece como hijo del `Canvas`.
        *   **Asignación de Equipo y Skin:** Para el jugador humano (`i == 0`), se utilizan los datos almacenados en `GameState.singleton` (`team` y `skin`). Para los bots, se asignan equipos de forma estratégica (usando `setNoTeam` para evitar equipos duplicados con el humano o con otros bots, o `FreeTeam` para una asignación aleatoria) y una skin aleatoria (`setRSkin`).
        *   **Configuración Inicial del Jugador:** Cada `Figther` recibe su `initialLives`, un `visualFigther` (para UI), un `indexFigther` (su posición en el array), y un `figtherName` (del `GameState` para el humano, o un identificador genérico para bots).
        *   **Asignación de Roca Inicial:** Se calcula la roca inicial (`initialStone`) para cada jugador basándose en el espaciado en la `PlayZone`, asegurando una distribución equitativa.

### Update
El método `Update()` gestiona el avance de la partida a través de una máquina de estados controlada por la variable `actualAction` del enum `SetMoments`. Se ejecuta cada frame y reacciona al estado actual del juego.

*   **`SetMoments.PickDice`**: No realiza acciones directamente; es un estado de espera a que el jugador elija un dado.
*   **`SetMoments.RollDice`**: Registra el tiempo actual en `_time`. Esto se usa para temporizar eventos posteriores, como la revelación del dado.
*   **`SetMoments.RevealDice`**: Después de un breve retraso (0.5 segundos), llama a `SetGlowing()` para resaltar las rocas disponibles a las que el jugador puede moverse, basándose en el valor del dado.
*   **`SetMoments.GlowRock`**: Si el turno actual es de un bot (`fighterTurn != 0`), el bot recibe la instrucción de elegir una roca llamando a `ThinkingRocks()`.
*   **`SetMoments.MoveToRock`**, **`SetMoments.SelectCombat`**: No realizan acciones directamente; son estados de transición.
*   **`SetMoments.PickCard`**: Comprueba si todos los jugadores en la batalla han elegido sus cartas.
    *   Itera sobre `_fighters`. Si un jugador tiene una carta seleccionada (`getPicked() != null`) o no está en la batalla (`!fighter.IsFigthing()`), se ignora. Si falta alguna carta, `_allPlayersChose` permanece `false`.
    *   Si `_allPlayersChose` es `true`, el estado cambia a `SetMoments.Reveal`.
*   **`SetMoments.SetMoments.Reveal`**: Procesa los combates de cartas una vez que todos los jugadores han elegido sus cartas.
    *   Recopila las cartas elegidas por todos los jugadores.
    *   Calcula los resultados de combate individuales (`Results[][]`) para cada par de jugadores usando el método `IndividualCombat()`.
    *   Determina el daño total (`destiny[]`) para cada jugador y aplica los cambios de vida usando `addPlayerLive()`.
    *   Registra el tiempo actual y cambia el estado a `SetMoments.Result`.
*   **`SetMoments.Result`**: Muestra los resultados de la ronda y maneja las consecuencias.
    *   Después de 5 segundos, verifica si el jugador humano (`_fighters[0]`) ha perdido. Si es así, finaliza el juego (`EndGamer(false)`) y cambia el estado a `SetMoments.End`.
    *   Si el humano no ha perdido, itera sobre los bots y elimina a aquellos cuya vida (`GetPlayerLive()`) ha llegado a cero. Actualiza el array `_fighters` y `manyFighters`.
    *   Reindexa los `indexFigther` de los jugadores restantes.
    *   Comprueba si solo queda un jugador. Si es así, el juego termina con una victoria (`EndGamer(true)`) y el estado pasa a `SetMoments.End`.
    *   De lo contrario, el estado pasa a `SetMoments.Loop` para iniciar una nueva ronda.
*   **`SetMoments.Loop`**: Prepara el juego para la siguiente ronda.
    *   Avanza `fighterTurn` al siguiente jugador en el ciclo.
    *   Llama a `RefillHand()` y `ThrowCard()` para todos los jugadores para preparar sus manos para la nueva ronda.
    *   Reinicia la máscara de bits `_playersFighting`.
    *   Transiciona al estado `SetMoments.Round` si es el turno del jugador humano (index 0) o `SetMoments.PickDice` si es el turno de un bot.
*   **`SetMoments.Round`**: Inicia visualmente una nueva ronda.
    *   Incrementa el contador `Round`.
    *   Invoca `startRound()` en el componente `Roundanimation` (se nota un TODO en el código sobre la refactorización de `FindFirstObjectByType` y el nombre de la clase).
    *   Cambia el estado a `SetMoments.Rounded`.
*   **`SetMoments.End`**: No realiza acciones directamente; es el estado final de la partida.

## Otros métodos

### Results IndividualCombat(Card one, Card two)
Este método calcula el resultado de un combate uno a uno entre dos cartas (`one` y `two`). La lógica tiene en cuenta el `CombatType` actual, los elementos de las cartas y sus valores.

*   **Casos Base:** Si alguna de las cartas es `null`, el combate resulta en un `Results.Draw`.
*   **Combate Elemental Específico:** Si el `CombatType` no es `Full` (es decir, es un elemento fijo) y los elementos de las cartas `one` y `two` son diferentes, la victoria o derrota se determina directamente por si el elemento de la carta `one` coincide con el `CombatType` establecido.
*   **Lógica de Elementos y Valores:**
    *   Calcula `elementDiff`, la diferencia cíclica entre los elementos de las cartas, teniendo en cuenta la cantidad total de elementos (`countElements`) y la mitad de ellos (`halfElements`).
    *   **Para un número par de elementos:** Si la `elementDiff` no es 0 (mismo elemento) ni `halfElements` (elementos opuestos), se determina el ganador por si `elementDiff` es mayor que `halfElements`. Si los elementos son los mismos o directamente opuestos, se compara el `GetValue()` de las cartas: la de mayor valor gana, la de menor valor pierde, y si son iguales, es un empate.
    *   **Para un número impar de elementos:** Si los elementos son diferentes (`elementDiff != 0`), se determina el ganador comparando `elementDiff` con `halfElements`. Si los elementos son iguales, se comparan los `GetValue()`: la de mayor valor gana, la de menor valor pierde, y si son iguales, es un empate.

### void ArriveAtRock()
Este método se invoca cuando el jugador en turno ha completado su movimiento a una roca. Su propósito es configurar el siguiente paso en la secuencia de juego: determinar si hay un combate multijugador y establecer el tipo de combate o permitir su selección.

*   **Identificación de Jugadores en Roca:** Obtiene la `RockBehavior` actual del jugador en turno.
*   **Alcance del Combate:**
    *   Si `rocky.manyOn()` devuelve `true`, significa que hay varios jugadores en la misma roca. En este caso, `_playersFighting` se actualiza con una máscara de bits (`rocky.GetPlayersOn()`) que representa solo a los jugadores en esa roca específica, indicando un combate localizado.
    *   Si no hay más de un jugador, `_playersFighting` se establece para incluir a todos los jugadores activos `((int)Mathf.Pow(2, manyFighters) - 1)`, lo que implica un combate general contra todos.
*   **Tipo de Combate en la Roca:**
    *   **Si la roca permite elegir elemento (`rocky.inscription == Inscription.pick`):**
        *   Si es el turno del jugador humano (`Turn() == 0`), el estado del juego cambia a `SetMoments.SelectCombat`, permitiendo al jugador elegir un elemento.
        *   Si es el turno de un bot, se selecciona un `CombatType` aleatorio y el estado cambia directamente a `SetMoments.PickCard`.
    *   **Si la roca tiene un elemento predefinido (`rocky.inscription` es un elemento específico):** El `CombatType` se establece directamente al elemento de la roca y el estado del juego cambia a `SetMoments.PickCard`.

### void MoveToRock(RockBehavior rocker)
Este método simplemente actualiza la roca actual del `playerToken` del jugador en turno a la `rocker` proporcionada y luego cambia el `actualAction` a `SetMoments.MoveToRock`. Esto indica que el jugador está en proceso de movimiento.

### bool PickElement(Element element)
Este método permite al jugador humano seleccionar el tipo de elemento de combate en rocas que lo permiten (`Inscription.pick`).

*   Verifica que el estado actual del juego sea `SetMoments.SelectCombat`. Si no lo es, devuelve `false`.
*   Intenta convertir el `element` elegido al `CombatType` y lo asigna a `CombatType`.
*   Si la asignación es exitosa, el estado del juego cambia a `SetMoments.PickCard` y el método devuelve `true`.
*   En caso de error durante la conversión (aunque poco probable con los enums), imprime la excepción y devuelve `false`.

### void EndRounded()
Este método se invoca para finalizar el estado de "ronda terminada". Si el estado actual es `SetMoments.Rounded`, lo cambia a `SetMoments.PickDice`, preparando el juego para que los jugadores elijan su dado para la siguiente acción.

### void StartRolling()
Este método inicia la fase de lanzamiento de dado. Simplemente cambia el estado `actualAction` a `SetMoments.RollDice`.

### void Rolled()
Este método se invoca después de que el dado ha sido lanzado. Si el estado actual es `SetMoments.RollDice`, lo cambia a `SetMoments.RevealDice`, indicando que el valor del dado está listo para ser mostrado.

### void SetGlowing(int value)
Este método es responsable de resaltar las rocas a las que el jugador actual puede moverse después de lanzar el dado.

*   Obtiene la roca actual (`lander`) del jugador en turno.
*   Usa `lander.getNeighbor(value)` para obtener las dos rocas vecinas a las que el jugador puede moverse según el valor `value` del dado.
*   Establece la propiedad `shiny` a `true` para ambas rocas, haciendo que se destaquen visualmente.
*   Cambia el estado del juego a `SetMoments.GlowRock`.
*   Si el turno no es del jugador humano (`fighterTurn != 0`), instruye al `BotPlayer` del bot actual a `PickRock(rocker)`, es decir, a elegir una de las rocas resaltadas.

### void Surrender()
Este método permite al jugador humano rendirse en la partida. Establece el estado del juego a `SetMoments.End` y llama a `EndGamer(false)` en el componente `EndGame` para finalizar la partida, registrándola como una derrota.

## Getters y Setters

1.  **Round:** `int` (read-only). Proporciona el número de la ronda actual.
2.  **CombatType:** `CombatType` (read-only). Indica el tipo de elemento (Fire, Earth, Water, Air, Full) que está activo para el combate en la roca actual.
3.  **CombatJudgeInstance:** `CombatJudge` (estático, read-only). Es la instancia Singleton de la clase `CombatJudge`, permitiendo acceso global a sus funcionalidades.
4.  **GetSetMoments:** `SetMoments`. Retorna el estado actual de la máquina de estados del juego (`actualAction`).
5.  **GetPlayersFighting:** `int`. Retorna una máscara de bits que representa a los jugadores que están participando en el combate actual.
6.  **FocusOnTurn:** `bool`. Indica si el turno actual pertenece al jugador humano (`_fighters[0]`).
7.  **Turn:** `int`. Retorna el índice del jugador cuyo turno es actualmente (`fighterTurn`).
8.  **HurtPlayer:** `bool`. Retorna el valor de la propiedad `noHurt` del jugador humano (`_fighters[0]`), indicando si el jugador no ha recibido daño.
9.  **MoveToRock:** Establece la roca de destino para el jugador en turno, iniciando el movimiento.
10. **PickElement:** Intenta establecer el `CombatType` según el elemento seleccionado por el jugador.
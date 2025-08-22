# `Combatjudge.cs`

## 1. Propósito General
Este script es el **controlador principal del juego** en Beast Card Clash. Gestiona el flujo de las rondas, la lógica de combate entre jugadores, la gestión de los participantes (`Figther`s) y la progresión a través de los diferentes estados del juego, desde el lanzamiento de dados hasta la resolución de combates y la determinación del ganador. Interactúa con múltiples sistemas clave como la gestión de jugadores (`Figther`), la interfaz de usuario, los dados (`dice`), las zonas de juego (`PlayZone`), y los componentes de fin de juego (`EndGame`).

## 2. Componentes Clave

### `Element` (Enum)
- **Descripción:** Define los tipos elementales de las cartas y rocas en el juego: `fire`, `earth`, `water`, `air`. Estos elementos son fundamentales para la lógica de combate elemental.

### `SetMoments` (Enum)
- **Descripción:** Enumera los diferentes estados o "momentos" por los que transita el juego durante una ronda. Actúa como los estados de una máquina de estados finitos que controla el flujo principal del juego.
- **Valores Clave:**
    - `PickDice`, `RollDice`, `RevealDice`: Relacionados con la fase de lanzamiento de dados.
    - `GlowRock`, `MoveToRock`, `SelecCombat`: Relacionados con el movimiento a rocas y la selección de tipo de combate.
    - `PickCard`, `Reveal`, `Result`: Relacionados con la selección, revelación y resolución de cartas.
    - `End`, `Loop`, `round`, `rounded`: Estados de fin de juego, inicio de bucle de turno, inicio y fin de ronda.

### `Results` (Enum)
- **Descripción:** Define los posibles resultados de un combate individual entre dos cartas: `lose`, `draw`, `win`.

### `CombatType` (Enum)
- **Descripción:** Especifica el tipo de combate que se está llevando a cabo, que puede ser uno de los elementos (`fire`, `earth`, `water`, `air`) o `full` (combate sin restricción elemental).

### `Combatjudge` (Clase)
- **Descripción:** Es un `MonoBehaviour` que actúa como el cerebro central del juego. Implementa un patrón Singleton para asegurar que solo exista una instancia. Orquesta el ciclo de juego, inicializa a los jugadores, procesa las acciones de los turnos, resuelve los combates y gestiona las condiciones de victoria/derrota.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] int manyFigthers`: Define la cantidad de jugadores/bots activos en la partida. Se inicializa aleatoriamente en `Start`.
    - `public int round { get; private set; }`: El número de ronda actual del juego. Es de solo lectura desde fuera.
    - `[SerializeField] GameObject player`: Prefab o referencia al objeto del jugador principal.
    - `[SerializeField] GameObject Bots`: Prefab o referencia al objeto de los jugadores controlados por IA.
    - `Figther[] figthers`: Un array que almacena todas las instancias de los combatientes (`Figther`) en el juego.
    - `[SerializeField] int figtherTurn`: El índice del combatiente (`Figther`) cuyo turno es actualmente.
    - `[SerializeField] SetMoments actualAction`: El estado actual de la máquina de estados del juego (ver `SetMoments` enum).
    - `[SerializeField] public int maxDice`: El valor máximo que puede obtenerse al lanzar los dados.
    - `[SerializeField] public int initialLives`: Las vidas iniciales con las que comienzan todos los combatientes.
    - `public CombatType combatType { get; private set; }`: El tipo de combate establecido para la fase actual, influenciando la resolución de cartas.
    - `public static Combatjudge combatjudge`: La instancia Singleton de esta clase.

- **Métodos Principales:**
    - `void Start()`:
        - **Descripción:** Se ejecuta una vez al inicio del juego. Establece la instancia Singleton, inicializa variables de juego (número de combatientes, turno, ronda, etc.). Localiza y configura los combatientes existentes o instancia nuevos a partir de los prefabs, asignándoles propiedades como equipo, aspecto, vidas iniciales, nombre y roca inicial.
        - **Lógica Clave:** Realiza una reorganización inicial de los objetos `Figther` encontrados en la escena y luego instancia/configura los combatientes restantes para alcanzar la cantidad deseada (`manyFigthers`), asignando roles de jugador y bots, equipos aleatorios para bots y sus ubicaciones iniciales en las rocas.

    - `void Update()`:
        - **Descripción:** El método principal del ciclo de juego. Contiene una máquina de estados (`switch` basado en `actualAction`) que controla la progresión del juego a través de sus diferentes fases.
        - **Lógica Clave (Máquina de Estados):**
            - **`PickDice`, `RollDice`, `RevealDice`**: Fases para la acción de los dados, incluyendo la espera y revelación del resultado, y el inicio del brillo de las rocas.
            - **`GlowRock`**: El turno del jugador o bot para seleccionar una roca después del lanzamiento de dados.
            - **`MoveToRock`, `SelecCombat`**: Fases para el movimiento del combatiente y la selección del tipo de combate (si la roca lo permite).
            - **`PickCard`**: Espera a que todos los combatientes en la roca actual hayan seleccionado una carta. Una vez seleccionadas, avanza a `Reveal`.
            - **`Reveal`**: Recupera las cartas seleccionadas por todos los combatientes y calcula los resultados individuales de cada enfrentamiento (`IndvCombat`). Luego, suma los resultados para determinar el daño/curación de cada combatiente y avanza al estado `Result`.
            - **`Result`**: Espera un tiempo (5 segundos) y luego verifica las vidas de los combatientes. Elimina a los combatientes cuyas vidas han llegado a cero. Si solo queda un combatiente, finaliza el juego (victoria/derrota). De lo contrario, reinicia la ronda o el ciclo de turnos (`Loop`).
            - **`Loop`**: Prepara el siguiente turno: avanza `figtherTurn`, reabastece las manos de cartas de todos los combatientes y los prepara para lanzar una carta. Decide si la siguiente acción es iniciar una nueva ronda o la fase de `PickDice`.
            - **`round`**: Incrementa el contador de ronda y activa una animación de inicio de ronda.
            - **`End`**: Marca el final del juego, invocando la lógica de `EndGame`.

    - `Results IndvCombat(Card one, Card two)`:
        - **Descripción:** Resuelve un combate individual entre dos cartas. Determina si `one` gana, pierde o empata contra `two` basándose en el tipo de combate (`combatType`), los elementos de las cartas y, en caso de empate elemental, los valores de las cartas.
        - **Lógica Clave:** Implementa la lógica de "piedra-papel-tijera" elemental, donde la victoria o derrota se define por la relación entre los elementos (e.g., fuego > tierra). Si los elementos son iguales o en un tipo de combate `full`, la carta con mayor valor numérico gana. Considera el número par/impar de elementos para la lógica de ventaja/desventaja.

    - `void ArriveAtRock()`:
        - **Descripción:** Se llama cuando el combatiente activo llega a una roca. Determina cuántos jugadores están en esa roca y el tipo de combate basado en la inscripción de la roca.
        - **Lógica Clave:** Si la roca permite la elección de elemento (`Inscription.pick`), el jugador activo (si es el jugador humano) puede seleccionar el tipo de combate; de lo contrario, el tipo de combate se fija por la inscripción de la roca.

    - `void MoveToRock(RockBehavior rocker)`:
        - **Descripción:** Asigna la roca de destino al `playerToken` del combatiente en turno y establece el estado del juego a `MoveToRock`.

    - `bool pickElement(Element element)`:
        - **Descripción:** Permite al jugador elegir un elemento para el `combatType` si el juego está en el estado `SelecCombat`.
        - **Retorno:** `true` si la selección fue exitosa, `false` si no se pudo seleccionar (ej. no es el estado correcto).

    - `void SetGlowing(int value)`:
        - **Descripción:** Recibe un valor (del dado) y resalta las rocas adyacentes a la roca actual del combatiente que corresponden a ese valor, permitiendo al jugador o bot elegir la siguiente roca. Llama a `BotPlayer.PickRock` si el turno es de un bot.

- **Lógica Clave Global:**
    - **Singleton:** El script utiliza el patrón Singleton (`public static Combatjudge combatjudge;`) para asegurar que solo haya una instancia en la escena y sea fácilmente accesible desde otros scripts.
    - **Máquina de Estados:** La ejecución del juego se controla mediante una máquina de estados implementada con el enum `SetMoments` y el `switch` en el método `Update`. Esto proporciona una estructura clara para la progresión del turno y la resolución de eventos.
    - **Gestión de Combatientes:** `Combatjudge` es responsable de la inicialización, seguimiento de vidas y eliminación de los objetos `Figther` a lo largo del juego.
    - **Lógica de Combate:** Centraliza la lógica de resolución de combates, tanto a nivel individual de cartas (`IndvCombat`) como a nivel de grupo en una roca (`Reveal`).
    - **Orden de Ejecución:** `[DefaultExecutionOrder(-1)]` asegura que este script se ejecute antes que la mayoría de los demás scripts en Unity, lo cual es crítico para un controlador de juego central.

## 3. Dependencias y Eventos

- **Componentes Requeridos:**
    - Este script no tiene un atributo `[RequireComponent]` explícito, pero funcionalmente requiere la existencia de objetos con scripts como `Figther`, `PlayZone`, `Canvas`, `dice`, `EndGame`, `Roundanimation`, y `RockBehavior` en la escena para operar correctamente.

- **Eventos (Entrada):**
    - **Llamadas Directas de Otros Scripts:** Este script expone varios métodos públicos (`ArriveAtRock`, `MoveToRock`, `pickElement`, `StartRoling`, `Roled`, `SetGlowing`, `endRoundeded`, `Surrender`) que son invocados por otros sistemas (e.g., UI, lógica de `dice`, `RockBehavior`, `BotPlayer`) para notificar al juez de combate sobre acciones del jugador o del juego.
    - **Detección de Objetos en Escena:** Utiliza `FindObjectsByType` y `FindFirstObjectByType` para localizar instancias de `Figther`, `PlayZone`, `Canvas`, `dice`, `EndGame`, `Roundanimation`, y `RockBehavior` al inicio del juego o durante la ejecución.

- **Eventos (Salida):**
    - **Actualizaciones de Estado:** Cambia el valor de `actualAction`, notificando implícitamente a otros sistemas que puedan estar monitoreando este estado (aunque no directamente vía un `UnityEvent`).
    - **Manipulación de Combatientes:** Llama a métodos en objetos `Figther` (e.g., `setTeam`, `setSkin`, `setNoTeam`, `FreeTeam`, `setPlayerLive`, `setRSkin`, `randomSpecie`, `RefillHand`, `ThrowCard`, `addPlayerLive`, `getPicked`, `IsFigthing`) para configurar, actualizar y gestionar su estado.
    - **Control de Bots:** Invoca métodos en el componente `BotPlayer` (e.g., `ThinkingRocks`, `PickRock`) para controlar el comportamiento de la IA.
    - **Actualizaciones de UI/Animaciones:** Llama a métodos en `EndGame` (`EndGamer`) y `Roundanimation` (`startRound`) para actualizar el estado del juego y mostrar animaciones relevantes.
    - **Manipulación del Juego:** Destruye GameObjects de `Figther`s eliminados. Modifica las propiedades de `RockBehavior` (e.g., `shiny`).
    - **Singleton (`combatjudge`):** Al ser una instancia estática y pública, `Combatjudge` actúa como un punto de acceso central para otros scripts que necesitan interactuar con la lógica del juego o acceder a su estado.
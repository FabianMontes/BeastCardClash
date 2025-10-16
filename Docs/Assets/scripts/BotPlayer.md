# `BotPlayer`
El script `BotPlayer` es un componente fundamental para la implementación de la inteligencia artificial (IA) en **Beast Card Clash**, permitiendo que un jugador controlado por la computadora participe en las batallas. Su función principal es simular el comportamiento de un jugador humano, tomando decisiones estratégicas como elegir cartas, lanzar dados y seleccionar tipos de ataque o movimientos en el tablero, todo ello con tiempos de "pensamiento" aleatorios para ofrecer una experiencia de juego más orgánica y menos predecible.

Este componente se adjunta a un GameObject que representa al bot dentro del juego. Utiliza varios estados de la batalla (gestionados por `CombatJudge`) para determinar qué acción debe realizar en cada momento, interaccionando con otros componentes como `Fighter`, `HandCard`, `Dice`, `SelectType` y `RockBehavior` para ejecutar sus decisiones. Su diseño permite un desarrollo rápido y una experiencia de jugador inmersiva, simulando un oponente con un comportamiento estratégico básico.

# Métodos

## Métodos de Unity

### `Awake`
El script utiliza el atributo `[DefaultExecutionOrder(1)]` para indicar que el método `Awake` (y consecuentemente `Start` y `Update`) de este script debe ejecutarse *después* de scripts con orden de ejecución por defecto o menor. Aunque el script no define explícitamente un método `Awake`, este atributo es relevante para la fase de inicialización.

### `Start`
El método `Start` se llama una vez al inicio del ciclo de vida del script, después de `Awake`. Su propósito es inicializar las referencias a otros componentes y las variables de estado internas del `BotPlayer`:

```csharp
void Start()
{
    // Inicializa las variables
    figther = GetComponent<Fighter>();
    hand = transform.GetChild(0).GetChild(1);
    picking = false;
}
```

*   `figther`: Obtiene una referencia al componente `Fighter` que debe estar adjunto al mismo GameObject que `BotPlayer`. Este componente representa la entidad del jugador (bot) en el combate, proporcionando acceso a su estado como si está luchando o qué carta ha elegido.
*   `hand`: Obtiene una referencia al `Transform` que representa la "mano" de cartas del bot. La implementación actual asume una jerarquía específica donde la mano es el segundo hijo del primer hijo del GameObject al que `BotPlayer` está adjunto (`transform.GetChild(0).GetChild(1)`). Las cartas individuales estarán como hijos de este `Transform`.
*   `picking`: Inicializa la bandera booleana `picking` a `false`. Esta bandera se utiliza para controlar el flujo de las decisiones del bot, evitando que ciertas lógicas se repitan innecesariamente en cada `Update` frame o para simular un "tiempo de pensamiento".

### `Update`
El método `Update` se llama una vez por frame y contiene la lógica principal para el comportamiento del bot durante el combate. Opera como una máquina de estados simplificada, reaccionando a los diferentes `SetMoments` (fases del combate) gestionados por el `CombatJudge` y simulando tiempos de "pensamiento" aleatorios antes de tomar decisiones.

La lógica dentro de `Update` se divide en varios bloques condicionales (`if`/`else if`), cada uno manejando una fase específica del turno del bot:

1.  **Preparación para elegir carta (`SetMoments.PickCard`)**:
    *   Detecta cuando el bot debe elegir una carta (`figther.IsFigthing()`, `CombatJudge.Instance.GetSetMoments() == SetMoments.PickCard`, `figther.GetPicked() == null`, y `!picking`).
    *   Cuando se cumplen estas condiciones, el bot "empieza a pensar" estableciendo un tiempo aleatorio (`total`) entre 1 y 3 segundos y activando la bandera `picking`.
    ```csharp
    if (figther.IsFigthing() && CombatJudge.Instance.GetSetMoments() == SetMoments.PickCard && figther.GetPicked() == null && !picking)
    {
        time = Time.time;
        total = Random.Range(1.0f, 3.0f);
        picking = true;
    }
    ```

2.  **Selección de carta (`SetMoments.PickCard`)**:
    *   Una vez que el bot está en modo `picking` y el tiempo de "pensamiento" (`Time.time - time > total`) ha transcurrido, selecciona una carta.
    *   Busca aleatoriamente una carta seleccionable entre los hijos del `Transform hand`. La lógica incluye un bucle `while` que intenta encontrar una carta válida (que no sea `null` y sea `isClickable()`) hasta 100 veces, iterando por los índices de la mano para asegurar robustez en caso de que algunas ranuras estén vacías o las cartas no sean seleccionables. Este enfoque práctico es útil en un desarrollo indie para manejar situaciones inesperadas sin una gestión de errores más compleja.
    *   Llama al método `SelectedCard()` de la carta elegida para ejecutar su lógica de selección.
    ```csharp
    else if (picking && CombatJudge.Instance.GetSetMoments() == SetMoments.PickCard)
    {
        if (Time.time - time > total)
        {
            picking = false;
            int a = Random.Range(0, 6);
            int b = 0;
            HandCard card = hand.transform.GetChild(a).GetComponent<HandCard>();

            while ((card == null || !card.isClickable()) && b < 100)
            {
                a = (a + 1) % 6;
                card = hand.transform.GetChild(a).GetComponent<HandCard>();
                b++;
            }
            card.SelectedCard();
        }
    }
    ```

3.  **Lanzamiento del dado (preparación) (`SetMoments.PickDice`)**:
    *   Cuando es el turno del bot para lanzar el dado (`CombatJudge.Instance.GetSetMoments() == SetMoments.PickDice` y `CombatJudge.Instance.Turn() == figther.indexFighter`), establece un tiempo de "pensamiento" aleatorio (entre 0.5 y 3 segundos) antes de que el dado sea lanzado visualmente.
    *   Inicia el proceso de lanzamiento del dado llamando a `FindFirstObjectByType<Dice>().Roll()`.
    ```csharp
    else if (CombatJudge.Instance.GetSetMoments() == SetMoments.PickDice && CombatJudge.Instance.Turn() == figther.indexFighter)
    {
        time = Time.time;
        total = Random.Range(0.5f, 3.0f);
        FindFirstObjectByType<Dice>().Roll();
    }
    ```

4.  **Lanzamiento del dado (finalización) (`SetMoments.PickDice`)**:
    *   Una vez que el tiempo de espera ha transcurrido (`Time.time - time > total`) y sigue siendo el turno del bot, finaliza la animación de lanzamiento del dado llamando a `FindFirstObjectByType<Dice>().Unroll()`.
    ```csharp
    else if (Time.time - time > total && CombatJudge.Instance.Turn() == figther.indexFighter)
    {
        FindFirstObjectByType<Dice>().Unroll();
    }
    ```

5.  **Preparación para elegir tipo de ataque (`SetMoments.SelectCombat`)**:
    *   Cuando el bot debe elegir un tipo de ataque (`CombatJudge.Instance.GetSetMoments() == SetMoments.SelectCombat`, `!picking` y `CombatJudge.Instance.Turn() == figther.indexFighter`), establece un tiempo de "pensamiento" (entre 0.5 y 2 segundos) y activa la bandera `picking`.
    ```csharp
    else if (CombatJudge.Instance.GetSetMoments() == SetMoments.SelectCombat && !picking && CombatJudge.Instance.Turn() == figther.indexFighter)
    {
        time = Time.time;
        total = Random.Range(0.5f, 2.0f);
        picking = true;
    }
    ```

6.  **Selección de tipo de ataque (`SetMoments.SelectCombat`)**:
    *   Después de transcurrido el tiempo de "pensamiento" y siendo el turno del bot, el script realiza una pequeña lógica estratégica: cuenta la cantidad de cartas de cada elemento disponibles en su mano.
    *   Identifica el elemento con mayor número de cartas disponibles.
    *   Llama al método `PickElement()` del componente `SelectType` (obtenido a través de `GetComponentInChildren`) para comunicar la elección del elemento. Esta elección es rudimentaria pero efectiva para un juego indie, basándose en la disponibilidad de recursos.
    ```csharp
    else if (CombatJudge.Instance.GetSetMoments() == SetMoments.SelectCombat && picking && CombatJudge.Instance.Turn() == figther.indexFighter)
    {
        if (Time.time - time > total)
        {
            picking = false;
            int[] elem = new int[4];
            for (int i = 0; i < hand.childCount; i++)
            {
                HandCard card = hand.transform.GetChild(i).GetComponent<HandCard>();
                if (card == null) continue;
                elem[(int)card.GetCard().GetElement()] = elem[(int)card.GetCard().GetElement()] + 1;
            }

            int big = 0;
            for (int i = 1; i < 4; i++)
            {
                if (elem[i] > elem[big]) big = i;
            }
            GetComponentInChildren<SelectType>().PickElement(big);
        }
    }
    ```

## Otros métodos

### `public void PickRock(RockBehavior[] rocks)`
Este método público es llamado externamente (probablemente por `CombatJudge` o un sistema de movimiento) para informar al bot qué rocas del tablero están disponibles como opciones de movimiento. El bot "piensa" durante un tiempo aleatorio antes de decidir.

*   `picking`: Se desactiva (`false`) para asegurar que no se solapen lógicas de "pensamiento" anteriores.
*   `time`: Almacena el momento actual para iniciar el temporizador.
*   `total`: Establece un tiempo aleatorio (entre 1 y 2 segundos) que el bot "tardará en pensar" antes de elegir la roca.
*   `this.rocks`: Almacena el arreglo de `RockBehavior` disponibles para su posterior selección.

### `public void ThinkingRocks()`
Este método es el complemento de `PickRock`. Se encarga de tomar la decisión final sobre a qué roca moverse, una vez que el tiempo de "pensamiento" ha transcurrido.

*   Verifica si el tiempo establecido en `PickRock` ya ha pasado (`Time.time - time > total`).
*   Si el tiempo ha transcurrido, elige una roca al azar del arreglo `rocks` almacenado.
*   Informa al `CombatJudge` sobre la roca seleccionada, invocando `CombatJudge.Instance.MoveToRock()`.

## Getters y Setters

1.  `public void PickRock(RockBehavior[] rocks)`: Establece el arreglo de objetos `RockBehavior` disponibles para el movimiento del bot, junto con un temporizador de "pensamiento" interno.
2.  No existen métodos "getter" explícitos o propiedades públicas definidas dentro del script `BotPlayer` para acceder directamente a su estado interno desde componentes externos. El control del bot se realiza a través de las acciones que ejecuta en respuesta a los eventos del juego.
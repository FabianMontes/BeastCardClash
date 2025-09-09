# BotPlayer
Este script `BotPlayer` es el controlador de inteligencia artificial para los bots en **Beast Card Clash**, permitiéndoles participar en las distintas fases del combate de manera autónoma. Su función principal es simular la toma de decisiones de un jugador humano, introduciendo pausas temporales ("pensamiento") antes de realizar acciones como seleccionar cartas, lanzar dados, elegir tipos de ataque o moverse por el tablero.

El script coordina las acciones del bot con el estado global del combate a través del `CombatJudge`, y gestiona las interacciones con los elementos de UI y lógica específicos del juego, como las `HandCard` (cartas en mano), el `Dice` (dado) y los `RockBehavior` (rocas del tablero). Está configurado para ejecutarse temprano en el ciclo de vida de los scripts de Unity, asegurando que el bot pueda reaccionar a los cambios de fase del combate de forma oportuna.

La lógica se centra en una máquina de estados implícita dentro del método `Update`, que reacciona a los momentos del combate y al turno actual para ejecutar la acción apropiada con un tiempo de respuesta aleatorio, haciendo que la experiencia contra el bot sea más dinámica y menos predecible.

# Métodos

## Métodos de Unity

### Start
El método `Start` se ejecuta una vez al inicio del ciclo de vida del script. Su propósito es inicializar las referencias a otros componentes y variables esenciales para el funcionamiento del bot.

```csharp
void Start()
{
    // Inicializa las variables
    figther = GetComponent<Figther>();
    hand = transform.GetChild(0).GetChild(1);
    picking = false;
}
```

-   Obtiene una referencia al componente `Figther` adjunto al mismo GameObject. `Figther` representa al personaje del bot y es crucial para consultar su estado (e.g., si está en combate, qué carta ha elegido, su índice de jugador).
-   Localiza la `Transform` que representa la "mano de cartas" del bot. Se asume una estructura jerárquica específica donde la mano es el segundo hijo del primer hijo del GameObject actual. Los hijos de esta `Transform` de la mano son las `HandCard` individuales.
-   Establece la bandera `picking` a `false`. Esta bandera se utiliza para controlar el flujo de las decisiones del bot, evitando que una acción se repita innecesariamente en cada frame mientras el bot está "pensando" o en un estado de espera.

### Update
El método `Update` se invoca en cada frame y contiene la lógica principal del `BotPlayer` para la toma de decisiones durante el combate. Utiliza una serie de condiciones `if-else if` para detectar el momento actual del juego y el turno del bot, simulando el "pensamiento" y la acción subsiguiente.

```csharp
void Update()
{
    // Si no hay bot, no hace nada
    if (figther == null) return;
    // ... (lógica de decisiones del bot) ...
}
```

Primero, verifica si la referencia `figther` es `null`; si lo es, el script no procede con ninguna acción, actuando como una "cláusula de guarda" para evitar errores. A continuación, maneja varias fases del combate:

#### 1. Determinación del tiempo de "pensar" antes de elegir una carta
Si el bot está en combate, es el momento de `SetMoments.PickCard` (selección de carta), no ha seleccionado una carta aún y no está en proceso de "pensar" (`!picking`):
-   Registra el `Time.time` actual.
-   Establece un `total` de tiempo aleatorio entre 1 y 3 segundos para simular el tiempo que el bot tarda en "pensar".
-   Activa la bandera `picking` para indicar que el bot está en fase de "pensamiento".

```csharp
if (figther.IsFigthing() && CombatJudge.CombatJudgeInstance.GetSetMoments() == SetMoments.PickCard && figther.getPicked() == null && !picking)
{
    time = Time.time;
    total = Random.Range(1.0f, 3.0f);
    picking = true;
}
```

#### 2. Selección de carta una vez transcurrido el tiempo de "pensar"
Si el bot está en fase de "pensar" (`picking`) y el momento sigue siendo `SetMoments.PickCard`:
-   Comprueba si el tiempo transcurrido (`Time.time - time`) es mayor que el `total` establecido.
-   Si el tiempo ha transcurrido:
    -   Desactiva la bandera `picking`.
    -   Selecciona una carta al azar de entre los hijos de la `hand` del bot.
    -   Realiza un bucle de hasta 100 intentos para asegurar que la carta elegida no sea nula y sea `isClickable()` (seleccionable). Si la carta actual no es válida, avanza al siguiente índice (con `modulo 6` para mantenerse en el rango de 0 a 5).
    -   Una vez encontrada una carta válida, llama a `card.SelectedCard()` para que la carta se marque como elegida.

```csharp
else if (picking && CombatJudge.CombatJudgeInstance.GetSetMoments() == SetMoments.PickCard)
{
    if (Time.time - time > total)
    {
        picking = false;
        int a = Random.Range(0, 6); // a: Índice de la carta a elegir
        int b = 0; // b: Contador para limitar la búsqueda de cartas
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

#### 3. Establecimiento del tiempo antes de lanzar el dado
Si el momento actual es `SetMoments.PickDice` y es el turno del bot (`CombatJudge.CombatJudgeInstance.Turn() == figther.indexFigther`):
-   Registra el `Time.time` actual.
-   Establece un `total` de tiempo aleatorio entre 0.5 y 3 segundos para simular el tiempo antes de "lanzar" el dado.
-   Llama directamente a `FindFirstObjectByType<Dice>().Roll()` para iniciar la animación o lógica de lanzamiento del dado.

```csharp
else if (CombatJudge.CombatJudgeInstance.GetSetMoments() == SetMoments.PickDice && CombatJudge.CombatJudgeInstance.Turn() == figther.indexFigther)
{
    time = Time.time;
    total = Random.Range(0.5f, 3.0f);
    FindFirstObjectByType<Dice>().Roll();
}
```

#### 4. Finalización del lanzamiento del dado
Si el tiempo establecido ha transcurrido (`Time.time - time > total`) y sigue siendo el turno del bot:
-   Llama a `FindFirstObjectByType<Dice>().Unroll()` para finalizar la animación o lógica de lanzamiento del dado.

```csharp
else if (Time.time - time > total && CombatJudge.CombatJudgeInstance.Turn() == figther.indexFigther)
{
    FindFirstObjectByType<Dice>().Unroll();
}
```

#### 5. Establecimiento del tiempo antes de elegir el tipo de ataque
Si el momento actual es `SetMoments.SelectCombat`, el bot no está "pensando" (`!picking`) y es su turno:
-   Registra el `Time.time` actual.
-   Establece un `total` de tiempo aleatorio entre 0.5 y 2 segundos para simular el "pensamiento" antes de elegir el tipo de ataque.
-   Activa la bandera `picking`.

```csharp
else if (CombatJudge.CombatJudgeInstance.GetSetMoments() == SetMoments.SelectCombat && !picking && CombatJudge.CombatJudgeInstance.Turn() == figther.indexFigther)
{
    time = Time.time;
    total = Random.Range(0.5f, 2.0f);
    picking = true;
}
```

#### 6. Elección del elemento de ataque una vez transcurrido el tiempo de "pensar"
Si el momento actual es `SetMoments.SelectCombat`, el bot está "pensando" (`picking`) y es su turno:
-   Comprueba si el tiempo establecido ha transcurrido.
-   Si el tiempo ha transcurrido:
    -   Desactiva la bandera `picking`.
    -   Inicializa un arreglo `elem` de tamaño 4 para contar la cantidad de cartas de cada tipo elemental que el bot tiene en su mano.
    -   Itera sobre todas las cartas en la mano (`hand.childCount`), extrae el componente `HandCard` y cuenta el número de cartas para cada elemento utilizando el valor `(int)card.GetCard().GetElement()`.
    -   Encuentra el índice del elemento (`big`) que tiene la mayor cantidad de cartas.
    -   Llama a `GetComponentInChildren<SelectType>().PickElement(big)` para seleccionar el elemento de ataque con la mayor cantidad de cartas en la mano del bot. Se asume que `SelectType` es un componente hijo que gestiona la selección del tipo de ataque.

```csharp
else if (CombatJudge.CombatJudgeInstance.GetSetMoments() == SetMoments.SelectCombat && picking && CombatJudge.CombatJudgeInstance.Turn() == figther.indexFigther)
{
    if (Time.time - time > total)
    {
        picking = false;
        int[] elem = new int[4]; // Array para contar cartas por elemento

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

### `void PickRock(RockBehavior[] rocks)`
Este método es invocado externamente para preparar al bot para que elija una de las rocas disponibles en el tablero a la cual moverse.

-   **Parámetros:**
    -   `rocks` (`RockBehavior[]`): Un arreglo de objetos `RockBehavior` que representan las rocas del tablero a las que el bot puede moverse.
-   **Funcionamiento:**
    -   Reinicia la bandera `picking` a `false` para asegurar que el bot esté listo para una nueva decisión.
    -   Registra el `Time.time` actual para iniciar un temporizador.
    -   Establece un `total` de tiempo aleatorio (entre 1 y 2 segundos) para simular el "pensamiento" del bot antes de elegir la roca.
    -   Almacena el arreglo `rocks` recibido en la variable de instancia `this.rocks` para que `ThinkingRocks()` pueda acceder a ellas más tarde.

### `void ThinkingRocks()`
Este método se encarga de ejecutar la decisión del bot de elegir una roca, pero solo después de que el tiempo de "pensamiento" establecido por `PickRock` haya transcurrido.

-   **Funcionamiento:**
    -   Verifica si el tiempo transcurrido desde que se llamó a `PickRock` (`Time.time - time`) es mayor que el `total` de "pensamiento".
    -   Si el tiempo ha transcurrido:
        -   Selecciona una roca al azar del arreglo `rocks` previamente almacenado.
        -   Notifica al `CombatJudge` (`CombatJudge.CombatJudgeInstance.MoveToRock()`) sobre la roca elegida, indicando el movimiento deseado del bot.

## Getters y Setters
El script `BotPlayer` no expone directamente métodos públicos que actúen como "Getters" o "Setters" para sus variables de instancia internas (`figther`, `hand`, `time`, `total`, `picking`, `rocks`). En su lugar, gestiona su estado internamente y utiliza métodos públicos (`PickRock`, `ThinkingRocks`) para reaccionar a eventos o para realizar acciones en el juego. Las interacciones con el estado de otros componentes se realizan llamando a sus propios métodos o propiedades.
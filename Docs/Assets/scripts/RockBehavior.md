# `RockBehavior`
Este script define el comportamiento de los objetos "roca" en el juego, que actúan como nodos estratégicos o puntos de interacción para los jugadores. Cada roca posee una `Inscription` (inscripción) que representa su tipo elemental o de efecto, determinando su apariencia visual y su posible funcionalidad en el combate. Las rocas están organizadas y gestionadas por un componente `PlayZone` padre, lo que sugiere una disposición espacial específica, probablemente circular, donde los jugadores pueden moverse y sobre las cuales pueden interactuar. El script maneja la visualización de la roca (sprite, color, brillo), la detección de clics del jugador y la gestión de qué `PlayerToken`s (fichas de jugador) se encuentran actualmente sobre ella.

La ejecución de este script está configurada con `DefaultExecutionOrder(-2)`, lo que significa que sus métodos de ciclo de vida (como `Start`) se ejecutarán muy temprano en el orden de los scripts de Unity, asegurando que las rocas estén inicializadas antes que la mayoría de otros componentes que puedan depender de ellas.

# Métodos

## Métodos de Unity

### `Start`
Este método se llama una vez en el ciclo de vida del script, justo antes de la primera llamada a `Update`, después de que el GameObject se activa. Se encarga de la inicialización visual y posicional de la roca.

1.  **Obtención de referencias a `SpriteRenderer`:**
    Se obtienen referencias a los componentes `SpriteRenderer` para las diferentes partes visuales de la roca:
    -   `simbol`: El renderizador del sprite de la inscripción, que se encuentra en el segundo hijo del GameObject (`transform.GetChild(1)`).
    -   `shyn`: El renderizador del sprite de brillo, ubicado en el primer hijo (`transform.GetChild(0)`).
    -   `itself`: El renderizador del sprite de la roca principal, adjunto al propio GameObject.
2.  **Configuración visual según la inscripción:**
    Se asigna el sprite y el color de la roca y su inscripción basándose en el valor de la variable `inscription`. Los arrays `sprite` y `colors` deben estar configurados en el Inspector de Unity con los activos visuales correspondientes a cada `Inscription`.
    ```csharp
    simbol.sprite = sprite[(int)inscription];
    itself.color = colors[(int)inscription];
    ```
3.  **Posicionamiento y Rotación:**
    Si la roca tiene una referencia válida a un `PlayZone` padre (`father`), se calcula su posición, escala y rotación.
    -   **Posición:** Se coloca la roca a una distancia `father.radius` del centro del `PlayZone` en la dirección especificada por `direction`. Esto sugiere una disposición radial o circular de las rocas.
        ```csharp
        transform.position = father.transform.position + direction * father.radius;
        ```
    -   **Escala:** Se ajusta la escala de la roca utilizando el valor `father.RockScale`, lo que permite al `PlayZone` controlar el tamaño de todas las rocas bajo su gestión.
        ```csharp
        transform.localScale = Vector3.one * father.RockScale;
        ```
    -   **Rotación:** Se rota el GameObject para que mire en una dirección específica. La rotación `Quaternion.Euler(90, angle - 90, 0)` parece orientar la roca de manera que esté "mirando hacia arriba" (90 grados en X) y luego "girada hacia la izquierda" (Y) según un `angle` predefinido, lo cual es común en arreglos circulares donde cada elemento debe apuntar hacia afuera o hacia el centro.

### `Update`
Este método se llama una vez por fotograma. Su función principal es controlar la visibilidad del efecto de brillo de la roca (`shyn`) basándose en el estado actual del combate.

-   **Control del brillo:** Si el momento actual del juego, según lo determinado por el `CombatJudge` (un componente global de gestión del combate), *no* es `SetMoments.GlowRock`, la propiedad `shiny` de la roca se establece en `false`. Posteriormente, el GameObject `shyn` (que probablemente contiene el efecto visual de brillo) se activa o desactiva según el valor de `shiny`.
    ```csharp
    if (CombatJudge.Instance.GetSetMoments() != SetMoments.GlowRock) shiny = false;
    shyn.gameObject.SetActive(shiny);
    ```
    Esto asegura que las rocas solo brillen cuando el `CombatJudge` lo permite, por ejemplo, durante una fase del turno del jugador donde se espera que seleccione una roca.

### `OnMouseDown`
Este método se invoca cuando el usuario presiona el botón del ratón mientras el cursor está sobre un `Collider` de este GameObject (la roca). Es crucial para la interacción del jugador.

-   **Condición de interacción:** La roca solo responde a un clic si `shiny` es `true` (está brillando) y si el `CombatJudge` indica que es el turno del jugador (`CombatJudge.Instance.FocusOnTurn()` devuelve `true`).
-   **Acción:** Si las condiciones se cumplen, se busca la instancia del `CombatJudge` en la escena y se llama a su método `MoveToRock()`, pasándole esta `RockBehavior` como argumento. Esto indica que el `CombatJudge` es responsable de procesar el movimiento del jugador hacia la roca seleccionada.
    ```csharp
    if (shiny && CombatJudge.Instance.FocusOnTurn()) FindFirstObjectByType<CombatJudge>().MoveToRock(this);
    ```

## Otros métodos

### `RockBehavior[] getNeighbor(int al)`
Este método devuelve un array con dos referencias a objetos `RockBehavior` que son vecinos de la roca actual en la disposición del `PlayZone` padre.

-   **`al` (int):** Representa la distancia o "pasos" para encontrar a los vecinos. Por ejemplo, `al=1` para vecinos adyacentes, `al=2` para vecinos a dos posiciones de distancia, etc.
-   **Funcionamiento:**
    1.  Obtiene la cantidad total de rocas (`many`) del `PlayZone` padre.
    2.  Calcula los índices de los vecinos en ambas direcciones (uno en el sentido de las agujas del reloj, otro en sentido contrario) utilizando aritmética modular (`% many`) para manejar el "bucle" de una disposición circular.
    3.  Accede a los `RockBehavior` de esos GameObjects hijos del `PlayZone` padre.
    4.  Retorna un array con las dos referencias encontradas.
    ```csharp
    int many = father.many;
    RockBehavior[] neighbors = new RockBehavior[2];
    int oneside = ((numbchild + al) % many);
    int otherside = ((numbchild - al + many) % many);
    neighbors[0] = father.transform.GetChild(oneside).GetComponent<RockBehavior>();
    neighbors[1] = father.transform.GetChild(otherside).GetComponent<RockBehavior>();
    return neighbors;
    ```
    Este método es fundamental para la navegación y las mecánicas de adyacencia en el tablero de juego.

### `void AddPlayer(PlayerToken token)`
Este método permite añadir un `PlayerToken` a la roca, registrando que un jugador se ha movido o está presente en esta ubicación.

-   **Manejo de array dinámico:**
    -   Si `playersOn` es `null` (es el primer jugador en la roca), se crea un nuevo array de tamaño 1 y se asigna el `token`.
    -   Si ya hay jugadores, se crea un nuevo array `newPlay` con un tamaño mayor. Los jugadores existentes se copian al nuevo array, y el nuevo `token` se añade al final. Finalmente, `playersOn` se actualiza con `newPlay`.
    -   Aunque no es la forma más performante de manejar colecciones dinámicas en C# (una `List<PlayerToken>` sería más eficiente), este enfoque cumple su función y es directo para un proyecto indie.

### `void RemovePlayer(PlayerToken token)`
Este método elimina un `PlayerToken` específico de la roca, lo que ocurre cuando un jugador se mueve fuera de ella.

-   **Manejo de array dinámico:**
    -   Si `playersOn` es `null`, no hay jugadores que remover, por lo que el método retorna.
    -   Se itera sobre `playersOn` para encontrar y "marcar" (estableciéndolos en `null`) todos los `token`s que coinciden con el que se va a remover, contando cuántos se eliminan (`a`).
    -   Se crea un nuevo array `newPlay` con el tamaño ajustado (`playersOn.Length - a`).
    -   Se copian solo los `PlayerToken`s no nulos (los que no fueron removidos) al nuevo array.
    -   Finalmente, `playersOn` se actualiza con `newPlay`.
    Este proceso permite mantener el array `playersOn` sin elementos nulos después de la eliminación.

### `bool manyOn()`
Este método verifica si hay más de un `PlayerToken` ocupando esta roca.

-   Retorna `true` si el array `playersOn` no es `null` y su propiedad `Length` es mayor que 1. En caso contrario, retorna `false`. Es útil para mecánicas de juego que activan efectos cuando varios jugadores están en la misma roca.

### `int GetPlayersOn()`
Este método calcula y devuelve un valor entero que representa un "bitmask" o una combinación numérica de los jugadores presentes en la roca.

-   Para cada `PlayerToken` (`p`) en la roca, se calcula `2` elevado a la potencia de `p.player.indexFighter` y se suma al resultado total.
-   `p.player.indexFighter` es probable un índice único para cada jugador (e.g., Jugador 0, Jugador 1). Esto permite identificar de forma compacta qué jugadores están en la roca (por ejemplo, si Jugador 0 y Jugador 2 están en la roca, el valor podría ser `2^0 + 2^2 = 1 + 4 = 5`).
    ```csharp
    int many = 0;
    foreach (PlayerToken p in playersOn)
    {
        many += (int)Mathf.Pow(2,p.player.indexFighter);
    }
    return many;
    ```
    Es útil para lógicas de combate o eventos que dependen de combinaciones específicas de jugadores.

### `int ManyPlayerOn()`
Este método devuelve el número exacto de `PlayerToken`s que se encuentran actualmente en esta roca.

-   Simplemente retorna la `Length` del array `playersOn`. Es una forma directa de saber cuántos jugadores están en una ubicación específica.

## Getters y Setters

1.  `father`: Obtiene o establece la referencia al `PlayZone` padre de esta roca.
2.  `inscription`: Obtiene o establece el tipo de inscripción que tiene la roca, que define sus propiedades y apariencia.
3.  `direction`: Obtiene o establece el vector de dirección utilizado para el posicionamiento radial de la roca desde el centro del `PlayZone`.
4.  `angle`: Obtiene o establece el ángulo de rotación de la roca, usado para su orientación en el `PlayZone`.
5.  `numbchild`: Obtiene o establece un identificador numérico o índice para esta roca dentro de su `PlayZone` padre.
6.  `shiny`: Obtiene o establece un booleano que controla si la roca debe mostrar su efecto de brillo, indicando que es interactiva o relevante en el momento actual.
# RockBehavior
El script `RockBehavior` es un componente central en el proyecto, diseñado para gestionar el comportamiento y la representación de cada "roca" individual en el tablero de juego. Estas rocas actúan como espacios interactivos donde los `PlayerToken` (fichas de los jugadores o personajes) pueden posicionarse y realizar acciones.

Cada instancia de `RockBehavior` se define por su `Inscription` (tipo de inscripción), que puede ser `fire`, `earth`, `water`, `air`, `pick`, `duel` o `empty`. Este tipo no solo determina su apariencia visual (sprite y color), sino también su rol potencial en las mecánicas de juego.

Las funciones principales del script incluyen:

-   **Inicialización y Configuración Visual:** Al inicio, la roca carga el sprite y el color adecuados según su `Inscription`.
-   **Posicionamiento Dinámico:** Se posiciona y escala automáticamente en el tablero de juego, tomando como referencia un componente `PlayZone` padre.
-   **Control de Estado Visual:** Gestiona un efecto de brillo (`shiny`) que se activa o desactiva en función de las fases de combate.
-   **Interacción del Jugador:** Permite que los jugadores hagan clic en la roca para interactuar con ella, siempre que ciertas condiciones de juego se cumplan.
-   **Gestión de Ocupación:** Mantiene un registro de qué `PlayerToken` están actualmente sobre ella, permitiendo añadir, eliminar y contar estos tokens.
-   **Acceso a Vecinos:** Ofrece un método para consultar las rocas adyacentes, facilitando la navegación y la lógica de movimiento en un tablero circular.

El script utiliza el atributo `[DefaultExecutionOrder(-2)]`, lo que garantiza que su método `Start` se ejecute muy temprano en el ciclo de vida de los scripts de Unity. Esto asegura que todas las propiedades visuales y posicionales de las rocas estén configuradas antes de que la mayoría de otros scripts o sistemas intenten interactuar con ellas.

# Métodos

## Métodos de Unity

### Start
Este método se invoca una vez al inicio del ciclo de vida del script, antes de la primera actualización de la escena. Su propósito principal en `RockBehavior` es inicializar la apariencia y la posición de la roca en el tablero.

1.  **Obtención de Componentes `SpriteRenderer`:** Se buscan y asignan referencias a los `SpriteRenderer` necesarios para la roca y sus efectos visuales. Se asume una estructura de GameObject específica:
    ```csharp
    simbol = transform.GetChild(1).GetComponent<SpriteRenderer>(); // El símbolo de la inscripción.
    shyn = transform.GetChild(0).GetComponent<SpriteRenderer>();   // El efecto de brillo.
    itself = transform.GetComponent<SpriteRenderer>();           // La roca base.
    ```
    Esto implica que el GameObject de la roca tiene al menos dos hijos, uno para el brillo y otro para el símbolo.

2.  **Configuración Visual Inicial:** Utiliza la propiedad `inscription` (expuesta en el Inspector y con valor por defecto `Inscription.empty`) para asignar el `Sprite` y `Color` correctos de la roca desde los arrays `sprite` y `colors` predefinidos.
    ```csharp
    simbol.sprite = sprite[(int)inscription];
    itself.color = colors[(int)inscription];
    ```

3.  **Posicionamiento y Rotación Dinámica:** Si la roca tiene una referencia a un `PlayZone` (`father`), se posiciona, escala y rota en relación con este `PlayZone`. Esto sugiere que `PlayZone` es el contenedor que orquesta la disposición de todas las rocas en un patrón específico, probablemente circular.
    ```csharp
    if (father == null) return; // Si no hay PlayZone padre, no se posiciona dinámicamente.
    transform.position = father.transform.position + direction * father.radius;
    transform.localScale = Vector3.one * father.RockScale;
    transform.rotation = Quaternion.Euler(90, angle - 90, 0); // Ajusta la rotación para que el sprite mire hacia arriba y se oriente en el plano horizontal.
    ```
    Los valores `direction`, `angle` y `numbchild` son propiedades que probablemente son configuradas por el `PlayZone` para cada roca individual.

### Update
Este método se ejecuta en cada frame del juego. En `RockBehavior`, su única responsabilidad es mantener actualizado el estado visual del efecto de brillo (`shyn`) de la roca.

-   **Control del Efecto de Brillo:** Consulta el estado actual del juego a través del singleton `CombatJudge.CombatJudgeInstance`. Si la fase de combate actual *no* es `SetMoments.GlowRock`, la propiedad `shiny` de esta roca se desactiva. Luego, el GameObject `shyn` (que contiene el `SpriteRenderer` para el brillo) se activa o desactiva para reflejar el estado actual de `shiny`.
    ```csharp
    if (CombatJudge.CombatJudgeInstance.GetSetMoments() != SetMoments.GlowRock) shiny = false;
    shyn.gameObject.SetActive(shiny);
    ```
    Esto asegura que el efecto de brillo solo sea visible cuando `CombatJudge` lo permite explícitamente y durante la fase `GlowRock`.

### OnMouseDown
Este método es un evento callback de Unity que se dispara cuando el usuario hace clic o toca el collider 2D/3D adjunto a este GameObject. `OnMouseDown` en `RockBehavior` gestiona la interacción del jugador con la roca.

-   **Condiciones de Interacción:** La roca solo responde a un clic si se cumplen dos criterios esenciales:
    1.  `shiny` es `true`: La roca debe estar visiblemente "brillando".
    2.  `CombatJudge.CombatJudgeInstance.FocusOnTurn()`: El `CombatJudge` debe confirmar que es el turno del jugador o una fase donde la interacción con las rocas está permitida.
-   **Activación de Acción:** Si ambas condiciones son verdaderas, se invoca el método `MoveToRock` del `CombatJudge`, pasándole la instancia actual de `RockBehavior`. Esto indica que el `CombatJudge` es el encargado de procesar la selección de la roca, posiblemente para mover una ficha o activar una habilidad.
    ```csharp
    if (shiny && CombatJudge.CombatJudgeInstance.FocusOnTurn()) FindFirstObjectByType<CombatJudge>().MoveToRock(this);
    ```

## Otros métodos

### public RockBehavior[] getNeighbor(int al)
Este método está diseñado para encontrar las rocas vecinas a la roca actual en el `PlayZone` circular.

-   **Parámetro `al` (int):** Representa la distancia o el desplazamiento en el círculo para encontrar a los vecinos. Un valor de `1` buscará los vecinos inmediatos; un valor mayor buscará rocas más alejadas en ambas direcciones.
-   **Funcionamiento:** Calcula dos índices, `oneside` y `otherside`, que corresponden a las posiciones de los vecinos en el arreglo de hijos del `PlayZone` padre. Utiliza el operador módulo (`%`) con `father.many` (el número total de rocas) para manejar el comportamiento de "vuelta al inicio" en el diseño circular. Finalmente, recupera el componente `RockBehavior` de los hijos del `PlayZone` en esas posiciones.
-   **Retorno:** Un array de dos objetos `RockBehavior` que representan los vecinos calculados.
    ```csharp
    public RockBehavior[] getNeighbor(int al)
    {
        int many = father.many;
        RockBehavior[] neighbors = new RockBehavior[2];
        int oneside = ((numbchild + al) % many);
        int otherside = ((numbchild - al + many) % many); // + many para asegurar un resultado positivo antes del módulo.
        neighbors[0] = father.transform.GetChild(oneside).GetComponent<RockBehavior>();
        neighbors[1] = father.transform.GetChild(otherside).GetComponent<RockBehavior>();
        return neighbors;
    }
    ```

### public void AddPlayer(PlayerToken token)
Añade un `PlayerToken` a la colección (`playersOn`) de fichas que se encuentran actualmente en esta roca.

-   **Parámetro `token` (PlayerToken):** La instancia de `PlayerToken` que se va a añadir.
-   **Funcionamiento:** Este método gestiona dinámicamente el array `playersOn`. Si `playersOn` es `null`, lo inicializa con el `token`. Si ya contiene elementos, crea un nuevo array de mayor tamaño, copia todos los elementos existentes y añade el nuevo `token` al final, reasignando `playersOn` al nuevo array. Este enfoque, aunque funcional, implica la recreación del array en cada adición, lo cual es una consideración de rendimiento para grandes cantidades de operaciones o en escenarios con muchos jugadores.

### public void RemovePlayer(PlayerToken token)
Elimina una instancia específica de `PlayerToken` de la colección (`playersOn`) de fichas que se encuentran en esta roca.

-   **Parámetro `token` (PlayerToken):** La instancia de `PlayerToken` que se va a eliminar.
-   **Funcionamiento:** Primero, itera sobre `playersOn` para identificar y marcar con `null` todas las ocurrencias del `token` a eliminar, y cuenta cuántos elementos se marcaron. Luego, crea un nuevo array de menor tamaño y copia solo los `PlayerToken` no nulos, reasignando `playersOn` al nuevo array. Al igual que `AddPlayer`, este método implica la recreación del array para la eliminación.

## Getters y Setters

1.  `getNeighbor (RockBehavior[])`: Calcula y devuelve un array de dos objetos `RockBehavior` que son los vecinos de esta roca, basándose en un índice de distancia.
2.  `AddPlayer (void)`: Añade un `PlayerToken` a la colección de jugadores que ocupan esta roca.
3.  `RemovePlayer (void)`: Elimina un `PlayerToken` específico de la colección de jugadores que ocupan esta roca.
4.  `manyOn (bool)`: Indica si hay más de un `PlayerToken` en esta roca (`true`) o no (`false`).
5.  `GetPlayersOn (int)`: Devuelve un valor entero que representa la combinación de `PlayerToken` presentes en la roca, codificado como un bitmask basado en el `indexFigther` de cada jugador. Útil para identificar de forma compacta qué jugadores están presentes.
6.  `ManyPlayerOn (int)`: Devuelve el número total de `PlayerToken` que se encuentran actualmente en esta roca.
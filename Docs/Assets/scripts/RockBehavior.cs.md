# `RockBehavior.cs`

## 1. Propósito General
Este script gestiona el comportamiento individual de una "roca" en el tablero de juego. Se encarga de su inicialización visual y posicional, de reaccionar a la interacción del jugador (clics), y de mantener un registro de los `PlayerToken` (fichas de jugador) que se encuentran sobre ella, permitiendo también la consulta de rocas vecinas.

## 2. Componentes Clave

### `Inscription` (Enum)
- **Descripción:** Define los diferentes tipos de "inscripciones" o elementos que una roca puede poseer. Estos valores numéricos se utilizan para indexar los `Sprite` asociados a cada tipo.
- **Valores:**
    - `fire = 0`: Elemento Fuego.
    - `earth = 1`: Elemento Tierra.
    - `water = 2`: Elemento Agua.
    - `air = 3`: Elemento Aire.
    - `duel = 4`: Elemento Duelo.
    - `pick = 6`: Elemento Selección.
    - `empty = 5`: Sin elemento.

### `RockBehavior` (Class)
- **Descripción:** Componente `MonoBehaviour` que controla la lógica y la representación de una roca individual en el escenario del juego. Cada instancia de `RockBehavior` representa un punto estratégico en el tablero. Se ejecuta con una `DefaultExecutionOrder` de `-2`, lo que significa que se inicializa muy temprano en el ciclo de vida de los scripts de Unity.
- **Variables Públicas / Serializadas:**
    - `public PlayZone father;`: Referencia al objeto `PlayZone` padre al que pertenece esta roca. Se utiliza para la posición relativa y la escala.
    - `[SerializeField] private Sprite[] sprite;`: Un arreglo de `Sprite` que almacena las imágenes para cada tipo de `Inscription`. Se asigna en el Inspector de Unity.
    - `[SerializeField] public Inscription inscription = Inscription.empty;`: El tipo de inscripción elemental de esta roca. Determina el `Sprite` del símbolo mostrado. Se puede configurar en el Inspector.
    - `public Vector3 direction = Vector3.forward;`: Vector de dirección utilizado para calcular la posición de la roca respecto a su `father`.
    - `public float angle = 0;`: Ángulo de rotación de la roca, también usado en el posicionamiento relativo.
    - `public int numbchild = 0;`: Un índice que identifica la posición de esta roca dentro de su `PlayZone` padre, útil para cálculos de vecinos.
    - `public bool shiny = false;`: Booleano que controla si la roca debe "brillar" (cambiar a color amarillo) para indicar que es interactuable.

- **Métodos Principales:**
    - `void Start()`:
        - **Descripción:** Método del ciclo de vida de Unity que se ejecuta una vez al inicio. Inicializa referencias a los `SpriteRenderer` (`simbol` para la inscripción y `itself` para la roca misma). Asigna el sprite correcto al símbolo basado en la `inscription` de la roca. Si tiene un `father` (PlayZone), calcula y aplica la posición, escala y rotación de la roca en relación con este, colocándola en un círculo.
        - **Fragmento de Código Clave:**
            ```csharp
            simbol = transform.GetChild(0).GetComponent<SpriteRenderer>();
            itself = transform.GetComponent<SpriteRenderer>();
            simbol.sprite = sprite[(int)inscription];
            // ... posicionamiento relativo a 'father' ...
            ```

    - `void Update()`:
        - **Descripción:** Método del ciclo de vida de Unity que se ejecuta en cada frame. Comprueba el estado del juego a través de `Combatjudge.combatjudge.GetSetMoments()`. Si el momento actual no es `SetMoments.GlowRock`, la propiedad `shiny` se establece en `false`. Actualiza el color de la propia roca (`itself.color`) a amarillo si `shiny` es `true`, o a negro si es `false`, indicando si es interactuable.

    - `private void OnMouseDown()`:
        - **Descripción:** Callback de Unity que se invoca cuando el usuario hace clic con el ratón sobre el collider de esta roca. Si la roca está marcada como `shiny` (brillante), invoca el método `MoveToRock` del `Combatjudge` central, pasándose a sí misma como el destino del movimiento.
        - **Fragmento de Código Clave:**
            ```csharp
            if (shiny) FindFirstObjectByType<Combatjudge>().MoveToRock(this);
            ```

    - `public RockBehavior[] getNeighbor(int al)`:
        - **Descripción:** Devuelve un arreglo de dos objetos `RockBehavior` que representan los vecinos de esta roca, a una distancia `al` (offset) en ambas direcciones (en sentido horario y antihorario) alrededor de la `PlayZone` padre. Utiliza la propiedad `numbchild` y el número total de rocas en la zona (`father.many`) para calcular los índices de los vecinos.
        - **Parámetros:**
            - `al`: Un entero que representa la distancia (número de posiciones) al vecino.
        - **Retorna:** Un arreglo `RockBehavior[]` con dos elementos: el vecino en una dirección y el vecino en la otra.

    - `public void AddPlayer(PlayerToken token)`:
        - **Descripción:** Agrega un `PlayerToken` al arreglo `playersOn`, que rastrea qué fichas están actualmente sobre esta roca. Si el arreglo es nulo, lo inicializa; de lo contrario, lo redimensiona para incluir el nuevo token.

    - `public void RemovePlayer(PlayerToken token)`:
        - **Descripción:** Elimina un `PlayerToken` específico del arreglo `playersOn`. Busca el token, lo marca como nulo y luego crea un nuevo arreglo sin el token eliminado, redimensionando el arreglo dinámicamente.

    - `public bool manyOn()`:
        - **Descripción:** Comprueba si hay más de un `PlayerToken` presente sobre esta roca.
        - **Retorna:** `true` si `playersOn` no es nulo y su longitud es mayor que 1; de lo contrario, `false`.

    - `public int GetPlayersOn()`:
        - **Descripción:** Calcula y devuelve un valor entero basado en la suma de potencias de 2 (`2^indexFigther`) para cada `PlayerToken` en la roca. Esto podría ser un identificador único o una máscara de bits que representa la combinación de jugadores presentes.

    - `public int ManyPlayerOn()`:
        - **Descripción:** Devuelve el número total de `PlayerToken` que se encuentran actualmente sobre esta roca. Es decir, la longitud del arreglo `playersOn`.

- **Lógica Clave:**
    - **Posicionamiento Circular:** En `Start`, la roca se posiciona dinámicamente en un patrón circular alrededor de su `PlayZone` padre, utilizando `direction`, `angle`, `father.radius` y `father.RockScale`.
    - **Gestión de Jugadores:** Los métodos `AddPlayer` y `RemovePlayer` permiten mantener una lista dinámica de `PlayerToken` sobre la roca, aunque la implementación de redimensionamiento de arreglos es manual y podría ser más eficiente usando `List<PlayerToken>`.
    - **Interacción de Brillo y Click:** La roca solo es interactuable mediante clic si su propiedad `shiny` es `true`, lo que a su vez es controlado por el estado del juego definido en `Combatjudge`.

## 3. Dependencias y Eventos

-   **Componentes Requeridos:** Ningún componente está explícitamente requerido mediante `[RequireComponent]`. Sin embargo, el script asume la existencia de un `SpriteRenderer` en sí mismo y en su primer hijo.
-   **Eventos (Entrada):**
    -   `OnMouseDown()`: Este script se suscribe implícitamente al evento de clic del ratón de Unity, ejecutando su lógica cuando el usuario hace clic en el objeto de juego al que está adjunto este script.
-   **Eventos (Salida):**
    -   Este script no emite eventos de forma explícita (como `UnityEvent` o `Action`). En su lugar, interactúa directamente con el `Combatjudge` central llamando a su método `MoveToRock` cuando la roca es clicada y está "brillante". Esto crea un acoplamiento directo con la clase `Combatjudge`.
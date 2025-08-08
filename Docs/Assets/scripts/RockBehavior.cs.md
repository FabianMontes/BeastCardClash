# `RockBehavior.cs`

## 1. Propósito General
Este script gestiona el comportamiento, la representación visual y la interacción de una "Roca" individual dentro de una `PlayZone` en el juego. Su rol principal es definir las propiedades de la roca (como su inscripción elemental), controlar su estado visual (por ejemplo, si brilla o no) y registrar qué `PlayerToken`s (fichas de jugador) se encuentran sobre ella. Interactúa con el sistema `Combatjudge` para permitir movimientos de jugadores a estas rocas durante fases específicas del juego.

## 2. Componentes Clave

### `Inscription` (enum)
-   **Descripción:** Este `enum` define los distintos tipos de "inscripciones" o elementos que puede poseer una roca. Cada valor entero del enum se asocia a un tipo específico de sprite que representa la inscripción visualmente y, presumiblemente, determina sus efectos en la jugabilidad.
-   **Valores:**
    -   `fire = 0`
    -   `earth = 1`
    -   `water = 2`
    -   `air = 3`
    -   `duel = 4`
    -   `empty = 5`
    -   `pick = 6`
    (Nótese que `empty` y `pick` no siguen un orden consecutivo con los elementos, lo que indica que sus valores numéricos son significativos para el mapeo con el array de sprites.)

### `RockBehavior` (class)
-   **Descripción:** Esta clase, que hereda de `MonoBehaviour`, es el controlador principal para cada objeto "Roca" en la escena. Se encarga de su inicialización posicional y de rotación dentro de una `PlayZone`, de la actualización de su color y brillo basado en el estado del juego, y de la lógica para añadir o remover `PlayerToken`s que se mueven a ella.
-   **Variables Públicas / Serializadas:**
    -   `public PlayZone father`: Referencia al objeto `PlayZone` (zona de juego) al que pertenece esta roca. Se utiliza para calcular la posición, escala y rotación de la roca, así como para acceder a las rocas vecinas.
    -   `[SerializeField] private Sprite[] sprite`: Un array de `Sprite`s que contiene las imágenes para cada tipo de `Inscription`. El sprite a mostrar para la roca se selecciona usando el valor entero de `inscription` como índice.
    -   `[SerializeField] public Inscription inscription = Inscription.empty`: El tipo de inscripción asignado a esta roca específica. Determina el símbolo visual que se mostrará y, a nivel de juego, qué tipo de efecto o propiedad tiene la roca.
    -   `public Vector3 direction = Vector3.forward`: Vector que indica la dirección desde el centro del `PlayZone` padre hacia esta roca. Es clave para el posicionamiento radial de la roca.
    -   `public float angle = 0`: Ángulo de rotación que contribuye a la orientación de la roca.
    -   `public int numbchild = 0`: Un índice numérico que representa la posición de esta roca entre las rocas hijas de su `PlayZone` padre. Es fundamental para determinar los vecinos.
    -   `public bool shiny = false`: Un flag booleano que controla si la roca debe aparecer brillante (de color amarillo). Las rocas brillantes son interactivas y pueden ser seleccionadas por el jugador.
-   **Métodos Principales:**
    -   `void Start()`: Se invoca una vez al inicio del ciclo de vida del script.
        -   Obtiene referencias a los componentes `SpriteRenderer` de la roca y de su símbolo.
        -   Asigna el sprite correcto al símbolo de la roca basándose en su `inscription`.
        -   Si tiene un `PlayZone` padre asignado, calcula y aplica su posición, escala y rotación. La posición se calcula radialmente desde el centro del padre, la escala se ajusta por `father.RockScale`, y la rotación asegura que el símbolo mire hacia arriba (`90` grados en X) y se alinee correctamente con el ángulo (`angle - 90` en Y).
        ```csharp
        simbol = transform.GetChild(0).GetComponent<SpriteRenderer>();
        itself = transform.GetComponent<SpriteRenderer>();
        simbol.sprite = sprite[(int)inscription];

        if (father == null) return;

        transform.position = father.transform.position + direction * father.radius;
        transform.localScale = Vector3.one * father.RockScale;
        transform.rotation = Quaternion.Euler(90, angle - 90, 0);
        ```
    -   `void Update()`: Se llama una vez por fotograma.
        -   Controla el estado `shiny` de la roca: si el momento actual del `Combatjudge` no es `GlowRock` (es decir, no es el momento en que las rocas deben brillar), `shiny` se establece en `false`.
        -   Actualiza el color del `SpriteRenderer` principal de la roca (`itself`) a amarillo si `shiny` es `true`, o a negro si es `false`, proporcionando retroalimentación visual al jugador.
        ```csharp
        if (Combatjudge.combatjudge.GetSetMoments() != SetMoments.GlowRock) shiny = false;
        itself.color = shiny ? Color.yellow : Color.black;
        ```
    -   `private void OnMouseDown()`: Este método se invoca cuando el usuario hace clic con el botón principal del ratón sobre el collider 2D/3D asociado a esta roca.
        -   Si la roca está marcada como `shiny` (brillante y seleccionable), busca la instancia activa de `Combatjudge` en la escena e invoca su método `MoveToRock`, pasando esta roca como el destino del movimiento. Esto inicia el proceso de movimiento de un `PlayerToken` a esta roca.
        ```csharp
        if (shiny) FindFirstObjectByType<Combatjudge>().MoveToRock(this);
        ```
    -   `public RockBehavior[] getNeighbor(int al)`: Retorna un array de dos `RockBehavior`s que son vecinos de esta roca en la `PlayZone`. El parámetro `al` (probablemente "alrededor" o "a los lados") indica la distancia en número de rocas para encontrar a los vecinos en ambas direcciones (izquierda y derecha) a lo largo del círculo de rocas. Utiliza operaciones de módulo para manejar el "envoltorio" de las posiciones en el círculo.
    -   `public void AddPlayer(PlayerToken token)`: Añade un `PlayerToken` a la lista interna `playersOn` que rastrea qué jugadores están sobre esta roca. Si la lista es nula, se inicializa; de lo contrario, se crea un nuevo array más grande para incluir el nuevo token y se copia la información existente.
    -   `public void RemovePlayer(PlayerToken token)`: Elimina un `PlayerToken` específico de la lista `playersOn`. Recorre la lista para encontrar y "eliminar" el token (estableciéndolo en `null` temporalmente), luego reconstruye un nuevo array sin los elementos nulos.
    -   `public bool manyOn()`: Retorna `true` si hay más de un `PlayerToken` en esta roca, de lo contrario `false`.
    -   `public int GetPlayersOn()`: Calcula un valor entero combinando los índices de los jugadores presentes en la roca. Para cada `PlayerToken` sobre la roca, se suma 2 elevado a la potencia de `p.player.indexPlayer`. Esto crea una máscara de bits o un identificador único para la combinación de jugadores en la roca.
    -   `public int ManyPlayerOn()`: Retorna el número total de `PlayerToken`s que se encuentran actualmente sobre esta roca.

-   **Lógica Clave:**
    La inicialización de las rocas depende en gran medida de su `PlayZone` padre para su posicionamiento y tamaño. El control visual del brillo (`shiny`) es dinámico y se sincroniza con el estado global de combate a través de `Combatjudge`, permitiendo que las rocas sean interactivas solo en momentos apropiados. La gestión de `PlayerToken`s se realiza mediante la adición y eliminación de elementos en un array dinámico, que aunque funcional, implica la recreación de arrays para cada operación, lo que podría ser una consideración de rendimiento para un gran número de operaciones. Los métodos para obtener vecinos y la información de los jugadores que ocupan la roca son esenciales para la lógica de movimiento y las reglas del juego.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere que el GameObject al que está adjunto tenga un `SpriteRenderer` y un `Collider` (para `OnMouseDown`), y que su primer hijo (`transform.GetChild(0)`) también tenga un `SpriteRenderer`. También espera la presencia de un `PlayZone` como padre.
-   **Eventos (Entrada):**
    -   `void Start()`: Se inicializa al inicio de la escena o cuando el GameObject se activa.
    -   `void Update()`: Se ejecuta en cada frame para actualizar el estado visual.
    -   `private void OnMouseDown()`: Responde a la interacción del usuario (clic del ratón) si la roca es clicable.
-   **Eventos (Salida):**
    -   Este script invoca el método `MoveToRock(this)` en el `Combatjudge` principal (`FindFirstObjectByType<Combatjudge>()`) cuando el jugador hace clic en una roca brillante. Esto sirve como una señal para el sistema de combate de que se ha seleccionado una roca como destino para un movimiento.
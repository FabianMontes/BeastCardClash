# `RockBehavior.cs`

## 1. Propósito General
Este script `RockBehavior` es fundamental para gestionar el comportamiento y la representación visual de cada "roca" individual en el tablero de juego de Beast Card Clash. Controla su posicionamiento inicial, su estado visual (como el brillo para indicar seleccionabilidad) y lleva un registro de los `PlayerToken`s que se encuentran actualmente sobre ella, interactuando directamente con el sistema `Combatjudge` para la lógica central del juego.

## 2. Componentes Clave

### `Inscription` (Enum)
El `Inscription` enum define los diferentes tipos o "inscripciones" que una `RockBehavior` puede poseer. Estas inscripciones probablemente representan afinidades elementales (fuego, tierra, agua, aire) o mecánicas de juego específicas (duelo, pick), además de un estado `empty` por defecto. Cada valor numérico se asocia directamente con un índice en el array de `Sprite`s para la representación visual.

### `RockBehavior` (Clase)
`RockBehavior` es una clase que hereda de `MonoBehaviour`, lo que significa que se adjunta a GameObjects en la escena de Unity y participa en su ciclo de vida. Está configurada para ejecutarse muy temprano en el orden de ejecución de scripts (`DefaultExecutionOrder(-2)`), asegurando que su inicialización ocurra antes que la mayoría de los demás scripts.

*   **Descripción:** Esta clase es responsable de controlar las entidades de roca individuales en el juego. Se encarga de su apariencia visual, su posición y rotación dentro de una `PlayZone` (la zona de juego principal), y gestiona una colección de `PlayerToken`s que actualmente ocupan la roca. Facilita la interacción del jugador a través de clics y comunica eventos clave al sistema `Combatjudge`.

*   **Variables Públicas / Serializadas:**
    *   `public PlayZone father;`: Una referencia al objeto `PlayZone` padre. Este `PlayZone` es crucial, ya que define el diseño general (ej. un círculo) y las propiedades (radio, escala) dentro de las cuales se posicionan estas rocas.
    *   `[SerializeField] private Sprite[] sprite;`: Un array de objetos `Sprite` que son asignados desde el Inspector de Unity. Cada sprite en este array corresponde a un tipo de `Inscription` y se utiliza para mostrar visualmente la característica elemental o funcional de la roca.
    *   `[SerializeField] public Inscription inscription = Inscription.empty;`: La inscripción específica o tipo elemental de esta roca, configurable en el Inspector. Por defecto, su valor es `Inscription.empty`.
    *   `public Vector3 direction = Vector3.forward;`: Utilizado en la fase de inicialización para calcular la posición de la roca de forma radial desde su `father`.
    *   `public float angle = 0;`: Un ángulo de rotación inicial, también usado durante la configuración para orientar la roca dentro de la `PlayZone`.
    *   `public int numbchild = 0;`: Un índice que representa la posición de esta roca dentro de la colección de rocas de su `father`. Se usa para el cálculo de vecinos y la disposición.
    *   `public bool shiny = false;`: Una bandera booleana que controla si la roca debe tener un efecto visual de "brillo" (cambiar a amarillo). Este estado se utiliza para indicar que la roca es actualmente seleccionable por el jugador.

*   **Métodos Principales:**
    *   `void Start()`: Este método del ciclo de vida de Unity se llama una vez cuando el script se activa por primera vez. Se encarga de inicializar las referencias a los `SpriteRenderer`s de la roca (uno para la roca misma, `itself`, y otro para su símbolo de inscripción, `simbol`). Configura el sprite del símbolo basándose en el valor de `inscription`. Si tiene un `father` asignado, calcula y aplica la posición, escala y rotación de la roca para que se ajuste a un diseño radial alrededor del `PlayZone` padre, utilizando propiedades como `father.radius` y `father.RockScale`.
    *   `void Update()`: Se llama una vez por cada frame del juego. En cada frame, verifica el estado actual del juego a través de `Combatjudge.combatjudge.GetSetMoments()`. Si el juego no está en el momento `SetMoments.GlowRock`, la bandera `shiny` se establece en `false`. Luego, ajusta el color del `SpriteRenderer` principal de la roca (`itself.color`) a `Color.yellow` si `shiny` es verdadero, o a `Color.black` si es falso, creando un efecto de brillo dinámico.
    *   `private void OnMouseDown()`: Este método de evento de Unity se invoca cuando el usuario hace clic o toca el Collider del GameObject al que está adjunto este script. Si la roca está "brillante" (`shiny` es verdadero), llama al método `MoveToRock` del `Combatjudge` (localizándolo en la escena), pasando esta misma roca como argumento. Esto permite que el jugador interactúe y mueva sus tokens a rocas que están activas y seleccionables.
    *   `public RockBehavior[] getNeighbor(int al)`: Este método calcula y devuelve un array que contiene dos instancias de `RockBehavior` que son "vecinas" a la roca actual, basándose en un desplazamiento `al` (distancia a lo largo del círculo). Utiliza el `numbchild` de la roca actual y la cantidad total de rocas (`father.many`) para calcular los índices de los vecinos en ambos lados, manejando el "envoltorio" si se llega al final o al principio del círculo.
    *   `public void AddPlayer(PlayerToken token)`: Añade un `PlayerToken` a la lista interna `playersOn`, que rastrea todos los jugadores que están ocupando esta roca. Gestiona la inicialización de la lista si es nula y redimensiona dinámicamente el array para acomodar al nuevo jugador.
    *   `public void RemovePlayer(PlayerToken token)`: Elimina un `PlayerToken` específico de la lista `playersOn`. Busca y elimina todas las instancias del token (aunque típicamente solo debería haber una) y luego reconstruye el array sin los tokens eliminados, reduciendo su tamaño.
    *   `public bool manyOn()`: Devuelve `true` si hay más de un `PlayerToken` presente en esta roca, lo que podría indicar una condición para un duelo o un evento especial.
    *   `public int GetPlayersOn()`: Calcula y devuelve un valor entero único basado en los jugadores actualmente en la roca. Para cada `PlayerToken`, calcula `2` elevado a la potencia de `p.player.indexFigther` y suma estos valores. Esto crea una especie de máscara de bits o identificador combinado que representa la composición de jugadores en la roca.
    *   `public int ManyPlayerOn()`: Devuelve el número total de `PlayerToken`s que están actualmente sobre esta roca. Es una cuenta directa de la longitud del array `playersOn`.

*   **Lógica Clave:**
    *   **Configuración Posicional:** La clase utiliza su referencia `father` (una `PlayZone`) junto con sus propiedades `direction`, `angle` y `numbchild` para posicionarse y rotarse automáticamente de forma correcta dentro de un diseño de tablero circular o radial, ajustándose a la escala definida por `father.RockScale`.
    *   **Sistema de Brillo e Interacción:** La roca puede entrar en un estado de "brillo" (cambiando a amarillo) cuando la bandera `shiny` es verdadera. Este estado es controlado dinámicamente por el `Combatjudge` basándose en el momento actual del juego (`SetMoments.GlowRock`). Cuando la roca está brillante, es reactiva a los clics del jugador a través de `OnMouseDown`, lo que le permite al `Combatjudge` procesar el movimiento del jugador hacia esa roca.
    *   **Gestión de Presencia de Jugadores:** El script mantiene una colección dinámica (`playersOn`) de `PlayerToken`s que se encuentran físicamente sobre la roca. Los métodos `AddPlayer` y `RemovePlayer` se encargan de la gestión de esta colección, mientras que `manyOn`, `GetPlayersOn` y `ManyPlayerOn` proporcionan distintas formas de consultar la presencia y composición de los jugadores en la roca.

## 3. Dependencias y Eventos

*   **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, para su correcto funcionamiento visual e interactivo, el GameObject al que se adjunta `RockBehavior` debe tener al menos un componente `SpriteRenderer` (para la roca principal) y otro `SpriteRenderer` en su primer objeto hijo (para el símbolo de la inscripción). Además, requiere un componente `Collider` (o `Collider2D`) para detectar las interacciones del ratón (e.g., `OnMouseDown`).

*   **Eventos (Entrada):**
    El script se suscribe implícitamente al evento de clic del ratón `OnMouseDown` proporcionado por el motor Unity. Este es el principal punto de entrada para la interacción del jugador con las rocas.

*   **Eventos (Salida):**
    Este script no invoca explícitamente `UnityEvent`s o `Action`s para notificar a otros sistemas de forma genérica. Su interacción con el `Combatjudge` se realiza directamente a través de una llamada a método (`FindFirstObjectByType<Combatjudge>().MoveToRock(this)`), lo que indica una dependencia directa con ese sistema.
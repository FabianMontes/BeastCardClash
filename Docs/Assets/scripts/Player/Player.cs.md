# `Player.cs`

## 1. Propósito General
Este script gestiona el movimiento principal del jugador dentro del juego. Su rol fundamental es hacer que el GameObject al que está adjunto (que representa al jugador) siga continuamente la posición de un `Transform` objetivo específico, utilizando el sistema de navegación `NavMesh` de Unity.

## 2. Componentes Clave

### Player
- **Descripción:** La clase `Player` hereda de `MonoBehaviour`, lo que significa que puede adjuntarse a un GameObject en la jerarquía de Unity. Su función principal es controlar el desplazamiento del jugador, dirigiéndolo automáticamente hacia un punto designado en el mundo del juego.
- **Variables Públicas / Serializadas:**
    - `[SerializeField] Transform target;`: Un objeto `Transform` que define el punto al que el jugador debe moverse. Esta variable es serializada y visible en el Inspector de Unity, permitiendo asignar el objetivo desde el editor.
    - `NavMeshAgent agent;`: Una referencia al componente `NavMeshAgent` adjunto al mismo GameObject. `NavMeshAgent` es crucial para permitir que el jugador navegue por el entorno, evitando obstáculos y encontrando el camino.
- **Métodos Principales:**
    - `void Start()`: Este método es parte del ciclo de vida de Unity y se ejecuta una vez al inicio del script.
        - Se encarga de obtener el componente `NavMeshAgent` del GameObject actual utilizando `TryGetComponent<NavMeshAgent>(out agent)`. Esta es una forma segura de obtener el componente, ya que no lanzará un error si el componente no está presente. Si el componente no se encuentra, el script no podrá controlar el movimiento del jugador.
    - `void Update()`: Este método es parte del ciclo de vida de Unity y se ejecuta en cada frame del juego.
        - `agent.SetDestination(target.position);`: En cada frame, se actualiza el destino del `NavMeshAgent` a la posición actual del `target`. Esto provoca que el jugador siga de manera continua y fluida el objetivo asignado.
- **Lógica Clave:**
    La lógica central del script es bastante directa: al inicio, el script busca un componente `NavMeshAgent` en el GameObject. Luego, en cada fotograma del juego, le indica a ese `NavMeshAgent` que calcule y siga una ruta hacia la posición actual del `Transform` definido como `target`. Esto establece un comportamiento de "seguir al objetivo" para el jugador.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Aunque el script no utiliza el atributo `[RequireComponent]`, la funcionalidad del `Player` depende críticamente de la existencia de un componente `NavMeshAgent` en el mismo GameObject. Sin él, el jugador no podrá navegar ni moverse.
- **Eventos (Entrada):**
    Este script no se suscribe directamente a eventos personalizados (`UnityEvent` o `Action`). Su funcionamiento se basa en los métodos de ciclo de vida de Unity: `Start` (para inicialización) y `Update` (para la lógica de movimiento constante).
- **Eventos (Salida):**
    Este script no invoca ningún evento personalizado para notificar a otros sistemas sobre el estado o acciones del jugador. Se enfoca exclusivamente en la lógica de movimiento.
# `Player.cs`

## 1. Propósito General
Este script de Unity es responsable de gestionar el movimiento automático de un `GameObject` (presumiblemente el jugador o una entidad controlada por IA) a lo largo de un `NavMesh`. Su rol principal es dirigir un `NavMeshAgent` hacia una posición de destino predefinida que se configura desde el Inspector de Unity.

## 2. Componentes Clave

### `Player`
- **Descripción:** Esta clase hereda de `MonoBehaviour`, lo que permite adjuntarla a cualquier `GameObject` en una escena de Unity. Su función es habilitar el movimiento autónomo del `GameObject` al que está adjunta, utilizando el sistema de navegación de Unity (`NavMesh`). El script asume que el `GameObject` ya tiene un componente `NavMeshAgent` configurado.
- **Variables Públicas / Serializadas:**
    - `[SerializeField] Transform target;`: Una referencia a un componente `Transform` que define la posición de destino a la que el `NavMeshAgent` debe moverse. Al ser `[SerializeField]`, esta variable puede ser asignada y modificada directamente desde el Inspector de Unity, facilitando la configuración del objetivo de movimiento en el editor.
    - `NavMeshAgent agent;`: Una referencia privada al componente `NavMeshAgent` que debe estar adjunto al mismo `GameObject` que este script. Este componente es fundamental para calcular rutas y ejecutar el movimiento a través del `NavMesh`.
- **Métodos Principales:**
    - `void Start()`: Este método es parte del ciclo de vida de Unity y se llama una vez al inicio, antes de la primera actualización del frame.
        ```csharp
        void Start()
        {
            TryGetComponent<NavMeshAgent>(out agent);
        }
        ```
        Su propósito es obtener una referencia al componente `NavMeshAgent` adjunto al `GameObject` actual. Utiliza `TryGetComponent` para intentar obtener el componente de forma segura, evitando errores si no se encuentra.
    - `void Update()`: Este método se invoca una vez por cada frame del juego. Es donde se ejecuta la lógica de movimiento continua.
        ```csharp
        void Update()
        {
            agent.SetDestination(target.position);
        }
        ```
        En cada frame, este método actualiza el destino del `NavMeshAgent` a la posición actual del `target` configurado. Esto hace que el `GameObject` con el `NavMeshAgent` se mueva constantemente hacia la posición del `target`.
- **Lógica Clave:**
    La lógica central del script reside en el método `Update`. Una vez inicializado el `NavMeshAgent` en `Start`, el método `Update` se encarga de llamar repetidamente a `agent.SetDestination(target.position)`. Esto provoca que el `NavMeshAgent` recalcule y siga una ruta hacia la posición actual del `target` en cada frame. Este patrón es comúnmente utilizado para implementar comportamientos de seguimiento continuo o movimiento dirigido hacia un objetivo dinámico.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Aunque no se utiliza un atributo `[RequireComponent]` explícito, este script depende fundamentalmente de la presencia de un componente `NavMeshAgent` en el mismo `GameObject` al que está adjunto. Si este componente no está presente, el script no podrá inicializar su referencia `agent` y, por lo tanto, no funcionará.
- **Eventos (Entrada):** Este script no se suscribe a eventos de entrada del usuario (como clics de ratón o pulsaciones de teclado) ni a eventos externos de UI. Su comportamiento es autónomo y se basa únicamente en el ciclo de vida de `MonoBehaviour`.
- **Eventos (Salida):** Este script no emite ni invoca ningún tipo de evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas o scripts sobre su estado o acciones. Su función es puramente de control de movimiento interno del `GameObject`.
# `Player.cs`

## 1. Propósito General
Este script principal gestiona el movimiento de un objeto de juego ("Player") dentro del entorno 3D de Unity. Su función es hacer que el jugador siga continuamente un `Transform` objetivo predefinido, utilizando el sistema de navegación de Unity (`NavMeshAgent`).

## 2. Componentes Clave

### `Player`
-   **Descripción:** La clase `Player` es un `MonoBehaviour` que se encarga de la lógica de movimiento automático de la entidad del jugador. Se adhiere a un GameObject y lo dota de la capacidad de navegar un `NavMesh` para perseguir un objetivo.
-   **Variables Públicas / Serializadas:**
    -   `[SerializeField] Transform target`: Esta variable, visible en el Inspector de Unity, es el `Transform` del GameObject que el jugador debe seguir. Es crucial para definir el punto de destino constante del movimiento del jugador.
    -   `NavMeshAgent agent`: Una variable privada que almacena una referencia al componente `NavMeshAgent` adjunto al mismo GameObject que este script. Este componente es esencial para que el jugador pueda interactuar con el sistema de navegación de Unity y moverse por el `NavMesh`.
-   **Métodos Principales:**
    -   `void Start()`: Este método del ciclo de vida de Unity se ejecuta una vez al inicio del ciclo de vida del script, después de que el objeto es instanciado. Su propósito es inicializar la referencia al componente `NavMeshAgent` del GameObject.
        ```csharp
        void Start()
        {
            TryGetComponent<NavMeshAgent>(out agent);
        }
        ```
        Utiliza `TryGetComponent` para obtener de forma segura el `NavMeshAgent`, lo que evita errores si el componente no está presente, aunque su ausencia impediría el movimiento.
    -   `void Update()`: Este método se invoca una vez por cada frame del juego. Es responsable de la lógica de movimiento continua del jugador. En cada frame, instruye al `NavMeshAgent` para que establezca su destino en la posición actual del `target`.
        ```csharp
        void Update()
        {
            agent.SetDestination(target.position); 
        }
        ```
        Esto asegura que el jugador siempre se esté moviendo hacia la ubicación más reciente del `target`.
-   **Lógica Clave:**
    La lógica central de este script reside en su ciclo de `Update`. Una vez que se ha inicializado la referencia al `NavMeshAgent` en `Start`, el método `Update` llama repetidamente a `agent.SetDestination(target.position)`. Esto hace que el `NavMeshAgent` recalcule y ejecute una ruta hacia la posición actual del `target` en cada frame, logrando un comportamiento de "persecución" constante. La efectividad de esta lógica depende de la presencia de un `NavMeshAgent` en el GameObject y un `NavMesh` en la escena.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Aunque no se utiliza el atributo `[RequireComponent]`, este script depende fundamentalmente de la presencia de un componente `NavMeshAgent` en el mismo GameObject para funcionar correctamente. Sin él, el movimiento no será posible.
-   **Eventos (Entrada):** Este script no se suscribe explícitamente a ningún evento (como clics de botón o colisiones) de otros sistemas. Su comportamiento se basa únicamente en el ciclo de vida de Unity (`Start`, `Update`) y la configuración de sus variables serializadas.
-   **Eventos (Salida):** Este script no invoca ni emite ningún evento (como `UnityEvent` o `Action`) para notificar a otros sistemas sobre su estado o acciones.
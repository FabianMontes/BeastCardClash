# `WorldPlayer.cs`

## 1. Propósito General
Este script gestiona el movimiento autónomo de un objeto en el mundo del juego, haciendo que siga constantemente la posición de un objetivo predefinido. Su rol principal es utilizar el sistema de navegación (NavMesh) de Unity para desplazar una entidad de forma inteligente a través del escenario.

## 2. Componentes Clave

### `WorldPlayer`
- **Descripción:** Es una clase `MonoBehaviour` que se adjunta a un objeto de juego y lo habilita para la navegación automática. Utiliza el componente `NavMeshAgent` para calcular rutas y mover el objeto hacia una posición objetivo.

- **Variables Públicas / Serializadas:**
    - `NavMeshAgent agent`: Esta variable privada almacena una referencia al componente `NavMeshAgent` que debe estar adjunto al mismo GameObject. Es esencial para controlar las capacidades de navegación del objeto.
    - `[SerializeField] Transform target`: Esta es una variable serializada que permite asignar un objeto `Transform` (normalmente de otro GameObject) desde el Inspector de Unity. El `WorldPlayer` se moverá continuamente hacia la posición actual de este `target`.

- **Métodos Principales:**
    - `void Start()`:
        - **Descripción:** Este es un método del ciclo de vida de Unity, llamado una vez al iniciar el script.
        - **Acción:** Su función principal es intentar obtener una referencia al componente `NavMeshAgent` adjunto al mismo GameObject.
        - **Fragmento de código:**
        ```csharp
        TryGetComponent<NavMeshAgent>(out agent);
        ```
        Si el componente `NavMeshAgent` no está presente, la variable `agent` permanecerá nula, lo que podría causar errores en tiempo de ejecución si no se maneja adecuadamente.

    - `void Update()`:
        - **Descripción:** Este es un método del ciclo de vida de Unity, llamado una vez por cada frame del juego.
        - **Acción:** En cada frame, establece la posición actual del `target` como el nuevo destino para el `NavMeshAgent`. Esto asegura que el objeto siga al `target` continuamente.
        - **Fragmento de código:**
        ```csharp
        agent.SetDestination(target.position);
        ```

- **Lógica Clave:**
La lógica principal del script se centra en la "persecución" continua de un objetivo. En el método `Start`, el script se prepara obteniendo su componente de navegación. Luego, en cada fotograma (`Update`), le indica al `NavMeshAgent` que se dirija hacia la posición actual del `target` asignado. El `NavMeshAgent` se encarga internamente de calcular la ruta más eficiente y de mover el GameObject, evitando obstáculos en el `NavMesh` pre-cocinado del escenario.

## 3. Dependencias y Eventos

- **Componentes Requeridos:**
    Este script depende directamente de la presencia de un componente `NavMeshAgent` en el mismo GameObject. Aunque no se utiliza `[RequireComponent]` explícitamente, el script no funcionará correctamente sin él, ya que intenta acceder a este componente en `Start`.

- **Eventos (Entrada):**
    Este script no se suscribe a eventos externos (como eventos de UI, colisiones o eventos personalizados). Su comportamiento está impulsado únicamente por los métodos de ciclo de vida de Unity (`Start` y `Update`) y la posición cambiante del `target`.

- **Eventos (Salida):**
    Este script no invoca ningún evento (`UnityEvent`, `Action`, etc.) ni notifica a otros sistemas sobre su estado o acciones. Su función es puramente de control de movimiento interno.
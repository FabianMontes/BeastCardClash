# `Player.cs`

## 1. Propósito General
Este script `Player.cs` es un componente fundamental para el movimiento básico de una entidad "jugador" o controlable dentro del juego. Gestiona la navegación autónoma de su GameObject hacia un `target` predefinido, utilizando el sistema de NavMesh de Unity. Su rol principal es permitir que la entidad se mueva automáticamente hacia una posición específica.

## 2. Componentes Clave

### `Player`
- **Descripción:** La clase `Player` hereda de `MonoBehaviour`, lo que significa que puede ser adjuntada a cualquier GameObject en una escena de Unity. Su función es habilitar el movimiento autónomo del GameObject al que está unido, dirigiéndolo continuamente hacia una posición de destino (`target`) utilizando un `NavMeshAgent`.
- **Variables Públicas / Serializadas:**
    - `target` (Tipo: `Transform`): Esta variable, marcada con `[SerializeField]`, permite que se le asigne un `Transform` (la posición y rotación de cualquier GameObject en la escena) directamente desde el Inspector de Unity. Representa el punto o el objeto al que el "jugador" debe navegar.
    - `agent` (Tipo: `NavMeshAgent`): Esta variable privada almacena una referencia al componente `NavMeshAgent` que debe estar presente en el mismo GameObject. Es esencial para que el script pueda controlar la navegación.
- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se llama una vez al inicio del juego, antes de la primera actualización del frame. Su propósito es inicializar la referencia al componente `NavMeshAgent`. Utiliza `TryGetComponent<NavMeshAgent>(out agent)` para intentar obtener el componente de forma segura. Si el componente existe en el GameObject, su referencia se asigna a la variable `agent`.
    - `void Update()`: Este método del ciclo de vida de Unity se llama una vez por cada frame del juego. En cada llamada, su función es actualizar constantemente el destino del `NavMeshAgent` a la posición actual del `target`. Esto provoca que el GameObject se mueva continuamente hacia el `target` especificado.
        ```csharp
        agent.SetDestination(target.position);
        ```
- **Lógica Clave:** La lógica central de este script es un comportamiento de "seguir objetivo" continuo. Una vez que se ha obtenido la referencia al `NavMeshAgent` en `Start`, cada frame en `Update` se instruye al `NavMeshAgent` para que recalcule su ruta y se mueva hacia la posición actual del `target`. Esto depende de que la escena tenga un NavMesh horneado y de que el `NavMeshAgent` esté configurado correctamente para navegar sobre él.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Aunque no se utiliza explícitamente el atributo `[RequireComponent(typeof(NavMeshAgent))]`, este script depende fundamentalmente de la presencia de un componente `NavMeshAgent` en el mismo GameObject para funcionar. Sin un `NavMeshAgent`, la variable `agent` permanecería nula, y cualquier intento de llamar a `agent.SetDestination()` resultaría en un error `NullReferenceException`.
- **Eventos (Entrada):** Este script no se suscribe a eventos externos (como eventos de UI, acciones del jugador o eventos personalizados). Su ejecución está impulsada exclusivamente por los métodos de ciclo de vida de Unity (`Start` y `Update`).
- **Eventos (Salida):** El script `Player.cs` no invoca ni emite ningún evento (como `UnityEvent` o `Action`) para notificar a otros sistemas del juego sobre su estado o acciones. Su función es puramente interna para la gestión del movimiento.
# Player
Este script, `Player.cs`, es una pieza fundamental para la gestión del movimiento de una entidad controlada por el jugador dentro del entorno del juego. Su función principal es facilitar el desplazamiento autónomo y la navegación de la entidad a través de un `NavMesh`, dirigiéndola continuamente hacia un punto de interés definido por el jugador.

El script integra dos componentes clave de Unity: un `Transform` (`target`) que representa el destino deseado por el jugador, y un `NavMeshAgent` que utiliza la información del `target` para calcular y ejecutar la ruta de movimiento, evitando obstáculos de forma inteligente. En cada ciclo de actualización del juego (fotograma), `Player.cs` se asegura de que la entidad asignada siga de cerca la posición actual del `target`, lo que permite un control de movimiento fluido y reactivo.

# Métodos

## Métodos de Unity

### Start
El método `Start` se ejecuta una única vez al comienzo de la vida del script, antes de la primera actualización del fotograma. Su propósito es inicializar las referencias a los componentes necesarios para el correcto funcionamiento del script.

```csharp
void Start()
{
    TryGetComponent<NavMeshAgent>(out agent);
}
```

En esta implementación, `Start` intenta obtener una referencia al componente `NavMeshAgent` que se espera esté adjunto al mismo GameObject donde reside el script `Player`. La referencia obtenida se almacena en la variable privada `agent`. Es crucial que un `NavMeshAgent` esté presente en el GameObject para que el script pueda controlar el movimiento; de lo contrario, la variable `agent` podría ser `null`, lo que resultaría en errores en tiempo de ejecución al intentar interactuar con ella.

### Update
El método `Update` se invoca en cada fotograma del juego, lo que lo convierte en el lugar ideal para la lógica de movimiento continuo.

```csharp
void Update()
{
    agent.SetDestination(target.position);
}
```

Dentro de `Update`, el script instruye al `NavMeshAgent` (`agent`) para que establezca su destino. La posición de destino se obtiene directamente del componente `Transform` asignado a la variable `target` (`target.position`). Esto significa que, mientras el juego se esté ejecutando, la entidad que utiliza este script se moverá constantemente hacia la posición actual del objeto `target`. Este patrón permite que el jugador manipule el `target` (por ejemplo, con un clic en el terreno o mediante un control de interfaz de usuario), y la entidad del `Player` se encargará de seguirlo automáticamente a través del camino generado por el NavMesh.

## Otros métodos
No hay métodos personalizados definidos en este script más allá de los métodos del ciclo de vida de Unity.

## Getters y Setters
Este script no define métodos o propiedades públicas explícitas para obtener o establecer valores. Sin embargo, gestiona un parámetro importante que se configura directamente desde el Inspector de Unity para influir en su comportamiento:

1. `target` (Transform): Establece el objeto `Transform` que el `NavMeshAgent` del jugador seguirá como destino. Esta referencia se configura directamente en el Inspector de Unity, permitiendo al desarrollador asignar el punto hacia el cual la entidad controlada por este script se moverá.
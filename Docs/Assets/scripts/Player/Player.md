# `Player`
El script `Player`, que hereda de `MonoBehaviour`, es un componente fundamental para gestionar el movimiento autónomo de un objeto en el entorno de juego de Beast Card Clash utilizando el sistema de navegación NavMesh de Unity. Su función principal es permitir que la entidad a la que está adjunto se desplace de manera eficiente hacia un punto de destino predefinido, lo que podría representar el movimiento de un avatar del jugador en una interfaz de lobby, un tablero de juego o una unidad específica durante su turno.

El script establece una dependencia clave con un componente `NavMeshAgent` (que debe estar presente en el mismo `GameObject`) y una referencia a un `Transform` llamado `target`. Este `target` actúa como el punto de destino, permitiendo que la lógica externa del juego dicte hacia dónde debe moverse el objeto controlado por este script. La simplicidad del script radica en su enfoque en el "cómo" se mueve, dejando el "cuándo" y el "dónde" a otros componentes, lo que lo hace flexible para diferentes mecánicas de movimiento dentro de un juego de estrategia por turnos.

# Métodos

## Métodos de Unity

### `Start()`
Este método se invoca una vez al inicio del ciclo de vida del script, antes de la primera actualización del frame. Su propósito es inicializar las referencias necesarias para el funcionamiento del script.

```csharp
void Start()
{
    TryGetComponent<NavMeshAgent>(out agent);
}
```

En concreto, `Start()` intenta obtener una referencia al componente `NavMeshAgent` que debe estar adjunto al mismo `GameObject` que este script `Player`. Si el componente se encuentra, la referencia se almacena en la variable privada `agent`. Este paso es crítico, ya que `NavMeshAgent` es el motor que permite la navegación y el cálculo de rutas dentro del entorno de juego.

### `Update()`
El método `Update()` se ejecuta en cada frame del juego y es el encargado de mantener el comportamiento de movimiento continuo del objeto.

```csharp
void Update()
{
    agent.SetDestination(target.position);
}
```

Dentro de este método, se instruye al `NavMeshAgent` (`agent`) para que establezca su destino (`SetDestination`) a la posición actual del `Transform` referenciado por `target`. Esto significa que, en cada frame, el `NavMeshAgent` recalcula su ruta (si es necesario) y guía al objeto hacia la posición más reciente del `target`. Este patrón es ideal para un seguimiento dinámico, donde el `target` puede moverse o ser actualizado por otras lógicas del juego (por ejemplo, al hacer clic en una ubicación en el mapa o seleccionar una casilla de destino para una unidad).

## Otros métodos
Este script no contiene métodos definidos por el desarrollador aparte de los métodos de ciclo de vida de Unity.

## Getters y Setters

1.  `target`: Este campo es un `Transform` que define el objetivo al cual el jugador intentará moverse. Está marcado con `[SerializeField]`, lo que permite que sea asignado directamente desde el Inspector de Unity, actuando como un 'setter' visual y facilitando la configuración inicial del objetivo sin necesidad de un método público explícito. Su valor es la posición a la que el `NavMeshAgent` se dirigirá constantemente.
2.  `agent`: Esta variable es una referencia al componente `NavMeshAgent`. Se obtiene internamente en el método `Start()` utilizando `TryGetComponent<NavMeshAgent>(out agent);`. No está expuesta públicamente como un getter o setter, ya que su gestión es interna del script para controlar el movimiento.
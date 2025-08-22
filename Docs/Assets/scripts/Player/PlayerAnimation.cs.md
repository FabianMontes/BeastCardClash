# `PlayerAnimation.cs`

## 1. Propósito General
Este script `PlayerAnimation` es responsable de gestionar la apariencia y las animaciones de un personaje jugador, permitiendo cambiar su modelo visual (especie de animal) y actualizar su animación en función de la velocidad de movimiento. Interactúa principalmente con un componente `NavMeshAgent` para obtener datos de velocidad y con un componente `MeshAnimation` (asumido para manejar las animaciones específicas del modelo) para actualizar el estado visual del personaje.

## 2. Componentes Clave

### `SpieceEnum`
- **Descripción:** Un `enum` simple que define las diferentes especies de animales disponibles para los personajes en el juego. Cada miembro del `enum` corresponde a un tipo de animal específico: `bear` (oso), `frog` (rana), `condor` (cóndor), `chamaleon` (camaleón). Se utiliza como índice para seleccionar prefabs de una lista.

### `PlayerAnimation`
- **Descripción:** Esta clase hereda de `MonoBehaviour` y se encarga de instanciar y controlar el modelo 3D del personaje del jugador, así como de sincronizar sus animaciones con el movimiento. Actúa como un puente entre la lógica de navegación (velocidad del `NavMeshAgent`) y el sistema de animación del modelo 3D.

- **Variables Públicas / Serializadas:**
    - `Prefabs` (`List<GameObject>`): Una lista de prefabs de `GameObject` que representan los diferentes modelos 3D de las especies de animales. El índice de esta lista se correlaciona con los valores del `SpieceEnum`.
    - `SpieceEnum` (`SpieceEnum`): Define la especie de animal actual que este `PlayerAnimation` debe representar. Se utiliza para seleccionar el prefab adecuado de la lista `Prefabs`.
    - `Agent` (`NavMeshAgent`): Una referencia al componente `NavMeshAgent` del personaje. Se utiliza para obtener la velocidad actual del personaje y así poder actualizar la animación de movimiento.

- **Métodos Principales:**
    - `void Start()`:
        Este método se invoca una vez al inicio del ciclo de vida del script. Su propósito principal es instanciar el modelo 3D del animal seleccionado y configurarlo como hijo del objeto que posee este script.
        ```csharp
        GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
        Children.transform.localScale = MeshScale;
        Children.transform.parent = transform;
        Children.transform.localPosition = MeshPosition;
        ```
        Además, busca el componente `MeshAnimation` dentro del prefab instanciado y lo inicializa, estableciendo la primera "skin" (apariencia visual) del modelo.

    - `void Update()`:
        Este método se llama una vez por cada frame. Su función es asegurar que la animación del personaje se actualice continuamente en función de su velocidad de movimiento.
        ```csharp
        if (MeshAnimate == null) return;
        string SpeedRatio = (Agent.velocity.magnitude / Agent.speed).ToString();
        MeshAnimate.UpdateAnimation("Speed", SpeedRatio);
        ```
        Calcula una relación de velocidad dividiendo la magnitud de la velocidad actual del `NavMeshAgent` por su velocidad máxima, y luego usa este valor para actualizar una animación general de "Velocidad" a través del componente `MeshAnimate`.

    - `void UpdateSpiece(SpieceEnum Spiece)`:
        Este método permite cambiar dinámicamente la especie de animal que representa el jugador.
        ```csharp
        if (Spiece == SpieceEnum) return;
        SpieceEnum = Spiece;
        GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
        Children.transform.parent = transform;
        MeshAnimate = Children.GetComponent<MeshAnimation>();
        ```
        Si la nueva especie es diferente a la actual, actualiza la variable `SpieceEnum` e instancia un *nuevo* prefab del animal correspondiente. El nuevo modelo se configura como hijo del `GameObject` actual y se obtiene su componente `MeshAnimation` para futuras actualizaciones. Es importante notar que la implementación actual de este método *no destruye el modelo anterior*, lo que podría llevar a modelos duplicados si se llama repetidamente.

- **Lógica Clave:**
    La lógica central del script reside en la actualización continua de la animación de "velocidad" del personaje (`Update`), basándose en el movimiento detectado por el `NavMeshAgent`. Al iniciar el juego (`Start`) o al cambiar la especie del animal (`UpdateSpiece`), el script se encarga de instanciar el modelo 3D correcto y de enlazarlo con su sistema de animación (`MeshAnimation`). La posición y escala del modelo instanciado se ajustan para encajar correctamente como un hijo del `GameObject` principal.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere que el `GameObject` al que está adjunto (o un `GameObject` relacionado) tenga un componente `NavMeshAgent` asignado a la variable `Agent` para que la lógica de animación de velocidad funcione correctamente. También depende de la existencia de un componente `MeshAnimation` en los prefabs de animales instanciados.

- **Eventos (Entrada):**
    El script no se suscribe explícitamente a eventos de Unity (`UnityEvent` o `Action`). Su método `UpdateSpiece` es público y está diseñado para ser invocado externamente por otros scripts o sistemas en el juego cuando sea necesario cambiar la especie del animal del jugador.

- **Eventos (Salida):**
    Este script no invoca ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios o acciones realizadas. Se enfoca exclusivamente en la gestión interna de la animación y el modelo del jugador.
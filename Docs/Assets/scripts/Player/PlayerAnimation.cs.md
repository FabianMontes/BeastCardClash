# `PlayerAnimation.cs`

## 1. Propósito General
Este script de `MonoBehaviour` gestiona la representación visual y la animación del personaje animal del jugador en el juego. Se encarga de instanciar el modelo 3D correcto para la especie seleccionada y de actualizar sus animaciones basándose principalmente en la velocidad de movimiento del personaje, interactuando directamente con el sistema de navegación (`NavMeshAgent`) y un componente de animación de malla (`MeshAnimation`).

## 2. Componentes Clave

### `SpieceEnum`
- **Descripción:** Este `enum` define las diferentes especies de animales disponibles en el juego. Cada valor entero del `enum` corresponde a un índice dentro de la lista de prefabs de animales para facilitar la carga.
- **Valores:**
    - `bear`
    - `frog`
    - `condor`
    - `chamaleon`

### `PlayerAnimation`
- **Descripción:** Es la clase principal de este archivo, hereda de `MonoBehaviour`. Su función es actuar como un controlador para el modelo 3D del personaje, asegurando que se cargue la especie correcta, se posicione y escale adecuadamente, y que sus animaciones visuales reflejen el estado de movimiento del personaje.
- **Variables Públicas / Serializadas:**
    - `[SerializeField] private List<GameObject> Prefabs;`
        - **Uso:** Contiene una lista de los prefabs `GameObject` de cada modelo de animal disponible. El índice en esta lista se corresponde con los valores de `SpieceEnum`. Permite al diseñador asignar los modelos desde el Inspector de Unity.
    - `[SerializeField] private SpieceEnum SpieceEnum;`
        - **Uso:** Define la especie de animal que este `PlayerAnimation` debe cargar y controlar inicialmente. También se actualiza cuando la especie del animal cambia en tiempo de ejecución.
    - `[SerializeField] private NavMeshAgent Agent;`
        - **Uso:** Una referencia al componente `NavMeshAgent` del mismo `GameObject` al que está adjunto este script. Se utiliza para obtener la velocidad actual del personaje y así controlar la velocidad de las animaciones de movimiento.
- **Métodos Principales:**
    - `void Start()`:
        - **Descripción:** Este método se llama una vez al inicio del ciclo de vida del script. Su propósito es inicializar la representación visual del animal.
        - **Lógica:** Instancia el prefab del animal correspondiente a la `SpieceEnum` actual, lo ajusta en tamaño y posición localmente (a la mitad del tamaño y ligeramente hundido para que la base esté a nivel del suelo), lo establece como hijo del `GameObject` de este script, y obtiene una referencia a su componente `MeshAnimation`. Finalmente, configura una "piel" inicial y prepara una lista para futuras configuraciones de piel.
        ```csharp
        GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
        Children.transform.localScale = MeshScale;
        Children.transform.parent = transform;
        Children.transform.localPosition = MeshPosition;
        MeshAnimate = Children.GetComponent<MeshAnimation>();
        MeshAnimate.SetSkin(0); // Establece la primera piel
        ```
    - `void Update()`:
        - **Descripción:** Se llama una vez por fotograma. Es responsable de mantener la animación del animal sincronizada con su movimiento.
        - **Lógica:** Calcula un `SpeedRatio` (ratio de velocidad) dividiendo la magnitud de la velocidad actual del `NavMeshAgent` por su velocidad máxima. Este ratio se utiliza para controlar la velocidad de reproducción de la animación del animal. Si no hay un `MeshAnimate` (por ejemplo, si el prefab no se cargó correctamente), el método retorna para evitar errores.
        ```csharp
        string SpeedRatio = (Agent.velocity.magnitude / Agent.speed).ToString();
        MeshAnimate.UpdateAnimation("Speed", SpeedRatio);
        ```
    - `void UpdateSpiece(SpieceEnum Spiece)`:
        - **Descripción:** Este método permite cambiar la especie del animal que se está representando en tiempo de ejecución.
        - **Parámetros:** `SpieceEnum Spiece`: La nueva especie de animal a la que se desea cambiar.
        - **Lógica:** Primero, verifica si la nueva especie es diferente de la actual para evitar instanciaciones innecesarias. Si es diferente, actualiza la `SpieceEnum` interna, instancia el prefab de la nueva especie, lo establece como hijo y obtiene su componente `MeshAnimation`. Luego, aplica una "piel" específica almacenada en `SpieceList` para esa especie.
        - **Nota:** Actualmente, este método instancia un nuevo modelo sin destruir el anterior. Esto podría resultar en múltiples modelos superpuestos si se llama repetidamente, y la nueva instancia no hereda la escala ni la posición local de la misma manera que en `Start()`. Esto es un punto para futura revisión.
        ```csharp
        SpieceEnum = Spiece;
        GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
        Children.transform.parent = transform;
        MeshAnimate = Children.GetComponent<MeshAnimation>();
        MeshAnimate.SetSkin(SpieceList[(int)SpieceEnum]);
        ```
- **Lógica Clave:**
    La lógica principal se centra en la gestión del ciclo de vida del modelo de animal. En `Start`, se inicializa el modelo, y en `Update`, su animación se sincroniza con el movimiento. El método `UpdateSpiece` permite una capacidad de "hot-swap" para la especie del animal, aunque con la salvedad mencionada de no destruir la instancia anterior y de no aplicar consistentemente el escalado y la posición local definidos en `Start()`. El script asume la existencia de un componente `MeshAnimation` en los prefabs de animales, el cual es el encargado de interpretar los comandos de animación.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Aunque no se utiliza explícitamente el atributo `[RequireComponent]`, este script depende fundamentalmente de que un componente `NavMeshAgent` esté presente y asignado en el Inspector al campo `Agent` para su correcto funcionamiento en `Update()`. También asume que los prefabs de animales en la lista `Prefabs` tienen un componente `MeshAnimation` adjunto, el cual es crucial para la gestión de las animaciones.
- **Eventos (Entrada):** Este script no se suscribe directamente a eventos de Unity (`UnityEvent`) o C# `Action`. Sus métodos (`Start`, `Update`) son parte del ciclo de vida de Unity, y `UpdateSpiece` es un método público que puede ser invocado por otros scripts para cambiar la especie del animal.
- **Eventos (Salida):** Este script no invoca ningún evento (`UnityEvent` o `Action`) para notificar a otros sistemas sobre cambios o estados internos. Su rol es puramente de control visual y de animación.
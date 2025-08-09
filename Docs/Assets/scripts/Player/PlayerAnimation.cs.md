# `PlayerAnimation.cs`

## 1. Propósito General
Este script `PlayerAnimation` es el encargado de gestionar la representación visual del personaje del jugador, controlando qué animal se muestra y cómo se anima. Su rol principal es instanciar el modelo 3D del animal correcto y actualizar sus animaciones basándose en el movimiento del personaje, interactuando con el sistema de navegación (`NavMeshAgent`) y un componente de animación de malla (`MeshAnimation`).

## 2. Componentes Clave

### `SpieceEnum`
- **Descripción:** Esta es una enumeración (`enum`) que define los diferentes tipos de especies de animales que pueden ser representadas en el juego. Cada valor en este `enum` corresponde a un índice en la lista de prefabs de animales, permitiendo seleccionar la especie de manera clara y tipada.
    ```csharp
    public enum SpieceEnum
    {
        bear, frog, condor, chamaleon
    }
    ```

### `PlayerAnimation`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, controla la lógica de instanciación y animación del modelo 3D de un animal para un personaje. Se encarga de cargar el prefab del animal seleccionado, posicionarlo y escalarlo correctamente, y luego actualizar su animación de movimiento basándose en la velocidad del personaje controlada por un `NavMeshAgent`. También ofrece una función para cambiar la especie del animal dinámicamente durante el juego.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] private List<GameObject> Prefabs`: Una lista de objetos `GameObject` que actúa como un catálogo de los modelos 3D de los animales. El índice de cada prefab en esta lista corresponde a un valor en el `SpieceEnum`, permitiendo al script instanciar el modelo correcto.
    - `[SerializeField] private SpieceEnum SpieceEnum`: Define la especie del animal que este `PlayerAnimation` debe representar actualmente. Se configura en el Inspector de Unity.
    - `[SerializeField] private NavMeshAgent Agent`: Una referencia al componente `NavMeshAgent` del mismo `GameObject` al que está adjunto este script. Este se utiliza para obtener la velocidad actual del personaje, un dato crucial para controlar la animación de movimiento.
    - `private MeshAnimation MeshAnimate`: Una referencia al componente `MeshAnimation` que se encuentra en el prefab del animal instanciado. Se asume que este componente maneja la lógica de animación específica del modelo 3D (como la mezcla de animaciones o el cambio de "pieles").
    - `private List<int> SpieceList`: Una lista que parece estar destinada a almacenar el índice de "piel" (skin) actual para cada especie de animal. Aunque se inicializa en `Start` con cuatro ceros, su propósito completo no es evidente sin el contexto del componente `MeshAnimation`.
    - `private Vector3 MeshScale`: Un `Vector3` que define la escala a la que se instanciará el modelo del animal. Actualmente, lo ajusta a la mitad de su tamaño original (`0.5f, 0.5f, 0.5f`).
    - `private Vector3 MeshPosition`: Un `Vector3` que define un desplazamiento de posición local para el modelo del animal una vez instanciado. Está configurado para mover el modelo 1 unidad hacia abajo (`0, -1, 0`), probablemente para alinear su base con el punto de pivote del `GameObject` padre.

- **Métodos Principales:**
    - `void Start()`:
        - **Descripción:** Este método se llama una vez al inicio del ciclo de vida del script. Su función es instanciar el modelo 3D del animal basándose en la `SpieceEnum` configurada en el Inspector. Después de instanciarlo, ajusta su escala y posición local, lo establece como hijo del `GameObject` actual y obtiene una referencia al componente `MeshAnimation` adjunto al modelo. Finalmente, llama a `SetSkin(0)` en `MeshAnimate` y puebla `SpieceList` con ceros.
        - **Fragmento de Código:**
            ```csharp
            GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
            Children.transform.localScale = MeshScale;
            Children.transform.parent = transform;
            Children.transform.localPosition = MeshPosition;
            MeshAnimate = Children.GetComponent<MeshAnimation>();
            MeshAnimate.SetSkin(0);
            // ... inicialización de SpieceList
            ```
    - `void Update()`:
        - **Descripción:** Se llama una vez por cada fotograma. Si `MeshAnimate` es válido, calcula un `SpeedRatio` dividiendo la magnitud de la velocidad actual del `NavMeshAgent` por su velocidad máxima. Este ratio se convierte a una cadena de texto y se pasa al método `UpdateAnimation` del `MeshAnimation`, presumiblemente para controlar la mezcla entre animaciones de movimiento (por ejemplo, desde estar quieto a caminar o correr).
        - **Fragmento de Código:**
            ```csharp
            if (MeshAnimate == null) return;
            string SpeedRatio = (Agent.velocity.magnitude / Agent.speed).ToString();
            MeshAnimate.UpdateAnimation("Speed", SpeedRatio);
            ```
    - `void UpdateSpiece(SpieceEnum Spiece)`:
        - **Descripción:** Este método público permite cambiar la especie del animal en tiempo de ejecución. Si la nueva especie (`Spiece`) es la misma que la actual, el método no hace nada. De lo contrario, actualiza la `SpieceEnum` interna, instancia el nuevo prefab del animal, lo asigna como hijo del `GameObject` actual y obtiene una nueva referencia al `MeshAnimation`. Finalmente, llama a `SetSkin` en el nuevo `MeshAnimation` utilizando el valor de `SpieceList` correspondiente a la nueva especie.
        - **Nota:** No se observa en el código un manejo explícito para destruir el modelo del animal previamente instanciado cuando se cambia la especie, lo que podría llevar a que múltiples modelos de animales permanezcan en la jerarquía si esta función se llama repetidamente. Además, la escala y posición local definidas por `MeshScale` y `MeshPosition` no se aplican en esta función al instanciar el nuevo modelo, lo que podría causar inconsistencias.
        - **Fragmento de Código:**
            ```csharp
            void UpdateSpiece(SpieceEnum Spiece)
            {
                if (Spiece == SpieceEnum) return;
                SpieceEnum = Spiece;
                GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
                Children.transform.parent = transform;
                MeshAnimate = Children.GetComponent<MeshAnimation>();
                MeshAnimate.SetSkin(SpieceList[(int)SpieceEnum]);
            }
            ```

- **Lógica Clave:**
    La lógica central del script se basa en la instanciación dinámica de modelos 3D según una enumeración de especies. Inicializa un modelo en `Start`, luego lo anima continuamente en `Update` basándose en la información de movimiento provista por el `NavMeshAgent`. La capacidad de cambiar la especie del animal en tiempo de ejecución a través de `UpdateSpiece` es una característica clave, aunque se debe considerar la gestión de los modelos antiguos.

## 3. Dependencias y Eventos

- **Componentes Requeridos:**
    - Este script requiere implícitamente que el `GameObject` al que está adjunto también tenga un componente `NavMeshAgent` asignado en el Inspector, ya que accede a sus propiedades de velocidad (`Agent.velocity`, `Agent.speed`). Si no está presente, se producirá un error en tiempo de ejecución.
    - Los `GameObject`s en la lista `Prefabs` deben contener un componente `MeshAnimation`, ya que el script intenta obtener y usar este componente después de la instanciación.

- **Eventos (Entrada):**
    - El script expone el método público `UpdateSpiece(SpieceEnum Spiece)`, que puede ser llamado por otros sistemas o scripts en el juego para solicitar un cambio en la especie del animal que se muestra.

- **Eventos (Salida):**
    - Este script no invoca ni publica ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios o acciones realizadas (por ejemplo, cuando se cambia la especie del animal). Su función principal es reactiva y de actualización visual interna.
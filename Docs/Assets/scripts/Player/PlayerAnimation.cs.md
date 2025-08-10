# `PlayerAnimation.cs`

## 1. Propósito General
Este script gestiona la representación visual y las animaciones del personaje animal del jugador en "Beast Card Clash". Su rol principal es instanciar el modelo 3D correcto según la especie seleccionada y sincronizar sus animaciones con el movimiento del personaje, interactuando con Unity's `NavMeshAgent` y un componente de animación personalizado `MeshAnimation`.

## 2. Componentes Clave

### `SpieceEnum`
- **Descripción:** Un tipo `enum` que define las diferentes especies de animales disponibles para los personajes en el juego. Cada miembro de este `enum` corresponde a un índice numérico, que se utiliza para seleccionar el prefab de modelo 3D adecuado de una lista.

### `PlayerAnimation`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, es la encargada de controlar la apariencia y el comportamiento de animación del modelo 3D de un personaje animal. Se ocupa de la instanciación inicial del modelo, de asegurar que se muestre la especie correcta y de actualizar las animaciones de movimiento basándose en la velocidad del personaje, para lo cual se apoya en un `NavMeshAgent` y un script auxiliar `MeshAnimation`.
- **Variables Públicas / Serializadas:**
    - `Prefabs` (`List<GameObject>`): Esta es una lista de prefabs de `GameObject` que representan los distintos modelos 3D de animales. El índice de cada prefab en esta lista se corresponde con un valor en el `SpieceEnum`, permitiendo al script instanciar el modelo adecuado para la especie configurada.
    - `SpieceEnum` (`SpieceEnum`): Una variable que define cuál de las especies de animales configuradas en el `enum` debe ser representada por esta instancia de `PlayerAnimation`. Se utiliza para determinar qué prefab de `Prefabs` instanciar.
    - `Agent` (`NavMeshAgent`): Una referencia al componente `NavMeshAgent` asociado al personaje. El script utiliza este `Agent` para obtener la velocidad actual del personaje, lo cual es fundamental para controlar la velocidad de las animaciones de movimiento del animal.
- **Métodos Principales:**
    - `void Start()`: Este método es parte del ciclo de vida de Unity y se ejecuta una única vez cuando el `MonoBehaviour` es habilitado por primera vez. Su objetivo es preparar el modelo del animal:
        1.  Instancia el prefab del animal inicial, seleccionado según el `SpieceEnum` establecido en el Inspector de Unity.
        2.  Ajusta la escala (`MeshScale`) y la posición local (`MeshPosition`) del modelo instanciado para que se adapte correctamente como hijo del `GameObject` al que está adherido este script.
        3.  Obtiene una referencia al componente `MeshAnimation` del modelo instanciado, que será el encargado de manejar las animaciones y la "skin" (apariencia visual) del modelo.
        4.  Inicializa la "skin" del modelo a la primera variante disponible (`SetSkin(0)`).
        5.  Inicializa la lista interna `SpieceList` con valores predeterminados (cuatro ceros), lo que sugiere que cada especie comienza con su "skin" predeterminada de índice 0.
        ```csharp
        GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
        Children.transform.localScale = MeshScale;
        Children.transform.parent = transform;
        // ...
        ```
    - `void Update()`: Este método también es parte del ciclo de vida de Unity y se ejecuta en cada fotograma del juego. Su función principal es mantener las animaciones del animal sincronizadas con su movimiento:
        1.  Primero, verifica si la referencia `MeshAnimate` es nula para evitar errores.
        2.  Calcula una `SpeedRatio` dividiendo la magnitud de la velocidad actual del `NavMeshAgent` por su velocidad máxima configurada. Esta relación indica qué tan rápido se está moviendo el animal en comparación con su velocidad máxima.
        3.  Usa esta `SpeedRatio` para actualizar el parámetro "Speed" en el componente `MeshAnimation`, lo que controla la velocidad de las animaciones de movimiento del animal (por ejemplo, caminar más rápido o más lento).
        ```csharp
        if (MeshAnimate == null) return;
        string SpeedRatio = (Agent.velocity.magnitude / Agent.speed).ToString();
        MeshAnimate.UpdateAnimation("Speed", SpeedRatio);
        ```
    - `void UpdateSpiece(SpieceEnum Spiece)`: Este método público permite cambiar la especie del animal que el `PlayerAnimation` está representando en tiempo de ejecución.
        1.  Verifica si la nueva `Spiece` es la misma que la actual; si es así, no hace nada para evitar trabajo innecesario.
        2.  Actualiza la variable `SpieceEnum` del script a la nueva especie.
        3.  Instancia un *nuevo* modelo 3D de animal correspondiente a la `Spiece` recién seleccionada.
        4.  Establece el nuevo modelo instanciado como hijo del `GameObject` actual.
        5.  Obtiene y actualiza la referencia al componente `MeshAnimation` del modelo recién instanciado.
        6.  Configura la "skin" del nuevo modelo utilizando un índice almacenado en la lista `SpieceList` que corresponde a la especie seleccionada, permitiendo que diferentes especies puedan tener diferentes apariencias iniciales o por defecto.
        ```csharp
        if (Spiece == SpieceEnum) return;
        SpieceEnum = Spiece;
        GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
        // ...
        ```
- **Lógica Clave:**
    La lógica central de `PlayerAnimation` gira en torno a la gestión dinámica del modelo 3D de un personaje animal. Se encarga de instanciar el modelo adecuado basado en una enumeración de especies (`SpieceEnum`) y, fundamentalmente, de conectar el movimiento del personaje (detectado por un `NavMeshAgent`) con las animaciones de su modelo 3D a través de un componente `MeshAnimation`. El método `UpdateSpiece` es crucial para la jugabilidad, ya que permite cambiar la especie del animal en cualquier momento, lo que implica instanciar un nuevo modelo y configurar su aspecto. Es importante notar que la implementación actual de `UpdateSpiece` instancia un nuevo modelo sin destruir explícitamente el anterior, lo que podría requerir una gestión externa para evitar la acumulación de objetos no deseados en la escena.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, pero su funcionamiento correcto depende fundamentalmente de la presencia de un componente `NavMeshAgent` en el mismo `GameObject` o en un `GameObject` accesible para que pueda obtener los datos de velocidad necesarios para las animaciones. Además, espera que los prefabs de modelos 3D asignados en la lista `Prefabs` contengan el componente `MeshAnimation` para poder controlar sus apariencias y animaciones.
- **Eventos (Entrada):**
    - El script `PlayerAnimation` no se suscribe directamente a eventos de Unity (`UnityEvent`) ni a delegados (`Action`) de otros scripts para recibir notificaciones. Sin embargo, su método público `UpdateSpiece` está diseñado para ser invocado externamente por otros sistemas del juego (por ejemplo, un sistema de selección de personaje o un evento de transformación) para cambiar la especie del animal en tiempo real.
- **Eventos (Salida):**
    - Este script no expone ni invoca ningún evento propio (`UnityEvent` o `Action`) para notificar a otros sistemas sobre cambios en su estado o acciones realizadas. Toda su interacción se limita a leer datos de su `NavMeshAgent` y manipular las propiedades y métodos de su `MeshAnimation` asociado.
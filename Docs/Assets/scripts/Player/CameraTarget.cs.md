# `CameraTarget.cs`

## 1. Propósito General
Este script se encarga de gestionar la posición de un objeto de juego específico, convirtiéndolo en un objetivo dinámico para la cámara principal del juego. Su función principal es asegurar que este objeto siempre coincida con la posición del jugador, proporcionando así un punto de seguimiento estable para la cámara que, crucialmente, no se vea afectado por la rotación del jugador.

## 2. Componentes Clave

### `CameraTarget`
- **Descripción:** `CameraTarget` es una clase que hereda de `MonoBehaviour`, lo que significa que puede ser adjuntada a cualquier GameObject en una escena de Unity. Su rol es simple pero fundamental: mantener su propia posición sincronizada con la del objeto `Player` de referencia. Esto permite que actúe como un "punto de mira" o un "punto de pivote" para la cámara, permitiendo que la cámara siga al jugador mientras se mantiene independiente de la orientación o rotación del mismo.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] Transform Player;`: Esta variable es de tipo `Transform` y está marcada con `[SerializeField]`, lo que la hace visible y editable directamente desde el Inspector de Unity. Aquí se asigna la referencia al componente `Transform` del jugador en la escena. La posición de este `Transform` será replicada por el `CameraTarget` en cada fotograma del juego.

- **Métodos Principales:**
    - `void Start()`: Este es un método del ciclo de vida de Unity que se invoca una vez al inicio, antes de la primera actualización del script, después de que el `MonoBehaviour` ha sido creado. En la implementación actual de `CameraTarget`, el método `Start` está vacío, lo que indica que no se requiere ninguna lógica de inicialización específica al inicio.
    - `void Update()`: Este método del ciclo de vida de Unity se llama una vez por cada fotograma del juego. Es el corazón de la funcionalidad de `CameraTarget`. Dentro de `Update`, la posición del GameObject al que este script está adjunto (`transform.position`) se actualiza constantemente para igualar la posición del `Player` asignado (`Player.position`).

- **Lógica Clave:** La lógica central del script reside enteramente en el método `Update`. La línea `transform.position = Player.position;` es la que realiza la operación de seguimiento. Un comentario importante en el código original (`// El objeto alternativo no rota, lo cual es necesario para neutralizar la rotación del jugador en la cámara`) clarifica que el `CameraTarget` solo copia la *posición* del jugador. Esto es vital para las configuraciones de cámara que necesitan seguir al jugador pero desean mantener una rotación fija o controlada independientemente, sin heredar los giros o inclinaciones del personaje. Al no modificar su propia rotación, el `CameraTarget` asegura que la cámara tenga un punto de referencia estable para mirar o seguir.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, por lo que no impone la presencia obligatoria de otros componentes en el mismo GameObject. Sin embargo, para su correcto funcionamiento, es indispensable que la variable `Player` en el Inspector de Unity tenga asignada una referencia válida a un componente `Transform` (generalmente el del jugador) para poder seguir su posición.

- **Eventos (Entrada):** `CameraTarget` no se suscribe a ningún evento externo (como eventos de UI, colisiones o notificaciones de otros sistemas de juego). Su ejecución se basa exclusivamente en los métodos del ciclo de vida de Unity, en particular `Update`, que se invoca automáticamente en cada fotograma.

- **Eventos (Salida):** Este script no emite ni invoca ningún tipo de evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios o estados. Su responsabilidad se limita a actualizar su propia posición basándose en la del `Player`.
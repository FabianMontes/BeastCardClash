# `animationControleer`
Este script, denominado `animationControleer`, se encarga de gestionar y sincronizar las animaciones de los personajes del juego con el estado actual del combate y las acciones del jugador. Actúa como un puente entre la lógica del juego (quién es el jugador, qué está haciendo, cómo va el combate) y la representación visual de su personaje a través de un componente `Animator`.

Su funcionamiento principal incluye:
1.  **Selección del modelo de personaje**: Al inicio, activa el modelo 3D del personaje correcto basado en la configuración del `Fighter` asociado al jugador.
2.  **Control de animaciones de combate**: Establece un estado inicial de "lucha" para el animador.
3.  **Actualización de parámetros de animación**: En cada frame, ajusta los parámetros del `Animator` (como la velocidad de movimiento, el estado de fin de turno, si el personaje ganó y el tipo de daño elemental recibido) según los datos proporcionados por los componentes `PlayerToken` y el Singleton `CombatJudge`.

La clase está configurada con `[DefaultExecutionOrder(1)]`, lo que asegura que su método `Start` se ejecute después de la mayoría de los otros scripts, garantizando que los componentes de los que depende (como `PlayerToken`) ya estén inicializados. Esta funcionalidad es crucial para que el juego pueda representar visualmente la diversidad de animales colombianos y las distintas facultades de la UNAL a través de modelos dinámicos en un contexto de estrategia por turnos.

# Métodos

## Métodos de Unity

### `Start()`
Este método se ejecuta una vez al inicio del ciclo de vida del script, justo antes de la primera actualización de frame. Su función principal es inicializar las referencias necesarias y configurar el personaje para el combate.

1.  **Obtención de referencias**:
    ```csharp
    player = GetComponent<PlayerToken>();
    figther = player.player;
    ```
    Aquí se obtiene una referencia al componente `PlayerToken` adjunto al mismo GameObject. Luego, de `PlayerToken`, se extrae el objeto `Fighter` asociado, que contiene los datos específicos del personaje, como su índice (`indexFighter`) y estado. Esta relación `PlayerToken` -> `Fighter` es fundamental para vincular la instancia en el campo de batalla con los datos del personaje.

2.  **Impresión de índice**:
    ```csharp
    print(figther.indexFighter);
    ```
    Se imprime el índice del personaje en la consola, lo cual es útil para la depuración y para verificar que el personaje correcto ha sido cargado.

3.  **Activación del modelo**:
    ```csharp
    setModel(figther.indexFighter);
    ```
    Se llama al método `setModel` para activar el modelo 3D correcto del personaje basándose en `figther.indexFighter`. Esto asegura que solo el personaje seleccionado (que representa un animal y una facultad específicos) sea visible.

4.  **Configuración del Animator**:
    ```csharp
    animato = transform.GetChild(figther.indexFighter).GetComponentInChildren<Animator>();
    animato.SetBool("isFigthing", true);
    ```
    Se obtiene el componente `Animator` del modelo 3D activo (que es un GameObject hijo del GameObject principal al que este script está adjunto). Una vez obtenido, se establece el parámetro booleano `isFigthing` del `Animator` a `true`, lo que probablemente activa una animación base de combate o una animación de "idle" que indica que el personaje está listo para la acción.

### `Update()`
Este método se llama una vez por cada frame del juego. Su rol es mantener los parámetros del `Animator` sincronizados con el estado actual del juego en tiempo real, reflejando las interacciones en el combate por turnos.

1.  **Velocidad de movimiento**:
    ```csharp
    animato.SetFloat("Speed", player.Speed());
    ```
    El parámetro `Speed` del `Animator` se actualiza con el valor retornado por el método `Speed()` del componente `PlayerToken`. Esto permite que las animaciones de movimiento del personaje (si las hay, por ejemplo, para ataques o movimientos en el tablero) se ajusten dinámicamente.

2.  **Estado de fin de turno**:
    ```csharp
    animato.SetBool("EndTurn", CombatJudge.Instance.GetSetMoments() == SetMoments.Result);
    ```
    El parámetro booleano `EndTurn` del `Animator` se establece a `true` si el estado actual del combate, obtenido del Singleton `CombatJudge`, es `SetMoments.Result`. Esto podría disparar animaciones específicas que marcan la conclusión de una ronda o una secuencia de acciones, coherente con la naturaleza de juego de estrategia por turnos.

3.  **Estado de victoria/derrota**:
    ```csharp
    animato.SetBool("didWin", figther.NoHurt);
    ```
    El parámetro booleano `didWin` del `Animator` se establece a `true` si la propiedad `NoHurt` del objeto `Fighter` es `true`. Esto podría indicar que el personaje ganó una fase, resistió un ataque o no recibió daño en la última ronda, activando animaciones correspondientes de victoria o resistencia.

4.  **Tipo de daño elemental**:
    ```csharp
    animato.SetInteger("ElementHurt", (int)CombatJudge.Instance.CombatType);
    ```
    El parámetro entero `ElementHurt` del `Animator` se actualiza con el valor entero del tipo de combate actual, obtenido de `CombatJudge.Instance.CombatType`. Esto permite que las animaciones reaccionen a diferentes tipos de elementos o interacciones de combate (por ejemplo, animaciones específicas para daño de fuego, agua, etc.), alineándose con la "estrategia elemental" del juego.

## Otros métodos

### `void setModel(int index)`
`void setModel(int index)`
Este método es responsable de activar el modelo 3D correcto del personaje y desactivar los demás. Se asume que los modelos de los personajes son GameObjects hijos del GameObject al que está adjunto este script, y están indexados del 0 al 3. Este enfoque es directo para la gestión de modelos en un juego indie.

1.  **Desactivación de modelos existentes**:
    ```csharp
    transform.GetChild(0).gameObject.SetActive(false);
    transform.GetChild(1).gameObject.SetActive(false);
    transform.GetChild(2).gameObject.SetActive(false);
    transform.GetChild(3).gameObject.SetActive(false);
    ```
    Las primeras cuatro líneas desactivan explícitamente los GameObjects hijos con índices del 0 al 3. Esto asegura que no haya múltiples modelos de personajes activos simultáneamente y que la visualización del animal/facultad sea la correcta.

2.  **Activación del modelo deseado**:
    ```csharp
    transform.GetChild(index).gameObject.SetActive(true);
    ```
    Finalmente, el GameObject hijo correspondiente al `index` proporcionado (que proviene de `figther.indexFighter`) se activa. Esto hace visible el modelo 3D del personaje seleccionado, permitiendo la carga dinámica de personajes únicos del bestiario colombiano.

## Getters y Setters
Este script no define getters o setters explícitos en el formato tradicional de C# (propiedades con `get` y `set`). En su lugar, interactúa con propiedades y métodos públicos de otros componentes y Singletons para obtener información crucial para la gestión de animaciones. A continuación se listan las propiedades y métodos de terceros que este script utiliza como 'getters':

1.  `player.player`: Proporciona acceso al objeto `Fighter` asociado con el `PlayerToken`.
2.  `player.Speed()`: Retorna un valor flotante que representa la velocidad del jugador, usado para animaciones de movimiento o acción.
3.  `figther.indexFighter`: Retorna un valor entero que identifica el índice del personaje, crucial para seleccionar el modelo 3D y el `Animator` correspondiente.
4.  `figther.NoHurt`: Retorna un valor booleano que indica si el personaje no ha sufrido daño, usado para controlar animaciones de victoria o estado saludable.
5.  `CombatJudge.Instance.GetSetMoments()`: Retorna el estado actual del turno de combate (presumiblemente un valor de un enumerador `SetMoments`), utilizado para activar animaciones de fin de turno.
6.  `CombatJudge.Instance.CombatType`: Retorna el tipo de combate actual (presumiblemente un valor de un enumerador), que se convierte a entero para ser usado en las animaciones y reaccionar a la estrategia elemental.
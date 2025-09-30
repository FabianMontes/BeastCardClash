# PlayerAnimation
El script `PlayerAnimation` es un componente de Unity (`MonoBehaviour`) encargado de gestionar la representación visual del personaje del jugador y sus animaciones básicas en el juego **Beast Card Clash**. Su función principal es permitir al jugador adoptar diferentes formas de animales, representadas por prefabs específicos, y animar estas formas según el movimiento del personaje.

Este script define un enumerado llamado `SpieceEnum` que lista las especies de animales disponibles en el juego: `bear`, `frog`, `condor`, `chamaleon`. Este enumerado se utiliza para seleccionar qué prefab de animal debe ser instanciado y mostrado.

En su funcionamiento, `PlayerAnimation` realiza las siguientes tareas:
-   **Inicialización:** Al inicio del juego, instancia el prefab del animal seleccionado (definido en el Inspector) como un objeto hijo del jugador. Configura su escala, posición y obtiene una referencia al componente `MeshAnimation` del prefab para controlar sus animaciones.
-   **Actualización de Animación:** Continuamente, en cada fotograma, calcula la relación de velocidad del jugador utilizando el componente `NavMeshAgent` adjunto. Esta relación se usa para actualizar la animación del animal, permitiendo transiciones suaves entre estados como "idle" y "correr".
-   **Cambio de Especie:** Proporciona un método público para cambiar dinámicamente la especie del animal del jugador durante el juego. Al cambiar de especie, instancia un nuevo prefab del animal elegido y reemplaza la referencia al `MeshAnimation` para el nuevo modelo.

Este script interactúa directamente con los componentes `NavMeshAgent` (para obtener datos de movimiento) y `MeshAnimation` (para controlar la apariencia y animaciones del modelo 3D). También depende de una lista de `Prefabs` que deben ser asignados en el Inspector, donde cada prefab corresponde a una especie específica del `SpieceEnum`.

# Métodos

## Métodos de Unity

### Start
**Explicación completa del funcionamiento del método:**
El método `Start` se ejecuta una única vez cuando el script se inicializa. Su propósito es configurar la apariencia inicial del personaje del jugador.

1.  **Instanciación del Prefab:** Utiliza la variable `SpieceEnum` (configurada en el Inspector de Unity) para determinar qué prefab de animal de la lista `Prefabs` debe instanciarse. La conversión `(int)SpieceEnum` asume que el orden de los prefabs en la lista coincide con el orden de los valores en el enumerado `SpieceEnum`. El nuevo `GameObject` se instancia en la posición y rotación del objeto al que está adjunto `PlayerAnimation`.
    ```csharp
    GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
    ```
2.  **Configuración del Modelo:**
    *   Establece la escala local del nuevo modelo a `MeshScale` (`new Vector3(0.5f, 0.5f, 0.5f)`), reduciendo su tamaño a la mitad.
    *   Asigna el objeto actual (`this.transform`) como padre del nuevo modelo (`Children.transform.parent = transform;`), convirtiéndolo en un objeto hijo.
    *   Ajusta la posición local del modelo a `MeshPosition` (`new Vector3(0, -1, 0)`), moviéndolo hacia abajo para que quede a nivel del suelo o para ajustarse al pivote del modelo.
    ```csharp
    Children.transform.localScale = MeshScale;
    Children.transform.parent = transform;
    Children.transform.localPosition = MeshPosition;
    ```
3.  **Obtención y Configuración de Animación:**
    *   Recupera el componente `MeshAnimation` del modelo hijo recién instanciado y lo almacena en la variable privada `MeshAnimate`. Este componente se espera que maneje las animaciones específicas del modelo 3D.
    *   Llama al método `SetSkin(0)` del `MeshAnimation` para inicializar el modelo con su primera 'skin' o variación visual, sugiriendo que `MeshAnimation` puede manejar múltiples texturas o materiales para un mismo modelo.
    ```csharp
    MeshAnimate = Children.GetComponent<MeshAnimation>();
    MeshAnimate.SetSkin(0);
    ```

### Update
**Explicación completa del funcionamiento del método:**
El método `Update` se ejecuta en cada fotograma del juego. Su responsabilidad es mantener la animación del animal sincronizada con el movimiento del personaje.

1.  **Verificación de Nulidad:** Primero, comprueba si la referencia a `MeshAnimate` es nula. Esto es una medida de seguridad para evitar errores si, por alguna razón, el componente `MeshAnimation` no se encontró en el prefab o el prefab no se cargó correctamente. Si `MeshAnimate` es nulo, el método retorna, deteniendo la ejecución posterior en este fotograma.
    ```csharp
    if (MeshAnimate == null) return;
    ```
2.  **Cálculo de Relación de Velocidad:**
    *   Calcula una `SpeedRatio` (relación de velocidad) dividiendo la magnitud de la velocidad actual del `NavMeshAgent` (`Agent.velocity.magnitude`) por su velocidad máxima configurada (`Agent.speed`). Este valor normalizado (típicamente entre 0 y 1) indica qué tan rápido se está moviendo el `NavMeshAgent` en relación con su velocidad máxima potencial.
    *   Convierte este valor flotante a una cadena de texto para pasarlo al sistema de animación.
    ```csharp
    string SpeedRatio = (Agent.velocity.magnitude / Agent.speed).ToString();
    ```
3.  **Actualización de Animación:** Llama al método `UpdateAnimation` del componente `MeshAnimate`, pasándole la clave `"Speed"` y la `SpeedRatio` calculada como una cadena. Esto indica que el componente `MeshAnimation` utiliza un sistema de animación basado en parámetros de cadena, donde `"Speed"` probablemente controla la transición entre animaciones como "Idle" (cuando la velocidad es baja) y "Run" (cuando la velocidad es alta).
    ```csharp
    MeshAnimate.UpdateAnimation("Speed", SpeedRatio);
    ```

## Otros métodos

### UpdateSpiece(SpieceEnum Spiece)
**Tipado:** `void UpdateSpiece(SpieceEnum Spiece)`
**Explicación completa del funcionamiento del método:**
Este método permite cambiar la especie del animal del jugador de forma dinámica durante la ejecución del juego, por ejemplo, como parte de una habilidad activada por una carta o una mecánica de juego que requiere la transformación del personaje.

1.  **Verificación de Especie Actual:** Comprueba si la especie (`Spiece`) que se intenta establecer ya es la especie actual (`SpieceEnum`). Si son iguales, el método retorna para evitar recrear el mismo modelo y realizar operaciones innecesarias, optimizando el rendimiento.
    ```csharp
    if (Spiece == SpieceEnum) return;
    ```
2.  **Actualización de Especie e Instanciación:**
    *   Actualiza la variable privada `SpieceEnum` a la nueva especie proporcionada.
    *   Instancia un **nuevo** prefab del animal correspondiente a la nueva `SpieceEnum` de la lista `Prefabs`, de manera similar a cómo se hace en el método `Start`. Este nuevo objeto aparecerá en la posición y rotación del objeto `PlayerAnimation`.
    *   Asigna el objeto `PlayerAnimation` como padre del nuevo modelo (`Children.transform.parent = transform;`).
    ```csharp
    SpieceEnum = Spiece;
    GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
    Children.transform.parent = transform;
    ```
3.  **Actualización de Componente de Animación:**
    *   Recupera el componente `MeshAnimation` del **nuevo** modelo hijo instanciado y actualiza la referencia `MeshAnimate` para que apunte a este nuevo componente. Esto asegura que las actualizaciones de animación futuras (realizadas en `Update`) se apliquen al modelo de la nueva especie.
    ```csharp
    MeshAnimate = Children.GetComponent<MeshAnimation>();
    ```
<br>
> [!NOTE]
> Es importante notar que este método **no destruye explícitamente el prefab del animal previamente instanciado**. Esto implica que el sistema que invoca a `UpdateSpiece` (o el propio componente `MeshAnimation`) debería encargarse de la limpieza del modelo antiguo para evitar la acumulación de objetos en la escena, o bien, esta es una simplificación aceptada para agilizar el desarrollo, en línea con la filosofía de desarrollo del proyecto enfocada en la experiencia de desarrollo rápida.

## Getters y Setters
No hay getters o setters explícitos (propiedades) definidos en este script. La modificación de la especie se realiza a través del método `UpdateSpiece`.
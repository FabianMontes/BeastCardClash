# `PlayerAnimation`
Este script de `MonoBehaviour` es el encargado de gestionar la representación visual (modelo 3D y su animación) del personaje del jugador dentro del juego. Su función principal es instanciar dinámicamente el modelo 3D del animal correspondiente a la especie seleccionada, ajustarlo a la posición y escala correctas, y sincronizar sus animaciones con el movimiento del personaje, utilizando para ello el componente `NavMeshAgent`.

El script interactúa con:
*   Una lista de prefabs `GameObject` que contienen los modelos 3D de las diferentes especies de animales.
*   Un componente `NavMeshAgent` del mismo `GameObject` para obtener la información de velocidad y actualizar las animaciones de movimiento.
*   Un componente `MeshAnimation` (que se espera exista en cada prefab de animal) para controlar las skins y las animaciones específicas del modelo.

En esencia, `PlayerAnimation` actúa como un puente entre la lógica de navegación y movimiento del personaje (gestionada por `NavMeshAgent` y otros posibles scripts) y la presentación visual del animal, asegurando que el modelo correcto sea visible y animado apropiadamente según las acciones del jugador.

# Métodos

## Métodos de Unity

### `Start()`
Este método se invoca una vez al inicio del ciclo de vida del script. Su propósito es inicializar la representación visual del personaje:

1.  **Instanciación del Prefab:** Carga el prefab del animal correspondiente a la `SpieceEnum` configurada en el inspector, utilizándolo como índice para acceder a la lista `Prefabs`. Este prefab es instanciado en la posición y rotación del `GameObject` al que está adjunto `PlayerAnimation`.
    ```csharp
    GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
    ```
2.  **Configuración del Transform:** Ajusta la escala del modelo instanciado a `MeshScale` (0.5 en todos los ejes), lo establece como hijo del `GameObject` actual (`transform`), y ajusta su posición local a `MeshPosition` (0 en X, -1 en Y, 0 en Z) para asegurar que esté al nivel del suelo y correctamente escalado.
    ```csharp
    Children.transform.localScale = MeshScale;
    Children.transform.parent = transform;
    Children.transform.localPosition = MeshPosition;
    ```
3.  **Obtención y Configuración de `MeshAnimation`:** Busca el componente `MeshAnimation` en el prefab recién instanciado y lo almacena en la variable `MeshAnimate`. Luego, inicializa la "skin" del modelo llamando a `MeshAnimate.SetSkin(0)`, lo que sugiere que cada modelo puede tener diferentes apariencias o variantes.
    ```csharp
    MeshAnimate = Children.GetComponent<MeshAnimation>();
    MeshAnimate.SetSkin(0);
    ```

### `Update()`
Este método se invoca una vez por frame y se encarga de mantener las animaciones del personaje sincronizadas con su movimiento:

1.  **Verificación de `MeshAnimate`:** Primero, comprueba si `MeshAnimate` es nulo. Esto actúa como una medida de seguridad para evitar errores si el prefab no se cargó correctamente o si el componente `MeshAnimation` no se encontró.
    ```csharp
    if (MeshAnimate == null) return;
    ```
2.  **Cálculo de `SpeedRatio`:** Calcula una relación de velocidad dividiendo la magnitud de la velocidad actual del `NavMeshAgent` (`Agent.velocity.magnitude`) por la velocidad máxima configurada del agente (`Agent.speed`). Este valor, convertido a `string`, representa la "intensidad" del movimiento.
    ```csharp
    string SpeedRatio = (Agent.velocity.magnitude / Agent.speed).ToString();
    ```
3.  **Actualización de Animación:** Llama al método `UpdateAnimation` de `MeshAnimate`, pasándole "Speed" como nombre de parámetro y `SpeedRatio` como su valor. Esto sugiere que el componente `MeshAnimation` tiene un sistema interno para interpretar este parámetro y ajustar las animaciones de caminar, correr, o estar en reposo del modelo.
    ```csharp
    MeshAnimate.UpdateAnimation("Speed", SpeedRatio);
    ```

## Otros métodos

### `UpdateSpiece(SpieceEnum Spiece)`
Este método permite cambiar dinámicamente la especie del animal que representa al jugador durante la ejecución del juego.

1.  **Verificación de Cambio:** Comprueba si la `SpieceEnum` que se intenta establecer es la misma que la `SpieceEnum` actual. Si son idénticas, el método retorna sin realizar ningún cambio, optimizando el rendimiento.
    ```csharp
    if (Spiece == SpieceEnum) return;
    ```
2.  **Actualización de Especie:** Asigna la nueva especie a la variable `SpieceEnum`.
    ```csharp
    SpieceEnum = Spiece;
    ```
3.  **Instanciación del Nuevo Prefab:** Instancia un nuevo prefab del animal correspondiente a la `SpieceEnum` actualizada. Es importante destacar que **este método no destruye el prefab previamente instanciado**. Esto podría llevar a que múltiples modelos de animales coexistan en la escena si se llama repetidamente sin gestionar la eliminación de los modelos anteriores.
    ```csharp
    GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
    ```
4.  **Configuración de Padre y `MeshAnimation`:** Establece el nuevo prefab como hijo del `GameObject` actual y obtiene su componente `MeshAnimation`, reemplazando la referencia `MeshAnimate` anterior por la del nuevo modelo.
    ```csharp
    Children.transform.parent = transform;
    MeshAnimate = Children.GetComponent<MeshAnimation>();
    ```

## Getters y Setters

1.  `UpdateSpiece(SpieceEnum Spiece)`: Establece la especie del animal.
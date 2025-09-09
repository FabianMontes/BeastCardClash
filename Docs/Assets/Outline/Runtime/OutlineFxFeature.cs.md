# OutlineFxFeature
`OutlineFxFeature` es un componente fundamental para el sistema de renderizado de Unity, diseñado para implementar un efecto de *outline* o contorno alrededor de objetos específicos en la escena. Como un `ScriptableRendererFeature` para el Universal Render Pipeline (URP), se integra directamente en el flujo de renderizado, permitiendo aplicar este efecto como un paso de post-procesado configurable.

Su función principal es ofrecer una manera sencilla y controlada de resaltar visualmente elementos, lo cual es de gran utilidad en **Beast Card Clash** para destacar, por ejemplo, la carta seleccionada por el jugador, un personaje bajo un efecto particular, o unidades específicas en el tablero. El script permite ajustar la apariencia del contorno mediante propiedades como la solidez, el grosor, el modo de detección de bordes (suave o duro) y el filtro utilizado (cruz o caja), además de soportar un patrón de máscara para efectos visuales adicionales.

La filosofía del proyecto, centrada en una buena experiencia de desarrollo, se refleja en cómo `OutlineFxFeature` abstrae la complejidad de los efectos de post-procesado del URP, proporcionando una interfaz limpia para que otros componentes del juego (presumiblemente un script `Outline` adjunto a los objetos) puedan registrarse fácilmente para ser contorneados. Esto permite a los desarrolladores aplicar el efecto sin preocuparse por los detalles internos de renderizado o *shaders*.

Internamente, gestiona *render targets* temporales, compila el *shader* de contorno con los parámetros adecuados, y utiliza una malla de pantalla completa para aplicar el efecto en un paso de *blit*.

# Métodos

## Métodos de Unity

### Create()
Este método se invoca cuando la característica de renderizado (ScriptableRendererFeature) es creada o habilitada. Su propósito principal es la inicialización del sistema de contorno.

*   **Inicialización del `Pass`**: Crea una instancia del `Pass` interno (`_pass`), que es la clase encargada de definir y ejecutar las operaciones de renderizado del contorno. Le asigna una referencia a esta misma característica (`_owner = this`) para que el `Pass` pueda acceder a sus propiedades y configuraciones.
*   **Limpieza de la lista de renderers**: La lista estática `_renderers`, que contiene todos los objetos registrados para ser contorneados en el frame actual, se vacía. Esto asegura que cada frame comience con una lista limpia de objetos a procesar.
*   **Validación de material y contenido**: Llama a los métodos privados `_validateContent()` y `_validateMaterial()` para asegurarse de que el *shader* necesario y el material de contorno estén correctamente asignados y configurados según las propiedades actuales.
*   **Inicialización de la malla de pantalla**: Si `k_ScreenMesh` (la malla utilizada para las operaciones de *blit* de pantalla completa) es nula, se inicializa con un triángulo que cubre toda la pantalla. Esto es eficiente para aplicar efectos de post-procesado.
*   **Configuración de `ShaderTagId`**: Inicializa `k_ShaderTags` con una lista de IDs de *tags* de *shaders* comunes (`SRPDefaultUnlit`, `UniversalForward`, `UniversalForwardOnly`). Estas *tags* se utilizan para identificar qué *shaders* deben ser procesados por el *render pass* cuando se recopilan los objetos a dibujar, asegurando que se capture la geometría correcta para el contorno.

```csharp
public override void Create()
{
    _pass = new Pass() { _owner = this };
    _pass.Init();
    _renderers.Clear();
    
    _validateContent();
    _validateMaterial();
    
    if (k_ScreenMesh == null)
    {
        k_ScreenMesh = new Mesh();
        _initScreenMesh(k_ScreenMesh, Matrix4x4.identity);
    }
    
    if (k_ShaderTags == null)
    {
        k_ShaderTags = new List<ShaderTagId>(new[]
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly")
        });
    }
}
```

### AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
Este método se invoca para cada cámara que se está renderizando, y es el punto donde la característica decide si agregar o no su `RenderPass` personalizado a la cola de renderizado.

*   **Filtrado por tipo de cámara**: Primero, comprueba si la cámara actual es una cámara de juego (`CameraType.Game`) o una vista de escena (`CameraType.SceneView`). Si no es ninguna de estas, el pase no se añade, lo que optimiza el rendimiento al evitar renderizar el contorno en cámaras como vistas previas de UI o mini-mapas no interactivos.
*   **Verificación de objetos para contornear**: Si la lista `_renderers` (que almacena referencias a los objetos que deben ser contorneados en este frame) está vacía, el pase tampoco se añade, ya que no hay nada que contornear.
*   **Cálculo de `_step`**: Se calcula un vector `_step` que representa el desplazamiento de píxeles para el efecto de contorno. Este cálculo tiene en cuenta el `_thickness` del contorno y el `aspect ratio` de la pantalla para mantener una apariencia consistente. Si el `_mode` es `Soft`, el `_step` se duplica para un efecto más pronunciado.
*   **Encolado del `Pass`**: Finalmente, si se cumplen las condiciones anteriores, el `_pass` configurado se encola en el `renderer` para que se ejecute en el momento especificado por `_event`.

```csharp
public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
{
    // in game or scene view only
    if (renderingData.cameraData.cameraType != CameraType.Game && renderingData.cameraData.cameraType != CameraType.SceneView)
        return;
    
    if (_renderers.Count == 0)
        return;
    
    var aspect = Screen.width / (float)Screen.height;
    _step.x = _thickness / aspect;
    _step.y = _thickness;
    _step  *= 0.007f;
    if (_mode == Mode.Soft)
        _step *= 2f;
    
    renderer.EnqueuePass(_pass);
}
```

## Otros métodos

### Render(Outline inst)
`public static void Render(Outline inst)`
Este es un método estático que permite a otros componentes del proyecto registrar una instancia de `Outline` (presumiblemente un `MonoBehaviour` adjunto a un `GameObject`) para ser procesada por este sistema de contorno.

*   **Registro de instancias**: Agrega la instancia `inst` a la lista estática `_renderers`. Esta lista es utilizada por el `RenderPass` para identificar qué objetos necesitan un contorno en el frame actual. Es el mecanismo principal para que los objetos de **Beast Card Clash** soliciten ser resaltados.

### _validateMaterial()
`private void _validateMaterial()`
Este método privado es responsable de inicializar y configurar el material `_outlineMat` que se utilizará para aplicar el efecto de contorno.

*   **Creación del material**: Crea una nueva instancia de `Material` utilizando el *shader* asignado a `_shader`.
*   **Activación de palabras clave de shader**: Basándose en las propiedades `_mode` (Hard/Soft), `_filter` (Cross/Box) y `_solidMask._enabled` (si la máscara sólida está activa), este método habilita las palabras clave (`keywords`) correspondientes en el *shader* del material. Esto permite que el *shader* compile y ejecute diferentes ramas de código optimizadas para cada configuración de contorno.

```csharp
private void _validateMaterial()
{
    _outlineMat = new Material(_shader);
    switch (_mode)
    {
        case Mode.Soft:
            _outlineMat.EnableKeyword("SOFT");
            break;
        case Mode.Hard:
            _outlineMat.EnableKeyword("HARD");
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }
    
    // ... (similar logic for _filter)
    
    if (_solidMask._enabled)
    {
        _outlineMat.EnableKeyword("ALPHA_MASK");
    }
}
```

### _validateContent()
`private void _validateContent()`
Este método privado se asegura de que los activos esenciales, como el *shader* de contorno y el patrón de la máscara sólida, estén correctamente asignados. Su lógica está encapsulada dentro de `#if UNITY_EDITOR`, lo que significa que solo se ejecuta en el editor de Unity, no en el build final del juego.

*   **Búsqueda de shader**: Si `_shader` es nulo, intenta encontrar el *shader* "Hidden/OutlineFx/Main" en los recursos del proyecto y lo asigna.
*   **Carga de patrón de máscara**: Si `_solidMask._pattern` es nulo, intenta cargar la textura "checker.png" que se espera esté en el mismo directorio que el *shader* de contorno.
*   **Marcar como sucio**: `UnityEditor.EditorUtility.SetDirty(this)` asegura que los cambios realizados en las propiedades de este `ScriptableRendererFeature` se guarden en el editor.

### _initScreenMesh(Mesh mesh, Matrix4x4 mat)
`private static void _initScreenMesh(Mesh mesh, Matrix4x4 mat)`
Este método estático privado construye una malla de triángulo simple que cubre toda la pantalla. Esta malla es fundamental para las operaciones de *post-procesado* y *blit* que no necesitan información compleja de geometría, ya que el efecto se aplica a toda la superficie de la pantalla.

*   **Creación de vértices y UVs**: Define tres vértices y sus correspondientes coordenadas UV, de modo que cuando se rendericen, cubran la pantalla completa. Se encarga de invertir las coordenadas UV si el sistema de gráficos comienza en la parte superior (`SystemInfo.graphicsUVStartsAtTop`).
*   **Asignación de triángulos**: Configura los índices de los vértices para formar un único triángulo.
*   **Optimización**: `mesh.UploadMeshData(true)` optimiza el rendimiento al enviar los datos de la malla a la GPU de manera eficiente.

### _blit(CommandBuffer cmd, RTHandle from, RTHandle to, Material mat, int pass = 0)
`private static void _blit(CommandBuffer cmd, RTHandle from, RTHandle to, Material mat, int pass = 0)`
Este es un método estático de utilidad para realizar una operación de *blit* (copiado y/o procesado) de una textura a otra utilizando un `CommandBuffer` de URP. Es una función común en los efectos de post-procesado.

*   **Asignación de textura fuente**: Establece la textura de origen (`from`) como la textura principal global en el *shader* (`_MainTexId`).
*   **Asignación de render target**: Establece el `RTHandle` de destino (`to`) como el objetivo de renderizado.
*   **Dibujado de la malla de pantalla**: Dibuja `k_ScreenMesh` (el triángulo de pantalla completa) utilizando el `mat` proporcionado y un `pass` específico del *shader*.

### _alloc(string id)
`private static RTHandle _alloc(string id)`
Este método estático privado es una función auxiliar para asignar un nuevo `RTHandle` (Render Texture Handle) con un nombre dado.

*   **Asignación de RTHandle**: Utiliza `RTHandles.Alloc()` para crear un nuevo `RTHandle`, facilitando la gestión de *render textures* temporales en el pipeline.

## Getters y Setters

1.  **Solid (float)**: Obtiene o establece el nivel de solidez del relleno del contorno. El valor se restringe entre 0 (transparente) y 1 (completamente sólido).
2.  **Thickness (float)**: Obtiene o establece el grosor del contorno. El valor se restringe entre 0 (sin grosor) y 1 (grosor máximo).
3.  **Mask (bool)**: Obtiene o establece si la máscara sólida (SolidMask) está habilitada. Al cambiar este valor, también se habilita o deshabilita la palabra clave "ALPHA_MASK" en el material de contorno (`_outlineMat`), lo que activa o desactiva la funcionalidad de máscara en el *shader*.
# OutlineFxFeature

`OutlineFxFeature.Pass` es una clase anidada dentro de la clase parcial `OutlineFxFeature`. Representa un `ScriptableRenderPass` dentro del Universal Render Pipeline (URP) de Unity, encargado de la lógica de renderizado para aplicar un efecto de contorno (outline) a objetos específicos en la escena. Su función principal es gestionar los pasos de renderizado necesarios para dibujar estos contornos, utilizando un material y una configuración definidos por su clase contenedora `OutlineFxFeature`.

Este pass opera en varias etapas para lograr el efecto de contorno:
1.  **Inicialización y Configuración**: Al inicio, se configura para ejecutarse en un evento de renderizado específico y se prepara un `RenderTarget` intermedio (`_buffer`) para dibujar los objetos.
2.  **Preparación del Shader**: Durante la ejecución, ajusta las propiedades del material de contorno (`_owner._outlineMat`) con valores como el umbral de alfa, la solidez y los parámetros de un posible patrón de máscara animado.
3.  **Renderizado a Buffer Intermedio**: Dibuja los objetos que requieren contorno en el `_buffer` temporal. Esto se realiza iterando sobre una colección de renderizadores proporcionada por la clase `OutlineFxFeature`, configurando texturas y colores para cada uno y utilizando el material de contorno.
4.  **Aplicación Final del Contorno**: Una vez dibujados los objetos en el `_buffer`, se realiza una operación de "blit" (copiar con procesamiento) desde este `_buffer` al `RTHandle` de salida final (que puede ser el buffer de color de la cámara o un `RTHandle` personalizado). En este paso, el material de contorno aplica el efecto visual del contorno basándose en la información renderizada previamente.
5.  **Limpieza de Recursos**: Después de cada frame, se liberan los recursos temporales asignados, como el `RenderTarget` intermedio y los buffers de comandos, para evitar fugas de memoria y optimizar el rendimiento.

En el contexto de "Beast Card Clash", este sistema de contornos es crucial para resaltar elementos interactivos como cartas seleccionadas, unidades en el tablero o puntos de interés, mejorando la retroalimentación visual al jugador y la claridad de la interfaz. La flexibilidad en su configuración permite a los desarrolladores ajustar rápidamente el aspecto del contorno para adaptarse a las necesidades estéticas y funcionales del juego, priorizando una buena experiencia de desarrollo y de jugador por encima de una implementación excesivamente rígida.

# Métodos

## Métodos de Unity

### Init()
Este método se llama para inicializar el `ScriptableRenderPass`. Es aquí donde se configuran los parámetros base del pass antes de que empiece a ejecutarse cada frame.

```csharp
public void Init()
{
    renderPassEvent = _owner._event;
    _buffer         = new RenderTarget().Allocate(nameof(_buffer));
}
```

*   **`renderPassEvent = _owner._event;`**: Asigna el evento de renderizado en el ciclo del URP en el que este pass debe ejecutarse. El valor se obtiene de la instancia `_owner` de `OutlineFxFeature`, permitiendo que la clase principal controle cuándo se inserta el pass en el pipeline.
*   **`_buffer = new RenderTarget().Allocate(nameof(_buffer));`**: Asigna un nuevo `RenderTarget` con el nombre `_buffer`. Este `RenderTarget` servirá como un buffer intermedio donde se dibujarán los objetos a los que se les aplicará el contorno, antes de que el efecto final se aplique al buffer de la cámara.

### Execute(ScriptableRenderContext context, ref RenderingData renderingData)
Este es el método principal del `ScriptableRenderPass` y se invoca una vez por cámara por frame (o según el `renderPassEvent` configurado). Contiene toda la lógica de renderizado para aplicar el efecto de contorno.

```csharp
public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
{
    // ... lógica de renderizado ...
}
```

La ejecución de este método se puede desglosar en los siguientes pasos:

1.  **Obtención y Preparación del Buffer de Comandos**:
    ```csharp
    var cmd  = CommandBufferPool.Get(nameof(OutlineFxFeature));
    var desc = renderingData.cameraData.cameraTargetDescriptor;
    desc.colorFormat = RenderTextureFormat.ARGB32;
    _buffer.Get(cmd, desc);
    ```
    Se obtiene un `CommandBuffer` del pool, se ajusta la descripción del `RenderTarget` de la cámara a `ARGB32` (para asegurar la transparencia necesaria para el contorno) y se configura el `_buffer` temporal con esta descripción.

2.  **Validación del Material**:
    ```csharp
    if (_owner._outlineMat == null)
        return;
    ```
    Si el material de contorno (`_owner._outlineMat`) no está asignado, el pass no puede renderizar y se interrumpe la ejecución.

3.  **Configuración de Propiedades del Shader**:
    ```csharp
    _owner._outlineMat.SetFloat(s_Alpha, _owner._alphaCutout);
    _owner._outlineMat.SetFloat(s_Solid, _owner._solid);
    ```
    Se asignan valores de umbral alfa (`_alphaCutout`) y solidez (`_solid`) al material de contorno, controlando cómo se renderiza el contorno.

    También se gestiona la aplicación de una máscara sólida (`_solidMask`) si está habilitada, calculando offsets y escalas para animar un patrón en el contorno:
    ```csharp
    if (_owner._solidMask._enabled)
    {
        var sm = _owner._solidMask;
        _owner._outlineMat.SetTexture(s_AlphaTex, sm._pattern);
        // ... cálculo de offsets para animación ...
        _owner._outlineMat.SetVector(s_AlphaTO, new Vector4(sm._scale * (Screen.width / (float)Screen.height) / aspectTex, sm._scale, xOffset, yOffset));
    }
    ```
    Esto permite contornos con patrones de textura animados, lo cual puede ser útil para efectos visuales distintivos en "Beast Card Clash", como indicar estados especiales de una carta.

4.  **Determinación del `RTHandle` de Salida**:
    Se maneja la obtención del `RTHandle` de salida final (`_output`), que puede ser el buffer de color de la cámara o un `RTHandle` personalizado si la configuración `_owner._output.Enabled` lo indica. Se incluye un manejo condicional para compatibilidad entre versiones de Unity.

    ```csharp
    #if !UNITY_2022_1_OR_NEWER
                    if (_owner._output.Enabled == false)
                        _output = RTHandles.Alloc(renderingData.cameraData.renderer.cameraColorTarget);
    #else
    				_output = renderingData.cameraData.renderer.cameraColorTargetHandle;
    #endif
                    if (_owner._output.Enabled)
                        _output = _alloc(_owner._output.Value);
    ```

5.  **Renderizado al Buffer Intermedio (`_buffer`)**:
    Se configura `_buffer.Handle` como el `RenderTarget` actual y se limpia con un color transparente.
    ```csharp
    cmd.SetRenderTarget(_buffer.Handle.nameID);
    cmd.ClearRenderTarget(false, true, Color.clear, 1f);
    ```
    Si `_owner._attachDepth` está activo, también se adjunta el buffer de profundidad de la cámara para que los objetos dibujados al `_buffer` respeten la oclusión.

6.  **Dibujo de Renderizadores con Contorno**:
    Se itera sobre la colección `_renderers` (implicada en la clase `OutlineFxFeature` como los objetos marcados para contorno). Para cada objeto:
    ```csharp
    foreach (var inst in _renderers)
    {
        if (inst == null)
            continue;
        
        cmd.SetGlobalTexture(s_MainTex, inst._renderer.sharedMaterial.mainTexture);
        cmd.SetGlobalColor(s_Color, inst.Color);
        cmd.DrawRenderer(inst._renderer, _owner._outlineMat, 0, 0);
        
    }
    _renderers.Clear();
    ```
    *   Se establece la textura principal (`_MainTex`) y el color (`_Color`) globalmente para el shader.
    *   Se invoca `cmd.DrawRenderer`, que dibuja el `_renderer` del objeto usando el `_owner._outlineMat` (material de contorno) en el primer sub-shader (pass 0).
    *   Finalmente, la lista `_renderers` se limpia, indicando que estos objetos han sido procesados para el frame actual.

7.  **Aplicación Final del Contorno (Blit)**:
    ```csharp
    cmd.SetGlobalVector(s_Step, _owner._step);
    _blit(_buffer.Handle, _output, _owner._outlineMat, 1);
    ```
    Se establece el vector `_Step` en el shader (que típicamente controla la dirección y el grosor del contorno) y se realiza una operación `_blit`. Esta operación copia el contenido de `_buffer.Handle` a `_output`, utilizando el material de contorno `_owner._outlineMat` con el segundo sub-shader (pass 1), que es donde el efecto de contorno real se calcula y se aplica.

8.  **Ejecución y Liberación del Buffer de Comandos**:
    ```csharp
    _execute();
    ```
    Los comandos acumulados en `cmd` se envían a la GPU para su ejecución, y el `CommandBuffer` se libera de nuevo al pool.

### FrameCleanup(CommandBuffer cmd)
Este método se llama después de que el pass haya terminado su ejecución para el frame actual. Su propósito principal es liberar cualquier recurso temporal que haya sido asignado durante la fase de `Execute` o `Init`.

```csharp
public override void FrameCleanup(CommandBuffer cmd)
{
    _buffer.Release(cmd);
    
#if !UNITY_2022_1_OR_NEWER
    RTHandles.Release(_output);
#else
    if (_owner._output.Enabled)
        RTHandles.Release(_output);
#endif
}
```

*   **`_buffer.Release(cmd);`**: Libera el `RenderTarget` intermedio `_buffer` que fue asignado en `Init()`. Es crucial para evitar fugas de memoria.
*   **`RTHandles.Release(_output);`**: Libera el `RTHandle` de salida `_output` si este fue asignado internamente por el pass (cuando `_owner._output.Enabled` es falso en versiones anteriores de Unity) o si `_owner._output.Enabled` es verdadero para versiones más nuevas. Esto garantiza que cualquier `RTHandle` creado específicamente para este pass sea devuelto al sistema de manejo de texturas.

## Otros métodos

### _blit(RTHandle from, RTHandle to, Material mat, int pass = 0) (local)
Este es un método auxiliar privado definido localmente dentro de `Execute`. Su función es simplificar la llamada a una función `_blit` estática (implícita en la clase `OutlineFxFeature` padre) que realiza la copia de una textura a otra mientras aplica un shader.

```csharp
void _blit(RTHandle from, RTHandle to, Material mat, int pass = 0)
{
    OutlineFxFeature._blit(cmd, from, to, mat, pass);
}
```

*   **`cmd`**: El `CommandBuffer` actual donde se añaden los comandos de renderizado.
*   **`from`**: El `RTHandle` de la textura de origen.
*   **`to`**: El `RTHandle` de la textura de destino.
*   **`mat`**: El `Material` con el shader a aplicar durante la operación de blit.
*   **`pass`**: El índice del pass del sub-shader a usar dentro del material. En este contexto, el pass `1` es el que aplica el efecto de contorno.

> [!NOTE]
> La presencia de un método estático `OutlineFxFeature._blit` sugiere que la clase `OutlineFxFeature` centraliza funcionalidades comunes de blitting, promoviendo la reutilización de código.

### _execute() (local)
Este es otro método auxiliar privado definido localmente dentro de `Execute`. Sirve para encapsular la lógica de ejecución del `CommandBuffer` y su posterior liberación.

```csharp
void _execute()
{
    context.ExecuteCommandBuffer(cmd);
    CommandBufferPool.Release(cmd);
}
```

*   **`context.ExecuteCommandBuffer(cmd);`**: Envía el `CommandBuffer` actual (`cmd`) a la `ScriptableRenderContext` para que sus comandos sean procesados por la GPU.
*   **`CommandBufferPool.Release(cmd);`**: Devuelve el `CommandBuffer` al `CommandBufferPool`, permitiendo que sea reutilizado en el futuro y evitando la creación y destrucción constante de objetos, lo que es una buena práctica para la optimización en Unity.

## Getters y Setters

La clase `OutlineFxFeature.Pass` no define métodos explícitos de "getter" o "setter" con la sintaxis de propiedades de C#. Sin embargo, interactúa con la clase `OutlineFxFeature` (`_owner`) a través de sus campos públicos. La única propiedad pública directamente en `Pass` es:

1.  `public OutlineFxFeature _owner`: Este campo es establecido externamente para proporcionar una referencia a la instancia principal de `OutlineFxFeature`, permitiendo al pass acceder a la configuración (material, parámetros de contorno, etc.) y la lista de renderizadores que necesita procesar.
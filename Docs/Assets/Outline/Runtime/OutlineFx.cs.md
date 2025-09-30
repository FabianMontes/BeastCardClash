# OutlineFx
El script `OutlineFx` es un componente de Unity diseñado para gestionar y aplicar un efecto de contorno (outline) a los objetos del juego. Este script es una implementación específica de la clase base `Outline` y se centra en el control directo del color y la transparencia del contorno. Su simplicidad lo hace ideal para el proyecto "Beast Card Clash", permitiendo a los desarrolladores configurar rápidamente los colores de resaltado para cartas, personajes o elementos interactivos, lo cual es crucial para la experiencia de usuario en un juego de estrategia por turnos.

Una característica importante de este script es el atributo `[ExecuteAlways]`. Este atributo indica que el script se ejecutará tanto en el modo de edición (cuando Unity está abierto pero el juego no está corriendo) como en el modo de juego. Esto es especialmente útil para el desarrollo de efectos visuales como los contornos, ya que permite a los diseñadores y programadores ver los cambios en tiempo real en la escena sin necesidad de iniciar el juego. Esto agiliza el proceso de ajuste visual, alineándose con el objetivo del proyecto de mejorar la experiencia de desarrollo.

El script `OutlineFx` reside dentro del namespace `OutlineFx`, lo que ayuda a organizar el código y evitar conflictos de nombres con otras clases.

```csharp
namespace OutlineFx
{
    [ExecuteAlways]
    public class OutlineFx : Outline
    {
        // ...
    }
}
```

# Métodos

## Métodos de Unity

### Atributo `[ExecuteAlways]`
Aunque `OutlineFx` no define explícitamente métodos de ciclo de vida de Unity como `Awake`, `Start` o `Update`, la presencia del atributo `[ExecuteAlways]` en la clase es fundamental para su comportamiento.

-   **Funcionamiento:** Este atributo fuerza a que el componente se ejecute en todo momento:
    -   **Modo de Edición:** Cuando la escena está abierta en el editor, el script puede inicializarse y actualizarse (si tuviera métodos `Update` o `LateUpdate` definidos) incluso cuando el juego no está en ejecución. Esto es particularmente útil para visualizar y ajustar el efecto de contorno en tiempo real directamente en la vista de escena, lo que permite una iteración visual rápida sin necesidad de entrar constantemente al modo de juego.
    -   **Modo de Juego:** El script se comporta como un componente normal en el modo de juego, donde sus propiedades y lógica se aplican según la ejecución del juego.

La inclusión de `[ExecuteAlways]` demuestra un enfoque en la mejora de la experiencia del desarrollador, permitiendo un ajuste visual inmediato y eficiente de los contornos para los diversos elementos visuales de "Beast Card Clash", como la selección de cartas de animales colombianos o la visualización de unidades en el tablero.

## Otros métodos

Este script no implementa métodos adicionales más allá de los accesores de propiedades. Todas sus funcionalidades principales se exponen a través de sus propiedades públicas.

## Getters y Setters

1.  `Color Color`: Permite obtener o establecer el color principal del contorno. Este es un `override` de una propiedad de la clase base `Outline`, lo que significa que `OutlineFx` proporciona su propia implementación para cómo se maneja el color del contorno. El valor se almacena internamente en la variable `_color`.

    ```csharp
    public override Color Color
    {
        get => _color;
        set => _color = value;
    }
    ```

2.  `float Alpha`: Permite obtener o establecer únicamente el componente de transparencia (canal alfa) del color del contorno. Este accesorio es una conveniencia para los desarrolladores, ya que permite modificar la opacidad del contorno de forma independiente del resto de los canales de color (rojo, verde, azul), sin tener que manipular la estructura `Color` directamente. Un valor de `0.0` representa un contorno completamente transparente, mientras que `1.0` es completamente opaco. Esto puede ser útil para efectos de aparición/desaparición o para indicar el estado de un elemento en el juego.

    ```csharp
    public float Alpha
    {
        get => _color.a;
        set => _color.a = value;
    }
    ```
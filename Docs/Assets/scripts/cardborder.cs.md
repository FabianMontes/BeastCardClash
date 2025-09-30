# cardborder
Este script, `cardborder`, es un componente fundamental para la representación visual de las cartas en la mano del jugador dentro de **Beast Card Clash**. Su función principal es gestionar dinámicamente el color del borde de una carta, adaptándolo según su elemento y su estado de interactividad. Se adjunta a un GameObject que contiene un componente `Image`, el cual es el encargado de mostrar visualmente este borde.

El script interactúa directamente con el componente `HandCard` de su padre, obteniendo información crucial sobre la carta actual (`Card`) que se encuentra en esa posición de la mano. Basándose en el elemento de la carta (`GetElement()`) y si es clicable (`isClickable()`) o es un `picker` (selector), `cardborder` ajusta el color del borde. Si una carta no es clicable y no es un `picker`, su borde se oscurece para proporcionar una clara retroalimentación visual al jugador sobre su estado interactivo.

> [!NOTE]
> Actualmente, la clase `cardborder` no sigue la convención de nomenclatura PascalCase para clases en C#. Se recomienda renombrarla a `CardBorder` en una futura refactorización para adherirse a las buenas prácticas.

# Métodos

## Métodos de Unity

### Start
Este método se invoca una vez en el ciclo de vida del script, justo antes de la primera actualización del frame. Su propósito principal es inicializar las referencias a los componentes necesarios para el funcionamiento del script.

```csharp
void Start()
{
    card = GetComponentInParent<HandCard>();
    image = transform.GetComponent<Image>();
}
```

*   **`card = GetComponentInParent<HandCard>();`**: Obtiene una referencia al componente `HandCard` que se encuentra en un GameObject padre. Esto es crucial ya que `cardborder` necesita interactuar con la lógica de la carta padre para determinar su estado y propiedades.
*   **`image = transform.GetComponent<Image>();`**: Obtiene una referencia al componente `Image` adjunto al mismo GameObject que el script `cardborder`. Este componente `Image` será el encargado de visualizar el borde de la carta, y el script modificará su color.
    > [!TODO]
    > Se recomienda cambiar `transform.GetComponent<Image>()` por la versión abreviada `GetComponent<Image>()` para mayor claridad y eficiencia, ya que busca el componente en el propio GameObject.

### Update
Este método se invoca una vez por cada frame del juego. Es donde reside la lógica principal para actualizar el color del borde de la carta en tiempo real, respondiendo a los cambios en el estado de la carta.

```csharp
void Update()
{
    // Si no hay carta, deshabilita la imagen
    if (card.GetCard() == null)
    {
        image.enabled = false;
        return;
    }

    // Si hubo carta, establece su color y habilita la imagen
    Color color = colors[(int)card.GetCard().GetElement()];
    image.enabled = true;

    // ... lógica de oscurecimiento ...

    // Establece el color de la imagen
    image.color = color;
}
```

El método `Update` realiza los siguientes pasos en cada frame:

1.  **Verificación de existencia de carta**: Primero, comprueba si el `HandCard` padre contiene una carta (`card.GetCard() == null`).
    *   Si no hay carta, el componente `Image` del borde se deshabilita (`image.enabled = false`) para que no sea visible, y el método `Update` termina.
2.  **Establecimiento del color base**: Si hay una carta, el `Image` del borde se habilita (`image.enabled = true`). El color base del borde se obtiene del array `colors` utilizando el elemento de la carta (`card.GetCard().GetElement()`) como índice. Esto permite que cada tipo de elemento (`Element`) tenga un color de borde distintivo.
3.  **Lógica de oscurecimiento para no interactuables**:
    ```csharp
    if (!card.isClickable() && !card.picker)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v); // Convierte a HSV
        v = darkValue; // Ajusta el valor de luminosidad
        color = Color.HSVToRGB(h, s, v); // Vuelve a convertir a RGB
    }
    ```
    *   Esta sección verifica si la carta **no es clicable** (`!card.isClickable()`) y **no es un `picker`** (`!card.picker`). Si ambas condiciones son verdaderas, se interpreta que la carta no debe ser interactuable en el estado actual del juego.
    *   Para reflejar esto visualmente, el color se oscurece. Esto se logra convirtiendo el color actual a su representación HSV (Hue, Saturation, Value), modificando el componente `Value` (luminosidad) al `darkValue` predefinido, y luego convirtiendo el color de nuevo a RGB.
    > [!TODO]
    > Se sugiere pasar las variables `h`, `s`, `v` directamente como parámetros a los métodos `Color.RGBToHSV` y `Color.HSVToRGB` para mejorar la legibilidad y posible eficiencia.
4.  **Aplicación del color final**: Finalmente, el color (ya sea el color base o el oscurecido) se aplica al componente `Image` (`image.color = color`), actualizando el aspecto visual del borde de la carta en el juego.
# `cardborder`
El script `cardborder` es un componente visual que gestiona el color del borde de una carta en el juego, adaptándolo según su elemento y su estado de interactividad. Actúa como un elemento visual complementario a la lógica de la carta principal, proporcionando retroalimentación visual al jugador sobre las propiedades o el estado actual de la carta. Se adjunta a un GameObject que es hijo de una carta de mano (`HandCard`), y utiliza el componente `Image` de su propio GameObject para dibujar el borde. Su objetivo principal es realzar visualmente las cartas y ofrecer una indicación clara de su estado, como si está o no disponible para ser seleccionada o jugada.

> [!NOTE]
> Se ha identificado una oportunidad de mejora para renombrar esta clase a `CardBorder` para seguir las convenciones de nomenclatura en C# (PascalCase).

# Métodos

## Métodos de Unity

### `Start()`
El método `Start()` se ejecuta una vez al inicio del ciclo de vida del script. Su función es inicializar las referencias a otros componentes necesarios para el funcionamiento de `cardborder`.

Primero, obtiene una referencia al script `HandCard` que se encuentra en el GameObject padre. Esto es crucial ya que `cardborder` necesita acceder a la información de la carta que representa (`HandCard`) para determinar su elemento y estado.

```csharp
card = GetComponentInParent<HandCard>();
```

Luego, obtiene una referencia al componente `Image` adjunto al mismo GameObject que `cardborder`. Este componente `Image` es el borde visual cuya apariencia será modificada por el script.

```csharp
image = transform.GetComponent<Image>();
// TODO: Cambiar image = transform.GetComponent<Image>(); por image = GetComponent<Image>();
```
> [!NOTE]
> Se ha identificado una oportunidad de mejora para optimizar la obtención del componente `Image` utilizando `GetComponent<Image>()` directamente, que es ligeramente más eficiente que `transform.GetComponent<Image>()` cuando el componente está en el mismo GameObject.

### `Update()`
El método `Update()` se ejecuta en cada fotograma del juego. Su responsabilidad principal es asegurar que el color del borde de la carta se mantenga actualizado de acuerdo con el estado de la carta de mano (`HandCard`) y la carta que contiene.

#### 1. Gestión de Visibilidad
Verifica si la `HandCard` a la que está asociado el borde tiene una carta asignada (`card.GetCard() != null`). Si no hay una carta asignada, el borde se deshabilita para que no se muestre.

```csharp
if (card.GetCard() == null)
{
    image.enabled = false;
    return;
}
```
Si se detecta que hay una carta, asegura que el borde esté visible.

```csharp
image.enabled = true;
```

#### 2. Determinación del Color Base
Obtiene el elemento de la carta actual (a través de `card.GetCard().GetElement()`) y lo usa como índice para seleccionar un color del arreglo `colors`. Este arreglo `colors` se espera que contenga los colores base para cada tipo de elemento de carta (ej: fuego, agua, tierra, etc.).

```csharp
Color color = colors[(int)card.GetCard().GetElement()];
```

#### 3. Aplicación del Efecto Oscurecido (Feedback Visual)
Aplica un efecto de oscurecimiento al borde si la carta no es "clicable" y no está en modo "picker". Esto sirve como una señal visual para el jugador de que la carta no puede ser interactuada en ese momento.

```csharp
if (!card.isClickable() && !card.picker)
{
    float h, s, v;
    Color.RGBToHSV(color, out h, out s, out v);
    v = darkValue; // Establece la luminosidad al valor predefinido
    color = Color.HSVToRGB(h, s, v);
}
// TODO: Pasar las variables como parámetros de RGBToHSV y HSVToRGB
```
> [!TIP]
> La lógica de oscurecimiento se basa en el modelo de color HSV (Hue, Saturation, Value/Brightness). Se convierte el color actual a HSV, se ajusta el componente `v` (Value) al valor preestablecido `darkValue` (que es más bajo, indicando menos brillo) y luego se convierte de nuevo a RGB. Esto permite oscurecer el color manteniendo su tono y saturación.
>
> [!NOTE]
> Se ha identificado una oportunidad de mejora para pasar directamente los valores de `h`, `s`, `v` como parámetros a los métodos `RGBToHSV` y `HSVToRGB` en lugar de utilizar variables temporales, lo cual puede simplificar el código.

#### 4. Aplicación Final del Color
Finalmente, el color (ya sea el color base o el oscurecido) se aplica al componente `Image` del borde.

```csharp
image.color = color;
```

## Getters y Setters
Este script no implementa métodos públicos explícitos que funcionen como getters o setters. Sin embargo, expone datos para su configuración a través del Inspector de Unity mediante el atributo `[SerializeField]`, permitiendo ajustar el comportamiento visual del borde sin necesidad de modificar el código.

1. `colors`: Un arreglo de colores (`Color[]`) que permite definir los colores base para cada elemento de las cartas. Se asigna desde el Inspector de Unity y su índice se corresponde con el valor `enum` del elemento de la carta.
2. `darkValue`: Un valor flotante (`float`) que determina la luminosidad (`Value` en HSV) del borde cuando la carta no es clicable. Se configura desde el Inspector de Unity, y su valor debe estar entre 0.0f (negro) y 1.0f (brillo completo).
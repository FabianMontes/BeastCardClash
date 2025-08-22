# `cardborder.cs`

## 1. Propósito General
El script `cardborder` gestiona la apariencia visual del borde de una carta en el juego, específicamente su color y visibilidad. Su función principal es adaptar el color del borde de una carta basándose en su elemento y en su estado de interactividad (si es clicable o no), trabajando en conjunto con el componente `HandCard` de la carta.

## 2. Componentes Clave

### `cardborder` (Clase `MonoBehaviour`)
-   **Descripción:** Esta clase es un `MonoBehaviour`, lo que significa que debe adjuntarse a un GameObject en Unity. Su rol es renderizar visualmente el borde de una carta, ajustando su color según el tipo elemental de la carta y atenuándolo si la carta no está interactuable. Es probable que este script esté en un GameObject hijo del que contiene el script `HandCard`, ya que busca el `HandCard` en un componente padre.
-   **Variables Públicas / Serializadas:**
    -   `Color[] colors`: Un array de objetos `Color`. Cada índice de este array se corresponde con un tipo elemental de carta (obtenido del método `GetElement()` de la carta) y se usa para establecer el color base del borde. Estas colores son configurables directamente en el Inspector de Unity.
    -   `float darkValue`: Un valor flotante que determina la luminosidad a la que se reduce el color del borde cuando la carta no es interactuable. Un valor más bajo resultará en un color más oscuro. También se configura en el Inspector.
-   **Métodos Principales:**
    -   `void Start()`: Este es un método del ciclo de vida de Unity, llamado una vez al inicio del ciclo de vida del script.
        -   Inicializa las referencias a los componentes `HandCard` y `Image`.
        -   Obtiene la referencia al script `HandCard` buscando un componente de ese tipo en los GameObjects padres (`GetComponentInParent<HandCard>()`).
        -   Obtiene la referencia al componente `Image` adjunto al mismo GameObject donde reside este script (`transform.GetComponent<Image>()`). Este `Image` es el elemento visual que representa el borde de la carta.
    -   `void Update()`: Este es un método del ciclo de vida de Unity, llamado una vez por cada frame.
        -   Es responsable de actualizar continuamente el estado visual del borde.
        -   Comprueba si la carta asociada (`card.GetCard()`) es `null`. Si no hay carta, el borde se deshabilita (`image.enabled = false;`) y la función termina, ocultando el borde.
        -   Si hay una carta, el color base del borde se selecciona del array `colors` utilizando el elemento de la carta como índice. Por ejemplo, si `card.GetCard().GetElement()` devuelve `0`, se usará `colors[0]`.
        -   El borde se habilita (`image.enabled = true;`) para asegurar su visibilidad.
        -   Se aplica una lógica para oscurecer el borde:
            -   Si la carta *no* es clicable (`!card.isClickable()`) Y *no* es un "picker" (`!card.picker`), el color del borde se modifica.
            -   La modificación se realiza convirtiendo el color actual a su representación HSV (Tono, Saturación, Valor/Luminosidad), ajustando el valor de Luminosidad (`v`) al `darkValue` configurado, y luego convirtiéndolo de nuevo a RGB. Esto crea un efecto de atenuación.
        -   Finalmente, el color resultante (original o atenuado) se asigna a la propiedad `color` del componente `Image` (`image.color = color;`).
-   **Lógica Clave:**
    La lógica principal reside en el método `Update`, que se ejecuta constantemente para mantener el borde sincronizado con el estado de la carta. La detección de si la carta está presente (`card.GetCard() == null`) es crucial para alternar la visibilidad del borde. La atenuación del color, basada en los métodos `isClickable()` y la propiedad `picker` de la `HandCard`, utiliza una conversión de color RGB a HSV y viceversa para modificar solo la luminosidad, manteniendo el tono y la saturación originales, lo que proporciona una indicación visual clara de interactividad.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:**
    -   Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere:
        -   Un componente `Image` en el mismo GameObject para modificar su apariencia visual.
        -   Un script `HandCard` en un GameObject padre para obtener la información de la carta y su estado de interactividad.
-   **Eventos (Entrada):**
    -   Este script no se suscribe explícitamente a ningún evento de Unity o C# (`UnityEvent`, `Action`). En su lugar, obtiene el estado de la carta directamente a través de llamadas a métodos (`GetCard()`, `GetElement()`, `isClickable()`) y la lectura de propiedades (`picker`) del objeto `HandCard` en cada `Update`.
-   **Eventos (Salida):**
    -   Este script no invoca ningún evento de Unity o C# para notificar a otros sistemas. Su efecto se limita a modificar las propiedades visuales de un componente `Image` adjunto.
# CardValue
Este script, `CardValue`, se encarga de visualizar el valor numérico de una carta en una interfaz de usuario, específicamente utilizando un componente `TextMeshProUGUI`. Su función principal es asegurar que el texto mostrado en un objeto de UI siempre refleje el valor actual de la carta a la que está asociado, o que permanezca vacío si no hay ninguna carta asignada. Es un componente esencial para la representación visual de las propiedades de las cartas dentro del juego "Beast Card Clash", donde la estrategia elemental a menudo depende de los valores numéricos de las cartas.

**Funcionamiento:**
`CardValue` opera buscando dos componentes clave al inicio del juego: un script `HandCard` en su jerarquía padre y un `TextMeshProUGUI` en su propio GameObject. Una vez que obtiene estas referencias, en cada frame, verifica si la `HandCard` tiene una carta activa. Si la tiene, extrae el valor de esa carta a través de su método `GetValue()` y lo convierte a texto para actualizar el `TextMeshProUGUI`. Si la `HandCard` no contiene ninguna carta (por ejemplo, si la ranura de la mano está vacía o la carta ha sido jugada), el texto del `TextMeshProUGUI` se limpia, mostrando un espacio en blanco. Esto garantiza una representación dinámica y precisa del estado de las cartas en la mano del jugador, contribuyendo a una buena experiencia de desarrollo al encapsular la lógica de actualización del valor.

# Métodos

## Métodos de Unity

### Start
Este método se ejecuta una única vez al inicio del ciclo de vida del script, justo antes de la primera actualización de `Update`. Su propósito fundamental es inicializar las referencias a otros componentes necesarios para el funcionamiento de `CardValue`.

```csharp
void Start()
{
    card = GetComponentInParent<HandCard>();
    textMeshPro = GetComponent<TextMeshProUGUI>();
}
```

-   **`card = GetComponentInParent<HandCard>();`**: Aquí, el script busca y asigna una referencia al componente `HandCard` que se encuentre en alguno de los GameObjects padres de este `GameObject` actual. Esto establece una dependencia jerárquica clara, donde el `GameObject` con `CardValue` debe ser un hijo (directo o indirecto) de un `GameObject` que gestiona la lógica de la carta en la mano (la `HandCard`). Esta es una interacción clave que permite a `CardValue` acceder a los datos de la carta real.

    > [!NOTE] Estructura de componentes
    > La utilización de `GetComponentInParent<HandCard>()` implica que el GameObject con el script `CardValue` debe ser un hijo (directo o indirecto) de un GameObject que contenga el script `HandCard`. Esto establece una relación jerárquica clara para la gestión de las cartas en la mano dentro de la interfaz.

-   **`textMeshPro = GetComponent<TextMeshProUGUI>();`**: Busca y asigna una referencia al componente `TextMeshProUGUI` que esté adjunto al mismo `GameObject` que este script `CardValue`. Este `TextMeshProUGUI` es el elemento de UI que se utilizará para mostrar el valor de la carta.

### Update
Este método se invoca en cada frame del juego y es el encargado de mantener actualizado el valor de la carta mostrado en pantalla. Su ejecución continua asegura que cualquier cambio en la carta subyacente se refleje inmediatamente en la interfaz de usuario, lo cual es crucial en un juego de cartas donde los valores pueden cambiar o las cartas pueden ser reemplazadas.

```csharp
void Update()
{
    if (card.GetCard() == null)
    {
        textMeshPro.text = "";
        return;
    }

    textMeshPro.text = card.GetCard().GetValue().ToString();
}
```

-   **`if (card.GetCard() == null)`**: La primera acción es verificar si el componente `HandCard` (obtenido en `Start`) tiene una carta real asignada en ese momento. Esto es crucial para manejar situaciones donde una ranura de la mano podría estar vacía o una carta ya fue jugada.
    -   Si no hay carta (`null`), `textMeshPro.text = "";` se ejecuta para limpiar el texto y mostrar un espacio en blanco.
    -   `return;` detiene la ejecución del resto del método `Update` para este frame, ya que no hay un valor de carta que mostrar.
-   **`textMeshPro.text = card.GetCard().GetValue().ToString();`**: Si existe una carta asignada a la `HandCard`, esta línea se encarga de:
    1.  `card.GetCard()`: Obtener la referencia al objeto `Card` real desde el script `HandCard`.
    2.  `.GetValue()`: Llamar a un método en el objeto `Card` para obtener su valor numérico.
    3.  `.ToString()`: Convertir ese valor numérico a una cadena de texto.
    4.  Asignar esta cadena al `text` del `TextMeshProUGUI` para su visualización.

## Otros métodos
El script `CardValue` no implementa métodos públicos o privados adicionales que no sean los de ciclo de vida de Unity (`Start`, `Update`). Todas las operaciones de inicialización y actualización se gestionan dentro de estos dos métodos.

## Getters y Setters
El script `CardValue` no define getters o setters públicos propios. Su funcionalidad se basa en acceder a métodos de obtención de datos de otros componentes del proyecto, lo cual es un patrón común para mantener la encapsulación de datos:

1.  **`HandCard.GetCard()`**: Este método, perteneciente al script `HandCard` (referenciado en el padre del GameObject), es utilizado por `CardValue` para obtener la instancia del objeto `Card` que está siendo gestionado en la mano del jugador. `CardValue` no modifica la carta, solo accede a ella para leer sus propiedades.
2.  **`Card.GetValue()`**: Este método, perteneciente al objeto `Card` (obtenido a través de `HandCard.GetCard()`), es invocado para recuperar el valor numérico intrínseco de la carta. Este valor es un dato fundamental de la carta que `CardValue` se encarga de visualizar en la interfaz.
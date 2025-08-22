# `CardElement.cs`

## 1. Propósito General
Este script `CardElement` es responsable de visualizar el elemento elemental de una carta dentro del juego Beast Card Clash. Se encarga de mostrar el ícono correspondiente al tipo de elemento de la carta, asegurando que el visual se actualice dinámicamente según la carta asignada a su componente `HandCard` padre.

## 2. Componentes Clave

### `CardElement`
- **Descripción:** La clase `CardElement` es un `MonoBehaviour` que se adjunta a un objeto de juego en la jerarquía de Unity. Su función principal es controlar la visualización del ícono elemental de una carta, obteniendo la información de la carta desde un componente `HandCard` ubicado en su padre o un ancestro.

- **Variables Públicas / Serializadas:**
    - `private Sprite[] elements`: Un array de `Sprite` que almacena los diferentes íconos visuales para cada tipo de elemento del juego (e.g., agua, fuego, tierra). Estos sprites se configuran directamente desde el Inspector de Unity.
    - `HandCard card`: Una referencia al componente `HandCard` que se encuentra en un objeto de juego padre. Este `HandCard` es crucial, ya que provee la instancia de la carta actual y, por ende, su tipo de elemento.
    - `Image image`: Una referencia al componente `Image` que reside en el mismo objeto de juego donde está adjunto este script `CardElement`. Es el componente visual que mostrará el ícono del elemento.

- **Métodos Principales:**
    - `void Start()`: Este método se ejecuta una vez al inicio del ciclo de vida del script. Su propósito es inicializar las referencias a otros componentes necesarios.
        - Se obtiene una referencia al componente `HandCard` buscando en los padres del objeto de juego actual. Esto asume que el `CardElement` es un hijo de un objeto que contiene el `HandCard`.
        - Se obtiene una referencia al componente `Image` que se encuentra en el mismo objeto de juego que `CardElement`.

        ```csharp
        card = GetComponentInParent<HandCard>();
        image = transform.GetComponent<Image>();
        ```

    - `void Update()`: Este método se llama una vez por cada frame. Su lógica principal es mantener actualizada la visualización del ícono del elemento de la carta.
        - Primero, verifica si la `HandCard` tiene una carta asignada (`card.GetCard() == null`). Si no hay una carta, el componente `Image` se deshabilita (`image.enabled = false`) para ocultar cualquier ícono previo y la ejecución del método termina.
        - Si hay una carta, el componente `Image` se habilita (`image.enabled = true`), y su `sprite` se actualiza. El sprite se selecciona del array `elements` utilizando el valor del elemento de la carta (`card.GetCard().GetElement()`) como índice, el cual se convierte a un entero. Esto implica que los valores de los elementos (probablemente un `enum`) corresponden directamente a los índices de los sprites en el array.

        ```csharp
        if (card.GetCard() == null)
        {
            image.enabled = false;
            return;
        }

        image.enabled = true;
        image.sprite = elements[(int)card.GetCard().GetElement()];
        ```

- **Lógica Clave:**
    La lógica central del script reside en el método `Update`, que actúa como un bucle de renderizado para el elemento de la carta. Constantemente verifica el estado de la `HandCard` a la que está asociada. Si la `HandCard` no contiene una carta activa (por ejemplo, cuando no hay una carta en la mano del jugador), el ícono del elemento se oculta. Por el contrario, si hay una carta presente, el script recupera su tipo de elemento y lo utiliza para seleccionar el sprite de ícono correcto de su colección predefinida, asegurando que la representación visual sea siempre coherente con los datos de la carta.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere que un componente `Image` esté presente en el mismo GameObject para poder visualizar el ícono del elemento. Adicionalmente, requiere que un componente `HandCard` esté presente en un GameObject padre para poder obtener la información de la carta.

- **Eventos (Entrada):** Este script no se suscribe explícitamente a ningún evento de Unity (`UnityEvent`) o C# (`Action`). Su actualización se basa en el ciclo de vida de `MonoBehaviour` a través del método `Update`, que se ejecuta continuamente.

- **Eventos (Salida):** El script `CardElement` no invoca ningún evento para notificar a otros sistemas o scripts sobre cambios en su estado o en la carta que está visualizando. Su rol es puramente de visualización y reacción a la información que obtiene de la `HandCard` padre.
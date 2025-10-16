# `Card`
`Card` es un componente `MonoBehaviour` que representa una única carta coleccionable en el juego `Beast Card Clash`. Este script es fundamental para definir las propiedades básicas de cada carta, incluyendo su elemento (`element`), su valor numérico (`value`) y un identificador único (`identifier`). Su propósito principal es encapsular los datos de una carta y proporcionar métodos para acceder a ellos, permitiendo que otras partes del sistema de juego interactúen con las características de cada carta. También incluye un `indexer`, un índice numérico que puede ser utilizado para ordenar o referenciar la carta en colecciones.

El atributo `[DefaultExecutionOrder(-5)]` indica que la inicialización de este script se ejecuta muy temprano en el ciclo de vida de Unity, incluso antes que la mayoría de los demás scripts. Esto asegura que cualquier componente que dependa de la información de las cartas (si se inicializan en `Awake` o `OnEnable`) tenga acceso a sus propiedades desde el principio.

Las cartas en el juego se caracterizan por:
-   Un `Elemento` (como Fuego, Tierra, Agua, Aire), que probablemente influye en las interacciones de combate o estrategia.
-   Un `Valor` numérico, que podría representar poder de ataque, defensa o algún otro atributo clave.
-   Un `Identificador` único, generado a partir de su valor y elemento, que facilita la referencia y gestión de las cartas.
-   Un `Indexer`, un índice numérico que podría usarse para ordenamiento o referencia interna en listas.

Este script está diseñado para ser flexible en su inicialización, permitiendo la asignación de propiedades de la carta a través de un método `Initialize` que recibe un valor de una enumeración externa (`DeckCardsList`). Esto sugiere un sistema donde las cartas predefinidas son cargadas o generadas a partir de una lista maestra. La presencia de un método `Start` comentado que generaba valores aleatorios indica que el sistema de inicialización pudo haber evolucionado desde una aproximación más aleatoria a una más estructurada, adaptándose a las necesidades de un proyecto indie.

# Métodos

## Métodos de Unity

### `Start()`
Este método se encuentra actualmente comentado en el script:
```csharp
// private void Start()
// {
//     // Obtiene los valores de la carta de forma aleatoria
//     value = UnityEngine.Random.Range(1, 11);
//     element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length);

//     // El identificador es la concatenación del valor y del elemento, en un string
//     identifier = value.ToString() + element.ToString();
// }
```
Si estuviera activo, el método `Start()` sería llamado una vez al inicio del ciclo de vida del objeto `Card`. Su función sería inicializar los valores de la carta (`value` y `element`) de forma aleatoria.
-   `value` se asignaría un número entero entre 1 y 10 (ambos inclusive).
-   `element` se asignaría a uno de los elementos disponibles en la enumeración `Element` de forma aleatoria.
-   Finalmente, el `identifier` de la carta se construiría concatenando el valor numérico y el nombre del elemento como cadenas de texto.

El hecho de que esté comentado sugiere que el proceso de inicialización de las cartas ha sido centralizado o modificado para utilizar el método `Initialize(DeckCardsList card)`. Esto permite un control más específico sobre las propiedades de las cartas, en lugar de una generación completamente aleatoria al inicio del juego, lo cual es común en proyectos donde la definición de cartas es más estática o basada en configuraciones predefinidas.

## Otros métodos

### `Initialize(DeckCardsList card)`
```csharp
public void Initialize(DeckCardsList card)
{
    // Asigna el valor numérico directo de la carta
    value = (int)card;

    // Extrae y asigna el elemento en función de como inicia su nombre
    string cardName = card.ToString();

    if (cardName.StartsWith("Fire")) { element = Element.Fire; }
    else if (cardName.StartsWith("Earth")) { element = Element.Earth; }
    else if (cardName.StartsWith("Water")) { element = Element.Water; }
    else if (cardName.StartsWith("Air")) { element = Element.Air; }

    // El identificador es la concatenación del valor y del elemento, en un string
    identifier = value.ToString() + element.ToString();
}
```
Este método público es el encargado de configurar las propiedades de la carta de forma programática. Recibe un parámetro `card` del tipo `DeckCardsList`, que se asume es una enumeración externa que lista todas las cartas posibles en el mazo.

El funcionamiento es el siguiente:
1.  **Asignación del valor:** El `value` numérico de la carta se establece directamente mediante la conversión explícita del valor del enumerador `DeckCardsList` a un entero (`(int)card`). Esto implica que los valores numéricos asignados a los miembros de la enumeración `DeckCardsList` corresponden directamente al valor que la carta debe tener en el juego. Por ejemplo, si `DeckCardsList.FireAxe` tiene el valor `5`, entonces `value` será `5`.
2.  **Extracción del elemento:** El elemento (`element`) de la carta se determina analizando el nombre de la entrada de la enumeración `DeckCardsList`. Se convierte el valor del enumerador a su representación de cadena (`card.ToString()`) y se verifica si la cadena resultante comienza con "Fire", "Earth", "Water" o "Air". Según la coincidencia, se asigna el `Element` correspondiente. Este enfoque permite que una única enumeración `DeckCardsList` codifique tanto el valor como el tipo elemental de una carta a través de su nombre (ej., "FireAxe" para el elemento `Fire`).
3.  **Generación del identificador:** Finalmente, el `identifier` de la carta se construye concatenando el `value` numérico (convertido a string) y el `element` (también convertido a string). Este identificador sirve como una clave única para referenciar y diferenciar la carta en el sistema de juego (ej., "5Fire").

Este método es crucial para la creación y gestión de las cartas del juego, permitiendo una inicialización controlada basada en una fuente de datos predefinida (la enumeración `DeckCardsList`).

## Getters y Setters

1.  `GetValue()`: Obtiene el valor numérico (`int`) de la carta.
2.  `GetElement()`: Obtiene el tipo de elemento (`Element`) de la carta.
3.  `GetID()`: Obtiene el identificador único (`string`) de la carta.
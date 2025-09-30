# Card
Este script `Card` es un componente fundamental en el desarrollo de "Beast Card Clash", representando una carta individual dentro del juego. Su propósito principal es encapsular y gestionar los atributos básicos de una carta, como su elemento (energía elemental), su valor numérico y un identificador único que combina estos atributos.

La inicialización de una carta es en gran parte aleatoria, asignando un valor numérico entre 1 y 10 y un elemento de forma aleatoria al momento de su creación en el método `Start`. Esta aleatoriedad permite una generación dinámica y variada de cartas, lo cual es coherente con el enfoque de un juego de cartas coleccionables donde la diversidad es clave y se alinea con la filosofía del proyecto de fomentar una buena experiencia de desarrollo.

El componente está configurado para ejecutarse muy temprano en el ciclo de vida de Unity, gracias al atributo `[DefaultExecutionOrder(-5)]`. Esto asegura que sus atributos estén inicializados y listos antes de que la mayoría de los otros scripts de la escena puedan intentar acceder a ellos, lo cual es crucial en un sistema donde las propiedades de las cartas son consultadas tempranamente por otros componentes del juego. Además de sus valores aleatorios, la carta mantiene un `indexer` público que, aunque no se modifica directamente en este script, es probable que sea utilizado por sistemas externos (como un gestor de mazos o un sistema de interfaz de usuario) para ordenar o referenciar las cartas.

La interacción con otros componentes del proyecto se realiza a través de métodos "getter" públicos, que permiten a otros scripts consultar el `value`, `element` e `identifier` de la carta de manera segura y controlada, facilitando la implementación de la lógica del juego y las interacciones entre cartas.

# Métodos

## Métodos de Unity

### Start
El método `Start` se invoca una vez al inicio del ciclo de vida del script, justo antes de que se actualice el primer frame. Su función principal en el script `Card` es la inicialización aleatoria de los atributos principales de la carta:

1.  **Asignación de Valor:** Se asigna un valor numérico aleatorio a la variable `value`. Este valor se genera en un rango de 1 a 10 (ambos inclusive).
    ```csharp
    value = UnityEngine.Random.Range(1, 11);
    ```
2.  **Asignación de Elemento:** Se asigna un elemento aleatorio a la variable `element`. El elemento se selecciona de los valores definidos en el `enum` `Element`, garantizando que la carta pertenezca a una categoría elemental predefinida. La elección del elemento es dinámica, adaptándose al número total de elementos disponibles en el `enum`.
    ```csharp
    element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length);
    ```
3.  **Generación de Identificador:** Se crea un identificador único para la carta (`identifier`) concatenando la representación en `string` del `value` y el `element` asignados. Este identificador es útil para depuración, visualización en la interfaz de usuario o como clave para sistemas de datos.
    ```csharp
    identifier = value.ToString() + element.ToString();
    ```
La ejecución temprana de este método (garantizada por `[DefaultExecutionOrder(-5)]`) asegura que las cartas estén completamente inicializadas con sus atributos aleatorios antes de que cualquier otro script dependiente pueda intentar interactuar con ellas.

## Otros métodos

### GetValue() : int
Este método público proporciona un acceso de solo lectura al valor numérico asignado a la carta. Otros componentes del juego pueden invocar este método para consultar la potencia o el costo de la carta, lo cual es fundamental para las mecánicas de estrategia.
```csharp
public int GetValue()
{
    return value;
}
```

### GetElement() : Element
Este método público permite a otros scripts obtener el elemento (tipo elemental) al que pertenece la carta. Esto es crucial para la mecánica de "estrategia elemental y habilidades únicas" mencionada en el `README.md`, donde las interacciones entre cartas pueden depender fuertemente de sus elementos.
```csharp
public Element GetElement()
{
    return element;
}
```

### GetID() : string
Este método público devuelve el identificador único de la carta, que es una combinación del valor y el elemento. Este `string` puede ser utilizado para mostrar información en la interfaz de usuario, para el seguimiento de cartas en el log del juego, o como una clave para sistemas de gestión de activos o datos, facilitando la identificación de cada carta en el sistema.
```csharp
public string GetID()
{
    return identifier;
}
```

## Getters y Setters

1.  GetValue : int: Obtiene el valor numérico (tipo `int`) de la carta.
2.  GetElement : Element: Obtiene el elemento (tipo `Element`) asignado a la carta.
3.  GetID : string: Obtiene el identificador único (tipo `string`) de la carta.
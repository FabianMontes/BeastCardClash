# `Card.cs`

## 1. Propósito General
Este script define la estructura y el comportamiento fundamental de una carta individual en el juego "Beast Card Clash". Gestiona sus propiedades clave como el elemento, el valor y un identificador único, inicializando estos atributos de forma aleatoria al inicio del juego.

## 2. Componentes Clave

### `Card`
- **Descripción:** `Card` es una clase `MonoBehaviour` que representa la base de una carta coleccionable en el juego. Su propósito principal es encapsular los datos esenciales de una carta, como su tipo elemental (`Element`), su valor numérico y un identificador único. Al heredar de `MonoBehaviour`, puede ser adjuntada a un GameObject en la jerarquía de Unity, permitiendo que cada instancia de carta sea una entidad visual y funcional dentro de la escena. La directiva `[DefaultExecutionOrder(-5)]` indica que este script se ejecutará muy temprano en el ciclo de vida de Unity, antes que la mayoría de los otros scripts, lo que puede ser útil para asegurar que las propiedades de la carta estén listas rápidamente.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] private Element element;`: Define el tipo elemental de la carta (ej., Fuego, Agua). Es una variable privada que se hace visible y editable en el Inspector de Unity gracias a `[SerializeField]`. Su valor es asignado aleatoriamente durante la inicialización. La enumeración `Element` se asume definida en otra parte del proyecto.
    - `[SerializeField] private int value;`: Representa el valor numérico o la "fuerza" de la carta. Al igual que `element`, es privada pero visible en el Inspector y se inicializa aleatoriamente con un valor entre 1 y 10.
    - `[SerializeField] public int indexer = 0;`: Un entero público y serializado que puede usarse para ordenar o indexar las cartas dentro de una colección o mano. Es accesible desde otros scripts y su valor por defecto es 0.
    - `public string identifier;`: Una cadena pública que almacena un identificador único para la carta, generado concatenando su `value` y su `element`.

- **Métodos Principales:**
    - `private void Start()`: Este es un método del ciclo de vida de Unity, invocado una vez al comienzo del juego, justo antes del primer frame. Su rol es crucial para la inicialización de las propiedades de la carta:
        - Asigna un `value` aleatorio entre 1 y 10 (inclusive).
        - Asigna un `element` aleatorio de la enumeración `Element`. La forma en que se selecciona el elemento (`Enum.GetNames(typeof(Element)).Length`) implica que la enumeración `Element` contiene los diferentes tipos de elementos disponibles.
        - Construye el `identifier` de la carta uniendo el valor numérico con el nombre del elemento.
        ```csharp
        value = UnityEngine.Random.Range(1, 11); // Asigna un valor aleatorio entre 1 y 10
        element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length); // Asigna un elemento aleatorio
        identifier = value.ToString() + element.ToString(); // Genera el ID único
        ```
    - `public int GetValue()`: Un método "getter" que proporciona acceso de solo lectura al valor numérico (`value`) de la carta.
    - `public Element GetElement()`: Un método "getter" que proporciona acceso de solo lectura al tipo elemental (`element`) de la carta.
    - `public string GetID()`: Un método "getter" que proporciona acceso de solo lectura al identificador único (`identifier`) de la carta.
    - `void Update()`: Este es otro método del ciclo de vida de Unity, llamado una vez por cada frame. En el código actual, está vacío, lo que indica que esta clase `Card` no implementa ninguna lógica que requiera actualización constante en cada fotograma.

- **Lógica Clave:**
    La lógica central de la clase `Card` reside en su fase de inicialización. Al usar el método `Start()`, cada instancia de `Card` genera de forma autónoma sus características fundamentales (`value` y `element`) de manera aleatoria. Esta aleatoriedad es vital para la diversidad de cartas en el juego. Una vez inicializadas, estas propiedades son accesibles a través de los métodos "getter" correspondientes, permitiendo que otros sistemas del juego (como el sistema de combate o de reglas) consulten la información de la carta de forma controlada.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, lo que significa que no fuerza la presencia de otros componentes en el mismo GameObject para funcionar correctamente.
- **Eventos (Entrada):** La clase `Card` no se suscribe a eventos externos (como eventos de UI o notificaciones de otros scripts). Su comportamiento inicial se activa únicamente por el método `Start()` del ciclo de vida de Unity.
- **Eventos (Salida):** Este script no emite ni invoca ningún tipo de evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios en su estado o acciones. Su función es principalmente almacenar y proporcionar datos.
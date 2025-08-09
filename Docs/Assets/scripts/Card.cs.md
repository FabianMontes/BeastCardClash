# `Card.cs`

## 1. Propósito General
Este script `Card` es un componente de Unity fundamental que representa y gestiona las propiedades base de una carta individual dentro del juego "Beast Card Clash". Su rol principal es almacenar y exponer los atributos clave de una carta, como su valor numérico y su tipo elemental, además de generar un identificador único para cada instancia.

## 2. Componentes Clave

### `Card`
- **Descripción:** La clase `Card` hereda de `MonoBehaviour`, lo que significa que es un componente que puede adjuntarse a un GameObject en la escena de Unity. Actúa como el contenedor de datos para una carta, inicializando sus propiedades elementales y de valor de forma aleatoria al inicio del juego y proveyendo métodos para acceder a ellas. El atributo `[DefaultExecutionOrder(-5)]` asegura que su método `Start` se ejecute muy temprano en el ciclo de vida de Unity, antes que la mayoría de otros scripts.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] private Element element;`: Define el tipo elemental de la carta. Su valor se inicializa aleatoriamente al inicio del juego. Es importante notar que el `enum Element` no está definido en este archivo, lo que indica que es una dependencia externa (presumiblemente definida en otro script o ensamblado).
    - `[SerializeField] private int value;`: Representa el valor numérico de la carta. También se inicializa aleatoriamente al inicio del juego.
    - `[SerializeField] public int indexer = 0;`: Una variable de tipo entero que es pública y serializada. Esto permite su visibilidad y modificación desde el Inspector de Unity, así como su acceso desde otros scripts. Su propósito específico no se detalla en este código, pero sugiere que podría usarse para identificar la posición o el orden de la carta en alguna colección o lista.
    - `public string identifier;`: Una cadena de texto pública que se genera combinando el `value` y el `element` de la carta. Sirve como un identificador único para cada instancia de carta.

- **Métodos Principales:**
    - `private void Start()`: Este es un método del ciclo de vida de Unity, invocado una vez al principio del juego, justo antes de la primera actualización del frame. En `Card`, este método es crucial porque es donde las propiedades `value` y `element` de la carta se asignan aleatoriamente. El `value` se establece en un rango de 1 a 10 (exclusivo del 11), y el `element` se elige aleatoriamente de los valores disponibles en el `enum Element`. Finalmente, el `identifier` de la carta se construye concatenando estos valores aleatorios.
        ```csharp
        private void Start()
        {
            value = UnityEngine.Random.Range(1, 11);
            element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length);
            identifier = value.ToString() + element.ToString();
        }
        ```
    - `public int GetValue()`: Un método público sencillo que devuelve el valor numérico (`int`) actual de la carta.
    - `public Element GetElement()`: Un método público que devuelve el tipo elemental (`Element`) actual de la carta.
    - `public string GetID()`: Un método público que devuelve el identificador único (`string`) de la carta.
    - `void Update()`: Este es otro método del ciclo de vida de Unity, que se llama una vez por frame. En el código actual, está vacío, lo que indica que no hay lógica de actualización continua implementada para la carta en este script.

- **Lógica Clave:** La lógica central de la clase `Card` reside en su método `Start`. Esta sección se encarga de la inicialización automática de las propiedades `value` y `element` de manera aleatoria, utilizando el generador de números aleatorios de Unity y basándose en los valores definidos en el `enum Element`. Esta aleatorización es fundamental para la variabilidad de las cartas en el juego. La posterior generación del `identifier` a partir de estos valores permite una referencia rápida y única a cada instancia de carta.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, su funcionalidad depende de la existencia de un `enum Element` que no está definido en este mismo archivo, implicando una dependencia de otro archivo de código en el proyecto. El atributo `[DefaultExecutionOrder(-5)]` ajusta su prioridad de ejecución en Unity.
- **Eventos (Entrada):** El script `Card` no se suscribe a ningún evento de Unity o del juego en el código proporcionado. Su comportamiento inicial se desencadena por el método `Start` del ciclo de vida de Unity, y su funcionalidad posterior se basa en las llamadas directas a sus métodos `Get`.
- **Eventos (Salida):** Este script no invoca ni emite ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios en sus propiedades o la realización de acciones. Funciona principalmente como un contenedor de datos con capacidades de auto-inicialización.
# `Card.cs`

## 1. Propósito General
Este script `Card.cs` actúa como el modelo de datos fundamental para una carta individual dentro del juego "Beast Card Clash". Su propósito principal es encapsular y gestionar las propiedades intrínsecas de una carta, como su elemento, valor numérico e identificador único, inicializando estos atributos de forma aleatoria al inicio del juego.

## 2. Componentes Clave

### `Card`
- **Descripción:** La clase `Card` hereda de `MonoBehaviour`, lo que significa que puede ser adjuntada a un objeto de juego (GameObject) en Unity y participar en el ciclo de vida del motor. Representa la información base y el comportamiento de una sola carta en el juego, incluyendo su tipo elemental, un valor numérico asociado y un identificador único. El atributo `[DefaultExecutionOrder(-5)]` indica que el método `Start` de este script se ejecutará antes que la mayoría de los demás scripts en la escena.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] private Element element;`: Esta variable de tipo `Element` (un tipo de dato que se asume está definido en otra parte del proyecto, probablemente un `enum`) almacena el tipo elemental de la carta. Al ser `[SerializeField]`, es editable desde el Inspector de Unity a pesar de ser privada, facilitando la depuración y configuración inicial.
    - `[SerializeField] private int value;`: Un entero que representa el valor numérico de la carta. También es editable en el Inspector.
    - `[SerializeField] public int indexer = 0;`: Una variable pública de tipo entero, también serializada, que puede ser utilizada para indexar o identificar la posición de la carta dentro de una colección o un arreglo. Se inicializa a `0`.
    - `public string identifier;`: Una cadena de texto pública que almacena un identificador único para cada instancia de la carta.

- **Métodos Principales:**
    - `private void Start()`: Este es un método del ciclo de vida de Unity, invocado una vez al comienzo de la vida del script, justo antes de que se actualice por primera vez. Su función clave es inicializar aleatoriamente las propiedades de la carta:
        - Asigna un valor numérico aleatorio entre 1 y 10 (inclusive) a la variable `value`.
        - Asigna un tipo elemental aleatorio a la variable `element`, seleccionándolo de los valores definidos en el `enum Element`. Esto implica una dependencia con el `enum Element` que debe estar definido en el proyecto.
        - Construye el `identifier` de la carta concatenando su `value` numérico y el `element` asignado.
        ```csharp
        value = UnityEngine.Random.Range(1, 11);
        element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length);
        identifier = value.ToString() + element.ToString();
        ```
    - `public int GetValue()`: Un método público que devuelve el valor numérico (`int`) actual de la carta.
    - `public Element GetElement()`: Un método público que devuelve el tipo elemental (`Element`) actual de la carta.
    - `public string GetID()`: Un método público que devuelve el identificador (`string`) único de la carta.
    - `void Update()`: Otro método del ciclo de vida de Unity que se invoca una vez por cada fotograma. En este script, el método está vacío, lo que indica que no hay lógica de actualización continua implementada para la carta en este componente.

- **Lógica Clave:**
    La lógica central de este script reside en el método `Start()`, donde se generan aleatoriamente los atributos principales de la carta (`value` y `element`). Esta inicialización automática garantiza que cada instancia de `Card` creada en el juego (o en la escena) tendrá propiedades distintas y aleatorias, simulando la diversidad de cartas coleccionables. El `identifier` generado a partir de estos atributos aleatorios provee una forma sencilla de referenciar o depurar cartas específicas.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, por lo que no impone la presencia de otros componentes en el mismo GameObject para funcionar.
- **Eventos (Entrada):** Este script no se suscribe explícitamente a ningún evento de usuario (como clics de botón) o eventos de otros sistemas. Su activación principal es a través del ciclo de vida de Unity (`Start`, `Update`).
- **Eventos (Salida):** Este script no invoca ningún `UnityEvent`, `Action` o patrón de observador para notificar a otros sistemas sobre cambios o acciones relacionadas con la carta. Funciona principalmente como un contenedor de datos y un inicializador.
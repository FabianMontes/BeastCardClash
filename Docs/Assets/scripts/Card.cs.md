# `Card.cs`

## 1. Propósito General

El script `Card.cs` es un componente fundamental de Unity que gestiona los datos principales y la inicialización de una carta individual en el juego "Beast Card Clash". Su rol principal es definir las propiedades intrínsecas de una carta, como su valor numérico y tipo elemental, y generar un identificador único. No interactúa directamente con sistemas de combate o UI, sino que proporciona los datos base que otros sistemas de juego consumirán.

## 2. Componentes Clave

### `Card`

La clase `Card` es un `MonoBehaviour`, lo que significa que debe adjuntarse a un `GameObject` en una escena de Unity para funcionar. Esta clase encapsula la identidad y las propiedades básicas de una carta, encargándose de su configuración inicial de forma autónoma.

*   **Variables Públicas / Serializadas:**
    *   `[SerializeField] private Element element;`: Esta variable privada, visible en el Inspector de Unity, define el tipo elemental de la carta (e.g., Fuego, Agua, Tierra). El `enum Element` debe estar definido en otra parte del proyecto. Su valor es asignado aleatoriamente durante la inicialización de la carta.
    *   `[SerializeField] private int value;`: También privada y visible en el Inspector, esta variable almacena el valor numérico de la carta. Este valor es aleatorio y se establece entre 1 y 10 al inicio del juego, lo que probablemente impacta la fuerza o las habilidades de la carta.
    *   `[SerializeField] public int indexer = 0;`: Esta variable pública y serializada permite asignar un índice a la carta. Aunque se inicializa en 0, su propósito es probablemente para ordenar o identificar la carta dentro de una colección o mano, y puede ser configurada manualmente en el Inspector o por otros scripts.
    *   `public string identifier;`: Una cadena de texto pública que sirve como identificador único para cada instancia de carta. Se genera combinando el `value` y el `element` de la carta, asegurando que cada carta tenga una "firma" distintiva.

*   **Métodos Principales:**
    *   `private void Start()`: Este es un método del ciclo de vida de Unity que se invoca una única vez al inicio, cuando el script es habilitado por primera vez. Su función crucial es inicializar las propiedades aleatorias de la carta:
        *   `value` se establece como un número entero aleatorio entre 1 y 10 (incluidos).
        *   `element` se asigna a uno de los tipos definidos en el `enum Element` de forma aleatoria.
        *   `identifier` se construye concatenando la representación en cadena del `value` y el `element` recién asignados.
        Este método garantiza que cada carta creada en la escena tenga características únicas y predefinidas desde el principio.

        ```csharp
        value = UnityEngine.Random.Range(1, 11);
        element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length);
        identifier = value.ToString() + element.ToString();
        ```
    *   `public int GetValue()`: Este método público proporciona acceso de solo lectura al valor numérico (`value`) de la carta. Es la forma principal en que otros scripts pueden consultar la "fuerza" o el número asociado a esta carta.
    *   `public Element GetElement()`: De manera similar, este método público permite a otros sistemas obtener el tipo elemental (`element`) de la carta. Esto es vital para la lógica de juego que depende de las interacciones elementales.
    *   `public string GetID()`: Este método público devuelve el `identifier` único de la carta, que puede ser útil para depuración, registro o para identificar específicamente una carta en colecciones.
    *   `void Update()`: Este es un método del ciclo de vida de Unity que se ejecuta en cada frame. En el código actual, está vacío, lo que indica que la clase `Card` no requiere lógica de actualización continua después de su inicialización.

*   **Lógica Clave:**
    La lógica central de la clase `Card` reside en su inicialización en el método `Start()`. Al asignar `value` y `element` de manera aleatoria y luego generar un `identifier` a partir de ellos, la clase garantiza que cada instancia de `Card` creada en el juego sea única en sus propiedades fundamentales desde el momento en que se carga la escena. Esto establece la base para la diversidad de cartas en el juego sin requerir configuración manual compleja para cada una.

## 3. Dependencias y Eventos

*   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, lo que significa que no fuerza la adición de otros componentes a su `GameObject`. Sin embargo, al ser un `MonoBehaviour`, necesita un `GameObject` padre para existir en la jerarquía de Unity.
*   **Eventos (Entrada):** El script `Card` no se suscribe a ningún evento externo (como clics de botones, cambios de estado de otros objetos, etc.). Su comportamiento principal se desencadena exclusivamente por el ciclo de vida de Unity (específicamente, el método `Start`).
*   **Eventos (Salida):** Este script no invoca ni expone ningún evento (como `UnityEvent` o `Action`) para notificar a otros sistemas sobre cambios en sus propiedades o estados. Su función es principalmente de proveedor de datos, y los otros sistemas deben consultar sus propiedades a través de los métodos "getter" públicos (`GetValue`, `GetElement`, `GetID`).
*   **Orden de Ejecución:** La directiva `[DefaultExecutionOrder(-5)]` es importante. Indica que este script se ejecutará muy temprano en el orden de ejecución de scripts de Unity, incluso antes que la mayoría de los scripts predeterminados. Esto podría ser relevante si otros sistemas o scripts necesitan acceder a los valores inicializados de las cartas en un momento muy temprano del ciclo de vida de una escena.
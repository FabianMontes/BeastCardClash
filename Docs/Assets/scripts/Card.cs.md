Aquí tienes la documentación técnica para el archivo `Card.cs`, diseñada para un nuevo miembro del equipo:

---

# `Card.cs`

## 1. Propósito General
Este script define y gestiona los atributos fundamentales de una carta individual en el juego "Beast Card Clash". Actúa principalmente como un contenedor de datos para las propiedades intrínsecas de una carta, como su valor numérico, su elemento y un identificador único, inicializándolos de manera aleatoria al inicio.

## 2. Componentes Clave

### `Card`
- **Descripción:** La clase `Card` es un `MonoBehaviour`, lo que significa que puede ser adjuntada a un GameObject en la jerarquía de Unity y sus métodos de ciclo de vida serán ejecutados por el motor. Su rol principal es encapsular la información esencial de una carta individual en el juego, permitiendo que otras lógicas accedan a sus propiedades. El atributo `[DefaultExecutionOrder(-5)]` asegura que los métodos de esta clase, como `Start()`, se ejecuten muy temprano en el orden de ejecución de scripts de Unity, incluso antes que la mayoría de otros scripts.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] private Element element;`: Define el tipo elemental de la carta. Esta variable es privada pero serializable, lo que permite su asignación o visualización en el Inspector de Unity. El tipo `Element` se asume como una enumeración (enum) definida en otro lugar del proyecto.
    - `[SerializeField] private int value;`: Representa el valor numérico o la "fuerza" de la carta. Al igual que `element`, es privada y serializable.
    - `[SerializeField] public int indexer = 0;`: Un entero público y serializable, inicializado en 0. Podría usarse para mantener un índice o una posición relativa de la carta, por ejemplo, dentro de una mano o un mazo.
    - `public string identifier;`: Una cadena de texto pública que actúa como un identificador único para la carta, generado a partir de su `value` y `element`.

- **Métodos Principales:**
    - `private void Start()`: Este es un método del ciclo de vida de Unity, invocado una vez al comienzo cuando el script está habilitado. Su función principal es inicializar los atributos de la carta:
        - Asigna un `value` aleatorio entre 1 y 10 (inclusive).
        - Asigna un `element` aleatorio de entre todos los elementos definidos en la enumeración `Element`.
        - Construye el `identifier` concatenando el `value` y el `element` como cadenas.
    - `public int GetValue()`: Un método getter que devuelve el valor numérico (`int`) actual de la carta.
    - `public Element GetElement()`: Un método getter que devuelve el tipo elemental (`Element`) actual de la carta.
    - `public string GetID()`: Un método getter que devuelve el identificador único (`string`) de la carta.
    - `void Update()`: Este es otro método del ciclo de vida de Unity, invocado una vez por cada frame. En el código actual, está vacío, lo que indica que no hay lógica que deba ejecutarse continuamente frame a frame para las propiedades básicas de la carta.

- **Lógica Clave:**
    La lógica central de este script reside en el método `Start()`. Aquí, cada instancia de `Card` es auto-inicializada con propiedades `value` y `element` aleatorias. Esta aleatoriedad en la inicialización es fundamental para la variabilidad del juego, asegurando que cada carta creada tenga atributos distintos sin necesidad de configuración manual. El `identifier` generado es crucial para referenciar o diferenciar cartas de manera programática.

```csharp
    private void Start()
    {
        value = UnityEngine.Random.Range(1, 11); // Valor aleatorio entre 1 y 10
        element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length); // Elemento aleatorio
        identifier = value.ToString() + element.ToString(); // Generación del ID
    }
```

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, por lo que no impone la presencia de otros componentes en el mismo GameObject.
- **Eventos (Entrada):** Este script no se suscribe a ningún evento externo (como `button.onClick` o eventos personalizados) en el código proporcionado. Su inicialización ocurre a través del método de ciclo de vida `Start()` de Unity.
- **Eventos (Salida):** Este script no invoca explícitamente ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas. Sus datos son accesibles a través de sus métodos getter públicos.

---
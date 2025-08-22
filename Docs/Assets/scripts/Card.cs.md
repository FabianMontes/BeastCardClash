# `Card.cs`

## 1. Propósito General
Este script define las propiedades fundamentales de una carta dentro del juego "Beast Card Clash". Gestiona los valores esenciales de una carta, como su elemento y su valor numérico, y se encarga de su inicialización al inicio del juego.

## 2. Componentes Clave

### `Card`
-   **Descripción:** La clase `Card` hereda de `MonoBehaviour`, lo que le permite ser un componente adjunto a GameObjects en Unity. Su propósito principal es encapsular la información intrínseca de una carta del juego, como su tipo elemental, su poder numérico y un identificador único. Además, se encarga de inicializar estas propiedades de forma aleatoria cuando el juego comienza.
    El atributo `[DefaultExecutionOrder(-5)]` asegura que este script se ejecute muy temprano en el ciclo de vida de los scripts de Unity, incluso antes que la mayoría de los demás componentes. Esto es útil para garantizar que las propiedades de la carta estén definidas antes de que otros sistemas intenten acceder a ellas.

-   **Variables Públicas / Serializadas:**
    *   `[SerializeField] private Element element;`: Define el tipo elemental de la carta (e.g., Fuego, Agua, Tierra). Al ser `private` con `[SerializeField]`, es editable desde el Inspector de Unity sin ser directamente accesible desde otros scripts, salvo a través de métodos públicos. La definición del `enum Element` no está incluida en este archivo y se asume que existe en otro lugar del proyecto.
    *   `[SerializeField] private int value;`: Representa el valor numérico o la fuerza de la carta. También es editable en el Inspector y su valor se inicializa aleatoriamente en tiempo de ejecución.
    *   `[SerializeField] public int indexer = 0;`: Un índice entero para la carta. Es público y serializado, lo que significa que es accesible desde otros scripts y visible/editable en el Inspector. Su uso exacto podría depender de sistemas externos (e.g., posición en una mano o en el tablero).
    *   `public string identifier;`: Un identificador único de tipo cadena para la carta, generado dinámicamente al concatenar su valor y su elemento. Es público para facilitar su acceso desde otros componentes.

-   **Métodos Principales:**
    *   `private void Start()`: Este es un método del ciclo de vida de Unity, llamado una vez cuando el script está habilitado por primera vez en el GameObject.
        ```csharp
        private void Start()
        {
            value = UnityEngine.Random.Range(1, 11);
            element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length);
            identifier = value.ToString() + element.ToString();
        }
        ```
        Dentro de este método, se inicializan aleatoriamente el `value` de la carta (un número entre 1 y 10, ambos inclusive) y el `element` (eligiendo uno al azar del `enum Element`). Finalmente, se construye el `identifier` concatenando estos dos valores.
    *   `public int GetValue()`: Retorna el valor numérico (`int`) de la carta.
    *   `public Element GetElement()`: Retorna el elemento (`Element`) de la carta.
    *   `public string GetID()`: Retorna el identificador único (`string`) de la carta.

-   **Lógica Clave:**
    La lógica central de este script reside en el método `Start()`, donde se genera la "identidad" fundamental de la carta. El `value` se determina aleatoriamente entre 1 y 10, y el `element` se selecciona aleatoriamente de todos los posibles valores definidos en el `enum Element`. Posteriormente, se crea un `identifier` combinando estos dos valores, lo que permite una representación única y legible de la carta (ej: "5Fire", "8Water").

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, por lo que no requiere explícitamente la presencia de ningún otro componente en el mismo GameObject para funcionar.
-   **Eventos (Entrada):** Este script no se suscribe a ningún evento externo (como eventos de UI o de otros sistemas). Su lógica de inicialización ocurre de forma interna en el método `Start()` de Unity.
-   **Eventos (Salida):** Este script no invoca ni emite ningún evento (como `UnityEvent` o `Action`) para notificar a otros sistemas sobre cambios o acciones relacionadas con la carta. Proporciona sus propiedades a través de métodos `Get` públicos.
-   **Dependencias Implícitas:** Requiere la definición del `enum Element` en algún lugar accesible del proyecto para que el código compile y funcione correctamente.
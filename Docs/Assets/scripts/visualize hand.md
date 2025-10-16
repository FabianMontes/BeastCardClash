# `visualizehand`
Este script `visualizehand` tiene como función principal controlar la visibilidad y la interactividad de un elemento de la interfaz de usuario que representa la "mano" del jugador en el juego, así como las "cartas" contenidas en ella. Su comportamiento está directamente acoplado al estado de activación de otro GameObject, denominado `guide`, que actúa como un interruptor maestro para la mano.

En el contexto de "Beast Card Clash", donde se utilizan cartas coleccionables, este script es fundamental para mostrar u ocultar la mano del jugador y sus cartas según la lógica del juego (por ejemplo, durante ciertas fases del turno o cuando se muestra un menú). Al controlar los componentes `Image` y `Button` de los elementos de la interfaz de usuario, este script no solo afecta la representación visual, sino también la capacidad del jugador para interactuar con las cartas.

El script está diseñado para ejecutarse en un orden específico (`DefaultExecutionOrder(2)`), lo que sugiere que su inicialización y actualización pueden depender o ser una dependencia de otros sistemas que se establecen antes o después. En la jerarquía de Unity, se espera que este script esté adjunto a un GameObject que sea padre de la interfaz de usuario de la mano. Específicamente, el script busca la "mano" como el segundo hijo de su propio GameObject padre (`transform.GetChild(1)`), y las "cartas" como hijos directos de esta "mano".

```csharp
[DefaultExecutionOrder(2)]
public class visualizehand : MonoBehaviour
{
    [SerializeField] Transform guide;
    bool activelast;
    // ...
}
```

La variable `guide` es crucial, ya que el estado `activeSelf` de este `Transform` (es decir, si el GameObject está activo en la jerarquía) es el único factor que determina la visibilidad e interactividad de la mano y sus cartas.

# Métodos

## Métodos de Unity

### `Start()`
Este método se ejecuta una única vez al inicio, antes del primer `Update` después de que el MonoBehaviour es creado. Su propósito es inicializar la "mano" del jugador en un estado oculto.

```csharp
void Start()
{
    activelast = false;
    Transform hand = transform.GetChild(1);
    hand.GetComponent<Image>().enabled = activelast;
    for (int i = 0; i < hand.childCount; i++)
    {
        hand.GetChild(i).GetComponent<Image>().enabled = activelast;
    }
}
```

1.  **`activelast = false;`**: Se inicializa la variable de estado `activelast` a `false`. Esto asegura que la mano y sus cartas comiencen ocultas.
2.  **`Transform hand = transform.GetChild(1);`**: Se obtiene una referencia al `Transform` del objeto que representa la "mano". Se asume que este objeto es el *segundo hijo* (índice 1) del GameObject al que está adjunto el script `visualizehand`.
3.  **`hand.GetComponent<Image>().enabled = activelast;`**: Se deshabilita el componente `Image` del objeto "mano". Al deshabilitar el `Image`, el elemento visual de la mano se oculta.
4.  **Bucle `for`**: Se itera a través de todos los hijos del objeto "mano". Para cada hijo (que se asume representa una "carta"), se deshabilita su componente `Image`. Esto oculta visualmente todas las cartas en la mano.

### `Update()`
Este método se ejecuta una vez por frame y es responsable de actualizar continuamente la visibilidad y la interactividad de la mano y sus cartas basándose en el estado del `guide` GameObject.

```csharp
void Update()
{
    bool isactiv = guide.gameObject.activeSelf;

    activelast = isactiv;
    Transform hand = transform.GetChild(1);
    hand.GetComponent<Image>().enabled = activelast;
    for (int i = 0; i < hand.childCount; i++)
    {
        hand.GetChild(i).GetComponent<Image>().enabled = activelast;
        hand.GetChild(i).GetComponent<Button>().enabled = activelast;
        //hand.GetChild(i).GetChild(0).gameObject.SetActive(activelast);
    }
}
```

1.  **`bool isactiv = guide.gameObject.activeSelf;`**: Se consulta el estado de activación del GameObject al que pertenece el `guide` `Transform`. Si `guide.gameObject` está activo en la jerarquía, `isactiv` será `true`; de lo contrario, `false`. Este es el control maestro para la visibilidad de la mano.
2.  **`activelast = isactiv;`**: La variable `activelast` se actualiza con el estado actual del `guide`. Aunque no se utiliza directamente en una condición `if` para evitar procesamiento redundante en este script específico, es una buena práctica para rastrear cambios de estado.
3.  **`Transform hand = transform.GetChild(1);`**: Al igual que en `Start()`, se obtiene la referencia al `Transform` del objeto "mano".
4.  **`hand.GetComponent<Image>().enabled = activelast;`**: El componente `Image` de la mano se habilita o deshabilita en función del estado de `activelast` (que a su vez refleja el estado del `guide`). Esto controla la visibilidad de la propia "mano".
5.  **Bucle `for`**: Se itera a través de todos los hijos del objeto "mano" (las "cartas").
    *   **`hand.GetChild(i).GetComponent<Image>().enabled = activelast;`**: El componente `Image` de cada carta se habilita o deshabilita, controlando su visibilidad individual.
    *   **`hand.GetChild(i).GetComponent<Button>().enabled = activelast;`**: El componente `Button` de cada carta también se habilita o deshabilita. Esto es crucial, ya que no solo se ocultan visualmente las cartas, sino que también se desactiva su interactividad. Si el `guide` está inactivo, las cartas no se pueden clickear.
    *   **`//hand.GetChild(i).GetChild(0).gameObject.SetActive(activelast);`**: Esta línea comentada indica una funcionalidad previa o planificada. Sugiere que cada "carta" podría tener un hijo adicional (índice 0) que también debía activarse o desactivarse junto con la carta. Esto podría ser, por ejemplo, un texto, un ícono o algún efecto visual específico de la carta. La decisión de comentarla puede deberse a un cambio en el diseño, una simplificación o una refactorización.

## Otros métodos
Este script no contiene métodos personalizados definidos fuera de los métodos de ciclo de vida de Unity.

## Getters y Setters
Este script no implementa propiedades de C# con `get` o `set` explícitos.

1.  `guide`: Este campo es de tipo `Transform` y está serializado (`[SerializeField]`), lo que permite que sea asignado desde el Inspector de Unity. Representa el `Transform` de un GameObject que actúa como interruptor maestro para la visibilidad e interactividad de la mano.
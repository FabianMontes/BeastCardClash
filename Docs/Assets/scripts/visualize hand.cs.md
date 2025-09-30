# visualizehand
Este script es un componente esencial para la gestión visual y de interacción de la mano de cartas del jugador en "Beast Card Clash". Su función principal es controlar la visibilidad y la capacidad de interacción (a través de botones) de la mano de cartas y sus elementos individuales, basándose en el estado de actividad de un objeto `guide` de referencia.

El script opera bajo la premisa de una jerarquía de GameObjects específica: espera que el GameObject al que está adjunto contenga un segundo hijo (índice 1), el cual representa la "mano" (`hand`). A su vez, se espera que este objeto `hand` contenga sus propios hijos, cada uno representando una carta individual. Tanto la `hand` como cada "carta" deben tener un componente `Image`, y cada "carta" también debe tener un componente `Button`.

El comportamiento de `visualizehand` está diseñado para sincronizar la apariencia y la funcionalidad de la mano de cartas con el estado de un objeto `guide`. Por ejemplo, si `guide` representa el indicador del turno del jugador, la mano de cartas se hará visible e interactuable solo cuando sea el turno del jugador, proporcionando una clara indicación visual y control sobre el flujo del juego.

La directiva `[DefaultExecutionOrder(2)]` indica que este script se ejecutará después de la mayoría de los demás scripts que tienen el orden de ejecución predeterminado (0) o uno menor. Esto asegura que el estado del objeto `guide` ya haya sido establecido por otros sistemas antes de que `visualizehand` intente leerlo y reaccionar a él, evitando posibles problemas de sincronización en la activación de la interfaz de usuario.

```csharp
[DefaultExecutionOrder(2)]
public class visualizehand : MonoBehaviour
{
    [SerializeField] Transform guide;
    bool activelast;
    // ...
}
```
El campo `guide` debe ser asignado desde el Inspector de Unity con una referencia al objeto que actúa como el activador principal para la mano de cartas.

# Métodos

## Métodos de Unity

### Start
Este método se invoca una única vez al inicio del ciclo de vida del script, antes de la primera llamada a `Update`. Su propósito es inicializar el estado de la mano de cartas, asegurando que esté oculta e inactiva al comienzo del juego o de la escena.

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
1.  `activelast = false;`: Inicializa la variable `activelast` a `false`. Esta variable se utiliza para mantener un registro del estado de activación del `guide` en el frame anterior, aunque en `Start` simplemente se asegura un estado inicial de "inactivo".
2.  `Transform hand = transform.GetChild(1);`: Obtiene una referencia al segundo hijo del GameObject al que este script está adjunto. Este GameObject es asumido como el contenedor de la mano de cartas del jugador. La indexación directa (`GetChild(1)`) se utiliza para un acceso rápido y directo a la estructura conocida de la UI de la mano.
3.  `hand.GetComponent<Image>().enabled = activelast;`: Deshabilita el componente `Image` del GameObject `hand`. Esto hace que el contenedor visual de la mano sea invisible.
4.  El bucle `for` itera a través de cada uno de los hijos del GameObject `hand`. Se espera que cada uno de estos hijos represente una carta individual en la mano.
5.  `hand.GetChild(i).GetComponent<Image>().enabled = activelast;`: Dentro del bucle, deshabilita el componente `Image` de cada carta, haciéndolas invisibles individualmente.

### Update
Este método se invoca una vez por cada frame del juego. Su función es verificar continuamente el estado de actividad del objeto `guide` y, en consecuencia, actualizar la visibilidad y la capacidad de interacción de la mano de cartas y de cada una de las cartas que contiene.

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
1.  `bool isactiv = guide.gameObject.activeSelf;`: Comprueba si el GameObject referenciado por `guide` está actualmente activo en la jerarquía de la escena. Esta es la condición principal que determina el estado de la mano de cartas.
2.  `activelast = isactiv;`: Actualiza la variable `activelast` con el estado actual de `isactiv`. Aunque `activelast` no se usa directamente para evitar actualizaciones redundantes en este script (como en un patrón 'dirty flag'), su presencia sugiere una posible evolución futura para optimizar las actualizaciones.
3.  `Transform hand = transform.GetChild(1);`: Similar a `Start`, obtiene una referencia al GameObject que representa la mano de cartas.
4.  `hand.GetComponent<Image>().enabled = activelast;`: Habilita o deshabilita el componente `Image` del contenedor `hand` basándose en el estado de `activelast` (que es el estado actual de `guide.gameObject.activeSelf`).
5.  El bucle `for` itera a través de cada una de las cartas individuales dentro del GameObject `hand`.
6.  `hand.GetChild(i).GetComponent<Image>().enabled = activelast;`: Dentro del bucle, habilita o deshabilita el componente `Image` de cada carta, controlando su visibilidad.
7.  `hand.GetChild(i).GetComponent<Button>().enabled = activelast;`: Habilita o deshabilita el componente `Button` de cada carta. Esto es crucial para la jugabilidad, ya que determina si el jugador puede interactuar con las cartas (por ejemplo, seleccionarlas para jugar).
8.  `//hand.GetChild(i).GetChild(0).gameObject.SetActive(activelast);`: Esta línea comentada sugiere una funcionalidad previamente considerada o para futuras implementaciones, donde el primer hijo de cada carta individual también podría ser activado o desactivado. Esto podría ser útil para elementos visuales dentro de cada carta, como un marcador de estado, un efecto visual o un icono que solo debería ser visible cuando la carta está activa.

## Otros métodos
Este script no contiene métodos públicos o protegidos definidos por el desarrollador más allá de los métodos de ciclo de vida de Unity (`Start`, `Update`).

## Getters y Setters
Este script no expone propiedades públicas o métodos específicos para obtener o establecer datos internamente manejados. El campo `guide` es de tipo `[SerializeField]` y se configura directamente desde el Inspector de Unity.
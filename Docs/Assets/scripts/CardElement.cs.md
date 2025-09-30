# CardElement
`CardElement` es un componente `MonoBehaviour` fundamental en la interfaz de usuario de **Beast Card Clash**, un juego de cartas de estrategia por turnos. Su función principal es **visualizar el elemento asociado a una carta** que se está mostrando en la mano del jugador. En el contexto del juego, donde la "estrategia elemental" es un pilar, este script se encarga de que el ícono correcto del elemento (Tierra, Agua, Fuego, etc.) se muestre dinámicamente en el lugar correspondiente de la carta en la UI.

Este script espera ser un componente en un GameObject que sea hijo de otro GameObject que contenga el script `HandCard`. Además, el GameObject donde reside `CardElement` debe tener un componente `Image` adjunto, el cual será utilizado para mostrar el sprite del elemento. La asignación de los sprites de los elementos se realiza a través del Inspector de Unity mediante el array `elements`.

El funcionamiento de `CardElement` es reactivo: cada fotograma, verifica si existe una carta válida asociada al componente `HandCard` de su padre. Si la hay, actualiza la imagen para mostrar el ícono del elemento de esa carta; si no hay carta, la imagen se deshabilita, haciéndola invisible.

# Métodos

## Métodos de Unity

### Start
Este método se invoca una única vez al inicio del ciclo de vida del script, antes del primer fotograma. Su propósito es inicializar las referencias a los componentes necesarios para el correcto funcionamiento de `CardElement`.

```csharp
void Start()
{
    // Inicializa la carta y su imagen
    card = GetComponentInParent<HandCard>();
    image = transform.GetComponent<Image>();
}
```

1.  **Obtención de `HandCard`**: Busca y almacena una referencia al componente `HandCard` en el GameObject padre. Esto es crucial ya que `CardElement` no contiene directamente los datos de la carta, sino que los obtiene a través de su componente `HandCard` ascendente en la jerarquía.
2.  **Obtención de `Image`**: Busca y almacena una referencia al componente `Image` adjunto al mismo GameObject que `CardElement`. Este componente será el encargado de mostrar el sprite del elemento de la carta en la interfaz de usuario.

### Update
Este método se invoca una vez por cada fotograma del juego. Su responsabilidad principal es mantener la imagen del elemento actualizada, asegurándose de que refleje el elemento de la carta actual o se oculte si no hay ninguna carta presente.

```csharp
void Update()
{
    // Si no hay carta, deshabilita la imagen
    if (card.GetCard() == null)
    {
        image.enabled = false;
        return;
    }

    // Si hay carta, habilita la imagen y establece el ícono correspondiente
    image.enabled = true;
    image.sprite = elements[(int)card.GetCard().GetElement()];
}
```

El flujo de `Update` es el siguiente:

1.  **Verificación de la carta**: Primero, comprueba si la referencia a la carta obtenida a través de `card.GetCard()` es `null`.
    *   Si es `null`, significa que no hay una carta asignada o disponible. En este caso, la imagen del elemento (`image`) se deshabilita (`image.enabled = false`), volviéndose invisible, y el método `Update` termina su ejecución para este fotograma.
2.  **Actualización del sprite**: Si la referencia a la carta *no* es `null`:
    *   La imagen del elemento se habilita (`image.enabled = true`), asegurándose de que sea visible.
    *   El sprite de la imagen (`image.sprite`) se actualiza. Para ello, utiliza el array `elements` (que se configura en el Inspector con los íconos de los elementos). El índice del sprite se determina convirtiendo el valor del elemento de la carta a un entero. Esto implica que `card.GetCard().GetElement()` devuelve un valor (probablemente un `enum`) que puede ser directamente casteado a un `int` para usarse como índice en el array `elements`.
    > [!NOTE]
    > La forma en que `card.GetCard().GetElement()` se mapea a los índices del array `elements` es crítica y debe ser consistente. Se asume que el `enum` de elementos (o su equivalente) en el script `Card` tiene un orden que coincide con el orden de los sprites definidos en el array `elements` en el Inspector.

---
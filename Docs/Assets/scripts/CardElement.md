# `CardElement`
El script `CardElement` es el componente encargado de gestionar y mostrar visualmente el icono elemental de una carta dentro de la interfaz de usuario. Su función principal es asegurar que el `Sprite` del elemento de la carta, como Fuego, Agua, Tierra, etc., se refleje correctamente en un `Image` UI. Este script trabaja en conjunto con el componente `HandCard` (presumiblemente ubicado en un GameObject padre) para obtener los datos de la carta activa y, en base a su tipo elemental, actualiza el `Image` correspondiente con el icono apropiado de una lista predefinida de `Sprite`s. Es fundamental para comunicar visualmente a los jugadores el tipo elemental de cada carta, una mecánica estratégica clave en **Beast Card Clash**.

# Métodos

## Métodos de Unity

### `Start()`
Este método de ciclo de vida de Unity se invoca una vez al inicio, cuando la instancia del script es cargada. Su propósito fundamental es inicializar las referencias a los componentes con los que interactuará `CardElement`:
1.  Obtiene una referencia al script `HandCard`, el cual se espera que esté adjunto a un GameObject padre. Esto se logra mediante `GetComponentInParent<HandCard>()`. Este diseño implica una estructura jerárquica donde el elemento visual (controlado por `CardElement`) es un hijo del GameObject que contiene los datos y la lógica de la carta (`HandCard`).
2.  Adquiere una referencia al componente `Image` que se encuentra en el mismo GameObject que `CardElement` utilizando `transform.GetComponent<Image>()`. Este componente `Image` es donde finalmente se visualizará el icono elemental.

```csharp
void Start()
{
    card = GetComponentInParent<HandCard>();
    image = transform.GetComponent<Image>();
}
```
Esta fase de inicialización garantiza que `CardElement` disponga de acceso tanto al proveedor de datos de la carta (`HandCard`) como a su propio componente de visualización (`Image`) antes de que comience el ciclo de actualización del juego.

### `Update()`
Este método de ciclo de vida de Unity se ejecuta en cada fotograma del juego. Su objetivo es monitorear continuamente el estado de la carta y actualizar la representación visual del icono elemental en consecuencia:
1.  **Verificación de la presencia de la carta:** El método comienza comprobando si hay un objeto `Card` activo asociado al componente `HandCard`. Esto se realiza llamando a `card.GetCard()`.
    ```csharp
    if (card.GetCard() == null)
    {
        image.enabled = false;
        return;
    }
    ```
    Si `card.GetCard()` devuelve `null`, significa que no hay una carta activa actualmente (por ejemplo, el espacio en la mano está vacío o la carta ha sido jugada). En este escenario, el componente `Image` se deshabilita (`image.enabled = false`), lo que hace que el icono elemental sea invisible, y el método `Update()` termina su ejecución para el fotograma actual.

2.  **Visualización del icono:** Si se detecta la presencia de una carta (`card.GetCard()` no es `null`), el componente `Image` se habilita (`image.enabled = true`).
    ```csharp
    image.enabled = true;
    image.sprite = elements[(int)card.GetCard().GetElement()];
    ```
    Posteriormente, la propiedad `sprite` del `Image` se actualiza. El `Sprite` específico se selecciona del array `elements`. El índice para este array se determina mediante:
    *   La llamada a `card.GetCard().GetElement()`: Esta secuencia implica que el script `HandCard` tiene un método `GetCard()` que devuelve una instancia de una clase o struct `Card`. A su vez, este objeto `Card` tiene un método `GetElement()` que retorna el tipo elemental de la carta.
    *   La conversión del resultado a un `int`: `(int)card.GetCard().GetElement()` convierte el tipo elemental (probablemente un valor de un `enum`) en un número entero, permitiendo su uso como índice para el array `elements` de `Sprite`s.

> [!NOTE] Relación entre Elementos y Sprites
> La forma en que `CardElement` selecciona el `Sprite` correcto para el elemento de la carta es clave. Se asume que el método `card.GetCard().GetElement()` devuelve un valor (probablemente un `enum`) que, al ser casteado a un entero (`(int)`), corresponde directamente al índice del `array elements`. Esto significa que el orden de los `Sprite` en el `array elements` en el Inspector de Unity debe coincidir con el orden numérico de los valores definidos en el `enum` de elementos del juego.

Este mecanismo de actualización continua asegura que el icono elemental mostrado esté siempre sincronizado con la carta que se representa en ese momento, adaptándose a los cambios en el estado de la carta o a su ausencia.

> [!TIP] Consideraciones de Rendimiento
> El método `Update()` se ejecuta cada fotograma. Para un juego de cartas donde la información elemental de una carta no cambia frecuentemente una vez en mano, se podría considerar una aproximación basada en eventos (ej. un evento `OnCardChanged` en `HandCard`) para actualizar la imagen solo cuando sea necesario. Sin embargo, para un proyecto indie como **Beast Card Clash**, este enfoque en `Update()` es simple, robusto y fácil de implementar, priorizando la experiencia de desarrollo sin un impacto significativo en el rendimiento para este componente específico.

## Otros métodos
Este script no define ningún método adicional aparte de los métodos de ciclo de vida de Unity (`Start` y `Update`).

## Getters y Setters
Este script no implementa getters o setters públicos para sus propias variables internas (`card`, `image`, `elements`). Las variables `card` e `image` son privadas y se acceden internamente, mientras que el array `elements` es un campo serializado (`[SerializeField]`) que permite la asignación directa de `Sprite`s desde el Inspector de Unity.
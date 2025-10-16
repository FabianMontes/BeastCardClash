# `CardValue`
El script `CardValue` es un componente de Unity (`MonoBehaviour`) que, utilizando los namespaces `TMPro` y `UnityEngine`, está diseñado para mostrar dinámicamente el valor numérico de una carta en la interfaz de usuario. Su función principal es establecer una conexión con un componente `HandCard` (que se espera esté en un GameObject padre) para obtener la referencia de la carta actual y, posteriormente, extraer su valor. Este valor numérico se proyecta entonces en un componente `TextMeshProUGUI` adjunto al mismo GameObject que `CardValue`.

Este componente es esencial para la representación visual de la información de las cartas en el juego "Beast Card Clash". Al asegurar que el valor de cada carta se muestre de manera clara y actualizada, `CardValue` contribuye directamente a la experiencia de usuario, permitiendo a los jugadores comprender rápidamente las propiedades de las cartas y tomar decisiones estratégicas, alineándose con la estrategia elemental y las habilidades únicas mencionadas en el `README.md`.

# Métodos

## Métodos de Unity

### `Start()`
El método `Start()` se invoca una única vez al inicio del ciclo de vida del script. Se ejecuta después de que todos los objetos han sido inicializados, pero antes de la primera actualización de frame. Su propósito en `CardValue` es inicializar las referencias a los componentes con los que interactuará el script:

1.  **Obtención de `HandCard`**: Busca y asigna a la variable `card` una referencia al componente `HandCard`. Se utiliza `GetComponentInParent<HandCard>()` para buscar este componente en el propio GameObject que contiene `CardValue` o en cualquiera de sus GameObjects padres. Esto establece el vínculo entre el visualizador del valor y la lógica que gestiona la carta principal.
    ```csharp
    card = GetComponentInParent<HandCard>();
    ```
2.  **Obtención de `TextMeshProUGUI`**: Asigna a la variable `textMeshPro` una referencia al componente `TextMeshProUGUI` que debe estar adjunto al mismo GameObject que este script `CardValue`. Este componente es el lienzo donde se dibujará el texto del valor de la carta.
    ```csharp
    textMeshPro = GetComponent<TextMeshProUGUI>();
    ```

### `Update()`
El método `Update()` se ejecuta en cada frame del juego. Su responsabilidad principal es mantener el texto del valor de la carta constantemente actualizado en la interfaz de usuario, reflejando cualquier cambio en el estado de la carta. La lógica implementada es la siguiente:

1.  **Verificación de la presencia de la carta**: El script primero comprueba si hay una carta válida asociada al componente `HandCard` (la referencia `card` obtenida en `Start()`). Para ello, invoca el método `GetCard()` del objeto `HandCard`.
    ```csharp
    if (card.GetCard() == null)
    {
        textMeshPro.text = "";
        return;
    }
    ```
    Si el método `card.GetCard()` retorna `null` (lo que implica que no hay una carta asignada o está vacía), el texto del `TextMeshProUGUI` se limpia (se establece como una cadena vacía) y la ejecución del método `Update()` finaliza para el frame actual. Esto asegura que no se muestre ningún valor si no hay una carta.
2.  **Actualización del texto del valor**: Si se detecta una carta (`card.GetCard()` no es `null`), el script procede a obtener el valor numérico de dicha carta. Esto se logra llamando al método `GetValue()` del objeto `Card` que `HandCard.GetCard()` ha proporcionado. El valor resultante (que presumiblemente es un tipo numérico) se convierte a una cadena de texto utilizando `.ToString()` y se asigna a la propiedad `text` del componente `textMeshPro`, actualizando así la visualización en pantalla.
    ```csharp
    textMeshPro.text = card.GetCard().GetValue().ToString();
    ```
    Este ciclo de verificación y actualización en cada frame garantiza que la interfaz de usuario sea reactiva y muestre siempre el valor correcto de la carta, lo cual es fundamental para una "buena experiencia de jugador".

## Otros métodos
Este script `CardValue` no define métodos públicos adicionales fuera de los métodos de ciclo de vida estándar de Unity (`Start`, `Update`).

## Getters y Setters
Este script `CardValue` no implementa ningún getter o setter público para exponer o modificar directamente sus variables internas (`card` o `textMeshPro`). Su diseño se centra en la lógica de visualización, obteniendo los datos necesarios de otros componentes a través de sus métodos.
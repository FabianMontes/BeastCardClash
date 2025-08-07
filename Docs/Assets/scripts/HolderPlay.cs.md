Aquí está la documentación técnica para el script `HolderPlay.cs`, siguiendo las directrices proporcionadas.

---

# `HolderPlay.cs`

## 1. Propósito General
Este script gestiona la referencia a una única carta que ha sido "seleccionada" o "tomada" por el jugador en un momento dado. Su propósito principal es mantener el rastro de esta carta activa y proporcionar métodos para acceder a ella, liberarla o reemplazarla, lo que es fundamental para la interacción del jugador con las cartas en el juego.

## 2. Componentes Clave

### `HolderPlay`
*   **Descripción:** `HolderPlay` es un script de Unity que hereda de `MonoBehaviour`, lo que le permite ser adjuntado a un GameObject en la escena. Se encarga de contener y gestionar la referencia a la carta actualmente "recogida" o seleccionada por un jugador o una entidad del juego. Permite a otros sistemas saber qué carta está activa, y ofrece funciones para 'deseleccionar' la carta actual o 'seleccionar' una nueva, manejando la visibilidad de la carta previamente seleccionada.

*   **Variables Públicas / Serializadas:**
    *   `Card cardPicked`: Esta es una variable privada de tipo `Card` marcada con el atributo `[SerializeField]`. Esto significa que, aunque es privada y no directamente accesible desde otros scripts, su valor puede ser asignado y modificado directamente desde el Inspector de Unity. `cardPicked` almacena la referencia a la instancia de `Card` que representa la carta que ha sido seleccionada o está actualmente activa.

*   **Métodos Principales:**

    *   `public Card GetPicked()`:
        Este método proporciona acceso de solo lectura a la carta que está actualmente almacenada en la variable `cardPicked`. Es un método sencillo que devuelve la instancia de `Card` que representa la carta que ha sido "seleccionada".
        ```csharp
        public Card GetPicked()
        {
            return cardPicked;
        }
        ```

    *   `public void LosePick()`:
        Este método se encarga de "deseleccionar" o liberar la carta que está actualmente en `cardPicked`. Su lógica es la siguiente:
        1.  Verifica si `cardPicked` no es `null` (es decir, si hay una carta actualmente seleccionada).
        2.  Si hay una carta, activa su GameObject (`cardPicked.gameObject.SetActive(true)`). Esto es crucial, ya que sugiere que la carta podría haber sido desactivada o "escondida" visualmente al ser seleccionada, y este método la "devuelve" a un estado visible antes de ser liberada.
        3.  Finalmente, establece `cardPicked` a `null`, indicando que ya no hay ninguna carta seleccionada.
        ```csharp
        public void LosePick()
        {
            if (cardPicked != null) cardPicked.gameObject.SetActive(true);
            cardPicked = null;
        }
        ```

    *   `public void PlayCard(Card card)`:
        Este método permite asignar una nueva carta como la carta "seleccionada" o activa. Recibe una instancia de `Card` como parámetro (`card`). La lógica interna es la siguiente:
        1.  Comprueba si ya hay una carta asignada a `cardPicked`.
        2.  Si `cardPicked` no es `null`, invoca el método `LosePick()` primero. Esto asegura que cualquier carta previamente seleccionada sea manejada correctamente (reactivada y su referencia liberada) antes de seleccionar la nueva.
        3.  Finalmente, asigna la `card` pasada como parámetro a `cardPicked`, convirtiéndola en la nueva carta activa para este `HolderPlay`.
        ```csharp
        public void PlayCard(Card card)
        {
            if (cardPicked != null) LosePick();
            cardPicked = card;
        }
        ```

*   **Lógica Clave:** La lógica central de este script gira en torno a gestionar una referencia única a una carta activa. La función `PlayCard` implementa un comportamiento de "una sola carta activa a la vez", asegurando que si una nueva carta es "seleccionada," cualquier carta previamente seleccionada sea primero "deseleccionada" correctamente (incluyendo la reactivación de su GameObject) antes de asignar la nueva.

## 3. Dependencias y Eventos
*   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, lo que significa que no impone la existencia de otros componentes específicos en el mismo GameObject para funcionar correctamente. Puede ser adjuntado a cualquier GameObject.

*   **Eventos (Entrada):** Según el código proporcionado, `HolderPlay` no se suscribe explícitamente a ningún evento de Unity o de otros sistemas (como eventos de UI de botones o eventos de entrada de usuario directos). Sus métodos públicos (`GetPicked`, `LosePick`, `PlayCard`) están diseñados para ser invocados externamente por otros scripts del juego que necesiten interactuar con la carta seleccionada.

*   **Eventos (Salida):** Este script no invoca ni publica eventos de salida (como `UnityEvent` o `Action` personalizados) para notificar a otros sistemas sobre cambios en `cardPicked`. Otros sistemas que necesiten reaccionar a la selección o deselección de una carta deberán consultar el estado de `cardPicked` a través de `GetPicked()` o ser informados por el script que invoca `PlayCard()`.

---
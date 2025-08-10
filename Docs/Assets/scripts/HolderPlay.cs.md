# `HolderPlay.cs`

## 1. Propósito General

Este script `HolderPlay` es un `MonoBehaviour` que gestiona la referencia a una única "carta seleccionada" (`Card`) en un momento dado. Su función principal es servir como un puntero para la carta que está siendo activamente manipulada o considerada por el jugador en un contexto específico del juego (ej. la carta que se va a jugar).

## 2. Componentes Clave

### `HolderPlay`

-   **Descripción:** `HolderPlay` es una clase `MonoBehaviour` que actúa como un gestor simple para la carta que el jugador tiene "seleccionada" o "elegida" en un determinado momento de la interfaz de juego. Mantiene una referencia a la `Card` actualmente seleccionada y proporciona métodos para establecer, obtener o "deseleccionar" esta carta, ajustando también su estado de activación en la jerarquía de Unity.

-   **Variables Públicas / Serializadas:**
    -   `[SerializeField] Card cardPicked`: Esta variable almacena una referencia al objeto `Card` que ha sido "seleccionado" o "elegido". Debido a `[SerializeField]`, puede ser asignada desde el Inspector de Unity, aunque su propósito principal es mantener el estado de la carta seleccionada durante el tiempo de ejecución.

-   **Métodos Principales:**
    -   `public Card GetPicked()`:
        *   **Descripción:** Este método devuelve la instancia de `Card` que está actualmente referenciada como la carta seleccionada (`cardPicked`).
        *   **Parámetros:** Ninguno.
        *   **Retorno:** La `Card` actualmente seleccionada.
        *   **Fragmento de código:**
            ```csharp
            public Card GetPicked()
            {
                return cardPicked;
            }
            ```

    -   `public void LosePick()`:
        *   **Descripción:** Este método es responsable de "deseleccionar" la carta actualmente elegida. Si hay una carta (`cardPicked`) asignada, primero asegura que su `GameObject` se vuelva activo (asumiendo que pudo haber sido desactivado al ser "elegida"), y luego establece `cardPicked` a `null`, liberando la referencia.
        *   **Parámetros:** Ninguno.
        *   **Retorno:** `void`.
        *   **Fragmento de código:**
            ```csharp
            public void LosePick()
            {
                if (cardPicked != null) cardPicked.gameObject.SetActive(true);
                cardPicked = null;
            }
            ```

    -   `public void PlayCard(Card card)`:
        *   **Descripción:** Este método se utiliza para designar una nueva `Card` como la carta seleccionada. Antes de asignar la nueva `card` a `cardPicked`, llama automáticamente a `LosePick()` para asegurarse de que cualquier carta previamente seleccionada sea deseleccionada correctamente y su `GameObject` se reactive.
        *   **Parámetros:**
            *   `Card card`: La nueva instancia de `Card` que se desea establecer como la carta seleccionada.
        *   **Retorno:** `void`.
        *   **Fragmento de código:**
            ```csharp
            public void PlayCard(Card card)
            {
                if (cardPicked != null) LosePick();
                cardPicked = card;
            }
            ```

-   **Lógica Clave:**
    La lógica central del `HolderPlay` radica en la gestión de una única referencia a una `Card`. El método `PlayCard` implementa un patrón de "solo una a la vez" al invocar `LosePick` antes de asignar una nueva carta. Esto garantiza que al seleccionar una nueva carta, la anterior es automáticamente "liberada" y su `GameObject` es reactivado, lo que es crucial para la gestión visual del tablero o la mano del jugador. La asunción implícita es que al "seleccionar" una carta (antes de "jugarla"), su `GameObject` podría volverse inactivo, y `LosePick` revierte este estado.

## 3. Dependencias y Eventos

-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, depende de la existencia de una clase `Card` (no definida en este archivo, pero necesaria para el tipo `cardPicked`) y asume que las instancias de `Card` tienen un `GameObject` asociado que puede ser activado o desactivado.

-   **Eventos (Entrada):** Este script no se suscribe directamente a eventos de interfaz de usuario de Unity (como `Button.onClick`) o eventos personalizados. Sus métodos públicos (`GetPicked`, `LosePick`, `PlayCard`) están diseñados para ser invocados por otros scripts o sistemas de juego que necesiten interactuar con la carta seleccionada.

-   **Eventos (Salida):** Este script no dispara ni invoca ningún `UnityEvent` o `Action` personalizado para notificar a otros sistemas sobre cambios en su estado. Las interacciones se manejan mediante el acceso directo a sus métodos públicos y la observación de los cambios en el estado de activación de los `GameObject` de las cartas.
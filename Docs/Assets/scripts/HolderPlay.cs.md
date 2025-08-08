# `HolderPlay.cs`

## 1. Propósito General
Este script `HolderPlay` es un componente de Unity diseñado para gestionar una única carta que ha sido "seleccionada" o "tomada" por el jugador en un momento dado. Actúa como un puntero o un 'placeholder' para la carta activa que está siendo considerada para una acción, como ser jugada, y gestiona su estado de visibilidad básico.

## 2. Componentes Clave

### `HolderPlay`
-   **Descripción:** `HolderPlay` es una clase que hereda de `MonoBehaviour`, lo que significa que puede adjuntarse a un `GameObject` en la escena de Unity. Su función principal es mantener una referencia a un objeto `Card` específico, que representa la carta actualmente seleccionada o "recogida" por el sistema del juego. Este script facilita la manipulación del estado de esta carta, permitiendo consultarla, anular su selección y reemplazarla por otra.

-   **Variables Públicas / Serializadas:**
    -   `[SerializeField] Card cardPicked;`
        Esta variable de tipo `Card` (que se asume es otra clase del proyecto que representa una carta) es serializada, lo que la hace visible y editable en el Inspector de Unity. `cardPicked` almacena la referencia a la carta que el script está gestionando como "seleccionada" o "en mano". Es el estado central que este componente mantiene.

-   **Métodos Principales:**
    -   `public Card GetPicked()`:
        Este método público permite a otros scripts o sistemas del juego obtener una referencia a la `Card` que actualmente está almacenada en `cardPicked`. No toma parámetros y devuelve el objeto `Card` referenciado. Su propósito es proporcionar acceso de lectura a la carta seleccionada.

    -   `public void LosePick()`:
        Este método se encarga de "soltar" la carta actualmente seleccionada. Primero, verifica si `cardPicked` no es `null` (es decir, si hay una carta seleccionada). Si la hay, reactiva el `GameObject` asociado a esa carta (`cardPicked.gameObject.SetActive(true)`), sugiriendo que la carta podría haber sido desactivada visualmente al ser "recogida". Finalmente, establece `cardPicked` a `null`, eliminando la referencia a la carta.

    -   `public void PlayCard(Card card)`:
        Este método es utilizado para establecer una nueva carta como la `cardPicked`. Recibe un objeto `Card` como parámetro, que será la nueva carta seleccionada. Antes de asignar la nueva carta, el método llama a `LosePick()` si ya había una carta en `cardPicked`. Esto asegura que solo una carta esté "seleccionada" a la vez y que la carta previamente seleccionada vuelva a su estado original (probablemente visible y no "en mano"). Luego, asigna la `card` recibida como parámetro a la variable `cardPicked`.

-   **Lógica Clave:**
    La lógica principal de `HolderPlay` reside en gestionar el ciclo de vida de una carta "seleccionada" de forma exclusiva. Cuando se invoca `PlayCard(Card card)`, garantiza que cualquier carta previamente seleccionada sea "liberada" (potencialmente volviendo a ser visible) antes de establecer la nueva carta. Esto previene múltiples selecciones y maneja la transición de estado visual de las cartas. El método `LosePick()` encapsula la lógica para limpiar la selección actual y preparar el sistema para una nueva.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`, por lo que no impone la presencia de otros componentes en el mismo `GameObject` para funcionar.

-   **Eventos (Entrada):**
    El script `HolderPlay` no se suscribe explícitamente a ningún evento de Unity (como `button.onClick` o delegados personalizados) dentro del código proporcionado. Sus métodos públicos (`GetPicked`, `LosePick`, `PlayCard`) están diseñados para ser invocados por otros scripts o sistemas del juego cuando sea necesario gestionar la carta seleccionada.

-   **Eventos (Salida):**
    El script `HolderPlay` no invoca ni emite eventos (como `UnityEvent` o `Action` personalizados) para notificar a otros sistemas sobre cambios en el estado de la carta seleccionada. La interacción con este script se realiza directamente a través de sus métodos públicos. Sin embargo, sí modifica el estado de un `GameObject` externo (la carta previamente seleccionada) al llamar a `SetActive(true)`.
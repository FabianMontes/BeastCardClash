# `HolderPlay.cs`

## 1. Propósito General
Este script gestiona la selección de una única carta en el contexto de la interfaz de usuario o la lógica de juego. Permite mantener una referencia a la carta "elegida" o "en foco", facilitando su posterior recuperación, deselección o reemplazo por otra carta. Interactúa principalmente con objetos de tipo `Card`.

## 2. Componentes Clave

### `HolderPlay`
- **Descripción:** La clase `HolderPlay` es un `MonoBehaviour` que se encarga de almacenar y manipular una referencia a una única instancia de `Card`. Su función principal es actuar como un "contenedor" para la carta actualmente seleccionada o activa, permitiendo a otros sistemas interactuar con ella de manera controlada.

- **Variables Públicas / Serializadas:**
    - `cardPicked` (`Card`): Esta variable privada, serializada para ser visible y configurable en el Inspector de Unity, almacena la referencia a la carta que ha sido "elegida" o "jugada" recientemente. Es el núcleo del estado de este `HolderPlay`.
    ```csharp
    [SerializeField] Card cardPicked;
    ```

- **Métodos Principales:**
    - `public Card GetPicked()`: Este método permite a otros scripts obtener la referencia a la carta que actualmente está almacenada en `cardPicked`. Devuelve la instancia de `Card` o `null` si no hay ninguna carta seleccionada.
    - `public void LosePick()`: Se utiliza para "deseleccionar" o "perder" la carta actualmente elegida. Si `cardPicked` no es nula, este método reactiva el `GameObject` asociado a esa carta y luego establece `cardPicked` a `null`, liberando la referencia. Esto es útil para limpiar el estado cuando la carta ya no es el foco.
    - `public void PlayCard(Card card)`: Este es el método principal para establecer una nueva carta como la `cardPicked`. Antes de asignar la nueva `card` a `cardPicked`, verifica si ya existía una carta seleccionada (`cardPicked != null`). Si es así, llama a `LosePick()` para deseleccionar y manejar adecuadamente la carta anterior, asegurando que solo una carta esté "en foco" a la vez.

- **Lógica Clave:**
    La lógica central de `HolderPlay` se basa en mantener una referencia singular a una `Card`. Cuando se intenta "jugar" o seleccionar una nueva carta a través de `PlayCard()`, el script maneja automáticamente la deselección de cualquier carta previamente "elegida". Esto se logra llamando a `LosePick()` para la carta anterior, lo que garantiza un estado consistente donde solo una carta está activa o en el foco de `HolderPlay` en cualquier momento. Si una carta es "perdida" manualmente con `LosePick()`, su `GameObject` asociado se reactiva (si no lo estaba ya) y la referencia se anula.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, por lo que no tiene dependencias obligatorias de otros componentes en el mismo `GameObject` para su funcionamiento básico.
- **Eventos (Entrada):** Este script no se suscribe explícitamente a eventos de Unity (como `UI.Button.onClick`) dentro de su propio código. Sus métodos públicos (`GetPicked`, `LosePick`, `PlayCard`) están diseñados para ser invocados por otros scripts o sistemas de juego que necesiten manipular el estado de la carta seleccionada.
- **Eventos (Salida):** `HolderPlay.cs` no expone ni invoca ningún evento de salida (como `UnityEvent` o `Action`) para notificar a otros sistemas sobre cambios en su estado. Las interacciones se manejan directamente a través de las llamadas a sus métodos públicos.
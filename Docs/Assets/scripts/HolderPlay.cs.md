# `HolderPlay.cs`

## 1. Propósito General
Este script gestiona el estado de una única carta que ha sido "seleccionada" o "recogida" por el jugador o el sistema de juego. Su rol principal es mantener una referencia a la carta actualmente activa para ser "jugada", permitiendo obtenerla, liberarla o reemplazarla. Interactúa directamente con objetos de tipo `Card`, asumiendo que `Card` es otro `MonoBehaviour` en el proyecto.

## 2. Componentes Clave

### `HolderPlay`
- **Descripción:** `HolderPlay` es un script que hereda de `MonoBehaviour`, lo que le permite ser adjuntado a un GameObject en la escena de Unity. Su función principal es actuar como un contenedor para la carta que ha sido seleccionada para una acción posterior (ej. ser jugada). Se asegura de que solo una carta esté en este estado de "seleccionada" a la vez.

- **Variables Públicas / Serializadas:**
    - `cardPicked`: Una variable de tipo `Card` marcada con `[SerializeField]`. Esta es la variable central del script, utilizada para almacenar la referencia a la carta que ha sido actualmente "seleccionada" o "recogida". Al ser `[SerializeField]`, es visible y puede ser asignada desde el Inspector de Unity, aunque su gestión principal se realiza a través de los métodos públicos del script.

- **Métodos Principales:**
    - `public Card GetPicked()`:
        - **Descripción:** Este método público permite a otros scripts obtener una referencia a la carta que actualmente está seleccionada en el `HolderPlay`.
        - **Retorna:** El objeto `Card` que está siendo mantenido como `cardPicked`, o `null` si ninguna carta ha sido seleccionada.

    - `public void LosePick()`:
        - **Descripción:** Este método se encarga de "deseleccionar" la carta actualmente recogida. Si `cardPicked` contiene una referencia válida, su GameObject asociado se reactiva (lo que sugiere que podría haber sido desactivado al ser "recogido") y la referencia `cardPicked` se establece en `null`. Esto prepara el `HolderPlay` para una nueva selección.
        - **Lógica Clave:**
          ```csharp
          if (cardPicked != null) cardPicked.gameObject.SetActive(true);
          cardPicked = null;
          ```
          Esta lógica asegura que, si una carta estaba seleccionada, su GameObject vuelva a estar activo en la jerarquía antes de que la referencia se anule.

    - `public void PlayCard(Card card)`:
        - **Descripción:** Este método público es la forma principal de establecer una nueva carta como la `cardPicked`. Recibe un objeto `Card` como parámetro y lo asigna a `cardPicked`. Antes de realizar la asignación, verifica si ya hay una carta seleccionada. Si es así, llama a `LosePick()` para manejar la carta anterior, garantizando que solo una carta sea "recogida" en un momento dado.
        - **Parámetros:**
            - `card`: La nueva instancia de `Card` que se desea establecer como la carta actualmente "recogida".
        - **Lógica Clave:**
          ```csharp
          if (cardPicked != null) LosePick();
          cardPicked = card;
          ```
          Esta lógica previene múltiples selecciones, limpiando la anterior antes de establecer la nueva.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, por lo que no impone la presencia de otros componentes en el mismo GameObject.

- **Eventos (Entrada):** Este script no se suscribe a ningún evento (`UnityEvent`, `Action`, etc.) en el código proporcionado. Sus métodos públicos (`GetPicked`, `LosePick`, `PlayCard`) están diseñados para ser invocados externamente por otros sistemas del juego.

- **Eventos (Salida):** Este script no invoca ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios en su estado. Su efecto principal es la modificación interna de la variable `cardPicked` y la activación/desactivación de GameObjects de `Card` externos.
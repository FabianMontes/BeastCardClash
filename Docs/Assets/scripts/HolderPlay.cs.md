# `HolderPlay.cs`

## 1. Propósito General
`HolderPlay` es un script de Unity que gestiona la referencia a una única "carta seleccionada" por el jugador en un momento dado. Su rol principal es mantener el estado de qué carta está actualmente "en mano" o siendo manipulada, proporcionando métodos para obtener, deseleccionar o establecer una nueva carta, y asegurando que solo una carta esté activa como "seleccionada" a la vez. Interactúa con el sistema de cartas del juego (`Card` script o clase) y posiblemente con la interfaz de usuario que permite al jugador seleccionar cartas.

## 2. Componentes Clave

### `HolderPlay`
- **Descripción:** `HolderPlay` es una clase que hereda de `MonoBehaviour`, lo que significa que puede ser adjuntada como un componente a un GameObject en la escena de Unity. Su función es actuar como un contenedor de estado para la carta que ha sido "recogida" por el jugador. Esto es útil en mecánicas donde un jugador selecciona una carta de su mano o del campo para realizar una acción, y esa selección necesita ser gestionada centralmente.
- **Variables Públicas / Serializadas:**
    - `[SerializeField] Card cardPicked`: Esta variable privada, pero visible y editable desde el Inspector de Unity gracias al atributo `[SerializeField]`, almacena una referencia al objeto `Card` (se asume que `Card` es una clase o script que representa una carta del juego) que ha sido "seleccionada" o "recogida" por el jugador. Si no hay ninguna carta seleccionada, su valor será `null`.
- **Métodos Principales:**
    - `public Card GetPicked()`:
        - **Descripción:** Permite a otros scripts obtener una referencia a la `Card` que está actualmente asignada a `cardPicked`.
        - **Retorna:** El objeto `Card` actualmente seleccionado, o `null` si no hay ninguna carta seleccionada.
    - `public void LosePick()`:
        - **Descripción:** Gestiona la deselección de la carta actualmente "recogida". Si `cardPicked` no es `null`, este método reactiva el `GameObject` asociado a la carta (`cardPicked.gameObject.SetActive(true)`) antes de liberar la referencia estableciendo `cardPicked` a `null`. Esto sugiere que la carta podría haber sido desactivada o su visibilidad modificada al ser "recogida".
    - `public void PlayCard(Card card)`:
        - **Descripción:** Establece la carta pasada como parámetro (`card`) como la nueva carta "recogida". Antes de hacer esto, verifica si ya hay una carta previamente seleccionada (`cardPicked`). Si la hay, invoca a `LosePick()` para deseleccionar y manejar la carta anterior, asegurando que solo una carta pueda estar "recogida" a la vez.
        - **Parámetros:**
            - `Card card`: La nueva carta que se desea establecer como la carta "recogida" o seleccionada.
- **Lógica Clave:** La lógica central de `HolderPlay` es mantener un estado de "una única carta seleccionada". Cuando se intenta "seleccionar" una nueva carta a través de `PlayCard`, el script asegura que cualquier carta previamente seleccionada sea "deseleccionada" (lo que incluye potencialmente reactivar su GameObject) antes de asignar la nueva. Esto simplifica la gestión de la interacción del jugador con las cartas, evitando la necesidad de que los scripts que "juegan" o "recogen" cartas se preocupen por el estado de selecciones anteriores.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, por lo que no requiere que otros componentes específicos estén presentes en el mismo GameObject para funcionar.
- **Eventos (Entrada):** `HolderPlay` no se suscribe explícitamente a ningún evento de Unity o de otros sistemas (por ejemplo, `button.onClick.AddListener`). Sus métodos públicos están diseñados para ser invocados directamente por otros scripts que necesiten interactuar con el estado de la carta seleccionada.
- **Eventos (Salida):** El script `HolderPlay` no emite ni invoca ningún tipo de evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios en su estado interno (como cuando se selecciona o deselecciona una carta). Las interacciones con este script se realizan a través de llamadas directas a sus métodos públicos.
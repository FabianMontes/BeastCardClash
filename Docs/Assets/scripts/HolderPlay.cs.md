# `HolderPlay.cs`

## 1. Propósito General

Este script de Unity (`MonoBehaviour`) actúa como un gestor de la carta actualmente "seleccionada" o "recogida" por el jugador en el contexto del juego. Su función principal es mantener una referencia a una única carta que ha sido elegida, permitiendo a otros sistemas del juego acceder a ella y gestionar su estado de selección. Interactúa principalmente con el sistema de cartas del juego, controlando qué carta está activa en un momento dado como "la seleccionada".

## 2. Componentes Clave

### `HolderPlay`
- **Descripción:** La clase `HolderPlay` hereda de `MonoBehaviour`, lo que significa que es un componente que puede adjuntarse a un GameObject en la escena de Unity. Su rol es centralizar la gestión de una única instancia de `Card` que el jugador ha "recogido" o "seleccionado". Esto es fundamental para mecánicas donde se necesita una carta activa para realizar una acción, como jugar una carta o descartarla.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] Card cardPicked;`: Esta variable privada está marcada con `[SerializeField]`, lo que la hace visible y editable directamente desde el Inspector de Unity. Almacena una referencia a la instancia del script `Card` que representa la carta actualmente seleccionada por el jugador. Su valor será `null` si no hay ninguna carta seleccionada.

- **Métodos Principales:**
    - `public Card GetPicked()`: Este método proporciona acceso externo a la carta que está actualmente seleccionada. Retorna la instancia de `Card` almacenada en `cardPicked`. Otros scripts pueden llamar a este método para saber qué carta ha sido elegida por el jugador.

    - `public void LosePick()`: Su propósito es deseleccionar la carta que está actualmente en `cardPicked`. Si `cardPicked` no es `null` (es decir, hay una carta seleccionada), este método primero reactiva el GameObject asociado a esa carta (`cardPicked.gameObject.SetActive(true)`). Esta acción sugiere que, al ser "seleccionada", la carta pudo haber sido desactivada visualmente o movida a un estado inactivo. Finalmente, la referencia `cardPicked` se establece en `null`, indicando que ya no hay ninguna carta seleccionada.

    - `public void PlayCard(Card card)`: Este es el método principal para establecer una nueva carta como la "seleccionada". Recibe un objeto `Card` como parámetro. Antes de asignar la nueva carta a `cardPicked`, el método verifica si ya existe una carta seleccionada (`cardPicked != null`). Si es así, llama a `LosePick()` para asegurar que la carta previamente seleccionada sea adecuadamente deseleccionada y su GameObject reactivado, manteniendo así un estado consistente donde solo una carta puede estar "recogida" a la vez. Después de esta limpieza, la nueva `card` pasada como argumento se asigna a `cardPicked`.

- **Lógica Clave:**
    La lógica central del script gira en torno a la gestión de un estado binario: o hay una carta seleccionada (`cardPicked` tiene una referencia) o no la hay (`cardPicked` es `null`). La función `PlayCard` impone la regla de que solo una carta puede estar "recogida" al mismo tiempo, manejando la deselección automática de la carta anterior. La reactivación del GameObject en `LosePick()` es una implicación importante para la retroalimentación visual del juego, sugiriendo que las cartas pueden ser temporalmente "ocultadas" o "movidas" al ser seleccionadas.

## 3. Dependencias y Eventos

- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]` para imponer la presencia de otros componentes en el mismo GameObject.

- **Eventos (Entrada):**
    `HolderPlay` no se suscribe explícitamente a ningún evento del ciclo de vida de Unity (como `Awake`, `Start`, `Update`) ni a eventos de interacción de UI (como `Button.onClick`). Sus métodos públicos (`GetPicked`, `LosePick`, `PlayCard`) están diseñados para ser llamados por otros scripts o sistemas de juego que necesiten interactuar con la carta seleccionada.

- **Eventos (Salida):**
    El script `HolderPlay` no invoca ningún `UnityEvent` ni `Action` para notificar a otros sistemas sobre cambios en su estado. La gestión de la carta seleccionada se realiza internamente a través de la variable `cardPicked` y la modificación directa del estado `gameObject.SetActive` de la carta deseleccionada.
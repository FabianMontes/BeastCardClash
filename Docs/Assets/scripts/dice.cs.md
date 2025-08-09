# `dice.cs`

## 1. Propósito General
El script `dice.cs` gestiona el comportamiento interactivo de un dado en el juego. Su rol principal es simular el lanzamiento de un dado, permitiendo al jugador iniciar el "rodado" mediante interacción con el ratón y comunicar el valor final obtenido al sistema de combate principal.

## 2. Componentes Clave

### `dice`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, controla la lógica y la representación visual de un dado en la escena. Es responsable de actualizar su valor numérico de forma aleatoria mientras está siendo "rodado" y de mostrar este valor en un componente de texto. También maneja las interacciones del usuario a través del ratón para iniciar y finalizar el lanzamiento del dado.
- **Variables Públicas / Serializadas:**
    - `int value`: Almacena el número actual que muestra el dado. Este valor cambia rápidamente mientras el dado está "rodando" y se detiene en un valor final.
    - `int maxValue`: Define el valor máximo posible que el dado puede alcanzar (ej. 6 para un dado tradicional). Se inicializa al inicio del juego a partir de una variable global del sistema de combate.
    - `TextMeshPro texter`: Una referencia al componente TextMeshPro que se encuentra como hijo del GameObject del dado. Este componente se utiliza para mostrar el `value` actual del dado en pantalla.
    - `bool roling`: Un indicador booleano que determina si el dado está actualmente en proceso de "rodado" (es decir, su valor se está generando aleatoriamente cada fotograma).
- **Métodos Principales:**
    - `void Start()`: Este método se ejecuta una vez al inicio del ciclo de vida del script.
        - Inicializa `maxValue` con el valor de `Combatjudge.combatjudge.maxDice`, lo que establece el rango máximo para el lanzamiento del dado.
        - Establece `roling` a `false`, asegurando que el dado no comience a rodar automáticamente.
        - Obtiene la referencia al componente `TextMeshPro` del GameObject hijo, que se utilizará para actualizar el texto del dado.
    - `void Update()`: Se invoca una vez por cada fotograma del juego.
        - Si la variable `roling` es `true`, el `value` del dado se actualiza con un número aleatorio entre 1 y `maxValue` (inclusive). Este comportamiento continuo simula visualmente un dado rodando.
        - En cada fotograma, el texto del componente `texter` se actualiza para mostrar el `value` actual del dado.
    - `private void OnMouseDown()`: Se ejecuta cuando el usuario presiona el botón del ratón mientras el cursor está sobre el Collider del GameObject del dado.
        - Este método verifica dos condiciones importantes del estado del juego a través de `Combatjudge.combatjudge`: que el momento actual sea `SetMoments.PickDice` (indicando que es el momento de elegir un dado) y que el turno actual esté enfocado en la acción del jugador (`Combatjudge.combatjudge.FocusONTurn()`).
        - Si ambas condiciones se cumplen, se activa `roling` a `true`, iniciando el proceso de "rodado" visual del dado.
    - `private void OnMouseExit()`: Se ejecuta cuando el cursor del ratón abandona el área del Collider del GameObject del dado.
        - Si el dado estaba "rodando" (`roling` es `true`), detiene el proceso (`roling = false`) y notifica al sistema de combate el valor final obtenido a través de `Combatjudge.combatjudge.Roled(value)`.
    - `private void OnMouseUp()`: Se ejecuta cuando el usuario suelta el botón del ratón mientras el cursor está sobre el Collider del GameObject del dado.
        - De manera similar a `OnMouseExit()`, si el dado estaba "rodando" (`roling` es `true`), detiene el proceso (`roling = false`) y envía el valor final del dado a `Combatjudge.combatjudge.Roled(value)`.
- **Lógica Clave:**
    La lógica central del script gira en torno al lanzamiento del dado controlado por el usuario. Cuando el jugador hace clic sobre el dado, si el estado del juego lo permite, se activa un "modo de rodado" (`roling = true`). Durante este modo, el dado muestra rápidamente diferentes valores aleatorios. El rodado se detiene cuando el jugador deja de interactuar con el dado (ya sea moviendo el ratón fuera del dado o soltando el botón del ratón), momento en el cual se selecciona un valor final y se comunica al sistema de combate para su procesamiento.

## 3. Dependencias y Eventos

- **Componentes Requeridos:**
    - Aunque no se utiliza `[RequireComponent]`, este script necesita que el GameObject al que está adjunto tenga un componente `Collider` (ej. `BoxCollider`, `SphereCollider`) para detectar las interacciones del ratón (`OnMouseDown`, `OnMouseExit`, `OnMouseUp`).
    - También requiere que un componente `TextMeshPro` esté presente como hijo del GameObject del dado para poder visualizar el valor del dado.
- **Eventos (Entrada):**
    - Este script responde a los eventos de entrada del ratón específicos de Unity:
        - `OnMouseDown()`: Para iniciar el rodado del dado.
        - `OnMouseExit()`: Para finalizar el rodado si el ratón se mueve fuera del dado.
        - `OnMouseUp()`: Para finalizar el rodado si se suelta el botón del ratón.
- **Eventos (Salida):**
    - Una vez que el dado ha finalizado su rodado, este script invoca el método `Combatjudge.combatjudge.Roled(value)`, pasando el valor final del dado al sistema de combate principal. Esto permite que otros sistemas reaccionen al resultado del lanzamiento del dado.
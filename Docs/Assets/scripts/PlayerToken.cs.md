# `PlayerToken.cs`

## 1. Propósito General
Este script, adjunto a un GameObject, representa el token visual de un jugador en el tablero de juego. Su rol principal es gestionar la posición del token en relación con los "rocky" (rocas o puntos de interés en el tablero) y actualizar su apariencia visual basada en la especie del jugador asociado. Interactúa estrechamente con los sistemas de movimiento del jugador y la lógica de combate.

## 2. Componentes Clave

### `PlayerToken`
- **Descripción:** La clase `PlayerToken` es un `MonoBehaviour` que controla el comportamiento y la representación visual de un token de jugador en el escenario. Se encarga de sincronizar la posición del token con la roca donde se encuentra el jugador y de establecer su color según la especie del personaje (`Figther`) que representa.

- **Variables Públicas / Serializadas:**
    - `public RockBehavior rocky`: Esta variable pública referencia el objeto `RockBehavior` que representa la roca o posición actual sobre la que se encuentra el token. Es esencial para determinar dónde debe posicionarse visualmente el token y para interactuar con la lógica de la roca.
    - `public Figther player`: Esta variable pública referencia el objeto `Figther` (luchador/personaje) asociado a este token. Permite al `PlayerToken` acceder a información del jugador, como su especie, para configurar su apariencia.
    - `public RockBehavior lastRock`: Esta variable pública almacena una referencia a la última `RockBehavior` en la que se encontraba el token. Es utilizada para notificar a la roca anterior que el jugador ha salido de ella antes de moverse a una nueva.

- **Métodos Principales:**

    - `void Start()`:
        Este método se ejecuta una vez al inicio del ciclo de vida del script.
        Su función principal es inicializar la posición del token y su apariencia visual.
        ```csharp
        void Start()
        {
            if (rocky != null)
            {
                transform.position = rocky.transform.position;
                lastRock = rocky;
                rocky.AddPlayer(this);
            }
            // ... (lógica de color)
        }
        ```
        Primero, si la variable `rocky` está asignada, el token se posiciona inmediatamente en la ubicación de la `rocky` inicial. También se asigna `rocky` a `lastRock` y se invoca el método `AddPlayer` en la roca actual para registrar la presencia del token.
        Luego, accede al `SpriteRenderer` del GameObject y cambia su color basándose en la especie del jugador (`player.GetSpecie()`). Se utilizan diferentes colores predefinidos (verde, marrón, magenta, amarillo) para representar especies como camaleón, oso, serpiente y rana, respectivamente.

    - `void Update()`:
        Este método se invoca una vez por cada frame del juego.
        ```csharp
        void Update()
        {
            if (rocky != null)
            {
                if (rocky.transform.position != transform.position)
                {
                    // ... (lógica de movimiento)
                }
            }
        }
        ```
        Su propósito es detectar cambios en la posición de la roca actual (`rocky`). Si la posición del token ya no coincide con la posición de la `rocky` asignada (lo que implica que la `rocky` ha sido actualizada o que el jugador ha "elegido" una nueva roca a través de otro sistema), el token actualiza su propia posición.
        Al detectar un cambio, primero notifica a la `lastRock` que el jugador ya no está allí (`lastRock.RemovePlayer(this)`). Luego, actualiza su posición a la de la nueva `rocky`, notifica a la nueva `rocky` de su presencia (`rocky.AddPlayer(this)`), y actualiza `lastRock` para que apunte a la roca recién ocupada. Finalmente, invoca `Combatjudge.combatjudge.ArriveAtRock()` para señalar a la lógica de combate que el jugador ha llegado a una nueva roca.

- **Lógica Clave:**
    La lógica central de `PlayerToken` reside en su capacidad para reaccionar al cambio de la roca asignada (`rocky`). Aunque el código no muestra cómo `rocky` es *actualizada* externamente, el método `Update` está diseñado para detectar cuando la posición del token ya no coincide con la de `rocky`. Esto implica un sistema externo que asigna una nueva `RockBehavior` al campo `rocky` cuando el jugador se mueve. El `PlayerToken` entonces se encarga de la parte visual del movimiento y de notificar a las rocas involucradas (la anterior y la actual) sobre la presencia o ausencia del jugador, así como al sistema de combate sobre el cambio de posición. La asignación de color al inicio también es una parte clave para la identificación visual del jugador.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, para su funcionamiento visual, se espera que el GameObject al que está adjunto tenga un componente `SpriteRenderer` para poder modificar su color en tiempo de ejecución.

- **Eventos (Entrada):**
    Este script no se suscribe directamente a eventos de Unity (`UnityEvent`, `Action`). Su activación principal se basa en el ciclo de vida de `MonoBehaviour` (`Start`, `Update`) y en el cambio externo de la referencia `rocky`.

- **Eventos (Salida):**
    Este script invoca métodos en otros sistemas o componentes para notificar cambios de estado:
    - Llama a `rocky.AddPlayer(this)` y `lastRock.RemovePlayer(this)`: Notifica a los objetos `RockBehavior` sobre la entrada y salida del token.
    - Llama a `Combatjudge.combatjudge.ArriveAtRock()`: Notifica al sistema de combate (`Combatjudge`) que el token ha llegado a una nueva roca, lo que probablemente activa la lógica de combate o fase de juego relevante.
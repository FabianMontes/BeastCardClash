# `dice.cs`

## 1. Propósito General
El script `dice.cs` gestiona el comportamiento y la visualización de un dado en el juego. Su función principal es simular el lanzamiento de un dado, generar un valor aleatorio y rotar el modelo 3D del dado para mostrar el resultado, interactuando directamente con el sistema `Combatjudge` para determinar cuándo y cómo se puede lanzar el dado.

## 2. Componentes Clave

### `dice`
-   **Descripción:** Esta clase, que hereda de `MonoBehaviour`, controla la lógica y la representación visual de un único dado en la escena de Unity. Responde a interacciones del ratón (clic, entrada y salida del cursor) y actualiza su estado y apariencia en función de la fase de combate actual, según lo dictado por el sistema `Combatjudge`.

-   **Variables Públicas / Serializadas:**
    -   `public int value { get; private set; } = 0;`: Representa el valor actual del dado una vez que ha "aterrizado". Es de solo lectura desde fuera de la clase.
    -   `int maxValue;`: Un campo interno que almacena el valor máximo que el dado puede generar. Se inicializa al inicio del juego a partir de la configuración del sistema `Combatjudge`.
    -   `bool roling;`: Una bandera interna que indica si el dado está actualmente en proceso de "lanzamiento" (es decir, su animación de rotación y generación de valor aleatorio está activa).

-   **Métodos Principales:**
    -   `void Start()`: Este método del ciclo de vida de Unity se ejecuta una vez al inicio. Inicializa `maxValue` obteniéndolo de `Combatjudge.combatjudge.maxDice` y asegura que la bandera `roling` esté en `false`.
    -   `void Update()`: Este método del ciclo de vida se ejecuta en cada frame. Si la bandera `roling` es `true`, activa el proceso de lanzamiento del dado:
        -   Notifica al sistema `Combatjudge` que se ha iniciado un lanzamiento (`Combatjudge.combatjudge.StartRoling()`).
        -   Genera un nuevo valor aleatorio para el dado (`value = Random.Range(1, maxValue + 1);`).
        -   Ajusta la rotación del `transform` del GameObject al que está adjunto el script para simular visualmente la cara del dado correspondiente al `value` generado.

        ```csharp
        if (roling)
        {
            Combatjudge.combatjudge.StartRoling();
            value = Random.Range(1, maxValue + 1);
            // ... (lógica de rotación basada en el valor)
            transform.rotation = Quaternion.Euler(vector3);
        }
        ```

    -   `private void OnMouseDown()`: Se invoca cuando el usuario hace clic con el botón del ratón mientras el cursor está sobre el Collider del dado. Si el sistema `Combatjudge` indica que es el turno para enfocar al dado, llama al método `Roll()`.
    -   `public void Roll()`: Este método público se encarga de iniciar el lanzamiento del dado. Solo procede si el momento actual en el sistema `Combatjudge` es `SetMoments.PickDice`, momento en el que establece la bandera `roling` a `true`, lo que activa la lógica de `Update()` para la simulación del lanzamiento.
    -   `private void OnMouseExit()`: Se invoca cuando el cursor del ratón sale del Collider del dado. Si es el turno para enfocar al dado, llama al método `Unroll()`.
    -   `public void Unroll()`: Este método público detiene el proceso de lanzamiento del dado. Si la bandera `roling` es `true`, la establece a `false` y notifica al sistema `Combatjudge` que el dado ha finalizado su lanzamiento (`Combatjudge.combatjudge.Roled()`).
    -   `private void OnMouseUp()`: Se invoca cuando el usuario suelta el botón del ratón mientras el cursor está sobre el Collider del dado. Similar a `OnMouseExit()`, llama a `Unroll()` si es el turno para enfocar al dado.

-   **Lógica Clave:**
    La lógica central del dado reside en el estado `roling` y su interacción con los eventos del ratón y el sistema `Combatjudge`. Cuando un jugador hace clic o mantiene el ratón sobre el dado en el momento correcto (`FocusONTurn()` y `SetMoments.PickDice`), la bandera `roling` se activa. Mientras `roling` es `true`, el dado "gira" visualmente en cada frame, actualizando su valor y rotación, y notificando al `Combatjudge` que está en proceso de lanzamiento. Al soltar el clic o mover el ratón fuera del dado, la bandera `roling` se desactiva, deteniendo la rotación y notificando al `Combatjudge` que el lanzamiento ha finalizado. La rotación visual del dado es una simulación sencilla, asignando una orientación fija para cada posible valor del dado (1-6).

## 3. Dependencias y Eventos

-   **Componentes Requeridos:**
    Este script debe estar adjunto a un GameObject que posea un componente Collider (por ejemplo, `BoxCollider`, `SphereCollider`, `MeshCollider`) para que los métodos `OnMouseDown`, `OnMouseExit` y `OnMouseUp` funcionen correctamente y detecten las interacciones del ratón.

-   **Eventos (Entrada):**
    El script reacciona a los siguientes eventos de entrada implícitos de Unity, que se disparan sobre el Collider del GameObject:
    -   `OnMouseDown()`: Cuando se pulsa el botón principal del ratón.
    -   `OnMouseExit()`: Cuando el cursor del ratón sale del Collider.
    -   `OnMouseUp()`: Cuando se suelta el botón principal del ratón.

-   **Eventos (Salida):**
    Este script no invoca eventos de `UnityEvent` o `Action` directamente para ser suscritos por otros componentes. En su lugar, interactúa directamente con el sistema `Combatjudge` a través de llamadas a métodos estáticos (o de singleton, asumiendo `Combatjudge.combatjudge` es una instancia accesible globalmente):
    -   `Combatjudge.combatjudge.StartRoling()`: Notifica al sistema de combate que el dado ha comenzado a "rodar".
    -   `Combatjudge.combatjudge.Roled()`: Notifica al sistema de combate que el dado ha terminado de "rodar" y su valor está listo.
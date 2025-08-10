# `dice.cs`

## 1. Propósito General
Este script gestiona el comportamiento y la visualización de un dado individual dentro del juego. Es responsable de simular el "lanzamiento" del dado al generar valores aleatorios y de informar el resultado final al sistema de combate principal, `Combatjudge`.

## 2. Componentes Clave

### `dice`
- **Descripción:** La clase `dice` es un `MonoBehaviour` que controla un objeto dado en la escena de Unity. Se encarga de la lógica de "lanzamiento" (simulada por un cambio rápido de números), la determinación del valor final y su visualización. Requiere un componente `Collider` en el mismo GameObject para detectar interacciones del ratón y un componente `TextMeshPro` como hijo para mostrar el valor del dado.

- **Variables Públicas / Serializadas:**
    - `int value`: Representa el valor numérico actual que el dado está mostrando. Este valor cambia rápidamente durante la fase de "lanzamiento".
    - `int maxValue`: Define el valor máximo posible que este dado puede alcanzar al rodar (e.g., 6 para un dado estándar). Este límite se obtiene del sistema `Combatjudge`.
    - `TextMeshPro texter`: Es una referencia al componente `TextMeshPro` que se encarga de renderizar y mostrar el valor numérico del dado en la interfaz. Se espera que este componente sea un hijo del GameObject que contiene el script.
    - `bool roling`: Una bandera booleana que indica si el dado está actualmente en su estado de "lanzamiento" activo. Cuando es `true`, el dado genera continuamente números aleatorios.

- **Métodos Principales:**
    - `void Start()`: Se invoca una vez al inicio del ciclo de vida del script. Su función es inicializar `maxValue` obteniéndolo de `Combatjudge.combatjudge.maxDice`, establecer `roling` a `false` (el dado no comienza rodando) y obtener una referencia al componente `TextMeshPro` que se encuentra entre sus hijos, para poder actualizar el texto.
    - `void Update()`: Este método se ejecuta una vez por cada fotograma del juego. Si la variable `roling` es `true`, el método genera un nuevo valor aleatorio para `value` (entre 1 y `maxValue` inclusive), simulando visualmente el rodar del dado. Posteriormente, actualiza el texto del componente `TextMeshPro` (`texter.text`) para mostrar el `value` actual.
    - `private void OnMouseDown()`: Un evento de Unity que se dispara cuando el botón del ratón es presionado sobre el `Collider` del GameObject. El dado solo empezará a "rodar" (`roling = true`) si el estado actual del juego (consultado a `Combatjudge.combatjudge.GetSetMoments()`) es `SetMoments.PickDice` y si es el turno del jugador para interactuar con el dado (`Combatjudge.combatjudge.FocusONTurn()`).
    - `private void OnMouseExit()`: Este evento de Unity se activa cuando el cursor del ratón abandona el `Collider` del GameObject después de haber sido presionado. Si el dado estaba en proceso de "rodar" (`roling` era `true`), este método detiene el "lanzamiento" (`roling = false`) y comunica el valor final de `value` al sistema `Combatjudge` a través del método `Combatjudge.combatjudge.Roled()`.
    - `private void OnMouseUp()`: Similar a `OnMouseExit()`, este evento de Unity se invoca cuando el botón del ratón es liberado mientras el cursor está sobre el `Collider` del GameObject. Si el dado estaba "rodando", se detiene el "lanzamiento" (`roling = false`) y se notifica el valor final de `value` al `Combatjudge` mediante `Combatjudge.combatjudge.Roled()`.

- **Lógica Clave:**
    La simulación del "lanzamiento" del dado se logra mediante la actualización continua del `value` con números aleatorios en cada `Update` mientras la bandera `roling` está activa. El proceso de "rodar" se inicia cuando el jugador hace clic sobre el dado bajo ciertas condiciones de juego (`OnMouseDown`). El "lanzamiento" se detiene y el valor final del dado se fija cuando el jugador suelta el clic o arrastra el ratón fuera del dado (`OnMouseExit` o `OnMouseUp`), momento en el que se informa el resultado al sistema de combate.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Aunque no se usa el atributo `[RequireComponent]`, este script necesita implícitamente un componente `Collider` (como `BoxCollider` o `MeshCollider`) en el mismo GameObject para detectar las interacciones del ratón. También espera un componente `TextMeshPro` como hijo del GameObject para mostrar el valor del dado.

- **Eventos (Entrada):**
    Este script responde a eventos de interacción del ratón integrados en Unity: `OnMouseDown()`, `OnMouseExit()`, y `OnMouseUp()`. Además, consulta el estado del juego y el foco del turno al sistema `Combatjudge` llamando a `Combatjudge.combatjudge.GetSetMoments()` y `Combatjudge.combatjudge.FocusONTurn()`.

- **Eventos (Salida):**
    Una vez que el dado ha "rodado" y su valor final ha sido determinado, este script notifica el resultado al sistema de combate llamando al método `Combatjudge.combatjudge.Roled(value)`, pasando el valor final del dado como parámetro.
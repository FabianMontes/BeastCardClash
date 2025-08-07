# `dice.cs`

## 1. Propósito General
Este script gestiona el comportamiento y la visualización de un dado individual dentro del juego. Se encarga de simular el "lanzamiento" visual del dado y de comunicar el valor final obtenido al sistema de lógica de combate (`Combatjudge`).

## 2. Componentes Clave

### `dice`
- **Descripción:** La clase `dice` es un componente de Unity (`MonoBehaviour`) que debe adjuntarse a un objeto de juego con un collider. Su función principal es representar un dado, permitiendo al jugador interactuar con él para iniciar una tirada simulada y capturar el resultado.
- **Variables Públicas / Serializadas:**
    - `int value`: Almacena el valor numérico actual que el dado está mostrando o ha finalizado. Durante el "lanzamiento" visual, este valor cambia constantemente.
    - `int maxValue`: Define el valor máximo posible que este dado puede obtener en una tirada. Se inicializa al inicio del juego a partir de `Combatjudge.combatjudge.maxDice`.
    - `TextMeshPro texter`: Una referencia al componente `TextMeshPro` que se encuentra en un objeto hijo del dado. Este componente se utiliza para renderizar el `value` actual del dado en la interfaz gráfica.
    - `bool roling`: Una bandera booleana que indica si el dado está actualmente en el proceso de "lanzamiento" visual. Cuando `true`, el valor del dado se actualiza aleatoriamente cada fotograma.
- **Métodos Principales:**
    - `void Start()`: Este método se invoca una vez al inicio del ciclo de vida del script.
        - Inicializa `maxValue` obteniéndolo de la propiedad `maxDice` del singleton `Combatjudge.combatjudge`, asegurando que el dado conoce su rango de valores posible.
        - Establece `roling` a `false`, indicando que el dado no está "lanzándose" inicialmente.
        - Obtiene una referencia al componente `TextMeshPro` en un objeto hijo para poder actualizar el texto del dado.
    - `void Update()`: Se invoca una vez por fotograma.
        - Si la bandera `roling` es `true`, el `value` del dado se actualiza constantemente a un número aleatorio entre 1 y `maxValue` (inclusive), simulando visualmente el giro del dado.
        - En cada fotograma, el texto del componente `texter` se actualiza para mostrar el `value` actual.
    - `private void OnMouseDown()`: Un callback de Unity que se invoca cuando el usuario presiona el botón del ratón sobre el collider del objeto al que está adjunto este script.
        - Inicia el "lanzamiento" visual del dado (`roling = true`) solo si el estado actual del juego (obtenido de `Combatjudge.combatjudge.GetSetMoments()`) es `SetMoments.PickDice` y es el turno del jugador activo (`Combatjudge.combatjudge.FocusONTurn()`).
    - `private void OnMouseExit()`: Un callback de Unity que se invoca cuando el puntero del ratón sale del área del collider del objeto mientras el botón del ratón no está presionado.
        - Si el dado estaba "lanzándose" (`roling` es `true`), detiene el lanzamiento visual (`roling = false`) y notifica al sistema `Combatjudge` (`Combatjudge.combatjudge.Roled(value)`) el valor final en el que se detuvo el dado.
    - `private void OnMouseUp()`: Un callback de Unity que se invoca cuando el usuario suelta el botón del ratón mientras el puntero aún está sobre el collider del objeto.
        - Al igual que `OnMouseExit()`, si el dado estaba "lanzándose", detiene el lanzamiento visual y comunica el valor final al sistema `Combatjudge`.
- **Lógica Clave:**
    La lógica central del dado opera mediante un simple estado `roling`. Cuando `roling` es `true`, el dado actualiza su valor aleatoriamente cada fotograma, creando un efecto de "lanzamiento". Este estado se activa al hacer clic en el dado (`OnMouseDown`) bajo condiciones específicas del juego. El "lanzamiento" se detiene y el valor final se comunica a `Combatjudge` cuando el ratón se levanta del dado (`OnMouseUp`) o sale del collider del dado (`OnMouseExit`), asumiendo que el dado estaba activamente "lanzándose".

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, pero funcionalmente requiere la presencia de:
    - Un `Collider` en el GameObject para detectar interacciones del ratón (`OnMouseDown`, `OnMouseExit`, `OnMouseUp`).
    - Un componente `TextMeshPro` en uno de sus objetos hijos para mostrar el valor del dado.
- **Eventos (Entrada):**
    - Este script se suscribe implícitamente a los eventos del sistema de entrada de Unity para interacciones con el ratón:
        - `OnMouseDown`: Se activa cuando el botón del ratón es presionado sobre el dado.
        - `OnMouseExit`: Se activa cuando el puntero del ratón sale del collider del dado.
        - `OnMouseUp`: Se activa cuando el botón del ratón es liberado sobre el dado.
- **Eventos (Salida):**
    - Este script notifica al sistema de combate a través de una llamada estática/singleton:
        - `Combatjudge.combatjudge.Roled(value)`: Invoca este método para comunicar el valor final obtenido del dado una vez que el lanzamiento ha terminado (ya sea por `OnMouseUp` o `OnMouseExit`).
# `dice.cs`

## 1. Propósito General
Este script gestiona la funcionalidad de un dado interactivo dentro del juego. Se encarga de simular el "lanzamiento" de un dado, mostrando un valor aleatorio mientras se está "rodando" y reportando el valor final a un sistema central de combate cuando el dado deja de rodar. Su interacción principal es con el sistema `Combatjudge`, que dictamina cuándo el dado puede ser lanzado y recibe el resultado del lanzamiento.

## 2. Componentes Clave

### `dice`
- **Descripción:** La clase `dice` es un `MonoBehaviour` que controla el comportamiento y la visualización de un objeto dado en la escena. Permite a los jugadores interactuar con el dado (simulando un lanzamiento) para obtener un valor numérico que presumiblemente se utiliza en la lógica del combate.

- **Variables Públicas / Serializadas:**
    Este script no expone variables directamente serializables en el Inspector de Unity (`[SerializeField]`) ni variables públicas. Todas las variables son privadas y gestionadas internamente por el script:
    - `int value`: Almacena el valor actual del dado. Este valor se actualiza constantemente mientras el dado está "rodando" y representa el resultado final cuando el lanzamiento termina.
    - `int maxValue`: Define el valor máximo que el dado puede alcanzar. Se inicializa al inicio del juego a partir de una configuración del sistema de combate.
    - `TextMeshPro texter`: Es una referencia al componente `TextMeshPro` que se encuentra en un objeto hijo del dado. Se utiliza para mostrar visualmente el `value` actual del dado en el juego.
    - `bool roling`: Una bandera booleana que indica si el dado está actualmente en estado de "rodar". Cuando es `true`, el dado actualiza su `value` de forma aleatoria en cada frame.

- **Métodos Principales:**
    - `void Start()`: Este método se ejecuta una única vez al inicio del ciclo de vida del script. Su propósito es inicializar las variables clave del dado:
        - `maxValue` se obtiene de `Combatjudge.combatjudge.maxDice`, estableciendo el rango superior para los posibles resultados del dado.
        - `roling` se establece inicialmente a `false`, asegurando que el dado no comience a rodar automáticamente.
        - `texter` se inicializa obteniendo una referencia al componente `TextMeshPro` que es hijo del GameObject al que está adjunto este script.

    - `void Update()`: Este método se ejecuta en cada frame del juego. Su lógica principal es:
        - Si la bandera `roling` es `true`, el `value` del dado se actualiza con un número aleatorio entre 1 y `maxValue` (inclusive). Esto crea el efecto visual de un dado rodando rápidamente a través de diferentes valores.
        - Independientemente del estado de `roling`, el texto del `TextMeshPro` (`texter.text`) se actualiza para mostrar el `value` actual del dado.

    - `private void OnMouseDown()`: Este método de callback de Unity se invoca cuando el usuario presiona el botón del ratón sobre el collider del GameObject al que está adjunto este script.
        - Contiene una condición crucial: el dado solo puede comenzar a "rodar" si el sistema `Combatjudge` indica que el momento actual del juego es `SetMoments.PickDice` y que la atención está en el turno actual (`Combatjudge.combatjudge.FocusONTurn()` es `true`).
        - Si ambas condiciones se cumplen, la bandera `roling` se establece a `true`, iniciando el proceso de "rodado" del dado.

    - `private void OnMouseExit()`: Este método de callback se invoca cuando el cursor del ratón abandona el área del collider del GameObject mientras el botón del ratón está presionado o después de haberlo presionado.
        - Si el dado estaba "rodando" (`roling` es `true`), detiene el rodado (`roling = false`) y notifica el valor final del dado al sistema de combate a través de `Combatjudge.combatjudge.Roled(value)`.

    - `private void OnMouseUp()`: Este método de callback se invoca cuando el usuario suelta el botón del ratón mientras el cursor está sobre el collider del GameObject.
        - Similar a `OnMouseExit()`, si el dado estaba "rodando" (`roling` es `true`), detiene el rodado (`roling = false`) y envía el valor final del dado al sistema de combate mediante `Combatjudge.combatjudge.Roled(value)`.

- **Lógica Clave:**
    La lógica central del dado se basa en el estado booleano `roling`. Cuando `OnMouseDown` es activado bajo las condiciones de juego correctas (determinadas por `Combatjudge`), `roling` se vuelve `true`. Esto hace que el método `Update` comience a generar valores aleatorios rápidamente, simulando un lanzamiento. El "lanzamiento" se detiene y el valor final se fija cuando el ratón se suelta (`OnMouseUp`) o se sale del área del dado (`OnMouseExit`), momento en el cual el valor es comunicado al `Combatjudge` para su uso en la lógica del juego.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere que el GameObject o uno de sus hijos tenga un componente `TextMeshPro` (para la visualización del número) y un `Collider` (para detectar interacciones del ratón como `OnMouseDown`, `OnMouseExit`, `OnMouseUp`).

- **Eventos (Entrada):**
    Este script se suscribe implícitamente a los eventos de interacción del ratón de Unity para su propio GameObject:
    - `OnMouseDown()`: Se activa cuando el botón del ratón se presiona sobre el dado.
    - `OnMouseExit()`: Se activa cuando el cursor del ratón sale del área del dado.
    - `OnMouseUp()`: Se activa cuando el botón del ratón se suelta sobre el dado.

- **Eventos (Salida):**
    Este script no invoca `UnityEvent`s o `Action`s propios. Sin embargo, interactúa directamente con el sistema `Combatjudge` (asumiendo que `Combatjudge.combatjudge` es una instancia accesible, posiblemente un Singleton o una referencia estática):
    - Llama a `Combatjudge.combatjudge.Roled(value)` para notificar el valor final del dado al sistema de combate una vez que el "lanzamiento" ha terminado.
    - Lee propiedades y llama a métodos del `Combatjudge` como `Combatjudge.combatjudge.maxDice`, `Combatjudge.combatjudge.GetSetMoments()`, y `Combatjudge.combatjudge.FocusONTurn()` para determinar cuándo y cómo debe comportarse el dado.
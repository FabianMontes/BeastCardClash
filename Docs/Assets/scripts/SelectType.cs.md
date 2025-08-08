# `SelectType.cs`

## 1. Propósito General
Este script `SelectType` es un componente de la interfaz de usuario (UI) encargado de gestionar la visibilidad y la interacción con los elementos de selección de tipo o elemento en el juego. Su función principal es mostrar u ocultar estos elementos de la UI basándose en el estado actual del combate, y procesar la selección del jugador mediante interacción con el sistema central de lógica de combate (`Combatjudge`).

## 2. Componentes Clave

### `SelectType`
-   **Descripción:** Esta clase, que hereda de `MonoBehaviour`, controla un grupo específico de elementos de la UI diseñados para permitir al jugador elegir un "elemento" o "tipo" de combate. Se espera que este script esté adjunto a un GameObject padre que contenga como hijos los diferentes botones o representaciones visuales de los elementos seleccionables, así como un componente `Image` que actúe como fondo o contenedor visual para estos elementos. La clase se encarga de la lógica de cuándo estos elementos deben ser visibles y de comunicar la selección del jugador al sistema de combate principal.

-   **Variables Públicas / Serializadas:**
    *   `prevSetMoment`: Esta variable privada de tipo `SetMoments` (un enumerador que define los diferentes estados o fases del combate, presumiblemente definido en otro lugar) se utiliza para almacenar el estado del combate en el ciclo anterior. Su propósito es detectar cambios en el estado del juego, lo que permite que el script reaccione solo cuando el estado de combate transiciona a una fase relevante para la selección de tipo.

-   **Métodos Principales:**
    *   `void Start()`: Este método se invoca una única vez al inicio del ciclo de vida del script. Inicializa `prevSetMoment` al estado `SetMoments.PickDice` y, crucialmente, llama a `Visib(false)` para asegurar que los elementos de UI de selección de tipo estén ocultos al comenzar el juego. Esto establece un estado inicial donde la UI de selección no es visible hasta que sea necesaria.

    *   `void Update()`: Llamado en cada fotograma del juego, este método es el corazón de la lógica de detección de estados. Consulta continuamente el estado de combate actual a través de `Combatjudge.combatjudge.GetSetMoments()`. Si detecta un cambio de estado (`momo != prevSetMoment`), verifica si el nuevo estado es `SetMoments.SelecCombat`. Si es así, significa que es el momento apropiado para que el jugador realice una selección de tipo, por lo que invoca `Visib(true)` para hacer visibles los elementos de la UI. Finalmente, actualiza `prevSetMoment` con el estado actual para la próxima verificación.

    *   `private void Visib(bool isVisible)`: Un método auxiliar privado que controla la visibilidad de los elementos de la UI. Recibe un valor booleano `isVisible`. Utiliza `transform.GetChild(index).gameObject.SetActive(isVisible)` para activar o desactivar los GameObjects hijos con índices 0, 1, 2 y 3, que se asume que son los botones o iconos de los diferentes tipos. Además, habilita o deshabilita el componente `Image` del GameObject al que está adjunto este script, probablemente para mostrar u ocultar un panel de fondo.

    *   `public void PickElement(int element)`: Este método público está diseñado para ser invocado directamente por la UI, por ejemplo, cuando un botón de selección de tipo es presionado. Recibe un entero `element` que se castea a un tipo `Element` (otro enumerador, presumiblemente, que representa los diferentes tipos de combate). Llama a `Combatjudge.combatjudge.pickElement((Element)element)` para comunicar la selección del jugador al sistema de combate. Si esta llamada devuelve `true` (indicando que la selección fue exitosa y válida), el método invoca `Visib(false)` para ocultar inmediatamente los elementos de la UI de selección, ya que la elección del jugador ha sido procesada.

-   **Lógica Clave:**
    La lógica principal de `SelectType` gira en torno a una máquina de estados implícita gestionada por `Combatjudge`. El script permanece inactivo y con su UI oculta hasta que el estado del juego, monitoreado en `Update`, transiciona específicamente a `SetMoments.SelecCombat`. En ese momento, la UI se vuelve visible, permitiendo al jugador interactuar. Una vez que el jugador selecciona un tipo a través de `PickElement`, el script delega la validación y el procesamiento de esa selección al `Combatjudge`. Si la selección es confirmada por `Combatjudge`, la UI de selección se oculta nuevamente, esperando el próximo ciclo de combate donde pueda ser requerida. Este enfoque garantiza que la UI solo esté presente cuando sea contextualmente relevante para el jugador.

## 3. Dependencias y Eventos

-   **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere que el GameObject al que esté adjunto tenga un componente `Image` y que tenga al menos cuatro GameObjects hijos (índices 0-3) que representen los elementos de la UI de selección, para que el método `Visib` funcione correctamente.

-   **Eventos (Entrada):**
    El método `public void PickElement(int element)` está diseñado para ser suscrito a eventos de interacción de la UI, como el evento `OnClick()` de un `Button` en Unity. Esto significa que los botones de selección de tipo en la UI del juego invocarán directamente este método cuando sean presionados por el jugador.

-   **Eventos (Salida):**
    Este script no invoca directamente `UnityEvent`s o `Action`s para notificar a otros sistemas. En su lugar, interactúa con otros sistemas (específicamente con `Combatjudge`) a través de llamadas a métodos directas (`Combatjudge.combatjudge.GetSetMoments()` y `Combatjudge.combatjudge.pickElement(...)`). Esto sugiere un patrón de comunicación directa con un singleton o un objeto global del sistema de combate.
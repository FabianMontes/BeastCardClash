# `SelectType.cs`

## 1. Propósito General
Este script `SelectType` es un componente `MonoBehaviour` fundamental para la interfaz de usuario del juego. Su rol principal es gestionar la visibilidad de los elementos de la UI relacionados con la selección de tipo o elemento de combate, activándolos solo cuando el juego se encuentra en una fase específica y desactivándolos una vez que la selección ha sido realizada. Interactúa directamente con el sistema `Combatjudge` para consultar el estado actual del juego y notificar la selección del jugador.

## 2. Componentes Clave

### `SelectType`
La clase `SelectType` es un script de Unity que se adjunta a un GameObject en la escena. Es responsable de controlar una parte específica de la interfaz de usuario del juego.

*   **Descripción:** `SelectType` es una clase `MonoBehaviour` que controla la visibilidad de un grupo de elementos de la interfaz de usuario, presumiblemente botones o indicadores para la selección de un tipo o elemento en el combate. Monitorea el estado del juego a través del singleton `Combatjudge` para decidir cuándo mostrar u ocultar estos elementos.

*   **Variables Públicas / Serializadas:** No hay variables públicas o serializadas directamente expuestas en el Inspector de Unity en este script. La variable `prevSetMoment` es interna y se utiliza para rastrear el estado anterior del juego y detectar cambios eficientemente.

*   **Métodos Principales:**

    *   `void Start()`: Este método se ejecuta una vez al inicio del ciclo de vida del script. Inicializa `prevSetMoment` con el valor `SetMoments.PickDice` y oculta inmediatamente todos los elementos de la UI gestionados por este script llamando a `Visib(false)`. Esto asegura que la interfaz de selección no sea visible hasta que sea necesaria.

    *   `void Update()`: Ejecutándose en cada frame, este método es el corazón de la lógica de visibilidad. Obtiene el momento actual del juego (`SetMoments`) a través de `Combatjudge.combatjudge.GetSetMoments()`. Si el momento actual es diferente al `prevSetMoment` almacenado, el script verifica si el nuevo momento es `SetMoments.SelecCombat`. Si lo es, activa la visibilidad de los elementos de la UI llamando a `Visib(true)`. Finalmente, actualiza `prevSetMoment` al momento actual para la próxima iteración.

    *   `private void Visib(bool isVisible)`: Este es un método auxiliar privado que encapsula la lógica para activar o desactivar un conjunto de GameObjects hijos y el componente `Image` adjunto al propio GameObject de este script. Se asume que los hijos en los índices 0, 1, 2 y 3 son los elementos de la UI que deben ser controlados. El componente `Image` en el GameObject principal también se habilita o deshabilita.

    *   `public void PickElement(int element)`: Este método público está diseñado para ser invocado externamente, probablemente desde un evento de UI como el `onClick` de un botón. Recibe un entero `element` que se convierte al tipo `Element` (presumiblemente un `enum`). Llama a `Combatjudge.combatjudge.pickElement()` con el elemento seleccionado. Si `pickElement` devuelve `true` (indicando que la selección fue exitosa), el script oculta inmediatamente los elementos de la UI llamando a `Visib(false)`, lo que sugiere que la selección ha concluido para ese turno o fase.

*   **Lógica Clave:** La lógica principal del script se centra en una máquina de estados simplificada para la interfaz de usuario de selección de elementos. En cada `Update`, monitorea el estado global del juego (gestionado por `Combatjudge`). Cuando el juego entra en la fase `SetMoments.SelecCombat`, el script muestra su UI. Una vez que el jugador ha seleccionado un elemento a través del método `PickElement` (probablemente al hacer clic en un botón de UI), el script notifica a `Combatjudge` la selección y luego oculta su propia UI, esperando que el juego avance a la siguiente fase antes de volver a ser relevante.

## 3. Dependencias y Eventos

*   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, pero implícitamente requiere que el GameObject al que está adjunto tenga un componente `Image` y al menos cuatro GameObjects hijos en los índices 0 a 3, ya que `Visib` intenta acceder a ellos para controlar su visibilidad.

*   **Eventos (Entrada):**
    *   El método `PickElement(int element)` está diseñado para ser un *listener* o *callback* de un evento de UI, típicamente el `onClick()` de botones que representan las diferentes opciones de elementos.

*   **Eventos (Salida):**
    *   Este script no emite eventos (`UnityEvent` o `Action`) directamente. Sin embargo, su acción de salida principal es llamar a `Combatjudge.combatjudge.pickElement()`, lo que efectivamente notifica al sistema central de combate sobre la elección del jugador y potencialmente desencadena un cambio de estado en ese sistema. También tiene un efecto colateral de modificar la visibilidad de sus propios elementos de UI.
# `SelectType.cs`

## 1. Propósito General
Este script `SelectType` gestiona la visibilidad de un conjunto de elementos de la interfaz de usuario (UI) que permiten al jugador seleccionar un tipo o "elemento" durante una fase específica del combate. Su rol principal es mostrar u ocultar estas opciones de UI en función del estado actual del juego, interactuando directamente con el sistema `Combatjudge` para obtener el progreso del combate y comunicar la selección del jugador.

## 2. Componentes Clave

### `SelectType`
- **Descripción:** La clase `SelectType` es un componente de `MonoBehaviour` que se adjunta a un GameObject en la escena de Unity. Su función es controlar cuándo se muestran las opciones de selección de elemento al jugador y, posteriormente, enviar la selección al sistema de combate.
- **Variables Públicas / Serializadas:**
    - `prevSetMoment`: Una variable privada de tipo `SetMoments` que almacena la fase de combate detectada en el `Update` anterior. Se utiliza para identificar cuándo ha cambiado la fase de combate, evitando actualizaciones innecesarias de la visibilidad de la UI.
- **Métodos Principales:**
    - `void Start()`: Este método se ejecuta una vez al inicio del ciclo de vida del script.
        - Inicializa `prevSetMoment` a `SetMoments.PickDice` y llama a `Visib(false)` para asegurar que los elementos de UI de selección de tipo estén ocultos desde el principio.
    - `void Update()`: Se invoca en cada frame del juego.
        - Obtiene la fase actual del combate a través de `Combatjudge.combatjudge.GetSetMoments()`.
        - Si la fase actual (`momo`) es diferente de la fase anterior (`prevSetMoment`), verifica si la nueva fase es `SetMoments.SelecCombat`. Si lo es, hace visibles los elementos de UI de selección de tipo llamando a `Visib(true)`.
        - Finalmente, actualiza `prevSetMoment` a la fase actual para el próximo ciclo de `Update`.
    - `private void Visib(bool isVisible)`: Un método auxiliar privado que controla la visibilidad de los elementos de UI asociados.
        - Se espera que el GameObject al que está adjunto este script tenga al menos cuatro GameObjects hijos (índices 0 a 3) y un componente `Image` en el propio GameObject.
        - Este método activa o desactiva estos GameObjects hijos y habilita o deshabilita el componente `Image` basándose en el valor de `isVisible`.
    - `public void PickElement(int element)`: Este método público está diseñado para ser invocado por eventos de UI (como clics de botones).
        - Recibe un entero `element`, que se castea a un tipo `Element` (presumiblemente un `enum` definido en otro lugar).
        - Llama a `Combatjudge.combatjudge.pickElement((Element)element)` para comunicar la selección del jugador al sistema de combate.
        - Si `pickElement` devuelve `true` (lo que implica que la selección fue exitosa o se procesó), el script oculta inmediatamente los elementos de UI de selección de tipo llamando a `Visib(false)`.
- **Lógica Clave:**
    La lógica central del script reside en su bucle `Update`, que actúa como un simple observador del estado de la fase de combate. Cuando el `Combatjudge` indica que la fase actual ha cambiado a `SetMoments.SelecCombat`, el script reacciona mostrando las opciones de UI. Una vez que el jugador selecciona un elemento mediante el método `PickElement`, el script notifica al `Combatjudge` y luego se oculta, esperando la próxima vez que se requiera la selección de un tipo.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza `[RequireComponent]`. Sin embargo, funcionalmente requiere que el GameObject al que está adjunto tenga un componente `Image` y al menos cuatro GameObjects hijos, ya que el método `Visib` intenta acceder a ellos para controlar su visibilidad.
- **Dependencias Directas de Código:**
    - `Combatjudge.combatjudge`: El script interactúa directamente con una instancia singleton o estática de `Combatjudge` para obtener la fase de combate actual (`GetSetMoments()`) y para enviar la selección del elemento (`pickElement()`).
    - `SetMoments` y `Element`: Se asume la existencia de estas enumeraciones, que definen las fases del juego y los tipos de elementos seleccionables, respectivamente.
- **Eventos (Entrada):**
    - El método `public void PickElement(int element)` está diseñado para ser un "receptor" de eventos, típicamente invocado por componentes de UI de Unity como `Button.onClick` en el Inspector, donde se le pasa un valor entero que representa el elemento seleccionado.
- **Eventos (Salida):**
    - Este script no expone `UnityEvents` o `Actions` propios para notificar a otros sistemas. En su lugar, comunica la selección del elemento directamente llamando al método `Combatjudge.combatjudge.pickElement()`.
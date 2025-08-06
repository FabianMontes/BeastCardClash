# `SelectType.cs`

## 1. Propósito General
Este script gestiona la visibilidad y la interacción de una interfaz de usuario (UI) que permite al jugador seleccionar un tipo de elemento durante una fase específica del combate. Actúa como un controlador de UI que se activa o desactiva en función del estado actual del juego, interactuando directamente con el sistema de juicio de combate (`Combatjudge`) para registrar la selección del jugador.

## 2. Componentes Clave

### `SelectType`
- **Descripción:** La clase `SelectType` es un componente de Unity (`MonoBehaviour`) que controla un panel de selección de elementos en la UI. Su función principal es mostrar u ocultar este panel en el momento adecuado del juego y procesar la elección del jugador, comunicándola al sistema central de combate.

- **Variables Públicas / Serializadas:**
    - `prevSetMoment`: Esta variable privada de tipo `SetMoments` almacena el estado de combate anterior del juego. Se utiliza para detectar cuándo hay un cambio en el estado del juego, lo que permite al script reaccionar solo cuando es necesario, evitando comprobaciones redundantes. Aunque es privada, su rol es clave para la lógica de activación de la UI.

- **Métodos Principales:**
    - `void Start()`: Este método se llama una vez al inicio del ciclo de vida del script. Su propósito es inicializar la variable `prevSetMoment` al estado `SetMoments.PickDice` y, crucialmente, asegurar que el panel de selección de elementos esté oculto (`Visib(false)`) cuando el juego comienza, preparándolo para ser activado más tarde por la lógica del juego.

    - `void Update()`: Ejecutándose una vez por fotograma, `Update` es el corazón de la lógica de activación de la UI. En cada fotograma, consulta el estado actual del combate a través de `Combatjudge.combatjudge.GetSetMoments()`. Si detecta un cambio de estado y el nuevo estado es `SetMoments.SelecCombat`, hace visible la UI de selección de elementos. Esto asegura que la UI solo aparezca cuando el juego requiere que el jugador elija un elemento.

    - `private void Visib(bool isVisible)`: Una función auxiliar interna que controla la visibilidad de los elementos visuales que componen el panel de selección. Recibe un booleano `isVisible` y lo utiliza para activar o desactivar los primeros cuatro GameObjects hijos del `GameObject` al que está adjunto este script, así como el componente `Image` del propio `GameObject`. Esto sugiere que el panel de selección se compone de un fondo (`Image`) y cuatro opciones de selección representadas por los GameObjects hijos.

    - `public void PickElement(int element)`: Este método público está diseñado para ser invocado por eventos de UI, como la pulsación de un botón. Toma un entero `element` (que se espera que represente un tipo de `Element` mediante un cast implícito) y lo pasa al sistema `Combatjudge` a través de `Combatjudge.combatjudge.pickElement()`. Si el intento de seleccionar el elemento es exitoso (es decir, `pickElement` devuelve `true`), el panel de selección se oculta automáticamente, indicando que la elección ha sido procesada y no es necesaria una interacción adicional del usuario en ese momento.

- **Lógica Clave:**
    La lógica central de `SelectType` reside en su capacidad para reaccionar a los cambios en el estado del combate. Utiliza la variable `prevSetMoment` dentro del método `Update` para detectar transiciones de estado. Cuando el sistema de combate, representado por `Combatjudge`, entra en la fase `SetMoments.SelecCombat`, este script se encarga de mostrar la UI de selección. Una vez que el jugador ha realizado una selección válida a través del método `PickElement`, el script notifica al `Combatjudge` y, si la selección es aceptada, oculta nuevamente la UI. Esto crea un flujo de usuario donde el panel de selección solo es visible y funcional en el momento exacto en que es relevante para la jugabilidad. La estructura `transform.GetChild(X).gameObject.SetActive(isVisible)` implica que este script espera una jerarquía específica de GameObjects para controlar su UI.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, asume la presencia de un componente `Image` en el mismo GameObject para controlar su visibilidad a través de `GetComponent<Image>().enabled`. Además, para su funcionamiento, los GameObjects hijos (al menos los primeros cuatro) deben existir en la jerarquía del GameObject para que la función `Visib` pueda activarlos/desactivarlos.

- **Eventos (Entrada):**
    - Este script se suscribe implícitamente a los cambios de estado del juego al consultar el método `GetSetMoments()` del singleton `Combatjudge.combatjudge` en cada `Update`.
    - El método `public void PickElement(int element)` está diseñado para ser invocado por eventos externos, probablemente asociados a componentes de UI como `Button.onClick` en el Inspector de Unity, donde cada botón pasaría un valor `int` específico que corresponde a un tipo de `Element`.

- **Eventos (Salida):**
    - Este script invoca el método `pickElement((Element)element)` en el singleton `Combatjudge.combatjudge`, comunicando la selección del jugador al sistema de combate principal. No emite sus propios `UnityEvent` o `Action` para notificar a otros sistemas, su interacción de "salida" es directa con `Combatjudge`.
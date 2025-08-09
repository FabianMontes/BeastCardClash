# `SelectType.cs`

## 1. Propósito General
Este script tiene como propósito principal gestionar la visibilidad y la interacción de la interfaz de usuario (UI) para la selección de tipos elementales (como "Fuego", "Agua", etc.) durante una fase específica del combate. Su función es aparecer en el momento adecuado del juego, permitir al jugador elegir un elemento y luego ocultarse, comunicando la selección al sistema de combate principal.

## 2. Componentes Clave

### `SelectType`
- **Descripción:** La clase `SelectType` es un componente de Unity (`MonoBehaviour`) que se adjunta a un `GameObject` en la escena. Este `GameObject` se espera que contenga los elementos de la UI (típicamente botones para cada tipo elemental y una imagen de fondo) que el jugador utilizará para hacer su selección. El script controla cuándo estos elementos de la UI deben ser visibles y procesa la selección del jugador.

- **Variables Públicas / Serializadas:**
    - `prevSetMoment` (privada): Esta variable de tipo `SetMoments` (un `enum` definido en otro lugar, presumiblemente en `Combatjudge`) se utiliza para almacenar el estado del juego en el fotograma anterior. Es crucial para que el script pueda detectar cuándo el estado del combate ha cambiado a la fase de selección de tipo, lo que activa la UI.

- **Métodos Principales:**
    - `void Start()`: Este método se ejecuta una vez al inicio del ciclo de vida del script, antes del primer `Update`. Su función es inicializar `prevSetMoment` a `SetMoments.PickDice` y, de inmediato, ocultar todos los elementos de la UI de selección llamando a `Visib(false)`. Esto asegura que la UI no sea visible al inicio del juego.
    - `void Update()`: Este método se invoca en cada fotograma del juego. Dentro de él, el script consulta al `Combatjudge` (a través de su instancia estática `Combatjudge.combatjudge`) para obtener el estado actual del combate (`SetMoments`). Si este estado actual es diferente al `prevSetMoment` almacenado y el nuevo estado es `SetMoments.SelecCombat`, el script procede a hacer visible la UI de selección (`Visib(true)`). Finalmente, actualiza `prevSetMoment` con el estado actual.
    - `private void Visib(bool isVisible)`: Un método auxiliar privado que encapsula la lógica para mostrar u ocultar los elementos de la UI. Toma un parámetro booleano `isVisible`. Cuando se llama, itera sobre los primeros cuatro GameObjects hijos del `GameObject` al que `SelectType` está adjunto, activando o desactivando su propiedad `activeSelf`. Adicionalmente, habilita o deshabilita el componente `Image` presente en el propio `GameObject` de `SelectType`. Esto sugiere una estructura UI donde los botones de selección son hijos y el fondo es la `Image` del padre.
    - `public void PickElement(int element)`: Este es un método público diseñado para ser invocado por un evento de UI, como el `OnClick` de un botón. Recibe un valor entero `element` que representa el tipo elemental elegido por el jugador. Este entero se convierte al `enum` `Element` (también presumiblemente definido en `Combatjudge`) y se pasa al método `pickElement` del `Combatjudge`. Si `Combatjudge.pickElement` procesa la selección exitosamente (devuelve `true`), el método `PickElement` procede a ocultar la UI de selección llamando a `Visib(false)`.

- **Lógica Clave:**
    La lógica principal de `SelectType` reside en su capacidad para reaccionar a los cambios en el estado del combate. Utiliza un patrón de "máquina de estados simple" monitoreando el `SetMoments` del `Combatjudge`. Cuando el combate entra en la fase `SetMoments.SelecCombat`, la UI se muestra. Una vez que el jugador ha hecho una selección válida que es aceptada por el `Combatjudge`, la UI se oculta automáticamente, indicando que la fase de selección ha concluido.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, su funcionalidad depende intrínsecamente de que el `GameObject` al que está adjunto tenga una estructura específica:
    *   Debe tener un componente `Image` adjunto, ya que `Visib` intenta habilitar/deshabilitar este componente.
    *   Debe tener al menos cuatro GameObjects hijos, ya que `Visib` manipula la propiedad `activeSelf` de los hijos en los índices 0, 1, 2 y 3. Estos hijos son presumiblemente los botones para cada tipo elemental.
- **Eventos (Entrada):** El método `public void PickElement(int element)` está explícitamente diseñado para ser el receptor de eventos de la UI, como los que se disparan cuando un `Button` es clicado. Los botones de la UI de selección deben ser configurados en el Inspector de Unity para llamar a este método, pasando el entero correspondiente al elemento que representan.
- **Eventos (Salida):** Este script no emite eventos de Unity (`UnityEvent` o `Action`) para notificar a otros sistemas sobre la selección de un elemento. En cambio, se comunica directamente con el sistema `Combatjudge` llamando a su método `pickElement` para pasar la elección del jugador. La acción de ocultar la UI es una consecuencia interna de esta comunicación exitosa.
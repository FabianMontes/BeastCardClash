# `dice.cs`

## 1. Propósito General
El script `dice.cs` gestiona el comportamiento y la representación visual de un dado interactivo en el juego. Su función principal es permitir al jugador "lanzar" un dado digital, mostrar su valor actual y comunicar el resultado final del lanzamiento al sistema de juicio de combate (`Combatjudge`).

## 2. Componentes Clave

### `dice`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, controla la lógica y la interfaz de usuario de un dado individual dentro del entorno de Unity. Se encarga de la visualización del valor del dado, la simulación de su "lanzamiento" (mediante un cambio rápido de valores) y la interacción del usuario a través del ratón.

- **Variables Internas Clave:**
    - `int value`: Almacena el valor numérico actual que muestra el dado. Este valor se actualiza constantemente mientras el dado está "rodando" y representa el resultado final una vez que se detiene.
    - `int maxValue`: Define el valor máximo posible que este dado puede alcanzar en un lanzamiento. Se inicializa al inicio del juego a partir de la configuración global de los dados del sistema de combate.
    - `TextMeshPro texter`: Una referencia al componente `TextMeshPro` que se encuentra como hijo del GameObject del dado. Este componente es utilizado para mostrar visualmente el `value` actual del dado.
    - `bool roling`: Un indicador booleano que determina si el dado está actualmente en estado de "lanzamiento". Cuando es `true`, el dado cambia rápidamente su `value` a números aleatorios, simulando un rodar.

- **Métodos Principales:**
    - `void Start()`: Se llama una vez al inicio, justo antes de la primera actualización del frame. Este método inicializa `maxValue` obteniendo el valor máximo de los dados del `Combatjudge.combatjudge`, establece el estado `roling` en `false` (el dado no está rodando inicialmente) y obtiene la referencia al componente `TextMeshPro` para la visualización.
        ```csharp
        void Start()
        {
            maxValue = Combatjudge.combatjudge.maxDice;
            roling = false;
            texter = GetComponentInChildren<TextMeshPro>();
        }
        ```
    - `void Update()`: Se invoca en cada frame del juego. Si `roling` es `true`, actualiza continuamente `value` a un número aleatorio entre 1 y `maxValue`. Independientemente del estado de `roling`, este método siempre actualiza el texto del componente `TextMeshPro` para mostrar el `value` actual.
        ```csharp
        void Update()
        {
            if (roling)
            {
                value = Random.Range(1, maxValue + 1);
            }
            texter.text = value.ToString();
        }
        ```
    - `void OnMouseDown()`: Un método de callback de Unity que se dispara cuando el botón del ratón se presiona mientras el puntero está sobre el collider del GameObject al que está adjunto este script. Si las condiciones actuales del juego (determinadas por `Combatjudge.combatjudge`) permiten seleccionar un dado para el lanzamiento y el turno está enfocado correctamente, este método establece `roling` a `true`, iniciando el proceso de "lanzamiento" visual del dado.
        ```csharp
        private void OnMouseDown()
        {
            if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickDice && Combatjudge.combatjudge.FocusONTurn())
            {
                roling = true;
            }
        }
        ```
    - `void OnMouseExit()`: Otro método de callback de Unity que se invoca cuando el puntero del ratón sale del collider del GameObject. Si el dado estaba actualmente "rodando" (`roling` es `true`), este método detiene el lanzamiento (`roling = false`) y notifica al sistema de combate (`Combatjudge.combatjudge`) del valor final del dado.
    - `void OnMouseUp()`: Similar a `OnMouseExit()`, este callback de Unity se dispara cuando el botón del ratón se suelta mientras el puntero está sobre el collider del GameObject. Si el dado estaba "rodando", detiene el lanzamiento y comunica el valor final al sistema de combate. Esto asegura que el lanzamiento se finalice tanto si el ratón se arrastra fuera del dado como si se suelta directamente sobre él.

- **Lógica Clave:**
    La lógica central del dado se basa en una máquina de estados simple controlada por la variable `roling`. Cuando `roling` es `true` (activado por `OnMouseDown` bajo condiciones específicas del juego), el método `Update` simula un lanzamiento continuo cambiando el valor mostrado rápidamente. El lanzamiento se detiene (`roling` se establece en `false`) y el valor final se comunica al sistema de combate (`Combatjudge`) cuando el ratón sale del collider del dado (`OnMouseExit`) o cuando el botón del ratón se suelta (`OnMouseUp`). Este diseño permite una interacción intuitiva donde el usuario puede arrastrar el ratón para "lanzar" el dado y el valor se "fija" al soltar el clic o arrastrar fuera.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script requiere la presencia de un componente `Collider` (como un `BoxCollider` o `SphereCollider`) en el mismo GameObject para detectar las interacciones del ratón (`OnMouseDown`, `OnMouseExit`, `OnMouseUp`). También espera encontrar un componente `TextMeshPro` como hijo de su GameObject para la visualización del número.

- **Eventos (Entrada):**
    El script `dice` se suscribe implícitamente a los eventos de entrada del ratón de Unity (`OnMouseDown`, `OnMouseExit`, `OnMouseUp`) para gestionar la interacción del usuario con el dado.

- **Eventos (Salida):**
    Este script no invoca explícitamente eventos (`UnityEvent`, `Action`) definidos dentro de sí mismo. Sin embargo, actúa como un emisor de información al interactuar directamente con el singleton `Combatjudge.combatjudge`, llamando al método `Roled(value)` para notificar el resultado de un lanzamiento de dado. Esta interacción es crucial para que el sistema de combate procese los resultados del lanzamiento.
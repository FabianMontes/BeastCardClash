# `PlayZone.cs`

## 1. Propósito General
Este script es responsable de inicializar y configurar dinámicamente el "campo de juego" del juego "Beast Card Clash". Gestiona la creación y disposición circular de objetos tipo "roca" (`rockPrefab`), asignándoles propiedades y configuraciones iniciales. Interactúa estrechamente con el componente `RockBehavior` de cada roca para establecer su comportamiento y apariencia.

## 2. Componentes Clave

### `SetupConfig` (Enum)
- **Descripción:** Este `enum` define los diferentes modos de configuración para la generación de las "inscripciones" (tipos de elementos) en las rocas. Controla el patrón de asignación de `Inscription` a las rocas cuando se crea el campo de juego.
    - `normal`: Asigna inscripciones en un patrón cíclico y alternado.
    - `fullall`: Asigna la inscripción `Inscription.pick` a todas las rocas.
    - `fullone`: Asigna la inscripción `Inscription.duel` a todas las rocas.

### `PlayZone` (Clase `MonoBehaviour`)
- **Descripción:** La clase principal de este archivo, hereda de `MonoBehaviour`, lo que le permite ser adjuntada a un GameObject en Unity. Su función es construir la zona de juego inicial, instanciando y posicionando múltiples copias de un prefab de roca (`rockPrefab`) en una formación circular. También se encarga de configurar las propiedades iniciales de cada roca a través de su componente `RockBehavior`.
- **Variables Públicas / Serializadas:**
    - `radius` (`float`): Determina el radio del círculo en el que se distribuirán las rocas. Esta variable es visible y configurable desde el Inspector de Unity.
    - `many` (`int`): Define el número total de rocas que se instanciarán alrededor del círculo. Es visible y configurable en el Inspector.
    - `config` (`SetupConfig`): Un selector para el modo de configuración que se utilizará para asignar los tipos de `Inscription` a las rocas generadas. Visible en el Inspector.
    - `RockScale` (`float`): Define la escala a la que se instanciarán los GameObjects de las rocas. Aunque está declarada como pública y serializada, no se utiliza directamente en el método `Start` proporcionado. Visible en el Inspector.
    - `rockPrefab` (`GameObject`): La referencia al prefab de la roca que será clonada para poblar el campo de juego. Debe ser asignado desde el Inspector.

- **Métodos Principales:**
    - `void Start()`:
        - **Descripción:** Este es un método del ciclo de vida de Unity, llamado una vez al inicio del juego después de que el script ha sido inicializado. Es aquí donde se implementa la lógica principal para crear la zona de batalla.
        - **Lógica Clave:**
            1.  Calcula el ángulo necesario para distribuir `many` rocas uniformemente en un círculo completo (360 grados).
            2.  Itera `many` veces, una por cada roca a crear. En cada iteración:
                *   Calcula las coordenadas `x` y `z` para posicionar la roca en el círculo utilizando funciones trigonométricas (coseno y seno) y el `radius`.
                *   Determina el tipo de `Inscription` para la roca actual. Esta asignación varía según el valor de la variable `config`:
                    *   Si `config` es `normal`, las inscripciones se asignan en un patrón que alterna entre cuatro elementos (`elem`) y ocasionalmente introduce dos elementos "no únicos" (`nonelem`). La lógica específica de `Inscription` (los valores numéricos asociados) no se define en este archivo, lo que indica que `Inscription` es un `enum` definido externamente.
                    *   Si `config` es `fullall`, a todas las rocas se les asigna `Inscription.pick`.
                    *   Si `config` es `fullone`, a todas las rocas se les asigna `Inscription.duel`.
                *   Instancia una copia del `rockPrefab` en la escena.
                *   Establece la instancia de `rockPrefab` como hija del GameObject al que está adjunto este script `PlayZone`.
                *   Obtiene el componente `RockBehavior` de la roca recién creada y le asigna varias propiedades cruciales:
                    *   `father`: Una referencia a esta misma instancia de `PlayZone`.
                    *   `angle`: El ángulo negativo de su posición, potencialmente usado para la rotación o cálculos internos de `RockBehavior`.
                    *   `direction`: Un `Vector3` que representa la dirección desde el centro a la roca.
                    *   `inscription`: El tipo de inscripción determinado por la lógica de configuración.
                    *   `numbchild`: El índice `i` de la iteración, indicando el número de orden de la roca.
    - `void Update()`:
        - **Descripción:** Este método del ciclo de vida de Unity se llama una vez por cada frame. En el código actual, está vacío y no contiene ninguna lógica activa.

## 3. Dependencias y Eventos

- **Componentes Requeridos:**
    *   Este script no utiliza el atributo `[RequireComponent()]`. Sin embargo, es una dependencia crítica que cualquier `GameObject` asignado a la variable `rockPrefab` **debe tener un componente `RockBehavior` adjunto**, ya que `PlayZone` accede y modifica directamente sus propiedades (`father`, `angle`, `direction`, `inscription`, `numbchild`). Sin este componente, se produciría un error en tiempo de ejecución.
    *   Se requiere que exista un `enum` llamado `Inscription` definido en otro archivo o globalmente en el proyecto, ya que `PlayZone` hace referencia a él y sus valores para configurar las rocas.

- **Eventos (Entrada):**
    *   Este script no se suscribe a eventos externos de Unity (como clics de botón o notificaciones de otros scripts) en el código proporcionado. Su inicialización ocurre automáticamente a través del método de ciclo de vida `Start`.

- **Eventos (Salida):**
    *   Este script no invoca ningún `UnityEvent` ni `Action` para notificar a otros sistemas. Su función principal es la configuración directa de GameObjects y sus componentes.
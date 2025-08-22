# `PlayZone.cs`

## 1. Propósito General
Este script `PlayZone` es el responsable principal de la creación y configuración dinámica de la zona de juego o "estadio" en Beast Card Clash. Genera proceduralmente una disposición circular de elementos del tablero, representados por "rocas" (stones), asignándoles propiedades iniciales como su posición, orientación e "inscripción" basada en diferentes modos de configuración.

## 2. Componentes Clave

### `enum SetupConfig`
- **Descripción:** Este enumerador define los diferentes esquemas o modos de distribución de las "inscripciones" (tipos o propiedades) que serán asignadas a las rocas generadas en el campo de batalla.
- **Valores:**
    - `normal`: Un modo de configuración que distribuye inscripciones de forma variada, alternando entre un conjunto principal de elementos y elementos "no únicos" especiales.
    - `fullall`: Asigna la inscripción `Inscription.pick` a todas las rocas.
    - `fullone`: Asigna la inscripción `Inscription.duel` a todas las rocas.

### `PlayZone`
- **Descripción:** La clase `PlayZone` es un `MonoBehaviour` que gestiona la generación del entorno de juego circular. Su propósito principal es instanciar múltiples objetos `rockPrefab` en una formación circular y configurar sus propiedades iniciales, como su comportamiento y tipo de inscripción, al inicio del juego.
- **Variables Públicas / Serializadas:**
    - `[SerializeField] public float radius = 5;`: Define el radio del círculo sobre el cual se distribuirán las rocas. Es la distancia desde el centro de la `PlayZone` a cada roca.
    - `[SerializeField] public int many = 10;`: Determina la cantidad total de rocas que se generarán en la zona de juego.
    - `[SerializeField] SetupConfig config;`: Selecciona el modo de configuración (`normal`, `fullall`, `fullone`) que determinará cómo se asignarán las inscripciones a cada roca.
    - `[SerializeField] public float RockScale = 1;`: Controla la escala visual de cada roca instanciada.
    - `[SerializeField] GameObject rockPrefab;`: Es la referencia al prefab del objeto `GameObject` que representa una "roca" individual. Este prefab debe contener el script `RockBehavior` para que la configuración funcione correctamente.
- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se ejecuta una vez al inicio del script. Contiene toda la lógica para crear la zona de batalla:
        - Calcula el ángulo necesario entre cada roca para distribuirlas uniformemente en un círculo.
        - Itera `many` veces, una por cada roca a generar.
        - En cada iteración, determina las coordenadas `x` y `z` para la posición de la roca en el círculo utilizando trigonometría (`Mathf.Cos`, `Mathf.Sin`).
        - Aplica la lógica del `switch (config)` para decidir qué tipo de `Inscription` (que se asume es un `enum` definido en otro lugar del proyecto, como `Inscription.empty`, `Inscription.pick`, `Inscription.duel`) se asignará a la roca actual.
        - Instancia un `GameObject` a partir de `rockPrefab`.
        - Configura el padre de la roca instanciada para que sea el `transform` de esta `PlayZone`, manteniendo la jerarquía organizada.
        - Obtiene el componente `RockBehavior` de la roca instanciada y le asigna varias propiedades cruciales:
            - `father`: Una referencia a este mismo script `PlayZone`.
            - `angle`: El ángulo de rotación de la roca, ajustado para que "mire" hacia el centro.
            - `direction`: El vector de dirección desde el centro hasta la roca.
            - `inscription`: La `Inscription` determinada por la lógica de configuración.
            - `numbchild`: El índice de la roca dentro del bucle de creación.
    - `void Update()`: Este método se ejecuta una vez por fotograma. En el código actual, está vacío y no realiza ninguna acción.
- **Lógica Clave:**
    La lógica principal reside en el método `Start()`, donde se genera el "estadio" circular. Se calcula el ángulo para cada posición basándose en la cantidad de rocas (`many`) y se utiliza `Mathf.Cos` y `Mathf.Sin` para obtener las coordenadas `x` y `z` en un círculo. La distribución de las `Inscription`s es modular y dependiente del `config` seleccionado:
    ```csharp
    // Ejemplo de cómo se determina la inscripción dentro del bucle de Start():
    switch (config)
    {
        case SetupConfig.normal:
            // Lógica para distribuir inscripciones normales y "no únicas"
            // (Inscription)(elem) o (Inscription)(nonelem + 4)
            break;
        case SetupConfig.fullall:
            inscripcion = Inscription.pick;
            break;
        case SetupConfig.fullone:
            inscripcion = Inscription.duel;
            break;
    }
    ```
    Cada roca instanciada (`stone`) obtiene una referencia a su padre (`PlayZone`), su posición angular, dirección y su inscripción, lo que permite que el `RockBehavior` asociado a cada roca funcione de manera contextual.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Este script no utiliza el atributo `[RequireComponent]` para forzar la presencia de otros componentes en el mismo `GameObject`.
    - Sin embargo, el script depende fuertemente de la existencia de un componente `RockBehavior` en el `rockPrefab` que se instanciará, ya que accede y establece sus propiedades directamente (`stone.GetComponent<RockBehavior>()....`). Si `RockBehavior` no está presente, se producirá un error en tiempo de ejecución.
- **Eventos (Entrada):**
    - Este script no se suscribe a eventos externos de otros componentes (como `button.onClick` o eventos personalizados). Su funcionalidad principal se activa y completa durante el método de ciclo de vida `Start()` de Unity.
- **Eventos (Salida):**
    - `PlayZone` no invoca ni emite `UnityEvents` o `Actions` para notificar a otros sistemas. Su interacción con otras partes del juego se realiza estableciendo directamente las propiedades del script `RockBehavior` en los GameObjects que instancia.
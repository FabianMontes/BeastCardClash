# `PlayZone.cs`

## 1. Propósito General
Este script tiene como propósito principal la gestión de la creación y configuración inicial del "estadio" o "zona de batalla" en el juego Beast Card Clash. Genera dinámicamente una serie de objetos de "roca" (representados por `rockPrefab`) alrededor de un punto central, asignándoles propiedades como su posición, rotación e inscripción (tipo de elemento o habilidad).

## 2. Componentes Clave

### `enum SetupConfig`
Esta enumeración define los posibles patrones de configuración para las inscripciones de las rocas generadas en el estadio.

*   **`normal`**: Configuración estándar que mezcla inscripciones elementales con inscripciones especiales, distribuyéndolas en un patrón predefinido.
*   **`fullall`**: Todas las rocas recibirán la inscripción `Inscription.pick`.
*   **`fullone`**: Todas las rocas recibirán la inscripción `Inscription.duel`.

### `PlayZone`
Esta clase hereda de `MonoBehaviour` y es el componente central para la generación del entorno de juego. Está marcada con `[DefaultExecutionOrder(-3)]`, lo que asegura que su método `Start` se ejecute muy temprano en el ciclo de vida de la escena, antes de la mayoría de otros scripts.

#### Variables Públicas / Serializadas:
Las siguientes variables son expuestas en el Inspector de Unity para permitir la configuración del estadio desde el editor:

*   **`radius`**: Un valor `float` que determina la distancia desde el centro del objeto `PlayZone` a la que se colocarán las rocas generadas. Esencialmente, define el radio del círculo de rocas.
*   **`many`**: Un valor `int` que especifica la cantidad total de rocas que se generarán alrededor del círculo.
*   **`config`**: Una variable de tipo `SetupConfig` que define el patrón de inscripciones que se aplicará a las rocas individuales. Esto permite cambiar rápidamente la configuración elemental del estadio.
*   **`RockScale`**: Un valor `float` que controla la escala uniforme de los objetos de roca (`rockPrefab`) instanciados.
*   **`rockPrefab`**: Una referencia a un `GameObject` que se utilizará como plantilla (prefab) para cada una de las rocas que formarán el estadio. Este prefab debe contener el componente `RockBehavior`.

#### Métodos Principales:

*   **`void Start()`**:
    Este método del ciclo de vida de Unity se llama una vez al inicio del juego, justo antes del primer `Update`. Su función principal es inicializar el estadio. Calcula las posiciones angulares para cada roca basándose en `many` y `radius`, e itera para instanciar el `rockPrefab` la cantidad de veces especificada.

    Dentro de este método, se asigna una `Inscription` a cada roca instanciada basándose en el valor de la variable `config`. También se establece el `parent` de cada roca al `transform` de este objeto `PlayZone` y se le pasan propiedades relevantes al componente `RockBehavior` de la roca recién creada, como la referencia al `PlayZone` (`father`), el `angle` de su posición, su `direction` desde el centro, la `inscription` asignada y su `numbchild` (índice de creación).

    ```csharp
    void Start()
    {
        // ... cálculos de ángulo ...

        for (int i = 0; i < many; i++)
        {
            // ... lógica para determinar 'inscripcion' basada en 'config' ...

            GameObject stone = Instantiate(rockPrefab);
            stone.transform.parent = transform;
            stone.GetComponent<RockBehavior>().father = this;
            stone.GetComponent<RockBehavior>().angle = -angle * i;
            stone.GetComponent<RockBehavior>().direction = dir;
            stone.GetComponent<RockBehavior>().inscription = inscripcion;
            stone.GetComponent<RockBehavior>().numbchild = i;
        }
    }
    ```

*   **`void Update()`**:
    Este método del ciclo de vida de Unity se llama una vez por cada frame. En el contexto de este script, está vacío, lo que indica que no hay lógica de comportamiento en tiempo real o de actualización continua gestionada directamente por `PlayZone` después de la inicialización del estadio.

#### Lógica Clave:
La lógica principal de `PlayZone` reside en su método `Start`, donde se construye el estadio de batalla. Un bucle `for` itera `many` veces para crear un círculo de rocas. La elección de la `Inscription` para cada roca se maneja mediante una sentencia `switch` que evalúa la variable `config`:

*   Si `config` es `normal`, las inscripciones se asignan en un patrón que alterna entre un conjunto de elementos básicos y un par de elementos especiales (asumiendo que los valores 0-3 del enum `Inscription` son elementos y 4-5 son especiales).
*   Si `config` es `fullall` o `fullone`, todas las rocas reciben una inscripción específica (`Inscription.pick` o `Inscription.duel` respectivamente), creando un estadio uniforme en cuanto a ese tipo de inscripción.

Finalmente, cada roca instanciada obtiene una referencia a este objeto `PlayZone` a través de su componente `RockBehavior`, lo que permite que las rocas interactúen o consulten el `PlayZone` si es necesario en sus propias lógicas.

## 3. Dependencias y Eventos
*   **Componentes Requeridos:** Este script no utiliza explícitamente el atributo `[RequireComponent]`. Sin embargo, depende fundamentalmente de que el `rockPrefab` asignado en el Inspector contenga un componente `RockBehavior`, ya que intenta acceder a él para configurar las rocas.

*   **Eventos (Entrada):** `PlayZone` no se suscribe a ningún evento de Unity (como `onClick` de botones) ni a eventos personalizados (`UnityEvent`, `Action`). Su funcionamiento se basa únicamente en el ciclo de vida de `MonoBehaviour` (`Start`).

*   **Eventos (Salida):** Este script no invoca ningún evento (`UnityEvent` o `Action`) para notificar a otros sistemas. Su interacción con el resto del juego se realiza instanciando `GameObjects` y asignando propiedades a sus componentes `RockBehavior` existentes, permitiendo que `RockBehavior` gestione su propia lógica o eventos posteriormente.
```markdown
# `PlayZone.cs`

## 1. Propósito General
Este script `PlayZone` es el responsable de la generación procedural y configuración inicial del "estadio" o "zona de batalla" en el juego Beast Card Clash. Su función principal es crear un diseño circular de objetos "roca" (`rockPrefab`), asignándoles propiedades específicas que probablemente influyan en la jugabilidad, como el tipo de "inscripción".

## 2. Componentes Clave

### `enum SetupConfig`
- **Descripción:** Este `enum` define las diferentes configuraciones o "patrones" que se pueden aplicar al generar las inscripciones de las rocas en la zona de juego. Cada valor representa una estrategia distinta para asignar tipos de inscripción a los elementos del estadio.
- **Valores:**
    - `normal`: Un patrón mixto donde las inscripciones se asignan alternando entre un conjunto de elementos y un conjunto de elementos "no únicos".
    - `fullall`: Todas las rocas recibirán la inscripción `Inscription.pick`.
    - `fullone`: Todas las rocas recibirán la inscripción `Inscription.duel`.

### `PlayZone` (Clase)
- **Descripción:** La clase `PlayZone` hereda de `MonoBehaviour` y es el componente central para la configuración del escenario. Se encarga de instanciar y posicionar las rocas que forman el perímetro de la zona de juego, así como de inicializar sus comportamientos y propiedades. La directiva `[DefaultExecutionOrder(-3)]` asegura que este script se ejecute muy temprano en el ciclo de vida de Unity, antes que la mayoría de otros scripts.
- **Variables Públicas / Serializadas:**
    - `radius` (float): Determina el radio del círculo en el que se distribuirán las rocas. Visible en el Inspector bajo la categoría "StadiumSetup".
    - `many` (int): Define el número total de rocas que se generarán alrededor del círculo. Visible en el Inspector bajo la categoría "StadiumSetup".
    - `config` (SetupConfig): Un selector que permite elegir el patrón de asignación de inscripciones para las rocas, utilizando los valores definidos en el `enum SetupConfig`. Visible en el Inspector bajo la categoría "StadiumSetup".
    - `RockScale` (float): Un factor de escala que, aunque declarado, no se utiliza directamente en el método `Start` para modificar el tamaño de las rocas instanciadas en el código actual. Visible en el Inspector bajo la categoría "StadiumRock".
    - `rockPrefab` (GameObject): La referencia al prefab de GameObject que representa una "roca". Este objeto será instanciado repetidamente para construir el estadio. Visible en el Inspector bajo la categoría "StadiumRock".

- **Métodos Principales:**
    - `void Start()`:
        - **Descripción:** Este es un método del ciclo de vida de Unity, invocado una vez al inicio del juego, antes de la primera actualización de `frame`. Su propósito es generar la zona de batalla.
        - Calcula el ángulo de separación entre cada roca.
        - Itera `many` veces, una por cada roca a crear. En cada iteración:
            - Calcula las coordenadas `x` y `z` para posicionar la roca en un círculo alrededor del centro del objeto `PlayZone`.
            - Determina el tipo de `Inscription` para la roca basándose en la variable `config` (que puede ser `normal`, `fullall` o `fullone`), utilizando un algoritmo interno para distribuir las inscripciones. La `Inscription` es un `enum` que se asume está definido en otro archivo y representa el tipo elemental o de efecto asociado a la roca.
            - Instancia el `rockPrefab`.
            - Establece el objeto `PlayZone` como padre transformacional de la nueva roca para mantener una jerarquía limpia en la escena.
            - Obtiene el componente `RockBehavior` de la roca instanciada y le asigna varias propiedades: `father` (una referencia a este `PlayZone`), `angle` (el ángulo de rotación de la roca), `direction` (el vector desde el centro hasta la roca), `inscription` (el tipo de inscripción calculado) y `numbchild` (el índice de la roca en la secuencia de creación).
        - **Parámetros:** Ninguno.
        - **Retorno:** `void`.

    - `void Update()`:
        - **Descripción:** Otro método del ciclo de vida de Unity, llamado una vez por `frame`. En el código proporcionado, este método está vacío, lo que significa que `PlayZone` no tiene lógica de actualización continua implementada.
        - **Parámetros:** Ninguno.
        - **Retorno:** `void`.

- **Lógica Clave:**
    La lógica principal reside en el bucle `for` dentro del método `Start`. Este bucle es el encargado de la generación procedural de las rocas. Utiliza funciones trigonométricas (`Mathf.Cos`, `Mathf.Sin`) para distribuir las rocas uniformemente en un círculo. La asignación de la `inscription` a cada roca es condicional y se basa en el valor de `config`. Por ejemplo, cuando `config` es `normal`, las inscripciones se ciclan a través de 4 "elementos" y se insertan elementos "no únicos" en intervalos específicos, creando un patrón variado. Finalmente, se configura el comportamiento de cada roca a través de su componente `RockBehavior`, pasándole los datos calculados.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - El script `PlayZone` no utiliza el atributo `[RequireComponent]`. Sin embargo, tiene una dependencia implícita muy fuerte: el `rockPrefab` debe contener un componente `RockBehavior`, ya que el script intenta acceder y configurar este componente en cada roca instanciada (`stone.GetComponent<RockBehavior>()`). Si el `rockPrefab` no tiene este componente, el juego podría lanzar una excepción `NullReferenceException` en tiempo de ejecución.
    - Se asume la existencia de un `enum` llamado `Inscription` en algún otro archivo del proyecto, ya que `PlayZone` hace referencia a él (`Inscription.empty`, `Inscription.pick`, `Inscription.duel`, y su uso en el `switch` case).

- **Eventos (Entrada):** Este script no se suscribe explícitamente a ningún evento de Unity (como clics de UI, colisiones, etc.) ni a eventos personalizados. Su funcionalidad principal se ejecuta durante el método `Start`.

- **Eventos (Salida):** Este script no invoca ningún `UnityEvent` ni `Action` para notificar a otros sistemas sobre la generación de la zona de juego o el estado de sus rocas. Su salida es la configuración directa de los componentes `RockBehavior` de los objetos instanciados.
```
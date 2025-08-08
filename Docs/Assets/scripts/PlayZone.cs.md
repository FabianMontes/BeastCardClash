# `PlayZone.cs`

## 1. Propósito General
Este script `PlayZone.cs` es un componente de Unity (`MonoBehaviour`) que gestiona la configuración inicial y la generación dinámica del "estadio" o "zona de batalla" del juego. Su rol principal es instanciar una serie de objetos de tipo "roca" (`rockPrefab`) en una disposición circular y asignarles propiedades específicas, incluyendo un tipo de "inscripción", al inicio de la escena.

## 2. Componentes Clave

### `PlayZone`
- **Descripción:** Esta clase `PlayZone` hereda de `MonoBehaviour` y es responsable de construir el entorno interactivo donde se desarrollarán las batallas. Utiliza un `rockPrefab` para crear múltiples instancias de rocas, posicionándolas en un círculo y configurando sus comportamientos individuales a través del componente `RockBehavior` de cada roca. El `[DefaultExecutionOrder(-3)]` asegura que este script se ejecute muy temprano en el ciclo de vida de la escena, antes que la mayoría de los demás scripts.

- **Variables Públicas / Serializadas:**
    - `radius (float)`: Define el radio del círculo en el que se distribuirán las rocas. Visible y configurable en el Inspector de Unity bajo la sección "StadiumSetup".
    - `many (int)`: Determina la cantidad total de rocas que se generarán en el círculo. Visible y configurable en el Inspector de Unity bajo la sección "StadiumSetup".
    - `config (SetupConfig)`: Un enumerador que controla el patrón de asignación de las "inscripciones" a las rocas. Sus valores (`normal`, `fullall`, `fullone`) alteran cómo se configuran las rocas generadas. Visible y configurable en el Inspector de Unity bajo la sección "StadiumSetup".
    - `RockScale (float)`: Define la escala de cada roca instanciada. Aunque está serializado y es público, en el código actual proporcionado no se utiliza para aplicar la escala al `rockPrefab` instanciado. Visible y configurable en el Inspector de Unity bajo la sección "StadiumRock".
    - `rockPrefab (GameObject)`: Una referencia al prefab del objeto "roca" que será instanciado repetidamente para construir el estadio. Se espera que este prefab contenga el componente `RockBehavior`. Visible y configurable en el Inspector de Unity bajo la sección "StadiumRock".

- **Métodos Principales:**
    - `void Start()`: Este es un método del ciclo de vida de Unity que se ejecuta una vez antes del primer `Update` cuando el script se habilita. Contiene toda la lógica para la creación del estadio:
        1.  Calcula el ángulo de separación entre cada roca para distribuirlas equitativamente en el círculo.
        2.  Itera `many` veces, una por cada roca a generar.
        3.  En cada iteración, calcula la posición (X y Z) de la roca en el círculo usando `Mathf.Cos` y `Mathf.Sin` junto con el `radius`.
        4.  Determina el tipo de `Inscription` (un `enum` externo a este script) que se asignará a la roca, basándose en el valor de la variable `config` (ver "Lógica Clave" para detalles).
        5.  Instancia el `rockPrefab`.
        6.  Configura el objeto `stone` instanciado: lo hace hijo del objeto que contiene este script (`PlayZone`), y obtiene su componente `RockBehavior` para asignarle propiedades como `father` (una referencia a este `PlayZone`), `angle`, `direction`, la `inscription` calculada y `numbchild` (el índice de la roca).

- **Lógica Clave:**
    - **Generación Circular del Estadio:** El método `Start` utiliza un bucle `for` y funciones trigonométricas para posicionar `many` instancias de `rockPrefab` a lo largo de la circunferencia definida por `radius`.
    - **Configuración Dinámica de `Inscription`:** La lógica dentro del bucle `Start` decide qué `Inscription` (tipo elemental o de acción) se asignará a cada roca, basándose en la variable `config`:
        ```csharp
        switch (config)
        {
            case SetupConfig.normal:
                // Asigna inscripciones cíclicas y algunas "no únicas" a intervalos.
                break;
            case SetupConfig.fullall:
                inscripcion = Inscription.pick; // Todas las rocas son de tipo 'pick'.
                break;
            case SetupConfig.fullone:
                inscripcion = Inscription.duel; // Todas las rocas son de tipo 'duel'.
                break;
        }
        ```
        Cuando `config` es `normal`, se usa una lógica más compleja que alterna entre `Inscription.empty` (con un offset de 4) para ciertos elementos y `Inscription` 0-3 cíclicamente para la mayoría, creando un patrón variado en el estadio.

### `SetupConfig`
- **Descripción:** Este `enum` define las diferentes modalidades predefinidas para la generación de las "inscripciones" en las rocas del estadio.
    - `normal`: Establece un patrón variado de inscripciones cíclicas, con algunos puntos específicos que reciben un tipo de inscripción "no elemental" (`Inscription.empty` + 4).
    - `fullall`: Fuerza que todas las rocas generadas reciban la inscripción `Inscription.pick`.
    - `fullone`: Fuerza que todas las rocas generadas reciban la inscripción `Inscription.duel`.

## 3. Dependencias y Eventos

- **Componentes Requeridos:**
    - No utiliza `[RequireComponent]` explícitamente. Sin embargo, el script asume que el `rockPrefab` asignado en el Inspector **debe contener un componente `RockBehavior`**. Si este componente no está presente en el prefab, se producirá un error de tipo `NullReferenceException` al intentar acceder a `stone.GetComponent<RockBehavior>()`.

- **Eventos (Entrada):**
    - Este script no se suscribe a ningún evento de entrada de usuario ni a eventos de otros sistemas (como `button.onClick.AddListener`). Su lógica principal se ejecuta durante el método `Start`.

- **Eventos (Salida):**
    - Este script no invoca `UnityEvent` ni `Action` para notificar a otros sistemas de sus acciones. La comunicación se realiza de forma directa mediante la configuración de las propiedades del componente `RockBehavior` de cada roca instanciada.
    - **Dependencia Implícita:** Utiliza un `enum` llamado `Inscription` que no está definido dentro de este archivo `PlayZone.cs`. Se espera que `Inscription` esté definido en otro script o en un archivo global accesible por este proyecto.
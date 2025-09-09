# PlayZone
El script `PlayZone` es un componente de Unity `MonoBehaviour` cuyo propósito principal es la inicialización y configuración del área de juego o "estadio" para las batallas de *Beast Card Clash*. Dada su `DefaultExecutionOrder(-3)`, se ejecuta muy temprano en el ciclo de vida de Unity, asegurando que la zona de juego esté configurada antes que la mayoría de otros scripts.

Este script es responsable de:
*   Instanciar una serie de objetos `rockPrefab` en una disposición circular alrededor de un punto central.
*   Configurar cada roca instanciada, asignándole propiedades clave a su componente `RockBehavior`, como su padre, ángulo, dirección, un tipo de inscripción (`Inscription`) y un número de identificación.
*   La forma en que se distribuyen estas inscripciones depende de la configuración elegida (`SetupConfig`), permitiendo diferentes diseños de la zona de juego.

En línea con el enfoque del proyecto en una buena experiencia de desarrollo, `PlayZone` encapsula la lógica de generación del tablero, facilitando la creación de distintas configuraciones de batalla a través de sus variables serializadas.

# Métodos

## Métodos de Unity

### Start
El método `Start` es invocado una única vez al inicio del ciclo de vida del script. Su función primordial es la generación programática de la zona de juego o "estadio".

El proceso de creación sigue los siguientes pasos:
1.  **Cálculo de Posicionamiento:** Se determina el `angle` (ángulo) entre cada roca, dividiendo 360 grados por el número total de rocas (`many`). Este ángulo se convierte a radianes (`anglerad`) para su uso en funciones trigonométricas.
    ```csharp
    float angle = 360 / many;
    float anglerad = angle * Mathf.PI / 180f;
    ```
2.  **Inicialización de Variables para Inscripciones:** Se declaran e inicializan variables como `elem`, `nonelem`, `redelement`, `divelement`, y `nelem`. Estas variables son cruciales para la lógica de asignación de las inscripciones en la configuración `normal`.
3.  **Bucle de Creación de Rocas:** Se itera `many` veces, una por cada roca que se debe instanciar.
    ```csharp
    for (int i = 0; i < many; i++) // the estup creates like a none clock creations order
    {
        // ... lógica de creación ...
    }
    ```
    Dentro de cada iteración:
    *   **Determinación de Posición:** Se calculan las coordenadas `x` y `z` utilizando `Mathf.Cos` y `Mathf.Sin` junto con `anglerad * i` para posicionar la roca en un círculo.
    *   **Asignación de Inscripción:** Se utiliza un bloque `switch` para determinar el valor de la enumeración externa `Inscription` que se asignará a la roca actual, basándose en el valor de `config`:
        *   **`SetupConfig.normal`:** Esta configuración implementa una lógica más elaborada y no secuencial para la asignación de inscripciones. Utiliza las variables `elem`, `nonelem`, `redelement`, `divelement` y `nelem` para distribuir los valores `Inscription.empty`, `Inscription.pick`, `Inscription.duel`, y otros valores numéricos (presumiblemente `Inscription.element0` a `Inscription.element3`). La complejidad de esta lógica, como indica el comentario `// the estup creates like a none clock creations order`, busca una distribución específica de las inscripciones sin seguir un patrón de reloj predecible.
        *   **`SetupConfig.fullall`:** Todas las rocas recibirán la inscripción `Inscription.pick`.
        *   **`SetupConfig.fullone`:** Todas las rocas recibirán la inscripción `Inscription.duel`.
        ```csharp
        Inscription inscripcion = Inscription.empty;
        switch (config)
        {
            case SetupConfig.normal:
                if (i == divelement * nelem)
                {
                    inscripcion = (Inscription)(nonelem + 4);
                    nonelem = (nonelem + 1) % 2;
                    nelem++;
                }
                else
                {
                    inscripcion = (Inscription)(elem);
                    elem = (elem + 1) % 4;
                }
                break;
            case SetupConfig.fullall:
                inscripcion = Inscription.pick;
                break;
            case SetupConfig.fullone:
                inscripcion = Inscription.duel;
                break;
        }
        ```
    *   **Instanciación y Configuración del Prefab:**
        *   Se crea una instancia del `rockPrefab`.
        *   Se establece el `transform` del objeto `PlayZone` como padre de la roca instanciada.
        *   Se obtiene el componente `RockBehavior` de la roca y se le asignan las siguientes propiedades:
            *   `father`: Se referencia a la propia instancia de `PlayZone` (`this`).
            *   `angle`: Se le asigna el negativo del `angle * i` calculado. El comentario `// has to look at the oposite of the creation rotation` indica que la rotación de la roca debe ser opuesta a su ángulo de creación para una orientación correcta.
            *   `direction`: Se establece la dirección `Vector3(x, 0, z)` calculada para la posición de la roca.
            *   `inscription`: La inscripción determinada en el paso anterior.
            *   `numbchild`: El índice `i` del bucle, sirviendo como un identificador único para cada roca.
        ```csharp
        GameObject stone = Instantiate(rockPrefab);
        stone.transform.parent = transform;
        stone.GetComponent<RockBehavior>().father = this;
        stone.GetComponent<RockBehavior>().angle = -angle * i; // has to look at the oposite of the creation rotation
        stone.GetComponent<RockBehavior>().direction = dir;
        stone.GetComponent<RockBehavior>().inscription = inscripcion;
        stone.GetComponent<RockBehavior>().numbchild = i;
        ```

### Update
El método `Update` se ejecuta una vez por cada frame del juego. En el script `PlayZone`, este método se encuentra vacío. Esto indica que la funcionalidad de `PlayZone` es puramente de configuración inicial y no requiere de ninguna lógica continua o actualización en cada frame después de la inicialización de la zona de juego.

## Otros métodos
El script `PlayZone` no contiene métodos personalizados adicionales más allá de los métodos de ciclo de vida de Unity (`Start`, `Update`).

## Getters y Setters

1.  `radius: float`: Establece la distancia desde el centro del `PlayZone` a la que se instanciarán las rocas.
2.  `many: int`: Establece la cantidad total de rocas que se instanciarán para formar el `PlayZone`.
3.  `RockScale: float`: Define la escala deseada para las rocas. Aunque está declarado públicamente y se puede configurar en el Inspector de Unity, este script `PlayZone` *no utiliza directamente* este valor para aplicar la escala a los `rockPrefab` instanciados. Su uso está presumiblemente delegado a otro componente (como `RockBehavior`) o se trata de una propiedad futura.
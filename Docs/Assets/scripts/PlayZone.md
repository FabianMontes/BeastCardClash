# `PlayZone`
El script `PlayZone` es un `MonoBehaviour` fundamental encargado de la generación y configuración inicial de la zona de juego o "arena" en el proyecto Beast Card Clash. Su propósito principal es instanciar una serie de objetos representados como "rocas" (`rockPrefab`) y organizarlos en una formación circular alrededor de un punto central. Cada una de estas "rocas" es configurada con una `Inscription` específica, que probablemente representa un tipo elemental, una facultad de la UNAL o un rol especial, basada en un esquema de configuración predefinido.

Este script garantiza que el campo de batalla esté listo con sus elementos interactivos posicionados y con sus propiedades iniciales asignadas antes de que comience la lógica principal del juego. La anotación `[DefaultExecutionOrder(-3)]` indica que este script se ejecuta muy temprano en el ciclo de vida de la escena, asegurando que la zona de juego se configure antes que la mayoría de los demás componentes que puedan depender de su existencia.

Dentro del mismo archivo, se define el siguiente `enum`:
```csharp
enum SetupConfig
{
    normal, fullall, fullone
}
```
Este `enum` es utilizado para determinar la estrategia de asignación de `Inscription` a las "rocas" generadas. Cada valor (`normal`, `fullall`, `fullone`) corresponde a un método diferente de distribución de los tipos de `Inscription` en el tablero.

# Métodos

## Métodos de Unity

### `Start`
Este método se invoca una vez al inicio del ciclo de vida del script, antes de la primera actualización de `frame`. Su función principal es inicializar la zona de juego, instanciando y posicionando los elementos interactivos ("rocas").

1.  **Cálculo de Posiciones Circulares**: El método comienza calculando el ángulo de separación entre cada "roca" para distribuirlas uniformemente en un círculo, utilizando la cantidad definida por la variable `many`.
    ```csharp
    float angle = 360 / many;
    float anglerad = angle * Mathf.PI / 180f;
    ```
    Luego, itera `many` veces para generar cada "roca". En cada iteración, calcula las coordenadas `x` y `z` para colocar el objeto en el perímetro del círculo, utilizando funciones trigonométricas (`Mathf.Cos` y `Mathf.Sin`).
    ```csharp
    for (int i = 0; i < many; i++) // the estup creates like a none clock creations order
    {
        float x = Mathf.Cos(anglerad * i);
        float z = Mathf.Sin(anglerad * i);
        // ... logic for inscription and instantiation ...
    }
    ```

2.  **Asignación de `Inscription`**: Dentro del bucle de creación, se utiliza una estructura `switch` basada en la variable `config` (de tipo `SetupConfig`) para determinar qué `Inscription` se asignará a la "roca" actual. La `Inscription` es un `enum` que probablemente define el tipo elemental o rol de cada posición en el tablero (e.g., fuego, agua, tierra, aire, o roles especiales como "pick", "duel", "empty").
    *   **`SetupConfig.normal`**: Implementa una lógica más compleja para alternar entre diferentes `Inscription`s. Parece asignar cíclicamente cuatro tipos base (representados por `elem = (elem + 1) % 4`) y, en intervalos específicos (`divelement * nelem`), asigna otros dos tipos "no únicos" (representados por `nonelem + 4`).
    *   **`SetupConfig.fullall`**: Asigna la `Inscription.pick` a todas las "rocas" generadas.
    *   **`SetupConfig.fullone`**: Asigna la `Inscription.duel` a todas las "rocas" generadas.

    ```csharp
    switch (config)
    {
        case SetupConfig.normal:
            // ... (Logic para asignación alternada de Inscripciones) ...
            break;
        case SetupConfig.fullall:
            inscripcion = Inscription.pick;
            break;
        case SetupConfig.fullone:
            inscripcion = Inscription.duel; // set who you want in all maps
            break;
    }
    ```

3.  **Instanciación y Configuración del `rockPrefab`**: Una vez determinada la `Inscription`, se instancia el `rockPrefab`. Este nuevo objeto se establece como hijo del GameObject al que está adjunto `PlayZone` y se le accede a su componente `RockBehavior` para configurarlo.
    ```csharp
    GameObject stone = Instantiate(rockPrefab);
    stone.transform.parent = transform;
    stone.GetComponent<RockBehavior>().father = this;
    stone.GetComponent<RockBehavior>().angle = -angle * i; // has to look at the oposite of the creation rotation
    stone.GetComponent<RockBehavior>().direction = dir;
    stone.GetComponent<RockBehavior>().inscription = inscripcion;
    stone.GetComponent<RockBehavior>().numbchild = i;
    ```
    Se le pasa una referencia a sí mismo (`this`) como `father`, el ángulo de rotación opuesto al de su creación (para que miren hacia el centro), la dirección vectorial desde el centro, la `inscription` calculada y un número de identificación (`numbchild`). Esto establece la relación entre `PlayZone` y cada `RockBehavior` individual, permitiendo que las "rocas" conozcan su origen y configuración inicial.

### `Update`
Este método se invoca una vez por `frame`.
```csharp
void Update()
{

}
```
Actualmente, el método `Update` está vacío, lo que indica que el script `PlayZone` no realiza ninguna lógica o actualización continua después de su configuración inicial en `Start()`. Si en el futuro se requirieran interacciones o actualizaciones en tiempo real para la zona de juego, esta sería la ubicación para implementarlas.

## Getters y Setters

Los siguientes son campos públicos serializados, lo que significa que son accesibles desde otros scripts y configurables directamente desde el Inspector de Unity:

1.  `radius`: `public float`: Define el radio del círculo en el que se distribuirán los objetos `rockPrefab`. Un valor mayor resultará en una zona de juego más amplia.
2.  `many`: `public int`: Especifica la cantidad total de objetos `rockPrefab` que se generarán en la zona de juego. Este valor influye directamente en la densidad de los elementos y en el cálculo del ángulo de distribución.
3.  `config`: `public SetupConfig`: Controla el método de asignación de `Inscription` a cada `rockPrefab` instanciado, eligiendo entre las opciones definidas en el `enum SetupConfig` (`normal`, `fullall`, `fullone`).
4.  `RockScale`: `public float`: Permite ajustar la escala de los objetos `rockPrefab` instanciados.
5.  `rockPrefab`: `public GameObject`: La referencia al prefab del objeto "roca" que se utilizará para crear cada elemento interactivo en la zona de juego. Este prefab debe contener el componente `RockBehavior`.
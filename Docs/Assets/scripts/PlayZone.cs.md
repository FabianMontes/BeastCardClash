# `PlayZone.cs`

## 1. Propósito General
El script `PlayZone.cs` es el responsable principal de la generación dinámica del "estadio" o área de batalla en el juego. Se encarga de instanciar y posicionar de forma circular objetos (`rockPrefab`) que probablemente representan las "casillas" o "piedras" interactivas del tablero, y de inicializar sus propiedades clave, como el tipo de "inscripción" que contienen.

## 2. Componentes Clave

### `PlayZone`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, gestiona la creación de la zona de juego al inicio de la escena. Su rol principal es configurar un diseño circular de elementos interconectados (las "rocas") y asignarles un tipo de inscripción según una configuración predefinida. Se ejecuta muy temprano en el ciclo de vida de Unity (`[DefaultExecutionOrder(-3)]`) para asegurar que la zona de juego esté preparada antes que otros scripts dependan de ella.
- **Variables Públicas / Serializadas:**
    - `radius` (float): Define el radio del círculo donde se colocarán los objetos. Es configurable desde el Inspector de Unity.
    - `many` (int): Determina el número total de objetos (`rockPrefab`) que se instanciarán y se distribuirán alrededor del círculo.
    - `config` (SetupConfig): Un enumerador que controla el patrón de asignación de las "inscripciones" a las rocas. Permite configurar diferentes distribuciones para la zona de juego.
    - `RockScale` (float): Escala que se aplica a los objetos instanciados, permitiendo ajustar su tamaño visual.
    - `rockPrefab` (GameObject): El prefab que se instanciará repetidamente para construir la zona de juego. Este prefab debe contener un componente `RockBehavior`.
- **Métodos Principales:**
    - `void Start()`: Este es un método del ciclo de vida de Unity que se llama una vez al inicio del script. Contiene la lógica principal para la creación de la zona de batalla:
        - Calcula el ángulo para la distribución equitativa de los objetos en un círculo.
        - Itera `many` veces para instanciar cada `rockPrefab`.
        - Dentro del bucle, calcula la posición `(x, z)` para cada roca utilizando funciones trigonométricas (coseno y seno) para crear la forma circular.
        - Determina el tipo de `Inscription` para la roca actual basándose en la variable `config`:
            - Si `config` es `normal`, distribuye un patrón de inscripciones "normales" y "especiales" (identificadas por `nonelem + 4`).
            - Si `config` es `fullall`, asigna `Inscription.pick` a todas las rocas.
            - Si `config` es `fullone`, asigna `Inscription.duel` a todas las rocas.
        - Instancia el `rockPrefab`, lo establece como hijo del GameObject al que está adjunto `PlayZone`, y luego recupera el componente `RockBehavior` del objeto instanciado.
        - Asigna varias propiedades al `RockBehavior` de cada roca, incluyendo una referencia a este `PlayZone` (`father`), el ángulo y dirección de su posición, el tipo de `inscription` y su número de orden (`numbchild`).
    - `void Update()`: Este es un método del ciclo de vida de Unity que se llama una vez por cada frame. En el código actual, está vacío y no realiza ninguna acción.

### `SetupConfig` (Enumerador)
- **Descripción:** Este `enum` define los diferentes modos en que las "inscripciones" (tipos elementales o de habilidad) se asignarán a las "rocas" cuando se genere la zona de juego.
    - `normal`: Implementa una distribución balanceada de diferentes tipos de inscripciones, incluyendo algunas "especiales".
    - `fullall`: Todas las rocas recibirán la inscripción `pick`.
    - `fullone`: Todas las rocas recibirán la inscripción `duel`.

- **Lógica Clave:**
La lógica central de `PlayZone` reside en el método `Start()`. Utiliza un bucle para instanciar múltiples `rockPrefab` en una disposición circular. La complejidad radica en la sub-lógica de asignación de `Inscription`s a cada roca, la cual se adapta dinámicamente según el valor de `config`. Por ejemplo, en el modo `normal`, el script alterna entre diferentes tipos de inscripciones basándose en contadores internos (`elem`, `nonelem`, `nelem`) para crear un patrón variado.

```csharp
// Fragmento de la lógica de asignación de Inscription en Start()
switch (config)
{
    case SetupConfig.normal:
        if (i == divelement * nelem)
        {
            inscripcion = (Inscription)(nonelem + 4);
            // ... lógica para nonelem y nelem ...
        }
        else
        {
            inscripcion = (Inscription)(elem);
            // ... lógica para elem ...
        }
        break;
    // ... otros casos para fullall, fullone ...
}
```
Una vez que se determina la inscripción, el script se comunica directamente con el componente `RockBehavior` de cada roca instanciada para inicializar sus propiedades:
```csharp
// Fragmento de inicialización de RockBehavior en Start()
GameObject stone = Instantiate(rockPrefab);
stone.transform.parent = transform;
stone.GetComponent<RockBehavior>().father = this; // Referencia a PlayZone
stone.GetComponent<RockBehavior>().angle = -angle * i;
stone.GetComponent<RockBehavior>().direction = dir;
stone.GetComponent<RockBehavior>().inscription = inscripcion; // La inscripción determinada
stone.GetComponent<RockBehavior>().numbchild = i;
```
Esta comunicación directa establece la relación padre-hijo (lógica, no solo jerarquía de transformaciones) y configura las propiedades iniciales de cada roca para su posterior comportamiento en el juego.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, tiene una dependencia implícita muy fuerte de que el `rockPrefab` configurado en el Inspector contenga un componente llamado `RockBehavior`. Sin este componente, el script fallará al intentar acceder a `stone.GetComponent<RockBehavior>()`.
    - El enumerador `Inscription` no está definido en este archivo `PlayZone.cs`. Se asume que está definido en otro lugar del proyecto (probablemente en `RockBehavior.cs` o un script de utilidad global) y es accesible desde aquí.
- **Eventos (Entrada):** Este script no se suscribe a ningún evento de Unity (`UnityEvent`, `Action`) o eventos de interfaz de usuario directamente en el código proporcionado. Su lógica principal se activa durante el método `Start()` del ciclo de vida de Unity.
- **Eventos (Salida):** Este script no invoca explícitamente ningún `UnityEvent` o `Action` para notificar a otros sistemas. Su interacción con otras partes del juego es principalmente a través de la inicialización directa de las propiedades de los componentes `RockBehavior` de las rocas que crea.
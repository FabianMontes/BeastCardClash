# Roundanimation
Este script `Roundanimation` es responsable de gestionar y controlar la animación visual que se muestra al inicio de cada nueva ronda en el juego. Su función principal es hacer que un elemento de la interfaz de usuario (UI), que probablemente contiene el texto de la ronda, aparezca de forma animada, permanezca visible durante un tiempo y luego desaparezca, indicando así el progreso del juego entre rondas.

El script funciona escalando horizontalmente un objeto de UI desde una escala de cero (invisible) hasta su tamaño completo (visible), manteniéndolo en pantalla, y luego volviendo a escalarlo a cero. Durante este proceso, también actualiza el texto para mostrar el número de la ronda actual y coordina con el sistema de juicio de combate para notificar cuándo la animación ha terminado.

El objetivo de esta animación es proporcionar una señal visual clara a los jugadores sobre el inicio de una nueva fase de combate, mejorando la experiencia de usuario y la comprensión del flujo del juego.

# Métodos

## Métodos de Unity

### Awake
El método `Awake` no está implementado directamente en este script.

### Start
Este método se invoca una vez al inicio del ciclo de vida del script, antes de la primera actualización de `Update`. Se utiliza para inicializar el estado del objeto de UI al que está adjunto.

```csharp
void Start()
{
    round = false;
    showing = false;
    Vector3 vector3 = transform.localScale;
    vector3.x = 0;
    transform.localScale = vector3;
    text = GetComponentInChildren<TextMeshProUGUI>();
}
```

-   `round = false;`: Inicializa la propiedad estática `round` a `false`, indicando que ninguna animación de ronda está activa al principio.
-   `showing = false;`: Inicializa la variable `showing` a `false`, lo que significa que la animación no se está ejecutando inicialmente.
-   `Vector3 vector3 = transform.localScale; vector3.x = 0; transform.localScale = vector3;`: Establece la escala horizontal (`x`) del objeto a `0`. Esto hace que el elemento de la UI sea invisible al inicio, preparándolo para la animación de "aparición".
-   `text = GetComponentInChildren<TextMeshProUGUI>();`: Obtiene una referencia al componente `TextMeshProUGUI` que se encuentra como hijo del objeto actual. Este componente será utilizado para mostrar el texto de la ronda.

### Update
El método `Update` se llama una vez por cada frame y contiene la lógica principal para la animación de la ronda. Se encarga de gestionar el escalado del objeto, el tiempo de permanencia en pantalla y la transición entre las diferentes fases de la animación (escalado hacia arriba, mantenimiento y escalado hacia abajo).

La lógica en `Update` se divide en tres bloques condicionales que representan los estados de la animación:

1.  **Escalado hacia arriba (Aparición):**
    ```csharp
    if(showing && estado == false)
    {
        if (transform.localScale.x >= 1)
        {
            // ... (código para fijar escala y cambiar estado)
        }
        else
        {
            Vector3 vector3 = transform.localScale;
            vector3.x =  vector3.x + Time.deltaTime /  movescaletime;
            transform.localScale = vector3;
        }
    }
    ```
    Este bloque se ejecuta cuando la animación ha sido iniciada (`showing` es `true`) y el objeto aún no ha alcanzado su escala máxima (`estado` es `false`).
    -   Si la escala horizontal (`transform.localScale.x`) es mayor o igual a `1`, significa que el objeto ha terminado de aparecer. Se fija la escala a `1` para evitar sobrepasarla, se establece `estado` a `true` (indicando que ahora está en la fase de "mantenimiento" o visibilidad completa) y se registra `Time.time` en `timetytime` para iniciar el conteo del tiempo de retraso.
    -   Si no ha alcanzado la escala máxima, el objeto se escala incrementalmente hacia arriba sumando `Time.deltaTime` dividido por `movescaletime` a su escala horizontal. Esto crea un efecto de aparición suave.

2.  **Mantenimiento en pantalla (Retraso):**
    ```csharp
    if(showing && estado == true)
    {
        if(Time.time - timetytime >= timedelay)
        {
            showing = false;
        }
    }
    ```
    Este bloque se activa cuando el objeto ha aparecido completamente (`showing` es `true` y `estado` es `true`).
    -   Comprueba si el tiempo transcurrido desde que se registró `timetytime` (es decir, desde que el objeto alcanzó su tamaño completo) es mayor o igual al `timedelay` especificado.
    -   Si el tiempo de retraso ha pasado, se establece `showing` a `false`, lo que desencadena la siguiente fase de la animación (escalado hacia abajo).

3.  **Escalado hacia abajo (Desaparición):**
    ```csharp
    if(showing == false && estado == true)
    {
        if (transform.localScale.x <= 0)
        {
            // ... (código para fijar escala y cambiar estado)
        }
        else
        {
            Vector3 vector3 = transform.localScale;
            vector3.x = vector3.x - Time.deltaTime / movescaletime;
            transform.localScale = vector3;
            round = false;
            transform.parent.GetComponent<Image>().enabled = false;
            CombatJudge.CombatJudgeInstance.EndRounded();
        }
    }
    ```
    Este bloque se ejecuta cuando el objeto ha terminado de aparecer y el tiempo de retraso ha expirado (`showing` es `false` y `estado` es `true`).
    -   Si la escala horizontal (`transform.localScale.x`) es menor o igual a `0`, significa que el objeto ha terminado de desaparecer. Se fija la escala a `0`, se establece `estado` a `false` (indicando que la animación ha concluido) y se registra `Time.time` en `timetytime`.
    -   Si no ha alcanzado la escala mínima, el objeto se escala incrementalmente hacia abajo restando `Time.deltaTime` dividido por `movescaletime` a su escala horizontal, creando un efecto de desaparición suave.
    -   Durante este proceso de desaparición, también se realizan acciones importantes:
        -   `round = false;`: Se actualiza la propiedad estática `round` a `false`, señalando que la animación de ronda ha finalizado.
        -   `transform.parent.GetComponent<Image>().enabled = false;`: Deshabilita el componente `Image` del objeto padre. Esto sugiere que el padre es un panel o fondo visual que debe ocultarse una vez que la animación de texto de la ronda ha terminado.
        -   `CombatJudge.CombatJudgeInstance.EndRounded();`: Llama a un método en la instancia estática de `CombatJudge`. Esto notifica al sistema de juicio de combate que la animación de fin de ronda ha concluido, permitiéndole continuar con la lógica del juego.

## Otros métodos

### startRound() : void
Este método público es el punto de entrada para iniciar la animación de una nueva ronda.

```csharp
public void startRound()
{
    if(round == true)
    {
        return;
    }
    timetytime = Time.time;
    round = true;
    showing = true ;
    transform.parent.GetComponent<Image>().enabled = true;
    text.text = $"Ronda {CombatJudge.CombatJudgeInstance.Round}";
}
```

-   `if(round == true) { return; }`: Una comprobación para evitar iniciar la animación si ya está en curso, garantizando que no se superpongan múltiples animaciones de ronda.
-   `timetytime = Time.time;`: Actualiza `timetytime` con el tiempo actual para iniciar el conteo de la animación desde cero.
-   `round = true;`: Establece la propiedad estática `round` a `true`, indicando que una animación de ronda está activa.
-   `showing = true;`: Pone en marcha la lógica de animación dentro del método `Update`.
-   `transform.parent.GetComponent<Image>().enabled = true;`: Habilita el componente `Image` del objeto padre, presumiblemente para mostrar un fondo o panel junto con el texto de la ronda.
-   `text.text = $"Ronda {CombatJudge.CombatJudgeInstance.Round}";`: Actualiza el texto en el `TextMeshProUGUI` para mostrar la ronda actual. El número de ronda se obtiene de la propiedad `Round` de la instancia estática de `CombatJudge`.

## Getters y Setters

1.  `public static bool round { get; private set; }`: Este es un _property_ estático de tipo booleano. `get` permite que cualquier script del proyecto lea su valor, mientras que `private set` restringe la modificación de este valor al propio script `Roundanimation`. Indica si la animación de una ronda está actualmente activa (`true`) o no (`false`).
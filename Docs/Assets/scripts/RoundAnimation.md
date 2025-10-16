# `RoundAnimation`
Este script de Unity, adjunto a un `GameObject` que forma parte de la interfaz de usuario, es el encargado de gestionar y animar la aparición y desaparición de un mensaje indicando el inicio de una nueva ronda en el juego. Su función principal es mostrar el texto "Ronda X" (donde X es el número de la ronda actual), animarlo para que escale desde cero, permanezca visible durante un tiempo y luego escale de vuelta a cero, ocultándose. Además de la animación visual, este script se comunica con el sistema de combate (`CombatJudge`) para señalar el final de la fase de animación de la ronda, y gestiona un estado global (`RoundAnimation.round`) para indicar si una ronda está actualmente en proceso de ser anunciada.

El funcionamiento del script se basa en el seguimiento del tiempo y la manipulación directa de la escala del `GameObject` al que está adjunto. Utiliza varias variables `bool` para controlar las diferentes fases de la animación (escalado de entrada, espera, escalado de salida) y un `TextMeshProUGUI` para mostrar el texto del número de ronda.

# Métodos

## Métodos de Unity

### `Start()`
Este método se ejecuta una única vez al inicio del ciclo de vida del script. Su propósito es inicializar el estado del componente antes de que comience la lógica principal en `Update`.

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

-   `round = false;`: Asegura que el flag global `round` (que indica si una ronda está activa) se inicialice como `false`.
-   `showing = false;`: Asegura que el flag `showing` (que controla si la animación está en curso) también se inicialice como `false`.
-   `transform.localScale.x = 0;`: Establece la escala inicial en el eje X del `GameObject` a 0, haciendo que el elemento esté oculto desde el principio.
-   `text = GetComponentInChildren<TextMeshProUGUI>();`: Obtiene una referencia al componente `TextMeshProUGUI` que se encuentra como hijo del `GameObject` actual. Este componente se usará para mostrar el número de la ronda.

### `Update()`
Este método se llama una vez por cada frame del juego y contiene la lógica principal para la animación de la ronda. Implementa una máquina de estados sencilla para controlar el flujo de la animación (escalado de entrada, espera, escalado de salida).

```csharp
void Update()
{
    // Fase 1: Escalar hacia arriba (aparición)
    if(showing && estado == false) { /* ... */ }
    // Fase 2: Espera después de escalar hacia arriba
    if(showing && estado == true) { /* ... */ }
    // Fase 3: Escalar hacia abajo (desaparición)
    if(showing == false && estado == true) { /* ... */ }
}
```

1.  **Fase de Escalado de Entrada (`if(showing && estado == false)`)**:
    -   Esta fase se activa si la animación ha sido iniciada (`showing` es `true`) y aún no ha terminado de escalar hacia arriba (`estado` es `false`).
    -   Incrementa gradualmente la escala en el eje X (`transform.localScale.x`) usando `Time.deltaTime` y `movescaletime` para asegurar un movimiento suave e independiente de la tasa de frames.
    -   Una vez que la escala en X alcanza o supera 1, se fija en 1 (escala completa), `estado` se establece en `true` (indicando que el escalado de entrada ha terminado), y se registra el tiempo actual en `timetytime` para comenzar la cuenta de la fase de espera.

2.  **Fase de Espera (`if(showing && estado == true)`)**:
    -   Esta fase se activa si la animación está en curso (`showing` es `true`) y ya ha completado su escalado de entrada (`estado` es `true`).
    -   Verifica si el tiempo transcurrido desde que finalizó el escalado de entrada (`Time.time - timetytime`) es mayor o igual que el `timedelay` configurado.
    -   Si el `timedelay` ha transcurrido, `showing` se establece en `false`, lo que transiciona la animación a la fase de escalado de salida.

3.  **Fase de Escalado de Salida (`if(showing == false && estado == true)`)**:
    -   Esta fase se activa si la animación ha terminado su fase de espera (`showing` es `false`) y estaba visible (`estado` es `true`).
    -   Decrementa gradualmente la escala en el eje X (`transform.localScale.x`) de manera similar a la fase de entrada.
    -   Una vez que la escala en X alcanza o es menor que 0, se fija en 0 (totalmente oculto), `estado` se establece en `false` (la animación ha terminado por completo), y se registra el tiempo actual en `timetytime`.
    -   **Acciones clave al finalizar la animación:**
        -   `round = false;`: El flag global se desactiva, indicando que la animación de la ronda ha terminado.
        -   `transform.parent.GetComponent<Image>().enabled = false;`: Deshabilita el componente `Image` del `GameObject` padre. Esto sugiere que el padre es un panel de UI (como un fondo) que acompaña al texto de la ronda y debe ocultarse junto con la animación.
        -   `CombatJudge.Instance.EndRounded();`: Llama a un método en la instancia Singleton de `CombatJudge`. Esto es crucial para informar al sistema de combate que la animación de inicio de ronda ha finalizado y que la lógica del juego puede proceder.

## Otros métodos

### `void startRound()`
Este método público es el punto de entrada para iniciar la animación de una nueva ronda. Debe ser llamado por otros scripts (por ejemplo, el `CombatJudge`) cuando se desee anunciar el comienzo de una ronda.

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
    text.text = $"Ronda {CombatJudge.Instance.Round}";
}
```

-   `if(round == true) { return; }`: Una salvaguardia para evitar que la animación se inicie si ya hay una ronda en curso o anunciándose, garantizando que no se superpongan animaciones.
-   `timetytime = Time.time;`: Almacena el tiempo actual, lo que es útil para la fase inicial de la animación.
-   `round = true;`: Establece el flag global `round` a `true`, indicando que la animación de la ronda ha comenzado.
-   `showing = true;`: Activa la lógica de animación en el método `Update`, iniciando el escalado de entrada.
-   `transform.parent.GetComponent<Image>().enabled = true;`: Habilita el componente `Image` del `GameObject` padre, haciendo visible el fondo o panel de la animación.
-   `text.text = $"Ronda {CombatJudge.Instance.Round}";`: Actualiza el texto mostrado en el `TextMeshProUGUI` para reflejar el número de la ronda actual, obteniéndolo de la instancia de `CombatJudge`.

## Getters y Setters

1.  `static bool round`: `get`: Permite a otros scripts leer el estado actual de si una ronda está siendo anunciada o activa. `set` es `private`, lo que significa que solo el propio script `RoundAnimation` puede modificar este valor.
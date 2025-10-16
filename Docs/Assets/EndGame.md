# `EndGame`
El script `EndGame` es un componente de Unity responsable de gestionar la interfaz de usuario y la lógica de transición al finalizar una ronda de juego, mostrando un mensaje de "Victoria" o "Derrota". Su función principal es animar la aparición y desaparición de un elemento de UI (presumiblemente un panel que contiene el mensaje final), establecer el texto del resultado y, finalmente, notificar al sistema de estados del juego sobre el resultado para que el juego pueda proceder a la siguiente fase. Está diseñado para ofrecer una retroalimentación visual clara al jugador al concluir una partida, con un enfoque en una experiencia de usuario fluida y una implementación sencilla para los desarrolladores.

# Métodos

## Métodos de Unity

### `Start()`
El método `Start()` se invoca una vez al inicio del ciclo de vida del script. Su propósito es inicializar las variables de estado internas y preparar el GameObject para la secuencia de finalización del juego:
- Establece las banderas `round` y `showing` en `false` para asegurar que ninguna secuencia de fin de juego esté activa al inicio.
- Oculta inicialmente el elemento de UI controlando su escala, estableciendo `transform.localScale.x` a `0`, lo que lo hace invisible.
- Obtiene una referencia al componente `TextMeshProUGUI` que se espera que sea un hijo de este GameObject. Este componente se utilizará para mostrar el texto de "Victoria" o "Derrota".

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

### `Update()`
El método `Update()` se ejecuta en cada fotograma y contiene la lógica principal para animar la interfaz de usuario de fin de juego y gestionar las transiciones de estado. Monitorea las banderas `showing` y `estado` para determinar en qué fase de la secuencia se encuentra:

1.  **Escalando para mostrar el mensaje (`showing == true` y `estado == false`):**
    En esta fase, el script aumenta gradualmente la escala en el eje X del GameObject (`transform.localScale.x`). La velocidad de escalado está controlada por la variable `movescaletime`, asegurando una animación suave e independiente de la tasa de fotogramas.
    ```csharp
    vector3.x = vector3.x + Time.deltaTime / movescaletime;
    transform.localScale = vector3;
    ```
    Una vez que `transform.localScale.x` alcanza o supera `1`, el elemento se considera completamente visible. En este punto, `transform.localScale.x` se ajusta a `1`, la bandera `estado` se establece en `true` para indicar que el elemento está completamente mostrado, y `timetytime` se actualiza para marcar el inicio del retardo de visualización.

2.  **Retardo de visualización (`showing == true` y `estado == true`):**
    Después de que el elemento se ha escalado completamente, el script espera una duración especificada por la variable `timedelay`. Este retardo comienza a contar desde el momento en que se completó el escalado (cuando `timetytime` se actualizó por última vez).
    ```csharp
    if (Time.time - timetytime >= timedelay)
    {
        showing = false; // Inicia la fase de escalado hacia afuera
    }
    ```
    Una vez que transcurre el `timedelay`, la bandera `showing` se establece en `false`, lo que indica el inicio de la fase de ocultamiento.

3.  **Escalando para ocultar el mensaje (`showing == false` y `estado == true`):**
    En esta fase, el script disminuye gradualmente la escala en el eje X del GameObject. La bandera `round` también se establece en `false` durante esta fase, permitiendo que se inicie una nueva secuencia de fin de juego una vez que la actual haya terminado.
    ```csharp
    vector3.x = vector3.x - Time.deltaTime / movescaletime;
    transform.localScale = vector3;
    round = false;
    ```
    Cuando `transform.localScale.x` alcanza o cae por debajo de `0`, el elemento se considera completamente oculto. En este punto, `transform.localScale.x` se ajusta a `0`, la bandera `estado` se establece en `false` para indicar que el elemento está oculto, y `timetytime` se reinicia. Lo más importante es que, en este momento, se notifica al sistema global de estados del juego (`GameState.Singleton`) el resultado final (victoria o derrota) mediante el método `NextGameState`, utilizando la bandera `win` almacenada.
    ```csharp
    GameState.Singleton.NextGameState(win ? GameStates.Win : GameStates.Lose);
    transform.parent.GetComponent<Image>().enabled = false;
    ```
    Finalmente, el componente `Image` del GameObject padre se deshabilita, asegurando que cualquier panel de fondo o componente visual del padre también se oculte junto con el mensaje.

## Otros métodos

### `EndGamer(bool win)`
Este método público es el punto de entrada para iniciar la secuencia de fin de juego. Otros scripts del proyecto pueden llamarlo para activar la visualización del mensaje de victoria o derrota.

```csharp
public void EndGamer(bool win)
{
    if (round == true)
    {
        return;
    }
    timetytime = Time.time;
    round = true;
    showing = true;
    transform.parent.GetComponent<Image>().enabled = true;
    text.text = win ? "Has Ganado" : "Perdiste";
    this.win = win;
}
```
-   **Control de concurrencia:** Primero verifica si ya hay una secuencia de fin de juego en progreso (`round == true`). Si es así, el método retorna inmediatamente, evitando que se superpongan múltiples animaciones o cambios de estado.
-   **Inicialización:** Si no hay una secuencia activa, establece `timetytime` al tiempo actual, y las banderas `round` y `showing` a `true` para iniciar la animación de escalado en el método `Update()`.
-   **Activación de UI:** Habilita el componente `Image` del GameObject padre, que presumiblemente es el panel de fondo o contenedor del mensaje de fin de juego, haciéndolo visible.
-   **Establecimiento del texto:** Actualiza el componente `TextMeshProUGUI` (`text`) para mostrar el mensaje "Has Ganado" (si `win` es `true`) o "Perdiste" (si `win` es `false`).
-   **Almacenamiento del resultado:** Almacena el valor del parámetro `win` en la variable interna `this.win`, que será utilizada más adelante para la transición del estado del juego.

## Getters y Setters

1.  `round`: Esta propiedad estática indica si una secuencia de fin de juego está actualmente en curso. Se establece en `true` cuando se llama a `EndGamer` y se restablece a `false` una vez que la pantalla de fin de juego se ha escalado completamente hacia afuera y está oculta. Actúa como una bandera global para evitar que se desencadenen múltiples eventos de fin de juego simultáneamente. Su modificador `private set` asegura que solo el propio script `EndGame` pueda modificar su valor, mientras que otros scripts pueden leerlo pero no cambiarlo.
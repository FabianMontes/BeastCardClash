# EndGame
Este script `EndGame` es el componente clave para gestionar la presentación visual del resultado final de una ronda en el juego, ya sea una victoria o una derrota. Su función principal es controlar una animación de escalado para un elemento de la interfaz de usuario (UI), mostrando un mensaje de "Has Ganado" o "Perdiste", para luego hacer la transición al siguiente estado del juego una vez que la animación ha finalizado.

El script opera manipulando la escala en el eje X de su propio `GameObject` para crear un efecto de "aparecer" y "desaparecer". Al ser activado, el elemento de UI al que está adjunto (probablemente un panel o una tarjeta) se expande horizontalmente, muestra el mensaje de victoria o derrota por un breve periodo, y luego se contrae, indicando el fin de la secuencia y la transición al siguiente estado del juego a través del sistema `GameState`. La lógica interna gestiona los tiempos y estados para evitar múltiples activaciones y asegurar una experiencia fluida.

# Métodos

## Métodos de Unity

### Start
El método `Start` se ejecuta una única vez al inicio del ciclo de vida del script, justo antes de la primera actualización del fotograma. Su propósito es inicializar el estado del componente para que esté listo antes de cualquier interacción.

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

*   Inicializa la variable estática `round` a `false`, lo que indica que no hay una secuencia de fin de partida en curso.
*   Establece la variable `showing` a `false`, asegurando que la animación de aparecer no esté activa.
*   Establece la escala inicial del `GameObject` en el eje X a `0`, haciendo que el elemento de UI sea invisible al inicio del juego. Esto es fundamental para que la animación de "aparecer" sea efectiva.
*   Obtiene una referencia al componente `TextMeshProUGUI` dentro de los hijos del `GameObject` actual. Este componente es el encargado de mostrar el texto de "Has Ganado" o "Perdiste".

### Update
El método `Update` se invoca en cada fotograma del juego y contiene la lógica principal para las animaciones de escalado y las transiciones de estado. Está dividido en tres fases principales que controlan la apariencia del elemento de UI.

```csharp
void Update()
{
    // Fase 1: El elemento de UI se está expandiendo.
    if (showing && estado == false)
    {
        if (transform.localScale.x >= 1)
        {
            Vector3 vector3 = transform.localScale;
            vector3.x = 1;
            transform.localScale = vector3;
            estado = true;
            timetytime = Time.time;
        }
        else
        {
            Vector3 vector3 = transform.localScale;
            vector3.x = vector3.x + Time.deltaTime / movescaletime;
            transform.localScale = vector3;
        }
    }
    // Fase 2: El elemento de UI está completamente expandido y esperando.
    if (showing && estado == true)
    {
        if (Time.time - timetytime >= timedelay)
        {
            showing = false; // Comienza la fase de contracción.
        }
    }
    // Fase 3: El elemento de UI se está contrayendo.
    if (showing == false && estado == true)
    {
        if (transform.localScale.x <= 0)
        {
            Vector3 vector3 = transform.localScale;
            vector3.x = 0;
            transform.localScale = vector3;
            estado = false;
            timetytime = Time.time;
            GameState.singleton.NextGameState(win ? GameStates.win : GameStates.lose); // Transición al siguiente estado del juego.
            transform.parent.GetComponent<Image>().enabled = false; // Desactiva la imagen del padre.
        }
        else
        {
            Vector3 vector3 = transform.localScale;
            vector3.x = vector3.x - Time.deltaTime / movescaletime;
            transform.localScale = vector3;
            round = false; // Permite iniciar otra secuencia de fin de partida.
        }
    }
}
```

La lógica del `Update` se puede describir en tres estados principales:

1.  **Apareciendo (Expandiendo):**
    *   Condición: `showing` es `true` y `estado` es `false`.
    *   El elemento de UI incrementa su escala en X gradualmente, dando un efecto de que "aparece".
    *   Una vez que `transform.localScale.x` alcanza o supera `1`, se fija en `1` (completamente visible), `estado` se establece a `true` y `timetytime` guarda el tiempo actual para iniciar la cuenta regresiva del `timedelay`.

2.  **Visible (Esperando):**
    *   Condición: `showing` es `true` y `estado` es `true`.
    *   El elemento de UI permanece completamente visible por el tiempo especificado en `timedelay`.
    *   Una vez que el tiempo transcurrido (`Time.time - timetytime`) supera `timedelay`, la variable `showing` se establece a `false`, lo que indica el inicio de la fase de "desaparecer".

3.  **Desapareciendo (Contrayendo) y Transición de Estado:**
    *   Condición: `showing` es `false` y `estado` es `true`.
    *   El elemento de UI decrementa su escala en X gradualmente, dando un efecto de que "desaparece".
    *   Cuando `transform.localScale.x` alcanza o cae por debajo de `0`, se fija en `0` (invisible), `estado` se establece a `false`.
    *   En este punto crítico, se realiza la transición al siguiente estado del juego llamando a `GameState.singleton.NextGameState`, pasando `GameStates.win` o `GameStates.lose` según el resultado almacenado en la variable `win`.
    *   También se desactiva el componente `Image` del `GameObject` padre, lo que sugiere que este `EndGame` controla un elemento que está dentro de un contenedor mayor (como un panel de fondo).
    *   Finalmente, `round` se establece a `false`, permitiendo que se inicie una nueva secuencia de fin de partida si es necesario.

## Otros métodos

### EndGamer(bool win)
Este método público es la interfaz principal para otros scripts o eventos que necesiten iniciar la secuencia de fin de partida.

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

*   **`if (round == true) { return; }`**: Esta verificación evita que la secuencia de fin de partida se active múltiples veces mientras una ya está en curso, garantizando que el flujo de la UI y el estado del juego sean coherentes.
*   `timetytime = Time.time;`: Almacena el tiempo actual, lo que servirá como punto de partida para las animaciones y los temporizadores internos del `Update`.
*   `round = true;`: Establece la variable estática `round` a `true`, indicando que una secuencia de fin de partida ha comenzado.
*   `showing = true;`: Activa la fase de expansión del elemento de UI en el método `Update`.
*   `transform.parent.GetComponent<Image>().enabled = true;`: Habilita el componente `Image` del `GameObject` padre. Esto sugiere que el `EndGame` script maneja el contenido (texto y animaciones) dentro de un panel que tiene un componente `Image` para su fondo.
*   `text.text = win ? "Has Ganado" : "Perdiste";`: Establece el texto del `TextMeshProUGUI` obtenido en `Start` al mensaje apropiado ("Has Ganado" o "Perdiste") basándose en el valor booleano `win` pasado como parámetro.
*   `this.win = win;`: Guarda el resultado de la partida (`win`) en la variable interna del script, que será utilizada más tarde por el `Update` para determinar el `GameStates` a pasar al `GameState.singleton`.

## Getters y Setters

1.  `round` (static bool): Obtiene un valor que indica si una secuencia de fin de ronda está actualmente en progreso, impidiendo que se activen múltiples secuencias de fin de juego simultáneamente.
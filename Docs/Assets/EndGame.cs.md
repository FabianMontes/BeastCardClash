# `EndGame.cs`

## 1. Propósito General
Este script se encarga de gestionar la interfaz de usuario y la lógica de transición cuando una partida finaliza, ya sea por victoria o derrota. Controla la animación de aparición y desaparición de un mensaje de fin de juego y notifica al sistema de `GameState` el resultado.

## 2. Componentes Clave

### `EndGame`
- **Descripción:** La clase `EndGame` es un `MonoBehaviour` que gestiona la visualización del resultado final de una ronda (victoria o derrota) mediante una animación de escalado de la interfaz de usuario. Coordina el tiempo de muestra del mensaje y la transición al siguiente estado del juego.

- **Variables Públicas / Serializadas:**
    - `public static bool round { get; private set; }`: Una propiedad estática que actúa como un "candado" para asegurar que la secuencia de fin de juego solo se active una vez por ronda. `true` indica que la secuencia está en curso.
    - `[SerializeField] float timedelay = 2;`: Define la duración, en segundos, durante la cual el mensaje de "Has Ganado" o "Perdiste" permanece completamente visible antes de empezar a desaparecer.
    - `[SerializeField] float movescaletime = 0.5f;`: Determina la velocidad de la animación de escalado. Representa el tiempo, en segundos, que tarda el elemento de UI en escalar de 0 a 1 (o viceversa).

- **Métodos Principales:**
    - `void Start()`:
        Este método se ejecuta una vez cuando el script se inicializa. Su propósito es establecer el estado inicial de la UI de fin de juego. Inicializa `round` y `showing` a `false`, oculta el elemento de UI escalando su eje X a 0, y obtiene una referencia al componente `TextMeshProUGUI` que se encuentra en uno de sus hijos para poder actualizar el texto de victoria/derrota.

    - `void Update()`:
        Este método se ejecuta en cada frame y contiene la lógica principal para la animación de escalado y el control del tiempo de visualización. Opera en tres fases:
        1.  **Aparición (escalado hacia afuera):** Si `showing` es `true` y `estado` es `false`, el elemento de UI escala su tamaño en el eje X de 0 a 1. Una vez que alcanza el tamaño completo (X >= 1), `estado` se establece en `true` y `timetytime` se actualiza para iniciar la cuenta regresiva del `timedelay`.
        2.  **Retardo de visualización:** Si `showing` es `true` y `estado` es `true`, el script espera el tiempo definido por `timedelay`. Una vez transcurrido este tiempo, `showing` se establece en `false`, señalando el inicio de la fase de desaparición.
        3.  **Desaparición (escalado hacia adentro):** Si `showing` es `false` y `estado` es `true`, el elemento de UI escala su tamaño en el eje X de 1 a 0. Cuando el elemento está completamente oculto (X <= 0), `estado` se vuelve `false`, `round` se restablece a `false` (permitiendo una nueva secuencia de fin de juego), y se notifica al `GameState.singleton` el resultado final (`win` o `lose`). También se deshabilita el componente `Image` del padre, que probablemente es el panel de fondo de la UI.

    - `public void EndGamer(bool win)`:
        Este método público es el punto de entrada para activar la secuencia de fin de juego.
        - Primero, verifica si `round` ya es `true` para evitar activaciones múltiples si una secuencia ya está en curso.
        - Si no hay una secuencia activa, actualiza `timetytime` con el tiempo actual, establece `round` a `true` (bloqueando futuras activaciones), y `showing` a `true` (iniciando la animación de aparición).
        - Finalmente, habilita el componente `Image` del GameObject padre (haciendo visible el fondo de la UI de fin de juego), actualiza el texto del mensaje a "Has Ganado" o "Perdiste" según el parámetro `win`, y guarda el valor de `win` para su uso posterior en la transición de `GameState`.

- **Lógica Clave:**
La lógica principal del script reside en el método `Update`, que implementa una máquina de estados sencilla basada en las variables booleanas `showing` y `estado`, y el tiempo (`timetytime`). Esta máquina de estados gestiona la animación de escalado (hacer visible, esperar, hacer invisible) y asegura que las acciones finales (notificar `GameState`, ocultar el panel) se ejecuten solo después de que la animación de desaparición se haya completado. El uso de `round` como una bandera estática evita que múltiples eventos de fin de juego se superpongan o activen la secuencia incorrectamente.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Este script no usa `[RequireComponent]`, pero funcionalmente requiere que el GameObject al que está adjunto tenga un componente `TextMeshProUGUI` como hijo para mostrar el texto de victoria/derrota.
    - También asume que el GameObject padre tiene un componente `Image` que sirve como fondo para la UI de fin de juego, el cual es habilitado y deshabilitado por el script.

- **Eventos (Entrada):**
    El script es activado externamente mediante la llamada a su método público `EndGamer(bool win)`. Esto significa que otro sistema en el juego es responsable de determinar cuándo termina una ronda y con qué resultado, y luego invoca este método para mostrar la pantalla de fin de juego.

- **Eventos (Salida):**
    Este script no emite `UnityEvent` ni `Action` personalizados. Sin embargo, tiene una dependencia directa con el patrón Singleton `GameState.singleton` y, al finalizar la secuencia de UI, invoca el método `NextGameState()` para notificar al sistema de estados del juego sobre el resultado final (victoria o derrota). Esto permite que el juego transicione a una pantalla o estado apropiado (e.g., menú principal, pantalla de puntuaciones).
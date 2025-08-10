# `Roundanimation.cs`

## 1. Propósito General
Este script `Roundanimation` gestiona la animación visual de un indicador de "ronda" en el juego. Su función principal es controlar el escalado de un elemento UI (asociado a este `GameObject`) para mostrarlo, mantenerlo visible por un tiempo y luego ocultarlo, comunicando el estado general de la ronda a través de una propiedad estática.

## 2. Componentes Clave

### `Roundanimation`
-   **Descripción:** Esta clase, que hereda de `MonoBehaviour`, controla la secuencia de animación de un elemento visual, típicamente una imagen de UI, para indicar el inicio o la progresión de una ronda. Maneja las fases de aparición (escalado ascendente), mantenimiento y desaparición (escalado descendente) del elemento.
-   **Variables Públicas / Serializadas:**
    -   `public static bool round { get; private set; }`: Propiedad estática que indica si una ronda está actualmente "activa" o en proceso de animación. Su `private set` asegura que solo este script pueda modificar su valor, mientras que otras clases pueden leerlo.
    -   `[SerializeField] float timedelay = 2;`: Define la duración en segundos que el elemento animado permanece completamente visible (escalado al 100%) antes de comenzar a desaparecer.
    -   `[SerializeField] float movescaletime = 0.5f;`: Determina la velocidad de la animación de escalado. Un valor más pequeño resultará en una animación más rápida.
-   **Métodos Principales:**
    -   `void Start()`: Este método se ejecuta una vez al inicio cuando el script se habilita por primera vez. Inicializa la propiedad `round` y la variable `showing` a `false`. También asegura que el `GameObject` se inicialice con un escalado horizontal de 0 (`transform.localScale.x = 0`), dejándolo invisible al comienzo.
    -   `void Update()`: Este método se llama en cada frame y contiene la lógica principal de la máquina de estados de la animación:
        -   **Fase de aparición (Scaling Up):** Si `showing` es `true` y `estado` es `false`, el script incrementa progresivamente el escalado `x` del `GameObject` (`transform.localScale.x`) hasta que alcanza 1. Una vez que llega a 1, `estado` se establece en `true` y `timetytime` registra el momento actual (`Time.time`).
        -   **Fase de mantenimiento (Holding):** Si `showing` es `true` y `estado` es `true`, el script espera hasta que haya transcurrido el tiempo definido por `timedelay` desde que el elemento alcanzó su tamaño completo (`timetytime`). Al finalizar este retraso, `showing` se establece en `false`, lo que indica el inicio de la fase de desaparición.
        -   **Fase de desaparición (Scaling Down):** Si `showing` es `false` y `estado` es `true`, el script reduce progresivamente el escalado `x` del `GameObject` hasta que alcanza 0. Durante esta fase, la propiedad `round` se establece en `false` y el componente `Image` del padre del `GameObject` se deshabilita, lo que oculta completamente el indicador visual. Al llegar a 0, `estado` se restablece a `true` (esto podría ser una lógica para evitar re-entradas inmediatas o una señal de "completo", pero notifica que el ciclo completo ha terminado y el estado interno finalizado).
    -   `public void startRound()`: Este método público es el punto de entrada para iniciar la animación de la ronda. Primero verifica si `round` ya es `true` para evitar activaciones redundantes. Si no está activa, establece `round` a `true`, `showing` a `true` (para iniciar la fase de escalado ascendente) y habilita el componente `Image` del padre, haciendo visible el contenedor del indicador de ronda.

-   **Lógica Clave:**
    La animación se gestiona mediante una máquina de estados implícita dentro del método `Update`, controlada por las variables booleanas `showing` y `estado`, y el temporizador `timetytime`.
    -   `showing`: Indica si la animación está en curso (apareciendo o manteniéndose).
    -   `estado`: Se utiliza para distinguir entre la fase de "escalado ascendente" (`false`) y la fase de "mantenimiento/escalado descendente" (`true`) una vez que el elemento ha alcanzado su tamaño completo.
    La animación sigue un flujo lineal: `Oculto` -> `Escalado Arriba` -> `Mantenimiento` -> `Escalado Abajo` -> `Oculto`.

    **Observación Importante sobre Reusabilidad:** El script está diseñado para que la animación se dispare llamando a `startRound()`. Sin embargo, la variable `estado` solo se inicializa a `false` en `Start()`. Una vez que la animación de "desaparición" se completa (`transform.localScale.x <= 0`), `estado` se establece a `true` y no se vuelve a poner a `false` dentro del ciclo de `Update`. Esto significa que, tal como está implementado, una vez que la animación ha completado un ciclo completo de aparición y desaparición, la llamada subsiguiente a `startRound()` no permitirá que la animación de "escalado ascendente" se ejecute de nuevo porque `estado` permanecerá `true`. Para que la animación pueda ser repetida sin recargar la escena, la variable `estado` necesitaría ser explícitamente reseteada a `false` al inicio de `startRound()` o al final completo de la animación de desaparición.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere que su `GameObject` padre tenga un componente `UnityEngine.UI.Image`, ya que el script intenta habilitar/deshabilitar este componente durante la animación (`transform.parent.GetComponent<Image>().enabled = true/false;`).
-   **Eventos (Entrada):** Este script no se suscribe a ningún evento de Unity o de componentes externos. Es controlado externamente a través de llamadas directas a su método público `startRound()`.
-   **Eventos (Salida):** Este script no invoca ningún `UnityEvent` o `Action` personalizado para notificar a otros sistemas sobre el progreso o la finalización de la animación. Su principal "salida" de información es la propiedad estática `Roundanimation.round`.
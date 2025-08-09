# `Roundanimation.cs`

## 1. Propósito General
Este script gestiona una animación visual para indicar el inicio y fin de una ronda en el juego. Se encarga de escalar un elemento de la interfaz de usuario para que aparezca, permanezca visible por un tiempo y luego desaparezca, mientras comunica el estado de la ronda a otros sistemas a través de una variable estática.

## 2. Componentes Clave

### `Roundanimation`
- **Descripción:** Esta clase hereda de `MonoBehaviour` y es responsable de controlar la animación de escalado de un objeto de la UI, específicamente su componente `RectTransform` o `Transform`, a lo largo del eje X. Simula una aparición, una duración visible y una desaparición, y se espera que esté adjunto a un objeto UI que es hijo de otro objeto con un componente `Image`.
- **Variables Públicas / Serializadas:**
    - `public static bool round`: Una propiedad estática de solo lectura que indica si una ronda está actualmente activa o en curso. Otros scripts pueden consultar esta variable para saber el estado de la ronda.
    - `private bool showing`: Una bandera interna que controla si la animación está en fase de "mostrándose" (escalando hacia arriba o esperando) o "ocultándose" (escalando hacia abajo).
    - `private bool estado`: Una bandera interna que actúa como sub-estado dentro de la fase `showing`. `false` indica que el objeto está escalando hacia arriba o hacia abajo, y `true` indica que ha alcanzado su escala máxima (1) y está esperando el `timedelay` o ha alcanzado su escala mínima (0) y ha finalizado la animación de ocultamiento.
    - `[SerializeField] float timedelay`: Define el tiempo, en segundos, que el elemento animado permanece completamente visible (escala X de 1) antes de comenzar a desaparecer.
    - `[SerializeField] float timetytime`: Se utiliza para almacenar el `Time.time` en momentos clave de la animación (por ejemplo, cuando el elemento alcanza su escala máxima) para calcular la duración del `timedelay`.
    - `[SerializeField] float movescaletime`: Controla la velocidad de la animación de escalado. Un valor más pequeño resultará en una animación de escalado más rápida, y un valor más grande la hará más lenta.
- **Métodos Principales:**
    - `void Start()`:
        Este método se invoca una vez al inicio del ciclo de vida del script. Inicializa la variable estática `round` y la bandera `showing` a `false`. También establece la escala inicial del objeto a lo largo del eje X a 0, haciendo que el elemento sea invisible al principio.
        ```csharp
        void Start()
        {
            round = false;
            showing = false;
            Vector3 vector3 = transform.localScale;
            vector3.x = 0;
            transform.localScale = vector3;
        }
        ```
    - `void Update()`:
        Este método se llama una vez por fotograma y contiene la lógica principal de la máquina de estados de la animación. Gestiona las transiciones entre las fases de aparición, espera y desaparición del elemento.
        *   **Fase de Aparición (Escalado hacia arriba):** Si `showing` es `true` y `estado` es `false`, el script aumenta gradualmente la escala del eje X del objeto hasta que alcanza o supera 1. Una vez que la escala es 1, se fija en 1, `estado` se cambia a `true` (indicando que ha llegado al máximo), y se registra el tiempo actual en `timetytime` para la fase de espera.
        *   **Fase de Espera:** Si `showing` es `true` y `estado` es `true`, el script espera hasta que el tiempo transcurrido desde `timetytime` supere `timedelay`. Una vez que se cumple el retardo, `showing` se establece en `false` para iniciar la fase de desaparición.
        *   **Fase de Desaparición (Escalado hacia abajo):** Si `showing` es `false` y `estado` es `true` (lo que indica que ha terminado la fase de espera y ahora debe ocultarse), el script disminuye gradualmente la escala del eje X del objeto hasta que alcanza o desciende por debajo de 0. Una vez que la escala es 0, se fija en 0, `round` se establece en `false`, y el componente `Image` del padre del objeto se deshabilita para asegurar que no se dibuje. El valor `estado = true` y el registro de `timetytime` aquí parecen indicar un reinicio del estado para una futura animación.
    - `public void startRound()`:
        Este método público permite que otros scripts inicien la animación de la ronda. Primero, verifica si ya hay una ronda activa (`round == true`) para evitar múltiples inicios. Si no hay una ronda activa, establece `round` en `true`, `showing` en `true` (para comenzar la animación de aparición), y habilita el componente `Image` del padre del objeto, haciendo que el elemento sea visible para la animación.
        ```csharp
        public void startRound()
        {
            if(round == true)
            {
                return;
            }
            round = true;
            showing = true ;
            transform.parent.GetComponent<Image>().enabled = true;
        }
        ```
- **Lógica Clave:**
    La animación se gestiona a través de una máquina de estados simple dentro del método `Update`, controlada por las variables booleanas `showing` y `estado`.
    1.  **Inicialización (`Start`):** El elemento se oculta estableciendo su escala X a 0.
    2.  **Aparición (`Update` - Fase 1):** Cuando `startRound()` es llamado, `showing` se activa. El elemento escala gradualmente desde X=0 hasta X=1.
    3.  **Espera (`Update` - Fase 2):** Una vez que el elemento alcanza X=1, entra en un estado de espera, donde permanece completamente visible por el `timedelay` definido.
    4.  **Desaparición (`Update` - Fase 3):** Después del `timedelay`, el elemento comienza a escalar gradualmente desde X=1 hasta X=0, volviéndose invisible nuevamente. Al finalizar, la variable `round` se establece en `false` y la imagen del padre se deshabilita.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, tiene una dependencia implícita muy fuerte:
    *   Requiere que el objeto padre (`transform.parent`) del GameObject al que está adjunto este script tenga un componente `Image`. El script intenta acceder y habilitar/deshabilitar este componente para controlar la visibilidad general del efecto. Si el padre no tiene un `Image`, se generará un error de referencia nula en tiempo de ejecución.
- **Eventos (Entrada):** Este script no se suscribe a eventos de Unity (`UnityEvent`) ni a acciones personalizadas (`Action`). Su funcionalidad es invocada externamente a través de su método público `startRound()`.
- **Eventos (Salida):** Este script no invoca ningún evento ni acción para notificar a otros sistemas. Sin embargo, expone el estado actual de la ronda a través de la propiedad estática pública `Roundanimation.round`, que otros scripts pueden leer directamente.
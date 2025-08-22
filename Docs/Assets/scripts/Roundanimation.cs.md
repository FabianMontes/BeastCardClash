# `Roundanimation.cs`

## 1. Propósito General
Este script gestiona la animación visual de un indicador de ronda (ej. "Ronda X") dentro del juego. Controla la aparición y desaparición de un elemento de texto y su fondo, interactuando con el sistema de combate para señalar el inicio y el fin de la visualización de la ronda.

## 2. Componentes Clave

### `Roundanimation`
-   **Descripción:** `Roundanimation` es un `MonoBehaviour` responsable de la lógica de animación para mostrar el número de la ronda actual. Se encarga de escalar un elemento UI (probablemente un texto `TextMeshProUGUI` y su panel de fondo) para que aparezca, permanezca visible por un tiempo y luego desaparezca, notificando al sistema de combate cuando la animación ha terminado.

-   **Variables Públicas / Serializadas:**
    *   `public static bool round { get; private set; }`: Una propiedad estática que indica si una ronda está actualmente activa o en proceso de animación. Su `private set` asegura que solo esta clase puede modificar su valor, mientras otras pueden leerlo para saber el estado de la ronda.
    *   `[SerializeField] float timedelay = 2;`: Determina la duración en segundos durante la cual el texto de la ronda permanecerá completamente visible (en su escala máxima) antes de comenzar a desaparecer. Este valor se puede ajustar desde el Inspector de Unity.
    *   `[SerializeField] float movescaletime = 0.5f;`: Define el tiempo en segundos que tarda la animación en completar un ciclo de escala (ya sea para aparecer completamente o para desaparecer completamente). También es configurable en el Inspector de Unity.
    *   `private bool showing = false;`: Una bandera interna que controla si la animación de la ronda debería estar en su fase de "mostrando" (escalando o en espera).
    *   `private bool estado = false;`: Una bandera de estado que ayuda a la máquina de estados interna a diferenciar entre las fases de escalado (entrando, saliendo) y la fase de "mantener" la animación.
    *   `private float timetytime = 0;`: Una variable de tiempo utilizada para registrar el momento en que se inicia una fase de la animación (ej. el inicio de la fase de "mantener" o el inicio de una nueva ronda) para cálculos de duración.
    *   `TextMeshProUGUI text;`: Una referencia al componente `TextMeshProUGUI` que se espera sea un hijo de este GameObject. Este componente se utiliza para mostrar el texto real de la ronda (ej. "Ronda 1").

-   **Métodos Principales:**
    *   `void Start()`: Este método se llama una vez al inicio del ciclo de vida del script. Inicializa las banderas `round` y `showing` a `false`. También asegura que el GameObject que contiene este script comienza con una escala en X de 0 (es decir, invisible), y obtiene la referencia al componente `TextMeshProUGUI` que se encuentra en uno de sus hijos.
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
    *   `void Update()`: Este método se ejecuta en cada fotograma del juego y contiene la lógica principal de la animación. Gestiona tres fases distintas:
        1.  **Fase de Aparición (Scaling In):** Si `showing` es `true` y `estado` es `false`, el script aumenta gradualmente la escala `x` del GameObject hasta alcanzar o superar 1. Una vez que la escala es completa, `estado` se establece en `true` y `timetytime` se actualiza para marcar el inicio de la fase de "mantener".
        2.  **Fase de Mantener (Holding):** Si `showing` es `true` y `estado` es `true`, el script comprueba si el `timedelay` ha transcurrido desde que se inició esta fase (`timetytime`). Si es así, establece `showing` a `false` para indicar que la animación debe pasar a la fase de desaparición.
        3.  **Fase de Desaparición (Scaling Out):** Si `showing` es `false` y `estado` es `true`, el script disminuye gradualmente la escala `x` del GameObject hasta que alcanza o cae por debajo de 0. Una vez invisible, `estado` se restablece a `false`, `round` se pone a `false`, el componente `Image` del padre (presumiblemente el fondo del texto) se desactiva, y se llama a `Combatjudge.combatjudge.endRoundeded()` para notificar al sistema de combate que la animación de la ronda ha finalizado.
    *   `public void startRound()`: Este método público se utiliza para iniciar la animación de la ronda. Primero, verifica si ya hay una ronda activa (`round == true`) para evitar activaciones duplicadas. Si no hay una ronda activa, reinicia `timetytime`, establece `round` a `true` y `showing` a `true` para iniciar la animación de aparición. Además, activa el componente `Image` del objeto padre (que probablemente sirve como fondo para el texto de la ronda) y actualiza el texto `TextMeshProUGUI` para mostrar el número de la ronda actual, obteniéndolo de `Combatjudge.combatjudge.round`.
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
            text.text = $"Ronda {Combatjudge.combatjudge.round}";
        }
        ```

-   **Lógica Clave:** La lógica central de este script se basa en una máquina de estados simple controlada por las variables booleanas `showing` y `estado`, junto con cálculos de tiempo para interpolar suavemente la escala del elemento UI. Utiliza `Time.deltaTime` para un movimiento independiente de la tasa de fotogramas, lo que garantiza una animación fluida en diferentes configuraciones de hardware.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere un componente `TextMeshProUGUI` como hijo para mostrar el texto de la ronda. También espera que su GameObject padre tenga un componente `Image` que será habilitado y deshabilitado por el script.

-   **Eventos (Entrada):** Este script no se suscribe directamente a eventos de Unity o eventos C# estándar. Su método `startRound()` es un punto de entrada público que se espera sea invocado por otro script (ej. un gestor de juego o el script `Combatjudge`) para iniciar la animación de la ronda.

-   **Eventos (Salida):**
    *   Este script notifica el final de la animación de la ronda invocando el método `Combatjudge.combatjudge.endRoundeded()`. Esto crea una dependencia directa con el sistema `Combatjudge`, señalando que la fase visual de la ronda ha concluido y el combate puede continuar.
    *   Expone la propiedad estática `Roundanimation.round`, que otras partes del código pueden consultar para saber si una animación de ronda está actualmente en curso.
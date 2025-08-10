# `Roundanimation.cs`

## 1. Propósito General
Este script gestiona la animación visual de un indicador de ronda, controlando su aparición, permanencia y desaparición a través de la manipulación de su escala. Interactúa principalmente con el componente `Transform` del GameObject al que está adjunto y el componente `Image` de su padre para controlar la visibilidad.

## 2. Componentes Clave

### `Roundanimation`
-   **Descripción:** Esta clase, que hereda de `MonoBehaviour`, es responsable de animar la escala horizontal de un GameObject para simular la entrada y salida de un indicador de ronda. También controla un estado global estático (`round`) para indicar si una ronda está activa o en proceso de animación.
-   **Variables Públicas / Serializadas:**
    -   `public static bool round { get; private set; }`: Una propiedad estática que indica si la animación de la ronda está actualmente activa. Su setter es privado, lo que significa que solo este script puede modificar su valor, pero cualquier otro script puede leerlo.
    -   `[SerializeField] float timedelay`: Define el tiempo en segundos que el indicador de ronda permanece visible a su escala máxima antes de comenzar a desaparecer. Es configurable desde el Inspector de Unity.
    -   `[SerializeField] float movescaletime`: Determina la velocidad a la que el indicador de ronda se escala (tanto para aparecer como para desaparecer). Un valor más pequeño implica una animación más rápida. Es configurable desde el Inspector de Unity.
-   **Métodos Principales:**
    -   `void Start()`: Este método del ciclo de vida de Unity se llama una vez al inicio. Inicializa la variable `round` a `false` y `showing` a `false`. Crucialmente, establece la escala `x` del GameObject a `0`, asegurando que el indicador no sea visible al inicio.
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
    -   `void Update()`: Este método del ciclo de vida de Unity se llama en cada frame y contiene la lógica principal de la animación:
        -   **Fase de aparición (Scaling Up):** Si `showing` es `true` y `estado` es `false`, el script aumenta gradualmente `transform.localScale.x` hasta que alcanza o supera `1`. Una vez que esto ocurre, `estado` se establece en `true` (indicando que la fase de aparición ha terminado) y se registra el tiempo actual en `timetytime`.
        -   **Fase de permanencia (Delay at Full Scale):** Si `showing` es `true` y `estado` es `true` (el indicador está completamente visible), el script espera el tiempo definido por `timedelay`. Una vez transcurrido este tiempo, `showing` se establece en `false`, iniciando la fase de desaparición.
        -   **Fase de desaparición (Scaling Down):** Si `showing` es `false` y `estado` es `true` (el indicador ha terminado su fase de aparición/permanencia y ahora debe ocultarse), el script disminuye gradualmente `transform.localScale.x` hasta que alcanza o desciende de `0`. Al llegar a `0`, `round` se establece en `false` (indicando que la animación de ronda ha finalizado) y el componente `Image` del GameObject padre se deshabilita para asegurar la invisibilidad.
    -   `public void startRound()`: Este método público se utiliza para iniciar la animación de la ronda. Primero verifica si `round` ya es `true` para evitar activaciones redundantes. Si no está activa, establece `round` a `true`, `showing` a `true` (para comenzar la animación de aparición), y habilita el componente `Image` del GameObject padre, que es el elemento visual que representa la ronda.
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
-   **Lógica Clave:**
    El script implementa una máquina de estados sencilla gestionada por las variables `showing` y `estado`, junto con el tiempo. `showing` controla la dirección general (aparecer o desaparecer), mientras que `estado` ayuda a identificar si una transición de escala ha terminado (ya sea a escala máxima o mínima). La animación se logra modificando `transform.localScale.x` en cada `Update` utilizando `Time.deltaTime` para una velocidad consistente independientemente del framerate.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza la anotación `[RequireComponent]`. Sin embargo, depende implícitamente de tener un componente `Transform` en el mismo GameObject para modificar su escala. Además, busca y habilita/deshabilita un componente `Image` en su *GameObject padre*, lo que implica una relación jerárquica en la escena de Unity donde un `Image` es el visual principal y este script (en un hijo) controla su animación.
-   **Eventos (Entrada):** Este script no se suscribe directamente a eventos de Unity (`button.onClick`, etc.). Su activación principal es a través de la llamada pública a su método `startRound()`.
-   **Eventos (Salida):** El script no invoca explícitamente `UnityEvent` ni `Action`. En cambio, comunica el estado de la animación de la ronda a través de la propiedad estática `Roundanimation.round`. Otros scripts pueden leer esta propiedad para determinar si una ronda está actualmente activa o en curso de animación.
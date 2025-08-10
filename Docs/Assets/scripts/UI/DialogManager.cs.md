# `DialogManager.cs`

## 1. Propósito General
Este script gestiona la interacción básica de un personaje con el jugador (target) para mostrar u ocultar un panel de diálogo. Su rol principal es controlar la visibilidad de la interfaz de usuario de diálogo y pausar/reanudar el movimiento del jugador cuando el diálogo está activo.

## 2. Componentes Clave

### `DialogManager`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, controla la lógica para mostrar un panel de diálogo cuando el jugador está dentro de un rango específico y presiona una tecla. También se encarga de pausar el movimiento del jugador mientras el diálogo está visible.
- **Variables Públicas / Serializadas:**
    - `dialogPanel` (`GameObject`): Referencia al objeto `GameObject` que representa el panel de diálogo en la interfaz de usuario. Este panel se activa o desactiva para mostrar u ocultar el diálogo.
    - `namePanel` (`TextMeshProUGUI`): Componente de texto de TextMeshPro que se usa para mostrar el nombre del personaje en el panel de diálogo.
    - `characterName` (`string`): El nombre del personaje que aparecerá en el `namePanel` cuando el diálogo esté activo.
    - `maxDistance` (`float`): La distancia máxima entre el personaje (donde está este script) y el `target` (el jugador) para que la interacción de diálogo sea posible.
    - `target` (`Transform`): Una referencia al `Transform` del objeto que representa al jugador o el objetivo de interacción.
- **Métodos Principales:**
    - `void Start()`:
        - **Propósito:** Se ejecuta una vez al inicio del ciclo de vida del script.
        - **Acciones:** Obtiene una referencia al script `Target` del `target` especificado (asumiendo que el `target` tiene un componente `Target` para controlar su movimiento). Inicialmente, desactiva el `dialogPanel` para asegurarse de que no sea visible al comenzar el juego.
    - `void Update()`:
        - **Propósito:** Se llama una vez por cada fotograma. Contiene la lógica principal de detección de distancia y manejo de entrada para el diálogo.
        - **Acciones:**
            1.  Calcula la distancia entre la posición del personaje (el `GameObject` al que está adjunto este script) y la posición del `target`.
            2.  Verifica si esta distancia es menor o igual a `maxDistance`.
            3.  Si el `target` está en rango y el jugador presiona la tecla 'Z':
                *   Alterna el estado de activación del `dialogPanel` (si está activo, lo desactiva; si está inactivo, lo activa).
                *   Alterna el estado `enabled` del componente `targetMovement` de forma opuesta al `dialogPanel`. Esto significa que si el panel se activa, el movimiento del jugador se desactiva, y viceversa.
                *   Si el `dialogPanel` se ha activado, actualiza el texto del `namePanel` con el `characterName` configurado.
- **Lógica Clave:**
    La lógica central del `DialogManager` reside en su método `Update`. Monitorea constantemente la proximidad del jugador y la entrada del teclado (`KeyCode.Z`). Cuando se cumplen ambas condiciones, activa o desactiva el panel de diálogo. Es crucial la interacción con el script `Target` del jugador: el movimiento del jugador se desactiva automáticamente cuando el diálogo está abierto, y se vuelve a activar cuando el diálogo se cierra, proporcionando una experiencia de usuario fluida y sin interrupciones durante las conversaciones.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Aunque no utiliza el atributo `[RequireComponent]`, este script depende de que el `GameObject` asignado a la variable `target` tenga un componente llamado `Target` (presumiblemente un script que controle el movimiento del jugador) para poder deshabilitar/habilitar su movimiento.
- **Eventos (Entrada):**
    - Este script responde directamente a la entrada del teclado. Específicamente, detecta cuando la tecla `Z` es presionada (`Input.GetKeyDown(KeyCode.Z)`).
- **Eventos (Salida):**
    - Este script no invoca explícitamente `UnityEvent` o `Action` para notificar a otros sistemas. En su lugar, modifica directamente el estado (`SetActive`) del `dialogPanel` y el estado `enabled` del componente `Target` del jugador.
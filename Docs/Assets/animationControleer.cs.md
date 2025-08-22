# `animationControleer.cs`

## 1. Propósito General
Este script es el encargado principal de gestionar y sincronizar las animaciones de un personaje en combate. Su rol es traducir el estado del juego y las propiedades del personaje (`Figther`, `PlayerToken`) en parámetros para el componente `Animator`, asegurando que el modelo visible del personaje ejecute las animaciones correctas en cada momento.

## 2. Componentes Clave

### `animationControleer`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, controla la selección del modelo de personaje y la actualización de los parámetros del `Animator` asociado. Está diseñada para ser adjuntada a un GameObject que actúa como un contenedor para los diferentes modelos de personaje, cada uno como un hijo de este GameObject.
- **Variables Públicas / Serializadas:**
    - `Animator animato`: Una referencia al componente `Animator` del modelo de personaje actualmente activo. Se utiliza para establecer los diferentes parámetros de animación como velocidad, estado de combate, etc.
    - `Figther figther`: Una referencia a la instancia de la clase `Figther` que representa la lógica de combate y las estadísticas del personaje asociado. Este objeto contiene información vital como el índice del luchador (`indexFigther`) y si ha recibido daño (`noHurt`).
    - `PlayerToken player`: Una referencia al componente `PlayerToken` adjunto al mismo GameObject. Este componente es el puente para obtener el objeto `Figther` y provee información dinámica como la velocidad de movimiento del personaje.
- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se ejecuta una vez al inicio del ciclo de vida del script, antes de la primera actualización.
        - Se encarga de obtener las referencias a los componentes `PlayerToken` y `Figther` del GameObject.
        - Llama a `setModel()` para activar el modelo 3D correcto del personaje basado en `figther.indexFigther`.
        - Inicializa la variable `animato` obteniendo el componente `Animator` del modelo activo.
        - Establece el parámetro booleano `"isFigthing"` del `Animator` a `true`, lo que probablemente inicia una animación de reposo o preparación para el combate.
        - **Fragmento de código relevante:**
            ```csharp
            player = GetComponent<PlayerToken>();
            figther = player.player;
            setModel(figther.indexFigther);
            animato = transform.GetChild(figther.indexFigther).GetComponentInChildren<Animator>();
            animato.SetBool("isFigthing",true);
            ```
    - `void Update()`: Este método del ciclo de vida de Unity se llama una vez por cada frame del juego.
        - Su función principal es actualizar continuamente los parámetros del `Animator` basándose en el estado actual del juego.
        - Actualiza el parámetro `Float` `"Speed"` del `Animator` con el valor proporcionado por `player.Speed()`.
        - Establece el parámetro `Bool` `"EndTurn"` en `true` si el estado de combate global, obtenido de `Combatjudge.combatjudge.GetSetMoments()`, es `SetMoments.Result`. Esto controla animaciones al final del turno.
        - Actualiza el parámetro `Bool` `"didWin"` basándose en `figther.noHurt`, que indica si el luchador no ha sufrido daño (potencialmente una condición de victoria o de no-derrota).
        - Establece el parámetro `Integer` `"ElementHurt"` utilizando el tipo de combate actual (`combatType`) del `Combatjudge`, lo que permite al `Animator` disparar animaciones de reacción específicas a diferentes tipos de daño elemental.
        - **Fragmento de código relevante:**
            ```csharp
            animato.SetFloat("Speed", player.Speed());
            animato.SetBool("EndTurn", Combatjudge.combatjudge.GetSetMoments() == SetMoments.Result);
            animato.SetBool("didWin", figther.noHurt);
            animato.SetInteger("ElementHurt", (int) Combatjudge.combatjudge.combatType);
            ```
    - `void setModel(int index)`: Este método privado es una utilidad para gestionar la visibilidad de los modelos de personaje.
        - Desactiva explícitamente los primeros cuatro GameObjects hijos del GameObject al que está adjunto este script.
        - Luego, activa únicamente el GameObject hijo cuyo índice coincide con el parámetro `index`. Se asume que cada hijo representa un modelo de personaje distinto y que `figther.indexFigther` determinará cuál se muestra.
        - **Fragmento de código relevante:**
            ```csharp
            transform.GetChild(0).gameObject.SetActive(false);
            // ... (desactiva otros hijos)
            transform.GetChild(index).gameObject.SetActive(true);
            ```
- **Lógica Clave:** La lógica central de este script reside en su capacidad para actuar como un puente entre la información del estado del juego (proporcionada por `PlayerToken` y `Combatjudge.combatjudge`) y el sistema de animación de Unity. Al inicializar, selecciona y activa el modelo de personaje correcto. En cada frame, sincroniza continuamente los parámetros del `Animator` (como "Speed", "EndTurn", "didWin", "ElementHurt") con los datos más recientes del juego, permitiendo que las animaciones reflejen fielmente lo que ocurre en el combate. La propiedad `[DefaultExecutionOrder(1)]` asegura que este script se ejecute después de los scripts predeterminados o con un orden de ejecución inferior, lo que puede ser útil si los datos de `PlayerToken` o `Combatjudge` necesitan ser calculados antes de que las animaciones se actualicen.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, para su correcto funcionamiento, el GameObject al que esté adjunto debe tener un componente `PlayerToken` y sus GameObjects hijos deben contener los modelos de personaje con sus respectivos `Animator`s.
- **Eventos (Entrada):** Este script no se suscribe directamente a eventos programáticos (`UnityEvent`, `Action`). En su lugar, lee y reacciona a los valores y estados expuestos por las instancias de `PlayerToken` y `Combatjudge.combatjudge` en cada `Update`.
- **Eventos (Salida):** Este script no invoca ningún evento (`UnityEvent` o `Action`) para notificar a otros sistemas sobre cambios o acciones realizadas. Se enfoca exclusivamente en el control del `Animator` local.
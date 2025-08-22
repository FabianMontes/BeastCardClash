# `liveSeter.cs`

## 1. Propósito General
Este script `liveSeter` tiene como rol principal la actualización visual de la salud (o "vidas") de un personaje en la interfaz de usuario del juego. Gestiona la representación de la vida del personaje en una barra de progreso (`Slider`) y como un valor numérico en un texto (`TextMeshProUGUI`), interactuando directamente con el estado de vida de un componente `Figther` y la lógica de combate global.

## 2. Componentes Clave

### `liveSeter`
- **Descripción:** `liveSeter` es una clase `MonoBehaviour` de Unity diseñada para actuar como un controlador de UI para la visualización de la vida de un personaje. Su función es leer constantemente el valor de vida de un componente `Figther` asociado y reflejarlo en elementos de la interfaz de usuario, específicamente un `Slider` (probablemente una barra de vida) y un `TextMeshProUGUI` (para mostrar el valor numérico de la vida).

- **Variables Clave Internas:**
    Aunque no son variables públicas ni serializadas para el Inspector de Unity, las siguientes variables son fundamentales para el funcionamiento interno de este script, manteniendo referencias a los componentes con los que interactúa:
    *   `Figther figther`: Una referencia al componente `Figther` del personaje cuya vida se está monitorizando y mostrando. Se espera que este componente resida en un GameObject padre de aquel al que está adjunto `liveSeter`.
    *   `Slider slider`: Una referencia al componente `Slider` de la UI de Unity, que se utiliza para representar visualmente la vida como una barra de progreso. Se espera que este `Slider` sea un hijo del GameObject que contiene este script.
    *   `TextMeshProUGUI texter`: Una referencia al componente `TextMeshProUGUI`, que se encarga de mostrar el valor numérico exacto de la vida actual del `Figther`. Al igual que el `Slider`, se espera que sea un hijo del GameObject que contiene este script.

- **Métodos Principales:**
    *   `void Start()`:
        Este método de ciclo de vida de Unity se invoca una vez al inicio, antes de la primera actualización del frame. Su propósito es inicializar las referencias a los componentes necesarios para que el script funcione correctamente.
        Dentro de `Start`, el script obtiene la referencia al componente `Figther` buscando en los GameObjects padres (`GetComponentInParent<Figther>()`). Esto implica que `liveSeter` debe estar adjunto a un GameObject que es descendiente del GameObject que posee el `Figther`.
        Asimismo, obtiene las referencias al `Slider` y al `TextMeshProUGUI` buscando entre los GameObjects hijos (`GetComponentInChildren<Slider>()` y `GetComponentInChildren<TextMeshProUGUI>()` respectivamente). Esto establece una jerarquía en la escena donde los elementos de UI de vida son hijos del GameObject que alberga `liveSeter`.

    *   `void Update()`:
        Este método de ciclo de vida de Unity se ejecuta en cada frame del juego. Su función principal es asegurar que la interfaz de usuario de vida se mantenga sincronizada con el estado actual de la vida del `Figther`.
        Dentro de `Update`, el valor del `slider` se actualiza para reflejar el porcentaje de vida actual. Esto se logra dividiendo la vida actual del `figther` (obtenida a través de `figther.GetPlayerLive()`) por la vida inicial máxima del combate (obtenida de `Combatjudge.combatjudge.initialLives`). El resultado se convierte a tipo `float` para la asignación al `slider.value`.
        Simultáneamente, el contenido de texto del `texter` se actualiza para mostrar el valor numérico de la vida actual del `figther`, convertido a una cadena de texto mediante `.ToString()`.

- **Lógica Clave:**
    La lógica central del script `liveSeter` se implementa dentro del método `Update`. En cada fotograma, el script realiza dos operaciones clave: primero, calcula el progreso de la barra de vida (`Slider`) dividiendo la vida actual del `Figther` por la vida inicial máxima definida globalmente en `Combatjudge`. Esto proporciona una representación visual de la salud. Segundo, toma el valor numérico de la vida actual y lo convierte a texto para mostrarlo directamente en un `TextMeshProUGUI`. Esta ejecución continua en cada `Update` garantiza que la UI de vida del personaje esté siempre actualizada en tiempo real, proporcionando al jugador una información precisa y constante sobre el estado de salud.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, para su correcto funcionamiento, requiere lógicamente la presencia de un componente `Figther` en un GameObject padre y componentes `Slider` y `TextMeshProUGUI` en GameObjects hijos del GameObject donde `liveSeter` está adjunto. Si estos componentes no se encuentran en sus ubicaciones esperadas, el script no podrá inicializar sus referencias y no funcionará correctamente.

- **Eventos (Entrada):**
    El script `liveSeter` no se suscribe explícitamente a ningún evento externo (como `UnityEvent` o acciones C# delegadas). Su mecanismo de actualización se basa únicamente en los métodos de ciclo de vida de Unity (`Start` para inicialización y `Update` para la actualización continua por frame).

- **Eventos (Salida):**
    El script `liveSeter` no invoca ni emite ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas o componentes en el juego. Su rol es exclusivamente de lectura y actualización visual de la UI.
# `Sfxcombatmanager.cs`

## 1. Propósito General
Este script es responsable de gestionar y reproducir los efectos de sonido (SFX) asociados a las diferentes fases o "momentos" del combate dentro del juego. Interactúa directamente con el sistema `Combatjudge` para determinar cuándo y qué sonido debe reproducirse.

## 2. Componentes Clave

### `Sfxcombatmanager`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, actúa como un controlador central para los efectos de sonido durante el ciclo de combate. Su función principal es reaccionar a los cambios de estado del combate y reproducir el clip de audio correspondiente. Para ello, requiere que haya un componente `AudioSource` adjunto al mismo GameObject donde se encuentre este script.

- **Variables Públicas / Serializadas:**
    - `clips`: Un array de `AudioClip` (`AudioClip[]`). Esta variable está serializada (`[SerializeField]`), lo que permite que sea configurada y poblada directamente desde el Inspector de Unity. Contiene la colección de todos los efectos de sonido que pueden ser reproducidos por este manager. Cada elemento en el array representa un sonido específico que se puede invocar por su índice.

- **Métodos Principales:**
    - `void Start()`: Este es un método del ciclo de vida de Unity que se invoca una única vez al inicio, antes de la primera actualización del frame. Su propósito es inicializar la referencia al componente `AudioSource` que se encuentra en el mismo GameObject, utilizando `GetComponent<AudioSource>()`. Este `AudioSource` será el encargado de reproducir todos los clips de audio.
    - `void Update()`: Un método del ciclo de vida de Unity que se ejecuta una vez por cada fotograma. Contiene la lógica principal de este manager. En cada fotograma, consulta el estado actual del combate a través de `Combatjudge.combatjudge.GetSetMoments()`. Utiliza una sentencia `switch` para evaluar este estado y, dependiendo del "momento" actual del combate (ej., `RollDice`, `MoveToRock`, `Result`), llama al método `changeSource` para reproducir el efecto de sonido apropiado.
    - `public void changeSource(int index, bool Force, bool loop)`: Este método es el encargado de controlar la reproducción de los clips de audio.
        - `index` (int): El índice del `AudioClip` dentro del array `clips` que se desea reproducir. Un valor de `-1` indica que se debe detener cualquier sonido en reproducción sin iniciar uno nuevo.
        - `Force` (bool): Un indicador booleano. Si es `true`, forzará la reproducción del sonido especificado, incluso si ya hay un sonido en curso o si es el mismo clip que el último reproducido. Si es `false`, el método evitará la reproducción si el audio ya está sonando y no se fuerza, o si el `index` es el mismo que el del último clip reproducido.
        - `loop` (bool): Un booleano que determina si el `AudioClip` debe reproducirse en un bucle continuo.
        La implementación del método primero verifica las condiciones para evitar la reproducción (`Force` y el `last` clip). Luego, actualiza el índice del último clip reproducido (`last`), detiene cualquier sonido actual del `AudioSource` y, si el `index` no es `-1`, asigna el `AudioClip` correspondiente del array `clips` a la propiedad `resource` del `AudioSource` (Nota: la propiedad estándar para asignar un clip a un `AudioSource` en Unity es `AudioSource.clip`; la presencia de `resource` aquí es inusual y podría implicar una extensión personalizada del `AudioSource` o ser un error de tipografía). Finalmente, reproduce el sonido y configura su propiedad `loop`.

- **Lógica Clave:**
    La lógica central del `Sfxcombatmanager` se basa en un patrón de sondeo de estado dentro de su método `Update`. El script monitorea constantemente el progreso del combate a través del enum `SetMoments` del `Combatjudge`. Cada fase crítica del combate está mapeada a un índice específico del array `clips`. El método `changeSource` es fundamental para gestionar la reproducción, incluyendo la prevención de repeticiones innecesarias (mediante la variable `last` y el parámetro `Force`) y la capacidad de detener todos los sonidos o reproducir un clip en bucle.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script depende de la existencia de un componente `AudioSource` en el mismo GameObject donde está adjunto. Si este componente no está presente, el script no podrá inicializar su referencia de audio y, por lo tanto, no podrá reproducir ningún sonido.
- **Eventos (Entrada):** El `Sfxcombatmanager` no se suscribe explícitamente a eventos de Unity (`UnityEvent` o `Action`). En su lugar, opera mediante un bucle de sondeo (`Update` method) que consulta activamente el estado del sistema `Combatjudge` (`Combatjudge.combatjudge.GetSetMoments()`) para determinar cuándo actuar.
- **Eventos (Salida):** Este script no invoca ni publica ningún evento propio (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas. Su función se limita a la reproducción de sonido basada en el estado externo del combate.
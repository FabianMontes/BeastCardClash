Aquí tienes la documentación técnica para el script `RockBehavior.cs`, diseñada para un nuevo miembro del equipo:

---

# `RockBehavior.cs`

## 1. Propósito General
Este script gestiona el comportamiento y la apariencia de un objeto "Roca" dentro del entorno de juego. Controla su tipo elemental (`Inscription`), su estado visual (brillo y sprite), su posición dentro de una zona de juego (`PlayZone`), y la interacción con los `PlayerToken` que se encuentran sobre ella.

## 2. Componentes Clave

### `Inscription`
- **Descripción:** Un `enum` que define los diferentes tipos elementales o propiedades especiales que puede tener una roca. Cada valor tiene un índice asociado que probablemente corresponde a un `Sprite` en el array `sprite`.
- **Valores:**
    - `fire = 0`
    - `earth = 1`
    - `water = 2`
    - `air = 3`
    - `duel = 4`
    - `pick = 6`
    - `empty = 5`

### `RockBehavior`
- **Descripción:** Un script `MonoBehaviour` que controla la lógica y los atributos de una instancia individual de "Roca" en la escena de Unity. Se encarga de su inicialización visual, su interactividad y el seguimiento de los `PlayerToken` que la ocupan.
- **Variables Públicas / Serializadas:**
    - `public PlayZone father;`: Referencia al objeto `PlayZone` padre al que pertenece esta roca. Se usa para posicionamiento y escalado.
    - `[SerializeField] private Sprite[] sprite;`: Un array de `Sprite` que almacena las imágenes correspondientes a cada tipo de `Inscription`. Se asigna desde el Inspector de Unity.
    - `[SerializeField] public Inscription inscription = Inscription.empty;`: El tipo de inscripción elemental de esta roca, determinando su apariencia y posible función en el juego. Se asigna desde el Inspector.
    - `public Vector3 direction = Vector3.forward;`: Vector utilizado en el cálculo de la posición inicial de la roca respecto a su `father`.
    - `public float angle = 0;`: Ángulo utilizado en el cálculo de la rotación inicial de la roca.
    - `public int numbchild = 0;`: Un índice numérico que representa la posición de esta roca dentro de la colección de rocas de su `father` (`PlayZone`). Útil para identificar vecinos.
    - `public bool shiny = false;`: Un flag booleano que determina si la roca debe "brillar" (cambiar a color amarillo).

- **Métodos Principales:**
    - `void Start()`:
        - **Descripción:** Método del ciclo de vida de Unity que se ejecuta una vez al inicio.
        - **Funcionalidad:** Inicializa las referencias a los `SpriteRenderer` del símbolo (`simbol`) y de la propia roca (`itself`). Asigna el `sprite` correcto basado en la `inscription` de la roca. Si tiene un `father` asignado, calcula y aplica la posición, escala y rotación de la roca en relación con la `PlayZone` padre.
        - **Fragmento de código:**
            ```csharp
            simbol = transform.GetChild(0).GetComponent<SpriteRenderer>();
            itself = transform.GetComponent<SpriteRenderer>();
            simbol.sprite = sprite[(int)inscription];
            // ... (posicionamiento y rotación)
            ```
    - `void Update()`:
        - **Descripción:** Método del ciclo de vida de Unity que se ejecuta en cada frame.
        - **Funcionalidad:** Actualiza el color de la propia roca (`itself.color`). Si el momento actual del juego, según `Combatjudge`, no es `SetMoments.GlowRock`, desactiva el brillo (`shiny = false`). Luego, el color se establece a amarillo si `shiny` es verdadero, y a negro si es falso.
    - `private void OnMouseDown()`:
        - **Descripción:** Método de evento de Unity que se invoca cuando el usuario hace clic con el ratón sobre el collider 2D/3D de este GameObject.
        - **Funcionalidad:** Si la roca está brillando (`shiny` es `true`), busca una instancia de `Combatjudge` en la escena y llama a su método `MoveToRock`, pasándose a sí misma como argumento.
    - `public RockBehavior[] getNeighbor(int al)`:
        - **Descripción:** Calcula y devuelve dos rocas vecinas a una distancia `al` (offset) de esta roca dentro de su `PlayZone`.
        - **Parámetros:**
            - `al`: Un entero que indica el "desplazamiento" para encontrar vecinos.
        - **Retorno:** Un array de `RockBehavior` que contiene las dos rocas vecinas.
    - `public void AddPlayer(PlayerToken token)`:
        - **Descripción:** Añade un `PlayerToken` al array `playersOn`, que rastrea qué jugadores están actualmente en esta roca. Redimensiona el array dinámicamente si es necesario.
    - `public void RemovePlayer(PlayerToken token)`:
        - **Descripción:** Elimina una instancia específica de `PlayerToken` del array `playersOn`. Maneja el caso en que el array esté vacío o el token no se encuentre.
    - `public bool manyOn()`:
        - **Descripción:** Comprueba si hay más de un `PlayerToken` sobre esta roca.
        - **Retorno:** `true` si hay más de un jugador, `false` en caso contrario.
    - `public int GetPlayersOn()`:
        - **Descripción:** Calcula un valor entero combinando los índices de los jugadores presentes en la roca, utilizando una potencia de 2 (similar a una máscara de bits).
        - **Retorno:** Un `int` que representa la combinación de jugadores en la roca.
        - **Fragmento de código:**
            ```csharp
            int many = 0;
            foreach (PlayerToken p in playersOn)
            {
                many += (int)Mathf.Pow(2,p.player.indexFigther);
            }
            return many;
            ```
    - `public int ManyPlayerOn()`:
        - **Descripción:** Devuelve el número exacto de `PlayerToken`s que hay actualmente sobre la roca.
        - **Retorno:** Un `int` con la cantidad de jugadores.

- **Lógica Clave:**
    - **Posicionamiento y Rotación Circular:** Al inicio, las rocas se distribuyen circularmente alrededor de su `PlayZone` padre utilizando `direction`, `angle`, `radius` y `RockScale` del `father`.
    - **Brillo Interactivo:** La roca brilla (`shiny`) solo bajo ciertas condiciones controladas por `Combatjudge`. Cuando está brillando y se hace clic sobre ella, se notifica al `Combatjudge` para que realice una acción (`MoveToRock`).
    - **Gestión de Jugadores:** El script mantiene una lista dinámica (`playersOn`) de los `PlayerToken` que ocupan la roca, permitiendo añadir y remover jugadores de manera eficiente (aunque con recreación de arrays, lo que podría ser mejorado para escenarios de muy alta frecuencia).
    - **Cálculo de Vecinos:** Utiliza aritmética modular para encontrar rocas vecinas en un arreglo circular, lo que es crucial para la lógica de movimiento o interacción en el tablero.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Este script requiere un `SpriteRenderer` en el mismo GameObject y otro en su primer hijo (`transform.GetChild(0)`) para el símbolo. No hay `[RequireComponent]` explícito, por lo que estas dependencias son implícitas por el código. También asume la presencia de un `Collider2D` o `Collider` para `OnMouseDown`.
- **Eventos (Entrada):**
    - Se suscribe al evento `OnMouseDown()` de Unity, que se activa cuando el usuario hace clic sobre el GameObject.
    - Consulta el estado (`SetMoments`) del `Combatjudge` a través de `Combatjudge.combatjudge.GetSetMoments()` para controlar el efecto de brillo.
- **Eventos (Salida):**
    - Cuando se hace clic y la roca está brillando, este script invoca directamente el método `MoveToRock(this)` en la primera instancia de `Combatjudge` encontrada en la escena. No emite eventos de forma desacoplada (ej. `UnityEvent` o `Action`).

---
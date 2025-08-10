# `FolowerText.cs`

## 1. Propósito General
Este script tiene como propósito principal actualizar dinámicamente el texto de un componente `TextMeshProUGUI` basándose en propiedades de un objeto `Figther` (luchador) que se encuentra en un GameObject padre. Su función es mostrar en la interfaz de usuario información como la "especie" o la "vida" del luchador asociado.

## 2. Componentes Clave

### `enum TypeFollow`
- **Descripción:** Esta enumeración define los tipos de datos del `Figther` que el script `FolowerText` puede seguir y mostrar. Actualmente, permite al diseñador elegir entre mostrar la "especie" (`species`) o el valor de "vida" (`live`) del luchador.

### `FolowerText` (Clase)
- **Descripción:** Hereda de `MonoBehaviour` y es el componente principal que se adjunta a un GameObject en la jerarquía de Unity, típicamente a un objeto de texto UI. Se encarga de buscar un `Figther` en sus padres, obtener una propiedad específica de este y actualizar el `TextMeshProUGUI` en consecuencia.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] TypeFollow typeFollow;`: Esta es una variable serializada que permite seleccionar en el Inspector de Unity qué tipo de información del `Figther` se debe mostrar (especie o vida). La elección aquí determinará el comportamiento del script.
    - Internamente, el script utiliza dos referencias privadas: `textMeshPro`, que almacena una referencia al componente `TextMeshProUGUI` en el mismo GameObject, y `player`, que guarda una referencia al componente `Figther` encontrado en los GameObjects padres.

- **Métodos Principales:**
    - `void Start()`: Este método se invoca una vez al inicio del ciclo de vida del script, después de que el GameObject es activado.
        - En `Start`, el script primero obtiene una referencia a su propio componente `TextMeshProUGUI` utilizando `GetComponent<TextMeshProUGUI>()`.
        - Luego, busca un componente `Figther` en los GameObjects ascendentes (padres) de su propia jerarquía mediante `GetComponentInParent<Figther>()`.
        - Si se encuentra un `Figther`, un bloque `switch` utiliza el valor de la variable `typeFollow` para determinar qué información inicial obtener del `Figther` (ya sea la especie o la vida) y actualiza el texto del `TextMeshProUGUI` con ese valor.

    - `void Update()`: Este método se llama una vez por cada frame.
        - `Update` contiene una lógica de actualización continua, pero solo para el caso donde `typeFollow` está configurado como `TypeFollow.live`. Si la configuración es "vida", el script actualiza constantemente el texto del `TextMeshProUGUI` con el valor actual de la vida del `Figther` (`player.GetPlayerLive().ToString()`). Esto asegura que el valor de la vida en la UI se mantenga sincronizado con los cambios en el `Figther` durante el juego.

- **Lógica Clave:**
    La lógica principal de este script reside en su capacidad para actuar como un "observador" de un `Figther` en la jerarquía. En el momento de su inicialización (`Start`), configura su texto inicial. Luego, en cada frame (`Update`), solo si está configurado para seguir la vida del jugador, la actualiza constantemente. Esto implica que para la "especie", el texto se establece una vez al inicio y no cambia a menos que el `Figther` cambie su especie *después* de que `Start` se haya ejecutado (lo cual no está cubierto por este script). Para la "vida", el script garantiza una visualización en tiempo real.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Este script requiere un componente `TextMeshProUGUI` en el mismo GameObject para poder manipular su texto.
    - Además, para funcionar correctamente, necesita que exista un componente `Figther` en alguno de los GameObjects padres en la jerarquía. Aunque no utiliza el atributo `[RequireComponent]`, la funcionalidad del script depende directamente de la presencia de estos componentes.

- **Eventos (Entrada):**
    - El script no se suscribe explícitamente a eventos personalizados (como `UnityEvent` o `Action`). Su funcionamiento se basa enteramente en los métodos de ciclo de vida de Unity (`Start` y `Update`).

- **Eventos (Salida):**
    - Este script no invoca ningún evento ni notifica a otros sistemas sobre cambios o acciones. Su rol es puramente de presentación de datos.
# `FolowerText.cs`

## 1. Propósito General
Este script se encarga de mostrar dinámicamente información textual de un componente `Player` en un elemento de UI `TextMeshProUGUI`. Su rol principal es vincular y actualizar el texto de la interfaz de usuario para reflejar atributos específicos del jugador, como su "especie" o su "vida" (probablemente puntos de salud o un contador similar).

## 2. Componentes Clave

### `TypeFollow`
- **Descripción:** `TypeFollow` es un `enum` que define las categorías de datos del `Player` que el script `FolowerText` puede monitorizar y mostrar. Ofrece dos opciones: `species` para referirse a la identidad de la "especie" del jugador, y `live` para representar su valor de "vida" o un atributo cuantitativo similar que puede cambiar durante el juego.
- **Variables Públicas / Serializadas:** N/A (es una enumeración).
- **Métodos Principales:** N/A (es una enumeración).
- **Lógica Clave:** Este `enum` proporciona una manera clara y legible de seleccionar, a través del Inspector de Unity, qué tipo de información del jugador se visualizará mediante este componente de texto.

### `FolowerText`
- **Descripción:** `FolowerText` es una clase que extiende `MonoBehaviour`, diseñada para actuar como un controlador de texto en la interfaz de usuario. Su propósito es obtener datos de un componente `Player` (ubicado en un GameObject padre) y usarlos para actualizar el texto de un `TextMeshProUGUI` en el mismo GameObject. Permite mostrar información estática como la "especie" o valores dinámicos como la "vida".
- **Variables Públicas / Serializadas:**
    - `[SerializeField] TypeFollow typeFollow`: Esta variable es crucial ya que permite a los diseñadores seleccionar en el Inspector de Unity qué atributo del `Player` debe seguir y mostrar este componente. Las opciones son `species` o `live`, determinando el comportamiento del script.
- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se invoca una única vez al inicio del componente.
        *   Dentro de `Start`, el script adquiere referencias al componente `TextMeshProUGUI` presente en el mismo GameObject y al componente `Player` buscando en la jerarquía de GameObjects padres.
        *   Si se encuentra un `Player`, el script establece el texto inicial del `TextMeshProUGUI`. Utiliza la variable `typeFollow` para decidir qué información mostrar: si es `species`, se establece la "especie" del jugador; si es `live`, se establece el valor de "vida" del jugador. Este valor inicial se lee una vez.
        *   ```csharp
            void Start()
            {
                textMeshPro = GetComponent<TextMeshProUGUI>();
                player = GetComponentInParent<Player>();
                if (player != null)
                {
                    switch (typeFollow)
                    {
                        case TypeFollow.species:
                            textMeshPro.text = player.GetSpecie().ToString();
                            break;
                        case TypeFollow.live:
                            textMeshPro.text = player.GetPlayerLive().ToString();
                            break;
                    }
                }
            }
            ```
    - `void Update()`: Este método del ciclo de vida de Unity se ejecuta en cada frame del juego.
        *   `Update` es responsable de la actualización continua del texto. Sin embargo, esta actualización solo ocurre si la variable `typeFollow` está configurada como `TypeFollow.live`. Esto asegura que los cambios en el valor de "vida" del jugador se reflejen en tiempo real en la interfaz de usuario, mientras que la "especie" (que se asume estática) no se actualiza repetidamente.
        *   ```csharp
            void Update()
            {
                if (typeFollow == TypeFollow.live)
                {
                    textMeshPro.text = player.GetPlayerLive().ToString();
                }
            }
            ```
- **Lógica Clave:**
    *   La funcionalidad principal reside en su capacidad para obtener datos de un componente `Player` que se espera que esté en un GameObject padre. Esto implica que el `FolowerText` está diseñado para ser un elemento de UI hijo de la entidad del jugador o de un contenedor que incluya al jugador.
    *   La distinción entre actualizar la "especie" solo en `Start` y la "vida" continuamente en `Update` (`if (typeFollow == TypeFollow.live)`) es fundamental. Esto optimiza el rendimiento al evitar actualizaciones innecesarias para datos estáticos y asegura la reactividad para datos dinámicos.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script depende funcionalmente de dos componentes clave. Requiere un componente `TextMeshProUGUI` en el mismo GameObject para poder manipular y mostrar texto. Adicionalmente, requiere la presencia de un componente `Player` en un GameObject padre para obtener los datos que debe mostrar. Aunque no se utiliza `[RequireComponent]` explícitamente, estos componentes son necesarios para su operación.
- **Eventos (Entrada):** `FolowerText` no se suscribe a eventos personalizados (`UnityEvent` o `Action`) definidos por otros scripts. Su comportamiento está impulsado directamente por los métodos de ciclo de vida de Unity (`Start`, `Update`).
- **Eventos (Salida):** Este script no emite ni invoca ningún tipo de evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas o componentes. Su función es puramente de lectura y visualización de datos.
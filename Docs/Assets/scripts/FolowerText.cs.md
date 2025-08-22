# `FolowerText.cs`

## 1. Propósito General
Este script tiene como objetivo principal actualizar dinámicamente el texto de un componente `TextMeshProUGUI` para mostrar información específica (especie, vida o nombre) de un `Figther` (luchador) asociado. Sirve como un componente de UI que sigue y refleja el estado de un personaje en el juego, asegurando que la información presentada al jugador esté siempre actualizada.

## 2. Componentes Clave

### `TypeFollow`
-   **Descripción:** Es un `enum` que define los tipos de datos que el script `FolowerText` puede rastrear y presentar en el componente de texto. Permite al desarrollador seleccionar en el Inspector de Unity qué propiedad de un `Figther` debe ser visualizada por el texto asociado.
-   **Valores:**
    -   `species`: Indica que el texto debe mostrar la especie del luchador.
    -   `live`: Indica que el texto debe mostrar los puntos de vida actuales del luchador.
    -   `name`: Indica que el texto debe mostrar el nombre del luchador.

### `FolowerText`
-   **Descripción:** Esta clase, que hereda de `MonoBehaviour`, es la encargada de establecer el enlace entre un componente de texto de UI (`TextMeshProUGUI`) y las propiedades de un objeto `Figther`. Se espera que el componente `Figther` se encuentre en un GameObject padre en la jerarquía. El script inicializa el texto con la información deseada y lo mantiene actualizado en el caso de la vida del personaje.
-   **Variables Públicas / Serializadas:**
    -   `[SerializeField] TypeFollow typeFollow`: Esta variable es crucial, ya que determina qué tipo de información del `Figther` será mostrada por el texto. Al ser `[SerializeField]`, se puede configurar directamente en el Inspector de Unity, permitiendo elegir entre `species`, `live`, o `name`.
-   **Métodos Principales:**
    -   `void Start()`: Este método del ciclo de vida de Unity se ejecuta una vez al inicio, después de que el GameObject se ha activado.
        *   Obtiene una referencia al componente `TextMeshProUGUI` que debe estar adjunto al mismo GameObject donde reside este script.
        *   Busca y obtiene una referencia al componente `Figther` en uno de los GameObjects padres en la jerarquía.
        *   Una vez encontrado el `Figther`, utiliza la variable `typeFollow` para determinar qué propiedad (especie, vida o nombre) debe obtener de ese `Figther` y asignar su valor como texto inicial al `TextMeshProUGUI`.
        *   **Fragmento de código relevante:**
            ```csharp
            player = GetComponentInParent<Figther>();
            if(player != null)
            {
                switch (typeFollow)
                {
                    case TypeFollow.species:
                        text = player.GetSpecie().ToString();
                        break;
                    // ... otros casos
                }
                textMeshPro.text = text;
            }
            ```
    -   `void Update()`: Este método del ciclo de vida de Unity se ejecuta una vez por cada frame.
        *   Su función es mantener actualizada la información que muestra el texto. Sin embargo, solo realiza actualizaciones continuas si la variable `typeFollow` está configurada específicamente para seguir la vida (`TypeFollow.live`) del luchador.
        *   Si `typeFollow` es `live`, el texto del `TextMeshProUGUI` se actualiza con el valor actual de los puntos de vida del `Figther` en cada frame, garantizando que el UI refleje cualquier cambio en la salud del personaje en tiempo real.
        *   **Fragmento de código relevante:**
            ```csharp
            if (typeFollow == TypeFollow.live)
            {
                textMeshPro.text = player.GetPlayerLive().ToString();
            }
            ```
-   **Lógica Clave:** La lógica central del script se centra en la inicialización del texto en `Start` basándose en una configuración definida (`typeFollow`) y la búsqueda de un componente `Figther` en la jerarquía padre. Además, implementa una lógica de actualización continua en `Update` exclusivamente para el seguimiento de la vida, lo que es eficiente ya que otros datos como el nombre o la especie no cambian durante el juego.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Para que este script funcione correctamente, el GameObject al que se adjunta debe tener un componente `TextMeshProUGUI`. Además, el script espera encontrar un componente `Figther` en alguno de sus GameObjects padres en la jerarquía de la escena, estableciendo así una dependencia estructural.
-   **Eventos (Entrada):** Este script no se suscribe explícitamente a eventos (`UnityEvent`, `Action`, etc.) definidos por otros sistemas o clases. Su comportamiento está directamente impulsado por los métodos de ciclo de vida de Unity (`Start`, `Update`).
-   **Eventos (Salida):** Este script no invoca ni publica ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios en el texto o en el estado del `Figther`. Su rol es puramente observador y de representación visual.
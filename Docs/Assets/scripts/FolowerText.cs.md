# `FolowerText.cs`

## 1. Propósito General
Este script gestiona la visualización dinámica de texto en un componente `TextMeshProUGUI`. Su rol principal es mostrar información específica (la especie o la vida/salud) de un personaje `Figther` (luchador) asociado, actualizando esta información según sea necesario durante el juego.

## 2. Componentes Clave

### `TypeFollow`
- **Descripción:** Es un `enum` simple que define los tipos de datos que el script `FolowerText` puede rastrear y mostrar. Permite seleccionar si el texto debe seguir la "especie" o la "vida" del `Figther` asociado.

### `FolowerText`
- **Descripción:** Esta clase es un `MonoBehaviour` que se encarga de inicializar y actualizar el texto de un componente `TextMeshProUGUI` al que está adjunto. El texto mostrado depende de la información de un componente `Figther` que debe encontrarse en un GameObject padre, y del tipo de seguimiento (`typeFollow`) configurado.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] TypeFollow typeFollow;`: Esta variable serializada determina qué tipo de información del `Figther` será mostrada por el `TextMeshProUGUI`. Se puede configurar en el Inspector de Unity a `species` (especie) o `live` (vida/salud).

- **Métodos Principales:**
    - `void Start()`:
        - **Descripción:** Es un método del ciclo de vida de Unity que se llama una vez al inicio, antes de la primera actualización.
        - **Funcionalidad:**
            1.  Obtiene una referencia al componente `TextMeshProUGUI` adjunto al mismo GameObject.
            2.  Busca un componente `Figther` en los GameObjects padres.
            3.  Si se encuentra un `Figther`, inicializa el texto del `TextMeshProUGUI` según el valor de `typeFollow`:
                -   Si `typeFollow` es `TypeFollow.species`, el texto se establece con el valor devuelto por `player.GetSpecie().ToString()`.
                -   Si `typeFollow` es `TypeFollow.live`, el texto se establece con el valor devuelto por `player.GetPlayerLive().ToString()`.
        - **Fragmento de Código:**
            ```csharp
            void Start()
            {
                textMeshPro = GetComponent<TextMeshProUGUI>();
                player = GetComponentInParent<Figther>();
                // ... lógica para establecer el texto inicial
            }
            ```

    - `void Update()`:
        - **Descripción:** Es un método del ciclo de vida de Unity que se llama una vez por cada frame.
        - **Funcionalidad:**
            -   Si el `typeFollow` está configurado en `TypeFollow.live`, este método actualiza continuamente el texto del `TextMeshProUGUI` para reflejar el valor actual de la vida del `Figther` (`player.GetPlayerLive().ToString()`). Esto asegura que la barra de vida o el contador se mantenga actualizado.
            -   Si `typeFollow` es `TypeFollow.species`, este método no realiza ninguna acción adicional, ya que la especie no cambia durante el juego.
        - **Fragmento de Código:**
            ```csharp
            void Update()
            {
                if (typeFollow == TypeFollow.live)
                {
                    textMeshPro.text = player.GetPlayerLive().ToString();
                }
            }
            ```

- **Lógica Clave:**
    La lógica central de `FolowerText` radica en su capacidad para actuar como un "observador" de los datos de un `Figther`. Durante el `Start`, configura el estado inicial del texto. Luego, en `Update`, solo realiza actualizaciones continuas si está configurado para seguir la "vida" del `Figther`, lo que es crucial para mostrar valores que cambian con frecuencia (como la salud o puntos de vida). La selección entre "especie" y "vida" se gestiona mediante el enum `TypeFollow` y la variable `typeFollow` serializada.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    -   Este script requiere un componente `TextMeshProUGUI` en el mismo GameObject para poder manipular su texto.
    -   Implícitamente, requiere que exista un componente `Figther` en uno de los GameObjects padres para poder obtener la información de la especie o la vida/salud.

- **Eventos (Entrada):**
    -   Este script no se suscribe explícitamente a ningún evento (`UnityEvent`, `Action`, etc.) de otros sistemas. Su activación es impulsada por los métodos del ciclo de vida de Unity (`Start`, `Update`).

- **Eventos (Salida):**
    -   Este script no invoca ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas. Su función es puramente de visualización.
# `FolowerText.cs`

## 1. Propósito General
Este script gestiona la visualización de texto dinámico en elementos de interfaz de usuario (`TextMeshProUGUI`). Su rol principal es mostrar la "especie" o la "vida" de un personaje (`Figther`) al que está asociado, actualizando la información de vida en tiempo real durante el juego.

## 2. Componentes Clave

### `TypeFollow`
Este es un tipo enumerado (`enum`) que define las opciones de datos que el script `FolowerText` puede seguir y mostrar.
*   `species`: Indica que el texto debe mostrar la especie del personaje.
*   `live`: Indica que el texto debe mostrar el valor de vida actual del personaje.

### `FolowerText`
- **Descripción:** Esta clase es un componente de Unity (`MonoBehaviour`) diseñado para ser adjuntado a un GameObject que contiene un componente `TextMeshProUGUI`. Su función es enlazar el contenido de texto de ese `TextMeshProUGUI` con propiedades específicas de un personaje (`Figther`) que se encuentre en un GameObject padre.
- **Variables Públicas / Serializadas:**
    - `[SerializeField] TypeFollow typeFollow;`: Una variable serializada que permite al diseñador seleccionar desde el Inspector de Unity si el texto debe seguir la especie (`species`) o la vida (`live`) del personaje asociado.
- **Métodos Principales:**
    - `void Start()`: Este método se ejecuta una vez al inicio del ciclo de vida del script.
        -   Obtiene una referencia al componente `TextMeshProUGUI` en el mismo GameObject.
        -   Busca una referencia al componente `Figther` en los GameObjects padres del GameObject actual. Esto es crucial para que el script pueda acceder a los datos del personaje.
        -   Si se encuentra un `Figther`, inicializa el texto:
            -   Si `typeFollow` es `species`, el texto se establece al valor de la especie del `Figther` (`player.GetSpecie().ToString()`).
            -   Si `typeFollow` es `live`, el texto se establece al valor de vida inicial del `Figther` (`player.GetPlayerLive().ToString()`).
        ```csharp
        void Start()
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
            player = GetComponentInParent<Figther>();
            if(player != null)
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
    - `void Update()`: Este método se ejecuta una vez por cada frame del juego.
        -   Contiene la lógica para la actualización continua del texto.
        -   Si la variable `typeFollow` está configurada como `live`, el texto del `TextMeshProUGUI` se actualiza constantemente para reflejar el valor actual de la vida del personaje (`player.GetPlayerLive().ToString()`). Esta actualización ocurre cada fotograma, asegurando que la barra de vida o el contador de vida se mantenga al día.
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
    La lógica central del script reside en cómo selecciona y actualiza la información a mostrar. Al iniciar, el script determina qué propiedad del `Figther` (especie o vida) debe seguir basándose en la configuración `typeFollow` definida en el Inspector. Mientras que la "especie" se establece una única vez al inicio, el valor de "vida" se actualiza continuamente en el método `Update`, garantizando que el UI refleje los cambios en la salud del personaje en tiempo real.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    -   Implícitamente, este script requiere un componente `TextMeshProUGUI` en el mismo GameObject donde está adjunto, ya que intenta obtenerlo con `GetComponent<TextMeshProUGUI>()`.
    -   También requiere que haya un componente `Figther` en uno de los GameObjects padres del GameObject actual para poder acceder a los datos del personaje.
- **Eventos (Entrada):**
    -   Este script no se suscribe directamente a eventos de Unity ni a eventos personalizados (`UnityEvent` o `Action`). Su funcionamiento se basa en los métodos del ciclo de vida de Unity (`Start`, `Update`).
- **Eventos (Salida):**
    -   Este script no invoca ningún evento para notificar a otros sistemas. Su única función es la visualización de datos.
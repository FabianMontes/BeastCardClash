# `MeshAnimation.cs`

## 1. Propósito General
Este script gestiona las animaciones y la apariencia visual (skins) de un personaje o un objeto en el juego. Actúa como un controlador central para interactuar con los componentes `Animator` y `Renderer`, permitiendo cambiar el estado de las animaciones y la textura principal del modelo.

## 2. Componentes Clave

### `MeshAnimation`
- **Descripción:** La clase `MeshAnimation` es un `MonoBehaviour` que se adjunta a un GameObject en la escena de Unity. Su función principal es facilitar la manipulación de los parámetros de un `Animator` asociado y la textura de un `Renderer` hijo, permitiendo al objeto cambiar dinámicamente sus animaciones y su "skin" visual.

- **Variables Públicas / Serializadas:**
    - `Skins (List<Sprite>)`: Una lista de objetos `Sprite` que se puede configurar directamente desde el Inspector de Unity. Cada `Sprite` en esta lista representa una posible "skin" o apariencia visual para el modelo, ya que su textura se usará para el material del renderizador.

- **Métodos Principales:**
    - `void Awake()`:
        Este método es parte del ciclo de vida de Unity y se ejecuta cuando la instancia del script se está cargando. Su propósito es inicializar las referencias internas:
        - Obtiene una referencia al componente `Animator` que se encuentra en el mismo GameObject al que está adjunto este script.
        - Obtiene una referencia al primer componente `Renderer` que encuentre entre los hijos del GameObject. Esto es crucial para poder modificar la textura del modelo.

    - `public void UpdateAnimation(string variableName, string value)`:
        Este método permite actualizar de forma programática un parámetro dentro del controlador del `Animator`. Recibe el nombre del parámetro (`variableName`) y el valor deseado como una cadena de texto (`value`).
        La lógica interna detecta automáticamente el tipo de parámetro (`Bool`, `Float`, `Int`, `Trigger`) del `Animator` y luego intenta convertir el `value` de cadena al tipo correcto antes de asignarlo al parámetro correspondiente en el `Animator`. Si el parámetro no se encuentra en el `Animator`, se registra una advertencia.

        ```csharp
        public void UpdateAnimation(string variableName, string value)
        {
            // ... (comprobaciones de nulo)
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == variableName)
                {
                    switch (param.type)
                    {
                        // ... (manejo de tipos Bool, Float, Int, Trigger)
                    }
                    return;
                }
            }
            Debug.LogWarning($"No se encontró el parámetro '{variableName}' en el Animator.");
        }
        ```

    - `public void SetSkin(int index)`:
        Este método se utiliza para cambiar la apariencia visual del modelo. Toma un `index` como entrada, que corresponde a la posición de un `Sprite` dentro de la lista `Skins`. El método recupera la textura de ese `Sprite` y la asigna como la `mainTexture` del material del `objRenderer`. Incluye comprobaciones para asegurarse de que el índice sea válido y que las referencias no sean nulas, mostrando advertencias si hay problemas.

        ```csharp
        public void SetSkin(int index)
        {
            if (Skins == null || index < 0 || index >= Skins.Count)
            {
                Debug.LogWarning("Índice de skin inválido.");
                return;
            }

            if (objRenderer != null && Skins[index] != null)
            {
                Texture2D texture = Skins[index].texture;
                objRenderer.material.mainTexture = texture;
            }
        }
        ```

- **Lógica Clave:**
    La lógica principal de `UpdateAnimation` reside en su capacidad de inferir el tipo de parámetro del `Animator` a partir de su definición y luego intentar convertir un valor de cadena al tipo apropiado para establecerlo. Esto proporciona una interfaz flexible para controlar animaciones. Por otro lado, `SetSkin` gestiona el cambio visual del objeto extrayendo la textura de un `Sprite` de su lista serializada y aplicándola al material del renderizador, lo que permite alternar fácilmente entre diferentes apariencias.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Este script asume la presencia de un componente `Animator` en el mismo GameObject (obtenido a través de `GetComponent<Animator>()`).
    - También requiere que un componente `Renderer` (por ejemplo, `MeshRenderer` o `SpriteRenderer`) esté presente en uno de los GameObjects hijos para poder cambiar la textura.
    - No utiliza el atributo `[RequireComponent]`, por lo que estas dependencias no son forzadas por Unity en el editor, sino que son implícitas por el código.

- **Eventos (Entrada):**
    - Este script no se suscribe explícitamente a eventos de interfaz de usuario de Unity (como `Button.onClick`) ni a eventos personalizados (`UnityEvent`, `Action`). Sus métodos públicos están diseñados para ser invocados directamente por otros scripts.

- **Eventos (Salida):**
    - Este script no invoca ningún evento (`UnityEvent` o `Action`) para notificar a otros sistemas sobre cambios o estados. Toda su interacción es a través de sus métodos públicos.
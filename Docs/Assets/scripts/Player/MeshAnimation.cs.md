# `MeshAnimation.cs`

## 1. Propósito General
Este script gestiona los aspectos visuales y animados de un personaje o entidad en el juego. Su rol principal es controlar el componente `Animator` para disparar animaciones y actualizar la "skin" (textura principal) de un modelo 3D, permitiendo una fácil personalización y control visual desde otras partes del sistema.

## 2. Componentes Clave

### `MeshAnimation`
-   **Descripción:** `MeshAnimation` es un componente de Unity que hereda de `MonoBehaviour`, lo que significa que puede adjuntarse a un GameObject en la escena. Es responsable de inicializar y gestionar las referencias a los componentes `Animator` y `Renderer` asociados al objeto. Proporciona una interfaz para manipular las animaciones y la apariencia de la "skin" del personaje.
-   **Variables Públicas / Serializadas:**
    -   `Skins`: Una lista de objetos `Sprite` que se puede configurar directamente desde el Inspector de Unity. Cada `Sprite` en esta lista representa una "skin" diferente que el personaje puede adoptar. La textura de estos sprites se utilizará para cambiar la apariencia del modelo 3D.
        ```csharp
        [SerializeField] private List<Sprite> Skins;
        ```
-   **Métodos Principales:**
    -   `void Awake()`: Este método del ciclo de vida de Unity se ejecuta una vez cuando el script se carga o se habilita. Su función es inicializar las referencias a los componentes `Animator` (buscándolo en el mismo GameObject) y `Renderer` (buscándolo en los hijos del GameObject). Estas referencias son cruciales para que el script pueda controlar las animaciones y la skin.
        ```csharp
        private void Awake()
        {
            animator = GetComponent<Animator>();
            objRenderer = GetComponentInChildren<Renderer>();
        }
        ```
    -   `void Update()`: Este método de Unity se llama en cada frame del juego. Su propósito principal es mantener la "skin" del personaje sincronizada con el estado global del juego. Consulta el valor de `GameState.singleton.skin` y llama al método `SetSkin()` para aplicar la textura correspondiente en cada ciclo.
        ```csharp
        void Update()
        {
            SetSkin(GameState.singleton.skin);
        }
        ```
    -   `public void UpdateAnimation(string variableName, string value)`: Este método permite a otros scripts controlar los parámetros del componente `Animator`. Recibe dos parámetros de tipo `string`: `variableName`, que es el nombre del parámetro del Animator que se desea modificar, y `value`, que es el nuevo valor para ese parámetro. El método es inteligente y detecta automáticamente el tipo del parámetro (booleano, flotante, entero o trigger) para intentar convertir y asignar el valor correctamente. Si el parámetro no se encuentra, se registra una advertencia en la consola.
        ```csharp
        public void UpdateAnimation(string variableName, string value)
        {
            // ... lógica para iterar parámetros y asignar valores ...
        }
        ```
    -   `public void SetSkin(int index)`: Este método público se utiliza para cambiar la apariencia visual del modelo 3D del personaje. Recibe un `int` como `index`, que corresponde a la posición de la `Sprite` deseada dentro de la lista `Skins`. El método valida que el índice sea válido y, si lo es, toma la `Texture2D` del `Sprite` seleccionado y la asigna a la propiedad `mainTexture` del material del `objRenderer`, cambiando así la "skin" del modelo.
        ```csharp
        public void SetSkin(int index)
        {
            // ... lógica para cambiar la textura del material ...
        }
        ```
-   **Lógica Clave:**
    La lógica principal de `UpdateAnimation` radica en su capacidad para interpretar el tipo de un parámetro del `Animator` basado en su definición en el controlador del Animator. Esto permite una interfaz genérica (`string` para nombre y valor) para interactuar con parámetros de diferentes tipos (booleanos, enteros, flotantes, triggers) sin necesidad de sobrecargas de métodos. La detección de tipo y el parseo del valor aseguran que el `Animator` reciba el dato en el formato esperado.
    La integración con `GameState.singleton.skin` en el método `Update` es fundamental. Implica que la skin del personaje siempre reflejará el estado actual de esa variable global, garantizando que los cambios de skin definidos en `GameState` se apliquen visualmente de manera continua.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:**
    Este script asume y requiere la presencia de un componente `Animator` en el mismo GameObject donde está adjunto `MeshAnimation`. Adicionalmente, requiere un componente `Renderer` (como `MeshRenderer` o `SkinnedMeshRenderer`) en un GameObject hijo para poder manipular su material y cambiar la "skin". Si estos componentes no están presentes, las funcionalidades principales del script (animación y cambio de skin) no operarán y se producirán errores.
-   **Eventos (Entrada):**
    El script no se suscribe a eventos explícitos de Unity (como `UnityEvent` o `Action`). Sin embargo, su método `Update()` lee constantemente el valor de `GameState.singleton.skin` para determinar qué skin debe aplicar, lo que implica una dependencia activa del estado global del juego.
-   **Eventos (Salida):**
    Este script no emite ni invoca ningún tipo de evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios en su estado o la finalización de alguna acción. Sus métodos públicos (`UpdateAnimation`, `SetSkin`) están diseñados para ser llamados por otros sistemas que necesitan modificar la animación o la skin del personaje.
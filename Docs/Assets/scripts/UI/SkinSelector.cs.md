# `SkinSelector.cs`

## 1. Propósito General
Este script `SkinSelector.cs` es un componente de `MonoBehaviour` que gestiona la interacción del usuario con un objeto en la escena para seleccionar una "skin" (apariencia visual) específica. Su rol principal es detectar eventos del ratón (clics y sobrevuelos) y comunicar la selección al sistema global `GameState`, además de proporcionar retroalimentación visual mediante un efecto de contorno.

## 2. Componentes Clave

### `SkinSelector`
- **Descripción:** La clase `SkinSelector` es un script de Unity (`MonoBehaviour`) que se adjunta a un GameObject en la escena. Se encarga de la lógica de selección de una apariencia específica (skin) y de la retroalimentación visual al interactuar con el mouse. Cuando el usuario hace clic en el objeto al que está adjunto este script, se le indica al sistema `GameState` qué skin ha sido seleccionada.

- **Variables Públicas / Serializadas:**
    - `skinIndex`: Una variable entera privada (`int`) que se serializa y es visible en el Inspector de Unity gracias a `[SerializeField]`. Este índice es el identificador único de la skin que representa este `SkinSelector` particular. Cuando se selecciona el objeto, este `skinIndex` se pasa al sistema `GameState` para registrar la skin elegida.
    - `outline`: Una referencia privada de tipo `Outline` (del namespace `OutlineFx`). Esta variable almacena el componente de contorno que se encuentra en uno de los hijos del GameObject actual. Se utiliza para habilitar o deshabilitar un efecto visual (un "outline" o contorno) cuando el ratón interactúa con el objeto, sirviendo como feedback visual al jugador.

- **Métodos Principales:**
    - `void Start()`: Este es un método del ciclo de vida de Unity que se llama una vez al inicio, antes de la primera actualización del frame. Su función es inicializar el script:
        - Busca y asigna el componente `Outline` presente en uno de los GameObjects hijos.
        - Deshabilita inmediatamente el `outline` para asegurar que no sea visible por defecto.
        - Obtiene el componente `Animator` adjunto al mismo GameObject y establece el parámetro booleano "isFigthing" a `true`. Esto podría iniciar una animación predeterminada o un estado relacionado con el combate.
        ```csharp
        private void Start()
        {
            outline = GetComponentInChildren<Outline>();
            outline.enabled = false;
            GetComponent<Animator>().SetBool("isFigthing", true);
        }
        ```
    - `void OnMouseDown()`: Este método es un callback de evento de Unity que se invoca cuando el usuario presiona el botón del mouse mientras el cursor está sobre el collider del GameObject. Su propósito es notificar al sistema del juego sobre la selección de la skin:
        - Accede a la instancia singleton del `GameState`.
        - Llama al método `SetSkin` de `GameState`, pasándole el `skinIndex` de este `SkinSelector`. Esto registra la skin como seleccionada en el estado global del juego.
        ```csharp
        void OnMouseDown()
        {
            GameState.singleton.SetSkin(skinIndex);
        }
        ```
    - `void OnMouseEnter()`: Este método de evento de Unity se llama cuando el puntero del mouse entra en el área del collider del GameObject. Cuando esto ocurre, el script activa el `outline` visual, lo que suele indicar al jugador que el objeto está bajo el cursor y es interactuable.
        ```csharp
        private void OnMouseEnter()
        {
            outline.enabled = true;
        }
        ```
    - `void OnMouseExit()`: Este método de evento de Unity se invoca cuando el puntero del mouse sale del área del collider del GameObject. Como contraparte de `OnMouseEnter`, deshabilita el `outline` visual, retirando la retroalimentación una vez que el cursor ya no está sobre el objeto.
        ```csharp
        private void OnMouseExit()
        {
            outline.enabled = false;
        }
        ```

- **Lógica Clave:** La lógica central de `SkinSelector` se basa en la interacción del ratón. El script monitorea los eventos de entrada del ratón (clic, entrada y salida del puntero) sobre el collider del GameObject. Al hacer clic, transmite un `skinIndex` específico al sistema `GameState` para registrar la selección de una skin. Durante los eventos de "hover" (cuando el mouse está sobre el objeto), alterna el estado del componente `Outline` para proporcionar una señal visual al jugador. El método `Start` asegura una configuración inicial adecuada, como la desactivación del contorno por defecto y el ajuste de un parámetro del `Animator`.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Aunque no se utiliza explícitamente el atributo `[RequireComponent]`, para que este script funcione correctamente, el GameObject al que se adjunta, o uno de sus hijos, debe tener los siguientes componentes:
    - Un componente `Outline` (del sistema `OutlineFx`) en uno de sus GameObjects hijos.
    - Un componente `Animator` en el mismo GameObject.
    - Un componente `Collider` (como `BoxCollider`, `SphereCollider`, etc.) en el mismo GameObject, para que los métodos `OnMouseDown`, `OnMouseEnter` y `OnMouseExit` puedan detectar la interacción del ratón.

- **Eventos (Entrada):** Este script se suscribe y responde a los siguientes eventos de entrada de Unity, que son activados por la interacción del ratón con el collider del GameObject:
    - `OnMouseDown()`: Activado al hacer clic con el ratón sobre el objeto.
    - `OnMouseEnter()`: Activado cuando el cursor del ratón entra en el área del collider del objeto.
    - `OnMouseExit()`: Activado cuando el cursor del ratón sale del área del collider del objeto.

- **Eventos (Salida):** Este script invoca una acción externa al llamar directamente a `GameState.singleton.SetSkin(skinIndex)`. Esta llamada actúa como un evento de "salida", notificando al sistema global `GameState` qué skin ha sido seleccionada por el jugador.
# `Target.cs`

## 1. Propósito General
Este script `Target.cs` es un componente `MonoBehaviour` que gestiona el movimiento de un objeto en la escena. Actúa como un "cursor" o "blanco" que el jugador puede controlar, ofreciendo dos modos de interacción distintos: movimiento direccional continuo mediante teclado o posicionamiento instantáneo mediante clic del ratón.

## 2. Componentes Clave

### `Target`
- **Descripción:** La clase `Target` es un script de Unity que hereda de `MonoBehaviour`, lo que le permite adjuntarse a un objeto de juego en la escena. Su función principal es controlar la posición de dicho objeto basándose en la entrada del jugador, proporcionando dos esquemas de control configurables para la navegación del "blanco" en el entorno 3D.

- **Variables Públicas / Serializadas:**
    - `playerCamera` (`Transform`): Una referencia al componente `Transform` de la cámara del jugador. Esta variable es fundamental porque el movimiento direccional (cuando `useArrows` es `true`) se calcula relativo a la orientación de esta cámara (su "adelante" y "derecha"). También se utiliza para generar rayos desde la cámara en el modo de clic del ratón. Se marca con `[SerializeField]` para poder asignarla desde el Inspector de Unity.
    - `speed` (`float`): Define la velocidad a la que el objeto se mueve cuando se utiliza el modo de control por teclado/flechas. Este valor es un multiplicador que ajusta la rapidez del desplazamiento. Se marca con `[SerializeField]` para permitir su ajuste en el Inspector de Unity.
    - `useArrows` (`bool`): Un flag booleano que determina el esquema de control activo. Si es `true`, el objeto se moverá con las teclas de flecha o WASD. Si es `false`, el objeto se moverá a la posición del mundo donde el jugador haga clic con el ratón. Se marca con `[SerializeField]` para su configuración en el Inspector.

- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se llama una vez al inicio, antes de la primera actualización de un frame. En la implementación actual, está vacío, lo que indica que no se requiere ninguna inicialización específica para este script al momento de su creación.
    - `void Update()`: Este método del ciclo de vida de Unity se invoca una vez por cada frame del juego. Su lógica principal se centra en detectar la entrada del usuario y actualizar la posición del objeto `Target` según el modo de control (`useArrows`) configurado:
        - Si `useArrows` es `true`, calcula la dirección de movimiento a partir de las teclas presionadas y la aplica a la posición del objeto, escalada por `speed` y `Time.deltaTime` para asegurar un movimiento suave e independiente de la tasa de frames.
        - Si `useArrows` es `false`, monitorea el clic izquierdo del ratón. Al detectar un clic, lanza un rayo desde la cámara principal hacia la posición del cursor. Si el rayo impacta con un objeto en el mundo (que tenga un `Collider`), la posición del `Target` se ajusta instantáneamente al punto de impacto.
    - `Vector3 GetInputDirection()`: Este método auxiliar privado es responsable de procesar la entrada del teclado (teclas de flecha y WASD) y convertirla en un vector de dirección 3D. Se calcula la dirección sumando los vectores `playerCamera.forward` y `playerCamera.right` según las teclas presionadas. Es importante destacar que el vector resultante se normaliza (`dir.normalized`) para mantener una velocidad constante sin importar si se presionan múltiples teclas, y su componente `y` se fija a cero (`dir.y = 0`) para restringir el movimiento al plano horizontal, evitando desplazamientos verticales.

- **Lógica Clave:**
    La lógica central del script se encuentra en el método `Update`, que actúa como un selector de modo de control impulsado por la variable `useArrows`.
    - **Modo de Teclado/Flechas:** Cuando `useArrows` es `true`, el script llama continuamente a `GetInputDirection()` para obtener un vector de movimiento. Este vector se calcula dinámicamente en relación con la orientación de la `playerCamera` y se normaliza para asegurar que la velocidad sea consistente en todas las direcciones. La posición del objeto se actualiza incrementalmente cada frame, utilizando `Time.deltaTime` para suavizar el movimiento y hacerlo independiente de la velocidad de fotogramas.
        ```csharp
        if (useArrows)
        {
            transform.position += GetInputDirection() * speed * Time.deltaTime;
        }
        ```
    - **Modo de Ratón (Click):** Cuando `useArrows` es `false`, el script espera un clic izquierdo del ratón (`Input.GetMouseButtonDown(0)`). Una vez detectado el clic, se ejecuta un `Physics.Raycast` desde la cámara principal (`Camera.main`) hacia la posición del puntero del ratón en el mundo 3D. Si este rayo colisiona con algún `Collider` en la escena, la posición del objeto `Target` se teletransporta instantáneamente al punto exacto de la colisión (`hit.point`), permitiendo un control de "apuntar y hacer clic".
        ```csharp
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    transform.position = hit.point;
                }
            }
        }
        ```

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, para su correcto funcionamiento, se espera que:
    - Exista una cámara en la escena cuya `Transform` se asigne a la variable `playerCamera`.
    - Si se usa el modo de ratón (`useArrows = false`), los objetos en la escena con los que se espera interactuar mediante clic deben tener componentes `Collider` para que el `Physics.Raycast` pueda detectarlos.
    - `Camera.main` debe estar disponible (es decir, una cámara en la escena debe tener la etiqueta "MainCamera") para que el raycasting del ratón funcione.

- **Eventos (Entrada):** Este script no se suscribe a eventos de Unity ni a delegados de C# de otros componentes. En su lugar, obtiene la entrada del usuario directamente a través del sondeo de `Input.GetKey()` para las teclas del teclado y `Input.GetMouseButtonDown()` para los clics del ratón.

- **Eventos (Salida):** El script `Target.cs` no invoca ningún `UnityEvent` ni `Action` ni notifica a otros sistemas. Su única función es modificar su propia `transform.position` basándose en la entrada.
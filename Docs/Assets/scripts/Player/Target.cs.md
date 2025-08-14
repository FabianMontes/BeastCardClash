# `Target.cs`

## 1. Propósito General
Este script `Target` es un componente de `MonoBehaviour` que controla el movimiento de un objeto en el entorno 3D del juego. Su función principal es permitir al jugador mover el objeto utilizando dos esquemas de control distintos: ya sea mediante el teclado (WASD o flechas) relativo a la cámara, o teletransportándolo instantáneamente a la posición de un clic del ratón en el mundo.

## 2. Componentes Clave

### `Target`
- **Descripción:** La clase `Target` es un script que se adjunta a un `GameObject` en Unity y le otorga la capacidad de ser controlado por el usuario. Implementa lógica para procesar la entrada del jugador, ya sea de teclado o ratón, y traduce esa entrada en movimientos para el objeto.
- **Variables Públicas / Serializadas:**
    Este script expone varias variables serializadas que permiten configurar su comportamiento directamente desde el Inspector de Unity:
    *   `[SerializeField] Transform playerCamera;`: Una referencia al `Transform` de la cámara del jugador. Esta es crucial para el modo de movimiento por teclado, ya que permite que el movimiento del objeto sea relativo a la orientación actual de la cámara (por ejemplo, "adelante" para el objeto significará "adelante" desde la perspectiva de la cámara). Se evita el nombre "Camera" para prevenir conflictos.
    *   `[SerializeField] float speed = 10f;`: Un valor flotante que determina la velocidad de movimiento del objeto cuando se utiliza el modo de control por teclado. Un valor más alto resultará en un movimiento más rápido.
    *   `[SerializeField] bool useArrows = true;`: Una bandera booleana que define el modo de control activo. Si es `true`, el objeto se moverá mediante las entradas del teclado (flechas o WASD). Si se establece en `false`, el objeto se moverá al punto donde el jugador haga clic con el ratón en el entorno.

- **Métodos Principales:**
    *   `void Start()`: Este método es parte del ciclo de vida de Unity y se invoca una vez al inicio del script, antes de la primera actualización de frame. Actualmente, no contiene ninguna lógica, lo que indica que no hay inicializaciones específicas o configuraciones únicas requeridas al inicio para este componente.

    *   `void Update()`: Este es el método central que se ejecuta en cada frame del juego y contiene la lógica principal para el movimiento del objeto. Decide qué tipo de movimiento aplicar basándose en el valor de la variable `useArrows`.
        ```csharp
        void Update()
        {
            if (useArrows)
            {
                // Lógica de movimiento por teclado/WASD
            }
            else
            {
                // Lógica de movimiento por clic de ratón
            }
        }
        ```
        Si `useArrows` es `true`, calcula la dirección de entrada del teclado utilizando el método `GetInputDirection()` y actualiza la posición del `Transform` del objeto sumándole este vector de dirección, multiplicado por la `speed` y `Time.deltaTime` para asegurar un movimiento suave e independiente de la tasa de frames.
        Si `useArrows` es `false`, el script espera un clic izquierdo del ratón (`Input.GetMouseButtonDown(0)`). Al detectar un clic, lanza un rayo (`Ray`) desde la posición del cursor en la pantalla hasta el mundo 3D (`Camera.main.ScreenPointToRay`). Si este rayo colisiona con algún `Collider` en la escena (`Physics.Raycast`), el objeto `Target` se teletransporta instantáneamente a la posición del punto de impacto de esa colisión.

    *   `Vector3 GetInputDirection()`: Este es un método auxiliar privado que se encarga de procesar las entradas del teclado para determinar la dirección de movimiento deseada. Devuelve un vector `Vector3` que representa la dirección.
        ```csharp
        Vector3 GetInputDirection()
        {
            Vector3 dir = Vector3.zero;
            // Comprobaciones de teclas (UpArrow/W, LeftArrow/A, DownArrow/S, RightArrow/D)
            // ...
            dir.y = 0; // Se fija en horizontal
            return dir.normalized; // Se normaliza
        }
        ```
        Inicializa un vector de dirección a cero y luego lo modifica sumando vectores basados en las teclas de flecha o WASD presionadas. Estas direcciones se calculan relativas a la dirección `forward` y `right` de la `playerCamera`. Un aspecto importante de este método es que fuerza el componente `y` del vector de dirección a cero, lo que garantiza que el movimiento siempre se produzca en el plano horizontal (XY, asumiendo Y es arriba) y evita el movimiento vertical. Finalmente, normaliza el vector de dirección para asegurar que la magnitud del vector sea 1, lo cual es crucial para mantener una velocidad constante independientemente de si se presionan una o varias teclas simultáneamente.

- **Lógica Clave:** La lógica principal del script se bifurca en el método `Update` según el valor de `useArrows`, implementando dos modos de control de movimiento distintos. El modo de teclado (`useArrows = true`) calcula la dirección deseada utilizando `GetInputDirection()`, que ajusta el movimiento a la perspectiva de la cámara y lo normaliza para una velocidad consistente. El modo de ratón (`useArrows = false`) utiliza raycasting para determinar el punto de impacto de un clic en el mundo y reubica el objeto a esa posición, permitiendo una forma de teletransporte.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]` para forzar la presencia de otros componentes en el mismo `GameObject`. Sin embargo, para su correcto funcionamiento:
    *   En el modo de movimiento por teclado, la variable `playerCamera` debe estar asignada a un `Transform` válido de una cámara en la escena para que el movimiento sea relativo a la perspectiva de esta.
    *   En el modo de movimiento por ratón, es necesario que exista una `Camera` con la etiqueta "MainCamera" en la escena para que `Camera.main` pueda obtener la cámara de juego. Además, para que el raycast detecte colisiones y permita el teletransporte, los objetos en el entorno con los que se pretende interactuar deben tener componentes `Collider` adjuntos.
- **Eventos (Entrada):**
    El script escucha las entradas del usuario a través de la clase `Input` de Unity. Esto incluye:
    *   Presiones de teclas (flechas y WASD) mediante `Input.GetKey()` para controlar el movimiento direccional.
    *   Clics del botón izquierdo del ratón mediante `Input.GetMouseButtonDown(0)` para activar la teletransportación por clic.
- **Eventos (Salida):**
    Este script no invoca explícitamente ningún `UnityEvent` o `Action` personalizado para notificar a otros sistemas del juego sobre su estado o acciones. Su efecto principal es modificar directamente la posición del `GameObject` al que está adjunto.
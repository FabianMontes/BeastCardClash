# `Target.cs`

## 1. Propósito General
Este script gestiona el movimiento de un objeto en el entorno 3D del juego, ofreciendo dos modos de control distintos para el jugador: movimiento direccional con teclado (WASD o flechas) o teletransporte al hacer clic con el mouse. Su rol principal es traducir la entrada del usuario en cambios de posición del GameObject al que está adjunto.

## 2. Componentes Clave

### `Target`
- **Descripción:** La clase `Target` es un `MonoBehaviour` que controla el desplazamiento de un GameObject en la escena. Permite al usuario mover el objeto de dos maneras configurables: mediante la pulsación de teclas direccionales o a través de clics del mouse que determinan un punto de teletransporte.

- **Variables Públicas / Serializadas:**
    - `playerCamera` (Tipo: `Transform`): Una referencia al `Transform` de la cámara del jugador. Esta variable es crucial para calcular la dirección de movimiento relativa a la vista de la cámara cuando se utiliza el modo de control por teclado (WASD/flechas). Se serializa en el Inspector de Unity para permitir su asignación visual.
    - `speed` (Tipo: `float`): Define la velocidad a la que se mueve el GameObject cuando el modo de control por teclado está activo. Su valor es ajustable desde el Inspector para calibrar la rapidez del movimiento.
    - `useArrows` (Tipo: `bool`): Un booleano que actúa como un selector de modo de control. Si es `true`, el objeto se moverá usando las teclas de flecha o WASD. Si es `false`, el movimiento se realizará mediante clics del mouse. También es serializable para su configuración en el Inspector.

- **Métodos Principales:**
    - `void Update()`: Este es un método del ciclo de vida de Unity que se ejecuta una vez por cada frame. `Update` es el corazón de la lógica de movimiento en este script. Evalúa el valor de la variable `useArrows` para determinar qué modo de control está activo y luego invoca la lógica de movimiento correspondiente.
        - Si `useArrows` es `true`, llama a `GetInputDirection()` para obtener la dirección deseada y aplica esa dirección a la posición del objeto, multiplicando por `speed` y `Time.deltaTime` para un movimiento suave y dependiente del tiempo.
        - Si `useArrows` es `false`, detecta un clic izquierdo del mouse (`Input.GetMouseButtonDown(0)`). En caso de clic, lanza un rayo desde la posición del mouse en la pantalla hacia el mundo 3D (`Camera.main.ScreenPointToRay`). Si el rayo impacta con un collider (`Physics.Raycast`), el objeto se teletransporta instantáneamente a la posición del punto de impacto (`hit.point`).

    - `Vector3 GetInputDirection()`: Un método auxiliar privado diseñado para ser utilizado cuando el modo `useArrows` está activo. Su función es leer el estado de las teclas de entrada (flechas direccionales o WASD) y calcular un vector de dirección 3D resultante. Este método devuelve un `Vector3` normalizado que representa la dirección de movimiento deseada, asegurándose de que el movimiento sea horizontal (el componente `y` se establece en 0) y que la velocidad sea consistente independientemente de si se pulsa una o varias teclas a la vez.

- **Lógica Clave:**
    La lógica central del script reside en el método `Update`, que actúa como un despachador de entrada. Implementa una máquina de estados simple con dos modos de operación controlados por la variable `useArrows`. El primer modo, basado en teclado, calcula la dirección de movimiento relativa a la `playerCamera` y aplica un desplazamiento gradual. El segundo modo, basado en mouse, utiliza un raycast para determinar un punto en el espacio 3D y teletransporta el objeto a esa ubicación. La normalización de la dirección en `GetInputDirection()` asegura una velocidad constante y evita el movimiento vertical no deseado.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, para que su funcionalidad de teletransporte con el mouse funcione correctamente, la escena debe contener una cámara etiquetada como "MainCamera" (`Camera.main` se utiliza para lanzar rayos). Además, para que el raycast detecte puntos de impacto, deben existir colliders en el entorno 3D. El modo de movimiento por teclado también requiere que la variable `playerCamera` esté asignada en el Inspector para definir las direcciones de "adelante" y "derecha" relativas a una vista.

- **Eventos (Entrada):**
    El script escucha activamente las entradas del usuario a través de la clase `Input` de Unity. Esto incluye:
    - Pulsaciones de teclas direccionales (flechas y WASD) utilizando `Input.GetKey()`.
    - Clics del botón izquierdo del mouse utilizando `Input.GetMouseButtonDown(0)`.

- **Eventos (Salida):**
    Este script no invoca ningún evento de Unity (`UnityEvent`) ni de C# (`Action`) para notificar a otros sistemas sobre sus cambios. Modifica directamente la propiedad `transform.position` del GameObject al que está adjunto.
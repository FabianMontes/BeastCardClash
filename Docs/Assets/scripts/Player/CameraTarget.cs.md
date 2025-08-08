# `CameraTarget.cs`

## 1. Propósito General
Este script gestiona la posición de un GameObject en la escena, haciéndolo seguir de cerca a un objeto de jugador (`Player`). Su función principal es actuar como un punto de referencia posicional dinámico para la cámara del juego, permitiendo que esta siga al jugador sin heredar su rotación.

## 2. Componentes Clave

### `CameraTarget`
- **Descripción:** Esta clase hereda de `MonoBehaviour`, lo que le permite ser adjuntada a un GameObject en la escena de Unity. Su propósito es mantener la posición de su propio GameObject sincronizada con la posición de un `Transform` del jugador especificado. Los comentarios en el código sugieren que el GameObject al que se adjunta este script está diseñado para no rotar, lo cual es crucial para neutralizar la rotación del jugador en la cámara y proporcionar un punto de seguimiento estable.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] Transform Player`: Esta variable privada es serializada, lo que la hace visible y configurable directamente desde el Inspector de Unity. Se utiliza para asignar el componente `Transform` del objeto del jugador que este `CameraTarget` debe seguir.

- **Métodos Principales:**
    - `void Start()`: Un método del ciclo de vida de Unity que se invoca una vez al inicio, antes de la primera actualización del frame. En esta implementación, el método está vacío, lo que significa que no se ejecuta ninguna lógica inicial al comienzo del juego o cuando el script se habilita.
    - `void Update()`: Este método del ciclo de vida de Unity se ejecuta una vez por cada frame del juego. Su función principal es actualizar continuamente la posición del GameObject al que está adjunto este script. La posición del `CameraTarget` se establece para que sea idéntica a la posición del `Player` asignado en el Inspector.

- **Lógica Clave:**
    La lógica central del script reside en el método `Update`. En cada frame, se realiza una asignación directa de la posición del `Player` a la posición del `GameObject` de este script:
    ```csharp
    transform.position = Player.position;
    ```
    Esto asegura que el GameObject al que `CameraTarget` está adjunto se mueva exactamente con el jugador. El comentario en el código aclara que este "objeto alternativo" (el `CameraTarget`) no rota, lo que implica que solo se preocupa por la posición, no por la orientación del jugador, lo cual es útil para una cámara que necesita seguir una posición pero mantener su propia orientación.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, al heredar de `MonoBehaviour`, requiere estar adjunto a un `GameObject` en la escena de Unity para funcionar.
- **Eventos (Entrada):** El script `CameraTarget` no se suscribe ni escucha eventos externos de otros sistemas o componentes. Su comportamiento es impulsado únicamente por el ciclo de vida interno de Unity (`Update`).
- **Eventos (Salida):** Este script no invoca ni publica eventos personalizados (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas. Su efecto se limita a la manipulación directa de su propia posición.
# `CameraMovement.cs`

## 1. Propósito General

Este script gestiona el movimiento de la cámara principal del juego, asegurando que siga constantemente la posición de un objeto `Transform` designado, que en este caso es el "jugador". Su rol principal es mantener el enfoque de la vista del juego sobre el personaje controlado por el jugador.

## 2. Componentes Clave

### `CameraMovement`
- **Descripción:** La clase `CameraMovement` es un componente de Unity que hereda de `MonoBehaviour`. Su función es actualizar la posición del GameObject al que está adjunta (presumiblemente la cámara del juego) para que coincida con la posición de un objeto `Transform` de referencia en cada fotograma. Esto crea un efecto de "cámara sígueme".
- **Variables Públicas / Serializadas:**
    - `[SerializeField] Transform player;`: Esta variable de tipo `Transform` es el objetivo que la cámara seguirá. Está serializada (`[SerializeField]`) para que pueda ser asignada y configurada directamente desde el Inspector de Unity, permitiendo a los diseñadores o a otros ingenieros especificar fácilmente qué objeto debe seguir la cámara (por ejemplo, el GameObject del personaje principal).

- **Métodos Principales:**
    - `void Update()`: Este es un método del ciclo de vida de Unity que se invoca una vez por fotograma. Dentro de este método, el script actualiza la propiedad `transform.position` del GameObject al que está adjunto. Al asignar `player.position` a `transform.position`, la cámara se mueve instantáneamente para coincidir con la ubicación actual del GameObject del jugador en el mundo.

- **Lógica Clave:**
La lógica central de este script es muy sencilla: en cada fotograma del juego, la posición de la cámara se sincroniza directamente con la posición del objeto `player`. Esto garantiza un seguimiento constante y directo sin suavizado o retrasos, haciendo que la cámara permanezca fijada sobre el jugador en todo momento.

```csharp
void Update()
{
    transform.position = player.position;
}
```

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, por lo que no impone la necesidad de que el GameObject al que se adjunta tenga componentes específicos adicionales.
- **Eventos (Entrada):** El script se basa únicamente en el método `Update()` del ciclo de vida de Unity, que es invocado automáticamente por el motor en cada fotograma. No se suscribe a eventos externos ni a entradas específicas del usuario.
- **Eventos (Salida):** `CameraMovement` no invoca ningún evento (`UnityEvent` o `Action`) para notificar a otros sistemas o componentes sobre cambios en su estado o posición. Su función es puramente reactiva, actualizando su propia posición.
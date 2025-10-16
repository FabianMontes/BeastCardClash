# `CameraMovement`
Este script, `CameraMovement`, es un componente fundamental para controlar la posición de la cámara principal en `Beast Card Clash`. Su función principal es asegurar que el `GameObject` al que está adjunto (presumiblemente la cámara del juego) siga de forma directa y sin retardo a un `Transform` objetivo, designado como el "jugador" o entidad central.

Está diseñado para proporcionar un punto de vista constante durante el juego, manteniendo siempre la cámara centrada o siguiendo de cerca la posición de una entidad específica. Dada la naturaleza de `Beast Card Clash` como un juego de estrategia por turnos, este tipo de seguimiento rígido puede ser una elección intencional para acompañar movimientos discretos del "jugador" o para enfocar una acción en particular sin suavizados, ofreciendo una experiencia visual directa y predecible. La simplicidad de su implementación lo hace fácil de entender, mantener y modificar para futuras necesidades del proyecto.

# Métodos

## Métodos de Unity

### `Update`
`Update` es un método de ciclo de vida de Unity invocado una vez por frame. Es ideal para la lógica de juego que requiere ejecución continua, como el movimiento de la cámara.

```csharp
void Update()
{
    transform.position = player.position;
}
```

Dentro de este método:
-   `transform.position` hace referencia a la propiedad de posición del `GameObject` al que está adjunto el script `CameraMovement`. En un contexto de cámara, esto controlaría la ubicación espacial de la cámara en el mundo del juego.
-   `player.position` se refiere a la propiedad de posición del `Transform` del jugador que ha sido previamente asignado a la variable `player` en el Inspector de Unity.

La línea `transform.position = player.position;` establece la posición del `GameObject` de la cámara para que sea idéntica a la posición del `GameObject` del jugador en cada frame. Esto asegura que la cámara siempre estará directamente encima o en la misma posición que el jugador, produciendo un efecto de seguimiento "instantáneo" o "rígido". Este comportamiento es especialmente adecuado para juegos de estrategia por turnos donde los movimientos suelen ser discretos y un seguimiento suave no siempre es necesario o deseado.

## Otros métodos
Este script no define métodos adicionales más allá del ciclo de vida de Unity.

## Getters y Setters

1.  `player: Transform`: Establece el objeto `Transform` al que la cámara debe seguir. Este campo es una referencia crucial que se asigna desde el Inspector de Unity (gracias al atributo `[SerializeField]`), permitiendo a los diseñadores y programadores especificar visualmente qué entidad controlará la posición de la cámara en el juego.
    ```csharp
    [SerializeField] Transform player;
    ```
    El atributo `[Header("Player")]` simplemente organiza el Inspector de Unity, agrupando esta variable bajo una sección clara, facilitando su identificación y configuración.
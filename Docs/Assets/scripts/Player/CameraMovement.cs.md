# CameraMovement
Este script, `CameraMovement`, es un componente de Unity diseñado para gestionar el seguimiento de la cámara principal sobre un objetivo específico en el escenario. Su función principal es asegurar que el objeto `GameObject` al que está adjunto (presumiblemente la cámara del juego) mantenga su posición sincronizada con la posición de un `Transform` objetivo predefinido.

La implementación de este comportamiento es directa y sencilla, lo que lo hace ideal para proyectos donde la agilidad en el desarrollo y la claridad del código son prioritarias, como en nuestro proyecto **Beast Card Clash**. Permite a los desarrolladores establecer rápidamente un comportamiento de cámara de "seguir al jugador" sin configuraciones complejas, facilitando así la iteración y el prototipado rápido de las mecánicas de juego.

El script solo requiere que se le asigne el `Transform` del objetivo, que en el contexto de **Beast Card Clash** será la entidad que la cámara debe seguir, como el personaje del jugador o la unidad activa en un turno.

```csharp
public class CameraMovement : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] Transform player;

    void Update()
    {
        transform.position = player.position;
    }
}
```

# Métodos

## Métodos de Unity

### Update
El método `Update` es parte del ciclo de vida de Unity y se invoca una vez por cada frame del juego. Es el corazón de la lógica de seguimiento de la cámara en este script.

Su funcionamiento es el siguiente:
1.  En cada frame, `Update` accede a la propiedad `transform` del `GameObject` al que está adjunto este script (`CameraMovement`). Esta propiedad representa el `Transform` de dicho `GameObject` (es decir, la cámara, si el script está en ella).
2.  Luego, actualiza la `position` de este `Transform` para que sea idéntica a la `position` del `Transform` asignado a la variable `player`.

Esta operación continua asegura que la cámara siempre estará centrada en la misma ubicación que el `GameObject` referenciado como `player`, creando un efecto de seguimiento directo.

```csharp
void Update()
{
    transform.position = player.position;
}
```

## Getters y Setters
Este script no define métodos `getter` o `setter` públicos explícitos. La variable `player` es un campo serializado que se expone directamente en el Inspector de Unity para su asignación.

1.  `player` (Transform): Es un campo `SerializeField` que permite asignar el `Transform` del objeto que la cámara debe seguir directamente desde el Inspector de Unity. Esto elimina la necesidad de `getters` o `setters` programáticos para esta funcionalidad en este contexto.
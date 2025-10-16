# `SkinSelectorStartButton`
Este script, `SkinSelectorStartButton`, es un componente `MonoBehaviour` de Unity diseñado para facilitar la transición entre escenas del juego. Su función principal es cargar una escena específica en el motor de Unity. Dada la convención de nombres del proyecto, que apunta a una fase de selección de "skins" (apariencias para los personajes) antes de comenzar el juego, es muy probable que este script esté adjunto a un botón en la interfaz de usuario que, al ser pulsado, inicie la escena principal de juego una vez que el jugador ha realizado sus selecciones. Esto lo convierte en un elemento clave para la navegación entre los diferentes estados del juego (menús, selección de personaje y juego principal).

```csharp
public class SkinSelectorStartButton : MonoBehaviour
{
    // ...
}
```

La implementación es deliberadamente sencilla, enfocándose en la experiencia de desarrollo ágil mencionada en el contexto del proyecto. Utiliza el `SceneManager` de Unity para realizar la carga de la escena, que es la forma estándar de manejar las transiciones de escena en Unity.

# Métodos

## Métodos de Unity

Este script no implementa directamente ninguno de los métodos de ciclo de vida de Unity como `Awake`, `Start` o `Update`. Esto significa que su comportamiento no se activa automáticamente al inicio del componente o en cada fotograma, sino que depende de una invocación externa, muy probablemente a través de un evento de UI.

## Otros métodos

### `SetScene(int sceneIndex)`
Este método público es el corazón de la funcionalidad del script. Su propósito es cargar una escena del juego utilizando un índice entero (`sceneIndex`) que representa la posición de la escena en la configuración de "Build Settings" de Unity.

-   **Parámetros:**
    -   `sceneIndex` (tipo `int`): Un número entero que corresponde al índice de la escena que se desea cargar. Este índice se configura en Unity en `File > Build Settings...` bajo la sección "Scenes In Build".

-   **Funcionamiento:**
    El método invoca la función estática `LoadScene` de la clase `SceneManager` de Unity, pasándole el `sceneIndex` proporcionado.

    ```csharp
    public void SetScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    ```

    Esta operación reemplaza la escena actualmente activa con la escena especificada, recargando todos los objetos y scripts asociados a la nueva escena. Es fundamental asegurarse de que el `sceneIndex` sea válido y corresponda a una escena existente en la configuración de construcción del proyecto para evitar errores en tiempo de ejecución. Este método es ideal para ser vinculado a eventos de UI, como el evento `OnClick()` de un botón en el Inspector de Unity, permitiendo que la interacción del usuario dispare la carga de la siguiente etapa del juego.

## Getters y Setters

Este script no contiene métodos que actúen como *getters* o *setters* en el sentido tradicional de acceder o modificar propiedades internas del script. Su única función es realizar una acción (cargar una escena) en respuesta a una llamada externa.
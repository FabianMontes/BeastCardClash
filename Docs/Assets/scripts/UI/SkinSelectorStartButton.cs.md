# SkinSelectorStartButton
Este script `SkinSelectorStartButton` es un componente fundamental para la navegación entre escenas dentro del proyecto, específicamente diseñado para facilitar la transición desde la pantalla de selección de personajes o "skins" hacia la siguiente fase del juego, que podría ser una pantalla de carga o el inicio del gameplay.

Su función principal es orquestar la carga de una nueva escena en Unity al ser invocado, típicamente a través de un evento de interfaz de usuario como el clic de un botón. Al estar vinculado a un componente `Button` en la jerarquía de Unity, permite a los diseñadores y programadores definir fácilmente a qué escena se debe transicionar sin necesidad de modificar el código directamente, simplemente ajustando el índice de la escena en el Inspector de Unity.

Este enfoque simplificado para la gestión de escenas se alinea con la filosofía del proyecto de priorizar una buena experiencia de desarrollo, permitiendo transiciones claras y configurables desde elementos de UI, como un botón de "Iniciar Juego" después de seleccionar un animal o carta en "Beast Card Clash".

# Métodos

## Otros métodos

### SetScene(int sceneIndex)
Este método público es el único expuesto por el script y se encarga de la lógica de transición entre escenas.

**Funcionamiento:**
El método `SetScene` recibe un único parámetro de tipo `int` llamado `sceneIndex`. Este entero representa el índice de la escena que se desea cargar, tal como está configurado en las "Build Settings" de Unity (Archivo > Build Settings...).

Internamente, invoca a `SceneManager.LoadScene(sceneIndex)`. La clase `SceneManager` es parte del espacio de nombres `UnityEngine.SceneManagement` y es la API estándar de Unity para la gestión de carga y descarga de escenas. Al llamarla con un índice específico, Unity carga la escena correspondiente, reemplazando la escena actualmente activa.

```csharp
public void SetScene(int sceneIndex)
{
    SceneManager.LoadScene(sceneIndex);
}
```

**Uso previsto:**
Dado que el método es `public`, está diseñado para ser invocado externamente. Su uso más común y directo es ser asignado al evento `OnClick()` de un componente `Button` en la interfaz de usuario de Unity. Cuando el botón es presionado por el jugador, este método se ejecuta, desencadenando la carga de la nueva escena. Esto permite una transición fluida desde, por ejemplo, una pantalla de selección de personajes o cartas (donde el `SkinSelectorStartButton` estaría activo) hacia la pantalla de juego principal o una pantalla de carga intermedia.

**Ejemplo de configuración en Unity:**
1.  Asegúrate de que este script esté adjunto a un `GameObject` en tu escena (por ejemplo, el mismo `GameObject` que tiene el componente `Button`, o un `GameObject` controlador de UI).
2.  Selecciona el `Button` en la jerarquía.
3.  En el Inspector del `Button`, localiza la sección `OnClick()`.
4.  Haz clic en el botón `+` para añadir un nuevo evento.
5.  Arrastra el `GameObject` que contiene el script `SkinSelectorStartButton` al campo de objeto (`None (Object)`).
6.  En el desplegable de funciones, selecciona `SkinSelectorStartButton` -> `SetScene(int)`.
7.  Introduce el `sceneIndex` deseado (por ejemplo, `1` para la segunda escena en tus Build Settings) en el campo numérico que aparece.
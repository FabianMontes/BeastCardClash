# SkinSelector
Este script, `SkinSelector`, es un componente fundamental para la interacción del jugador con las representaciones visuales de los personajes en el juego `Beast Card Clash`. Su función principal es permitir la selección de un "skin" o personaje específico a través de una interfaz interactiva, proporcionando retroalimentación visual al jugador y comunicando la selección al sistema de gestión del estado del juego.

En el contexto de `Beast Card Clash`, donde cada personaje representa un animal autóctono y una facultad, este script se encarga de que la representación 3D o 2D de dicho personaje sea seleccionable. Cuando el jugador pasa el cursor por encima de un personaje, este se ilumina. Al hacer clic, se registra la selección del personaje, lo que permite al `GameState` del juego procesar esta elección y preparar al personaje para la partida. Además, asegura que los personajes se presenten con una animación inicial adecuada para el entorno de juego.

El script funciona detectando las interacciones del ratón (entrada/salida del cursor y clic) sobre el `Collider` asociado al GameObject donde reside. Utiliza un componente `Outline` (del paquete `OutlineFx`) para generar el efecto de resaltado visual y un `Animator` para controlar el estado de animación inicial del personaje. La selección final se comunica a una instancia global del `GameState` a través de un índice de skin configurable en el Inspector de Unity.

# Métodos

## Métodos de Unity

### Start
Este método se ejecuta una vez al inicio del ciclo de vida del script, cuando se habilita el GameObject o la escena comienza. Su propósito es inicializar los componentes necesarios y establecer el estado inicial del personaje en la interfaz de selección.

```csharp
private void Start()
{
    // Inicializa el Outline y el Animator y desactiva este último
    outline = GetComponentInChildren<Outline>();
    outline.enabled = false;
    GetComponent<Animator>().SetBool("isFigthing", true);
}
```

*   **Inicialización del `Outline`**: Se obtiene una referencia al componente `Outline` del personaje. Es importante notar que se utiliza `GetComponentInChildren<Outline>()`, lo que significa que el componente `Outline` no está directamente en el mismo GameObject que `SkinSelector`, sino en uno de sus hijos (probablemente el modelo 3D del personaje). Esto permite una mayor flexibilidad en la estructura de los prefabs de personaje. Una vez obtenido, el `outline` se desactiva por defecto (`outline.enabled = false;`) para que no aparezca resaltado hasta que el ratón interactúe con él.
*   **Configuración del `Animator`**: Se obtiene el componente `Animator` del GameObject actual (el que contiene el `SkinSelector`) y se establece el parámetro booleano `"isFigthing"` a `true`. Esto indica que el personaje debe comenzar en un estado de animación particular, posiblemente una pose de "listo para la batalla" o una pose de presentación activa, acorde con la naturaleza competitiva de `Beast Card Clash`.

### OnMouseDown
Este método es un *callback* de Unity que se invoca cuando el usuario presiona el botón principal del ratón mientras el puntero está sobre el `Collider` asociado a este GameObject. En el contexto de un selector de personajes, esta es la acción que confirma la elección del jugador.

```csharp
void OnMouseDown()
{
    GameState.singleton.SetSkin(skinIndex);
}
```

*   **Selección de Skin**: Cuando el personaje es clicado, el script notifica al sistema de gestión de estado del juego (`GameState`) que un nuevo skin ha sido seleccionado. Esto se logra llamando al método `SetSkin` de la instancia *singleton* de `GameState`, pasándole el `skinIndex` configurado para este `SkinSelector`. El `skinIndex` actúa como un identificador único para el personaje elegido, permitiendo a `GameState` cargar o configurar los datos correspondientes (habilidades, estadísticas, etc.) para la partida.

### OnMouseEnter
Este método es un *callback* de Unity que se invoca cuando el puntero del ratón entra en el área del `Collider` del GameObject. Se utiliza para proporcionar retroalimentación visual al jugador, indicando que el personaje es interactuable.

```csharp
private void OnMouseEnter()
{
    outline.enabled = true;
}
```

*   **Activación del `Outline`**: Al entrar el ratón, el componente `outline` se habilita (`outline.enabled = true;`), haciendo que el personaje se resalte visualmente. Este efecto le comunica al jugador que puede interactuar con el personaje.

### OnMouseExit
Este método es un *callback* de Unity que se invoca cuando el puntero del ratón sale del área del `Collider` del GameObject. Complementa a `OnMouseEnter` al eliminar la retroalimentación visual cuando el personaje ya no está bajo el cursor.

```csharp
private void OnMouseExit()
{
    outline.enabled = false;
}
```

*   **Desactivación del `Outline`**: Al salir el ratón, el componente `outline` se desactiva (`outline.enabled = false;`), eliminando el resaltado visual y devolviendo al personaje a su apariencia normal.

## Otros métodos
No hay métodos personalizados definidos en este script fuera de los callbacks de Unity.

## Getters y Setters
Este script no define getters o setters explícitos en forma de propiedades o métodos públicos para sus campos privados. El `skinIndex` es un campo privado pero configurable directamente desde el Inspector de Unity gracias al atributo `[SerializeField]`.
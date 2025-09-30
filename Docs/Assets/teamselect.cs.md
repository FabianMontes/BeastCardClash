# teamselect
El script `teamselect` es un componente fundamental para la interfaz de usuario (UI) de selección de equipos en Beast Card Clash. Su función principal es gestionar y visualizar el equipo actualmente seleccionado en el juego. Para ello, se encarga de:

1.  **Mostrar visualmente el equipo activo:** Asigna el `Sprite` correspondiente al equipo seleccionado a un componente `Image` adjunto al mismo GameObject, permitiendo una representación gráfica clara en la UI.
2.  **Permitir la selección de un equipo:** Proporciona un método público para que otros elementos de la UI (como botones) puedan cambiar el equipo actualmente activo a través del sistema de gestión de estado global (`GameState`).
3.  **Seguimiento opcional del estado global:** Puede configurarse para que su visualización siga automáticamente los cambios del equipo en el `GameState.singleton`, asegurando que la UI esté siempre sincronizada con la selección global.

Este script es crucial en las pantallas de preparación o selección donde el jugador elige qué "facultad" o "bando" representará, lo cual se alinea con la temática de las facultades de la UNAL y los animales colombianos del proyecto Beast Card Clash.

# Métodos

## Métodos de Unity

### Start
Este método se ejecuta una vez al inicio del ciclo de vida del script, antes de que se llame a `Update` por primera vez. Su propósito es inicializar la referencia al componente `Image` necesario para la funcionalidad del script.

```csharp
void Start()
{
    image = GetComponent<Image>();
}
```

Durante su ejecución, `Start` obtiene una referencia al componente `Image` que debe estar adjunto al mismo GameObject donde reside este script `teamselect`. Esta referencia se almacena en la variable privada `image`, lo que permite al script manipular el `Sprite` de la UI durante la ejecución. Si no hay un componente `Image` en el mismo GameObject, se generará un error en tiempo de ejecución.

### Update
El método `Update` se invoca una vez por cada frame del juego. Su lógica principal se centra en la actualización visual condicional del `Sprite` del equipo.

```csharp
void Update()
{
    if (Follow)
    {
        image.sprite = teams[(int)GameState.singleton.team];
    }
}
```

Este método verifica el valor de la variable booleana `Follow`. Si `Follow` es `true`, el script actualiza el `Sprite` del componente `image` para que coincida con el equipo actualmente seleccionado en el sistema de estado global del juego. Para ello, accede a `GameState.singleton.team`, que representa el equipo activo globalmente. Dado que `teams` es un array de `Sprite`, el valor de `GameState.singleton.team` (que probablemente es un `enum` de tipo `Team`) se convierte explícitamente a un entero (`int`) para usarlo como índice del array. Esto significa que la posición de cada `Sprite` en el array `teams` debe corresponder al valor numérico del `enum Team` asociado.

> 📝 **Nota:** El uso de `GameState.singleton` indica que existe un componente central en el proyecto que mantiene el estado global del juego, incluyendo la selección de equipos. Esto es una práctica común en el desarrollo de videojuegos para acceder a datos compartidos de forma sencilla.

## Otros métodos

### selectTeam(int team)
Este es un método público diseñado para ser invocado externamente, típicamente desde eventos de la UI, como el evento `OnClick()` de un `Button`. Su propósito es cambiar el equipo seleccionado globalmente en el juego.

```csharp
public void selectTeam(int team)
{
    GameState.singleton.SetTeam((Team)team);
}
```

Cuando se llama a `selectTeam`, recibe un valor entero (`int team`) que representa el nuevo equipo a seleccionar. Este valor entero se convierte explícitamente al tipo `Team` (que debe ser un `enum` definido en el proyecto) antes de pasarlo al método `SetTeam` del `GameState.singleton`. Esto asegura que el `GameState` actualice correctamente el equipo activo, y si la variable `Follow` del script `teamselect` está activa, la visualización se actualizará en el siguiente `Update`.

## Campos Configurable o de Estado

1.  `bool Follow`: Controla si el script debe actualizar automáticamente el `Sprite` de la UI para reflejar el equipo seleccionado en `GameState.singleton`. Si es `true`, la UI seguirá el estado global; si es `false`, la UI solo cambiará cuando el método `selectTeam` sea invocado directamente, o cuando otros scripts modifiquen el `sprite` de la `Image` directamente. Este campo es serializado (`[SerializeField]`) y puede configurarse desde el Inspector de Unity.
2.  `Sprite[] teams`: Un array de `Sprite`s. Cada `Sprite` en este array representa una de las posibles opciones de equipo disponibles en el juego. La posición (índice) de cada `Sprite` en el array se corresponde con el valor numérico del `enum Team` que representa. Este campo es serializado (`[SerializeField]`) y se configura desde el Inspector de Unity, permitiendo asignar los recursos gráficos para cada equipo.
3.  `Image image`: Una referencia al componente `UnityEngine.UI.Image` adjunto al mismo GameObject. Este componente es el encargado de mostrar visualmente el `Sprite` del equipo en la interfaz de usuario. Se inicializa en el método `Start()` mediante `GetComponent<Image>()`.
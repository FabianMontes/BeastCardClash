# `teamselect`
El script `teamselect.cs` es un componente de `MonoBehaviour` que gestiona la representación visual y la selección de equipos dentro de la interfaz de usuario del juego. Su función principal es vincular un componente `Image` de la UI a la selección de equipo actual del juego, así como permitir que otros elementos de la UI (como botones) actualicen esta selección globalmente. Este script actúa como un puente entre la interactividad de la interfaz de usuario y el estado central del juego, específicamente en lo que respecta al equipo activo.

Está diseñado para trabajar en conjunto con un sistema de `GameState` global (implementado como un `Singleton`), lo que permite que la selección del equipo sea persistente y accesible en todo el juego. La flexibilidad del script se observa en su capacidad de "seguir" automáticamente el estado del equipo global o de ser el disparador para cambiar dicho estado.

# Métodos

## Métodos de Unity

### `Start()`
Este método de ciclo de vida de Unity se ejecuta una única vez cuando el script se carga por primera vez. Su propósito es inicializar la referencia al componente `Image` que reside en el mismo `GameObject` al que está adjunto este script.

```csharp
void Start()
{
    image = GetComponent<Image>();
}
```

Almacenar esta referencia en la variable `image` evita la necesidad de llamar a `GetComponent<Image>()` repetidamente en cada `frame` (por ejemplo, en `Update()`), lo cual optimiza el rendimiento.

### `Update()`
Este método de ciclo de vida de Unity se ejecuta en cada `frame` del juego. Su lógica principal depende del valor del campo serializado `Follow`.

```csharp
void Update()
{
    if (Follow)
    {
        image.sprite = teams[(int)GameState.Singleton.Team];
    }
}
```

Si la variable booleana `Follow` está marcada como `true` en el Inspector de Unity, el script actualizará el `sprite` del componente `Image` adjunto. Este `sprite` se selecciona del array `teams` utilizando como índice el valor del equipo actual, que se obtiene de `GameState.Singleton.Team`. Esto permite que el `Image` siempre refleje visualmente el equipo activo del juego.

> [!NOTE]
> La conversión `(int)GameState.Singleton.Team` sugiere que `GameState.Singleton.Team` es una enumeración (`enum`) cuyos valores corresponden directamente a los índices del array `teams`, facilitando una asignación visual sencilla.

## Otros métodos

### `public void selectTeam(int team)`
Este método público está diseñado para ser invocado externamente, típicamente desde eventos de UI como un botón. Permite cambiar el equipo actualmente seleccionado en el estado global del juego.

```csharp
public void selectTeam(int team)
{
    GameState.Singleton.SetTeam((Team)team);
}
```

El parámetro `team` es un entero que se espera que represente el nuevo equipo a seleccionar. Internamente, este entero se convierte a un tipo `Team` (probablemente una enumeración) antes de pasarse al método `SetTeam` del `GameState` global. Esto asegura que la selección del equipo se actualice de manera consistente en todo el juego.

> [!IMPORTANT]
> La estructura de este método es fundamental para la interacción de la UI con la lógica central del juego. Un botón de "Seleccionar Equipo 1" en la UI, por ejemplo, podría invocar a `selectTeam(0)` en este script.

## Getters y Setters

Aunque el script no utiliza propiedades C# explícitas (getters/setters), sus campos serializados y métodos públicos cumplen funciones similares:

1.  `[SerializeField] bool Follow`: Permite establecer desde el Inspector de Unity si el `Image` debe actualizar su `sprite` de forma dinámica según el `GameState`.
2.  `[SerializeField] Sprite[] teams`: Permite asignar desde el Inspector de Unity el conjunto de `Sprites` que representan los diferentes equipos disponibles en el juego.
3.  `public void selectTeam(int team)`: Actúa como un *setter* para la propiedad `Team` dentro del `GameState.Singleton`, permitiendo actualizar el equipo activo del juego desde la UI.
4.  La línea `image.sprite = teams[(int)GameState.Singleton.Team];` en `Update()`: Actúa como un *getter* implícito de la propiedad `Team` dentro de `GameState.Singleton`, utilizándolo para determinar qué `sprite` mostrar.
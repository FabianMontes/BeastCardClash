# `PickerCard`
El script `PickerCard` es un componente de MonoBehaviour en el proyecto Beast Card Clash, encargado de gestionar la visualización de la carta que un `Fighter` (jugador) tiene actualmente "seleccionada" o "recogida" (`picked`). Su función principal es actuar como un intermediario visual, asegurando que el componente `HandCard` de un objeto hijo muestre siempre la carta que el `Fighter` padre ha elegido para interactuar. Esto es crucial para la experiencia de juego, ya que proporciona retroalimentación visual inmediata sobre qué carta está en consideración para ser jugada o utilizada.

El script opera observando el estado de la carta seleccionada por el `Fighter` en cada fotograma (`Update`). Si detecta un cambio en la carta seleccionada, actualiza el componente `HandCard` para reflejar dicho cambio, mostrando la nueva carta o un estado vacío si no hay ninguna carta seleccionada.

```csharp
public class PickerCard : MonoBehaviour
{
    // Referencia al componente HandCard hijo que mostrará la carta.
    HandCard card;
    // Referencia al componente Fighter padre, que es el propietario de la carta seleccionada.
    Fighter player;
    // Bandera para evitar actualizaciones redundantes si el estado de la carta seleccionada no ha cambiado.
    bool isPlaying = false;

    // ... (métodos)
}
```

# Métodos

## Métodos de Unity

### `Start()`
El método `Start()` se ejecuta una vez al inicio, después de que el objeto que contiene este script ha sido instanciado. Su propósito es inicializar las referencias a otros componentes con los que `PickerCard` debe interactuar.

1.  **Obtención de `HandCard`**: Busca y asigna una referencia al componente `HandCard` que se encuentre como hijo en la jerarquía del GameObject actual. Este `HandCard` es el responsable de la representación visual de la carta.
    ```csharp
    card = GetComponentInChildren<HandCard>();
    ```
2.  **Obtención de `Fighter`**: Busca y asigna una referencia al componente `Fighter` que se encuentre como padre en la jerarquía del GameObject actual. Este `Fighter` es el que gestiona lógicamente qué carta está "seleccionada".
    ```csharp
    player = GetComponentInParent<Fighter>();
    ```
3.  **Inicialización de la visualización de la carta**: Llama al método `SetCard(null)` en el `HandCard` recién obtenido. Esto asegura que, al inicio del juego, no se muestre ninguna carta por defecto, estableciendo un estado visual limpio.
    ```csharp
    card.SetCard(null);
    ```

### `Update()`
El método `Update()` se ejecuta en cada fotograma del juego. Su función principal es monitorear continuamente si el `Fighter` ha cambiado la carta que tiene "seleccionada" y, si es así, actualizar la visualización del `HandCard` para reflejar este cambio.

1.  **Detección de cambio en la carta seleccionada**: Compara el estado actual de la bandera `isPlaying` (que indica si se estaba mostrando una carta seleccionada) con el estado de la carta seleccionada del `Fighter` (`player.GetPicked() != null`).
    ```csharp
    if (isPlaying != (player.GetPicked() != null))
    ```
    -   `player.GetPicked()`: Se asume que este método del componente `Fighter` devuelve la `Card` actualmente seleccionada por el jugador, o `null` si no hay ninguna.
2.  **Actualización de estado y visualización**: Si hay un cambio (es decir, el `Fighter` acaba de seleccionar o deseleccionar una carta), se realizan dos acciones:
    -   La bandera `isPlaying` se actualiza para reflejar el nuevo estado de la carta seleccionada.
    -   El método `SetCard()` del `HandCard` se invoca con la carta actual obtenida de `player.GetPicked()`. Esto provoca que el `HandCard` actualice su visualización para mostrar la nueva carta (o dejar de mostrarla si `GetPicked()` devuelve `null`).
    ```csharp
        isPlaying = player.GetPicked() != null;
        card.SetCard(player.GetPicked());
    ```
    Este mecanismo evita actualizaciones innecesarias del `HandCard` si el estado de la carta seleccionada no ha cambiado, optimizando ligeramente el rendimiento.

## Otros métodos
Este script no define métodos públicos o privados adicionales que no sean parte del ciclo de vida de Unity (`Start`, `Update`). Todas las interacciones con otros componentes se realizan a través de sus métodos públicos (`SetCard` de `HandCard` y `GetPicked` de `Fighter`).

## Getters y Setters
Este script no define getters ni setters explícitos como propiedades públicas o métodos dedicados para exponer o modificar sus campos internos (`card`, `player`, `isPlaying`). El campo `isPlaying` es de uso interno exclusivo del script.
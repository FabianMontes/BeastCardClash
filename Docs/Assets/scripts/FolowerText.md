# `FolowerText`
`FolowerText` es un script de Unity diseñado para **mostrar dinámicamente información específica de un `Fighter` (personaje o carta) en un componente de texto de la interfaz de usuario**. Este script permite que un objeto de texto (utilizando `TextMeshProUGUI`) siga y actualice automáticamente la especie, la vida actual o el nombre de un `Fighter` asociado.

La flexibilidad de `FolowerText` reside en su capacidad para ser configurado directamente desde el Inspector de Unity a través del campo serializado `typeFollow`. Esto lo convierte en una herramienta versátil para construir elementos de UI (como barras de vida, nombres de personajes en las cartas, o descripciones de especie) que reflejan el estado de los personajes en el juego `Beast Card Clash`.

El script funciona buscando un componente `Fighter` en su GameObject padre. Una vez encontrado, inicializa el texto según la opción `TypeFollow` seleccionada. Si la opción es `live` (vida), el texto se actualizará continuamente en cada frame para reflejar cualquier cambio en los puntos de vida del `Fighter`, garantizando que la información mostrada sea siempre precisa durante el combate. Para `species` y `name`, el texto se establece una única vez al inicio, ya que estas propiedades no suelen cambiar durante el juego.

# Métodos

## Métodos de Unity

### `Start()`
El método `Start()` se invoca una vez al inicio del ciclo de vida del script, justo antes del primer `Update()`. Su propósito principal es la **inicialización de referencias y el establecimiento del texto inicial** que seguirá al `Fighter`.

```csharp
void Start()
{
    textMeshPro = GetComponent<TextMeshProUGUI>();
    string text = textMeshPro.text; // Esta línea es redundante ya que 'text' se reasigna inmediatamente después.

    player = GetComponentInParent<Fighter>();
    if (player != null)
    {
        switch (typeFollow)
        {
            case TypeFollow.species:
                text = player.GetSpecie().ToString();
                break;
            case TypeFollow.live:
                text = player.GetPlayerLive().ToString();
                break;
            case TypeFollow.name:
                text = player.fighterName;
                break;
        }
        textMeshPro.text = text;
    }
}
```

**Funcionamiento detallado:**
1.  **Obtención de `TextMeshProUGUI`:** Primero, se obtiene una referencia al componente `TextMeshProUGUI` que debe estar adjunto al mismo GameObject que este script. Esta es la UI que mostrará la información.
    ```csharp
    textMeshPro = GetComponent<TextMeshProUGUI>();
    ```
2.  **Búsqueda del `Fighter` padre:** El script busca un componente de tipo `Fighter` en los GameObjects padres. Esto establece una relación jerárquica donde el `Fighter` es el contenedor lógico de este elemento de texto de UI. Por ejemplo, un GameObject que representa una carta (`Fighter`) podría tener como hijo un GameObject con un `TextMeshProUGUI` y este script para mostrar su nombre o vida.
    ```csharp
    player = GetComponentInParent<Fighter>();
    ```
3.  **Configuración inicial del texto:** Si se encuentra un `Fighter` (`player != null`), se utiliza una estructura `switch` para determinar qué propiedad del `Fighter` debe seguir el texto, basándose en el valor de `typeFollow` (configurado en el Inspector):
    *   Si `typeFollow` es `species`, el texto se establece con el resultado de `player.GetSpecie()`.
    *   Si `typeFollow` es `live`, el texto se establece con el resultado de `player.GetPlayerLive()`.
    *   Si `typeFollow` es `name`, el texto se establece con el valor de `player.fighterName`.
    Finalmente, el `textMeshPro.text` se actualiza con la cadena generada.

### `Update()`
El método `Update()` se llama una vez por cada frame del juego. En el contexto de `FolowerText`, su función es **actualizar continuamente el texto** solo cuando se está siguiendo la vida (`TypeFollow.live`) del `Fighter`.

```csharp
void Update()
{
    if (typeFollow == TypeFollow.live)
    {
        textMeshPro.text = player.GetPlayerLive().ToString(); ;
    }
}
```

**Funcionamiento detallado:**
1.  **Condición de actualización:** El `Update()` solo procede si el campo `typeFollow` está configurado en `TypeFollow.live`. Esto es una optimización importante: el nombre y la especie de un `Fighter` generalmente no cambian durante el juego, por lo que actualizarlos en cada frame sería innecesario. Los puntos de vida, sin embargo, son una métrica que cambia constantemente.
2.  **Actualización de puntos de vida:** Si la condición se cumple, el texto del `TextMeshProUGUI` se actualiza con el valor actual de los puntos de vida del `Fighter`, obtenido a través del método `player.GetPlayerLive()`. Esto asegura que la barra de vida o el contador de vida visible siempre refleje el estado real del personaje.

## Otros métodos
El script `FolowerText` en sí mismo no define métodos públicos o privados adicionales más allá de los métodos de ciclo de vida de Unity (`Start`, `Update`). Sin embargo, interactúa fuertemente con métodos y propiedades de la clase `Fighter`.

## Getters y Setters
Este script no define getters o setters propios. En cambio, utiliza los siguientes métodos y propiedades de la clase `Fighter` para obtener la información que debe mostrar:

1.  `player.GetSpecie()`: Obtiene la información de la especie del `Fighter`. Esto probablemente devuelve un objeto o un `enum` que representa el animal autóctono, el cual luego se convierte a `string`.
2.  `player.GetPlayerLive()`: Obtiene los puntos de vida actuales del `Fighter`. Se espera que devuelva un valor numérico (entero o flotante).
3.  `player.fighterName`: Accede directamente al nombre del `Fighter`. Esto es probablemente un `string` que representa el nombre del personaje o su "personalidad académica".
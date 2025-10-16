# `SkinSelecter`
Este script, `SkinSelecter`, se encarga de gestionar y actualizar dinámicamente la apariencia visual (o "skin") de un objeto en el juego, específicamente para los tokens de personajes que representan animales y facultades de la UNAL. Su función principal es tomar un índice de skin y aplicar la textura correspondiente del `Sprite` asociado al material del `Renderer` del objeto. Está diseñado para interactuar con componentes padre y hermanos para obtener la información del jugador y su skin actual, lo que permite que la representación visual del personaje cambie según los datos de juego.

La directiva `[DefaultExecutionOrder(2)]` indica que este script se ejecutará después de aquellos con orden de ejecución predeterminado (o con orden 0 o 1). Esto sugiere que confía en que otros componentes esenciales, como `PlayerToken` o `Fighter`, ya se hayan inicializado antes de intentar acceder a ellos.

El script depende de una lista de `Sprites` (`Skins`) que deben ser asignados desde el Inspector de Unity, y de un componente `Renderer` en sí mismo o en uno de sus hijos, para poder modificar la textura del material.

# Métodos

## Métodos de Unity

### `Awake`
El método `Awake` se invoca cuando la instancia del script está siendo cargada. Su propósito principal es inicializar las referencias a los componentes necesarios que el script utilizará durante su ciclo de vida.

```csharp
private void Awake()
{
    objRenderer = GetComponentInChildren<Renderer>();
    figther = GetComponentInParent<PlayerToken>().player;
}
```

1.  **`objRenderer`**: Se obtiene una referencia al componente `Renderer`. Se utiliza `GetComponentInChildren<Renderer>()` lo que implica que el `Renderer` responsable de la visualización de la skin no necesariamente está en el mismo GameObject que el script `SkinSelecter`, sino que puede estar en uno de sus objetos hijos. Esto es útil para separar la lógica del controlador de la representación visual.
2.  **`figther`**: Se obtiene una referencia al objeto `Fighter` que representa al personaje en batalla. Para ello, primero busca el componente `PlayerToken` en el padre (`GetComponentInParent<PlayerToken>()`) y luego accede a la propiedad `player` de ese `PlayerToken`. Esto establece una conexión directa entre el selector de skin y los datos del jugador, asegurando que el script pueda leer el skin deseado del `Fighter`.

### `Update`
El método `Update` se llama una vez por cada frame del juego. En este script, su función es asegurar que la skin del personaje esté siempre actualizada con el valor actual definido en el componente `Fighter` del jugador.

```csharp
void Update()
{
    figther = GetComponentInParent<PlayerToken>().player;
    SetSkin(figther.Skin);
}
```

1.  **Actualización de `figther`**: De manera similar a `Awake`, el script vuelve a obtener la referencia al objeto `Fighter` del jugador. Este re-fetch continuo en cada frame puede ser una medida de seguridad para garantizar que siempre se esté apuntando a la instancia correcta del `Fighter`, especialmente si la instancia de `PlayerToken` o `Fighter` pudiera cambiar durante el tiempo de ejecución.
    > [!NOTE] Nota de rendimiento:
    > Re-obtener la referencia a `figther` y `PlayerToken` en cada frame con `GetComponentInParent` puede tener un impacto en el rendimiento si se ejecuta en muchos objetos. Para proyectos indie donde la experiencia de desarrollo es clave, esto puede ser aceptable, pero en proyectos más grandes, se podría considerar optimizar esta parte, por ejemplo, almacenando la referencia o actualizando la skin solo cuando la propiedad `figther.Skin` cambie.
2.  **Llamada a `SetSkin`**: Una vez obtenida la referencia al `Fighter`, se invoca el método `SetSkin`, pasándole como argumento el valor de la propiedad `Skin` del `Fighter`. Esto garantiza que la apariencia del objeto se sincronice constantemente con el skin seleccionado para el personaje en el juego.

## Otros métodos

### `SetSkin(int index)`
Este es un método público que permite cambiar la textura principal del material del `objRenderer` basándose en un índice proporcionado. Es el corazón de la funcionalidad de cambio de skin.

```csharp
public void SetSkin(int index)
{
    // Si no hay skin, o esta fuera de rango, alerta del error en consola
    if (Skins == null || index < 0 || index >= Skins.Count)
    {
        Debug.LogWarning("Índice de skin inválido.");
        return;
    }

    // Cambia la textura del material si existe
    if (objRenderer != null && Skins[index] != null)
    {
        Texture2D texture = Skins[index].texture;
        objRenderer.material.mainTexture = texture;
    }
}
```

1.  **Validación de entrada**: El método primero realiza una serie de comprobaciones para asegurar que el `index` proporcionado es válido:
    *   Verifica que la lista `Skins` no sea nula.
    *   Asegura que el `index` no sea negativo.
    *   Comprueba que el `index` esté dentro de los límites de la cantidad de skins disponibles en la lista (`index < Skins.Count`).
    Si alguna de estas condiciones no se cumple, se emite un mensaje de advertencia en la consola de Unity (`Debug.LogWarning`) y el método finaliza su ejecución para evitar errores.
2.  **Aplicación de la textura**: Si el `index` es válido, el script procede a cambiar la textura:
    *   Verifica que `objRenderer` no sea nulo y que el `Sprite` en la posición `index` de la lista `Skins` tampoco sea nulo.
    *   Extrae la `Texture2D` del `Sprite` seleccionado.
    *   Asigna esta `Texture2D` a la propiedad `mainTexture` del material de `objRenderer`. Esto es lo que visualmente modifica la apariencia del objeto en la escena, aplicando la nueva skin.

## Getters y Setters
Este script no define métodos `getter` o `setter` públicos explícitos para sus campos internos. Interactúa con las propiedades `Skin` del objeto `Fighter` (que se asume es un `getter`) y expone la lista `Skins` como un campo serializado para ser configurado en el Inspector de Unity.
# SkinSelecter
`SkinSelecter` es un script `MonoBehaviour` de Unity diseñado para gestionar y aplicar dinámicamente las apariencias visuales (skins) a los personajes o tokens de jugador en el juego. Su función principal es tomar un índice de skin, que representa un `Sprite` específico cargado en el editor, y aplicarlo como textura principal al `Renderer` de un objeto hijo.

El script se inicializa buscando un componente `Renderer` en sus hijos y un componente `Figther` que se encuentra a través de un `PlayerToken` en uno de sus padres. Este diseño permite que el `SkinSelecter` se monte en un objeto que es parte de una jerarquía más grande, donde el `PlayerToken` identifica al jugador y el `Figther` contiene la información del skin seleccionado.

El funcionamiento del script está centrado en mantener la apariencia del personaje sincronizada con el estado actual del juego o del jugador. Para ello, en cada frame, consulta la propiedad `skin` del `Figther` asociado y actualiza la textura del `Renderer` del personaje. Esto asegura que cualquier cambio en la skin del `Figther` se vea reflejado visualmente de inmediato.

El atributo `[DefaultExecutionOrder(2)]` indica que este script se ejecuta después de la mayoría de los scripts predeterminados (que tienen un orden de ejecución de 0 o 1). Esto es crucial para asegurar que los componentes de los que depende, como `PlayerToken` y `Figther`, ya estén inicializados cuando `SkinSelecter` intenta acceder a ellos en el método `Awake`.

# Métodos

## Métodos de Unity

### Awake
El método `Awake` se invoca cuando la instancia del script se está cargando. Se utiliza para inicializar referencias a componentes necesarios para el funcionamiento de `SkinSelecter`.

```csharp
private void Awake()
{
    objRenderer = GetComponentInChildren<Renderer>();
    figther = GetComponentInParent<PlayerToken>().player;
}
```

*   `objRenderer = GetComponentInChildren<Renderer>();`: Busca y asigna una referencia al primer componente `Renderer` encontrado en cualquiera de los objetos hijos de este `GameObject`. Esto implica que el objeto visual al que se aplicará la skin no es el mismo `GameObject` que contiene `SkinSelecter`, sino un hijo suyo.
*   `figther = GetComponentInParent<PlayerToken>().player;`: Busca el componente `PlayerToken` en el `GameObject` padre o en cualquier `GameObject` ancestral de este `GameObject`. Una vez que se encuentra el `PlayerToken`, se accede a su propiedad `player` (que se espera sea de tipo `Figther`) y se asigna a la variable `figther`. Esta línea establece una conexión con el objeto `Figther` que contiene la información del skin actual del jugador.

### Update
El método `Update` se llama una vez por frame. En `SkinSelecter`, su propósito es asegurar que la skin del personaje esté siempre actualizada según el estado actual del `Figther` asociado.

```csharp
void Update()
{
    figther = GetComponentInParent<PlayerToken>().player;
    SetSkin(figther.skin);
}
```

*   `figther = GetComponentInParent<PlayerToken>().player;`: Esta línea re-obtiene la referencia al `Figther` a través del `PlayerToken` padre en cada frame. Aunque esto podría ser un punto de optimización en proyectos con requisitos de rendimiento muy estrictos, en el contexto de este proyecto prioriza la simplicidad y la seguridad de que `figther` siempre refleje el estado más actual, incluso si la referencia al `PlayerToken` o su propiedad `player` pudiera cambiar durante el tiempo de ejecución.
*   `SetSkin(figther.skin);`: Llama al método `SetSkin` con el valor actual de la propiedad `skin` del objeto `figther`. Esto provoca que la skin visual se actualice a la seleccionada por el `Figther` en ese momento.

## Otros métodos

### SetSkin(int index)
El método `SetSkin` es el encargado de aplicar visualmente una skin al `Renderer` del personaje. Recibe un índice que corresponde a la posición de un `Sprite` en la lista `Skins`.

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

*   **Validación de entrada:**
    ```csharp
    if (Skins == null || index < 0 || index >= Skins.Count)
    {
        Debug.LogWarning("Índice de skin inválido.");
        return;
    }
    ```
    Antes de intentar aplicar la skin, el método verifica que la lista `Skins` no sea nula y que el `index` proporcionado esté dentro de los límites válidos de la lista. Si el índice es inválido, se imprime una advertencia en la consola de Unity, lo cual es útil para la depuración durante el desarrollo, y el método finaliza su ejecución.

*   **Aplicación de la textura:**
    ```csharp
    if (objRenderer != null && Skins[index] != null)
    {
        Texture2D texture = Skins[index].texture;
        objRenderer.material.mainTexture = texture;
    }
    ```
    Si el índice es válido y `objRenderer` no es nulo (es decir, se encontró un `Renderer` al inicio), se realiza lo siguiente:
    1.  `Texture2D texture = Skins[index].texture;`: Se obtiene la textura subyacente del `Sprite` ubicado en la posición `index` de la lista `Skins`.
    2.  `objRenderer.material.mainTexture = texture;`: Se asigna esta `Texture2D` como la `mainTexture` (textura principal) del material del `objRenderer`. Esto cambia la apariencia visual del `GameObject` hijo para mostrar la skin seleccionada.

## Getters y Setters

1.  `SetSkin(int index)`: Establece la skin visual del personaje utilizando el `Sprite` en la posición `index` de la lista `Skins`.
2.  `figther.skin`: Esta propiedad, aunque no es un getter de `SkinSelecter` directamente, es una fuente externa (`Figther`) de la que `SkinSelecter` "gettea" el índice de la skin a aplicar.
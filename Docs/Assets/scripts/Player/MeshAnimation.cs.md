# MeshAnimation
El script `MeshAnimation` es un componente fundamental para la gestión visual y de animaciones de los personajes o entidades en el proyecto **Beast Card Clash**. Su principal responsabilidad es coordinar la **apariencia (skin)** y el **estado de animación** de un objeto `GameObject` al que está adjunto.

Permite a los desarrolladores:
1.  **Gestionar múltiples "skins" o texturas** para un mismo objeto, cargándolas desde una lista de `Sprites` definida en el editor de Unity.
2.  **Controlar dinámicamente los parámetros de un `Animator`** adjunto, lo que facilita la activación de diferentes estados de animación.

Este componente se actualiza continuamente en función del estado global del juego (a través de `GameState.singleton.skin`), asegurando que la apariencia del personaje siempre refleje la selección actual del jugador. Está diseñado para ser flexible, permitiendo la manipulación de animaciones y skins desde otras partes del código del juego de manera desacoplada.

# Métodos

## Métodos de Unity

### Awake
Este método se ejecuta una única vez cuando el script se carga. Su propósito es inicializar las referencias a los componentes necesarios para el funcionamiento de `MeshAnimation`.

```csharp
private void Awake()
{
    // Inicializa los componentes
    animator = GetComponent<Animator>();
    objRenderer = GetComponentInChildren<Renderer>();
}
```

*   `animator`: Busca y obtiene una referencia al componente `Animator` que debe estar adjunto al mismo `GameObject` que este script. Si no se encuentra un `Animator`, las operaciones relacionadas con animaciones no tendrán efecto.
*   `objRenderer`: Busca y obtiene una referencia al componente `Renderer` (por ejemplo, `SpriteRenderer` o `MeshRenderer`) que debe estar adjunto a uno de los `GameObject` hijos de donde está este script. Este componente es esencial para aplicar las diferentes "skins" o texturas.

### Update
Este método se ejecuta en cada frame del juego. Su función es asegurar que la "skin" visual del objeto esté siempre sincronizada con el estado global del juego.

```csharp
void Update()
{
    // Actualiza la skin desde GameState
    SetSkin(GameState.singleton.skin);
}
```

Cada frame, `Update` llama al método `SetSkin` pasándole el valor de `GameState.singleton.skin`. Esto implica que hay un sistema de `GameState` global (implementado como un Singleton) que mantiene un registro de la `skin` actual seleccionada, permitiendo que los cambios en esta variable se reflejen instantáneamente en la apariencia del objeto.

## Otros métodos

### public void UpdateAnimation(string variableName, string value)
Este método proporciona una interfaz flexible para controlar los parámetros del componente `Animator` adjunto al `GameObject`. Permite modificar los estados de animación pasando los nombres de las variables y sus valores como cadenas de texto.

```csharp
public void UpdateAnimation(string variableName, string value)
{
    if (animator == null) return;

    foreach (AnimatorControllerParameter param in animator.parameters)
    {
        if (param.name == variableName)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    if (bool.TryParse(value, out bool boolValue)) animator.SetBool(variableName, boolValue);
                    break;
                // ... otros tipos ...
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(variableName);
                    break;
            }
            return;
        }
    }
    Debug.LogWarning($"No se encontró el parámetro '{variableName}' en el Animator.");
}
```

*   **Detección de tipo:** El método itera a través de todos los parámetros definidos en el `AnimatorController`. Cuando encuentra una coincidencia con `variableName`, determina automáticamente el tipo de parámetro (`Bool`, `Float`, `Int`, `Trigger`).
*   **Conversión y asignación:** Intenta convertir la cadena `value` al tipo de dato correspondiente (booleano, flotante, entero) y luego asigna ese valor al parámetro del `Animator`. Para los `Trigger`s, simplemente activa el disparador.
*   **Manejo de errores:** Si no se encuentra un `Animator` o si el parámetro especificado no existe, se emitirá una advertencia en la consola de Unity para facilitar la depuración.
*   **Utilidad:** Este enfoque permite a otros sistemas del juego (como la UI, la lógica de combate o controladores de entrada) activar y modificar animaciones sin necesidad de conocer los tipos de parámetros exactos del `Animator` o acceder directamente a su API.

### public void SetSkin(int index)
Este método es responsable de cambiar la textura visible del objeto, aplicando una de las "skins" precargadas en la lista `Skins`.

```csharp
public void SetSkin(int index)
{
    if (Skins == null || index < 0 || index >= Skins.Count)
    {
        Debug.LogWarning("Índice de skin inválido.");
        return;
    }

    if (objRenderer != null && Skins[index] != null)
    {
        Texture2D texture = Skins[index].texture;
        objRenderer.material.mainTexture = texture;
    }
}
```

*   **Validación:** Antes de intentar cambiar la skin, el método valida si la lista `Skins` está inicializada y si el `index` proporcionado está dentro de los límites válidos de la lista. Si no es así, se registra una advertencia.
*   **Aplicación de textura:** Si el índice es válido y las referencias existen, se extrae la `Texture2D` del `Sprite` ubicado en `Skins[index]`. Luego, esta textura se asigna a `objRenderer.material.mainTexture`, lo que actualiza la apariencia visual del objeto.
*   **`Skins`:** La lista `Skins` es una colección de `Sprite` que debe ser configurada manualmente en el Inspector de Unity. Cada `Sprite` en esta lista representa una posible "skin" que el objeto puede adoptar.

## Getters y Setters

1.  `public void UpdateAnimation(string variableName, string value)`: Establece el valor de un parámetro específico (`Bool`, `Float`, `Int` o `Trigger`) en el componente `Animator` del objeto.
2.  `public void SetSkin(int index)`: Establece la textura principal del `Renderer` del objeto, seleccionando un `Sprite` de la lista `Skins` mediante el `index` proporcionado.
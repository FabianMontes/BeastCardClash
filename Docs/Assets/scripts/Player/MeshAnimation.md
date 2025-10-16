# `MeshAnimation`
Este script, `MeshAnimation`, es un componente fundamental para la gestión de la representación visual dinámica de los personajes o elementos en **Beast Card Clash**. Su propósito principal es controlar tanto las animaciones como las texturas (skins) de un objeto de juego al que esté adjunto, fusionando mecánicas de animación con la capacidad de cambiar la apariencia visual.

Actúa como un puente entre la lógica del juego y los componentes visuales de Unity, como el `Animator` y el `Renderer`. Se encarga de actualizar los parámetros del `Animator` basándose en entradas de texto, lo que permite una configuración flexible de las transiciones y estados de animación. Además, gestiona la selección y aplicación de diferentes "skins" o texturas, lo que probablemente se utiliza para alternar entre distintas apariencias de los animales/facultades, añadiendo un elemento personalizable y visualmente atractivo al juego.

La forma en que interactúa con el resto del proyecto es clave:
*   **Integración con el sistema de estados (`GameState.Singleton`)**: Recupera la skin actual desde un singleton global (`GameState.Singleton.Skin`) en cada fotograma, lo que sugiere que el estado global del juego determina la apariencia visual del objeto. Esto es una forma sencilla y directa de sincronizar la UI/personajes con la lógica del juego.
*   **Control de Animator flexible**: Permite a otros sistemas del juego (quizás controladores de IA, entradas del jugador o eventos de juego) modificar el comportamiento de las animaciones a través de llamadas de método sencillas con `string` y `value`, lo que facilita la integración sin la necesidad de un acoplamiento fuerte a la estructura interna del Animator.
*   **Gestión de recursos visuales (`Skins`)**: La lista de `Skins` (`List<Sprite>`) configurada directamente en el Inspector de Unity permite a los diseñadores y artistas configurar fácilmente las apariencias disponibles sin necesidad de cambios en el código.

En el contexto de **Beast Card Clash**, este script es vital para dar vida a los "animales autóctonos con personalidad académica", permitiendo que sus apariencias y movimientos reflejen tanto su naturaleza como su "facultad" asignada. Este enfoque de desarrollo prioriza la facilidad de uso y la experiencia de desarrollo, al permitir una manipulación sencilla de aspectos visuales complejos.

# Métodos

## Métodos de Unity

### `Awake()`
El método `Awake()` se ejecuta una vez cuando el script se carga. Su función es inicializar las referencias a los componentes necesarios para el funcionamiento del `MeshAnimation`:

1.  Obtiene una referencia al componente `Animator` que se encuentra en el mismo GameObject donde está este script.
2.  Obtiene una referencia al componente `Renderer` que se encuentra en uno de los GameObjects hijos de la jerarquía. Esto es útil si el modelo 3D o el renderizador de la skin no están directamente en el mismo GameObject que el script `MeshAnimation`, sino en un hijo (ej. un modelo que contiene el MeshRenderer).

```csharp
private void Awake()
{
    animator = GetComponent<Animator>();
    objRenderer = GetComponentInChildren<Renderer>();
}
```

### `Update()`
El método `Update()` se llama una vez por cada fotograma del juego. Su propósito es mantener la apariencia visual del objeto sincronizada con el estado global del juego.

En cada fotograma, llama al método `SetSkin()` y le pasa el valor del índice de la skin actual, el cual es recuperado del singleton `GameState.Singleton.Skin`. Esto significa que la skin del objeto se actualizará continuamente para reflejar cualquier cambio en la skin seleccionada a nivel global por el juego.

> [!NOTE]
> La llamada a `SetSkin()` en cada `Update()` garantiza que la skin siempre esté actualizada, pero si la skin no cambia con frecuencia, podría considerarse una pequeña optimización mover esta lógica a un evento o solo cuando `GameState.Singleton.Skin` realmente cambie. Para la filosofía de desarrollo del proyecto, que prioriza una buena experiencia de desarrollo y funcionalidad directa, esta implementación es sencilla y efectiva.

```csharp
void Update()
{
    SetSkin(GameState.Singleton.Skin);
}
```

## Otros métodos

### `UpdateAnimation(string variableName, string value)`
Este método público permite modificar los parámetros del componente `Animator` de forma dinámica, utilizando nombres de variables y valores como cadenas de texto. Esto ofrece una gran flexibilidad para activar animaciones o cambiar estados desde otros scripts o sistemas de juego sin un acoplamiento directo a los nombres o tipos específicos de los parámetros del Animator.

Su funcionamiento es el siguiente:
1.  **Verificación del Animator**: Primero, comprueba si `animator` ha sido inicializado. Si no lo está, el método se detiene para evitar errores.
2.  **Iteración de parámetros**: Recorre todos los parámetros definidos en el controlador del `Animator` adjunto.
3.  **Coincidencia de nombre**: Busca un parámetro cuyo `name` coincida con `variableName`.
4.  **Asignación de valor por tipo**: Si encuentra una coincidencia, determina el `type` del parámetro (`Bool`, `Float`, `Int`, `Trigger`) y, en base a esto:
    *   Intenta parsear el `value` de `string` al tipo correspondiente (`bool`, `float`, `int`).
    *   Asigna el valor parseado al parámetro del `Animator` utilizando los métodos `SetBool`, `SetFloat`, `SetInteger` o `SetTrigger`.
    *   Una vez que el parámetro ha sido asignado, el método finaliza (`return`).
5.  **Advertencia de error**: Si después de revisar todos los parámetros no se encuentra una coincidencia para `variableName`, se imprime una advertencia en la consola para notificar al desarrollador.

```csharp
public void UpdateAnimation(string variableName, string value)
{
    if (animator == null) return; // Si no hay animator, se devuelve

    foreach (AnimatorControllerParameter param in animator.parameters)
    {
        if (param.name == variableName)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    if (bool.TryParse(value, out bool boolValue)) animator.SetBool(variableName, boolValue);
                    break;
                case AnimatorControllerParameterType.Float:
                    if (float.TryParse(value, out float floatValue)) animator.SetFloat(variableName, floatValue);
                    break;
                case AnimatorControllerParameterType.Int:
                    if (int.TryParse(value, out int intValue)) animator.SetInteger(variableName, intValue);
                    break;
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

### `SetSkin(int index)`
Este método público es responsable de cambiar la textura visual del objeto al que está adjunto el script, seleccionando una "skin" de la lista predefinida `Skins`.

Su funcionamiento es el siguiente:
1.  **Validación de entrada**: Verifica si la lista `Skins` es nula o si el `index` proporcionado está fuera del rango válido (entre 0 y el número total de skins - 1). Si la validación falla, se imprime una advertencia y el método se detiene.
2.  **Aplicación de textura**: Si el `objRenderer` existe y el `Sprite` en la posición `index` de la lista `Skins` no es nulo:
    *   Extrae la `Texture2D` del `Sprite` seleccionado.
    *   Asigna esta `Texture2D` como la `mainTexture` del material del `objRenderer`. Esto cambia la apariencia visual del objeto.

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

## Getters y Setters
Este script no contiene métodos públicos dedicados específicamente a la funcionalidad de obtener o establecer valores de propiedades internas en el patrón típico de Getters y Setters. Los campos internos se gestionan directamente o a través de los métodos públicos existentes que realizan acciones (`UpdateAnimation`, `SetSkin`).
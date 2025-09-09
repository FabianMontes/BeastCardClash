# Outline
El script `Outline` es una clase base abstracta de tipo `MonoBehaviour` que establece la funcionalidad fundamental para generar un efecto de contorno o resaltado alrededor de objetos en la escena. Su propósito principal es definir una interfaz común para cualquier tipo de contorno, permitiendo que las clases derivadas especifiquen el color del contorno y se integren con un sistema centralizado de renderizado de efectos visuales llamado `OutlineFxFeature`.

Al ser una clase abstracta, `Outline` no puede ser instanciada directamente en un GameObject. En cambio, otras clases (por ejemplo, `Outline_Card`, `Outline_Character`) deberán heredar de `Outline` e implementar sus propiedades y métodos abstractos para definir contornos específicos. Esto permite una gran flexibilidad para aplicar diferentes tipos de contornos a diversas entidades del juego, como cartas, personajes en el campo de batalla o elementos interactivos, lo cual es crucial para la jugabilidad estratégica de "Beast Card Clash".

El componente está configurado con los atributos `[ExecuteAlways]` y `[DisallowMultipleComponent]`.
*   `[ExecuteAlways]` asegura que el script funcione tanto en el modo de edición de Unity como en tiempo de ejecución. Esto es particularmente útil para los desarrolladores, ya que permite visualizar los contornos directamente en el editor sin necesidad de iniciar el juego, mejorando la experiencia de desarrollo al facilitar la iteración y el diseño visual.
*   `[DisallowMultipleComponent]` previene que se añadan múltiples componentes `Outline` (o sus derivados) al mismo GameObject, evitando conflictos y garantizando que cada objeto tenga un único efecto de contorno gestionado por este sistema.

Internamente, el script gestiona una referencia al componente `Renderer` del GameObject al que está adjunto. Este `Renderer` es esencial, ya que es el que dibuja el modelo 3D del objeto y, por lo tanto, es el punto de partida para generar el efecto de contorno.

# Métodos

## Métodos de Unity

### Awake, Start, Update
No hay implementaciones directas de los métodos `Awake`, `Start` o `Update` en esta clase `Outline`. La lógica principal se gestiona a través de `OnEnable` y `OnWillRenderObject`.

### OnEnable
Este método se invoca cuando el GameObject al que está adjunto el script se activa o se habilita. Su función es crucial para la inicialización del componente, ya que se encarga de obtener y almacenar una referencia al componente `Renderer` que se encuentra en el mismo GameObject.

```csharp
private void OnEnable()
{
    _renderer = GetComponent<Renderer>();
}
```

Al cachear el `Renderer` de esta manera, se evita la necesidad de buscarlo repetidamente en cada cuadro (por ejemplo, dentro de `Update`), lo que mejora la eficiencia. Este `_renderer` es fundamental para que el sistema `OutlineFxFeature` sepa qué objeto debe contornear.

### OnWillRenderObject
Este método es un *callback* de Unity que se invoca automáticamente una vez por cámara, justo antes de que el GameObject se vaya a renderizar. Es el punto principal donde se activa la lógica de contorno.

```csharp
private void OnWillRenderObject()
{
#if UNITY_EDITOR
    if (Application.isEditor && Equals(_renderer, null) == false)
    {
        if (TryGetComponent<Renderer>(out _renderer) == false)
            return;
    }
#endif
    
    OutlineFxFeature.Render(this);
}
```

La implementación tiene dos partes importantes:

1.  **Manejo en el editor (`#if UNITY_EDITOR`)**:
    *   Esta sección de código se ejecuta únicamente cuando el proyecto está abierto en el editor de Unity. Su propósito es asegurar la robustez del script en el entorno de desarrollo.
    *   Si el `_renderer` es `null` (lo cual podría ocurrir después de una recompilación de scripts en el editor o si el componente `Renderer` fue añadido o eliminado después del `OnEnable`), el script intenta re-obtener la referencia al `Renderer` utilizando `TryGetComponent`.
    *   Si no logra encontrar un `Renderer` (por ejemplo, si el GameObject no tiene uno), la ejecución del método se detiene (`return`), evitando posibles errores en tiempo de edición. Este comportamiento contribuye a una mejor experiencia de desarrollo, permitiendo a los programadores ver los contornos en el editor de manera fiable.

2.  **Delegación del renderizado (`OutlineFxFeature.Render(this)`)**:
    *   Una vez asegurado que se tiene una referencia válida al `Renderer`, el método invoca `OutlineFxFeature.Render(this)`. Esta línea es el corazón de la interacción del script con el sistema de contornos.
    *   `OutlineFxFeature` parece ser una clase estática o un *singleton* centralizado que gestiona toda la lógica de renderizado de los contornos para el proyecto.
    *   Al pasar `this` (la instancia actual de `Outline`), se le proporciona a `OutlineFxFeature` toda la información necesaria para renderizar el contorno del objeto, incluyendo el `_renderer` almacenado y la `Color` definida en las clases derivadas. Este enfoque centralizado es ideal para un juego indie como "Beast Card Clash", ya que simplifica la gestión de efectos visuales a gran escala sin sacrificar la flexibilidad.

## Getters y Setters

1.  `Color` (tipo `Color`): Esta es una propiedad abstracta que las clases derivadas de `Outline` deben implementar. Permite obtener y establecer el color específico que tendrá el contorno. Por ejemplo, una carta seleccionada podría tener un contorno azul, mientras que una unidad a punto de atacar podría tener un contorno rojo.
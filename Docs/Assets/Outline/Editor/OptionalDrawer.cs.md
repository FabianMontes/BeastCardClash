# OptionalDrawer
`OptionalDrawer` es un script de extensión del editor de Unity diseñado para mejorar la interfaz de usuario de las propiedades que utilizan el tipo genérico `Optional<T>` en el Inspector. Su propósito principal es presentar valores que pueden ser activados o desactivados de manera opcional en una forma compacta y visualmente intuitiva, combinando un campo de valor con un interruptor de activación.

Al ser un `CustomPropertyDrawer`, `OptionalDrawer` intercepta cómo Unity dibuja las propiedades de `Optional<T>` y las reemplaza con su propia representación personalizada. Esto es crucial para la experiencia de desarrollador, ya que simplifica la configuración de datos en el Inspector. En lugar de tener una variable booleana separada para "habilitado" y otra para el "valor", `Optional<T>` las encapsula, y `OptionalDrawer` las muestra como un único control unificado.

El script reside en el namespace `OutlineFx.Editor`, lo que sugiere que podría formar parte de un sistema más amplio de efectos de contorno (`OutlineFx`) o ser una utilidad general dentro de ese contexto. Aunque el script no provee la definición de `Optional<T>`, su implementación indica que `Optional<T>` debe contener al menos dos campos serializados:
- `value`: El valor real de tipo `T` que puede ser opcional.
- `enabled`: Un booleano que determina si `value` está activo o debe ser considerado.

Este enfoque mejora la claridad y reduce la posibilidad de errores al configurar datos del juego que tienen un estado opcional, contribuyendo directamente a una "buena experiencia de desarrollo" tal como se describe en el README del proyecto para *Beast Card Clash*. Como script de Editor, no tiene impacto en el rendimiento del juego en tiempo de ejecución, solo en la experiencia de configuración dentro del entorno de Unity.

# Métodos

## Métodos de Unity

### GetPropertyHeight
Este método sobrescrito de `PropertyDrawer` es invocado por Unity para determinar la altura vertical necesaria para dibujar la propiedad `Optional<T>` en el Inspector.

El método accede al campo `value` del `Optional<T>` subyacente y delega la determinación de la altura a `EditorGUI.GetPropertyHeight(valueProperty)`. Esto significa que la altura total del control visual del `Optional<T>` será la misma que la del `value` que contiene, sin añadir espacio extra para el toggle de activación/desactivación, ya que este se dibuja adyacente al campo del valor.

```csharp
public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
{
    var valueProperty = property.FindPropertyRelative("value");
    return EditorGUI.GetPropertyHeight(valueProperty);
}
```

### OnGUI
Este es el método principal sobrescrito de `PropertyDrawer` que Unity llama cada vez que necesita dibujar el control visual de la propiedad `Optional<T>` en el Inspector.

Dentro de este método, se extraen dos `SerializedProperty` claves del `property` de entrada: `valueProperty` (que representa el valor real opcional) y `enabledProperty` (que indica si el valor está activado). Luego, delega el proceso de dibujo real a un método estático `OnGui`, pasándole la posición, la etiqueta y las propiedades `enabled` y `value`. Esta delegación permite que la lógica de dibujo pueda ser reutilizada si fuera necesario en otros contextos fuera de esta instancia específica del `PropertyDrawer`.

```csharp
public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
{
    var valueProperty   = property.FindPropertyRelative("value");
    var enabledProperty = property.FindPropertyRelative("enabled");

    OnGui(position, label, enabledProperty, valueProperty);
}
```

## Otros métodos

### OnGui(Rect, GUIContent, SerializedProperty, SerializedProperty)
Este es un método estático que contiene la lógica central para dibujar el control personalizado de `Optional<T>`. Es llamado por el método `OnGUI` y es responsable de la disposición y el comportamiento visual del campo en el Inspector.

Su funcionamiento se divide en varios pasos:
1.  **Ajuste del ancho de la posición:** El ancho del `Rect` `position` se reduce para dejar espacio al toggle de activación que se dibujará a la derecha del campo de valor.
    ```csharp
    position.width -= k_ToggleWidth;
    ```
2.  **Grupo de deshabilitación:** Utiliza un `EditorGUI.DisabledGroupScope` para deshabilitar visualmente el campo del `value` si la propiedad `enabledProperty` es `false`. Esto proporciona una indicación clara de que el valor no está activo y evita que se pueda modificar cuando no debería.
    ```csharp
    using (new EditorGUI.DisabledGroupScope(!enabledProperty.boolValue))
        EditorGUI.PropertyField(position, valueProperty, label, true);
    ```
    > [!NOTE]
    > `EditorGUI.PropertyField` dibuja el campo del valor (`valueProperty`) utilizando el comportamiento de dibujo predeterminado de Unity. El parámetro `true` indica que se deben dibujar también los hijos del campo si los tuviera (por ejemplo, si el valor es un `struct` o una clase con propiedades serializadas).
3.  **Ajuste y restauración de la indentación:** Se guarda la indentación actual del Inspector, se restablece a 0 para el toggle y se restaura al valor original después de dibujarlo. Esto asegura que el toggle se alinee correctamente sin heredar la indentación del campo principal.
    ```csharp
    int indent = EditorGUI.indentLevel;
    EditorGUI.indentLevel = 0;
    // ... toggle drawing ...
    EditorGUI.indentLevel = indent;
    ```
4.  **Dibujo del toggle:** Se calcula la posición del toggle a la derecha del campo de valor, y se dibuja un `EditorGUI.Toggle` que permite al usuario activar o desactivar la `enabledProperty`. `GUIContent.none` se usa para que el toggle no tenga una etiqueta de texto.
    ```csharp
    var togglePos = new Rect(position.x + position.width + EditorGUIUtility.standardVerticalSpacing, position.y, k_ToggleWidth, EditorGUIUtility.singleLineHeight);
    enabledProperty.boolValue = EditorGUI.Toggle(togglePos, GUIContent.none, enabledProperty.boolValue);
    ```

Este método asegura que el usuario tenga un control directo sobre la activación del valor opcional directamente en el Inspector, haciendo la configuración más eficiente y menos propensa a errores.

## Getters y Setters
El script `OptionalDrawer` no define métodos o propiedades explícitas que funcionen como *getters* o *setters* directos para el estado interno de la clase. Su función principal es la de dibujar una interfaz de usuario para propiedades de otras clases, accediendo y modificando sus `SerializedProperty` de forma indirecta a través de la API de Unity.
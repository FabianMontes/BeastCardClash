# SolidMaskDrawer
Este script `SolidMaskDrawer` es un `PropertyDrawer` de Unity, lo que significa que es una herramienta personalizada del editor diseñada para controlar cómo se visualizan y editan ciertas propiedades en el Inspector de Unity. Específicamente, está configurado para dibujar la estructura `OutlineFxFeature.SolidMask`.

En el contexto de **Beast Card Clash**, donde los efectos visuales como los `OutlineFx` (efectos de contorno) son importantes para destacar personajes o cartas y proporcionar retroalimentación al jugador, este `PropertyDrawer` mejora la experiencia de desarrollo. Permite a los diseñadores y programadores configurar de manera más intuitiva y organizada los parámetros de una "máscara sólida" que, por su nombre, probablemente define cómo se aplica o se comporta un efecto de contorno sólido, o una sección de él.

La funcionalidad principal de `SolidMaskDrawer` es:
1.  **Organización del Inspector:** Transforma la presentación predeterminada de la estructura `OutlineFxFeature.SolidMask` en una interfaz más compacta y funcional.
2.  **Toggle de Habilitación:** Permite activar o desactivar la máscara sólida directamente en el Inspector. Cuando está deshabilitada, los campos relacionados con el patrón, la escala y la velocidad se muestran atenuados, indicando que no están activos.
3.  **Colapsado/Expandido:** Implementa un sistema de colapsado (Foldout) para ocultar o mostrar los detalles de la configuración (`_scale`, `_velocity`), manteniendo el Inspector limpio hasta que se necesiten los detalles.
4.  **Configuración de Patrón, Escala y Velocidad:** Proporciona campos para ajustar un `_pattern` (posiblemente una textura o tipo de patrón), `_scale` (tamaño o intensidad del patrón) y `_velocity` (velocidad de animación si el patrón es dinámico).

Este enfoque en el editor mejora la facilidad de uso para los miembros del equipo, siguiendo el objetivo del proyecto de priorizar la buena experiencia de desarrollo, incluso cuando no se siguen "buenas prácticas perfectas" en cada detalle, sino la utilidad y claridad.

# Métodos

## Métodos de Unity

### GetPropertyHeight
Este método es parte de la API de `PropertyDrawer` y se encarga de determinar la altura total, en píxeles, que el dibujador de propiedades necesita en el Inspector de Unity.

**Funcionamiento:**
El método calcula la altura basándose en el estado de expansión de la propiedad.
*   Si la propiedad (`property.isExpanded`) está colapsada (`false`), el dibujador solo ocupará el espacio de una línea (`EditorGUIUtility.singleLineHeight`). Esto muestra únicamente el `label` y el control de `_enabled` y `_pattern`.
*   Si la propiedad está expandida (`true`), el dibujador requerirá el espacio de tres líneas para acomodar el `label` inicial, `_scale` y `_velocity`.

```csharp
public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
{
    var lines = 3;
    
    if (property.isExpanded == false)
        lines = 1;
    
    return lines * EditorGUIUtility.singleLineHeight;
}
```
Este diseño ayuda a mantener el Inspector ordenado al ocultar la configuración detallada hasta que sea necesario.

### OnGUI
Este es el método principal del `PropertyDrawer` donde se dibuja la interfaz de usuario personalizada para la estructura `OutlineFxFeature.SolidMask` en el Inspector de Unity.

**Funcionamiento:**
1.  **Recuperación de Propiedades:** Al inicio, se obtienen referencias a los `SerializedProperty` correspondientes a los campos internos de `OutlineFxFeature.SolidMask` utilizando `property.FindPropertyRelative`. Estos campos son `_enabled`, `_pattern`, `_scale` y `_velocity`.

    ```csharp
    var enabled  = property.FindPropertyRelative(nameof(OutlineFxFeature.SolidMask._enabled));
    var pattern  = property.FindPropertyRelative(nameof(OutlineFxFeature.SolidMask._pattern));
    var scale    = property.FindPropertyRelative(nameof(OutlineFxFeature.SolidMask._scale));
    var velocity = property.FindPropertyRelative(nameof(OutlineFxFeature.SolidMask._velocity));
    ```

2.  **Dibujo de la Primera Línea (Habilitar y Patrón):** Se utiliza una función `OptionalDrawer.OnGui` (que se asume es un utilitario externo para dibujar propiedades opcionales) para renderizar el `label` principal, el campo `_enabled` y el campo `_pattern`. El `_fieldRect` auxiliar calcula la posición y el tamaño para esta línea.

3.  **Control de Expansión (Foldout):** Justo después, en la misma línea, se dibuja un `EditorGUI.Foldout`. Este control permite al usuario expandir o colapsar la sección de detalles de la máscara sólida, controlando el valor de `property.isExpanded`.

    ```csharp
    OptionalDrawer.OnGui(_fieldRect(line ++), label, enabled, pattern);
    property.isExpanded = EditorGUI.Foldout(_fieldRect(line - 1), property.isExpanded, GUIContent.none, true);
    ```

4.  **Dibujo Condicional de Propiedades Detalladas:**
    *   Si `property.isExpanded` es `false`, el método termina aquí, sin dibujar los campos `_scale` y `_velocity`.
    *   Si `property.isExpanded` es `true`, se incrementa `EditorGUI.indentLevel` para sangrar visualmente los campos anidados, mejorando la legibilidad.
    *   Se utiliza un `EditorGUI.DisabledGroupScope` para deshabilitar los campos `_scale` y `_velocity` si el campo `_enabled` está desmarcado (`!enabled.boolValue`). Esto proporciona una clara retroalimentación visual de que estos ajustes no tendrán efecto.
    *   Finalmente, `EditorGUI.PropertyField` se usa para dibujar los campos `_scale` y `_velocity` en sus respectivas líneas.
    *   Se restaura `EditorGUI.indentLevel` a su valor original.

Este proceso garantiza una interfaz de usuario limpia y funcional en el Inspector para la configuración de la máscara sólida, facilitando la personalización de efectos visuales como contornos en los elementos de juego de **Beast Card Clash**.

## Otros métodos

### _fieldRect(int line)
Este es un método auxiliar privado (`private`) dentro de `SolidMaskDrawer` que facilita el cálculo de las posiciones y dimensiones (`Rect`) de los controles de la interfaz de usuario en el Inspector.

**Funcionamiento:**
Toma un número de línea (`line`) como parámetro. A partir de la posición base (`position`) y la altura de una sola línea (`EditorGUIUtility.singleLineHeight`), calcula un nuevo `Rect` para un control específico.
*   La coordenada `x` y el `width` se mantienen iguales a los de la `position` base.
*   La coordenada `y` se calcula sumando `line * EditorGUIUtility.singleLineHeight` a la `position.y` base, lo que posiciona el control en la línea correcta.
*   La `height` se establece en `EditorGUIUtility.singleLineHeight`, asegurando que cada control ocupe el espacio de una sola línea.

```csharp
Rect _fieldRect(int line)
{
    return new Rect(position.x, position.y + line * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
}
```
Este método encapsula la lógica de posicionamiento, haciendo que el código del método `OnGUI` sea más legible y fácil de mantener al no repetir los cálculos de `Rect` para cada control.

## Getters y Setters

En el contexto de un `PropertyDrawer`, no hay métodos `Getters` o `Setters` explícitos en el script `SolidMaskDrawer` en el sentido tradicional. En su lugar, el `PropertyDrawer` interactúa directamente con los `SerializedProperty` de la estructura `OutlineFxFeature.SolidMask` para leer y escribir sus valores a través de los controles de la interfaz de usuario de Unity (como `EditorGUI.PropertyField`). Los siguientes puntos describen los datos de la estructura `OutlineFxFeature.SolidMask` que son pedidos o establecidos por este dibujador:

1.  `_enabled`: Establece si la funcionalidad de la máscara sólida está activa o inactiva. Cuando está deshabilitada, los otros parámetros de la máscara no tendrán efecto o estarán atenuados en el Inspector.
2.  `_pattern`: Establece el recurso o tipo de patrón visual que se aplicará a la máscara sólida. Esto podría ser una textura, un material o una configuración predefinida que define la apariencia del contorno.
3.  `_scale`: Establece el factor de escala o tamaño que se aplica al `_pattern` de la máscara sólida. Esto afecta qué tan grande o pequeño se visualiza el patrón. Solo es configurable si la máscara está habilitada.
4.  `_velocity`: Establece la velocidad a la que el `_pattern` de la máscara sólida se anima o se mueve. Esto es útil para crear efectos de contorno dinámicos. Solo es configurable si la máscara está habilitada.
# MenuTexts
`MenuTexts` es un `ScriptableObject` diseñado para centralizar y gestionar los textos localizados utilizados en los diferentes menús de la interfaz de usuario del juego. Su propósito principal es almacenar las cadenas de texto correspondientes a elementos comunes de la UI (como botones o etiquetas) en múltiples idiomas, facilitando así la internacionalización del proyecto *Beast Card Clash*.

Este script utiliza el atributo `[CreateAssetMenu]` para permitir la creación de instancias de este `ScriptableObject` directamente desde el editor de Unity. Esto se realiza accediendo al menú `Assets -> Create -> Localizations/Menu Texts`.

```csharp
[CreateAssetMenu(fileName = "MenuTexts", menuName = "Localizations/Menu Texts")]
public class MenuTexts : ScriptableObject
{
    // ...
}
```

Al ser un `ScriptableObject`, `MenuTexts` permite la creación de instancias de datos configurables directamente desde el editor de Unity, sin necesidad de asociarlos a un GameObject en la escena. Esto lo convierte en una solución eficiente para desacoplar los datos de localización de la lógica del juego y de los componentes de la UI, promoviendo una estructura más limpia y mantenible, especialmente útil en un proyecto indie donde la flexibilidad es clave.

Actualmente, este `ScriptableObject` soporta textos en español y en inglés para elementos como el botón de inicio, el botón de créditos y una etiqueta para la selección de idiomas. Otros scripts del proyecto (por ejemplo, managers de UI o componentes de texto de la interfaz) pueden hacer referencia a una instancia de `MenuTexts` para obtener la cadena de texto adecuada según el idioma seleccionado por el jugador. Esto permite una actualización dinámica y consistente de la interfaz de usuario en todo el juego, lo cual es fundamental para *Beast Card Clash* al buscar una experiencia educativa y lúdica que atraiga a una audiencia diversa, incluyendo aquellos interesados en la cultura y naturaleza colombiana.

# Métodos

## Métodos de Unity
Este `ScriptableObject` no contiene métodos de Unity estándar (como `Awake`, `Start`, `Update`, etc.) ya que su función es puramente la de un contenedor de datos estático, no un componente de lógica de escena.

## Otros métodos
Este `ScriptableObject` no implementa métodos personalizados. Su diseño se centra en la exposición directa de las variables de texto.

# Getters y Setters
Este `ScriptableObject` no implementa métodos `getter` o `setter` explícitos. En su lugar, todas las cadenas de texto son declaradas como variables `public string`, permitiendo el acceso y la modificación directa desde otros scripts. Este enfoque simplifica la manipulación de los datos y se alinea con la filosofía de desarrollo rápido y directo del proyecto.

Las variables públicas disponibles son:

1.  `public string startButton_es`: Almacena el texto para el botón de inicio en español. Valor por defecto: `"Iniciar"`.
2.  `public string creditsButton_es`: Almacena el texto para el botón de créditos en español. Valor por defecto: `"Créditos"`.
3.  `public string languagesLabel_es`: Almacena el texto para la etiqueta de idiomas en español. Valor por defecto: `"Idiomas"`.
4.  `public string startButton_en`: Almacena el texto para el botón de inicio en inglés. Valor por defecto: `"Start"`.
5.  `public string creditsButton_en`: Almacena el texto para el botón de créditos en inglés. Valor por defecto: `"Credits"`.
6.  `public string languagesLabel_en`: Almacena el texto para la etiqueta de idiomas en inglés. Valor por defecto: `"Languages"`.
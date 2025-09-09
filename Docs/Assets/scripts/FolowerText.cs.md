# FolowerText
El script `FolowerText` es un componente `MonoBehaviour` diseñado para actualizar dinámicamente el texto de un componente `TextMeshProUGUI` en función de la información obtenida de un componente `Figther` (luchador) ubicado en un objeto padre. Su función principal es mostrar en la interfaz de usuario detalles específicos del "luchador", como su especie, su cantidad de "vida" (live) o su nombre, adaptándose a los requisitos del juego Beast Card Clash para visualizar los atributos de los personajes en las cartas o en el campo de juego.

El script permite configurar, a través del Inspector de Unity, qué tipo de información del `Figther` debe seguir el texto. Una vez inicializado, obtiene la información y la muestra. Para el caso específico de la "vida" (live), el texto se actualiza continuamente, reflejando cualquier cambio en el valor de la vida del luchador en tiempo real.

# Métodos

## Métodos de Unity

### Start
El método `Start` se ejecuta una única vez al inicio del ciclo de vida del script, después de que todos los objetos han sido instanciados y se han establecido las referencias iniciales. Su propósito es la inicialización del componente `FolowerText`.

1.  **Obtención del `TextMeshProUGUI`:**
    Se recupera el componente `TextMeshProUGUI` adjunto al mismo `GameObject` donde reside `FolowerText`. Este es el componente visual de texto que el script controlará.

    ```csharp
    textMeshPro = GetComponent<TextMeshProUGUI>();
    ```

2.  **Búsqueda del `Figther` padre:**
    El script busca un componente `Figther` en los `GameObject`s ascendentes (padres) de la jerarquía. Esto establece la conexión con el "luchador" del cual se obtendrá la información.

    ```csharp
    player = GetComponentInParent<Figther>();
    ```

3.  **Configuración inicial del texto:**
    Si se encuentra un componente `Figther` padre, se procede a establecer el texto inicial basándose en el valor de la variable `typeFollow`, que se configura en el Inspector de Unity.

    ```csharp
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
                text = player.figtherName;
                break;
        }
        textMeshPro.text = text;
    }
    ```
    -   Si `typeFollow` es `species`, el texto mostrará la especie del luchador, obtenida a través de `player.GetSpecie()`.
    -   Si `typeFollow` es `live`, el texto mostrará la vida actual del luchador, obtenida a través de `player.GetPlayerLive()`.
    -   Si `typeFollow` es `name`, el texto mostrará el nombre del luchador, obtenido directamente de `player.figtherName`.

### Update
El método `Update` se invoca una vez por cada fotograma del juego. Su función principal en este script es garantizar que el texto que muestra la "vida" del luchador se mantenga siempre actualizado.

1.  **Actualización condicional de la vida:**
    Solo si `typeFollow` está configurado como `TypeFollow.live`, el texto del `TextMeshProUGUI` se actualiza con el valor actual de la vida del `Figther` padre. Esto asegura que cualquier cambio en la vida del luchador se refleje instantáneamente en la interfaz de usuario.

    ```csharp
    if (typeFollow == TypeFollow.live)
    {
        textMeshPro.text = player.GetPlayerLive().ToString();
    }
    ```
    Para otros tipos de seguimiento (especie o nombre), no es necesaria una actualización constante, ya que se asume que estos valores no cambian durante el juego o se establecen una única vez al inicio.

## Otros métodos
El script `FolowerText` no define métodos adicionales propios fuera de los métodos del ciclo de vida de Unity. Su lógica se encapsula completamente en `Start` y `Update`.

## Getters y Setters
Este script interactúa con el componente `Figther` para obtener datos. A continuación, se detallan los métodos y propiedades que `FolowerText` utiliza del componente `Figther` para su funcionamiento:

1.  `Figther.GetSpecie()`: Obtiene la especie del `Figther`.
2.  `Figther.GetPlayerLive()`: Obtiene el valor actual de la "vida" (puntos de salud o vitalidad) del `Figther`.
3.  `Figther.figtherName`: Accede al nombre del `Figther`.
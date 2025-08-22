# `SkinSelecter.cs`

## 1. Propósito General
Este script es fundamental para gestionar la apariencia visual de los personajes o entidades en el juego "Beast Card Clash". Su rol principal es aplicar dinámicamente una "skin" (textura) a un objeto 3D, asegurando que la representación visual del combatiente se alinee con el índice de skin especificado por la lógica del juego.

## 2. Componentes Clave

### `SkinSelecter`
- **Descripción:** La clase `SkinSelecter` es un `MonoBehaviour` que controla la apariencia visual de un objeto de juego, específicamente cambiando la textura principal de su material. Se encarga de seleccionar un `Sprite` de una lista predefinida y utilizar su textura para actualizar el `Renderer` asociado, permitiendo que el personaje cambie su "skin" en tiempo real. La ejecución de este script está configurada para ocurrir tempranamente en el ciclo de actualización de Unity (`[DefaultExecutionOrder(2)]`).

- **Variables Públicas / Serializadas:**
    - `Skins`: Una `List<Sprite>` que contiene todos los recursos de `Sprite` que pueden ser utilizados como skins. Esta lista se configura directamente desde el Inspector de Unity, permitiendo a los diseñadores asignar las diferentes apariencias disponibles para el personaje. El método `SetSkin` utiliza un índice numérico para seleccionar uno de estos `Sprite`s.

- **Métodos Principales:**
    - `void Awake()`: Este método del ciclo de vida de Unity se invoca cuando la instancia del script es cargada. Se utiliza para inicializar referencias a otros componentes necesarios. Busca un `Renderer` entre los hijos del GameObject actual para poder manipular su material y obtiene una referencia al objeto `Figther` del jugador a través del componente `PlayerToken` encontrado en un ancestro.
        ```csharp
        objRenderer = GetComponentInChildren<Renderer>();
        figther = GetComponentInParent<PlayerToken>().player;
        ```
    - `void Update()`: Llamado una vez por fotograma, este método del ciclo de vida de Unity se asegura de que la skin del personaje esté siempre actualizada. Re-obtiene la referencia al objeto `Figther` desde el `PlayerToken` padre en cada fotograma y luego invoca el método `SetSkin` utilizando el valor de `figther.skin`. Esto implica que la skin del combatiente puede cambiar dinámicamente y el sistema la reflejará constantemente.
    - `public void SetSkin(int index)`: Este método es el corazón de la funcionalidad del script. Recibe un entero `index` que representa la posición de la skin deseada dentro de la lista `Skins`. El método primero valida si la lista `Skins` existe y si el índice proporcionado está dentro de los límites válidos. Si el índice es válido y los componentes existen, extrae la `Texture2D` del `Sprite` seleccionado y la asigna a la propiedad `mainTexture` del material del `objRenderer`, actualizando así la apariencia visual del objeto. En caso de un índice inválido, emite una advertencia en la consola.
        ```csharp
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
        ```

- **Lógica Clave:** La lógica central de `SkinSelecter` reside en su capacidad para reaccionar a la propiedad `figther.skin` en cada fotograma, lo que permite actualizaciones visuales fluidas. El método `SetSkin` gestiona la carga y aplicación de texturas a partir de `Sprite`s, incluyendo una validación de seguridad para evitar errores por índices fuera de rango o recursos nulos. La directiva `[DefaultExecutionOrder(2)]` garantiza que esta lógica se ejecute en una fase temprana del fotograma, lo cual es útil si otros sistemas dependieran de la skin ya actualizada.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Aunque este script no utiliza el atributo `[RequireComponent]`, su funcionalidad depende de la presencia de los siguientes componentes:
    *   Un componente `Renderer` debe ser un hijo del GameObject al que está adjunto este script, ya que el script lo busca con `GetComponentInChildren<Renderer>()`.
    *   Un componente `PlayerToken` debe existir en un GameObject padre, y ese `PlayerToken` debe exponer un campo `public Figther player` para que el `SkinSelecter` pueda obtener la referencia al objeto `Figther` y acceder a su propiedad `skin`.

- **Eventos (Entrada):** Este script no se suscribe a eventos de `UnityEvent` ni a delegados C# (`Action`s). Su principal fuente de "entrada" son los eventos del ciclo de vida de Unity (`Awake`, `Update`) y el valor de la propiedad `figther.skin` que lee continuamente.

- **Eventos (Salida):** El script `SkinSelecter` no invoca ni publica eventos personalizados (`UnityEvent` o `Action`) para notificar a otros sistemas sobre los cambios de skin. Su función se limita a actualizar su propia representación visual. Sin embargo, sí emite advertencias en la consola (`Debug.LogWarning`) si detecta un intento de establecer una skin con un índice inválido.
# `MeshAnimation.cs`

## 1. Propósito General
Este script se encarga de gestionar la animación y la apariencia visual (skins) de un `GameObject` en Unity. Permite actualizar parámetros del `Animator` de forma dinámica y cambiar la textura principal del material del objeto utilizando una lista de sprites predefinidos.

## 2. Componentes Clave

### MeshAnimation
- **Descripción:** La clase `MeshAnimation` hereda de `MonoBehaviour`, lo que significa que puede ser adjuntada como un componente a cualquier `GameObject` en la escena de Unity. Su función principal es servir como un controlador centralizado para las animaciones y los "skins" (apariencias visuales) de un personaje o elemento del juego, facilitando la interacción con los componentes `Animator` y `Renderer`.

- **Variables Públicas / Serializadas:**
    - `Skins`: Esta variable serializada (`[SerializeField]`) es una lista de objetos `Sprite`. Se utiliza para almacenar las diferentes texturas o "skins" que el objeto puede adoptar. Los sprites se asignan directamente desde el Inspector de Unity, permitiendo al diseñador configurar fácilmente las apariencias disponibles.

- **Métodos Principales:**
    - `void Awake()`: Este es un método del ciclo de vida de Unity que se llama cuando el script es inicializado, incluso antes de que el juego comience. En `Awake`, el script obtiene referencias a los componentes `Animator` (adjunto al mismo `GameObject`) y `Renderer` (adjunto al mismo `GameObject` o a uno de sus hijos). Estas referencias son cruciales para que el script pueda controlar las animaciones y la apariencia del objeto.
    - `public void UpdateAnimation(string variableName, string value)`: Este método público permite cambiar los valores de los parámetros del componente `Animator` asociado. Recibe el nombre del parámetro como un `string` (`variableName`) y el nuevo valor también como un `string` (`value`). El método es inteligente; itera a través de los parámetros del `Animator`, detecta su tipo (booleano, flotante, entero o trigger) y luego intenta parsear el `value` de `string` al tipo correcto antes de actualizar el parámetro correspondiente en el `Animator`. Esto proporciona una forma flexible de controlar las animaciones desde otras partes del código que quizás no conozcan el tipo exacto del parámetro.
    - `public void SetSkin(int index)`: Este método público es responsable de cambiar la textura principal del material del `GameObject` al que está adjunto el script. Recibe un `index` que corresponde a la posición de un `Sprite` dentro de la lista `Skins`. El método realiza validaciones para asegurar que el índice sea válido y que el sprite exista. Si el índice es válido, toma la textura del sprite seleccionado y la asigna a la `mainTexture` del material del `Renderer` del objeto, cambiando así su apariencia visual.

- **Lógica Clave:**
    La lógica central del script reside en cómo `UpdateAnimation` gestiona la actualización de parámetros del `Animator`. En lugar de requerir que el llamador sepa el tipo específico de cada parámetro (por ejemplo, `SetBool`, `SetFloat`), este método encapsula esa complejidad. Itera a través de todos los parámetros definidos en el controlador del `Animator`, compara el `variableName` proporcionado, y una vez que encuentra una coincidencia, utiliza un `switch` para determinar el tipo de parámetro (`Bool`, `Float`, `Int`, `Trigger`). Luego, intenta convertir el `string` `value` al tipo de dato correspondiente usando métodos `TryParse` para mayor robustez, y finalmente llama al método `Set` apropiado del `Animator`.
    ```csharp
    // Fragmento de UpdateAnimation mostrando la lógica de detección de tipo
    foreach (AnimatorControllerParameter param in animator.parameters)
    {
        if (param.name == variableName)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    // ... parse y SetBool
                    break;
                // ... otros tipos
            }
            return;
        }
    }
    ```
    La función `SetSkin` también encapsula la lógica para cambiar la apariencia del objeto de manera segura. Primero, verifica la validez del índice y la existencia del sprite para evitar errores en tiempo de ejecución. Luego, extrae la textura del `Sprite` seleccionado y la aplica directamente al `mainTexture` del material del renderizador.
    ```csharp
    // Fragmento de SetSkin
    if (objRenderer != null && Skins[index] != null)
    {
        Texture2D texture = Skins[index].texture;
        objRenderer.material.mainTexture = texture;
    }
    ```

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, pero funcionalmente requiere que el `GameObject` al que está adjunto (o uno de sus hijos) tenga un componente `Animator` y un componente `Renderer` (como `SpriteRenderer`, `MeshRenderer`, etc.) para operar correctamente. Si estos componentes no están presentes, las referencias en `Awake` serán nulas y las llamadas subsiguientes a `UpdateAnimation` o `SetSkin` no tendrán efecto o generarán advertencias.
- **Eventos (Entrada):** Este script no se suscribe directamente a eventos de Unity como clics de botones o colisiones. En su lugar, sus métodos públicos (`UpdateAnimation` y `SetSkin`) están diseñados para ser invocados desde otros scripts o sistemas en el juego que necesiten controlar la animación o la apariencia del objeto.
- **Eventos (Salida):** Este script no invoca ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios en su estado o acciones realizadas. Simplemente modifica el estado del `Animator` y el `Renderer` directamente.
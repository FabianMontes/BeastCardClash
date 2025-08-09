# `MeshAnimation.cs`

## 1. Propósito General

Este script es un componente versátil diseñado para gestionar y controlar la animación y la apariencia visual (skins) de un `GameObject` en Unity. Actúa como un intermediario para interactuar con el componente `Animator` y el `Renderer` adjunto, permitiendo la actualización dinámica de parámetros de animación y el cambio de texturas.

## 2. Componentes Clave

### `MeshAnimation`

*   **Descripción:** `MeshAnimation` es un script de Unity que hereda de `MonoBehaviour`, lo que significa que puede ser adjuntado a cualquier `GameObject` en la escena. Su propósito principal es encapsular la lógica para manipular el `Animator` y el `Renderer` del objeto al que está adjunto, o de un hijo, permitiendo así controlar animaciones y aplicar diferentes texturas o "skins" programáticamente.

*   **Variables Públicas / Serializadas:**

    *   `[SerializeField] private List<Sprite> Skins;`
        Esta es una lista de objetos `Sprite` que se puede configurar directamente desde el Inspector de Unity. Cada `Sprite` en esta lista representa una posible "skin" o textura alternativa que el objeto puede adoptar. El script utiliza la textura subyacente de estos `Sprites` para aplicarla al material del `Renderer` del objeto.

    *   `private Animator animator;`
        Una referencia al componente `Animator` que se espera esté en el mismo `GameObject` que este script. Es el componente central para controlar las transiciones y estados de animación.

    *   `private Renderer objRenderer;`
        Una referencia al componente `Renderer` (por ejemplo, `SpriteRenderer`, `MeshRenderer`) que es responsable de dibujar el objeto en pantalla. Se busca en el mismo `GameObject` o en sus hijos. Este `Renderer` es utilizado para cambiar la textura principal del material del objeto.

*   **Métodos Principales:**

    *   `private void Awake()`:
        Este es un método del ciclo de vida de Unity que se ejecuta cuando la instancia del script es cargada. Su función es inicializar las referencias a los componentes `Animator` y `Renderer`. Busca el componente `Animator` directamente en el `GameObject` al que está adjunto el script (`GetComponent<Animator>()`) y busca un `Renderer` en el `GameObject` o en cualquiera de sus hijos (`GetComponentInChildren<Renderer>()`). Es crucial para asegurar que el script tenga acceso a estos componentes antes de que se intente utilizarlos.

    *   `public void UpdateAnimation(string variableName, string value)`:
        Este método público permite actualizar un parámetro del `Animator` de forma genérica. Recibe el nombre del parámetro como una cadena (`variableName`) y su nuevo valor también como una cadena (`value`).

        *   **Parámetros:**
            *   `variableName` (string): El nombre del parámetro del `Animator` que se desea modificar (por ejemplo, "IsRunning", "Speed", "JumpCount", "TriggerAttack").
            *   `value` (string): El valor que se desea asignar al parámetro. Este valor se intentará convertir automáticamente al tipo de dato correcto del parámetro (booleano, flotante, entero) o simplemente activará un "Trigger".

        *   **Retorno:** `void`. No devuelve ningún valor.

        Este método es fundamental para desacoplar la lógica de animación de las llamadas directas al `Animator`, permitiendo que otros sistemas del juego controlen las animaciones de manera flexible sin conocer el tipo específico de cada parámetro. Si el parámetro no se encuentra, se registra una advertencia en la consola.

    *   `public void SetSkin(int index)`:
        Este método público es responsable de cambiar la apariencia visual del objeto aplicando una nueva textura. Utiliza la lista `Skins` serializada en el Inspector.

        *   **Parámetros:**
            *   `index` (int): El índice numérico de la `Sprite` dentro de la lista `Skins` que se desea aplicar como nueva textura.

        *   **Retorno:** `void`. No devuelve ningún valor.

        Antes de aplicar la skin, el método realiza validaciones para asegurar que el índice sea válido y que la lista `Skins` y el `objRenderer` existan. Si el índice es válido, toma la textura del `Sprite` correspondiente y la asigna a la `mainTexture` del material del `objRenderer`. Esto es útil para cambiar la apariencia de un personaje o un objeto en tiempo de ejecución, por ejemplo, para variar el animal o la facultad representada.

*   **Lógica Clave:**

    La lógica central del script reside en cómo `UpdateAnimation` maneja la actualización de los parámetros del `Animator`. En lugar de tener múltiples métodos (`SetBoolAnimation`, `SetFloatAnimation`, etc.), `UpdateAnimation` itera a través de los parámetros existentes en el `AnimatorController`. Una vez que encuentra una coincidencia por nombre, utiliza una sentencia `switch` para determinar el tipo de parámetro (`Bool`, `Float`, `Int`, `Trigger`) y luego intenta convertir la cadena `value` al tipo apropiado antes de llamar al método `Set` correspondiente del `Animator`. Esta implementación es un ejemplo de diseño flexible que reduce la necesidad de código repetitivo y permite una interfaz unificada para el control de animaciones. `SetSkin` por su parte, es directa, enfocándose en la asignación de texturas basadas en una colección predefinida de `Sprites`.

## 3. Dependencias y Eventos

*   **Componentes Requeridos:**
    Aunque el script no utiliza el atributo `[RequireComponent]`, para su correcto funcionamiento en tiempo de ejecución, se espera que el `GameObject` al que se adjunta `MeshAnimation` (o uno de sus hijos) contenga un componente `Animator` y un componente `Renderer` (como `SpriteRenderer` o `MeshRenderer`). Sin estos, las referencias `animator` y `objRenderer` inicializadas en `Awake` serán nulas, y las funcionalidades de control de animación y cambio de skin no operarán.

*   **Eventos (Entrada):**
    Este script no se suscribe explícitamente a eventos de Unity (como `button.onClick` o eventos personalizados) dentro del código proporcionado. Sus métodos públicos (`UpdateAnimation`, `SetSkin`) están diseñados para ser invocados externamente por otros scripts o sistemas (ej. un `GameManager`, un script de `PlayerController`, o un sistema de UI).

*   **Eventos (Salida):**
    El script `MeshAnimation` no invoca ni publica ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios en la animación o en la skin. Su rol es meramente receptivo a las llamadas a sus métodos públicos. Las advertencias de depuración (`Debug.LogWarning`) son las únicas "salidas" de información observable, utilizadas para indicar problemas internos, no como un sistema de notificación.
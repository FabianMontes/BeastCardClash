Aquí tienes la documentación técnica para el script `FolowerText.cs`.

---

# `FolowerText.cs`

## 1. Propósito General
Este script gestiona la visualización dinámica de texto en un componente `TextMeshProUGUI`. Su rol principal es mostrar información específica, como el nombre de la especie o los puntos de vida de un `Figther` (luchador), asegurando que el texto se actualice cuando los datos subyacentes cambian. Interactúa principalmente con los sistemas de UI (`TextMeshProUGUI`) y de lógica de juego (`Figther`).

## 2. Componentes Clave

### `enum TypeFollow`
-   **Descripción:** Este enumerado define las dos categorías de información que el script `FolowerText` puede rastrear y mostrar. Es un mecanismo de configuración que permite especificar si el texto debe seguir la especie o los puntos de vida del luchador.
-   **Valores:**
    -   `species`: Indica que el texto debe mostrar el nombre de la especie del `Figther`.
    -   `live`: Indica que el texto debe mostrar los puntos de vida actuales del `Figther`.

### `class FolowerText : MonoBehaviour`
-   **Descripción:** `FolowerText` es un script de Unity que, al adjuntarse a un GameObject que también contiene un componente `TextMeshProUGUI`, se encarga de mostrar información de un `Figther` encontrado en un GameObject padre. Permite que elementos de la interfaz de usuario reflejen de manera reactiva datos clave del juego.
-   **Variables Públicas / Serializadas:**
    -   `[SerializeField] TypeFollow typeFollow`: Una variable serializada que aparece en el Inspector de Unity. Su valor determina qué dato específico (especie o vida) del `Figther` el script debe obtener y mostrar. Permite al diseñador o desarrollador configurar fácilmente el comportamiento de cada instancia de texto.
-   **Métodos Principales:**
    -   `void Start()`:
        -   **Descripción:** Este método del ciclo de vida de Unity se llama una vez al inicio, después de que el objeto es instanciado y habilitado. Su función es inicializar las referencias necesarias y establecer el texto inicial.
        -   **Lógica Clave:** Primero, obtiene una referencia al componente `TextMeshProUGUI` adjunto al mismo GameObject. Luego, busca un componente `Figther` en el árbol de jerarquía de los GameObjects padres (`GetComponentInParent<Figther>()`). Si encuentra un `Figther`, utiliza la configuración de `typeFollow` para determinar si debe mostrar el nombre de la especie (`player.GetSpecie().ToString()`) o los puntos de vida iniciales (`player.GetPlayerLive().ToString()`). Finalmente, actualiza el texto del componente `TextMeshProUGUI` con el valor obtenido.
    -   `void Update()`:
        -   **Descripción:** Este método del ciclo de vida de Unity se invoca una vez por cada frame del juego. Su propósito es mantener el texto actualizado en tiempo real si el `FolowerText` está configurado para mostrar los puntos de vida.
        -   **Lógica Clave:** Contiene una lógica condicional sencilla: solo si `typeFollow` está configurado en `TypeFollow.live`, el texto del `TextMeshProUGUI` se actualizará con los puntos de vida actuales del `Figther` (`player.GetPlayerLive().ToString()`). Esto garantiza que el medidor de vida del jugador se refleje dinámicamente en la UI sin afectar el rendimiento con actualizaciones innecesarias para otros tipos de texto (como el de especie, que no cambia).

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Aunque no se utiliza el atributo `[RequireComponent]`, este script asume la existencia de un `TextMeshProUGUI` en el mismo GameObject y un componente `Figther` en uno de sus GameObjects padres para funcionar correctamente. Sin estos componentes, el script no podrá inicializar sus referencias y la funcionalidad de visualización de texto no operará.
-   **Eventos (Entrada):** Este script no se suscribe explícitamente a eventos personalizados (como `UnityEvent` o `Action`). Su funcionamiento se basa enteramente en los métodos de ciclo de vida de Unity (`Start`, `Update`).
-   **Eventos (Salida):** El script `FolowerText` no invoca ningún evento ni notifica a otros sistemas. Su función es puramente de visualización; no emite señales ni dispara acciones en otras partes del juego.

---
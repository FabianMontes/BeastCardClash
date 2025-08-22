# `PickerCard.cs`

## 1. Propósito General
Este script `PickerCard` es un componente de UI que gestiona la visualización de la carta que un jugador (`Figther`) ha "seleccionado" o "recogido". Su función principal es mantener sincronizada la interfaz visual de una carta (`HandCard`) con el estado interno de la carta seleccionada por el jugador.

## 2. Componentes Clave

### `PickerCard`
- **Descripción:** La clase `PickerCard` es un `MonoBehaviour` que se adjunta a un objeto de juego en la jerarquía de Unity. Su rol es servir como un puente entre la lógica de selección de cartas de un `Figther` y su representación visual en la interfaz de usuario. Al inicio, busca un componente `HandCard` en sus hijos y un componente `Figther` en sus padres para establecer las referencias necesarias.

- **Variables Públicas / Serializadas:**
    Aunque no hay variables públicas ni serializadas explícitamente con `[SerializeField]`, el script utiliza las siguientes variables privadas para su funcionamiento interno:
    - `HandCard card`: Una referencia al componente `HandCard` que se encuentra como hijo del GameObject al que está adjunto este script. Este `HandCard` es el que se actualizará para mostrar la carta seleccionada.
    - `Figther player`: Una referencia al componente `Figther` que se encuentra en un GameObject padre. Este `Figther` es la fuente de información sobre la carta actualmente seleccionada por el jugador.
    - `bool isPlaying`: Una bandera interna que rastrea si el `HandCard` está actualmente mostrando una carta (es decir, si el `player` ha "recogido" una carta). Se utiliza para optimizar las actualizaciones y solo llamar a `SetCard` cuando hay un cambio en el estado de selección.

- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se llama una vez al inicio del juego, antes del primer frame. Se utiliza para inicializar las referencias a los componentes `HandCard` y `Figther`.
        ```csharp
        card = GetComponentInChildren<HandCard>();
        player = GetComponentInParent<Figther>();
        card.SetCard(null);
        ```
        Aquí, `GetComponentInChildren<HandCard>()` busca el componente `HandCard` en los hijos del objeto actual, y `GetComponentInParent<Figther>()` busca el `Figther` en los padres. Finalmente, `card.SetCard(null)` asegura que la interfaz de la carta seleccionada esté vacía al inicio.

    - `void Update()`: Este método del ciclo de vida de Unity se llama una vez por frame. Su propósito es monitorear continuamente el estado de la carta seleccionada por el `Figther` y actualizar la visualización de `HandCard` cuando sea necesario.
        ```csharp
        if(isPlaying != (player.getPicked() != null))
        {
            isPlaying = player.getPicked() != null;
            card.SetCard(player.getPicked());
        }
        ```

- **Lógica Clave:**
    La lógica central reside en el método `Update`, donde se implementa una verificación de estado para optimizar las actualizaciones. En lugar de llamar `card.SetCard()` en cada frame, el script compara el estado actual de `isPlaying` (si una carta está siendo mostrada) con el estado de la carta "recogida" por el jugador (`player.getPicked() != null`). Si estos estados difieren, significa que ha habido un cambio (una carta ha sido seleccionada o deseleccionada), y solo entonces se actualiza `isPlaying` y se llama a `card.SetCard()` con la nueva carta (o `null` si no hay ninguna). Esto evita actualizaciones innecesarias de la UI, mejorando el rendimiento.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere la presencia de un componente `HandCard` en uno de sus objetos hijos y un componente `Figther` en uno de sus objetos padres para operar correctamente, ya que busca estas referencias al inicio.

- **Eventos (Entrada):** Este script no se suscribe directamente a eventos de Unity (`UnityEvent`) ni a acciones personalizadas. En cambio, opera mediante un modelo de "polling", donde consulta el estado del `Figther` (`player.getPicked()`) en cada frame a través del método `Update`.

- **Eventos (Salida):** El `PickerCard` no invoca ni expone ningún evento (como `UnityEvent` o `Action`) para notificar a otros sistemas sobre cambios. Su principal "salida" es la manipulación directa del componente `HandCard` para actualizar su visualización.
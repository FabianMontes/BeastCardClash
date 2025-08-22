# `CardValue.cs`

## 1. Propósito General
Este script se encarga de la representación visual del valor numérico de una carta en la interfaz de usuario del juego. Su rol principal es mantener el texto de un elemento UI sincronizado con el valor actual de la carta a la que está asociado, interactuando directamente con el sistema de cartas de mano (`HandCard`).

## 2. Componentes Clave

### `CardValue`
- **Descripción:** `CardValue` es un `MonoBehaviour` que gestiona la actualización dinámica de un componente de texto (`TextMeshProUGUI`) para mostrar el valor de una carta. Se espera que este script esté adjunto a un GameObject que contenga un `TextMeshProUGUI` y que sea hijo de un GameObject que a su vez tenga el componente `HandCard`.
- **Variables Públicas / Serializadas:**
    - `HandCard card`: Una referencia privada al componente `HandCard` que se encuentra en un GameObject padre. Esta referencia es fundamental, ya que permite acceder al objeto `Card` real cuya información (específicamente su valor) necesita ser mostrada.
    - `TextMeshProUGUI textMeshPro`: Una referencia privada al componente `TextMeshProUGUI` adjunto al mismo GameObject donde reside este script. Este componente es el encargado de renderizar el valor numérico de la carta como texto en la UI.
- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se ejecuta una vez al inicio. Su función es inicializar las referencias necesarias. Busca el componente `HandCard` en los objetos padre del GameObject actual y obtiene el componente `TextMeshProUGUI` del propio GameObject al que está adjunto `CardValue`.
    - `void Update()`: Este método del ciclo de vida de Unity se llama en cada frame. Contiene la lógica principal para mantener el texto actualizado. Primero, verifica si la `HandCard` asociada tiene una carta asignada (`card.GetCard() == null`). Si no hay carta, el texto se limpia. Si hay una carta presente, recupera su valor (`card.GetCard().GetValue()`) y actualiza el contenido del `textMeshPro` para mostrar dicho valor.
- **Lógica Clave:** La lógica central del script reside en el método `Update`, que garantiza una sincronización constante entre el valor numérico de la carta y su representación visual en la UI. Este método maneja eficientemente los escenarios donde una carta puede no estar presente, asegurando que la interfaz de usuario se mantenga limpia y sin errores al ocultar el valor.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script funcionalmente requiere un componente `TextMeshProUGUI` en el mismo GameObject para que la inicialización en `Start` sea exitosa. Además, depende de la existencia de un componente `HandCard` en un GameObject padre para poder obtener la información de la carta. Aunque no se utiliza el atributo `[RequireComponent]`, estas son dependencias implícitas para su correcto funcionamiento.
- **Eventos (Entrada):** `CardValue` no se suscribe explícitamente a eventos personalizados de Unity (`UnityEvent`) o delegados de C# (`Action`). Opera utilizando los métodos de ciclo de vida estándar de Unity (`Start` y `Update`).
- **Eventos (Salida):** Este script no invoca ni emite ningún evento personalizado para notificar a otros sistemas. Su función es puramente de visualización y depende de consultar la información de la `HandCard`.
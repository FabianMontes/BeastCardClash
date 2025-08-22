# `MenuTexts.cs`

## 1. Propósito General
Este script define un `ScriptableObject` cuyo propósito principal es almacenar y gestionar los textos de la interfaz de usuario para los menús del juego, específicamente para diferentes idiomas. Actúa como un contenedor de datos centralizado para las cadenas de texto utilizadas en elementos como botones y etiquetas, facilitando la localización.

## 2. Componentes Clave

### `MenuTexts`
- **Descripción:** La clase `MenuTexts` hereda de `ScriptableObject`, lo que le permite ser un activo (asset) que puede ser creado y configurado directamente en el editor de Unity, independientemente de cualquier `GameObject`. Su función es servir como un repositorio de cadenas de texto localizables para los elementos de la UI del menú, organizadas por idioma. El atributo `[CreateAssetMenu]` permite crear instancias de esta clase a través del menú `Assets -> Create -> Localizations/Menu Texts` en el editor.

- **Variables Públicas / Serializadas:**
  La clase contiene una serie de variables `public string` que son visibles en el Inspector de Unity y pueden ser asignadas con los textos correspondientes. El uso del atributo `[Header]` organiza estas variables en secciones "Spanish" e "English" dentro del Inspector para una mejor legibilidad.
    - `startButton_es`: Almacena el texto para el botón de inicio en español, inicialmente "Iniciar".
    - `creditsButton_es`: Almacena el texto para el botón de créditos en español, inicialmente "Créditos".
    - `languagesLabel_es`: Almacena el texto para la etiqueta de idiomas en español, inicialmente "Idiomas".
    - `startButton_en`: Almacena el texto para el botón de inicio en inglés, inicialmente "Start".
    - `creditsButton_en`: Almacena el texto para el botón de créditos en inglés, inicialmente "Start" (nótese que en el código proporcionado, este valor es "Start" en lugar de "Credits").
    - `languagesLabel_en`: Almacena el texto para la etiqueta de idiomas en inglés, inicialmente "Languages".

  Ejemplo de definición de variables:
  ```csharp
  [Header("Spanish")]
  public string startButton_es = "Iniciar";
  // ...
  [Header("English")]
  public string startButton_en = "Start";
  ```

- **Métodos Principales:**
  No se definen métodos personalizados (`void` o con retorno) dentro de esta clase. `MenuTexts` es puramente una clase de datos, y su acceso y uso se realizan a través de la lectura de sus propiedades públicas por otros scripts.

- **Lógica Clave:**
  La lógica fundamental reside en el patrón de diseño de `ScriptableObject`. Al ser un `ScriptableObject`, se puede crear un único activo `MenuTexts` en el proyecto de Unity que contenga todas las cadenas de texto para ambos idiomas. Otros scripts de UI encargados de mostrar textos pueden entonces referenciar este activo y acceder a las propiedades `startButton_es`, `startButton_en`, etc., según el idioma seleccionado por el jugador. Esto desacopla los datos de localización del código que los utiliza, facilitando la gestión y actualización de textos.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
  Este script no utiliza el atributo `[RequireComponent]` ya que, como `ScriptableObject`, no se adjunta a `GameObjects` y, por lo tanto, no requiere otros componentes de Unity para funcionar.

- **Eventos (Entrada):**
  La clase `MenuTexts` no se suscribe a ningún evento de Unity o del sistema (como `button.onClick` o eventos de entrada del usuario). Su rol es estrictamente pasivo como proveedor de datos.

- **Eventos (Salida):**
  Este script no invoca ni emite ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre cambios o acciones. Simplemente expone sus datos para ser consultados por otros componentes del juego.
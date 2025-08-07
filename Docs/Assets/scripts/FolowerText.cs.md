Aquí tienes la documentación técnica para el archivo `FolowerText.cs`, diseñada para un nuevo miembro del equipo.

---

# `FolowerText.cs`

## 1. Propósito General

Este script de Unity es responsable de mostrar dinámicamente información textual de un personaje (`Figther`) en una interfaz de usuario. Principalmente, actualiza el texto de un componente `TextMeshProUGUI` para reflejar la especie o los puntos de vida de un `Figther` asociado, lo que lo hace útil para elementos de HUD o barras de estado.

## 2. Componentes Clave

### `enum TypeFollow`

Este `enum` define los tipos de datos que el script `FolowerText` puede rastrear y mostrar. Permite al diseñador elegir qué información del `Figther` debe reflejar el componente de texto.

```csharp
enum TypeFollow
{
    species, // Para mostrar la especie del Figther.
    live     // Para mostrar los puntos de vida actuales del Figther.
}
```

### `FolowerText`

La clase `FolowerText` es un componente de Unity (`MonoBehaviour`) que gestiona la visualización del texto. Se espera que se adjunte a un objeto de juego que también contenga un componente `TextMeshProUGUI`, y que se encuentre como hijo de un objeto que contenga un componente `Figther`.

#### Variables Públicas / Serializadas

*   `[SerializeField] TypeFollow typeFollow;`
    Esta variable serializada, configurable directamente desde el Inspector de Unity, determina qué tipo de información del `Figther` (especie o puntos de vida) debe mostrar el objeto `TextMeshProUGUI` al que está asociado este script. Su valor inicial es crucial para el comportamiento del script.

#### Métodos Principales

*   `void Start()`
    Este método del ciclo de vida de Unity se ejecuta una vez antes de la primera actualización del frame, después de que el objeto se ha inicializado. En `Start`, el script realiza las siguientes operaciones cruciales:
    1.  Obtiene una referencia al componente `TextMeshProUGUI` adjunto al mismo objeto de juego. Este es el componente que renderizará el texto.
    2.  Busca y obtiene una referencia al componente `Figther` en el objeto padre o en cualquier ancestro del objeto actual. Esto es fundamental para acceder a los datos del personaje.
    3.  Si se encuentra un `Figther`, utiliza el valor de `typeFollow` para determinar qué información inicial se debe mostrar:
        *   Si `typeFollow` es `TypeFollow.species`, el texto se inicializa con el nombre de la especie del `Figther` (obtenido a través de `player.GetSpecie().ToString()`).
        *   Si `typeFollow` es `TypeFollow.live`, el texto se inicializa con los puntos de vida actuales del `Figther` (obtenidos a través de `player.GetPlayerLive().ToString()`).
    4.  Finalmente, actualiza el texto del componente `TextMeshProUGUI` con la información obtenida.

*   `void Update()`
    Este método del ciclo de vida de Unity se llama una vez por cada frame. Su propósito principal es mantener el texto de los puntos de vida actualizado en tiempo real.
    1.  Comprueba si `typeFollow` está configurado en `TypeFollow.live`.
    2.  Si es así, el script actualiza continuamente el texto del `TextMeshProUGUI` para reflejar los puntos de vida actuales del `Figther` (`player.GetPlayerLive().ToString()`).
    Es importante notar que si `typeFollow` está configurado como `TypeFollow.species`, este método no realizará ninguna acción, lo que significa que el texto de la especie solo se establece una vez en `Start` y no se actualiza dinámicamente.

#### Lógica Clave

La lógica central de este script reside en su capacidad para actuar como un "observador" de los datos de un `Figther` y proyectarlos en un elemento de la interfaz de usuario. Utiliza la variable `typeFollow` para bifurcar su comportamiento: inicializa el texto según el tipo especificado en `Start`, y si el tipo es `live`, lo actualiza continuamente en `Update`. Esto permite tener etiquetas estáticas (como la especie) y etiquetas dinámicas (como la vida) usando el mismo componente. La dependencia de `GetComponentInParent<Figther>()` es crucial, ya que establece la relación jerárquica esperada entre el elemento de texto y el personaje `Figther` al que pertenece la información.

## 3. Dependencias y Eventos

*   **Componentes Requeridos:**
    Este script requiere la presencia de un componente `TextMeshProUGUI` en el mismo GameObject para poder mostrar el texto. No hay un atributo `[RequireComponent]` explícito en el código, pero es una dependencia implícita para su correcto funcionamiento.
    Además, depende de la existencia de un componente `Figther` en uno de sus objetos padre para poder acceder a los datos relevantes (especie, puntos de vida).

*   **Eventos (Entrada):**
    Este script no se suscribe directamente a eventos de Unity (`UnityEvent`) ni a acciones personalizadas (`Action`). Su activación y actualización se gestionan a través de los métodos del ciclo de vida de `MonoBehaviour` (`Start`, `Update`).

*   **Eventos (Salida):**
    Este script no invoca ningún evento (`UnityEvent`, `Action`) para notificar a otros sistemas sobre cambios o acciones. Su rol es puramente de visualización de datos.

---
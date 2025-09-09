# followShield
Este script `followShield` es un componente de MonoBehaviour que gestiona la visualización de un escudo o emblema asociado a una entidad "Figther" en el juego. Su principal función es inicializar y asignar el sprite de un componente `Image` hijo, basándose en el equipo al que pertenece el `Figther` que lo contiene o es su padre. Actúa como un elemento visual de feedback que identifica rápidamente la afiliación de un `Figther`.

El script interactúa con otros componentes de la siguiente manera:
*   **`Figther`**: Busca este componente en un objeto padre (`GetComponentInParent<Figther>()`). Esto implica que el GameObject al que se adjunta `followShield` es un hijo de un GameObject que posee un componente `Figther`, o es un GameObject que, a su vez, es hijo de uno con `Figther`. Utiliza el método `GetTeam()` de `Figther` para determinar el equipo.
*   **`Image`**: Busca este componente en un objeto hijo (`GetComponentInChildren<Image>()`). Esto significa que dentro de la jerarquía del GameObject con `followShield` debe haber un GameObject hijo que tenga un componente `Image` (típicamente un elemento de UI en un Canvas). Este `Image` es el que se actualizará con el sprite del escudo.

El array `shields` se expone en el Inspector de Unity gracias a `[SerializeField]`, permitiendo que los diseñadores o artistas asignen fácilmente los diferentes sprites de escudos correspondientes a los equipos del juego.

# Métodos

## Métodos de Unity

### Start
Este método se invoca una vez al inicio, justo antes de la primera actualización del frame. Su propósito es configurar los elementos necesarios para el funcionamiento del escudo al cargar la escena o instanciar el objeto.

1.  **Obtención del componente `Figther` padre**:
    Se intenta localizar y obtener una referencia al componente `Figther` que se encuentre en alguno de los GameObjects padre en la jerarquía.
    ```csharp
    figther = GetComponentInParent<Figther>();
    ```
    Esta línea es crucial, ya que establece la conexión entre el escudo y la entidad `Figther` a la que representa. Si no se encuentra un componente `Figther` en la jerarquía superior, la variable `figther` quedará nula, lo que podría generar errores en tiempo de ejecución si no se maneja adecuadamente (aunque el código actual asume que siempre se encontrará).

2.  **Obtención del componente `Image` hijo**:
    Se busca el componente `Image` en los GameObjects hijos de este objeto.
    ```csharp
    image = GetComponentInChildren<Image>();
    ```
    Este `Image` es el elemento visual que mostrará el sprite del escudo. Es fundamental que exista un `Image` como hijo del GameObject con `followShield` para que el script pueda funcionar correctamente.

3.  **Asignación del sprite del escudo**:
    Una vez obtenidos el `Figther` y la `Image`, se asigna el sprite adecuado a la `Image`.
    ```csharp
    image.sprite = shields[(int) figther.GetTeam()];
    ```
    *   `figther.GetTeam()`: Se llama al método `GetTeam()` del componente `Figther` para obtener el equipo al que pertenece. Se asume que `GetTeam()` devuelve un valor (probablemente un `enum` o un `int`) que representa el equipo.
    *   `(int) ...`: El valor devuelto por `GetTeam()` se convierte explícitamente a un entero. Esto sugiere que los índices del array `shields` corresponden directamente a los valores numéricos de los equipos (por ejemplo, `Team.Red` podría ser `0`, `Team.Blue` `1`, etc.).
    *   `shields[...]`: Se utiliza el entero resultante como índice para seleccionar un sprite del array `shields`.
    *   `image.sprite = ...`: El sprite seleccionado se asigna a la propiedad `sprite` del componente `Image`, haciendo que el escudo visual cambie para reflejar el equipo del `Figther`.

### Update
Este método se invoca una vez por cada frame.

En su estado actual, el método `Update` está vacío:
```csharp
void Update()
{
    
}
```
Esto indica que el script `followShield` realiza su configuración inicial una única vez en `Start` y no requiere de lógica continua por frame para su propósito actual. Si en el futuro fuera necesario que el escudo reaccionara a cambios dinámicos (por ejemplo, parpadear si el `Figther` está en un estado específico o cambiar de sprite si el equipo del `Figther` pudiera variar en tiempo real), esta sería la sección donde se implementaría dicha lógica.
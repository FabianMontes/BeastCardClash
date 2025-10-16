# `followShield`
El script `followShield` es un componente de Unity diseñado para gestionar la representación visual de un "escudo" o distintivo de equipo para una entidad `Fighter` dentro del juego. Su función principal es inicializar el sprite de este distintivo basándose en el equipo al que pertenece el `Fighter` asociado. Este componente está pensado para ser parte de una jerarquía de GameObjects, donde el `Fighter` reside en un GameObject padre y el distintivo visual (una `Image` de UI) se encuentra en un GameObject hijo.

Al iniciar, el script localiza el componente `Fighter` en su GameObject padre y el componente `Image` en uno de sus GameObjects hijos. Utiliza el método `GetTeam()` del `Fighter` para determinar el equipo y, en base a esto, selecciona el `Sprite` apropiado de un array preconfigurado (`shields`) para asignarlo a la `Image`. Esto asegura que cada `Fighter` muestre visualmente su afiliación de equipo desde el comienzo. El nombre del script sugiere un comportamiento de "seguimiento", sin embargo, dado que el método `Update` está vacío, el "seguimiento" se gestiona implícitamente a través de la relación padre-hijo en la jerarquía de Unity, donde el distintivo se mueve junto con su padre `Fighter`.

# Métodos

## Métodos de Unity

### `Start()`
El método `Start()` se ejecuta una única vez en el ciclo de vida del script, justo antes de la primera actualización del frame, una vez que el MonoBehaviour ha sido creado. Su propósito es inicializar las referencias necesarias y configurar el sprite del escudo.

Dentro de este método:
1. Se obtiene una referencia al componente `Fighter` del GameObject padre utilizando `GetComponentInParent<Fighter>()`. Esto implica que el GameObject al que está adjunto `followShield` debe ser un hijo de un GameObject que contenga un componente `Fighter`.
   ```csharp
   figther = GetComponentInParent<Fighter>();
   ```
2. Se obtiene una referencia al componente `Image` que se encuentra en uno de los GameObjects hijos de este mismo GameObject, utilizando `GetComponentInChildren<Image>()`. Esto sugiere que el distintivo visual es un elemento de UI (`Image`) contenido en un GameObject anidado.
   ```csharp
   image = GetComponentInChildren<Image>();
   ```
3. Finalmente, se asigna el sprite correcto a la `Image`. El índice para seleccionar el sprite del array `shields` se obtiene a partir del valor de retorno del método `GetTeam()` del `Fighter` asociado, el cual se convierte explícitamente a un entero. Esto indica que `GetTeam()` probablemente devuelve un `enum` que representa los diferentes equipos.
   ```csharp
   image.sprite = shields[(int)figther.GetTeam()];
   ```
Este proceso asegura que, al iniciar la escena, cada `Fighter` tenga el sprite de escudo correspondiente a su equipo asignado y visible.

### `Update()`
El método `Update()` se invoca una vez por cada frame del juego. En el script `followShield`, este método se encuentra vacío:
```csharp
void Update()
{

}
```
Esto indica que no hay lógica de actualización continua implementada en este script. El "seguimiento" al que hace referencia el nombre del script no se gestiona mediante un movimiento activo en cada frame, sino que se asume que el GameObject que contiene la `Image` del escudo es un hijo del GameObject `Fighter`, lo que garantiza que el escudo se mueva y rote junto con su padre de forma inherente a la jerarquía de transformación de Unity.

## Otros métodos
Este script no define métodos adicionales que no sean parte del ciclo de vida de Unity (`Start`, `Update`).

## Getters y Setters
El script `followShield` no implementa getters o setters personalizados a través de propiedades públicas o métodos. Contiene un campo serializado (`shields`) que es accesible y configurable directamente desde el Inspector de Unity:

1. `shields`: Array de tipo `Sprite[]`. Este campo serializado permite asignar una colección de `Sprite` assets directamente desde el Editor de Unity, los cuales representan los diferentes diseños de escudos disponibles para los equipos.
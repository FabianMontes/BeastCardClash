# combatfollower
El script `combatfollower` es un componente de Unity diseñado para actualizar visualmente un objeto `Image` basándose en el tipo de combate o estado actual del juego. Su función principal es servir como un indicador visual dinámico que refleja el estado interno del sistema de combate, proporcionando retroalimentación instantánea al jugador dentro de **Beast Card Clash**.

Este script se encarga de:
1.  Obtener una referencia al componente `Image` en el mismo GameObject al que está adjunto.
2.  Mantener una colección de `Sprite`s que representan los diferentes "tipos de combate" o estados posibles.
3.  Actualizar continuamente el `sprite` del componente `Image` para que coincida con el `CombatType` actual, que es determinado por la clase `CombatJudge`.

La simplicidad y el enfoque directo de este script se alinean con la filosofía del proyecto de priorizar una buena experiencia de desarrollo y de jugador, permitiendo una rápida implementación de indicadores visuales sin complejidades innecesarias.

# Métodos

## Métodos de Unity

### Start()
El método `Start` se ejecuta una vez al inicio, justo antes de la primera actualización del frame, siempre y cuando el `MonoBehaviour` esté habilitado.

En `combatfollower`, este método inicializa la referencia al componente `Image` que reside en el mismo GameObject que este script. Esto asegura que el script pueda manipular la imagen sin tener que buscarla en cada frame.

```csharp
void Start()
{
    image = GetComponent<Image>();
}
```

La referencia a `image` es crucial para que el script pueda cambiar el `sprite` que se muestra en la interfaz de usuario o en el mundo del juego. Si el GameObject no tiene un componente `Image` adjunto, `image` será `null`, lo que podría causar errores si no se maneja adecuadamente (aunque en este contexto, se asume que siempre estará presente).

### Update()
El método `Update` se invoca una vez por frame. Es el corazón del funcionamiento dinámico de `combatfollower`, ya que es responsable de la actualización continua del `sprite` mostrado.

```csharp
void Update()
{
    image.sprite = types[(int)CombatJudge.CombatJudgeInstance.CombatType];
}
```

Dentro de este método:
1.  Se accede a `CombatJudge.CombatJudgeInstance.CombatType`. Esto indica que existe una clase `CombatJudge` que probablemente implementa el patrón Singleton (accesible globalmente a través de `CombatJudgeInstance`) y expone una propiedad `CombatType`.
2.  La propiedad `CombatType` es casteada a un `int`. Esto sugiere fuertemente que `CombatType` es de un tipo `enum`, donde cada miembro del enumerado corresponde a un índice numérico.
3.  Este índice numérico se utiliza para seleccionar un `Sprite` del array `types`. El array `types` se configura en el Inspector de Unity gracias al atributo `[SerializeField]`.

Cada frame, el script verifica el `CombatType` actual y asigna el `Sprite` correspondiente del array `types` al componente `image`. Esto permite que el objeto visual cambie dinámicamente según el estado del combate determinado por `CombatJudge`, como, por ejemplo, diferentes fases de turno, tipos elementales activos o resultados de interacciones.

## Otros métodos
Este script no define métodos adicionales más allá de los métodos de ciclo de vida de Unity (`Start`, `Update`).

## Getters y Setters
Este script no define propiedades públicas con getters o setters explícitos. La variable `types` es un campo serializado (`[SerializeField]`) que se configura directamente desde el Inspector de Unity.
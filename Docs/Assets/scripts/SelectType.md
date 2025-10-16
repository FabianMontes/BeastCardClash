# `SelectType`
Este script, adjunto a un objeto `GameObject` en la jerarquía de Unity, se encarga de gestionar la visibilidad y la interacción de la interfaz de usuario (UI) para la selección de tipos elementales durante las fases específicas del combate. Su principal función es presentarse en el momento adecuado del turno de combate para el jugador principal (`indexFighter == 0`), permitiendo la elección de un "elemento" que impactará en la estrategia del juego, y luego ocultarse una vez realizada la selección o cuando las condiciones de combate cambian. Opera en conjunto con el componente `Fighter` al que está asociado y el sistema central de control de combate (`CombatJudge`).

# Métodos

## Métodos de Unity

### `Start`
Este método se ejecuta una única vez al inicio del ciclo de vida del script, antes del primer `Update`. Su propósito es inicializar las referencias necesarias y establecer el estado inicial de la UI de selección.

```csharp
void Start()
{
    figther = GetComponentInParent<Fighter>();
    Visib(false);
}
```

- **Obtención del Componente `Fighter`**: Se busca y se almacena una referencia al componente `Fighter` que se encuentre en el mismo `GameObject` o en uno de sus padres (`GetComponentInParent`). Esto es crucial para identificar a qué luchador pertenece esta UI de selección y, a su vez, permitirle interactuar con sus propiedades (como `indexFighter`).
- **Ocultamiento Inicial**: Inmediatamente después de obtener la referencia, se invoca el método `Visib(false)` para asegurar que la UI de selección de elementos esté oculta por defecto al inicio del juego.

### `Update`
Este método se llama una vez por cada frame del juego. Su función principal es monitorear continuamente el estado del combate y las condiciones del turno para determinar cuándo la UI de selección de elementos debe ser visible.

```csharp
void Update()
{
    SetMoments momo = CombatJudge.Instance.GetSetMoments();
    if (momo == SetMoments.SelectCombat && CombatJudge.Instance.FocusOnTurn() && figther.indexFighter == 0)
    {
        Visib(true);
    }
}
```

- **Verificación de Condiciones de Combate**: En cada frame, el script consulta al `CombatJudge` (instancia global del sistema de combate) para obtener el `SetMoments` actual del juego.
- **Activación Condicional**: La UI de selección (`Visib(true)`) se activa únicamente si se cumplen las siguientes tres condiciones simultáneamente:
    - El momento actual del combate (`momo`) es `SetMoments.SelectCombat`, lo que indica que el juego está en la fase donde se debe elegir un elemento.
    - `CombatJudge.Instance.FocusOnTurn()` devuelve `true`, señalando que es el turno del jugador que debe realizar una acción.
    - El `indexFighter` del `figther` asociado es `0`, lo que implica que la UI solo debe activarse para el primer jugador (o el jugador principal, en configuraciones de un solo jugador o locales).
- **Ocultamiento Implícito**: Si alguna de estas condiciones deja de cumplirse, el método `Visib(false)` no se llama explícitamente en `Update` para ocultar la UI. Sin embargo, la lógica en `PickElement` o en el `CombatJudge` al cambiar de `SetMoments` se encargaría de ocultarla.

## Otros métodos

### `Visib(bool isVisible)`
Este es un método privado auxiliar que controla la visibilidad de la interfaz de usuario de selección de elementos.

```csharp
private void Visib(bool isVisible)
{
    transform.GetChild(0).gameObject.SetActive(isVisible);
}
```

- **Control de Visibilidad**: Activa o desactiva el primer hijo (`GetChild(0)`) del `GameObject` al que está adjunto el script. Se asume que este primer hijo es el contenedor principal o el panel que agrupa todos los elementos visuales de la UI de selección de tipos.

### `PickElement(int element)`
Este método público está diseñado para ser invocado por eventos de la UI, como el clic de un botón, cuando el jugador selecciona un tipo elemental.

```csharp
public void PickElement(int element)
{
    if (CombatJudge.Instance.PickElement((Element)element)) Visib(false);
}
```

- **Selección de Elemento**: Recibe un `int` que representa el elemento elegido por el jugador. Este valor se convierte explícitamente al tipo `Element` (presumiblemente un `enum` definido en otro lugar del proyecto) y se pasa al método `PickElement` del `CombatJudge`.
- **Interacción con `CombatJudge`**: El `CombatJudge` es responsable de procesar la selección del elemento. Si `CombatJudge.Instance.PickElement()` devuelve `true` (indicando que la selección fue exitosa y aceptada por el sistema de combate), la UI de selección se oculta inmediatamente (`Visib(false)`).

## Getters y Setters

1. `PickElement(int)`: Este método público establece la elección de un tipo elemental en el sistema de combate global a través del `CombatJudge`, y oculta la UI de selección si la elección es válida.
# SelectType
Este script es responsable de gestionar la interfaz de usuario (UI) para la selección de un tipo elemental durante una fase específica del combate. Su función principal es mostrar u ocultar los botones de selección elemental al jugador, basándose en el estado actual del combate y el turno del jugador, y luego comunicar la elección realizada al sistema central de combate. Está diseñado para estar asociado con un `Figther` (personaje de combate) y controlar su respectiva UI de selección.

La lógica del script se centra en detectar cuándo es el momento adecuado para que el jugador (específicamente, el `Figther` con `indexFigther == 0`) elija un elemento, y una vez que se ha hecho una selección, informar al sistema de combate y ocultar la UI nuevamente.

# Métodos

## Métodos de Unity

### Start()
Este método se ejecuta una única vez al inicio del ciclo de vida del script, antes de la primera actualización de `Update()`.

Su propósito es inicializar la referencia al componente `Figther` y asegurar que la UI de selección elemental esté oculta al comienzo.

```csharp
void Start()
{
    figther = GetComponentInParent<Figther>();
    Visib(false);
}
```

*   `figther = GetComponentInParent<Figther>();`: Busca y asigna una referencia al componente `Figther` que se encuentra en uno de los GameObjects padre de este objeto. Esto vincula la instancia de `SelectType` a un personaje específico en el juego.
*   `Visib(false);`: Llama al método `Visib` para establecer la visibilidad de la UI de selección en `false`, asegurando que no se muestre al inicio del juego o de la escena.

### Update()
Este método se ejecuta en cada fotograma del juego. Contiene la lógica principal para determinar si la UI de selección elemental debe ser visible.

```csharp
void Update()
{
    SetMoments momo = CombatJudge.CombatJudgeInstance.GetSetMoments();
    if (momo == SetMoments.SelectCombat && CombatJudge.CombatJudgeInstance.FocusOnTurn() && figther.indexFigther == 0)
    {
        Visib(true);
    }
}
```

*   `SetMoments momo = CombatJudge.CombatJudgeInstance.GetSetMoments();`: Obtiene el momento o fase actual del combate a través de la instancia singleton de `CombatJudge`. La enumeración `SetMoments` define las diferentes etapas de un turno de combate.
*   `if (momo == SetMoments.SelectCombat && CombatJudge.CombatJudgeInstance.FocusOnTurn() && figther.indexFigther == 0)`: Esta condición compleja verifica tres criterios para mostrar la UI:
    *   `momo == SetMoments.SelectCombat`: Confirma que el combate se encuentra en la fase específica donde se espera que el jugador elija un elemento.
    *   `CombatJudge.CombatJudgeInstance.FocusOnTurn()`: Verifica si el turno actual está enfocado en el jugador (o entidad) que esta instancia de `SelectType` representa.
    *   `figther.indexFigther == 0`: Asegura que esta UI de selección solo se active para el `Figther` principal del jugador, asumiendo que `indexFigther == 0` identifica al jugador activo en lugar de un oponente o un compañero.
*   `Visib(true);`: Si todas las condiciones anteriores son verdaderas, se llama al método `Visib` para hacer visible la UI de selección elemental.

## Otros métodos

### `private void Visib(bool isVisible)`
Este método privado controla la visibilidad de la interfaz de usuario de selección elemental.

```csharp
private void Visib(bool isVisible)
{
    transform.GetChild(0).gameObject.SetActive(isVisible);
}
```

*   `transform.GetChild(0).gameObject.SetActive(isVisible);`: Accede al primer objeto hijo del GameObject al que está adjunto este script. Se espera que este primer hijo sea el panel o contenedor que agrupa todos los elementos visuales de la UI de selección (como botones para cada tipo elemental). Su estado `active` se establece en `true` o `false` según el valor del parámetro `isVisible`.

### `public void PickElement(int element)`
Este método público es el punto de entrada para procesar la selección de un elemento por parte del jugador. Probablemente esté conectado a eventos de UI, como los botones de selección de tipo elemental.

```csharp
public void PickElement(int element)
{
    if (CombatJudge.CombatJudgeInstance.PickElement((Element)element)) Visib(false);
}
```

*   `CombatJudge.CombatJudgeInstance.PickElement((Element)element)`: Llama al método `PickElement` del sistema `CombatJudge` para registrar la elección del elemento. El parámetro `element` se pasa como un entero y se castea explícitamente a la enumeración `Element`, lo que implica que cada valor entero corresponde a un tipo elemental diferente (e.g., 0 para Agua, 1 para Fuego, etc.). Este método en `CombatJudge` probablemente maneja la lógica de validación y aplicación del elemento elegido en el combate.
*   `if (...) Visib(false);`: Si el método `PickElement` del `CombatJudge` devuelve `true` (indicando que la selección del elemento fue exitosa y procesada), se llama a `Visib(false)` para ocultar inmediatamente la UI de selección, ya que la elección ha sido realizada.

## Getters y Setters
Este script no define propiedades C# explícitas con los modificadores `get` y `set`. Sin embargo, los siguientes métodos actúan de manera similar en cuanto a establecer valores o modificar el estado:

1.  `public void PickElement(int element)`: Establece el elemento elegido por el jugador en el sistema `CombatJudge`.
2.  `private void Visib(bool isVisible)`: Establece la visibilidad del panel de selección elemental.
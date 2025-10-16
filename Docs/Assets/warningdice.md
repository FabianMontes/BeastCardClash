# `warningdice`
El script `warningdice` es un componente de tipo `MonoBehaviour` encargado de controlar la visibilidad de una imagen de la interfaz de usuario (`Image`) para indicar al jugador en qué momento de su turno se encuentra. Específicamente, este componente se activa cuando es el turno del "Fighter" asociado y el juego se encuentra en una fase relacionada con la selección o lanzamiento de dados (`SetMoments.PickDice` o `SetMoments.RollDice`).

Su funcionamiento se basa en la interacción con el sistema central de combate (`CombatJudge`) para determinar el estado actual del juego y con un componente `Fighter` de un GameObject padre para identificar al jugador en turno. La imagen controlada por este script probablemente sirve como un indicador visual ("warning" o alerta) para el jugador activo durante las fases de manipulación de dados.

# Métodos

## Métodos de Unity

### `Start()`
Este método se ejecuta una vez al inicio de la vida del script, después de que el GameObject al que está adjunto ha sido inicializado. Su propósito principal es obtener las referencias a los componentes necesarios para el funcionamiento del script:

```csharp
void Start()
{
    figther = GetComponentInParent<Fighter>();
    image = GetComponent<Image>();
}
```

*   `figther = GetComponentInParent<Fighter>();`: Busca y asigna una referencia al componente `Fighter` que se encuentre en alguno de los GameObjects padres en la jerarquía. Esto sugiere que el GameObject con el script `warningdice` es un elemento UI o visual hijo de un GameObject que representa un luchador (`Fighter`) en el combate. La variable `figther` (notar el posible error tipográfico, debería ser `fighter`) almacenará esta referencia.
*   `image = GetComponent<Image>();`: Busca y asigna una referencia al componente `Image` que se encuentre en el *mismo* GameObject al que está adjunto este script. Esta `Image` es el elemento visual que `warningdice` activará o desactivará.

### `Update()`
Este método se ejecuta en cada frame del juego y es el encargado de la lógica principal del script, que consiste en determinar si la imagen de advertencia debe estar visible o no.

```csharp
void Update()
{
    SetMoments momo = CombatJudge.Instance.GetSetMoments();
    if (momo == SetMoments.PickDice || momo == SetMoments.RollDice)
    {
        if (CombatJudge.Instance.Turn() == figther.indexFighter)
        {
            image.enabled = true;
            return;
        }
    }
    image.enabled = false;
}
```

La lógica funciona de la siguiente manera:
1.  **Obtener el estado actual del combate**:
    *   `SetMoments momo = CombatJudge.Instance.GetSetMoments();`: Se accede a la instancia del singleton `CombatJudge` y se obtiene el estado actual del juego, representado por un valor de la enumeración `SetMoments`. Esto permite al script saber en qué fase de la ronda de combate se encuentra el juego.
2.  **Verificar la fase de dados**:
    *   `if (momo == SetMoments.PickDice || momo == SetMoments.RollDice)`: Se comprueba si la fase actual del combate es `PickDice` (selección de dados) o `RollDice` (lanzamiento de dados). Estas son las fases en las que se espera que el jugador interactúe con los dados.
3.  **Verificar el turno del jugador**:
    *   `if (CombatJudge.Instance.Turn() == figther.indexFighter)`: Si el juego está en una de las fases de dados, se procede a verificar si el turno actual (`CombatJudge.Instance.Turn()`) corresponde al índice del luchador asociado a este componente (`figther.indexFighter`). Esto asegura que la advertencia solo se muestre al jugador cuyo turno es.
4.  **Habilitar la imagen**:
    *   `image.enabled = true;`: Si ambas condiciones (fase de dados y turno correcto) son verdaderas, la imagen se habilita, haciéndola visible en la UI.
    *   `return;`: Se sale del método `Update` inmediatamente, ya que la acción necesaria ya se ha realizado.
5.  **Deshabilitar la imagen**:
    *   `image.enabled = false;`: Si alguna de las condiciones no se cumple (no es una fase de dados, o no es el turno de este luchador), la imagen se deshabilita, ocultándola de la UI.

## Otros métodos
El script `warningdice` no define métodos adicionales más allá de los métodos de ciclo de vida de Unity (`Start`, `Update`).

## Getters y Setters
Este script no expone directamente ningún getter o setter propio. Sin embargo, utiliza propiedades y métodos de acceso de otros componentes:

1.  `figther`: Accede a la instancia del componente `Fighter` obtenido de un padre.
2.  `figther.indexFighter`: Accede al identificador numérico (`indexFighter`) del luchador asociado.
3.  `CombatJudge.Instance.GetSetMoments()`: Invoca un método del singleton `CombatJudge` para obtener el `SetMoments` actual del juego.
4.  `CombatJudge.Instance.Turn()`: Invoca un método del singleton `CombatJudge` para obtener el índice del luchador cuyo turno es actualmente.
5.  `image.enabled`: Establece o lee el estado de visibilidad del componente `Image` adjunto al mismo GameObject.
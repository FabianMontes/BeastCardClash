# HandCard
El script `HandCard` es un componente fundamental en el sistema de cartas de **Beast Card Clash**, encargado de gestionar la lógica y la representación visual de una carta individual en la mano de un jugador o en un espacio de selección. Su principal función es controlar la interactividad del botón asociado a la carta, su visibilidad en pantalla y la conexión con el objeto de datos `Card` que define sus propiedades.

Este script opera en dos modos principales:
1.  **Carta Jugable (`playable = true`):** Representa una carta que el jugador puede seleccionar y jugar durante las fases de combate. Su interactividad está ligada al estado actual del combate, gestionado por `CombatJudge`.
2.  **Selector/Placeholder (`picker = true`):** Actúa como un espacio para mostrar una carta o indicar una ranura de selección, con una lógica de visibilidad y comportamiento ligeramente diferente, a menudo con estados de "semi-visible".

La clase implementa las interfaces `IPointerEnterHandler` e `IPointerExitHandler` para detectar la entrada y salida del puntero del ratón, lo que permite futuras interacciones como efectos al pasar el cursor. El atributo `[DefaultExecutionOrder(-4)]` indica que este script se ejecutará muy temprano en el ciclo de vida de los scripts de Unity, asegurando que su inicialización y estado estén listos antes que otros componentes que puedan depender de él.

Interacciona estrechamente con el componente `Figther` (probablemente el jugador o una entidad que maneja la mano de cartas) y el singleton `CombatJudge`, que centraliza el control de las fases del combate y el tipo de combate activo.

# Métodos

## Métodos de Unity

### Start
El método `Start` se ejecuta una vez al inicio del ciclo de vida del script. Su propósito es inicializar los componentes y estados esenciales de la carta.

1.  **Obtención de Componentes:** Busca el componente `Figther` en el GameObject padre (`GetComponentInParent<Figther>()`) y el componente `Button` en el propio GameObject (`transform.GetComponent<Button>()`). Estos son cruciales para la interacción con el jugador y la funcionalidad de clic.
2.  **Inicialización de Estado:** `prevSetMoment` se inicializa a `SetMoments.PickDice`, un valor de referencia para detectar cambios en las fases de combate.
3.  **Configuración Inicial de Visibilidad e Interacción:**
    *   Si la carta es `playable` (jugable), se desactiva su clic (`clickable(false)`) al inicio, ya que la interactividad se gestiona dinámicamente durante el combate.
    *   Si no es `playable`, se hace invisible (`Visib(false)`) y su botón se desactiva por completo (`button.interactable = false`).
4.  **Asignación de Carta:** Llama a `SetCard(card)` para asignar el objeto `Card` inicial y ajustar su visibilidad.
5.  **Control Visual de Modo `picker`:** Activa o desactiva el GameObject hijo en el índice 1 (`transform.GetChild(1)`) basándose en el valor de `picker`, lo que sugiere que este hijo es una representación visual clave que varía entre los modos normal y `picker`.

```csharp
void Start()
{
    player = GetComponentInParent<Figther>();
    prevSetMoment = SetMoments.PickDice;
    button = transform.GetComponent<Button>();
    if (playable)
    {
        clickable(false);
    }
    else
    {
        Visib(false);
        button.interactable = false;
    }

    SetCard(card);

    transform.GetChild(1).gameObject.SetActive(picker);
}
```

### Update
El método `Update` se ejecuta en cada frame y es el encargado de la lógica dinámica de la carta, especialmente en respuesta a los cambios en el estado del combate.

1.  **Monitoreo del Estado de Combate:** Obtiene el `SetMoments` actual del `CombatJudge.CombatJudgeInstance`.
2.  **Lógica para Cartas No `picker` (Cartas de Mano):**
    *   Detecta si la fase de combate (`momo`) ha cambiado.
    *   Si la fase es `SetMoments.PickCard` y el jugador está en combate (`player.IsFigthing()`):
        *   Verifica si el `CombatType` actual del `CombatJudge` es `Full` o si coincide con el elemento de la carta (`card.GetElement()`). Si es así, la carta se hace clickable (`clickable(true)`) y se incrementa un contador de cartas disponibles en el jugador (`player.avalaibleCard++`).
        *   De lo contrario, la carta permanece no clickable (o su estado de clic no cambia).
    *   Si la fase no es `SetMoments.PickCard`, la carta se desactiva (`clickable(false)`).
    *   Actualiza `prevSetMoment` para el siguiente ciclo.
    *   Si el jugador ya ha seleccionado una carta (`player.getPicked() != null`), todas las demás cartas se desactivan (`clickable(false)`), evitando múltiples selecciones.
3.  **Lógica para Cartas `picker` (Espacios de Selección):**
    *   Si el script está en modo `picker`, el jugador está en combate y la fase no es `SetMoments.SelectCombat`:
        *   Si la fase es `SetMoments.PickCard`, la carta se muestra "semi-visible" (`halfVisible(true)`) y se ajusta la transparencia de la imagen del primer hijo del primer hijo (`transform.GetChild(0).GetChild(0).GetComponent<Image>()`). Si no hay una carta asignada, su opacidad se reduce al 50%.
        *   Si la fase es `SetMoments.Reveal`, la carta se muestra completamente visible (`Visib(true)`) y se desactiva el estado "semi-visible" (`halfVisible(false)`).

```csharp
void Update()
{
    SetMoments momo = CombatJudge.CombatJudgeInstance.GetSetMoments();
    if (!picker) // Lógica para cartas en mano del jugador
    {
        if (momo != prevSetMoment)
        {
            if (momo == SetMoments.PickCard && player.IsFigthing())
            {
                if (CombatJudge.CombatJudgeInstance.CombatType == CombatType.Full || (int)CombatJudge.CombatJudgeInstance.CombatType == (int)card.GetElement())
                {
                    clickable(true);
                    player.avalaibleCard++;
                }
            }
            if (momo != SetMoments.PickCard)
            {
                clickable(false);
            }
            prevSetMoment = momo;
        }
        if (player.getPicked() != null) clickable(false);
        return;
    }

    // Lógica para cartas en modo 'picker' (espacios de selección)
    if (picker && player.IsFigthing() && momo != SetMoments.SelectCombat)
    {
        if (momo == SetMoments.PickCard)
        {
            halfVisible(true);
            Image chil = transform.GetChild(0).GetChild(0).GetComponent<Image>();
            Color color = chil.color;
            color.a = card == null ? 0.5f : 1f;
            chil.color = color;
        }
        else if (momo == SetMoments.Reveal)
        {
            halfVisible(false);
            Visib(true);
        }
    }
}
```

### OnPointerEnter(PointerEventData eventData)
Este método es parte de la interfaz `IPointerEnterHandler` y se invoca cuando el puntero del ratón entra en el área del componente UI de la carta. Actualmente, su implementación está vacía, lo que sugiere que podría ser un punto de extensión futuro para añadir efectos de resaltado, tooltips o previsualizaciones de cartas al pasar el ratón.

### OnPointerExit(PointerEventData eventData)
Este método es parte de la interfaz `IPointerExitHandler` y se invoca cuando el puntero del ratón sale del área del componente UI de la carta. Al igual que `OnPointerEnter`, su implementación está vacía, indicando un potencial punto de extensión para revertir los efectos de entrada del puntero o limpiar cualquier estado temporal.

## Otros métodos

### Visib(bool isVisible)
`Visib` es un método privado que controla la visibilidad del GameObject hijo en el índice 1 (`transform.GetChild(1)`). Este hijo probablemente representa el cuerpo principal o la ilustración de la carta. Hacerlo visible (`true`) o invisible (`false`) permite mostrar u ocultar la carta en la interfaz.

```csharp
private void Visib(bool isVisible)
{
    // Controla la visibilidad del componente visual principal de la carta.
    transform.GetChild(1).gameObject.SetActive(isVisible);
}
```

### ForceReveal()
Este método público fuerza a la carta a ser interactuable, haciendo que su botón sea clicable (`clickable(true)`). Podría utilizarse en situaciones específicas del juego donde una carta debe ser revelada y seleccionable de inmediato, independientemente de la fase de combate actual.

### SetCard(Card card)
Este método público es crucial para asignar un objeto `Card` (los datos de la carta) a esta instancia de `HandCard`.

1.  **Asignación de Datos:** Asigna el objeto `Card` proporcionado a la variable interna `this.card`.
2.  **Gestión de Visibilidad:**
    *   Si el `card` asignado es `null`, o si la carta no es `playable` y tampoco es `picker`, la carta se hace invisible (`Visib(false)`).
    *   De lo contrario, si la carta es `picker`, se le aplica el estado de "semi-visible" (`halfVisible(true)`).
    *   Si no es `picker` (es una carta jugable normal), se hace completamente visible (`Visib(true)`).

```csharp
public void SetCard(Card card)
{
    this.card = card; // Asigna el objeto Card

    if (card == null || (!playable && !picker))
    {
        Visib(false); // Oculta la carta
    }
    else
    {
        if (picker)
        {
            halfVisible(true); // Muestra en estado semi-visible para picker
        }
        else
        {
            Visib(true); // Muestra completamente
        }
    }
}
```

### SelectedCard()
Este método público se invoca cuando el jugador selecciona o hace clic en la carta.

1.  **Notificar al Jugador:** Llama al método `player.PlayCard(card)` en el componente `Figther` padre, pasando la carta seleccionada. Esto informa al sistema del jugador que una carta ha sido jugada.
2.  **Vaciar Slot:** Luego, llama a `SetCard(null)` para desvincular la carta de este slot de `HandCard`, lo que visualmente la oculta, simulando que la carta ha sido "jugada" y ya no está en la mano.

```csharp
public void SelectedCard()
{
    player.PlayCard(card); // Notifica que la carta ha sido jugada
    SetCard(null); // Elimina la carta del slot visualmente
}
```

### clickable(bool isClick)
Este método público controla si la carta puede ser interactuada (clicada) por el jugador y proporciona una retroalimentación visual al respecto.

1.  **Interacción del Botón:** Establece la propiedad `interactable` del componente `button` de la carta a `isClick`.
2.  **Retroalimentación Visual:** Modifica el color de tres componentes `Image` específicos (`transform.GetChild(1).GetChild(0)`, `transform.GetChild(1).GetChild(1)`, `transform.GetChild(1).GetChild(2)`) que probablemente son elementos visuales como bordes, indicadores o iconos dentro de la carta. Si `isClick` es `true`, los colores se establecen en `Color.white`; de lo contrario, se establecen en `Color.gray`, indicando que la carta está deshabilitada.

```csharp
public void clickable(bool isClick)
{
    button.interactable = isClick; // Controla la interactividad del botón
    // Ajusta el color de los indicadores visuales de clicabilidad
    if (isClick)
    {
        transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white;
        transform.GetChild(1).GetChild(1).GetComponent<Image>().color = Color.white;
        transform.GetChild(1).GetChild(2).GetComponent<Image>().color = Color.white;
    }
    else
    {
        transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.gray;
        transform.GetChild(1).GetChild(1).GetComponent<Image>().color = Color.gray;
        transform.GetChild(1).GetChild(2).GetComponent<Image>().color = Color.gray;
    }
}
```

### halfVisible(bool visible)
Este método privado controla la visibilidad del GameObject hijo en el índice 0 (`transform.GetChild(0)`). A diferencia de `Visib`, que probablemente controla la representación principal de la carta, `halfVisible` parece controlar un elemento secundario, posiblemente un marco, un fondo o un indicador de slot vacío/semi-activo, especialmente útil en el modo `picker`.

```csharp
private void halfVisible(bool visible)
{
    // Controla la visibilidad de un elemento visual secundario, posiblemente un placeholder.
    transform.GetChild(0).gameObject.SetActive(visible);
}
```

## Getters y Setters

1.  `GetCard(): Card`: Retorna el objeto `Card` actualmente asignado a esta instancia de `HandCard`.
2.  `isClickable(): bool`: Retorna `true` si el botón de la carta está interactuable (es clicable), y `false` en caso contrario.
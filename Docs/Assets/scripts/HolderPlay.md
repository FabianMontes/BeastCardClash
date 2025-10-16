# `HolderPlay`
`HolderPlay` es un script de Unity (`MonoBehaviour`) diseñado para gestionar una única instancia de `Card` que ha sido "seleccionada" o "recogida" (`cardPicked`) en el contexto del juego. Este componente es esencial para mantener un registro de la carta activa que está siendo manipulada por el jugador o que se encuentra en un estado transitorio, como en un proceso de selección o preparación para ser jugada.

El script provee una interfaz para interactuar con esta carta seleccionada, permitiendo:
1.  **Obtener la carta actual:** Recuperar la referencia a la `Card` que está siendo manejada.
2.  **Limpiar la selección:** Desvincular la carta actual, posiblemente reactivando su representación visual.
3.  **Establecer una nueva selección:** Asignar una `Card` diferente como la carta activa, gestionando cualquier carta previamente seleccionada.

La variable `cardPicked` está marcada con `[SerializeField]`, lo que permite que su valor sea visible y editable en el Inspector de Unity para propósitos de depuración o una configuración inicial, aunque su estado se controla principalmente a través de los métodos públicos del script, asegurando una gestión centralizada y controlada de la carta activa.

# Métodos

## Métodos de Unity

### `No hay métodos de ciclo de vida de Unity definidos en este script.`
Este script no implementa métodos de ciclo de vida estándar de Unity como `Awake`, `Start` o `Update`. Su lógica y funcionalidad se activan y gestionan mediante la invocación explícita de sus métodos públicos desde otros componentes o sistemas del juego, lo que implica que `HolderPlay` actúa como un servicio o un manipulador de estado que responde a eventos externos.

## Otros métodos

### `public void LosePick()`
Este método es responsable de "soltar" o "deseleccionar" la carta actualmente asignada a `cardPicked`. Ejecuta los siguientes pasos:

1.  **Verificación de existencia:** Comprueba si `cardPicked` contiene una referencia a una `Card` (es decir, no es `null`).
2.  **Activación de GameObject:** Si hay una carta asignada, se llama a `cardPicked.gameObject.SetActive(true)`. Esto sugiere que, al ser "recogida" o seleccionada, la carta pudo haber sido desactivada visualmente. Al "soltarla", se asegura que su GameObject vuelva a estar activo.
3.  **Limpieza de referencia:** Finalmente, `cardPicked` se establece en `null`, eliminando la referencia a la carta.

```csharp
public void LosePick()
{
    if (cardPicked != null) cardPicked.gameObject.SetActive(true);
    cardPicked = null;
}
```
Esta funcionalidad es crucial para gestionar el estado visual y lógico de las cartas cuando ya no están activamente seleccionadas, permitiendo que la carta retome su estado original (por ejemplo, en la mano del jugador o en el mazo) antes de que la referencia se desvincule.

### `public void PlayCard(Card card)`
El método `PlayCard` permite establecer una `Card` específica como la carta actualmente "recogida" o seleccionada dentro de este `HolderPlay`. Aunque el nombre `PlayCard` podría implicar la acción final de "jugar" una carta, en el contexto de este script parece referirse a la acción de **seleccionar** una carta para una interacción futura, como prepararla para ser ubicada en el campo de juego o activar sus habilidades.

El flujo de este método es el siguiente:

1.  **Gestión de selección previa:** Si ya existe una carta previamente seleccionada (`cardPicked` no es `null`), se invoca `LosePick()` para deseleccionarla y asegurar que su GameObject sea reactivado y la referencia limpiada. Esto garantiza que solo una carta esté "recogida" en un momento dado.
2.  **Asignación de nueva carta:** La `Card` proporcionada como argumento (`card`) se asigna a la variable `cardPicked`, convirtiéndola en la nueva carta activa o seleccionada.

```csharp
public void PlayCard(Card card)
{
    if (cardPicked != null) LosePick();
    cardPicked = card;
}
```
Este método es fundamental para la interacción del jugador o la lógica del juego al elegir una carta. Asegura una transición limpia entre cartas seleccionadas y prepara la nueva carta para su manejo posterior, como el arrastre por el tablero o la activación de un menú de opciones.

## Getters y Setters

1.  `public Card GetPicked()`: Este método getter retorna la instancia de `Card` que actualmente está siendo "recogida" o seleccionada por el `HolderPlay`. Si en el momento de la llamada no hay ninguna carta seleccionada (es decir, `cardPicked` es `null`), el método retornará `null`. Es utilizado por otros componentes para acceder a la carta actualmente activa.
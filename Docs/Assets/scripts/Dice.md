# `Dice`
Este script gestiona el comportamiento de un objeto de dado 3D dentro del juego, permitiendo su lanzamiento y la visualización de un valor aleatorio. Funciona como un componente interactivo que los jugadores pueden manipular para generar resultados numéricos, crucial para la mecánica de estrategia por turnos de "Beast Card Clash".

El dado no simula una animación de rotación física compleja, sino que cuando se activa el lanzamiento, genera un valor aleatorio y ajusta instantáneamente su orientación para mostrar la cara correspondiente al valor obtenido. Esto simplifica el desarrollo y garantiza una retroalimentación inmediata al jugador.

Su interacción principal es con el singleton `CombatJudge.Instance`, al cual consulta el estado del juego (`FocusOnTurn`, `GetSetMoments`) para determinar si el dado puede ser lanzado, obtiene el valor máximo permitido para el lanzamiento (`maxDice`), y le notifica sobre el inicio y fin del proceso de lanzamiento (`StartRolling`, `Rolled`).

# Métodos

## Métodos de Unity

### `Start()`
Se invoca una vez al inicio del ciclo de vida del script. Su función es inicializar las variables internas del dado:
- Establece `_maxValue` (el valor máximo que el dado puede lanzar) obteniéndolo de `CombatJudge.Instance.maxDice`.
- Inicializa `_rolling` a `false`, asegurando que el dado no se encuentre en un estado de lanzamiento al inicio.

```csharp
void Start()
{
    _maxValue = CombatJudge.Instance.maxDice;
    _rolling = false;
}
```

### `Update()`
Se invoca en cada frame del juego. Este método es el encargado de gestionar la lógica continua del dado mientras está en proceso de "lanzamiento":
- Verifica si la bandera `_rolling` es `true`. Si es `false`, el método no realiza ninguna acción adicional, optimizando el rendimiento.
- Si `_rolling` es `true`:
    - Notifica a `CombatJudge` que el dado ha comenzado a "girar" visualmente llamando a `CombatJudge.Instance.StartRolling()`.
    - Genera un nuevo valor aleatorio para el dado, asignándolo a la propiedad `Value`. El rango es de `1` a `_maxValue` (inclusive).
    - Ajusta la `transform.rotation` del GameObject para que la cara correspondiente al `Value` generado sea visible. Se asume que la cara con el valor `3` es la orientación por defecto del modelo 3D del dado.

```csharp
void Update()
{
    // Si no se lanza el dado, no hace nada
    if (!_rolling) return;

    // Inicia a rotar el dado con un valor aleatorio
    CombatJudge.Instance.StartRolling();
    Value = Random.Range(1, _maxValue + 1);

    // Rota el dado en cada frame
    Vector3 vector3 = new Vector3(0, 45, 0); // Rotación base
    switch (Value)
    {
        case 1:
            vector3.z = 90;
            break;
        case 2:
            vector3.x = -90;
            break;
        case 3:
            // 3 es el valor por defecto, entonces no tiene rotación adicional
            break;
        case 4:
            vector3.x = 180;
            break;
        case 5:
            vector3.x = 90;
            break;
        case 6:
            vector3.z = -90;
            break;
    }

    transform.rotation = Quaternion.Euler(vector3);
}
```

### `OnMouseDown()`
Este método de evento de Unity se dispara cuando el usuario presiona el botón del mouse mientras el cursor está sobre el collider del dado.
- Comprueba si `CombatJudge.Instance.FocusOnTurn()` es verdadero, lo que indica que es el turno del jugador actual y la interacción con el dado está permitida.
- Si la condición es verdadera, llama al método `Roll()` para iniciar el proceso de lanzamiento del dado.

### `OnMouseExit()`
Este método de evento de Unity se dispara cuando el cursor del mouse sale del área del collider del dado.
- Similar a `OnMouseDown()`, verifica `CombatJudge.Instance.FocusOnTurn()`.
- Si la condición es verdadera, llama al método `Unroll()` para detener el proceso de lanzamiento, asumiendo que el jugador ha "soltado" el dado visualmente.

### `OnMouseUp()`
Este método de evento de Unity se dispara cuando el usuario suelta el botón del mouse mientras el cursor está sobre el collider del dado.
- Al igual que los otros eventos de mouse, verifica `CombatJudge.Instance.FocusOnTurn()`.
- Si la condición es verdadera, llama al método `Unroll()` para finalizar el lanzamiento del dado, completando la interacción del usuario.

## Otros métodos

### `public void Roll()`
Este método público inicia el proceso de lanzamiento del dado.
- Primero, verifica que el momento actual del juego, según `CombatJudge.Instance.GetSetMoments()`, sea `SetMoments.PickDice`. Esta comprobación es crucial para asegurar que el dado solo pueda ser lanzado en fases específicas del juego donde esta acción es válida.
- Si la condición se cumple, establece la variable booleana `_rolling` a `true`. Esto activa la lógica de lanzamiento en el método `Update()`, lo que lleva a la generación de un nuevo valor aleatorio y la actualización visual del dado.

```csharp
public void Roll()
{
    if (CombatJudge.Instance.GetSetMoments() == SetMoments.PickDice) _rolling = true;
}
```

### `public void Unroll()`
Este método público detiene el proceso de lanzamiento del dado y notifica al `CombatJudge`.
- Primero, verifica si el dado realmente está en estado de lanzamiento (`_rolling` es `true`). Si no lo está, el método no hace nada.
- Si el dado está rodando, establece `_rolling` a `false`. Esto detiene la actualización continua en el método `Update()`.
- Finalmente, notifica a `CombatJudge` que el dado ha terminado de rodar llamando a `CombatJudge.Instance.Rolled()`, permitiendo que el juez de combate procese el resultado del lanzamiento.

```csharp
public void Unroll()
{
    if (!_rolling) return;

    _rolling = false;
    CombatJudge.Instance.Rolled();
}
```

## Getters y Setters

1. `public int Value { get; private set; }`: Obtiene el valor numérico actual que el dado ha lanzado. Solo el script `Dice` puede establecer este valor.
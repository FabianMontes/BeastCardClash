# Dice
Este script, `Dice`, gestiona el comportamiento de un dado virtual en el juego. Su función principal es simular el lanzamiento de un dado, asignarle un valor aleatorio, y controlar su visualización mediante rotaciones. Actúa como un componente interactivo que los jugadores pueden "lanzar" haciendo clic, y se integra estrechamente con el sistema de combate principal del juego, `CombatJudge`, para determinar cuándo se puede lanzar el dado y notificar el resultado.

Durante su estado de "lanzamiento", el dado actualiza constantemente su valor y rotación en cada frame, creando una animación de giro hasta que se detiene, momento en el que su valor final queda establecido y es comunicado al sistema de combate. Este diseño permite una experiencia de interacción fluida donde el dado responde a las acciones del jugador dentro del contexto del turno de combate.

# Métodos

## Métodos de Unity

### Awake, Start, Update
El script no implementa `Awake`.

### Start
Este método se ejecuta una única vez cuando el script se inicializa, al cargar el GameObject al que está adjunto. Su propósito es configurar los valores iniciales del dado:

-   **`_maxValue`**: Se asigna el valor máximo posible que el dado puede obtener, recuperándolo del singleton `CombatJudge.CombatJudgeInstance.maxDice`. Esto asegura que el dado se configure según las reglas del combate definidas centralmente.
-   **`_rolling`**: Se inicializa a `false`, indicando que el dado no está en proceso de lanzamiento al inicio del juego.

```csharp
void Start()
{
    _maxValue = CombatJudge.CombatJudgeInstance.maxDice;
    _rolling = false;
}
```

### Update
Se llama una vez por frame. Este método es el responsable de la lógica de "lanzamiento" visual y la asignación constante de valores aleatorios mientras el dado está en estado de giro (`_rolling` es `true`).

1.  **Verificación de estado**: Si `_rolling` es `false`, el método retorna inmediatamente, ahorrando recursos al no realizar cálculos innecesarios.
2.  **Notificación de inicio de lanzamiento**: Llama a `CombatJudge.CombatJudgeInstance.StartRolling()`. Esto informa al sistema de combate que el dado está actualmente en proceso de giro, lo que podría usarse para actualizar la UI o el estado del juego global.
3.  **Asignación de valor aleatorio**: `Value` se establece en un número entero aleatorio entre 1 y `_maxValue` (inclusive). **Importante**: esta asignación ocurre en cada frame mientras el dado gira, lo que contribuye al efecto visual de un dado girando rápidamente por diferentes caras.
4.  **Rotación visual del dado**: Se ajusta la rotación del GameObject (`transform.rotation`) para mostrar la cara correspondiente al valor actual de `Value`. Se utiliza una rotación base `(0, 45, 0)` y luego se modifican los ángulos en `x` o `z` mediante una estructura `switch` para orientar el dado correctamente, simulando que cada cara tiene un número específico.

```csharp
void Update()
{
    if (!_rolling) return;
    
    CombatJudge.CombatJudgeInstance.StartRolling();
    Value = Random.Range(1, _maxValue + 1);
    
    // ... (cálculo de rotación según Value) ...
    transform.rotation = Quaternion.Euler(vector3);
}
```

### OnMouseDown
Este método se invoca cuando el usuario presiona el botón del ratón mientras el puntero está sobre el collider del GameObject al que está adjunto el script.

-   Primero, verifica si `CombatJudge.CombatJudgeInstance.FocusOnTurn()` es `true`. Esto asegura que el jugador solo pueda interactuar con el dado cuando sea su turno o el momento apropiado del juego, según lo determine el `CombatJudge`.
-   Si la condición se cumple, se llama al método `Roll()` para iniciar el lanzamiento del dado.

```csharp
void OnMouseDown()
{
    if (CombatJudge.CombatJudgeInstance.FocusOnTurn()) Roll();
}
```

### OnMouseExit
Se invoca cuando el puntero del ratón deja de estar sobre el collider del GameObject.

-   Similar a `OnMouseDown`, verifica `CombatJudge.CombatJudgeInstance.FocusOnTurn()`.
-   Si la condición se cumple, se llama al método `Unroll()` para detener el lanzamiento del dado. Este comportamiento sugiere una posible interacción donde el jugador podría "flickear" o mover el ratón rápidamente fuera del dado para detener su giro, o simplemente soltarlo fuera del área del dado.

```csharp
void OnMouseExit()
{
    if (CombatJudge.CombatJudgeInstance.FocusOnTurn()) Unroll();
}
```

### OnMouseUp
Este método se invoca cuando el usuario suelta el botón del ratón mientras el puntero está sobre el collider del GameObject.

-   Al igual que `OnMouseDown` y `OnMouseExit`, verifica `CombatJudge.CombatJudgeInstance.FocusOnTurn()`.
-   Si la condición se cumple, se llama al método `Unroll()`. Este es un mecanismo común para finalizar una interacción de "mantener presionado y soltar", deteniendo el lanzamiento del dado cuando el jugador "suelta" el dado virtual.

```csharp
void OnMouseUp()
{
    if (CombatJudge.CombatJudgeInstance.FocusOnTurn()) Unroll();
}
```

## Otros métodos

### public void Roll()
Este método público inicia el proceso de lanzamiento del dado.

-   Verifica si el momento actual del juego, obtenido de `CombatJudge.CombatJudgeInstance.GetSetMoments()`, es igual a `SetMoments.PickDice`. Esta condición asegura que el dado solo puede ser lanzado durante una fase específica del juego donde se espera que el jugador elija un dado, previniendo lanzamientos en momentos inapropiados.
-   Si la condición se cumple, establece la variable `_rolling` a `true`, lo que activa la lógica de giro continuo en el método `Update()`.

```csharp
public void Roll()
{
    if (CombatJudge.CombatJudgeInstance.GetSetMoments() == SetMoments.PickDice) _rolling = true;
}
```

### public void Unroll()
Este método público detiene el proceso de lanzamiento del dado.

-   Primero, verifica si `_rolling` es `false`. Si ya lo es, significa que el dado no estaba girando o ya se detuvo, por lo que el método retorna para evitar acciones redundantes.
-   Si el dado estaba girando, establece `_rolling` a `false`, lo que detiene la lógica de giro en el método `Update()`.
-   Finalmente, llama a `CombatJudge.CombatJudgeInstance.Rolled()`. Esta notificación al `CombatJudge` es crucial, ya que le informa al sistema de combate que el dado ha terminado de girar y su valor final está listo para ser utilizado en la lógica del juego.

```csharp
public void Unroll()
{
    if (!_rolling) return;
    
    _rolling = false;
    CombatJudge.CombatJudgeInstance.Rolled();
}
```

## Getters y Setters

1.  **Value: `int`**: Proporciona el valor entero actual del dado después de haber sido "lanzado". Este es el resultado numérico del giro del dado y es de solo lectura desde fuera del script.
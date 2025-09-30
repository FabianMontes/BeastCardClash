# warningdice
El script `warningdice` es un componente `MonoBehaviour` de Unity cuya función principal es proporcionar una señal visual clara e inmediata al jugador, indicando cuándo es su turno para interactuar con los dados (seleccionarlos o lanzarlos) durante las fases específicas del combate en `Beast Card Clash`. Este componente controla la visibilidad de un elemento `Image` (probable icono o texto de advertencia) que se activa únicamente cuando se cumplen ciertas condiciones relativas al estado del combate y al turno del jugador. Esto asegura una experiencia de juego fluida, donde el jugador recibe retroalimentación visual directa sobre su ventana de acción, un aspecto crucial para la usabilidad en un juego de estrategia por turnos.

El `warningdice` depende directamente de la lógica de gestión de combate centralizada, obteniendo el estado actual del juego y el turno activo a través del `CombatJudge`. Su estructura y funcionamiento están diseñados para ser eficientes, actualizando su estado visual en cada frame para reflejar con precisión los cambios en el juego.

# Métodos

## Métodos de Unity

### Start
El método `Start` se ejecuta una única vez en el ciclo de vida del script, justo antes del primer `Update`, tras la inicialización del `MonoBehaviour`. Su propósito es inicializar las referencias a los componentes necesarios para el funcionamiento del `warningdice`.

En este método, se realizan las siguientes asignaciones:
1.  **`figther = GetComponentInParent<Figther>();`**: Se busca y se asigna una referencia al componente `Figther`. La clave aquí es el uso de `GetComponentInParent<T>()`, lo que sugiere que el script `warningdice` está adjunto a un GameObject que es un hijo (directo o indirecto) de otro GameObject que posee el componente `Figther`. Esto permite que el `warningdice` esté asociado a una entidad de "luchador" específica, de la cual obtendrá el `indexFigther` para identificar al jugador.

2.  **`image = GetComponent<Image>();`**: Se busca y se asigna una referencia al componente `Image` que reside en el mismo GameObject al que está adjunto el script `warningdice`. Este componente `Image` es el elemento visual que el script controlará para mostrar u ocultar la advertencia.

```csharp
void Start()
{
    figther = GetComponentInParent<Figther>();
    image = GetComponent<Image>();
}
```

### Update
El método `Update` se ejecuta una vez por cada frame del juego. Su responsabilidad es verificar continuamente el estado actual del combate y el turno del jugador para determinar si el elemento `Image` (la advertencia visual) debe estar visible o no.

El flujo de ejecución es el siguiente:
1.  **`SetMoments momo = CombatJudge.CombatJudgeInstance.GetSetMoments();`**: Se obtiene el momento o fase actual del combate a través del sistema `CombatJudge`. El uso de `CombatJudge.CombatJudgeInstance` indica que `CombatJudge` probablemente implementa el patrón Singleton, lo que permite un acceso global a su instancia para consultar el estado del juego. `SetMoments` es un enumerador que define las diferentes etapas de la ronda de combate, como `PickDice` (selección de dados) o `RollDice` (lanzamiento de dados).

2.  **Condición de visibilidad (`if` principal)**: El script verifica si la fase actual del combate (`momo`) es `SetMoments.PickDice` o `SetMoments.RollDice`. Estas son las fases críticas donde el jugador necesita una indicación para actuar con los dados.

    ```csharp
    if(momo == SetMoments.PickDice || momo == SetMoments.RollDice)
    {
        // ... lógica interna
    }
    ```

3.  **Condición de turno (`if` anidado)**: Si la fase es una de las mencionadas, se procede a verificar si el turno actual corresponde al jugador asociado con este `warningdice`. Esto se hace comparando el `indexFigther` del `figther` obtenido en `Start` con el `Turn()` actual reportado por `CombatJudge.CombatJudgeInstance`.

    ```csharp
    if(CombatJudge.CombatJudgeInstance.Turn() == figther.indexFigther)
    {
        image.enabled = true; // Activa la imagen
        return; // Sale del método para evitar desactivación posterior
    }
    ```

    > [!NOTE]
    > El uso de `return;` dentro de esta condición es una optimización. Si se cumplen ambas condiciones (fase correcta y turno del jugador), la imagen se habilita y el script sale inmediatamente del método `Update`, evitando la línea `image.enabled = false;` al final, que de otro modo la desactivaría innecesariamente.

4.  **Desactivación por defecto**: Si las condiciones para activar la imagen no se cumplen (es decir, no es una fase de dados o no es el turno del jugador asociado), la imagen se desactiva.

    ```csharp
    image.enabled = false;
    ```

Este ciclo de `Update` asegura que la advertencia visual se mantenga sincronizada en tiempo real con el estado del juego, ofreciendo una experiencia de usuario clara y responsiva en `Beast Card Clash`.
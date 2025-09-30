# Sfxcombatmanager
Este script `Sfxcombatmanager` es el componente encargado de gestionar la reproducción de efectos de sonido (SFX) durante las distintas fases del combate en "Beast Card Clash". Su función principal es reaccionar a los cambios de estado en el sistema de combate, orquestado por el `CombatJudge`, y reproducir los clips de audio correspondientes para proporcionar una retroalimentación sonora clara y envolvente al jugador.

El script mantiene una referencia a un `AudioSource` para la reproducción y un array de `AudioClip` asignados a través del Inspector de Unity. Utiliza un patrón de "polling" en su método `Update` para consultar continuamente el estado actual del combate y, basándose en este, activa la reproducción de SFX específicos. Esto contribuye directamente a la "buena experiencia de jugador" mencionada en el README, al dotar de impacto auditivo a cada momento clave de la batalla. Además, su diseño sencillo y directo facilita la comprensión y el mantenimiento para los programadores del proyecto.

# Métodos

## Métodos de Unity

### Start
Este método se ejecuta una única vez al inicio del ciclo de vida del script, antes de la primera actualización de `Update`. Su propósito es inicializar la referencia al componente `AudioSource` necesario para la reproducción de los sonidos.

```csharp
void Start()
{
    audio = GetComponent<AudioSource>();
}
```

Es crucial que el GameObject al que este script esté adjunto tenga un componente `AudioSource`, ya que `GetComponent<AudioSource>()` intentará obtenerlo de este. Si no se encuentra, la variable `audio` será nula, lo que podría provocar errores en tiempo de ejecución.

### Update
`Update` se invoca en cada frame del juego. Su función principal es monitorizar el estado actual del combate y disparar los efectos de sonido apropiados. Para ello, consulta el "momento" actual del combate a través del sistema `CombatJudge`.

```csharp
void Update()
{
    switch (CombatJudge.CombatJudgeInstance.GetSetMoments())
    {
        case SetMoments.PickDice:
            // No SFX específico para este momento.
            break;
        case SetMoments.RollDice:
            changeSource(0,true,true); // Reproduce el SFX de lanzar dados.
            break;
        case SetMoments.RevealDice:
            // No SFX específico para este momento.
            break;
        case SetMoments.GlowRock:
            changeSource(-1, true, false); // Detiene cualquier SFX en reproducción.
            break;
        case SetMoments.MoveToRock:
            changeSource(1, true, true); // Reproduce el SFX de movimiento a la roca.
            break;
        case SetMoments.SelectCombat:
            // No SFX específico para este momento.
            break;
        case SetMoments.PickCard:
            // No SFX específico para este momento.
            break;
        case SetMoments.Reveal:
            changeSource(2, true, false); // Reproduce el SFX de revelación.
            break;
        case SetMoments.Result:
            changeSource(CombatJudge.CombatJudgeInstance.HurtPlayer()? 3:4, true, false); // SFX condicional según si el jugador fue dañado.
            break;
        case SetMoments.End:
            // No SFX específico para este momento.
            break;
        case SetMoments.Loop:
            // No SFX específico para este momento.
            break;
        case SetMoments.Round:
            // No SFX específico para este momento.
            break;
        case SetMoments.Rounded:
            // No SFX específico para este momento.
            break;
    }
}
```

Utiliza una sentencia `switch` para evaluar el valor devuelto por `CombatJudge.CombatJudgeInstance.GetSetMoments()`, que se espera sea una enumeración `SetMoments`. Dependiendo del momento del combate, invoca el método `changeSource` con diferentes parámetros:
*   **`RollDice`**: Activa el clip en el índice `0`.
*   **`GlowRock`**: Llama a `changeSource` con `-1`, lo que tiene el efecto de detener la reproducción de cualquier sonido actual sin iniciar uno nuevo.
*   **`MoveToRock`**: Activa el clip en el índice `1`.
*   **`Reveal`**: Activa el clip en el índice `2`.
*   **`Result`**: Este es un caso especial donde el clip reproducido depende de si el jugador ha sido dañado, consultando `CombatJudge.CombatJudgeInstance.HurtPlayer()`. Si el jugador fue dañado, reproduce el clip en el índice `3`; de lo contrario, el clip en el índice `4`.
*   Para otros `SetMoments` (como `PickDice`, `RevealDice`, `SelectCombat`, etc.), actualmente no se reproduce ningún sonido. Esto puede ser una decisión de diseño para mantener ciertos momentos silenciosos o una oportunidad para futuras implementaciones de SFX.

## Otros métodos

### changeSource(int index, bool Force, bool loop)
Este método público es la interfaz principal para controlar la reproducción de los efectos de sonido. Permite iniciar o detener un clip de audio específico, con opciones para forzar la reproducción y definir si el sonido debe repetirse.

```csharp
public void changeSource(int index,bool Force,bool loop)
{
    // Evita reproducir el mismo clip si ya está sonando y no se fuerza, o si el índice es el mismo que el último reproducido.
    if ((audio.isPlaying && !Force )|| index == last)
    {
        return;
    }
    last = index; // Actualiza el índice del último clip reproducido.
    audio.Stop(); // Detiene cualquier clip que se esté reproduciendo actualmente.

    if(index == -1) // Si el índice es -1, significa que solo se debe detener el sonido.
    {
        return;
    }
    audio.resource = clips[index]; // Asigna el nuevo AudioClip de la lista.
    audio.Play(); // Inicia la reproducción del nuevo clip.
    audio.loop = loop; // Configura si el clip debe repetirse.
}
```

**Parámetros:**
*   `index` (int): El índice del `AudioClip` dentro del array `clips` que se desea reproducir. Un valor de `-1` es un caso especial para detener la reproducción actual sin iniciar un nuevo sonido.
*   `Force` (bool): Si es `true`, la reproducción del nuevo clip forzará la detención del clip actual y comenzará la reproducción del clip especificado, incluso si el mismo clip ya estaba sonando. Si es `false`, la reproducción solo ocurrirá si no hay un sonido activo o si el nuevo sonido es diferente al último reproducido.
*   `loop` (bool): Si es `true`, el clip de audio se repetirá indefinidamente después de su finalización. Si es `false`, se reproducirá una sola vez.

**Funcionamiento detallado:**
1.  **Condición de Salida Temprana**: Primero, verifica si el `AudioSource` ya está reproduciendo un sonido (`audio.isPlaying`) y si el parámetro `Force` es `false`. También verifica si el `index` solicitado es el mismo que el `last` clip reproducido. Si alguna de estas condiciones se cumple, el método retorna para evitar la reproducción redundante o no deseada.
    > [!NOTE]
    > La variable `last` es un mecanismo simple para evitar la repetición constante del mismo sonido si el estado del `CombatJudge` no cambia rápidamente entre frames, mejorando la "experiencia de desarrollo" al simplificar la lógica de control.
2.  **Actualización de `last`**: Si el método continúa, `last` se actualiza con el `index` del nuevo clip a reproducir.
3.  **Detener Actual**: Se llama a `audio.Stop()` para detener cualquier clip que esté sonando en ese momento en el `AudioSource`.
4.  **Manejo de `-1`**: Si el `index` proporcionado es `-1`, el método retorna inmediatamente después de detener cualquier sonido, lo que efectivamente silencia el `AudioSource`.
5.  **Asignar y Reproducir**: Si el `index` es válido (no `-1`), se asigna el `AudioClip` correspondiente de la lista `clips` al `AudioSource.clip` (observar que el código usa `audio.resource` que probablemente sea un error de escritura y debería ser `audio.clip` o `audio.clip = clips[index]`).
6.  **Iniciar Reproducción**: Se llama a `audio.Play()` para comenzar la reproducción del clip asignado.
7.  **Configurar Bucle**: Finalmente, se establece la propiedad `audio.loop` según el valor del parámetro `loop`.

## Getters y Setters
No se encuentran getters o setters públicos definidos explícitamente en este script que gestionen propiedades internas de forma directa. Las interacciones con el `AudioSource` y el array `clips` se manejan a través del método `changeSource` y la serialización pública de `clips` en el Inspector de Unity.
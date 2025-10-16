# `Sfxcombatmanager`
Este script `Sfxcombatmanager` es un componente de Unity (`MonoBehaviour`) que se encarga de la reproducción de efectos de sonido (SFX) durante las diferentes fases o "momentos" del combate en el juego "Beast Card Clash". Su función principal es escuchar el estado actual del combate, determinado por un sistema externo (`CombatJudge`), y en base a ese estado, reproducir el `AudioClip` correspondiente de un conjunto predefinido. Está diseñado para ofrecer una experiencia sonora dinámica que acompañe la progresión de las interacciones de combate, como la tirada de dados, el movimiento de elementos o la revelación de resultados.

El script gestiona internamente qué sonido se está reproduciendo para evitar interrupciones innecesarias o repeticiones del mismo clip, a menos que se fuerce explícitamente. Se apoya en un componente `AudioSource` adjunto al mismo GameObject para la reproducción de los sonidos y utiliza un array de `AudioClip`s que son asignados desde el Inspector de Unity.

# Métodos

## Métodos de Unity

### `Start`
Este método se invoca una vez al inicio del ciclo de vida del script, antes de la primera actualización de `Update`. Su propósito es inicializar el componente `AudioSource` que será utilizado para la reproducción de los efectos de sonido.

```csharp
void Start()
{
    audio = GetComponent<AudioSource>();
}
```

Aquí, el script obtiene una referencia al componente `AudioSource` que debe estar adjunto al mismo GameObject donde se encuentra `Sfxcombatmanager`. Esto asegura que el script tenga la capacidad de reproducir audio a través de este componente.

### `Update`
Este método se llama una vez por cada frame del juego. Su función es monitorear continuamente el estado del combate y activar los efectos de sonido apropiados.

```csharp
void Update()
{
    switch (CombatJudge.Instance.GetSetMoments())
    {
        // ... (casos del switch) ...
    }
}
```

Dentro de `Update`, se utiliza una estructura `switch` para evaluar el valor retornado por `CombatJudge.Instance.GetSetMoments()`. `CombatJudge.Instance` sugiere que `CombatJudge` es un patrón Singleton que proporciona el estado actual del juego. `GetSetMoments()` es un método que devuelve un valor de la enumeración `SetMoments`, la cual define las distintas fases del combate.

Dependiendo del momento de combate (`SetMoments`), se invoca al método `changeSource` con diferentes parámetros:

-   `SetMoments.RollDice`: Llama a `changeSource(0, true, true)` para reproducir el primer clip (índice 0) de forma forzada y en bucle.
-   `SetMoments.GlowRock`: Llama a `changeSource(-1, true, false)` para detener cualquier sonido actual (el índice -1 está programado para detener el audio) y no reproducir nada.
-   `SetMoments.MoveToRock`: Llama a `changeSource(1, true, true)` para reproducir el segundo clip (índice 1) de forma forzada y en bucle.
-   `SetMoments.Reveal`: Llama a `changeSource(2, true, false)` para reproducir el tercer clip (índice 2) de forma forzada y sin bucle.
-   `SetMoments.Result`: Llama a `changeSource(CombatJudge.Instance.HurtPlayer()? 3:4, true, false)`. Aquí, se evalúa el resultado del combate utilizando `CombatJudge.Instance.HurtPlayer()`. Si el jugador resulta herido, se reproduce el clip en el índice 3; de lo contrario, se reproduce el clip en el índice 4. La reproducción es forzada y sin bucle.

Otros momentos de `SetMoments` como `PickDice`, `RevealDice`, `SelectCombat`, `PickCard`, `End`, `Loop`, `Round` y `Rounded` no tienen asignado un efecto de sonido específico en este script, por lo que sus casos en el `switch` están vacíos.

## Otros métodos

### `public void changeSource(int index, bool Force, bool loop)`
Este método es el encargado de gestionar la reproducción de los clips de audio. Permite iniciar, detener y configurar la reproducción de un efecto de sonido específico.

-   `index`: Un valor entero que representa el índice del `AudioClip` a reproducir dentro del array `clips`. Un índice de `-1` tiene un comportamiento especial para detener la reproducción.
-   `Force`: Un valor booleano que, si es `true`, obliga la reproducción del nuevo clip, incluso si el `AudioSource` ya está reproduciendo algo o si el clip solicitado es el mismo que el último reproducido. Si es `false`, el método podría no hacer nada bajo ciertas condiciones.
-   `loop`: Un valor booleano que, si es `true`, configura el clip para que se reproduzca en bucle continuo.

```csharp
public void changeSource(int index,bool Force,bool loop)
{
    if ((audio.isPlaying && !Force )|| index == last)
    {
        return; // Evita reproducir si ya está sonando (y no es forzado) o si es el mismo clip.
    }
    last = index; // Almacena el índice del clip que se va a reproducir.
    audio.Stop(); // Detiene cualquier reproducción actual.

    if(index == -1)
    {
        return; // Si el índice es -1, solo se detiene el audio y no se reproduce nada nuevo.
    }
    audio.resource = clips[index]; // Asigna el AudioClip al AudioSource.
    audio.Play(); // Inicia la reproducción del clip.
    audio.loop = loop; // Configura si el clip se reproducirá en bucle.
}
```

El método primero verifica si ya hay un sonido en reproducción y `Force` es `false`, o si el `index` solicitado es el mismo que el `last` clip reproducido. Si alguna de estas condiciones es verdadera, el método retorna para evitar una reproducción redundante o interrupción innecesaria. Luego, actualiza `last` con el nuevo `index` y detiene cualquier sonido que se esté reproduciendo actualmente con `audio.Stop()`.

Si el `index` proporcionado es `-1`, el método simplemente detiene el audio y no asigna ni reproduce ningún nuevo clip, lo cual es útil para silenciar los SFX de combate explícitamente. De lo contrario, asigna el `AudioClip` correspondiente del array `clips` a la propiedad `resource` del `AudioSource` y lo reproduce con `audio.Play()`. Finalmente, configura la propiedad `loop` del `AudioSource` según el parámetro `loop`.

> [!NOTE]
> La línea `audio.resource = clips[index];` es utilizada para asignar el `AudioClip`. En la API estándar de Unity, la propiedad para asignar un clip a un `AudioSource` es `AudioSource.clip`. El uso de `audio.resource` en este script sugiere una posible variación, una extensión personalizada, o una convención interna del proyecto. Se documenta tal como aparece en el código.

## Getters y Setters
Este script no expone métodos públicos específicos para actuar como Getters o Setters de sus variables internas. La interacción con sus datos se realiza directamente a través de sus campos serializados en el Inspector de Unity (para `clips`) o internamente por sus propios métodos (`audio`, `last`).
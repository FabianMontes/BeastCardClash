# liveSeter
Este script de Unity, denominado `liveSeter`, se encarga de gestionar y actualizar la representación visual de la vida (o "live" como se le llama en el script) de un personaje dentro de la interfaz de usuario del juego. Su función principal es mostrar tanto una barra de progreso como un valor numérico que reflejan la salud actual de una entidad `Figther`.

El script está diseñado para ser adjuntado a un GameObject que es hijo de un GameObject que contiene el componente `Figther`. A su vez, el GameObject al que está adjunto `liveSeter` debe contener como hijos un componente `Slider` y un componente `TextMeshProUGUI`. Estos componentes UI son los que `liveSeter` manipulará para visualizar la información.

En resumen, `liveSeter` actúa como un puente entre la lógica del juego (la vida del `Figther`) y la representación visual de la interfaz de usuario (el `Slider` y el `TextMeshProUGUI`), asegurando que la información de salud del personaje esté siempre actualizada para el jugador. Este tipo de componente es fundamental en juegos como "Beast Card Clash", donde la información de estado de los personajes (que representan animales autóctonos de Colombia con personalidades académicas) es clave para la estrategia por turnos.

# Métodos

## Métodos de Unity

### Start
El método `Start` es llamado una vez al inicio del ciclo de vida del script, justo antes de la primera ejecución de `Update`. Su propósito en `liveSeter` es inicializar las referencias a los componentes necesarios para su funcionamiento.

```csharp
void Start()
{
    figther = GetComponentInParent<Figther>();
    slider = GetComponentInChildren<Slider>();
    texter = GetComponentInChildren<TextMeshProUGUI>();
}
```

*   **`figther = GetComponentInParent<Figther>();`**: Esta línea busca y asigna una referencia al componente `Figther`. Es crucial destacar que este `Figther` se espera que esté en un GameObject *padre* del GameObject al que está adjunto `liveSeter`. Esto sugiere una jerarquía de GameObjects donde el GameObject padre es la entidad que posee la lógica del `Figther` (probablemente el personaje en sí), y el GameObject con `liveSeter` es un sub-elemento dedicado a su UI, como una barra de salud flotante sobre el personaje.
*   **`slider = GetComponentInChildren<Slider>();`**: Aquí se obtiene una referencia al componente `Slider`. Este `Slider` debe ser un *hijo* del GameObject al que `liveSeter` está adjunto. Será la barra visual que muestra el progreso de la vida.
*   **`texter = GetComponentInChildren<TextMeshProUGUI>();`**: De manera similar, esta línea obtiene una referencia al componente `TextMeshProUGUI`, que también debe ser un *hijo* del mismo GameObject. Este componente se utilizará para mostrar el valor numérico exacto de la vida.

La correcta ejecución de `Start` es vital para que `liveSeter` pueda encontrar y manipular los elementos de UI y la información del `Figther` en los frames subsiguientes.

### Update
El método `Update` se ejecuta una vez por cada frame del juego. En `liveSeter`, su función es actualizar continuamente el `Slider` y el `TextMeshProUGUI` para reflejar la vida actual del `Figther`.

```csharp
void Update()
{
    slider.value = (float)figther.GetPlayerLive() / CombatJudge.CombatJudgeInstance.initialLives;
    texter.text = figther.GetPlayerLive().ToString();
}
```

*   **`slider.value = (float)figther.GetPlayerLive() / CombatJudge.CombatJudgeInstance.initialLives;`**: Esta línea calcula el valor del `Slider`.
    *   `figther.GetPlayerLive()`: Obtiene el valor actual de la vida del `Figther`. Se asume que el componente `Figther` expone un método para consultar su salud.
    *   `CombatJudge.CombatJudgeInstance.initialLives`: Obtiene el valor de la vida inicial (o máxima) de la instancia global de `CombatJudge`. Esto implica que `CombatJudge` es un patrón Singleton que centraliza la información relevante para el combate, incluyendo las vidas iniciales de los `Figther`. Dividir la vida actual por la vida inicial normaliza el valor, haciéndolo apto para el `Slider` (que típicamente espera un valor entre 0 y 1 para su propiedad `value` si su rango se establece por defecto). El `(float)` realiza un `cast` para asegurar que la división sea flotante y el resultado sea preciso.
    *   El resultado de esta operación se asigna a `slider.value`, actualizando visualmente la barra de vida.

*   **`texter.text = figther.GetPlayerLive().ToString();`**: Esta línea actualiza el texto de la UI.
    *   `figther.GetPlayerLive()`: De nuevo, se obtiene el valor actual de la vida del `Figther`.
    *   `.ToString()`: Convierte este valor numérico a una cadena de texto.
    *   El resultado se asigna a `texter.text`, mostrando el valor numérico de la vida del personaje en la UI.

El método `Update` garantiza que la UI de vida del personaje esté siempre sincronizada con su estado interno, proporcionando una retroalimentación constante al jugador sobre el estado de sus "animales colombianos con personalidad académica" en la estrategia por turnos de "Beast Card Clash".

## Getters y Setters
Aunque el script `liveSeter` no contiene getters o setters definidos explícitamente en su propio código, interactúa con ellos a través de otros componentes.

1.  `figther.GetPlayerLive()`: Recupera el valor de la vida actual del componente `Figther` asociado. Este es un método "getter" implícito del componente `Figther`.
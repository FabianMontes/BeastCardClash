# `liveSeter`
Este script, `liveSeter`, se encarga de actualizar dinámicamente la interfaz de usuario (UI) para mostrar el nivel de vida (o salud) de un personaje `Fighter`. Su función principal es vincular el estado de "vida" de un componente `Fighter` (presumiblemente el personaje actual en combate) con dos elementos visuales de la UI: un `Slider` que actúa como una barra de vida y un `TextMeshProUGUI` que muestra el valor numérico de la vida. Este enfoque desacopla la lógica de juego del `Fighter` de su representación visual, permitiendo que el estado de salud se refleje de forma clara y continua en la UI durante el desarrollo del juego, particularmente en un contexto de combate dentro de **Beast Card Clash**.

El script busca y establece sus referencias a los componentes necesarios durante la inicialización (`Start`), y luego en cada cuadro (`Update`), calcula el porcentaje de vida actual para el `Slider` y actualiza el texto con el valor absoluto de la vida.

# Métodos

## Métodos de Unity

### `Start`
Este método se invoca una vez al inicio del ciclo de vida del script, antes de la primera actualización del cuadro. Su propósito principal es obtener y asignar las referencias necesarias a otros componentes presentes en la jerarquía de GameObjects.

```csharp
void Start()
{
    figther = GetComponentInParent<Fighter>();
    slider = GetComponentInChildren<Slider>();
    texter = GetComponentInChildren<TextMeshProUGUI>();
}
```

-   `figther = GetComponentInParent<Fighter>();`: Busca un componente `Fighter` en el GameObject actual o en cualquiera de sus GameObjects padres. Esto indica que el script `liveSeter` está diseñado para ser un componente de un hijo de un GameObject que a su vez contiene el script `Fighter` (por ejemplo, el `Fighter` puede ser el personaje principal, y el `liveSeter` forma parte de un panel de UI adjunto a este). La variable `figther` (observar la posible falta de la 'i' en 'fighter') se utiliza posteriormente para obtener el valor de vida del personaje.
-   `slider = GetComponentInChildren<Slider>();`: Busca un componente `Slider` en el GameObject actual o en cualquiera de sus GameObjects hijos. Este `Slider` se utilizará para representar visualmente la barra de vida del personaje.
-   `texter = GetComponentInChildren<TextMeshProUGUI>();`: Busca un componente `TextMeshProUGUI` en el GameObject actual o en cualquiera de sus GameObjects hijos. Este componente se usará para mostrar el valor numérico de la vida del personaje.

### `Update`
Este método se invoca una vez por cada cuadro del juego. Su función es actualizar continuamente los elementos de la UI (`Slider` y `TextMeshProUGUI`) con el estado actual de la vida del `Fighter`.

```csharp
void Update()
{
    slider.value = (float)figther.GetPlayerLive() / CombatJudge.Instance.initialLives;
    texter.text = figther.GetPlayerLive().ToString();
}
```

-   `slider.value = (float)figther.GetPlayerLive() / CombatJudge.Instance.initialLives;`: Actualiza el valor del `Slider`.
    -   `figther.GetPlayerLive()`: Llama a un método del componente `Fighter` para obtener la vida actual del jugador. Este valor es el numerador de la operación.
    -   `CombatJudge.Instance.initialLives`: Accede a una instancia estática (`Instance`) de un script `CombatJudge` (presumiblemente un Singleton que gestiona aspectos del combate) para obtener el valor de la vida inicial o máxima. Este valor es el denominador.
    -   La división se castea a `float` para asegurar un cálculo de punto flotante y obtener un valor entre 0 y 1, que es lo que un `Slider` típicamente espera para representar un porcentaje.
    -   Esta línea asegura que el `Slider` se actualice para reflejar la proporción de vida actual respecto a la vida máxima.
-   `texter.text = figther.GetPlayerLive().ToString();`: Actualiza el texto mostrado por el componente `TextMeshProUGUI`.
    -   `figther.GetPlayerLive().ToString()`: Obtiene nuevamente el valor de la vida actual del `Fighter` y lo convierte a una cadena de texto para que pueda ser asignado al componente `TextMeshProUGUI`.
    -   Esto asegura que el texto numérico de la vida se mantenga sincronizado con el valor real del personaje.

## Getters y Setters

1.  `GetPlayerLive()`: Obtiene el valor de la vida actual del personaje `Fighter`. Este método no pertenece al script `liveSeter` sino al componente `Fighter` al que hace referencia.
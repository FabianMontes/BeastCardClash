# `DeckManager`
Este script es el responsable central de la gestión de la lógica relacionada con las barajas de cartas en el juego, incluyendo la generación de manos tanto para el jugador como para los enemigos. Actúa como un *Singleton*, asegurando que solo haya una instancia activa en la escena para centralizar el control de las cartas.

### Funcionamiento
1.  **Inicialización de Cartas:** Convierte un enumerador (`DeckCardsList`, que se espera contenga todas las cartas del juego) en una lista interna (`_cardList`) durante la fase `Awake`. Esto proporciona un pool completo de cartas disponibles para el juego.
2.  **Gestión de Puntos:** Mantiene un registro de `points` del jugador, una métrica clave que influye directamente en la calidad de las cartas que pueden aparecer en una mano generada.
3.  **Generación de Manos:**
    *   Proporciona un método (`GenerateHand`) para crear manos de cartas aleatorias y únicas, con un tamaño (`handSize`) configurable.
    *   Cuando genera una mano para un enemigo, aplica un *offset* (absoluto o relativo) a los `points` del jugador para simular variaciones en la dificultad del oponente.
    *   Filtra las cartas elegibles para una mano basándose en el valor `maxCardValue` calculado dinámicamente, asegurando que solo aparezcan cartas apropiadas para el nivel de puntos actual.
4.  **Actualización de Puntos:** Permite modificar los `points` del jugador basándose en el resultado de una batalla, sumando puntos al ganar y restando al perder, lo que impacta la progresión y la calidad de las manos futuras.
5.  **Acceso Global:** Implementa el patrón *Singleton* (`Instance`) para que otros componentes del juego puedan acceder fácilmente a sus funcionalidades sin necesidad de referencias directas en el Inspector.

### Interacción con el Proyecto
*   **Centralización de Lógica de Cartas:** Como *Singleton*, `DeckManager` es un punto de acceso fundamental para cualquier sistema que necesite interactuar con la lógica de las barajas, como el sistema de combate, la interfaz de usuario de la mano del jugador, o los componentes de IA de los enemigos.
*   **Dificultad y Progresión:** La variable `points` y los *offsets* (`absOffset`, `relOffset`) para los enemigos son cruciales para el sistema de progresión y la dificultad adaptativa del juego. A mayor `points` del jugador, mejores cartas podrá obtener y, potencialmente, enemigos más desafiantes (con rangos de puntos modificados) deberá enfrentar.
*   **Dependencia de `DeckCardsList`:** El script asume la existencia de un `enum` llamado `DeckCardsList`. Este `enum` es esencial, ya que define todas las cartas del juego. Se espera que cada valor del `enum` tenga un valor entero subyacente que el `DeckManager` utiliza para filtrar y seleccionar cartas (ej. `(int)card <= maxCardValue`). Es probable que estas entradas representen los "animales autóctonos" o "facultades" mencionados en el `README.md`.

# Métodos

## Métodos de Unity

### `Awake`
Este método se ejecuta cuando la instancia del script se está cargando. Su propósito principal es implementar el patrón *Singleton* y preparar los datos iniciales para la gestión de las cartas.

1.  **Implementación del Singleton:**
    Comprueba si ya existe otra instancia de `DeckManager` en la escena. Si es así, se autodestruye para garantizar que solo haya una instancia activa. En caso contrario, se establece a sí misma como la instancia única (`Instance = this`) y utiliza `DontDestroyOnLoad(gameObject)` para que esta instancia persista a través de las cargas de escena, lo cual es vital para un gestor central como este.
    ```csharp
    void Awake()
    {
        // Lógica del Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // ...
    }
    ```

2.  **Inicialización de `_cardList`:**
    Convierte todos los valores del enumerador `DeckCardsList` en una lista de tipo `List<DeckCardsList>`. Esta lista `_cardList` actúa como el pool completo de cartas disponibles en el juego a partir de las cuales se generarán las manos. Esto asegura que el `DeckManager` tenga conocimiento de todas las cartas posibles desde el inicio. Es fundamental que el `enum` `DeckCardsList` esté definido en otro lugar del proyecto.
    ```csharp
    // ...
        _cardList = Enum.GetValues(typeof(DeckCardsList)).Cast<DeckCardsList>().ToList();
    }
    ```

## Otros métodos

### `public List<DeckCardsList> GenerateHand(bool isEnemy, bool useAbsOffset)`
Este método es el corazón del `DeckManager`, encargado de crear y devolver una lista de cartas que representa una mano, ya sea para el jugador o para un enemigo. Permite ajustar la generación de la mano basándose en la dificultad o el rol del receptor.

1.  **Ajuste de Puntos para Enemigos:**
    Si el parámetro `isEnemy` es `true`, el método primero calcula un rango de puntos para el enemigo. Este rango se basa en los `points` actuales del jugador más un `offset` (que puede ser absoluto (`absOffset`) o relativo (`relOffset`)) para variar la dificultad de la mano enemiga. Después de calcular estos puntos modificados, el método se llama a sí mismo recursivamente con `isEnemy` en `false` para generar la mano final. Es importante notar que esta sobrescritura de `points` solo afecta a la ejecución actual de la recursión, no a los `points` persistentes del jugador.
    ```csharp
    if (isEnemy)
    {
        int offset = useAbsOffset ? absOffset : (int)(points * relOffset);
        int minPoints = points - offset;
        int maxPoints = points + offset;

        points = Random.Range(minPoints, maxPoints); // Los puntos se sobrescriben temporalmente para el enemigo
        return GenerateHand(false, useAbsOffset); // Llamada recursiva
    }
    ```

2.  **Filtro de Cartas Elegibles:**
    Se determina un `maxCardValue` llamando a `SetMaxCardValue()`. Este valor límite se utiliza para filtrar la lista global de cartas (`_cardList`), incluyendo solo aquellas cuyo valor entero es menor o igual a `maxCardValue`. Esto asegura que las cartas generadas estén dentro del rango de "poder" o "costo" adecuado para los `points` actuales.

3.  **Generación de la Mano:**
    Se crea una nueva lista `hand` y se rellena con cartas únicas seleccionadas aleatoriamente de las `eligibleCards`. El proceso se repite hasta que la mano alcanza el `handSize` definido o se agotan las cartas elegibles. `Mathf.Min` se usa para prevenir intentos de añadir más cartas de las disponibles. Las cartas se eligen sin repetición dentro de la misma mano.
    ```csharp
    List<DeckCardsList> hand = new List<DeckCardsList>();
    int cardsToChoose = Mathf.Min(handSize, eligibleCards.Count);

    while (hand.Count < cardsToChoose)
    {
        DeckCardsList chosenCard = eligibleCards[Random.Range(0, eligibleCards.Count)];
        if (!hand.Contains(chosenCard)) hand.Add(chosenCard); // Evita duplicados
    }
    return hand;
    ```
    Este método es crucial para la rejugabilidad y la dificultad del juego, ya que es el encargado de proveer las cartas a los participantes de una batalla.

### `public void SetPoints(bool result)`
Este método permite ajustar los `points` del jugador basándose en el resultado de una partida o batalla. Es una forma sencilla de implementar un sistema de progresión o castigo para el jugador.

*   Si `result` es `true` (victoria), se suman 2 puntos a los `points` actuales.
*   Si `result` es `false` (derrota), se resta 1 punto a los `points` actuales.

```csharp
public void SetPoints(bool result)
{
    points = result ? points + 2 : points - 1;
}
```
Este mecanismo impacta directamente en la calidad de las cartas que el jugador podrá generar en futuras manos, promoviendo una dificultad adaptativa.

### `int SetMaxCardValue()`
Este método interno calcula el valor máximo que una carta puede tener para ser elegible en la mano actual. El cálculo se basa en una combinación del `handSize` y los `points` actuales del jugador.

```csharp
int SetMaxCardValue()
{
    return handSize + points / 5;
}
```
Esta fórmula indica que a medida que los `points` del jugador aumentan, también lo hace el `maxCardValue`, permitiendo la aparición de cartas "más fuertes" o de mayor valor en sus manos. Es una métrica clave para el balance del juego, ligando directamente la experiencia de progresión con la disponibilidad de cartas.

## Getters y Setters

1.  `Instance`: Permite obtener la única instancia activa de `DeckManager` en la escena, siguiendo el patrón *Singleton*. Es de solo lectura.
2.  `PlayerHand`: Propiedad estática de solo lectura que se espera que contenga la lista de cartas de la mano del jugador. **Importante:** Actualmente, este `getter` no está siendo asignado explícitamente dentro del `DeckManager`. Si se pretende que la mano del jugador se almacene aquí para acceso global, debe haber una asignación correspondiente después de llamar a `GenerateHand` para el jugador.
3.  `points`: Un `int` serializado que representa los puntos actuales del jugador. Es público para la edición en el Inspector de Unity y se modifica internamente por `SetPoints` y, temporalmente, por `GenerateHand` al calcular los puntos del enemigo.
4.  `handSize`: Un `int` serializado que define el número de cartas que se generarán en una mano. Es configurable desde el Inspector de Unity.
5.  `absOffset`: Un `int` serializado que representa un desplazamiento absoluto en los puntos al generar una mano de enemigo (`isEnemy = true`). Configurable desde el Inspector.
6.  `relOffset`: Un `float` serializado que representa un desplazamiento relativo (porcentaje) en los puntos al generar una mano de enemigo (`isEnemy = true`). Configurable desde el Inspector.
7.  `playerHand`: Una `List<DeckCardsList>` pública y serializada que está visible en el Inspector de Unity. A pesar de su nombre, no se utiliza directamente dentro del script `DeckManager` para almacenar la mano generada del jugador. Esto podría ser una variable para propósitos de depuración o una redundancia con `PlayerHand`. **Recomendación:** Se sugiere al equipo clarificar el propósito exacto de esta variable y la propiedad `PlayerHand` para evitar confusión.
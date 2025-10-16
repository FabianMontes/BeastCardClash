# `Fighter`
El script `Fighter` es un componente fundamental dentro de **Beast Card Clash**, actuando como la representación lógica y operativa de cada personaje luchador en el juego. Adjunto a un `GameObject` en la escena de Unity, gestiona los atributos clave de un luchador, como su vida, equipo, especie, apariencia visual (skin), y su interacción con el sistema de cartas y el tablero de juego.

Este script es el punto central para la personalización y el control de un personaje. Se encarga de:
- **Inicializar la apariencia visual** del luchador, activando el modelo 3D correcto entre sus hijos.
- **Crear y posicionar el `PlayerToken`**, que es la representación visual del luchador en el tablero.
- **Generar y gestionar el mazo de cartas** del luchador, instanciando las cartas a partir de prefabs y asignándoles sus propiedades.
- **Manejar la vida del luchador**, incluyendo la aplicación de daño y curación, y asegurándose de que la vida se mantenga dentro de los límites definidos.
- **Gestionar la mano de cartas**, permitiendo el robo y la reposición de cartas.
- **Proporcionar métodos para la interacción con el tablero**, como el movimiento del `PlayerToken` entre las rocas.
- **Determinar si el luchador está participando** activamente en el combate actual.

Está diseñado para interactuar con otros sistemas del juego como `DeckManager` para la generación de mazos, `CombatJudge` para la gestión del combate y los momentos del juego, `PlayerToken` para el movimiento en el tablero, y `HolderPlay` para la gestión de cartas en juego. Su diseño prioriza una experiencia de desarrollo ágil y directa, permitiendo una fácil configuración y modificación de los personajes directamente desde el Inspector de Unity.

# Métodos

## Métodos de Unity

### `Start()`
Se ejecuta una vez al inicio del ciclo de vida del script, justo antes del primer `Update()`. Su propósito principal es inicializar el estado del luchador y sus componentes asociados.

- **Configuración Visual:** Desactiva todos los GameObjects hijos que representan posibles visuales del luchador, excepto el que corresponde al índice especificado en `visualFighter`. Esto asegura que solo el modelo correcto del personaje esté visible desde el inicio.
  ```csharp
  _lastVisualPlayer = visualFighter;
  transform.GetChild(4).gameObject.SetActive(false); // Ejemplo: desactiva un visual
  // ...
  transform.GetChild(visualFighter).gameObject.SetActive(true); // Activa el visual deseado
  ```
- **Inicialización del Token:** Si el `playerToken` aún no ha sido asignado, lo instancia utilizando el `tokenPrefab` en una posición inicial sobre `initialStone`. Luego, asigna una referencia a sí mismo (`this`) y a la `initialStone` al `playerToken`, vinculando el token al luchador y a su posición de inicio.
- **Generación del Mazo:** Utiliza el `DeckManager` (una instancia singleton, `DeckManager.Instance`) para generar la lista de cartas que conformarán el mazo del luchador. Itera sobre esta lista, instanciando `cardPrefab` para cada carta, asignándolos como hijos del `deckHolder` (ubicación lógica del mazo) y llamando a su método `Initialize` para configurar sus propiedades.
  ```csharp
  List<DeckCardsList> fighterDeck = DeckManager.Instance.GenerateHand(isEnemy, true);
  Transform deckHolder = transform.GetChild(0).GetChild(0);
  foreach (DeckCardsList card in fighterDeck)
  {
      Card newCard = Instantiate(cardPrefab, deckHolder).GetComponent<Card>();
      newCard.Initialize(card);
  }
  ```

### `Update()`
Se ejecuta en cada frame del juego. Su función principal es monitorear cambios y realizar actualizaciones continuas en el estado del luchador.

- **Actualización Visual:** Detecta si el índice `visualFighter` ha cambiado desde el último frame. Si es así, desactiva el visual anterior y activa el nuevo, permitiendo cambios dinámicos en la apariencia del luchador durante el juego (por ejemplo, al aplicar una nueva skin).
  ```csharp
  if (_lastVisualPlayer != visualFighter)
  {
      transform.GetChild(_lastVisualPlayer).gameObject.SetActive(false);
      transform.GetChild(visualFighter).gameObject.SetActive(true);
      _lastVisualPlayer = visualFighter;
  }
  ```
- **Control de Cartas en Mano:** Contiene una lógica condicional que verifica el estado actual del juego a través de `CombatJudge.Instance.GetSetMoments()` y si el luchador está participando en el combate (`!IsFigthing()`). Si el juego está en el momento de "PickCard" (elegir carta) y el luchador está activo, y además `availableCard` es 0 (indicando que no hay cartas "disponibles" para interacción, probablemente), entonces se fuerza la revelación de todas las cartas en la mano del jugador. Esto sugiere que `availableCard` podría estar relacionado con la selección o juego de cartas.
  ```csharp
  // Si no estamos eligiendo carta o no estamos peleando, no hacemos nada
  if (CombatJudge.Instance.GetSetMoments() != SetMoments.PickCard || !IsFigthing()) return;

  // Si hay cartas disponibles, no hacemos nada
  if (availableCard != 0) return;

  // Si las condiciones anteriores se cumplen, fuerza la revelación de las cartas en mano
  Transform hand = transform.GetChild(0).GetChild(1);
  for (int i = 0; i < handSize; i++)
  {
      hand.GetChild(i).GetComponent<HandCard>().ForceReveal();
  }
  ```

## Otros métodos

### `GetSpecie(): Specie`
Retorna la especie actual del luchador, definida por la variable `specie`.

### `SetTeam(Team newTeam): void`
Establece un nuevo equipo para el luchador, asignando el valor de `newTeam` a la variable `team`.

### `SetSkin(int newSkin): void`
Establece el índice de la skin del luchador, asignando `newSkin` a la propiedad `Skin`. Este cambio afectará la visibilidad de los visuales en el método `Update()`.

### `SetNoSkin(int skin): void`
Asigna una skin aleatoria al luchador, asegurándose de que no sea la misma que la `skin` proporcionada como parámetro. Esto es útil para evitar skins repetidas entre diferentes luchadores. El rango aleatorio actual es de 0 a 5.
```csharp
int a = Random.Range(0, 6);
while (a == skin)
{
    a = Random.Range(0, 6);
}
Skin = a;
```

### `SetRSkin(): void`
Asigna una skin aleatoria al luchador dentro del rango de 0 a 5.

### `SetNoTeam(Team noTeam): void`
Asigna un equipo aleatorio al luchador, pero garantiza que no sea el mismo que el `noTeam` proporcionado. El rango aleatorio actual es para equipos del 0 al 8.
```csharp
team = (Team)Random.Range(0, 8);
while (team == noTeam)
{
    team = (Team)Random.Range(0, 8);
}
```

### `FreeTeam(): void`
Asigna un equipo completamente aleatorio al luchador sin ninguna restricción, seleccionando un valor del enum `Team` del 0 al 8.

### `GetTeam(): Team`
Retorna el equipo actual al que pertenece el luchador, definido por la variable `team`.

### `GetPlayerLive(): int`
Retorna el valor actual de la vida del luchador, almacenado en `fighterLive`.

### `SetPlayerLive(int playerLive): void`
Establece directamente el valor de la vida del luchador a `playerLive`.

### `AddPlayerLive(int playerLife): void`
Modifica la vida del luchador sumándole `playerLife`.
- **Registro de Daño/Curación:** Actualiza la propiedad `NoHurt` a `true` si el cambio de vida es positivo o cero (indicando curación o ningún daño), y a `false` si es negativo (daño).
- **Clampeo de Vida:** Asegura que la vida del luchador no exceda el máximo (`CombatJudge.Instance.initialLives`) ni caiga por debajo de cero, ajustándola si es necesario.
```csharp
NoHurt = playerLife >= 0; // Si playerLife es >= 0, no sufrió daño (o se curó)
fighterLive += playerLife;
if (fighterLive > CombatJudge.Instance.initialLives) fighterLive = CombatJudge.Instance.initialLives;
if (fighterLive < 0) fighterLive = 0;
```

### `RandomSpecie(): void`
Establece la especie del luchador a `Bear` y asigna una skin aleatoria de oso (del 0 al 1), lo que implica que las skins 0 y 1 son variantes visuales para la especie oso.

### `MovePlayer(RockBehavior rocker): void`
Mueve el `playerToken` del luchador a una nueva roca, actualizando la propiedad `rocky` de `playerToken` con la `rocker` proporcionada.

### `GetPicked(): Card`
Obtiene la carta que ha sido seleccionada o "recogida" por el jugador. Delega esta lógica a un componente `HolderPlay` que se encuentra como hijo del luchador (`GetComponentInChildren<HolderPlay>()`).

### `PlayCard(Card card): void`
Indica que una carta debe ser jugada. También delega esta acción al componente `HolderPlay`, pasándole la `card` que se debe jugar.

### `DrawCard(int index): void`
Roba una carta del mazo y la coloca en un slot específico de la mano del jugador.
- **Selección de Carta:** Elige una carta aleatoria del mazo (`transform.GetChild(0).GetChild(0)`).
- **Verificación de Actividad:** Asegura que la carta seleccionada esté activa (no haya sido robada previamente) buscando una activa si la primera elegida no lo está.
- **Asignación a la Mano:** Asigna la carta robada al componente `HandCard` en el slot de la mano correspondiente al `index`.
- **Desactivación:** Desactiva el `GameObject` de la carta en el mazo para indicar que ya no está disponible para robar.
```csharp
int drawnCardIndex = Random.Range(0, deckSize);
Transform deck = transform.GetChild(0).GetChild(0);
Transform hand = transform.GetChild(0).GetChild(1);

// Busca una carta activa si la elegida inicialmente no lo está
while (!deck.GetChild(drawnCardIndex).gameObject.activeSelf)
{
    drawnCardIndex = (drawnCardIndex + 1) % deckSize;
}

hand.GetChild(index).GetComponent<HandCard>().SetCard(deck.GetChild(drawnCardIndex).GetComponent<Card>());
deck.GetChild(drawnCardIndex).gameObject.SetActive(false);
```

### `RefillHand(): void`
Recorre todos los slots de la mano del luchador. Si un slot está vacío (no contiene una carta `null`), llama a `DrawCard` para robar una nueva carta y llenar ese slot. Finalmente, resetea la variable `availableCard` a 0.

### `IsFigthing(): bool`
Determina si el luchador está participando en el combate actual.
- **Lógica de Bitmask:** Obtiene un valor entero (`figthers`) de `CombatJudge.Instance.GetPlayersFighting()`. Este valor parece ser un bitmask donde cada bit representa si un luchador con un `indexFighter` particular está activo en el combate.
- **Verificación de Participación:** Itera a través de los bits del `figthers` para verificar si el bit correspondiente al `indexFighter` de este luchador está establecido. Si lo está, el luchador está en combate; de lo contrario, no.
```csharp
int figthers = CombatJudge.Instance.GetPlayersFighting();
int a = 0;
while (figthers > 0)
{
    int red = figthers % 2; // Obtiene el bit menos significativo
    figthers = (int)Mathf.Floor(figthers / 2); // Desplaza los bits a la derecha

    if (a == indexFighter) return red != 0; // Comprueba si el bit del luchador está establecido
    a++;
}
return false;
```
Este método permite que el `CombatJudge` notifique a los luchadores si deben estar activos o no en una ronda o fase específica, usando un método eficiente de bitmasks.

### `ThrowCard(): void`
Lanza o descarta una carta. Delega esta acción al componente `HolderPlay`, llamando a `PlayCard` con `null` como argumento, lo que probablemente indica una acción de descarte en el `HolderPlay`.

## Getters y Setters

1. `Skin: int`: Obtiene el índice de la skin actual del luchador o establece un nuevo índice para la skin.
2. `NoHurt: bool`: Obtiene un valor que indica si el último cambio de vida fue positivo (curación) o nulo (ningún daño). Se establece automáticamente en el método `AddPlayerLive`.
3. `GetSpecie: Specie`: Obtiene la especie actual del luchador.
4. `SetTeam: void`: Establece el equipo del luchador.
5. `SetSkin: void`: Establece el índice de la skin del luchador.
6. `SetNoSkin: void`: Establece una skin aleatoria que no coincida con una skin dada.
7. `SetRSkin: void`: Establece una skin aleatoria.
8. `SetNoTeam: void`: Establece un equipo aleatorio que no coincida con un equipo dado.
9. `FreeTeam: void`: Establece un equipo aleatorio sin restricciones.
10. `GetTeam: Team`: Obtiene el equipo actual del luchador.
11. `GetPlayerLive: int`: Obtiene la vida actual del luchador.
12. `SetPlayerLive: void`: Establece la vida del luchador a un valor específico.
13. `AddPlayerLive: void`: Añade o resta vida al jugador, clamped entre un mínimo y un máximo, y actualiza `NoHurt`.
14. `RandomSpecie: void`: Establece la especie a Oso y una skin de oso aleatoria.
15. `MovePlayer: void`: Mueve el token del jugador a una roca específica.
16. `GetPicked: Card`: Obtiene la carta que ha sido seleccionada por el jugador.
17. `PlayCard: void`: Ejecuta la acción de jugar una carta.
18. `ThrowCard: void`: Ejecuta la acción de lanzar o descartar una carta.
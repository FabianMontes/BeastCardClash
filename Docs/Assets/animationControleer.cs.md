# animationControleer
Este script `animationControleer` es el encargado de gestionar y sincronizar las animaciones de un personaje en el juego con su estado actual y las fases de combate. Funciona como un puente entre la lógica del juego (velocidad del jugador, estado de combate, condiciones de victoria/derrota, tipo de daño elemental) y el componente `Animator` del modelo del personaje.

El script se ejecuta muy temprano en el ciclo de vida del juego, gracias al atributo `[DefaultExecutionOrder(1)]`, lo que asegura que las animaciones se inicialicen y actualicen de forma oportuna. Una de sus funciones principales es seleccionar dinámicamente el modelo 3D del personaje a animar de entre varios modelos hijos, basándose en el índice asociado al luchador (`Figther`).

En resumen, `animationControleer` asegura que el personaje visible en pantalla no solo esté presente, sino que su comportamiento visual (animaciones) refleje con precisión lo que está ocurriendo en la partida.

## Interacción con otros componentes del proyecto:
Este script interactúa con los siguientes componentes/sistemas del proyecto:
*   **`PlayerToken`**: Debe estar adjunto al mismo GameObject que `animationControleer`. Proporciona acceso al objeto `Figther` asociado y a la velocidad actual del jugador a través del método `Speed()`.
*   **`Figther`**: Un objeto (presumiblemente una clase custom) que se obtiene a través de `PlayerToken`. Contiene el `indexFigther` (para seleccionar el modelo correcto) y la propiedad `noHurt` (para determinar si el personaje ganó el combate sin recibir daño).
*   **`Animator`**: El componente de Unity que gestiona las animaciones del modelo 3D. El script busca este componente en los hijos del GameObject activo.
*   **`CombatJudge`**: Una clase global y estática (`CombatJudge.CombatJudgeInstance`) que proporciona el estado actual del combate (`GetSetMoments()`) y el tipo de combate elemental (`CombatType`).

El script también hace uso de `OutlineFx`, aunque no hay una llamada directa a sus métodos dentro de este script específico, su presencia en el `using` indica que podría ser utilizado por componentes hijos o en el contexto más amplio del GameObject.

# Métodos

## Métodos de Unity

### Awake
Este script no implementa el método `Awake`.

### Start
Este método se llama una vez al inicio, antes de la primera actualización del frame, después de que el GameObject que contiene el script se ha creado y está activo. Su propósito es inicializar las referencias a otros componentes y establecer el estado inicial de las animaciones.

```csharp
void Start()
{
    player = GetComponent<PlayerToken>();
    figther = player.player;
    print(figther.indexFigther);
    setModel(figther.indexFigther);
    animato = transform.GetChild(figther.indexFigther).GetComponentInChildren<Animator>();
    animato.SetBool("isFigthing",true);
}
```

**Funcionamiento:**
1.  **Obtener `PlayerToken`**: Recupera el componente `PlayerToken` del mismo GameObject.
2.  **Obtener `Figther`**: Accede a la instancia de `Figther` a través de la propiedad `player` del `PlayerToken`.
3.  **Depuración**: Imprime el `indexFigther` del luchador en la consola para propósitos de depuración.
4.  **Selección del modelo**: Llama al método `setModel` para activar el modelo 3D correcto basado en el `indexFigther`.
5.  **Obtener `Animator`**: Busca el componente `Animator` dentro de los hijos del modelo activo. Se asume que el `Animator` se encuentra en algún descendiente directo del GameObject del modelo seleccionado.
6.  **Inicializar animación de combate**: Establece el parámetro booleano `"isFigthing"` del `Animator` a `true`, indicando que el personaje está en un estado de combate desde el inicio.

### Update
Este método se llama una vez por cada frame del juego. Su función es mantener las animaciones del personaje sincronizadas con el estado del juego en tiempo real.

```csharp
void Update()
{
    animato.SetFloat("Speed", player.Speed());
    animato.SetBool("EndTurn", CombatJudge.CombatJudgeInstance.GetSetMoments() == SetMoments.Result);
    animato.SetBool("didWin", figther.noHurt);
    animato.SetInteger("ElementHurt", (int) CombatJudge.CombatJudgeInstance.CombatType);
}
```

**Funcionamiento:**
1.  **Actualizar velocidad**: Establece el parámetro flotante `"Speed"` del `Animator` con el valor retornado por `player.Speed()`, permitiendo que las animaciones de movimiento (caminar, correr) se adapten dinámicamente.
2.  **Detectar fin de turno**: Actualiza el parámetro booleano `"EndTurn"` del `Animator`. Se establece a `true` si el estado actual del `CombatJudge` indica que se ha llegado al momento de `Result` (fin del turno de combate o fase de resolución).
3.  **Detectar victoria sin daño**: Actualiza el parámetro booleano `"didWin"` del `Animator` basándose en la propiedad `figther.noHurt`. Si `noHurt` es `true`, el personaje ganó el combate sin recibir daño.
4.  **Detectar tipo de daño elemental**: Actualiza el parámetro entero `"ElementHurt"` del `Animator` con el tipo de combate elemental actual (`CombatType`) proporcionado por `CombatJudge`. Esto podría activar animaciones o efectos visuales específicos según el tipo de elemento involucrado en el daño.

## Otros métodos

### void setModel(int index)
Este método es responsable de activar el modelo 3D correcto del personaje, deshabilitando todos los demás modelos que puedan ser hijos del GameObject principal.

```csharp
void setModel(int index)
{
    transform.GetChild(0).gameObject.SetActive(false);
    transform.GetChild(1).gameObject.SetActive(false);
    transform.GetChild(2).gameObject.SetActive(false);
    transform.GetChild(3).gameObject.SetActive(false);

    transform.GetChild(index).gameObject.SetActive(true);
}
```

**Funcionamiento:**
1.  **Desactivación general**: Primero, itera implícitamente a través de los primeros cuatro hijos del GameObject al que está adjunto este script y los desactiva (`SetActive(false)`). Esto asume que el GameObject puede tener hasta 4 modelos diferentes como hijos directos, cada uno representando una posible variante de personaje (por ejemplo, los diferentes animales/facultades).
2.  **Activación específica**: Luego, activa únicamente el GameObject hijo cuyo índice coincide con el `index` proporcionado. Este `index` proviene del `figther.indexFigther`, asegurando que solo el modelo correspondiente al luchador actual esté visible.

> [!NOTE]
> La desactivación explícita de `transform.GetChild(0)` a `transform.GetChild(3)` sugiere una estructura fija de modelos predefinidos. Si el número de modelos puede variar, sería más robusto iterar sobre `transform.childCount` para desactivar todos los hijos antes de activar el deseado.

## Getters y Setters
El script `animationControleer` no contiene métodos custom que actúen exclusivamente como getters o setters para sus propias propiedades.
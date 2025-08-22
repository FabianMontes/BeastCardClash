# `warningdice.cs`

## 1. Propósito General
Este script gestiona la visibilidad de un elemento visual (una imagen) en la interfaz de usuario. Su función principal es actuar como un indicador visual que se activa solo cuando es el turno del `Figther` (luchador) asociado, durante las fases específicas de selección o lanzamiento de dados en el sistema de combate.

## 2. Componentes Clave

### `warningdice`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, controla la habilitación y deshabilitación de un componente `Image` basándose en el estado actual del juego y en el turno del jugador asociado. Está diseñada para ser parte de un GameObject que es hijo de un `Figther` y que contiene un componente `Image`.

- **Variables Públicas / Serializadas:**
    Este script no expone variables directamente en el Inspector de Unity (`[SerializeField]`) ni tiene variables públicas. Todas sus dependencias y referencias se obtienen en tiempo de ejecución.

- **Métodos Principales:**
    - `void Start()`: Este método se ejecuta una vez al inicio del ciclo de vida del script. Su propósito es inicializar las referencias internas necesarias:
        - Obtiene una referencia al componente `Figther` que se encuentra en el GameObject padre. Se espera que este script sea un hijo de un GameObject que contenga un `Figther`.
        - Obtiene una referencia al componente `Image` que reside en el mismo GameObject que este script.

        ```csharp
        Figther figther;
        Image image;
        void Start()
        {
            figther = GetComponentInParent<Figther>();
            image = GetComponent<Image>();
        }
        ```

    - `void Update()`: Este método se llama en cada frame del juego. Es el corazón de la lógica de visibilidad del indicador. Dentro de `Update`, el script realiza las siguientes comprobaciones para decidir si la imagen debe estar visible:
        1.  Obtiene el `SetMoments` actual del `Combatjudge` global.
        2.  Verifica si el `SetMoments` actual es `PickDice` o `RollDice`. Estas parecen ser fases del juego relacionadas con la manipulación de dados.
        3.  Si la fase coincide, verifica si el turno actual (`Combatjudge.combatjudge.turn()`) corresponde al índice del luchador asociado (`figther.indexFigther`).
        4.  Si ambas condiciones (fase de dados y turno del luchador) son verdaderas, el componente `Image` se habilita (`image.enabled = true`).
        5.  En cualquier otro caso (la fase no es de dados, o no es el turno del luchador), el componente `Image` se deshabilita (`image.enabled = false`).

        ```csharp
        void Update()
        {
            SetMoments momo = Combatjudge.combatjudge.GetSetMoments();
            if(momo == SetMoments.PickDice || momo == SetMoments.RollDice)
            {
                if(Combatjudge.combatjudge.turn() == figther.indexFigther)
                {
                    image.enabled = true;
                    return;
                }
            }
            image.enabled = false;
        }
        ```

- **Lógica Clave:**
    La lógica central del script se basa en un sistema de detección de estado y turno. En cada fotograma, el script consulta el estado global del combate (`Combatjudge`) para determinar la fase actual del juego (`SetMoments`) y el índice del jugador cuyo turno es. La imagen solo se mostrará si la fase del juego está dentro de las etapas de "selección de dados" o "lanzamiento de dados" Y, simultáneamente, es el turno del luchador al que este `warningdice` está asociado. Fuera de estas condiciones, el indicador permanece oculto, proporcionando una retroalimentación visual muy específica al jugador.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script asume la presencia de un componente `Image` en el mismo GameObject donde está adjunto. Además, asume que su GameObject padre tendrá un componente `Figther`. No utiliza el atributo `[RequireComponent]` explícitamente, pero funcionalmente lo requiere. También depende fuertemente de una instancia estática `Combatjudge.combatjudge`, que se espera que sea un sistema global de gestión de combate.

- **Eventos (Entrada):**
    El script no se suscribe a eventos de UI (como `onClick`) ni a eventos personalizados (`UnityEvent`, `Action`). Su funcionamiento se basa enteramente en el ciclo de vida de `MonoBehaviour` (`Start`, `Update`) y en la consulta directa del estado global del `Combatjudge` en cada `Update`.

- **Eventos (Salida):**
    Este script no invoca ningún evento (`UnityEvent`, `Action`, etc.) ni notifica a otros sistemas. Su única función es reaccionar visualmente a cambios de estado internos del juego sin propagar información.
# `teamselect.cs`

## 1. Propósito General
Este script de Unity gestiona la visualización del equipo seleccionado en la interfaz de usuario y permite la selección del mismo. Sirve como un puente entre el estado global del juego (`GameState`) y un elemento visual (`Image`) que representa el equipo actual.

## 2. Componentes Clave

### `teamselect`
- **Descripción:** La clase `teamselect` es un `MonoBehaviour` que se encarga de actualizar dinámicamente el `Sprite` de un componente `Image` en función del equipo actual seleccionado en el juego. También proporciona un método para cambiar el equipo a través de la interfaz de usuario. Este script espera estar adjunto a un `GameObject` que también tenga un componente `Image`.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] bool Follow`: Un booleano que, cuando es `true`, indica que el `Sprite` del componente `Image` adjunto debe actualizarse continuamente para reflejar el equipo seleccionado en `GameState.singleton.team` durante cada cuadro. Si es `false`, el script no actualizará automáticamente el `Sprite`.
    - `[SerializeField] Sprite[] teams`: Un array de `Sprite` que almacena los diferentes `Sprites` (imágenes) para cada equipo disponible en el juego. Se espera que el índice de este array corresponda al valor numérico del `enum` que representa a cada equipo.
    - `Image image`: Una referencia al componente `Image` adjunto al mismo `GameObject` que este script. Esta referencia se obtiene automáticamente en el método `Start()` y es el componente cuyo `Sprite` será modificado.

- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se llama una vez al inicio, después de que el script se ha cargado. Su función principal es obtener una referencia al componente `Image` que está adjunto al mismo `GameObject`. Esto es crucial para que el script pueda manipular la imagen mostrada.

    - `void Update()`: Este método del ciclo de vida de Unity se llama una vez por cuadro. Contiene la lógica para actualizar el `Sprite` de la imagen. Si la variable `Follow` está configurada a `true`, el `Sprite` de la imagen se establece al `Sprite` correspondiente del array `teams`, utilizando el valor del equipo actual (`GameState.singleton.team`) como índice. Esto permite una actualización visual en tiempo real del equipo.

    - `public void selectTeam(int team)`: Este método público está diseñado para ser invocado desde la interfaz de usuario, típicamente a través de un evento `onClick` de un botón. Toma un entero `team` como parámetro, el cual se convierte a un tipo `Team` (presumiblemente un `enum` definido en otro lugar) y luego se utiliza para actualizar el equipo seleccionado globalmente a través del método `SetTeam` del singleton `GameState`.

- **Lógica Clave:**
    La lógica central de este script reside en la sincronización del `Sprite` visible con el estado del equipo actual del juego. El método `Update` monitorea la bandera `Follow` para determinar si debe mantener la representación visual del equipo actualizada. Cuando `Follow` es `true`, utiliza el valor numérico del equipo almacenado en `GameState.singleton.team` para indexar el array `teams` y asignar el `Sprite` correspondiente a la `Image`. La función `selectTeam` proporciona el mecanismo para que elementos externos (como botones de la UI) puedan interactuar y cambiar el equipo globalmente.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Aunque no se utiliza el atributo `[RequireComponent]`, este script implícitamente requiere un componente `UnityEngine.UI.Image` en el mismo `GameObject` para funcionar correctamente, ya que intenta obtener y manipular este componente en tiempo de ejecución.

- **Eventos (Entrada):**
    - El método público `selectTeam(int team)` está diseñado para ser invocado externamente, muy probablemente por eventos de interfaz de usuario, como el `onClick()` de un `Button` en Unity, donde el valor `int` del equipo sería pasado como un parámetro dinámico.

- **Eventos (Salida):**
    - Este script no expone ni invoca explícitamente ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas. Su interacción con el resto del juego se realiza a través del singleton `GameState` (modificando `GameState.singleton.SetTeam` y leyendo `GameState.singleton.team`).
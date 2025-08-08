# `UIManager.cs`

## 1. Propósito General
El script `UIManager` es el encargado de gestionar la visibilidad de los diferentes menús principales del juego, como el menú de inicio, los créditos y el tutorial. Su función principal es asegurar que solo un menú esté activo y visible para el jugador en un momento dado, facilitando la navegación entre las secciones informativas del juego antes de iniciar la partida.

## 2. Componentes Clave

### `UIManager`
La clase `UIManager` es un `MonoBehaviour`, lo que significa que debe ser adjuntada a un GameObject en la escena de Unity para funcionar. Su propósito es controlar el estado de activación (visibilidad) de varios objetos `Canvas` que representan los menús.

**Variables Serializadas**
Este script expone las siguientes variables serializadas, lo que permite asignar los respectivos `Canvas` de Unity directamente desde el Inspector:

```csharp
[SerializeField] private Canvas StartMenu;
[SerializeField] private Canvas CreditsMenu;
[SerializeField] private Canvas TutorialMenu;
```
*   `StartMenu`: Representa el `Canvas` del menú principal o de inicio del juego.
*   `CreditsMenu`: Representa el `Canvas` que contiene la información de los créditos.
*   `TutorialMenu`: Representa el `Canvas` que muestra las instrucciones o el tutorial del juego.

Estas variables `private` con el atributo `[SerializeField]` garantizan que, aunque no son accesibles desde otros scripts, pueden ser configuradas fácilmente por los diseñadores en el editor de Unity.

**Métodos Principales**
El `UIManager` define varios métodos para controlar el flujo de los menús:

*   `void Start()`: Este es un método del ciclo de vida de Unity que se invoca una vez al inicio del script, antes de la primera actualización del frame. Su función aquí es establecer el menú de inicio (`StartMenu`) como el menú activo por defecto cuando la escena carga, llamando a `ShowStartMenu()`.
    ```csharp
    void Start()
    {
        ShowStartMenu();
    }
    ```

*   `void Update()`: Este método del ciclo de vida de Unity se llama una vez por frame. En la implementación actual, este método está vacío, lo que indica que no hay lógica de actualización continua o en tiempo real necesaria para la gestión de menús en este script.

*   `public void ShowStartMenu()`: Este método público se encarga de activar el `StartMenu` y desactivar (`SetActive(false)`) el `CreditsMenu` y el `TutorialMenu`. Está diseñado para ser invocado, por ejemplo, cuando el jugador hace clic en un botón "Jugar" o "Inicio" en otro menú.
    ```csharp
    public void ShowStartMenu()
    {
        StartMenu.gameObject.SetActive(true);
        CreditsMenu.gameObject.SetActive(false);
        TutorialMenu.gameObject.SetActive(false);
    }
    ```

*   `public void ShowCreditsMenu()`: Similar a `ShowStartMenu()`, este método activa el `CreditsMenu` y desactiva los otros dos menús. Es el método a invocar cuando el jugador desea ver la sección de créditos.

*   `public void ShowTutorialMenu()`: Este método activa el `TutorialMenu` mientras desactiva el `StartMenu` y el `CreditsMenu`. Se utiliza para mostrar las instrucciones o guía del juego al jugador.

**Lógica Clave**
La lógica central de este script es muy directa: en cualquier momento, solo uno de los objetos `Canvas` designados como menú estará activo (`.gameObject.SetActive(true)`), mientras que los demás permanecerán inactivos. Esto se logra mediante los métodos `Show...Menu()`, que establecen un estado exclusivo de visibilidad para cada menú.

## 3. Dependencias y Eventos
*   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, lo que significa que no tiene dependencias obligatorias de otros componentes en el mismo GameObject.

*   **Eventos (Entrada):** Los métodos `ShowStartMenu()`, `ShowCreditsMenu()`, y `ShowTutorialMenu()` son métodos públicos diseñados para ser enlazados a los eventos `OnClick()` de botones de la interfaz de usuario en el editor de Unity. Por ejemplo, un botón "Créditos" en el `StartMenu` invocará `ShowCreditsMenu()` al ser presionado.

*   **Eventos (Salida):** El `UIManager` actual no emite (`invokes`) eventos de Unity (`UnityEvent` o `Action`) ni notifica directamente a otros sistemas sobre cambios en el estado de los menús. Su interacción es unidireccional, manipulando directamente el estado de los `Canvas` asignados.
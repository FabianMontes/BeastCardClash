# PickerCard
`PickerCard` es un script de Unity que se encarga de gestionar la representación visual de una carta "seleccionada" (o "pickeada") dentro de la interfaz de usuario del juego **Beast Card Clash**. Su función principal es mostrar de forma dinámica la carta que un jugador (o entidad `Figther`) tiene actualmente seleccionada, actuando como un componente de UI que reacciona a los cambios en el estado del jugador.

Este componente se espera que esté adjunto a un GameObject que sirva como contenedor para la UI de una carta seleccionada. Su funcionamiento se basa en la interacción con otros dos componentes clave:
1.  **`HandCard`**: Un componente hijo que es responsable de la lógica y la visualización específica de una carta individual en la UI. `PickerCard` le pasa la información de la carta a mostrar.
2.  **`Figther`**: Un componente padre que representa al jugador o entidad que está "seleccionando" la carta. `PickerCard` consulta a este componente para saber qué carta debe mostrar.

En resumen, `PickerCard` actúa como un "observador" del estado de la carta seleccionada de un `Figther`, actualizando la `HandCard` visual asociada solo cuando es necesario, lo que contribuye a una interfaz de usuario reactiva y eficiente en el contexto de un juego de cartas por turnos como **Beast Card Clash**.

# Métodos

## Métodos de Unity

### Start
Este método se ejecuta una vez al inicio del ciclo de vida del script, justo antes de la primera actualización (Update). Su propósito es inicializar las referencias a otros componentes con los que `PickerCard` necesita interactuar.

```csharp
void Start()
{
    card = GetComponentInChildren<HandCard>();
    player = GetComponentInParent<Figther>();
    card.SetCard(null);
}
```

*   **`card = GetComponentInChildren<HandCard>();`**: Busca y asigna una referencia al componente `HandCard` que se encuentre en alguno de los GameObjects hijos de este `PickerCard`. Esto asume que el `PickerCard` es un contenedor que tiene la representación visual de la carta como un hijo.
*   **`player = GetComponentInParent<Figther>();`**: Busca y asigna una referencia al componente `Figther` que se encuentre en alguno de los GameObjects padres de este `PickerCard`. Esto establece el vínculo entre la UI de la carta seleccionada y el jugador (o `Figther`) al que pertenece.
*   **`card.SetCard(null);`**: Inicializa la `HandCard` para que no muestre ninguna carta al comienzo. Esto asegura que la interfaz de la carta seleccionada esté limpia hasta que el `Figther` realmente "seleccione" una carta.

### Update
Este método se llama una vez por cada frame del juego. Su función es verificar continuamente si la carta que el jugador tiene "seleccionada" ha cambiado y, si es así, actualizar la visualización de la `HandCard`.

```csharp
void Update()
{
    if(isPlaying != (player.getPicked() != null))
    {
        isPlaying = player.getPicked() != null;
        card.SetCard(player.getPicked());
    }
}
```

*   **`if(isPlaying != (player.getPicked() != null))`**: Esta es la lógica central de optimización del `Update`.
    *   `player.getPicked() != null`: Consulta al `Figther` asociado para saber si actualmente tiene alguna carta seleccionada.
    *   `isPlaying`: Es una variable de estado interna que almacena si, en el último ciclo, se estaba mostrando una carta.
    *   La condición `isPlaying != (player.getPicked() != null)` evalúa si el estado actual de la carta seleccionada por el jugador (hay una carta o no) es diferente del estado que `PickerCard` estaba mostrando previamente. Esto evita actualizar la `HandCard` en cada frame si no hay un cambio real en la selección de la carta, mejorando el rendimiento.
*   **`isPlaying = player.getPicked() != null;`**: Si se detecta un cambio, esta línea actualiza la variable `isPlaying` para reflejar el nuevo estado de la carta seleccionada.
*   **`card.SetCard(player.getPicked());`**: Si hay un cambio de estado, se llama al método `SetCard` de la `HandCard` para que muestre la carta que el `Figther` tiene seleccionada. Si `player.getPicked()` devuelve `null`, la `HandCard` se configurará para no mostrar ninguna carta.

## Otros métodos
Este script no define métodos públicos o privados adicionales fuera de los métodos de ciclo de vida de Unity (`Start`, `Update`). Todas sus operaciones se realizan dentro de estos dos métodos.

## Getters y Setters
Este script no define métodos que actúen como *getters* o *setters* para su estado interno o para exponer datos a otros componentes. Su rol es principalmente reactivo, utilizando *getters* de otros componentes (`player.getPicked()`) para actualizar su propia UI.
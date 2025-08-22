using UnityEngine;

[DefaultExecutionOrder(1)]
public class BotPlayer : MonoBehaviour
{
    // Variables de instancia
    Figther figther; // Bot que posee este script
    Transform hand; // Mano de cartas del bot (disponibles como hijos del GameObject)

    void Start()
    {
        // Inicializa las variables
        figther = GetComponent<Figther>();
        hand = transform.GetChild(0).GetChild(1);
        picking = false;
    }

    // Tiempo actual y total. Necesario para contabilizar
    float time;
    float total;

    // Bandera, que evita que se repita el código erróneamente en cada frame
    bool picking;

    void Update()
    {
        // Si no hay bot, no hace nada
        if (figther == null) return;

        // Determina el tiempo de pensar antes de elegir la carta
        // Verifica si el bot está en batalla (sin haber perdido, por ejemplo), en momento para seleccionar y no haya seleccionado carta en ese turno
        // Si se cumple, establece el tiempo para "pensar" del bot
        if (figther.IsFigthing() && Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickCard && figther.getPicked() == null && !picking)
        {
            // Almacena el momento en el que se acciona este bloque
            time = Time.time;

            // Establece un tiempo aleatorio entre 1 y 3 segundos (para "pensar")
            total = Random.Range(1.0f, 3.0f);

            // Activa la bandera
            picking = true;
        }
        // Determina y selecciona la carta cuando ya se acabó el tiempo de pensar
        // Verifica si el bot ya ha elegido y esté en momento para seleccionar
        // Si se cumple, empieza a elegir la carta
        else if (picking && Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickCard)
        {
            // Verifica si ya transcurrió el tiempo establecido
            // Si es así, elige carta
            if (Time.time - time > total)
            {
                // Reestablece la bandera
                picking = false;

                // Elige una carta al azar
                int a = Random.Range(0, 6); // a: Índice de la carta a elegir
                int b = 0; // b: Contador para limitar la búsqueda de cartas
                HandCard card = hand.transform.GetChild(a).GetComponent<HandCard>(); // Obtiene la carta

                // Verifica que la carta elegida exista y sea seleccionable
                // Repite hasta 100 veces (usando b)
                while ((card == null || !card.isClickable()) && b < 100)
                {
                    a = (a + 1) % 6; // Se mantiene en el rango [0, 5] usando mod (%)
                    card = hand.transform.GetChild(a).GetComponent<HandCard>(); // Obtiene la carta
                    b++;
                }

                // Elige la carta que salió del bucle
                card.SelectedCard();
            }
        }
        // Establece el tiempo antes de lanzar el dado cuando es turno del bot
        // Verifica si el bot está en momento de tomar el dado y es su turno
        // Si se cumple, establece el tiempo para "lanzar" el dado
        else if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickDice && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            // Almacena el momento en el que se acciona este bloque
            time = Time.time;

            // Establece un tiempo aleatorio entre 0.5 y 3 segundos (para "lanzar")
            total = Random.Range(0.5f, 3.0f);

            // Lanza el dado
            FindFirstObjectByType<dice>().Roll();
        }
        // Lanza el dado cuando ya se acabó el tiempo de lanzar
        // Verifica si el tiempo de espera ya acabó y es nuestro turno
        // Si se cumple, lanza el dado
        else if (Time.time - time > total && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            // Termina de lanzar el dado
            FindFirstObjectByType<dice>().Unroll();
        }
        // Establece el tiempo antes de elegir el tipo de ataque
        // Verifica si el bot está en momento de elegir ataque y es su turno
        // Si se cumple, establece el tiempo para "pensar"
        else if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.SelecCombat && !picking && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            // Almacena el momento en el que se acciona este bloque
            time = Time.time;

            // Establece un tiempo aleatorio entre 0.5 y 2 segundos (para "pensar")
            total = Random.Range(0.5f, 2.0f);

            // Activa la bandera
            picking = true;
        }
        // Elige el elemento de ataque cuando ya se acabó el tiempo de pensar
        // Verifica si el tiempo de espera ya acabó y es nuestro turno
        // Si se cumple, elige el elemento
        // TODO: Estar pendiente de esta parte en especial
        else if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.SelecCombat && picking && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            // Verifica si ya transcurrió el tiempo establecido
            if (Time.time - time > total)
            {
                // Reestablece la bandera
                picking = false;

                // Inicializa el arreglo. Cada elemento almacenará la cantidad de cartas disponibles de cada elemento
                int[] elem = new int[4];

                // Luego rellena el arreglo con los valores correspondientes
                for (int i = 0; i < hand.childCount; i++) // ? childcount evita errores si la mano cambia de tamaño alguna vez
                {
                    // Extrae la carta actual
                    HandCard card = hand.transform.GetChild(i).GetComponent<HandCard>();

                    // Salta si una carta no existe
                    if (card == null) continue;

                    // Pone la carta en el arreglo 
                    elem[(int)card.GetCard().GetElement()] = elem[(int)card.GetCard().GetElement()] + 1;
                }

                // Índice de la carta de mayor valor (actual)
                int big = 0;

                // Itera sobre los elementos para hallar el de mayor valor (corregido)
                for (int i = 1; i < 4; i++)
                {
                    if (elem[i] > elem[big]) big = i;
                }

                // Elige el elemento con más cartas
                GetComponentInChildren<SelectType>().PickElement(big);
            }
        }
    }

    // Prepara al bot para elegir a qué roca del tablero moverse.
    // Inicia un temporizador para simular que "piensa" su decisión.
    RockBehavior[] rocks;
    public void PickRock(RockBehavior[] rocks)
    {
        // Desactiva la bandera (esencial)
        picking = false;

        // Almacena el momento en el que se acciona este bloque
        time = Time.time;

        // Establece un tiempo aleatorio entre 1 y 2 segundos (para "pensar")
        total = Random.Range(1.0f, 2.0f);

        // Almacena en el arreglo las rocas disponibles
        this.rocks = rocks;
    }

    // Toma la decisión de elegir roca una vez se acaba el tiempo de pensar
    public void ThinkingRocks()
    {
        // Verifica si ya transcurrió el tiempo establecido
        if (Time.time - time > total)
        {
            // Elige una roca al azar
            int chosenRockIndex = Random.Range(0, rocks.Length);

            // Le indica a CombatJudge que se moverá a esa roca
            Combatjudge.combatjudge.MoveToRock(rocks[chosenRockIndex]);
        }
    }
}

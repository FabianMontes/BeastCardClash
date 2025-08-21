using System;
using UnityEngine;

// TODO: Corregir los nombres de método "rol" y "unrol", aquí y en el archivo dice.cs

[DefaultExecutionOrder(1)]
public class BotPlayer : MonoBehaviour
{
    // Variables de instancia
    Figther figther;
    Transform hand;

    void Start()
    {
        // Inicializa las variables
        figther = GetComponent<Figther>();
        hand = transform.GetChild(0).GetChild(1);
        picking = false;
    }

    // Tiempo, indicador si se está seleccionando y total
    float time;
    bool picking; // picking es una bandera que evita que se repita el código en cada frame
    float total;

    void Update()
    {
        // Si no hay jugador, no hace nada
        if (figther == null) return;

        // Determina el tiempo de pensar antes de elegir la carta
        // Verifica si el bot está en batalla (sin haber perdido, por ejemplo), en momento para seleccionar y no haya seleccionado carta en ese turno
        // Si se cumple, establece el tiempo para "pensar" del bot
        if (figther.IsFigthing() && Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickCard && figther.getPicked() == null && !picking)
        {
            // Almacena el momento en el que se acciona este bloque
            time = Time.time;

            // Establece un tiempo aleatorio entre 1 y 3 segundos (para "pensar")
            total = (float)Random.Range(100, 300) / 100.0f;

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
                int a = Random.Range(0, 6); // Índice de la carta a elegir
                int b = 0; // b controla la búsqueda de cartas
                HandCard card = hand.transform.GetChild(a).GetComponent<HandCard>(); // Carta elegida

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
            total = (float)Random.Range(50, 300) / 100.0f;

            // Lanza el dado
            FindFirstObjectByType<dice>().rol(); // ! ¿rol? ¿no será "roll"?
        }
        // Lanza el dado cuando ya se acabó el tiempo de lanzar
        // Verifica si el tiempo de espera ya acabó y es nuestro turno
        // Si se cumple, lanza el dado
        else if (Time.time - time > total && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            // Termina de lanzar el dado
            FindFirstObjectByType<dice>().unrol(); // ! ¿unrol? ¿no será "unroll"?
        }
        // Establece el tiempo antes de elegir el tipo de ataque
        // Verifica si el jugador está en momento de elegir ataque y es su turno
        // Si se cumple, establece el tiempo para "pensar"
        else if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.SelecCombat && !picking && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            // Almacena el momento en el que se acciona este bloque
            time = Time.time;

            // Establece un tiempo aleatorio entre 0.5 y 2 segundos (para "pensar")
            total = (float)Random.Range(50, 200) / 100.0f;

            // Activa la bandera
            picking = true;
        }
        // TODO: Hacer esto --------------------------------------
        // Elige el elemento de ataque cuando ya se acabó el tiempo de pensar
        // Verifica si el tiempo de espera ya acabó y es nuestro turno
        // Si se cumple, elige el elemento
        else if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.SelecCombat && picking && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            // Verifica si ya transcurrió el tiempo establecido
            if (Time.time - time > total)
            {
                // Reestablece la bandera
                picking = false;

                // Inicializa el arreglo. Cada elemento almacenará la cantidad de cartas disponibles de cada elemento
                int[] elem = new int[4];

                // Rellena el arreglo con los valores correspondientes
                for (int i = 0; i < 6; i++)
                {
                    // Extrae la carta actual
                    HandCard card = transform.GetChild(i).GetComponent<HandCard>();

                    // Pone en el arreglo 
                    elem[(int)card.GetCard().GetElement()] = elem[(int)card.GetCard().GetElement()] + 1;
                }

                int big = 0; // Índice de la carta de mayor valor (actual)
                             // int next = 1; // Siguiente carta

                // // Itera sobre los elementos para hallar el de mayor valor
                // while (next < 4)
                // {
                //     // Si hay más cartas en un elemento que en el actual, reasigna
                //     if (big < next) big = next;
                //     next++;
                // }

                // Itera sobre los elementos para hallar el de mayor valor (corregido)
                // ! Vas aquí

                // Elige el elemento con más cartas
                GetComponentInChildren<SelectType>().PickElement(big);
            }
        }
        // TODO: --------------------------------------
    }

    RockBehavior[] rocks;
    public void pickRock(RockBehavior[] rocks)
    {
        picking = false;
        time = Time.time;
        total = (float)Random.Range(100, 200) / 100.0f;
        this.rocks = rocks;
        return;
    }

    public void thinkingRocks()
    {
        if (Time.time - time > total)
        {
            int a = Random.Range(0, rocks.Length);
            Combatjudge.combatjudge.MoveToRock(rocks[a]);
        }
    }
}

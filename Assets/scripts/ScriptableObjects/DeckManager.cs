using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// TODO: Vas acá

/// <summary>Gestiona la baraja de cartas y las manos del jugador y los enemigos</summary>
public class DeckManager : MonoBehaviour
{
    // Getters e instancias
    public static DeckManager Instance { get; private set; } // Instancia estática para el patrón Singleton
    public static List<DeckCardsList> PlayerHand { get; private set; } // Getter público para la baraja obtenida para el jugador

    [Header("Deck variables")]
    [SerializeField] int points; // Puntos del jugador
    [SerializeField] int handSize = 6; // Tamaño de la mano por defecto
    [SerializeField] int absOffset = 2; // Offset absoluto de puntos
    [SerializeField] float relOffset = 0.1f; // Offset absoluto de puntos
    public List<DeckCardsList> playerHand; // Baraja del jugador
    List<DeckCardsList> _cardList; // Conversión del enum DeckCards a una lista

    void Awake()
    {
        // Lógica del Singleton
        if (Instance != null && Instance != this)
        {
            // Si ya existe una instancia, destruye este objeto para asegurar que solo haya uno
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Convierte el enum de cartas en una lista
        _cardList = Enum.GetValues(typeof(DeckCardsList)).Cast<DeckCardsList>().ToList();
    }

    /// <summary>Genera la mano y la devuelve. Se usa para el jugador y los enemigos</summary>
    /// <param name="isEnemy">Si es verdadero, empieza sacando el puntaje con el offset, si es falso, pasa directo a crear la mano</param>
    /// <param name="useAbsOffset">Si es true, quiere decir que usaremos la variación absoluta. Si no, usamos la relativa</param>
    /// <returns>La lista de cartas para su uso</returns>
    public List<DeckCardsList> GenerateHand(bool isEnemy = true, bool useAbsOffset = false)
    {
        // Si el jugador no es humano, se establece el puntaje como un valor aleatorio en rango para elegir el puntaje a usar
        if (isEnemy)
        {
            // Crea el offset como el valor absoluto o en su defecto un cast del offset relativo
            int offset = useAbsOffset ? absOffset : (int)(points * relOffset);
            int minPoints = points - offset;
            int maxPoints = points + offset;

            // Sobreescribe los puntos con un valor aleatorio entre minPoints y maxPoints
            points = Random.Range(minPoints, maxPoints);

            // Vuelve a llamarse a sí mismo, pero ahora para crear la baraja
            return GenerateHand(false, useAbsOffset);
        }

        // Filtra las cartas elegibles para esta mano específica basada en los puntos.
        int maxCardValue = SetMaxCardValue();
        List<DeckCardsList> eligibleCards = _cardList.Where(card => (int)card <= maxCardValue).ToList();

        // Invoca a la mano de cartas a rellenar y a los parámetros para ello
        List<DeckCardsList> hand = new List<DeckCardsList>();

        // Rellena la mano de cartas, sin repetir y hasta obtener el tamaño
        int cardsToChoose = Mathf.Min(handSize, eligibleCards.Count);

        while (hand.Count < cardsToChoose)
        {
            // Selecciona una carta al azar de la lista de cartas elegibles
            DeckCardsList chosenCard = eligibleCards[Random.Range(0, eligibleCards.Count)];
            if (!hand.Contains(chosenCard)) hand.Add(chosenCard);
        }

        return hand;
    }

    /// <summary>Establece los puntos al terminar una partida desde afuera. Suma dos puntos al ganar, resta uno al perder</summary>
    /// <param name="result">Resultado de batalla - true: ganar, false: perder</param>
    public void SetPoints(bool result)
    {
        points = result ? points + 2 : points - 1;
    }

    /// <summary>Establece el valor máximo que puede tener una carta disponible para el jugador</summary>
    /// <returns>Aumento de los puntos</returns>
    int SetMaxCardValue()
    {
        return handSize + points / 5;
    }
}

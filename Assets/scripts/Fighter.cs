using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Acetiles, Ingeniosos, Adn, Zootecnicos, RcpTeam, PlumaDorada, RealPincel, PhotoAgros, VaGames
}

public enum Specie
{
    Bear, Frog, Chameleon, Condor
}

// TODO: Ponerle los nombres de los métodos en PascalCase

[DefaultExecutionOrder(0)]
public class Fighter : MonoBehaviour
{
    // Getters e instancias
    public int Skin { get; private set; } // Getter público para la skin del jugador
    public bool NoHurt { get; private set; } // Getter. Indica si el último cambio de vida fue positivo (curación) o nulo

    // Variables del jugador
    [Header("Fighter Variables")]
    [SerializeField] private bool isEnemy = true; // Indica si el jugador es enemigo (útil para DeckManager)
    [SerializeField] int fighterLive; // Vida
    [SerializeField] public string fighterName; // Nombre
    [SerializeField] Team team; // Equipo
    [SerializeField] Specie specie; // Especie
    [SerializeField] int deckSize; // Tamaño de la baraja
    [SerializeField] int handSize = 6; // Tamaño de la mano
    public int availableCard; // Cartas disponibles

    // Variables del juego y demás
    [Header("Extra Data")]
    [SerializeField] GameObject tokenPrefab; // Prefab de la ficha
    [SerializeField] GameObject cardPrefab; // Prefab de la carta
    [SerializeField] public PlayerToken playerToken; // Token del jugador
    [SerializeField] public int visualFighter; // Índice del visual (prefab o modelo) del luchador que se debe activar
    [SerializeField] public int indexFighter = -1; // Identificador de jugador (en el arreglo)
    [SerializeField] public RockBehavior initialStone; // Roca inicial
    int _lastVisualPlayer; // Almacena el índice del último visual activado

    void Start()
    {
        // Desactiva todos los visuales excepto el visualFighter
        _lastVisualPlayer = visualFighter;
        transform.GetChild(4).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(visualFighter).gameObject.SetActive(true);

        // Crea el token del jugador si no existe ya
        if (playerToken == null) playerToken = Instantiate(tokenPrefab, initialStone.transform.position + Vector3.up * 1, Quaternion.identity).transform.GetComponent<PlayerToken>();

        // Asigna la clase misma al token y le da su roca actual
        playerToken.player = this;
        playerToken.rocky = initialStone;

        // TODO: Vas acá

        // // Crea un mazo provisional (depreciado)
        // for (int i = 0; i < deckSize; i++)
        // {
        //     Card card = Instantiate(cardPrefab, transform.GetChild(0).GetChild(0)).GetComponent<Card>();
        //     card.indexer = i;
        // }

        // Crea un mazo real (mejorado)
        List<DeckCardsList> fighterDeck = DeckManager.Instance.GenerateHand(isEnemy, true);

        // Obtenemos el objeto que contiene las cartas del mazo
        Transform deckHolder = transform.GetChild(0).GetChild(0);

        // Creamos los objetos dentro de deckHolder, carta por carta
        foreach (DeckCardsList card in fighterDeck)
        {
            Card newCard = Instantiate(cardPrefab, deckHolder).GetComponent<Card>();
            newCard.Initialize(card);
        }
    }

    void Update()
    {
        if (_lastVisualPlayer != visualFighter)
        {
            transform.GetChild(_lastVisualPlayer).gameObject.SetActive(false);
            transform.GetChild(visualFighter).gameObject.SetActive(true);
            _lastVisualPlayer = visualFighter;
        }

        // Si no estamos eligiendo carta o no estamos peleando, no hacemos nada
        if (CombatJudge.Instance.GetSetMoments() != SetMoments.PickCard || !IsFigthing()) return;

        // Si hay cartas disponibles, no hacemos nada
        if (availableCard != 0) return;

        Transform hand = transform.GetChild(0).GetChild(1);
        for (int i = 0; i < handSize; i++)
        {
            hand.GetChild(i).GetComponent<HandCard>().ForceReveal();
        }
    }

    /// <summary>Obtiene la especie del jugador</summary>
    public Specie GetSpecie() => specie;

    /// <summary>Establece el equipo del jugador</summary>
    public void SetTeam(Team newTeam) => team = newTeam;

    /// <summary>Establece la skin del jugador</summary>
    public void SetSkin(int newSkin) => Skin = newSkin;

    /// <summary>Establece una skin que no coincida con una en particular</summary>
    /// <param name="skin">Skin que no debe repetirse</param>
    public void SetNoSkin(int skin)
    {
        int a = Random.Range(0, 6);

        // Si la skin coincide, selecciona otra al azar de nuevo
        while (a == skin)
        {
            a = Random.Range(0, 6);
        }

        Skin = a;
    }

    /// <summary>Establece una skin al azar</summary>
    public void SetRSkin() => Skin = Random.Range(0, 6);

    /// <summary>Establece un equipo que no coincida con una en particular</summary>
    /// <param name="noTeam">Equipo que no debe repetirse</param>
    public void SetNoTeam(Team noTeam)
    {
        team = (Team)Random.Range(0, 8);

        // Si el equipo coincide, selecciona otro al azar de nuevo
        while (team == noTeam)
        {
            team = (Team)Random.Range(0, 8);
        }
    }

    /// <summary>Establece un equipo al azar</summary>
    public void FreeTeam() => team = (Team)Random.Range(0, 8);

    /// <summary>Obtiene el equipo del jugador</summary>
    public Team GetTeam() => team;

    /// <summary>Obtiene la vida actual del jugador</summary>
    public int GetPlayerLive() => fighterLive;

    /// <summary>Establece la vida del jugador</summary>
    public void SetPlayerLive(int playerLive) => fighterLive = playerLive;

    /// <summary>Le suma vida al jugador</summary>
    /// <param name="playerLife">Valor a aumentar</param>
    public void AddPlayerLive(int playerLife)
    {
        // NoHurt determina si el jugador sufrió daño
        NoHurt = playerLife >= 0;

        fighterLive += playerLife;

        // Si la vida supera el máximo, lo deja en ese máximo. Si baja de 0, lo deja en 0
        if (fighterLive > CombatJudge.Instance.initialLives) fighterLive = CombatJudge.Instance.initialLives;
        if (fighterLive < 0) fighterLive = 0;
    }

    /// <summary>Establece la especie a Oso y una skin de oso al azar</summary>
    public void RandomSpecie()
    {
        specie = Specie.Bear;
        Skin = Random.Range(0, 2);
    }

    /// <summary>Mueve al jugador a la roca elegida</summary>
    public void MovePlayer(RockBehavior rocker) => playerToken.rocky = rocker;

    /// <summary>Obtiene la carta elegida</summary>
    public Card GetPicked() => GetComponentInChildren<HolderPlay>().GetPicked();

    /// <summary>Lanza la carta elegida</summary>
    public void PlayCard(Card card) => GetComponentInChildren<HolderPlay>().PlayCard(card);


    /// <summary>TODO: Muestra la carta elegida por índice</summary>
    private void DrawCard(int index)
    {
        int drawnCardIndex = Random.Range(0, deckSize);
        Transform deck = transform.GetChild(0).GetChild(0);
        Transform hand = transform.GetChild(0).GetChild(1);
        while (!deck.GetChild(drawnCardIndex).gameObject.activeSelf)
        {
            drawnCardIndex = (drawnCardIndex + 1) % deckSize;
        }

        hand.GetChild(index).GetComponent<HandCard>().SetCard(deck.GetChild(drawnCardIndex).GetComponent<Card>());
        deck.GetChild(drawnCardIndex).gameObject.SetActive(false);
    }

    /// <summary>Rellena la mano</summary>
    public void RefillHand()
    {
        Transform hand = transform.GetChild(0).GetChild(1);
        for (int i = 0; i < handSize; i++)
        {
            if (hand.GetChild(i).GetComponent<HandCard>().GetCard() == null) DrawCard(i);
        }

        availableCard = 0;
    }

    /// <summary>Indica si el jugador está participando en el combate actual</summary>
    /// <returns></returns>
    public bool IsFigthing()
    {
        int figthers = CombatJudge.Instance.GetPlayersFighting();
        int a = 0;

        while (figthers > 0)
        {
            int red = figthers % 2;
            figthers = (int)Mathf.Floor(figthers / 2);

            if (a == indexFighter) return red != 0;
            a++;
        }

        return false;
    }

    /// <summary>Lanza una carta</summary>
    public void ThrowCard() => GetComponentInChildren<HolderPlay>().PlayCard(null);
}

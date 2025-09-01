using System;
using UnityEngine;

[DefaultExecutionOrder(-5)]
public class Card : MonoBehaviour
{
    [Header("Valores de la carta")]
    [SerializeField] private Element element; // Elemento de la carta
    [SerializeField] private int value; // Valor de la carta
    [SerializeField] public int indexer = 0; // Indice de la carta
    public string identifier; // Identificador de la carta (string)

    // private void Start()
    // {
    //     // Obtiene los valores de la carta de forma aleatoria
    //     value = UnityEngine.Random.Range(1, 11);
    //     element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length);

    //     // El identificador es la concatenación del valor y del elemento, en un string
    //     identifier = value.ToString() + element.ToString();
    // }

    // TODO: Vas acá
    public void Initialize(DeckCardsList card)
    {
        // Asigna el valor numérico directo de la carta
        value = (int)card;

        // Extrae y asigna el elemento en función de como inicia su nombre
        string cardName = card.ToString();

        if (cardName.StartsWith("Fire")) { element = Element.Fire; }
        else if (cardName.StartsWith("Earth")) { element = Element.Earth; }
        else if (cardName.StartsWith("Water")) { element = Element.Water; }
        else if (cardName.StartsWith("Air")) { element = Element.Air; }

        // El identificador es la concatenación del valor y del elemento, en un string
        identifier = value.ToString() + element.ToString();
    }

    /// <summary>Obtiene el valor de la carta</summary>
    public int GetValue() => value;

    /// <summary>Obtiene el elemento de la carta</summary>
    public Element GetElement() => element;

    /// <summary>Obtiene el identificador de la carta</summary>
    public string GetID() => identifier;
}

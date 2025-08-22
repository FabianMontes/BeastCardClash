using System;
using UnityEngine;

[DefaultExecutionOrder(-5)]
public class Card : MonoBehaviour
{
    // Variables
    [Header("Valores de la carta")]
    [SerializeField] private Element element; // Elemento de la carta
    [SerializeField] private int value; // Valor de la carta
    [SerializeField] public int indexer = 0; // Indice de la carta
    public string identifier; // Identificador de la carta (string)

    private void Start()
    {
        // Obtiene los valores de la carta de forma aleatoria
        value = UnityEngine.Random.Range(1, 11);
        element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length);

        // El identificador es la concatenación del valor y del elemento, en un string
        identifier = value.ToString() + element.ToString();
    }

    // Obtiene el valor de la carta
    public int GetValue()
    {
        return value;
    }

    // Obtiene el elemento de la carta
    public Element GetElement()
    {
        return element;
    }

    // Obtiene el identificador de la carta
    public string GetID()
    {
        return identifier;
    }
}

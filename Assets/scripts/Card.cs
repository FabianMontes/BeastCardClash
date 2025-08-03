using System;
using UnityEngine;

[DefaultExecutionOrder(-5)]
public class Card : MonoBehaviour
{
    [SerializeField] private Element element;
    [SerializeField] private int value;
    [SerializeField] public int indexer = 0;
    public string identifier;

    private void Start()
    {
        value = UnityEngine.Random.Range(1, 11);
        element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length);
        identifier = value.ToString() + element.ToString();
    }

    public int GetValue()
    {
        return value;
    }

    public Element GetElement()
    {
        return element;
    }

    public string GetID()
    {
        return identifier;
    }

    void Update()
    {

    }
}

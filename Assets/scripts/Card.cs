using System;
using UnityEngine;
[DefaultExecutionOrder(-5)]
public class Card: MonoBehaviour
{
    [SerializeField] private int value;
    [SerializeField] private Element element;
    private string identifie;
    [SerializeField] public int indexer = 0;

    private void Start()
    {
        value = UnityEngine.Random.Range(0, 11);
        element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length);
        identifie = value.ToString() + element.ToString();
    }

    public int GetValue()
    {
        return value;
    }

    public Element GetElement()
    {
        return element;
    }
    public String GetID()
    {
        return identifie;
    }

    void Update()
    {
        
        
    }



}

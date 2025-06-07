using System;
using UnityEngine;
public class Card: MonoBehaviour
{
    private int value;
    private Element element;
    private string Id;

    public Card(string Id)
    {
        element = (Element)UnityEngine.Random.Range(0,Enum.GetNames(typeof(Element)).Length);
        value = UnityEngine.Random.Range(0, 11);
        this.Id = Id;
    }

    public Card(int value, string Id)
    {
        this.value = value;
        element = (Element)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Element)).Length);
        this.Id = Id;
    }

    public Card(Element element, string Id)
    {
        this.element = element;
        value = UnityEngine.Random.Range(0, 11);
        this.Id = Id;
    }

    public Card(int value, Element elements)
    {
        this.value = value;
        this.element = elements;
        this.Id = value.ToString()+""+element.ToString();
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
        return Id;
    }

}

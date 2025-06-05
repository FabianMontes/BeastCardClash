using TMPro;
using UnityEngine;

public class dice : MonoBehaviour
{

    int value = 0;
    int maxValue;
    TextMeshPro texter;
    bool roling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxValue = FindFirstObjectByType<Combatjudge>().maxDice;
        roling = false;
        texter = GetComponentInChildren<TextMeshPro>();

    }

    // Update is called once per frame
    void Update()
    {
        if (roling)
        {
            value = Random.Range(1, maxValue + 1);
        }

        texter.text = value.ToString();
    }

    private void OnMouseDown()
    {
        if (FindFirstObjectByType<Combatjudge>().GetSetMoments() == SetMoments.PickDice)
        {
            roling = true;
        }


    }

    private void OnMouseExit()
    { 
        if (roling)
        {
            roling = false;
            FindFirstObjectByType<Combatjudge>().roled(value); 
        }

    }

    private void OnMouseUp()
    {
        if (roling)
        {
            roling = false;
            FindFirstObjectByType<Combatjudge>().roled(value);
        }
        
    }


}

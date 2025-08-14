using TMPro;
using UnityEngine;

public class dice : MonoBehaviour
{
    public int value { get; private set; } = 0;
    int maxValue;
    TextMeshPro texter;
    bool roling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxValue = Combatjudge.combatjudge.maxDice;
        roling = false;
        texter = GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        if (roling)
        {
            Combatjudge.combatjudge.StartRoling();
            value = Random.Range(1, maxValue + 1);
        }

        texter.text = value.ToString();
    }

    private void OnMouseDown()
    {
        if (Combatjudge.combatjudge.FocusONTurn())
        {
            rol();
        }
    }

    public void rol()
    {
        if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickDice)
        {
            roling = true;
        }
    }

    private void OnMouseExit()  
    {
        if (Combatjudge.combatjudge.FocusONTurn())
        {
            unrol();
            
        }
    }
    public void unrol()
    {
        if (roling)
        {
            roling = false;
            Combatjudge.combatjudge.Roled();
        }
    }

    private void OnMouseUp()
    {
        if (Combatjudge.combatjudge.FocusONTurn())
        {
            unrol();

        }
    }
}

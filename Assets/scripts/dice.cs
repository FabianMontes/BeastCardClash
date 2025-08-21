using TMPro;
using UnityEngine;

public class dice : MonoBehaviour
{
    public int value { get; private set; } = 0;
    int maxValue;

    bool roling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxValue = Combatjudge.combatjudge.maxDice;
        roling = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (roling)
        {
            Combatjudge.combatjudge.StartRoling();
            value = Random.Range(1, maxValue + 1);
            Vector3 vector3 = new Vector3(0, 45, 0);
            switch (value)
            {
                case 1:
                    vector3.z = 90;
                    break;
                case 2:
                    vector3.x = -90;
                    break;
                case 3:

                    break;
                case 4:
                    vector3.x = 180;
                    break;
                case 5:
                    vector3.x = 90;
                    break;
                case 6:
                    vector3.z = -90;
                    break;
            }
            transform.rotation = Quaternion.Euler(vector3);
        }


    }

    private void OnMouseDown()
    {
        if (Combatjudge.combatjudge.FocusONTurn())
        {
            Roll();
        }
    }

    public void Roll()
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
            Unroll();

        }
    }
    public void Unroll()
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
            Unroll();

        }
    }
}

using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class BotPlayer : MonoBehaviour
{
    Figther figther;
    Transform hand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        figther = GetComponent<Figther>();
        hand = transform.GetChild(0).GetChild(1);
        picking = false;
    }

    float time;
    bool picking;
    float total;
    // Update is called once per frame
    void Update()
    {
        if (figther == null)
        {
            return;
        }
        if (figther.IsFigthing() && Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickCard && figther.getPicked() == null && !picking)
        {
            time = Time.time;
            total = (float)Random.Range(100, 300) / 100.0f;
            picking = true;


        }
        else if (picking && Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickCard)
        {
            if (Time.time - time > total)
            {
                picking = false;
                int a = Random.Range(0, 6);
                int b = 0;
                HandCard card = hand.transform.GetChild(a).GetComponent<HandCard>();
                while ((card == null || !card.isClickable()) && b < 100)
                {
                    a = (a + 1) % 6;
                    card = hand.transform.GetChild(a).GetComponent<HandCard>();
                    b++;

                }
                card.SelectedCard();
            }

        }
        else if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickDice && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            time = Time.time;
            total = (float)Random.Range(50, 300) / 100.0f;
            FindFirstObjectByType<dice>().rol();
        }
        else if (Time.time - time > total && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            FindFirstObjectByType<dice>().unrol();
        }else if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.SelecCombat && !picking && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            time = Time.time;
            total = (float)Random.Range(50, 200) / 100.0f;
            picking = true;
        }else if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.SelecCombat && picking && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            if(Time.time -time > total)
            {
                picking = false;

                int[] elem = new int[4];
                

                for(int i = 0; i < 6; i++)
                {
                    HandCard card = transform.GetChild(i).GetComponent<HandCard>();
                    elem[(int)card.GetCard().GetElement()] = elem[(int)card.GetCard().GetElement()]+1;
                }
                int big = 0;
                int next = 1;
                while (next < 4)
                {
                    if (big < next)
                    {
                        big = next;
                    }
                    next++;
                }

                GetComponentInChildren<SelectType>().PickElement(next);
            }
        }

    }

    RockBehavior[] rocks;
    public void pickRock(RockBehavior[] rocks)
    {
        picking = false;
        time = Time.time;
        total = (float)Random.Range(100, 200) / 100.0f;
        this.rocks = rocks;
        return;


    }

    public void thinkingRocks()
    {
        if (Time.time - time > total)
        {
            int a = Random.Range(0, rocks.Length);
            Combatjudge.combatjudge.MoveToRock(rocks[a]);
        }
    }
}

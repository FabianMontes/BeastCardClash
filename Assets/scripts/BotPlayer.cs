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

    }

    float time;

    // Update is called once per frame
    void Update()
    {
        if (figther == null)
        {
            return;
        }
        if (figther.IsFigthing() && Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickCard && figther.getPicked() == null)
        {
            int a = Random.Range(0, 6);
            int b = 0;
            HandCard card = hand.transform.GetChild(a).GetComponent<HandCard>();
            while ((card == null || !card.isClickable())&& b<100)
            {
                a = (a+1)%6;
                card = hand.transform.GetChild(a).GetComponent<HandCard>();
                b++;
                
            }
            card.SelectedCard();

        }else if(Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickDice && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            time = Time.time;
            FindFirstObjectByType<dice>().rol();
        }else if (Time.time-time > 2 && Combatjudge.combatjudge.turn() == figther.indexFigther)
        {
            FindFirstObjectByType<dice>().unrol();
        }

    }

    public void pickRock(RockBehavior[] rocks)
    {
        int a = Random.Range(0, rocks.Length);
        Combatjudge.combatjudge.MoveToRock(rocks[a]);
    }
}

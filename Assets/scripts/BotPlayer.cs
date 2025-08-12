using Unity.VisualScripting;
using UnityEngine;

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

    // Update is called once per frame
    void Update()
    {
        if (figther == null)
        {
            return;
        }
        if (figther.IsFigthing() && Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickCard)
        {
            int a = Random.Range(0, 6);
            HandCard card = hand.transform.GetChild(a).GetComponent<HandCard>();
            while (card == null || !card.Selectable())
            {
                a = Random.Range(0, 6);
                card = hand.transform.GetChild(a).GetComponent<HandCard>();
                Debug.Break();

            }

            card.SelectedCard();

        }

    }

    public void pickRock(RockBehavior[] rocks)
    {
        int a = Random.Range(0, rocks.Length);
        Combatjudge.combatjudge.MoveToRock(rocks[a]);
    }
}

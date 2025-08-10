using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using UnityEngine.Playables;

[DefaultExecutionOrder(-4)]

public class HandCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] int handPos;
    [SerializeField] Card card;
    [SerializeField] bool playable = true; 
    Figther player;
    Button button;

    SetMoments prevSetMoment;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponentInParent<Figther>();
        prevSetMoment = SetMoments.PickDice;
        button = transform.GetComponent<Button>();
        Visib(false);
        SetCard(card);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playable)
        {
            return;
        }

        SetMoments momo = Combatjudge.combatjudge.GetSetMoments();
        if (momo != prevSetMoment)
        {
            if (momo == SetMoments.PickCard && player.IsFigthing())
            {
                if (Combatjudge.combatjudge.combatType == CombatType.full || (int)Combatjudge.combatjudge.combatType == (int)card.GetElement())
                {
                    Visib(true);
                    player.avalaibleCard++;
                }
                else
                {

                }
            }
            if (momo != SetMoments.PickCard)
            {
                Visib(false);
            }

            prevSetMoment = momo;
        }

        if (player.getPicked() != null) Visib(false);
    }

    private void Visib(bool isVisible)
    {
        //transform.GetChild(0).gameObject.SetActive(isVisible);
        button.interactable = isVisible;
    }

    public void ForceReveal()
    {
        Visib(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Vec transform.position
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void SetCard(Card card)
    {
        this.card = card;
        if (card == null)
        {
            transform.GetComponent<Image>().enabled = false;
        }
        else
        {

            Image image = transform.GetComponent<Image>();

        }
    }

    public void SelectedCard()
    {
        player.PlayCard(card);
        SetCard(null);
    }

    public Card GetCard()
    {
        return card;
    }

    public bool Selectable()
    {
        return button.interactable;
    }
}

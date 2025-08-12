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

        if (playable)
        {
            clickable(false);
        }
        else
        {
            Visib(false);
            button.interactable = false;
        }

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
                    clickable(true);
                    player.avalaibleCard++;
                }
                else
                {

                }
            }
            if (momo != SetMoments.PickCard)
            {
                clickable(false);
            }

            prevSetMoment = momo;
        }

        if (player.getPicked() != null) clickable(false);
    }

    private void Visib(bool isVisible)
    {
        transform.GetChild(0).gameObject.SetActive(isVisible);
        transform.GetChild(1).gameObject.SetActive(isVisible);
        transform.GetChild(2).gameObject.SetActive(isVisible);
        transform.GetChild(3).gameObject.SetActive(isVisible);
        
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
            Visib(false);
        }
        else
        {
            Visib(true);
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

    public void clickable(bool isClick)
    {
        button.interactable = isClick;
        if (isClick)
        {
            transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.white;
        }else {
            transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.gray;
        }
    }

    public bool isClickable()
    {
        return button.interactable == playable;
    }
}

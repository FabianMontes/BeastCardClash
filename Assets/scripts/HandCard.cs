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
    [SerializeField] public bool picker = false;
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

        transform.GetChild(1).gameObject.SetActive(picker);

    }

    // Update is called once per frame
    void Update()
    {
        SetMoments momo = Combatjudge.combatjudge.GetSetMoments();
        if (!picker)
        {
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
            return;
        }


        if (picker && player.IsFigthing() && momo != SetMoments.SelecCombat)
        {
            if (momo == SetMoments.PickCard)
            {
                halfVisible(true);
                Image chil = transform.GetChild(0).GetChild(0).GetComponent<Image>();
                Color color = chil.color;
                color.a = card == null ? 0.5f : 1f;
                chil.color = color;
            }
            else if (momo == SetMoments.Reveal)
            {
                halfVisible(false);
                Visib(true);
            }


        }


    }

    private void Visib(bool isVisible)
    {
        //transform.GetChild(0).gameObject.SetActive(isVisible);
        transform.GetChild(1).gameObject.SetActive(isVisible);

    }

    public void ForceReveal()
    {
        clickable(true);
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

        if (card == null || (!playable && !picker))
        {
            Visib(false);
        }
        else
        {
            if (picker)
            {
                halfVisible(true);
            }
            else
            {
                Visib(true);
            }


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

    public void clickable(bool isClick)
    {
        button.interactable = isClick;
        if (isClick)
        {
            transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white;
            transform.GetChild(1).GetChild(1).GetComponent<Image>().color = Color.white;
            transform.GetChild(1).GetChild(2).GetComponent<Image>().color = Color.white;
        }
        else
        {
            transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.gray;
            transform.GetChild(1).GetChild(1).GetComponent<Image>().color = Color.gray;
            transform.GetChild(1).GetChild(2).GetComponent<Image>().color = Color.gray;

        }
    }

    public bool isClickable()
    {
        return button.interactable;
    }

    private void halfVisible(bool visible)
    {
        transform.GetChild(0).gameObject.SetActive(visible);
        //transform.GetChild(2).gameObject.SetActive(front);
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;


public class HandCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] int handPos;
    [SerializeField] Card card;
    TextMeshProUGUI textMeshPro;
    Player player;
    Button button;




    SetMoments prevSetMoment;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        player = GetComponentInParent<Player>();
        prevSetMoment = SetMoments.PickDice;
        Visib(false);
    }

    // Update is called once per frame
    void Update()
    {
        SetMoments momo = Combatjudge.combatjudge.GetSetMoments();
        if (momo != prevSetMoment)
        {
            if (momo == SetMoments.PickCard)
            {
                if (Combatjudge.combatjudge.combatType == CombatType.full || (int)Combatjudge.combatjudge.combatType == (int)card.GetElement())
                    Visib(true);

            }
            if (momo == SetMoments.Loop || momo == SetMoments.End)
            {
                Visib(false);
            }


            prevSetMoment = momo;
        }

        if (prevSetMoment == SetMoments.PickCard)
        {
         
        }



    }

    private void Visib(bool isVisible)
    {
        //transform.GetChild(0).gameObject.SetActive(isVisible);
        button.interactable = isVisible;
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
            textMeshPro.text = "";
        }
        else
        {
            textMeshPro.text = card.GetID();
        }
    }

    public void SelectedCard()
    {

    }
}

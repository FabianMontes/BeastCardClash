using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-4)]

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
        button = transform.GetComponent<Button>();
        Visib(false);
        SetCard(card);
    }

    // Update is called once per frame
    void Update()
    {
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
            textMeshPro.text = "";
            transform.GetComponent<Image>().enabled = false;
        }
        else
        {
            textMeshPro.text = card.GetID();

            Image image = transform.GetComponent<Image>();

            image.enabled = true;
            switch (card.GetElement())
            {
                case Element.fire:
                    image.color = Color.red;
                    break;
                case Element.earth:
                    image.color = Color.green;
                    break;
                case Element.water:
                    image.color = Color.blue;
                    break;
                case Element.air:
                    image.color = Color.white;
                    break;
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
}

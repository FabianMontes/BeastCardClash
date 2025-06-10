
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickerCard : MonoBehaviour
{
    TextMeshProUGUI textMeshPro;
    Player player;
    


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
            if(momo == SetMoments.PickCard)
            {
                Visib(true);
                
            }
            if(momo == SetMoments.Loop || momo == SetMoments.End)
            {
                Visib(false);
            }


                prevSetMoment = momo;
        }

        if(prevSetMoment == SetMoments.PickCard)
        {
            Card card = player.getPicked();
            if (card == null)
            {
                textMeshPro.text = "";
            }
            else
            {
                textMeshPro.text = card.GetID();
            }
        }

        
    }

    private void Visib(bool isVisible)
    {
        transform.GetChild(0).gameObject.SetActive(isVisible);
        transform.GetComponent<Image>().enabled = isVisible;
    }
}

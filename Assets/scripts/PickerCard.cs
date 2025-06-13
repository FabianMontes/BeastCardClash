
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickerCard : MonoBehaviour
{
    TextMeshProUGUI textMeshPro;
    Player player;
    [SerializeField] HolderPlay holderPlay;


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
        //if (momo != prevSetMoment)
        //{
            if(momo == SetMoments.PickCard && player.IsFigthing())
            {
                Visib(true);
                
            }
            if(momo == SetMoments.Loop || momo == SetMoments.End)
            {
                Visib(false);
            }


          //      prevSetMoment = momo;
        //}
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

    private void Visib(bool isVisible)
    {
        transform.GetChild(0).gameObject.SetActive(isVisible);
        transform.GetComponent<Image>().enabled = isVisible;
    }
}

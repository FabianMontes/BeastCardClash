
using TMPro;
using UnityEngine;

public class FolowerCard : MonoBehaviour
{
    TextMeshProUGUI textMeshPro;
    
    [SerializeField] private Card card;
    string text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()  
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        
        if(card != null)
        {

            textMeshPro.text = card.GetID();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeCard(Card card)
    {
        this.card  = card;
        textMeshPro.text = card.GetID();
    }


}

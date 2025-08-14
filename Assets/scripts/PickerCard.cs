
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickerCard : MonoBehaviour
{
    HandCard card;
    Figther player;
    bool isPlaying = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        card = GetComponentInChildren<HandCard>();
        player = GetComponentInParent<Figther>();
        card.SetCard(null);
    }

    // Update is called once per frame
    void Update()
    {
        
        //card.gameObject.SetActive(player.getPicked() != null);
        if(isPlaying != (player.getPicked() != null))
        {
            isPlaying = player.getPicked() != null;
            card.SetCard(player.getPicked());
        }
        
    }
}

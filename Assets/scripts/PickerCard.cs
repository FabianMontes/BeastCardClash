
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickerCard : MonoBehaviour
{
    HandCard card;
    Fighter player;
    bool isPlaying = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        card = GetComponentInChildren<HandCard>();
        player = GetComponentInParent<Fighter>();
        card.SetCard(null);
    }

    // Update is called once per frame
    void Update()
    {

        //card.gameObject.SetActive(player.getPicked() != null);
        if (isPlaying != (player.GetPicked() != null))
        {
            isPlaying = player.GetPicked() != null;
            card.SetCard(player.GetPicked());
        }

    }
}

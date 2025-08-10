
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickerCard : MonoBehaviour
{
    HandCard card;
    Figther player;

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
        card.SetCard(player.getPicked());
        card.gameObject.SetActive(player.getPicked() != null);
    }
}


using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardValue : MonoBehaviour
{
    HandCard card;
    TextMeshProUGUI textMeshPro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        card = GetComponentInParent<HandCard>();
    }

    // Update is called once per frame
    void Update()
    {
        if (card.GetCard() == null)
        {
            textMeshPro.text = "";
            return;
        }
        textMeshPro.text = card.GetCard().GetValue().ToString();

    }
}

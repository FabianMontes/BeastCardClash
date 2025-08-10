
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardborder : MonoBehaviour
{
    [SerializeField] private Color[] colors;
    HandCard card;
    Image image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        card = GetComponentInParent<HandCard>();
        image = transform.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (card.GetCard() == null)
        {
            image.enabled = false;
            return;
        }
        image.enabled = true;
        image.color = colors[(int)card.GetCard().GetElement()];

    }
}

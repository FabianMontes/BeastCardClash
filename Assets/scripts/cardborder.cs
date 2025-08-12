
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardborder : MonoBehaviour
{
    [SerializeField] private Color[] colors;
    HandCard card;
    Image image;
    [SerializeField] float darkValue = 0.3f;
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
        Color color = colors[(int)card.GetCard().GetElement()];
        if (!card.isClickable())
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            v = v - darkValue;
            color = Color.HSVToRGB(h,s,v);
            
        }
        image.color = color;

    }
}

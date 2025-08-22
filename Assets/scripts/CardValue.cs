using TMPro;
using UnityEngine;

public class CardValue : MonoBehaviour
{
    // Variables
    HandCard card; // Carta
    TextMeshProUGUI textMeshPro; // Componente de texto del valor de la carta

    void Start()
    {
        // Inicializa la carta y su texto
        card = GetComponentInParent<HandCard>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Si no hay carta, limpia el texto
        if (card.GetCard() == null)
        {
            textMeshPro.text = "";
            return;
        }

        // Si hay carta, actualiza el texto con el valor de la carta
        textMeshPro.text = card.GetCard().GetValue().ToString();
    }
}

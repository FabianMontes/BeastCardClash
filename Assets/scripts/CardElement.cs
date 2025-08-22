using UnityEngine;
using UnityEngine.UI;

public class CardElement : MonoBehaviour
{
    // Variables
    [Header("Elements")]
    [SerializeField] private Sprite[] elements; // Lista de íconos para cada elemento
    HandCard card; // Carta
    Image image; // Imagen de la carta

    void Start()
    {
        // Inicializa la carta y su imagen
        card = GetComponentInParent<HandCard>();
        image = transform.GetComponent<Image>();
    }

    void Update()
    {
        // Si no hay carta, deshabilita la imagen
        if (card.GetCard() == null)
        {
            image.enabled = false;
            return;
        }

        // Si hay carta, habilita la imagen y establece el ícono correspondiente
        image.enabled = true;
        image.sprite = elements[(int)card.GetCard().GetElement()];
    }
}

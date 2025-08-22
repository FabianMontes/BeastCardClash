using UnityEngine;
using UnityEngine.UI;

// TODO: Renombra esta clase para que sea PascalCase

public class cardborder : MonoBehaviour
{
    // Variables
    [Header("Colors")]
    [SerializeField] private Color[] colors; // Color de la carta para cada elemento
    [SerializeField] float darkValue = 0.2f; // Valor de luminosidad de la carta
    HandCard card; // Carta
    Image image; // Imagen de la carta

    void Start()
    {
        // Obtiene la carta y su imagen correspondiente
        // TODO: Cambiar image = transform.GetComponent<Image>(); por image = GetComponent<Image>();
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

        // Si hubo carta, establece su color y habilita la imagen
        Color color = colors[(int)card.GetCard().GetElement()];
        image.enabled = true;

        // Verifica si la carta no es seleccionable
        if (!card.isClickable() && !card.picker)
        {
            // Valores HSV del color
            // TODO: Pasar las variables como parámetros de RGBToHSV y HSVToRGB
            float h, s, v;

            // Convierte el color a HSV, cambia la luminosidad y vuelve a convertirlo a RGB aplicando el cambio
            Color.RGBToHSV(color, out h, out s, out v);
            v = darkValue;
            color = Color.HSVToRGB(h, s, v);
        }

        // Establece el color de la imagen
        image.color = color;
    }
}

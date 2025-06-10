using UnityEngine;

public class HolderPlay: MonoBehaviour
{
    [SerializeField] Card cardPicked;

    public Card GetPicked()
    {
        return cardPicked;
    }
    public void LosePick()
    {
        if (cardPicked != null)
        {
            cardPicked.gameObject.SetActive(true);

        }
        cardPicked = null;
    }
}

using TMPro;
using UnityEngine;

enum TypeFollow
{
    species, live
}

public class FolowerText : MonoBehaviour
{
    TextMeshProUGUI textMeshPro;
    Figther player;
    [SerializeField] TypeFollow typeFollow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        string text = textMeshPro.text;

        player = GetComponentInParent<Figther>();
        if(player != null)

        {
            switch (typeFollow)
            {
                case TypeFollow.species:
                    text = player.GetSpecie().ToString();
                    break;
                case TypeFollow.live:
                    text = player.GetPlayerLive().ToString();
                    break;
            }
            textMeshPro.text = text;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (typeFollow == TypeFollow.live)
        {
            textMeshPro.text = player.GetPlayerLive().ToString(); ;
        }
    }
}

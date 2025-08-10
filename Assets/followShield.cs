using UnityEngine;
using UnityEngine.UI;

public class followShield : MonoBehaviour
{
    [SerializeField] Sprite[] shields;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Figther figther;
    Image image;
    void Start()
    {
        figther = GetComponentInParent<Figther>();
        image = GetComponentInChildren<Image>();
        image.sprite = shields[(int) figther.team];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

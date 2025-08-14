using UnityEngine;
using UnityEngine.UI;

public class combatfollower : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Sprite[] types;
    Image image;
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.sprite = types[(int)Combatjudge.combatjudge.combatType];
    }
}

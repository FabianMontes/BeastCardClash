using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class liveSeter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Figther figther;
    Slider slider;
    TextMeshProUGUI texter;

    void Start()
    {
        figther = GetComponentInParent<Figther>();
        slider = GetComponentInChildren<Slider>();
        texter = GetComponentInChildren<TextMeshProUGUI>();
        
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = (float)figther.GetPlayerLive() / Combatjudge.combatjudge.initialLives;
        texter.text = figther.GetPlayerLive().ToString();
    }
}

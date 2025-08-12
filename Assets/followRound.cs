using TMPro;
using UnityEngine;

public class followRound : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    TextMeshProUGUI texter;
    void Start()
    {
        texter = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        texter.text = $"Ronda {Combatjudge.combatjudge.round}";
    }
}

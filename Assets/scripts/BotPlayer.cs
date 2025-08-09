using UnityEngine;

public class BotPlayer : MonoBehaviour
{
    Figther figther;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        figther = GetComponent<Figther>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

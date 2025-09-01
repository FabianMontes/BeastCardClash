using UnityEngine;
using UnityEngine.UI;

public class teamselect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] bool Follow;
    [SerializeField] Sprite[] teams;
    Image image; 
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Follow)
        {
            image.sprite = teams[(int)GameState.Singleton.Team];
        }
    }

    public void selectTeam(int team)
    {
        GameState.Singleton.SetTeam((Team)team);
    }
}

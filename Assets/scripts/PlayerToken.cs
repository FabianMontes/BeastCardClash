using UnityEngine;

[DefaultExecutionOrder(1)]
public class PlayerToken : MonoBehaviour
{
    public RockBehavior rocky;
    public Player player;

    public RockBehavior lastRock;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rocky != null)
        {

            transform.position = rocky.transform.position;
            lastRock = rocky; 
            rocky.AddPlayer(this);
        }
        SpriteRenderer rnd = transform.GetComponent<SpriteRenderer>();
        switch (player.GetSpecie())
        {
            case Specie.chameleon:
                rnd.color = Color.green;
                break;
            case Specie.bear:
                rnd.color = new Color(0.5f,0.35f,0.25f);
                break;
            case Specie.snake:
                rnd.color = new Color(1f, 0f, 1f);
                break;
            case Specie.frog:
                rnd.color = Color.yellow;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(rocky != null)
        {
            if(rocky.transform.position != transform.position)
            {
                lastRock.RemovePlayer(this);

                transform.position = rocky.transform.position;
                rocky.AddPlayer(this);
                lastRock = rocky ;
                Combatjudge.combatjudge.ArriveAtRock();
            }
            
        }
    }
}

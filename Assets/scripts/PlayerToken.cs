using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(1)]

public class PlayerToken : MonoBehaviour
{
    public RockBehavior rocky;
    public Figther player;


    public RockBehavior lastRock;

    CharacterController characterController;
    Vector3 destiny;
    Vector3 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        if (rocky != null)
        {
            transform.position = rocky.transform.position;
            lastRock = rocky;
            rocky.AddPlayer(this);
        }
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rocky != null && player.indexFigther == Combatjudge.combatjudge.turn())
        {
            if (rocky.transform.position != destiny)
            {
                lastRock.RemovePlayer(this);
                /*Vector3 des = rocky.transform.position;
                if(rocky.ManyPlayerOn() != 0)
                {
                    Vector3 loc = rocky.transform.localPosition;
                    loc.x = loc.x + 3*rocky.ManyPlayerOn();
                    rocky.transform.localPosition = loc;
                }*/
                destiny = rocky.transform.position;
                //rocky.transform.position = des;
                direction = destiny - transform.position;
                direction.y = 0;
                direction= direction.normalized;
                rocky.AddPlayer(this);
                lastRock = rocky;
                return;
            }
            if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.MoveToRock)
            {
                characterController.Move(direction * Time.deltaTime * 50);
                Vector3 dir = destiny - transform.position;
                dir.y = 0;
                dir = dir.normalized;
                if (dir != direction && Combatjudge.combatjudge.GetSetMoments() == SetMoments.MoveToRock)
                {
                    transform.position = destiny;
                    Combatjudge.combatjudge.ArriveAtRock();
                    characterController.Move(Vector3.zero);
                }
            }
            
        }
    }
}

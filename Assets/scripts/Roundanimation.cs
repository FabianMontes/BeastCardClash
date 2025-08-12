using UnityEngine;
using UnityEngine.UI;

public class Roundanimation : MonoBehaviour
{

    public static bool round { get; private set; }
    private bool showing = false;
    private bool estado = false;
    [SerializeField] float timedelay = 2;
    [SerializeField] float timetytime = 0;
    [SerializeField] float movescaletime = 0.5f;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        round = false;
        showing = false;
        Vector3 vector3 = transform.localScale;
        vector3.x = 0;
        transform.localScale = vector3;
    }

    // Update is called once per frame
    void Update()
    {

        if(showing && estado == false)
        {
            if (transform.localScale.x >= 1)
            {
                Vector3 vector3 = transform.localScale;
                vector3.x = 1;
                transform.localScale = vector3;
                estado = true;
                timetytime = Time.time;
            }
            else
            {
                Vector3 vector3 = transform.localScale;
                vector3.x =  vector3.x + Time.deltaTime /  movescaletime;
                transform.localScale = vector3;
            }
        }
        if(showing && estado == true)
        {
            if(Time.time - timetytime >= timedelay)
            {
                showing = false;
            }
        }
        if(showing == false && estado == true)
        {
            if (transform.localScale.x <= 0)
            {
                Vector3 vector3 = transform.localScale;
                vector3.x = 0;
                transform.localScale = vector3;
                estado = true;
                timetytime = Time.time;
            }
            else
            {
                Vector3 vector3 = transform.localScale;
                vector3.x = vector3.x - Time.deltaTime / movescaletime;
                transform.localScale = vector3;
                round = false;
                transform.parent.GetComponent<Image>().enabled = false;
                Combatjudge.combatjudge.endRoundScreen();
            }

        }
    }

    public void startRound()
    {
        if(round == true)
        {
            return;
        }
        round = true;
        showing = true ;
        transform.parent.GetComponent<Image>().enabled = true;
    }
}

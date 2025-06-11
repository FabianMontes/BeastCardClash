using UnityEngine;
using UnityEngine.UI;


[DefaultExecutionOrder(2)]
public class visualizehand : MonoBehaviour
{
    [SerializeField] Transform guide;
    bool activelast;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activelast = false;
        Transform hand = transform.GetChild(1);
        hand.GetComponent<Image>().enabled = activelast;
        for (int i = 0; i < hand.childCount; i++)
        {
            hand.GetChild(i).GetComponent<Image>().enabled = activelast;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool isactiv = guide.gameObject.activeSelf;

        activelast = isactiv;
        Transform hand = transform.GetChild(1);
        hand.GetComponent<Image>().enabled = activelast;
        for (int i = 0; i < hand.childCount; i++)
        {
            hand.GetChild(i).GetComponent<Image>().enabled = activelast;
            hand.GetChild(i).GetComponent<Button>().enabled = activelast;
            hand.GetChild(i).GetChild(0).gameObject.SetActive(activelast);
        }
        
    }
}

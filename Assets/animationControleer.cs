using OutlineFx;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class animationControleer : MonoBehaviour
{
    Animator animato;
    Figther figther;
    PlayerToken player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerToken>();
        figther = player.player;
        print(figther.indexFigther);
        setModel(figther.indexFigther);
        animato = transform.GetChild(figther.indexFigther).GetComponentInChildren<Animator>();
        animato.SetBool("isFigthing",true);
    }

    // Update is called once per frame
    void Update()
    {
        animato.SetFloat("Speed", player.Speed());
    }

    void setModel(int index)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);


        transform.GetChild(index).gameObject.SetActive(true);
    }
}

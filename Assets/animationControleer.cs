using OutlineFx;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class animationControleer : MonoBehaviour
{
    Animator animato;
    Fighter figther;
    PlayerToken player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerToken>();
        figther = player.player;
        print(figther.indexFighter);
        setModel(figther.indexFighter);
        animato = transform.GetChild(figther.indexFighter).GetComponentInChildren<Animator>();
        animato.SetBool("isFigthing", true);
    }

    // Update is called once per frame
    void Update()
    {
        animato.SetFloat("Speed", player.Speed());
        animato.SetBool("EndTurn", CombatJudge.Instance.GetSetMoments() == SetMoments.Result);
        animato.SetBool("didWin", figther.NoHurt);
        animato.SetInteger("ElementHurt", (int)CombatJudge.Instance.CombatType);
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

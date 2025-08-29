using UnityEngine;
using UnityEngine.UI;

public class warningdice : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Figther figther;
    Image image;
    void Start()
    {
        figther = GetComponentInParent<Figther>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        SetMoments momo = CombatJudge.CombatJudgeInstance.GetSetMoments();
        if(momo == SetMoments.PickDice || momo == SetMoments.RollDice)
        {
            if(CombatJudge.CombatJudgeInstance.Turn() == figther.indexFigther)
            {
                image.enabled = true;
                return;
            }
        }



        image.enabled = false;
    }
}

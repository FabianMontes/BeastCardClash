using UnityEngine;
using UnityEngine.UI;

public class warningdice : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Fighter figther;
    Image image;
    void Start()
    {
        figther = GetComponentInParent<Fighter>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        SetMoments momo = CombatJudge.Instance.GetSetMoments();
        if (momo == SetMoments.PickDice || momo == SetMoments.RollDice)
        {
            if (CombatJudge.Instance.Turn() == figther.indexFighter)
            {
                image.enabled = true;
                return;
            }
        }



        image.enabled = false;
    }
}

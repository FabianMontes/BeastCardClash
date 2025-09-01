using UnityEngine;
using UnityEngine.UI;

public class SelectType : MonoBehaviour
{
    Fighter figther;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        figther = GetComponentInParent<Fighter>();
        Visib(false);
    }

    // Update is called once per frame
    void Update()
    {
        SetMoments momo = CombatJudge.Instance.GetSetMoments();
        if (momo == SetMoments.SelectCombat && CombatJudge.Instance.FocusOnTurn() && figther.indexFighter == 0)
        {
            Visib(true);
        }
    }

    private void Visib(bool isVisible)
    {
        transform.GetChild(0).gameObject.SetActive(isVisible);
    }

    public void PickElement(int element)
    {
        if (CombatJudge.Instance.PickElement((Element)element)) Visib(false);
    }
}

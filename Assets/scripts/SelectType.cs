using UnityEngine;
using UnityEngine.UI;

public class SelectType : MonoBehaviour
{
    SetMoments prevSetMoment;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prevSetMoment = SetMoments.PickDice;
        Visib(false);
    }

    // Update is called once per frame
    void Update()
    {
        SetMoments momo = Combatjudge.combatjudge.GetSetMoments();
        if (momo != prevSetMoment)
        {
            if (momo == SetMoments.SelecCombat && GetComponentInParent<Figther>().IsFigthing()) Visib(true);
            prevSetMoment = momo;
        }
    }

    private void Visib(bool isVisible)
    {
        transform.GetChild(0).gameObject.SetActive(isVisible);
    }

    public void PickElement(int element)
    {
        if (Combatjudge.combatjudge.pickElement((Element)element)) Visib(false);
    }
}

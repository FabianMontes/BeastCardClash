using UnityEngine;
using UnityEngine.UI;

public class SelectType : MonoBehaviour
{
    Figther figther;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        figther = GetComponentInParent<Figther>();
        Visib(false);
    }

    // Update is called once per frame
    void Update()
    {
        SetMoments momo = Combatjudge.combatjudge.GetSetMoments();
        if (momo == SetMoments.SelecCombat && Combatjudge.combatjudge.FocusONTurn() && figther.indexFigther == 0)
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
        if (Combatjudge.combatjudge.pickElement((Element)element)) Visib(false);
    }
}

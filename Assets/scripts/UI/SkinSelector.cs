using OutlineFx;
using UnityEngine;

public class SkinSelector : MonoBehaviour
{
    // Variables
    [SerializeField] private int skinIndex; // Índice de la skin necesaria
    Outline outline;
    private void Start()
    {
        outline = GetComponentInChildren<Outline>();
        outline.enabled = false;
        GetComponent<Animator>().SetBool("isFigthing", true);
    }
    void OnMouseDown()
    {
        GameState.singleton.SetSkin(skinIndex);
        
    }


    private void OnMouseEnter()
    {
        outline.enabled = true;
    }

    private void OnMouseExit()
    {
        outline.enabled = false;
    }

}
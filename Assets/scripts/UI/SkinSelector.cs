using OutlineFx;
using UnityEngine;

public class SkinSelector : MonoBehaviour
{
    // Variables
    [Header("Skin Selector")]
    [SerializeField] private int skinIndex; // Índice de la skin necesaria
    Outline outline; // Componente de contorno para el personaje

    private void Start()
    {
        // Inicializa el Outline y el Animator y desactiva este último
        outline = GetComponentInChildren<Outline>();
        outline.enabled = false;
        GetComponent<Animator>().SetBool("isFigthing", true);
    }

    // Selecciona la skin al presionarla en el selector
    void OnMouseDown()
    {
        GameState.singleton.SetSkin(skinIndex);
    }

    // Activa el Outlne cuando el mouse está encima
    private void OnMouseEnter()
    {
        outline.enabled = true;
    }

    // Desactiva el Outline cuando el mouse sale
    private void OnMouseExit()
    {
        outline.enabled = false;
    }
}
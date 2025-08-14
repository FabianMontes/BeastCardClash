using UnityEngine;

public class SkinSelector : MonoBehaviour
{
    // Variables
    [SerializeField] private int skinIndex; // Índice de la skin necesaria

    void OnMouseDown()
    {
        GameState.singleton.SetSkin(skinIndex);
    }
}
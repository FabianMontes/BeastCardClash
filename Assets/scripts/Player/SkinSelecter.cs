using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(2)]
public class SkinSelecter: MonoBehaviour
{
    // Variables
    [SerializeField] private List<Sprite> Skins;

    private Renderer objRenderer;
    Figther figther;

    private void Awake()
    {
        objRenderer = GetComponentInChildren<Renderer>();
        figther = GetComponentInParent<PlayerToken>().player;
    }

    void Update()
    {
        // Actualiza la skin desde GameState
        figther = GetComponentInParent<PlayerToken>().player;
        SetSkin(figther.skin);
    }

    // Actualiza un parámetro del Animator. Detecta el tipo de parámetro y lo asigna desde un string.
    // Cambia la textura del material usando el Sprite en la posición indicada.
    public void SetSkin(int index)
    {
        if (Skins == null || index < 0 || index >= Skins.Count)
        {
            Debug.LogWarning("Índice de skin inválido.");
            return;
        }

        if (objRenderer != null && Skins[index] != null)
        {
            Texture2D texture = Skins[index].texture;
            objRenderer.material.mainTexture = texture;
        }
    }
}
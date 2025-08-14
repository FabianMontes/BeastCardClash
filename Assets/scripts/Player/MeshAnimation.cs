using UnityEngine;
using System.Collections.Generic;

public class MeshAnimation : MonoBehaviour
{
    // Variables
    [SerializeField] private List<Sprite> Skins;

    private Animator animator;
    private Renderer objRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        objRenderer = GetComponentInChildren<Renderer>();
    }

    // Actualiza un parámetro del Animator. Detecta el tipo de parámetro y lo asigna desde un string.
    public void UpdateAnimation(string variableName, string value)
    {
        if (animator == null) return;

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == variableName)
            {
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        if (bool.TryParse(value, out bool boolValue))
                            animator.SetBool(variableName, boolValue);
                        break;

                    case AnimatorControllerParameterType.Float:
                        if (float.TryParse(value, out float floatValue))
                            animator.SetFloat(variableName, floatValue);
                        break;

                    case AnimatorControllerParameterType.Int:
                        if (int.TryParse(value, out int intValue))
                            animator.SetInteger(variableName, intValue);
                        break;

                    case AnimatorControllerParameterType.Trigger:
                        animator.SetTrigger(variableName);
                        break;
                }
                return;
            }
        }

        Debug.LogWarning($"No se encontró el parámetro '{variableName}' en el Animator.");
    }

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
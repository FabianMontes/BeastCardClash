using UnityEngine;
using System.Collections.Generic;

public class MeshAnimation : MonoBehaviour
{
    // Variables
    [Header("Skins")]
    [SerializeField] private List<Sprite> Skins; // Lista de skins disponibles
    private Animator animator; // Componente del Animator
    private Renderer objRenderer; // Componente del Renderer

    private void Awake()
    {
        // Inicializa los componentes
        animator = GetComponent<Animator>();
        objRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        // Actualiza la skin desde GameState
        SetSkin(GameState.singleton.skin);
    }

    // Actualiza un parámetro del Animator. Detecta el tipo de parámetro y lo asigna desde un string.
    public void UpdateAnimation(string variableName, string value)
    {
        // Si no hay animator, se devuelve
        if (animator == null) return;

        // Revisa los parámetros del Animator
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            // Revisa si el nombre del parámetro coincide con el nombre proporcionado
            if (param.name == variableName)
            {
                // Por cada parámetro, verifica su tipo y asigna el valor correspondiente
                switch (param.type)
                {
                    // Booleano
                    case AnimatorControllerParameterType.Bool:
                        if (bool.TryParse(value, out bool boolValue)) animator.SetBool(variableName, boolValue);
                        break;
                    // Flotante (float)
                    case AnimatorControllerParameterType.Float:
                        if (float.TryParse(value, out float floatValue)) animator.SetFloat(variableName, floatValue);
                        break;
                    // Entero
                    case AnimatorControllerParameterType.Int:
                        if (int.TryParse(value, out int intValue)) animator.SetInteger(variableName, intValue);
                        break;
                    // Cadena de texto (string)
                    case AnimatorControllerParameterType.Trigger:
                        animator.SetTrigger(variableName);
                        break;
                }

                return;
            }
        }

        // Si no encuentra, alerta del error en consola
        Debug.LogWarning($"No se encontró el parámetro '{variableName}' en el Animator.");
    }

    // Cambia la textura del material usando el Sprite en la posición indicada.
    public void SetSkin(int index)
    {
        // Si no hay skin, o esta fuera de rango, alerta del error en consola
        if (Skins == null || index < 0 || index >= Skins.Count)
        {
            Debug.LogWarning("Índice de skin inválido.");
            return;
        }

        // Cambia la textura del material si existe
        if (objRenderer != null && Skins[index] != null)
        {
            Texture2D texture = Skins[index].texture;
            objRenderer.material.mainTexture = texture;
        }
    }
}
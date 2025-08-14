using UnityEngine;

[CreateAssetMenu(fileName = "MenuTexts", menuName = "Localizations/Menu Texts")]
public class MenuTexts : ScriptableObject
{
    // Textos en español
    [Header("Spanish")]
    public string startButton_es = "Iniciar";
    public string creditsButton_es = "Créditos";
    public string languagesLabel_es = "Idiomas";

    // Textos en inglés
    [Header("English")]
    public string startButton_en = "Start";
    public string creditsButton_en = "Credits";
    public string languagesLabel_en = "Languages";
}
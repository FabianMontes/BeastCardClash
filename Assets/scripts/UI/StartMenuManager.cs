using TMPro;
using UnityEngine;

public class StartMenuManager : MonoBehaviour
{
    // Singletons del juego
    [Header("Managers")]
    [SerializeField] private GameState gameState; // Para saber el estado y el idioma
    [SerializeField] private MenuTexts menuTexts; // Para obtener los textos traducidos

    // Elementos de la UI
    [Header("UI Elements (Start Menu)")]
    [SerializeField] private TextMeshProUGUI startButtonText;
    [SerializeField] private TextMeshProUGUI creditsButtonText;
    [SerializeField] private TextMeshProUGUI languagesLabelText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Al iniciar, actualizamos el texto con el idioma que ya está guardado en GameState
        if (gameState != null) UpdateUIText(gameState.CurrentLanguage);
    }

    // Establece el idioma desde afuera
    public void SetLanguage(int languageIndex)
    {
        // Convertimos el índice (0 para español, 1 para inglés) al tipo Languages
        Languages newLanguage = (Languages)languageIndex;

        // Le decimos al manager que cambie el idioma en el GameState
        gameState.SetLanguage(newLanguage);

        // Actualizamos el texto de la UI inmediatamente
        UpdateUIText(newLanguage);
    }

    // Sobreescribe el texto según el idioma
    private void UpdateUIText(Languages language)
    {
        if (menuTexts == null)
        {
            Debug.LogError("El asset 'MenuTexts' no está asignado en el StartMenuManager.");
            return;
        }

        if (language == Languages.spanish)
        {
            startButtonText.text = menuTexts.startButton_es;
            creditsButtonText.text = menuTexts.creditsButton_es;
            languagesLabelText.text = menuTexts.languagesLabel_es;
        }
        else // English
        {
            startButtonText.text = menuTexts.startButton_en;
            creditsButtonText.text = menuTexts.creditsButton_en;
            languagesLabelText.text = menuTexts.languagesLabel_en;
        }
    }
}
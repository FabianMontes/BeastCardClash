using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Lista de posibles paneles en el menú de tutorial
public enum CurrentPanel
{
    Panel1, Panel2, Panel3
}

public class TutorialMenuManager : MonoBehaviour
{
    // Paneles de tutorial en español
    [Header("Spanish Tutorial Panels")]
    [SerializeField] private GameObject esPanels; // Padre de los páneles
    [SerializeField] private RawImage panel1Es;
    [SerializeField] private RawImage panel2Es;
    [SerializeField] private RawImage panel3Es;

    // Paneles de tutorial en inglés
    [Header("English Tutorial Panels")]
    [SerializeField] private GameObject enPanels; // Padre de los páneles
    [SerializeField] private RawImage panel1En;
    [SerializeField] private RawImage panel2En;
    [SerializeField] private RawImage panel3En;

    // Botones y variables
    [Header("Buttons")]
    [SerializeField] private GameState gameState; // Para saber el estado y el idioma
    [SerializeField] private Button NextButton; // Botón de siguiente página
    private CurrentPanel CurrentPanel; // Panel actual

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Desactiva la rama de páneles correspondientes al idioma no usado
        InitializePanels();

        // Al iniciar, mostramos solo el primer panel
        ShowPanel1();
    }

    // Desactivan los páneles del idioma inactivo
    void InitializePanels()
    {
        if (gameState.CurrentLanguage == Languages.spanish)
        {
            // Activa el panel español y desactiva el inglés
            esPanels.gameObject.SetActive(true);
            enPanels.gameObject.SetActive(false);

            // Desactiva los paneles individuales de la rama activa
            panel1Es.gameObject.SetActive(false);
            panel2Es.gameObject.SetActive(false);
            panel3Es.gameObject.SetActive(false);
        }
        else
        {
            // Activa el panel inglés y desactiva el español
            enPanels.gameObject.SetActive(true);
            esPanels.gameObject.SetActive(false);

            // Desactiva los paneles individuales de la rama activa
            panel1En.gameObject.SetActive(false);
            panel2En.gameObject.SetActive(false);
            panel3En.gameObject.SetActive(false);
        }
    }

    // Cambian de panel en cada caso. Cada panel es una pagina diferente de tutorial
    // Estos métodos públicos se asignarán a los eventos OnClick de los botones en el editor de Unity
    public void ShowPanel1()
    {
        if (gameState.CurrentLanguage == Languages.spanish)
        {
            panel1Es.gameObject.SetActive(true);
            panel2Es.gameObject.SetActive(false);
            panel3Es.gameObject.SetActive(false);
        }
        else
        {
            panel1En.gameObject.SetActive(true);
            panel2En.gameObject.SetActive(false);
            panel3En.gameObject.SetActive(false);
        }

        CurrentPanel = CurrentPanel.Panel1;
    }
    public void ShowPanel2()
    {
        if (gameState.CurrentLanguage == Languages.spanish)
        {

            panel1Es.gameObject.SetActive(false);
            panel2Es.gameObject.SetActive(true);
            panel3Es.gameObject.SetActive(false);
        }
        else
        {
            panel1En.gameObject.SetActive(false);
            panel2En.gameObject.SetActive(true);
            panel3En.gameObject.SetActive(false);
        }

        CurrentPanel = CurrentPanel.Panel2;
    }
    public void ShowPanel3()
    {
        if (gameState.CurrentLanguage == Languages.spanish)
        {

            panel1Es.gameObject.SetActive(false);
            panel2Es.gameObject.SetActive(false);
            panel3Es.gameObject.SetActive(true);
        }
        else
        {
            panel1En.gameObject.SetActive(false);
            panel2En.gameObject.SetActive(false);
            panel3En.gameObject.SetActive(true);
        }

        CurrentPanel = CurrentPanel.Panel3;
        // NextButton.gameObject.SetActive(false); // Ocultamos el botón de siguiente (opcional)
    }

    // Cambia la función del botón en función del panel actual
    public void OnClick()
    {
        switch (CurrentPanel)
        {
            case CurrentPanel.Panel1:
                ShowPanel2();
                break;
            case CurrentPanel.Panel2:
                ShowPanel3();
                break;
            case CurrentPanel.Panel3:
                // Cuando llegamos al último panel, el botón hace la misma función de saltar
                SkipButton();
                break;
            default:
                break;
        }
    }

    // Botón de saltar: Se va a la escena World
    // Separado para poderse usar en el OnClick por sí solo
    public void SkipButton()
    {
        SceneManager.LoadScene("SkinSelector");
    }
}

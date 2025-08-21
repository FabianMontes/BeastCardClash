using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Lista de posibles paneles en el menú de tutorial
public enum CurrentPanel
{
    Panel1, Panel2, Panel3
}

// TODO: refactorizar este código, en las funciones de activación y desactivación del paneles

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

    void Start()
    {
        // Desactiva la rama de páneles correspondientes al idioma no usado
        // Y al iniciar, mostramos solo el primer panel
        InitializePanels();
        ShowPanel1();
    }

    // Desactiva los páneles del idioma inactivo
    void InitializePanels()
    {
        // Espaañol: desactiva lo del inglés
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
        // Inglés: desactiva lo del español
        else if (gameState.CurrentLanguage == Languages.english)
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
    // Se asignan a los botones en el editor de Unity
    // Muestra el panel 1
    public void ShowPanel1()
    {
        // Español
        if (gameState.CurrentLanguage == Languages.spanish)
        {
            panel1Es.gameObject.SetActive(true);
            panel2Es.gameObject.SetActive(false);
            panel3Es.gameObject.SetActive(false);
        }
        // Inglés
        else
        {
            panel1En.gameObject.SetActive(true);
            panel2En.gameObject.SetActive(false);
            panel3En.gameObject.SetActive(false);
        }

        CurrentPanel = CurrentPanel.Panel1;
    }

    // Muestra el panel 2
    public void ShowPanel2()
    {
        // Español
        if (gameState.CurrentLanguage == Languages.spanish)
        {
            panel1Es.gameObject.SetActive(false);
            panel2Es.gameObject.SetActive(true);
            panel3Es.gameObject.SetActive(false);
        }
        // Inglés
        else
        {
            panel1En.gameObject.SetActive(false);
            panel2En.gameObject.SetActive(true);
            panel3En.gameObject.SetActive(false);
        }

        CurrentPanel = CurrentPanel.Panel2;
    }

    // Muestra el panel 3
    public void ShowPanel3()
    {
        // Español
        if (gameState.CurrentLanguage == Languages.spanish)
        {
            panel1Es.gameObject.SetActive(false);
            panel2Es.gameObject.SetActive(false);
            panel3Es.gameObject.SetActive(true);
        }
        // Inglés
        else
        {
            panel1En.gameObject.SetActive(false);
            panel2En.gameObject.SetActive(false);
            panel3En.gameObject.SetActive(true);
        }

        CurrentPanel = CurrentPanel.Panel3;

        // Ocultamos el botón de siguiente (opcional)
        // NextButton.gameObject.SetActive(false);
    }

    // Cambia la función del botón de siguiente página en función del panel actual
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

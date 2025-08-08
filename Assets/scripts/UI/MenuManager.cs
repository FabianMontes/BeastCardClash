using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Menús
    [SerializeField] private Canvas StartMenu;
    [SerializeField] private Canvas CreditsMenu;
    [SerializeField] private Canvas TutorialMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Al iniciar, mostramos solo el menú principal.
        ShowStartMenu();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Estos métodos públicos se asignarán a los eventos OnClick de los botones en el editor de Unity
    // Cada uno muestra un menú diferente 
    public void ShowStartMenu()
    {
        StartMenu.gameObject.SetActive(true);
        CreditsMenu.gameObject.SetActive(false);
        TutorialMenu.gameObject.SetActive(false);
    }

    public void ShowCreditsMenu()
    {
        StartMenu.gameObject.SetActive(false);
        CreditsMenu.gameObject.SetActive(true);
        TutorialMenu.gameObject.SetActive(false);
    }

    public void ShowTutorialMenu()
    {
        StartMenu.gameObject.SetActive(false);
        CreditsMenu.gameObject.SetActive(false);
        TutorialMenu.gameObject.SetActive(true);
    }
}
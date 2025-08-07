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
    // Paneles existentes y panel actual
    [SerializeField] private RawImage panel1;
    [SerializeField] private RawImage panel2;
    [SerializeField] private RawImage panel3;
    private CurrentPanel CurrentPanel;

    // Botón de siguiente página (necesario para ocultarlo luego)
    [SerializeField] private Button NextButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Al iniciar, mostramos solo el primer panel.
        ShowPanel1();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Estos métodos públicos se asignarán a los eventos OnClick de los botones en el editor de Unity.
    // Cambian de panel en cada caso.
    // Cada panel es una pagina diferente de tutorial.
    public void ShowPanel1()
    {
        panel1.gameObject.SetActive(true);
        panel2.gameObject.SetActive(false);
        panel3.gameObject.SetActive(false);
        CurrentPanel = CurrentPanel.Panel1;
    }
    public void ShowPanel2()
    {
        panel1.gameObject.SetActive(false);
        panel2.gameObject.SetActive(true);
        panel3.gameObject.SetActive(false);
        CurrentPanel = CurrentPanel.Panel2;
    }
    public void ShowPanel3()
    {
        panel1.gameObject.SetActive(false);
        panel2.gameObject.SetActive(false);
        panel3.gameObject.SetActive(true);
        CurrentPanel = CurrentPanel.Panel3;
        NextButton.gameObject.SetActive(false); // Ocultamos el botón de siguiente
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
                // Cuando llegamos al tercer y último panel
                // El botón desaparece, asi que esta parte nunca se ejecuta, pero puedes mantener este código si lo deseas.
                SkipButton();
                break;
            default:
                break;
        }
    }

    // Botón de saltar: Se va a la escena World
    public void SkipButton()
    {
        SceneManager.LoadScene("World");
    }
}

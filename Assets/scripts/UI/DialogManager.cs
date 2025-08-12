using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    // Componentes de UI
    [SerializeField] private GameObject dialogPanel; // Panel de diálogos
    [SerializeField] private TextMeshProUGUI namePanel; // Panel del nombre del personaje
    [SerializeField] private TextMeshProUGUI textPanel; // Panel del texto del diálogo

    // Componentes de jugador y juego
    [SerializeField] private GameState gameStateManager; // Asset de estado del juego
    [SerializeField] private Transform target; // Target (que el jugador sigue). Necesario para desactivarlo
    [SerializeField] private float maxDistance = 3f; // Distancia máxima para activar el diálogo
    private Target targetScript; // Script del target (necesario para desactivarlo)

    // Componentes de diálogo
    private Dialogs currentDialog; // (eliminar)
    private int currentDialogIndex; // Índice del diálogo actual
    private bool inDialog; // Indica si está en un diálogo

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Inicializa el script del target y desactiva el panel de diálogo
        targetScript = target.GetComponent<Target>();
        dialogPanel?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Calcula la distancia y evalúa si esta en el rango
        float distance = Vector3.Distance(transform.position, target.position);
        bool targetInRange = distance <= maxDistance;

        // Si no está en un diálogo, está en el rango y presiona Z, muestra el diálogo
        if (!inDialog && targetInRange && Input.GetKeyDown(KeyCode.Z)) ShowDialogPanel();

        // Si está en diálogo
        if (inDialog)
        {
            // Si presiona Z, avanza al siguiente diálogo
            if (Input.GetKeyDown(KeyCode.Z))
            {
                currentDialogIndex++;
                DisplayNextDialog();
            }

            // Si presiona C, termina el diálogo
            if (Input.GetKeyDown(KeyCode.C)) EndDialog();
        }

        print($"Estado actual: {gameStateManager.CurrentGameState}");
    }

    // Muestra el panel de diálogo
    void ShowDialogPanel()
    {
        // Si ya está en un diálogo, no hace nada
        if (inDialog) return;

        // Activa el panel de diálogo y desactiva el target
        inDialog = true;
        dialogPanel.SetActive(true);
        targetScript.enabled = false;

        // Establece el índice en 0 y empieza
        currentDialogIndex = 0;
        DisplayNextDialog();
    }

    // Accede al dialogo correspondiente a cada estado del juego
    Dialogs[] GetDialogsForState()
    {
        // Carga el contenido del archivo de diálogos. Si no hay, devuelve un array vacío
        DialogFile dialogFileContent = gameStateManager.DialogFileContent;
        if (dialogFileContent == null) return new Dialogs[0];

        // Devuelve los diálogos correspondientes al estado actual del juego
        switch (gameStateManager.CurrentGameState)
        {
            case GameStates.begin:
                return dialogFileContent.BeginDialogs;
            case GameStates.preGame:
                return dialogFileContent.PreGameDialogs;
            case GameStates.win:
                return dialogFileContent.WinDialogs;
            case GameStates.lose:
                return dialogFileContent.LoseDialogs;
            default:
                return new Dialogs[0];
        }
    }

    // Muestra el siguiente diálogo
    void DisplayNextDialog()
    {
        // Accede a los diálogos según el estado del juego
        var dialogs = GetDialogsForState();

        // Si hay más diálogos, pasa al siguiente. Si no, termina
        if (currentDialogIndex < dialogs.Length)
        {
            namePanel.text = dialogs[currentDialogIndex].character;
            textPanel.text = dialogs[currentDialogIndex].text;
            currentDialog = dialogs[currentDialogIndex];
        }
        else
        {
            EndDialog();
        }
    }

    // Termina el diálogo
    void EndDialog()
    {
        // Cierra el panel y activa el movimiento
        inDialog = false;
        dialogPanel.SetActive(false);
        targetScript.enabled = true;

        // Reinicia el índice del diálogo
        currentDialogIndex = 0;

        // Pasa al siguiente estado del juego
        gameStateManager.NextGameState(gameStateManager.CurrentGameState);
    }
}
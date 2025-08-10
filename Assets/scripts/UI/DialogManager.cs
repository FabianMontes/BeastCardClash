using TMPro;
using UnityEngine;

// Estados del juego (normal, antes de jugar, al ganar y al perder)
public enum GameStates
{
    normal, preGame, win, lose
}

// Extrae los diálogos desde el JSON
[System.Serializable]
public class DialogFile
{
    public Dialogs[] Dialogs;
    public Dialogs[] PreGameDialogs;
    public Dialogs[] WinDialogs;
    public Dialogs[] LoseDialogs;
}

// Extrae los diálogos desde DialogFile
[System.Serializable]
public class Dialogs
{
    public string character;
    public string text;
}

public class DialogManager : MonoBehaviour
{
    // Componentes de UI
    [SerializeField] private GameObject dialogPanel; // Panel de diálogo en la UI
    [SerializeField] private TextMeshProUGUI namePanel; // Panel de nombre del personaje en la UI
    [SerializeField] private TextMeshProUGUI textPanel; // Panel de texto del diálogo en la UI

    [SerializeField] private string characterName; // Nombre del personaje

    // Componentes de jugador y juego
    [SerializeField] private Transform target; // Target (jugador)
    [SerializeField] private float maxDistance = 3f; // Distancia máxima para mostrar el diálogo
    [SerializeField] private GameStates gameState; // Estado actual del juego
    private Target targetScript; // Script de movimiento del target (necesario para desactivarlo)

    // Componentes de diálogos
    [SerializeField] private TextAsset dialogFile; // Archivo JSON con los diálogos
    private DialogFile dialogFileContent; // Datos extraídos del archivo JSON
    private Dialogs currentDialog; // Diálogo actual
    private int currentDialogIndex; // Índice del diálogo actual
    private bool inDialog; // Indica si el jugador está en un diálogo

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Carga el script del target y oculta el panel de diálogo
        targetScript = target.GetComponent<Target>();
        dialogPanel?.SetActive(false); // El operador de acceso condicional a nulos (?.) simplifica la comprobación

        // Cargar el archivo JSON en el objeto
        if (dialogFile != null) dialogFileContent = JsonUtility.FromJson<DialogFile>(dialogFile.text);
    }

    // Update is called once per frame
    void Update()
    {
        // Calcula la distancia entre el personaje y el target y verifica si esta dentro del máximo
        float distance = Vector3.Distance(transform.position, target.position);
        bool targetInRange = distance <= maxDistance;

        // Si el jugador está en rango y presiona 'Z', llama a la función de diálogo
        // De lo contrario, verifica que esté en diálogo y si se presiona 'Z', avanza al siguiente diálogo
        if (!inDialog && targetInRange && Input.GetKeyDown(KeyCode.Z))
        {
            ShowDialogPanel();
        }
        else if (inDialog && Input.GetKeyDown(KeyCode.Z))
        {
            currentDialogIndex++;
            DisplayDialog();
        }
    }

    // Cambia el estado del juego, desde afuera
    public void SetGameState(GameStates newState)
    {
        gameState = newState;
    }

    // Muestra el panel de diálogo
    void ShowDialogPanel()
    {
        if (inDialog) return; // Evita reiniciar si ya estás en diálogo

        // Activa los diálogos y desactiva el movimiento
        inDialog = true;
        targetScript.enabled = false;
        dialogPanel.SetActive(true);

        currentDialogIndex = 0; // Pone el diálogo inicial (posición 0)
        DisplayDialog();
    }

    // Extrae la secuencia de diálogos según el estado actual del juego
    Dialogs[] GetDialogsForState()
    {
        switch (gameState)
        {
            case GameStates.normal:
                return dialogFileContent.Dialogs;
            case GameStates.preGame:
                return dialogFileContent.PreGameDialogs;
            case GameStates.win:
                return dialogFileContent.WinDialogs;
            case GameStates.lose:
                return dialogFileContent.LoseDialogs;
            default:
                return new Dialogs[0]; // Si el estado actual no es reconocido, devuelve un array vacío
        }
    }

    // Muestra el siguiente diálogo
    void DisplayDialog()
    {
        var dialogs = GetDialogsForState(); // Obtiene los diálogos según el estado actual

        // Verifica que estemos en medio del diálogo y que haya más diálogos
        // Si es así, muestra el diálogo
        // Si no, cierra el diálogo y activa el movimiento
        if (currentDialogIndex < dialogs.Length)
        {
            namePanel.text = dialogs[currentDialogIndex].character;
            textPanel.text = dialogs[currentDialogIndex].text;
            currentDialog = dialogs[currentDialogIndex];
        }
        else
        {
            inDialog = false;
            dialogPanel.SetActive(false);
            targetScript.enabled = true;
            currentDialogIndex = 0; // Reinicia el índice del diálogo
        }
    }
}

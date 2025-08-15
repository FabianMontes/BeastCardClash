using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.SceneManagement;

// Idiomas disponibles en el juego
[Serializable]
public enum Languages
{
    spanish, english
}

// Estados del juego
[Serializable]
public enum GameStates
{
    begin, preGame, win, lose, Repeat
}

// Contenido del archivo de diálogos
[Serializable]
public class DialogFile
{
    public Dialogs[] BeginDialogs;
    public Dialogs[] PreGameDialogs;
    public Dialogs[] WinDialogs;
    public Dialogs[] LoseDialogs;
    public Dialogs[] RepeatDialogs;
}

// Componente de diálogo
[Serializable]
public class Dialogs
{
    public string character;
    public string text;
}
public class GameState : MonoBehaviour
{

    public DialogFile DialogFileContent;
    public GameStates CurrentGameState { get; private set; }
    public Languages CurrentLanguage => language; // Getter público para el idioma

    public static GameState singleton;

    public int skin { get; private set; }
    public Team team { get; private set; }

    // Variables
    [SerializeField] private GameStates gameState = GameStates.begin; // Estado actual del juego (para probar en el editor)
    [SerializeField] private Languages language; // Idioma actual (por defecto español)
    [SerializeField] private TextAsset spanishFile; // Archivo de diálogos en español
    [SerializeField] private TextAsset englishFile; // Archivo de diálogos en inglés
    private TextAsset selectedFile; // Archivo de diálogos seleccionado

    // OnEnable es llamado automáticamente por Unity
    private void OnEnable()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        team = Team.ingeniosos;

        // Carga el archivo de diálogos
        LoadDialogFile();

        // Establece el estado actual del juego
        CurrentGameState = gameState;
        language = Languages.spanish;
    }

    // Carga el archivo de diálogos basado en el idioma seleccionado
    private void LoadDialogFile()
    {
        // Carga el idioma elegido
        selectedFile = (language == Languages.spanish) ? spanishFile : englishFile;

        // Intenta cargar el archivo de diálogos correspondiente
        if (selectedFile != null)
        {
            DialogFileContent = JsonUtility.FromJson<DialogFile>(selectedFile.text);
        }
        else
        {
            DialogFileContent = null;
            Debug.LogError("¡Error en GameState! No se ha asignado un archivo de diálogos en el Inspector.");
        }
    }

    // Cambia el estado del juego desde otros scripts y actúa en consecuencia
    public void NextGameState(GameStates newState)
    {
        switch (newState)
        {
            // begin: pasa a preGame
            case GameStates.begin:
                CurrentGameState = GameStates.preGame;
                break;
            // preGame: salta a la escena de batalla
            case GameStates.preGame:
                SceneManager.LoadScene(3);
                break;
            // win: no definido
            case GameStates.win:
                CurrentGameState = GameStates.Repeat;
                
                break;
            // lose: no definido
            case GameStates.lose:
                CurrentGameState = GameStates.Repeat;
                
                break;
            case GameStates.Repeat:
                SceneManager.LoadScene(3);
                break;
            // Por defecto: no hace nada
            default:
                break;
        }
    }

    // Permite que un gestor externo (como LanguagesManager) cambie el idioma.
    public void SetLanguage(Languages newLanguage)
    {
        language = newLanguage;
        LoadDialogFile(); // Recargamos los diálogos con el nuevo idioma.
    }

    // Establece la skin
    public void SetSkin(int newSkin)
    {
        skin = newSkin;
    }

    // Establece el equipo
    public void SetTeam(Team team)
    {
        this.team = team;
    }
}
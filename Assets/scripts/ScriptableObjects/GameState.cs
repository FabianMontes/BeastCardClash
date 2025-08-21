using UnityEngine;
using System;
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
    begin, preGame, win, lose, repeat
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
    // Propiedades
    public GameStates CurrentGameState { get; private set; } // Getter público para el estado actual del juego
    public Languages CurrentLanguage => language; // Getter público para el idioma
    public static GameState singleton; // Instancia única del GameState
    public string playerName { get; private set; } // Getter público para el nombre del jugador
    public int skin { get; private set; } // Getter público para la skin
    public Team team { get; private set; } // Getter público para el equipo

    // Variables
    [Header("GameState, languages and files")]
    [SerializeField] private GameStates gameState = GameStates.begin; // Estado actual del juego (para probar en el editor)
    [SerializeField] private Languages language; // Idioma actual (por defecto español)
    [SerializeField] private TextAsset spanishFile; // Archivo de diálogos en español
    [SerializeField] private TextAsset englishFile; // Archivo de diálogos en inglés
    private TextAsset selectedFile; // Archivo de diálogos seleccionado
    public DialogFile dialogFileContent; // Contenido del archivo de diálogos

    private void OnEnable()
    {
        // Mantiene la instancia única del GameState
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Establece el equipo (por defecto: Ingeniosos)
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
        // Si no hay, alerta del error en consola y establece el contenido en null
        if (selectedFile != null)
        {
            dialogFileContent = JsonUtility.FromJson<DialogFile>(selectedFile.text);
        }
        else
        {
            dialogFileContent = null;
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
                SceneManager.LoadScene(2);
                CurrentGameState = GameStates.repeat;
                break;
            // lose: no definido
            case GameStates.lose:
                SceneManager.LoadScene(2);
                CurrentGameState = GameStates.repeat;
                break;
            // repeat: vuelve a la escena de inicio
            case GameStates.repeat:
                SceneManager.LoadScene(3);
                break;
            // Por defecto: no hace nada
            default:
                break;
        }
    }

    // Cambia el idioma desde afuera
    public void SetLanguage(Languages newLanguage)
    {
        language = newLanguage;

        // Recargamos los diálogos con el nuevo idioma
        LoadDialogFile();
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

    // Establece el nombre del jugador
    public void SetPlayer(string name)
    {
        this.playerName = name;
    }
}
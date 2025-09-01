using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/// <summary>Idiomas disponibles en el juego</summary>
[Serializable]
public enum Languages
{
    Spanish, English
}


/// <summary>Estados del juego: Inicio, Previo a la batalla, Ganar, Perder y Repetir</summary>
[Serializable]
public enum GameStates
{
    Begin, PreGame, Win, Lose, Repeat
}

/// <summary>Contenido del archivo de diálogos, Una sección por cada etapa de la demo</summary>
[Serializable]
public class DialogFile
{
    public Dialogs[] BeginDialogs;
    public Dialogs[] PreGameDialogs;
    public Dialogs[] WinDialogs;
    public Dialogs[] LoseDialogs;
    public Dialogs[] RepeatDialogs;
}

/// <summary>Componente de diálogo: nombre del personaje y el diálogo que dice<summary>
[Serializable]
public class Dialogs
{
    public string character;
    public string text;
}

/// <summary>Almacena y gestiona los estados del juego, el jugador, los diálogos e idiomas</summary>
public class GameState : MonoBehaviour
{
    // Propiedades
    public static GameState Singleton; // Instancia única del GameState
    public GameStates CurrentGameState { get; private set; } // Getter público para el estado actual del juego
    public Languages CurrentLanguage => language; // Getter público para el idioma
    public string PlayerName { get; private set; } // Getter público para el nombre del jugador
    public int Skin { get; private set; } // Getter público para la skin
    public Team Team { get; private set; } // Getter público para el equipo

    // Variables
    [Header("GameState, languages and files")]
    [SerializeField] private GameStates gameState = GameStates.Begin; // Estado actual del juego (para probar en el editor)
    [SerializeField] private Languages language; // Idioma actual (por defecto español)
    [SerializeField] private TextAsset spanishFile; // Archivo de diálogos en español
    [SerializeField] private TextAsset englishFile; // Archivo de diálogos en inglés
    private TextAsset _selectedFile; // Archivo de diálogos seleccionado
    public DialogFile dialogFileContent; // Contenido del archivo de diálogos

    /// <summary>Cuando está activo el GameState</summary>
    private void OnEnable()
    {
        // Mantiene la instancia única del GameState
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Establece el equipo (por defecto: Ingeniosos)
        Team = Team.Ingeniosos;

        // Carga el archivo de diálogos
        LoadDialogFile();

        // Establece el estado actual del juego
        CurrentGameState = gameState;
        language = Languages.Spanish;
    }

    /// <summary>Carga el archivo de diálogos basado en el idioma seleccionado</summary>
    private void LoadDialogFile()
    {
        // Carga el idioma elegido
        _selectedFile = language == Languages.Spanish ? spanishFile : englishFile;

        // Intenta cargar el archivo de diálogos correspondiente. Si no hay, alerta del error en consola y establece el contenido en null
        if (_selectedFile != null)
        {
            dialogFileContent = JsonUtility.FromJson<DialogFile>(_selectedFile.text);
        }
        else
        {
            dialogFileContent = null;
            Debug.LogError("¡Error en GameState! No se ha asignado un archivo de diálogos en el Inspector.");
        }
    }

    /// <summary>Cambia el estado del juego desde otros scripts y actúa en consecuencia</summary>
    /// <param name="newState">Nuevo estado</param>
    public void NextGameState(GameStates newState)
    {
        switch (newState)
        {
            // begin: pasa a preGame
            case GameStates.Begin:
                CurrentGameState = GameStates.PreGame;
                break;
            // preGame: salta a la escena de batalla
            case GameStates.PreGame:
                SceneManager.LoadScene(3);
                break;
            // Win y Lose: pasa a la escena del mundo y establece el estado en repeat
            case GameStates.Win:
            case GameStates.Lose:
                SceneManager.LoadScene(2);
                CurrentGameState = GameStates.Repeat;
                break;
            // repeat: vuelve a la escena de batalla
            case GameStates.Repeat:
                SceneManager.LoadScene(3);
                break;
            // Por defecto (error hipotético): carga la escena de mundo y pasa a repeat
            default:
                SceneManager.LoadScene(2);
                CurrentGameState = GameStates.Repeat;
                break;
        }
    }

    /// <summary>Cambia el idioma desde afuera</summary>
    /// <param name="newLanguage">Nuevo idioma</param>
    public void SetLanguage(Languages newLanguage)
    {
        language = newLanguage;

        // Recargamos los diálogos con el nuevo idioma
        LoadDialogFile();
    }

    /// <summary>Establece la skin</summary>
    /// <param name="newSkin">Nueva skin</param>
    public void SetSkin(int newSkin)
    {
        Skin = newSkin;
    }

    /// <summary>Establece el equipo </summary>
    /// <param name="team">Nuevo equipo</param>
    public void SetTeam(Team team)
    {
        this.Team = team;
    }

    /// <summary>Establece el nombre del jugador</summary>
    /// <param name="newName">Nuevo nombre</param>
    public void SetPlayer(string newName)
    {
        this.PlayerName = newName;
    }
}

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
    begin, preGame, win, lose
}

// Contenido del archivo de diálogos
[Serializable]
public class DialogFile
{
    public Dialogs[] BeginDialogs;
    public Dialogs[] PreGameDialogs;
    public Dialogs[] WinDialogs;
    public Dialogs[] LoseDialogs;
}

// Componente de diálogo
[Serializable]
public class Dialogs
{
    public string character;
    public string text;
}

[Serializable]
public class GameState : ScriptableObject
{
    // Propiedad pública para el estado actual del juego y para acceder a los diálogos ya cargados
    public DialogFile DialogFileContent { get; private set; } // Contenido del archivo de diálogos
    public GameStates CurrentGameState { get; private set; } // Estado actual del juego
    public static GameState instance; // Instancia singleton del GameState

    // Variables
    [SerializeField] private GameStates gameState = GameStates.begin; // Estado actual del juego (para probar en el editor)
    [SerializeField] private SceneAsset battleScene; // Escena de batalla
    [SerializeField] private Languages language; // Idioma actual
    [SerializeField] private TextAsset spanishFile; // Archivo de diálogos en español
    [SerializeField] private TextAsset englishFile; // Archivo de diálogos en inglés
    private TextAsset selectedFile; // Archivo de diálogos seleccionado

    // OnEnable es llamado automáticamente por Unity
    private void OnEnable()
    {
        // Carga el archivo de diálogos y establece el estado actual del juego
        LoadDialogFile();
        CurrentGameState = gameState;
    }

    // Carga el archivo de diálogos basado en el idioma seleccionado
    private void LoadDialogFile()
    {
        // Carga el idioma elegido
        switch (language)
        {
            case Languages.spanish:
                selectedFile = spanishFile;
                break;
            case Languages.english:
                selectedFile = englishFile;
                break;
            default:
                break;
        }

        // Intenta cargar el archivo de diálogos correspondiente
        if (selectedFile != null)
        {
            DialogFileContent = JsonUtility.FromJson<DialogFile>(selectedFile.text);
        }
        else
        {
            Debug.LogError("¡Error en GameState! No se ha asignado un archivo de diálogos en el Inspector.");
            DialogFileContent = null;
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
                SceneManager.LoadScene(battleScene.name);
                break;
            // win: no definido
            case GameStates.win:
                break;
            // lose: no definido
            case GameStates.lose:
                break;
            // Por defecto: no hace nada
            default:
                break;
        }
    }
}
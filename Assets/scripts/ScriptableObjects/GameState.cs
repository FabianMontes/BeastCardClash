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

[CreateAssetMenu(fileName = "GameState", menuName = "Managers/Game State")]
public class GameState : ScriptableObject
{
    // Propiedad pública para el estado actual del juego y para acceder a los diálogos ya cargados
    public DialogFile DialogFileContent { get; private set; }
    public GameStates CurrentGameState { get; private set; }
    public Languages CurrentLanguage => language; // Getter público para el idioma

    // Variables
    [SerializeField] private GameStates gameState = GameStates.begin; // Estado actual del juego (para probar en el editor)
    [SerializeField] private SceneAsset battleScene; // Escena de batalla
    [SerializeField] private Languages language; // Idioma actual (por defecto español)
    [SerializeField] private TextAsset spanishFile; // Archivo de diálogos en español
    [SerializeField] private TextAsset englishFile; // Archivo de diálogos en inglés
    private TextAsset selectedFile; // Archivo de diálogos seleccionado

    // OnEnable es llamado automáticamente por Unity
    private void OnEnable()
    {
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

    // Permite que un gestor externo (como LanguagesManager) cambie el idioma.
    public void SetLanguage(Languages newLanguage)
    {
        language = newLanguage;
        LoadDialogFile(); // Recargamos los diálogos con el nuevo idioma.
    }
}
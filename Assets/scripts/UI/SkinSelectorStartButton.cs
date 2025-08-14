using UnityEngine;
using UnityEngine.SceneManagement;

public class SkinSelectorStartButton : MonoBehaviour
{
    // Establece la escena del juego
    public void SetScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Tooltip("Level Scene")]
    public string firstLevelSceneName = "Level1";

    public void OnStartGameClicked()
    {
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void OnQuitGameClicked()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
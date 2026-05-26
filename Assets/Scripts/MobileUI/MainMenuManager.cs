using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scenes")]
    public string hubWorldScene = "HubWorld";
    public string storyScene    = "StoryScene";

    public void OnPlay()
    {
        SceneManager.LoadScene(hubWorldScene);
    }

    public void OnBack()
    {
        SceneManager.LoadScene(storyScene);
    }

    public void OnExit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
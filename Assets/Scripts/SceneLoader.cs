using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void BackToHub()
    {
        SceneManager.LoadScene("HubWorld");
    }
}
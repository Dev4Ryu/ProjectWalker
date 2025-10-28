using UnityEngine.SceneManagement;
using UnityEngine;

public class data_SceneHandler : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Starting Game");
        SceneManager.LoadSceneAsync(1);
    }

     public void Quit()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


namespace MenuAssets
{
    public class MenuHandler : MonoBehaviour
    {

        public void GoToLink(string url)
        {
            Application.OpenURL(url);
        }
        public void QuitGame()
        {
        Debug.Log("Quitting game...");
        Application.Quit();

        // Optional for Editor testing
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        }
    }
}
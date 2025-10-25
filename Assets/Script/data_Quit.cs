using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Exit()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

        // Optional for Editor testing
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSave : MonoBehaviour {
    
    public void Save() {
        PlayerPrefs.SetInt("SceneSaved", SceneManager.GetActiveScene().buildIndex);
        Debug.Log("saved");
    }

    public void Load() {
        SceneManager.LoadScene(PlayerPrefs.GetInt("SceneSaved"));
        Debug.Log("loaded");
    }

    public void LoadNextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

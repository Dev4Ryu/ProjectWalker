using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] public GameObject loadingScreen;
    [SerializeField] public GameObject UI;
    public void LoadLevelBtn(string levelToLoad)
    {
        if(UI != null){
            UI.SetActive(false);
        }
        Instantiate(loadingScreen);

        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    // Update is called once per frame
    IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            print(progressValue);
            
            yield return null;
        }        
    }
}

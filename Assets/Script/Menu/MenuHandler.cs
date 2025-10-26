using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


namespace MenuAssets
{
    public class MenuHandler : MonoBehaviour
    {

        private void PlayGame()
        {
            StartCoroutine(LoadingScene("UntitledScene"));
        }
        private void QuitGame()
        {
            Debug.Log("Exit Game");
            Application.Quit();
        }
        public IEnumerator LoadingScene(string SceneToLoad)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(SceneToLoad);

            while (!operation.isDone)
            {
                yield return null;
            }
        }
        
    }
}
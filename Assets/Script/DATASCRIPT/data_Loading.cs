using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    public static LoadingScene Instance;

    [SerializeField] public GameObject loadingScreen;
    [SerializeField] public GameObject UI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevelBtn(string levelToLoad)
    {
        if (UI != null)
            UI.SetActive(false);

        Instantiate(loadingScreen);
        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    private IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            Debug.Log(progressValue);

            yield return null;
        }
    }
}

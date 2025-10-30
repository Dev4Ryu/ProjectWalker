using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    public static LoadingScene Instance;

    [SerializeField] public GameObject loadingScreen;
    [SerializeField] public GameObject UI;
    [SerializeField] private float minimumLoadTime = 2.5f; // seconds to show splash

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
        float startTime = Time.time;
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        loadOperation.allowSceneActivation = false; // prevent it from switching early

        while (loadOperation.progress < 0.9f)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            Debug.Log("Loading progress: " + progressValue);
            yield return null;
        }

        // Wait until minimum display time passes
        float elapsed = Time.time - startTime;
        if (elapsed < minimumLoadTime)
            yield return new WaitForSeconds(minimumLoadTime - elapsed);

        // Now allow the scene to activate
        loadOperation.allowSceneActivation = true;
    }
}

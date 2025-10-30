using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public string sceneName; 
    void Awake()
    {
        LoadingScene.Instance.LoadLevelBtn(sceneName);   
    }
}

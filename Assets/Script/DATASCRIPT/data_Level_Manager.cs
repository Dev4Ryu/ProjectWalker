using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class LevelManager : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> path;
    public List<GameObject> newBlock;
    public List<GameObject> newPath;
    public int gapBlock;
    public float timeOut;
    public string loadSceneName;
    public LoadingScene loadSceneManager;
    private int cleanPath;
    private float timeTick;
    void Update()
    {
        timeTick += Time.deltaTime;
        foreach (GameObject obj in path)
        {
            if(obj != null){
                obj.transform.position -= new Vector3(0,0,0.4f);
            }
        }
        if(path[0].transform.position.z <= -35){
            GeneratePath();
        }
    }
    private void GeneratePath(){
        GameObject nextPath = Instantiate(newPath[Random.Range(0,newPath.Count)], path[path.Count-1].transform.position 
        + new Vector3(0,0,gapBlock),transform.rotation,this.transform);
        
        path.Add(nextPath);
        Destroy(path[0].gameObject);
        path.RemoveAt(0);
        if(timeTick >= timeOut){
            cleanPath++;
            print("clean: " + cleanPath);
            if(cleanPath >= path.Count + path.Count/2){
                timeTick = 0.0f;
                loadSceneManager.LoadLevelBtn(loadSceneName);
            }
        }else{
            GameObject nextBlock = Instantiate(newBlock[Random.Range(0,newBlock.Count)], 
            nextPath.transform.position,nextPath.transform.rotation,nextPath.transform);
        }
    }
}

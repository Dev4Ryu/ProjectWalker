using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation; // <--- IMPORTANT

namespace StarterAssets
{    
    public class LevelManager : MonoBehaviour
    {
        [System.Serializable]
        public class StoryPath
        {
            public GameObject path;
            public int level;
        }

        public List<GameObject> path;
        public List<GameObject> newPath;
        public List<StoryPath> storyPath;
        public GameObject blockPath;
        public int gapPath;
        public int level;
        public int playerDistance = 60;
        public LoadingScene loadSceneManager;

        public NavMeshSurface navMeshSurface; // <--- add this
        
        void Start()
        {
            level = path.Count;
            if (navMeshSurface != null)
            {
                navMeshSurface.BuildNavMesh();
            }
        }
        void Update()
        {
            if (Vector3.Distance(path[0].transform.position,
                TurnBaseManager.turnBaseData.player.transform.position) >= playerDistance)
            {
                GeneratePath();
                blockPath.transform.position = new Vector3(
                    blockPath.transform.position.x + gapPath, 0, 0);
            }
        }

        private void GeneratePath()
        {
            foreach (StoryPath spawnPath in storyPath)
            {
                if (level == spawnPath.level)
                {
                    SpawnPath(spawnPath.path);
                }
                else
                {
                    
                    SpawnPath(newPath[Random.Range(0, newPath.Count)]);
                }
            }
        }
        private void SpawnPath(GameObject spawnPath)
        {
            GameObject nextPath = Instantiate(spawnPath, path[path.Count - 1].transform.position + new Vector3(gapPath, 0, 0),
                transform.rotation,
                this.transform
            );

            path.Add(nextPath);
            DestroyImmediate(path[0].gameObject);
            path.RemoveAt(0);

            level++;

            if (navMeshSurface != null)
            {
                navMeshSurface.BuildNavMesh();
            }
        }
    }
}

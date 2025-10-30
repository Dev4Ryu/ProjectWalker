using UnityEngine;
using System.Collections.Generic;

namespace StarterAssets
{
    public class SpawnChance : MonoBehaviour
    {
        [System.Serializable]
        public class EnemyData
        {
            public GameObject character;  // Enemy prefab
            [Range(0f, 1f)] public float chance;  // 0 = never, 1 = always
            public int minLevel;           // Minimum level required to spawn
        }

        [Header("Enemy Spawn Settings")]
        public List<EnemyData> enemies = new List<EnemyData>();

        void Start()
        {
            int level = TurnBaseManager.turnBaseData.level;
            TrySpawnEnemy(level);
            Destroy(gameObject);
        }

        void TrySpawnEnemy(int level)
        {
            // Filter enemies that can spawn at this level
            List<EnemyData> available = enemies.FindAll(e => level >= e.minLevel);

            if (available.Count == 0)
            {
                Debug.Log("No enemies available for this level: " + level);
                return;
            }

            // Roll random chance
            foreach (EnemyData enemy in available)
            {
                if (Random.value <= enemy.chance)
                {
                    Instantiate(enemy.character, transform.position, transform.rotation);
                    Debug.Log($"Spawned enemy: {enemy.character.name} (Chance {enemy.chance * 100}%)");
                    return;
                }
            }

            Debug.Log("No enemy spawned (all rolls failed)");
        }
    }
}

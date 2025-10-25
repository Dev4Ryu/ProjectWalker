using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace StarterAssets
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance;
        public PlayerController player;
        public List<EnemyController> enemies = new List<EnemyController>();
        public float turnDelay = 1f;  // time between turns

        private int currentIndex = 0;
        private bool playerTurn = true;
        private bool busy = false;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (player == null)
                player = FindObjectOfType<PlayerController>();
            enemies.AddRange(FindObjectsOfType<EnemyController>());
        }

        void Update()
        {
            if (busy) return;

            if (playerTurn)
            {
                // Player can move or attack
                player.enabled = true;

                // Example condition: when player finishes their action
                if (!player._canAttack) // or another flag you define
                {
                    EndTurn();
                }
            }
            else
            {
                player.enabled = false;
                StartCoroutine(EnemyTurn());
            }
        }

        public void EndTurn()
        {
            playerTurn = !playerTurn;
            busy = true;
            Invoke(nameof(NextTurn), turnDelay);
        }

        void NextTurn()
        {
            busy = false;
        }

        IEnumerator EnemyTurn()
        {
            busy = true;
            foreach (var e in enemies)
            {
                e.enabled = true;
                yield return new WaitForSeconds(1.5f); // let enemy act
                e.enabled = false;
            }
            yield return new WaitForSeconds(turnDelay);
            playerTurn = true;
            busy = false;
        }
    }
}

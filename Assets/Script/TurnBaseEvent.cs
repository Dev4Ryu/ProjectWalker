using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace StarterAssets
{
    public class TurnBaseManager : MonoBehaviour
    {
        public static TurnBaseManager turnBaseData;
        public PlayerController player;
        public List<ControllerHandler> charQueue = new List<ControllerHandler>();
        public CombatHandler combatTurnBase;
        public Vector3 originDistance;

        public float distance;

        public bool _turnBaseMode = false;
        public int queue;
        public AIController charSelect;

        void Awake()
        {
            turnBaseData = this;
        }

        void Start()
        {
            if (player != null)
            {
                charQueue.AddRange(FindObjectsOfType<PlayerController>());
            }
            charQueue.AddRange(FindObjectsOfType<AIController>());
        }

        void Update()
        {
            _turnBaseMode = charQueue.Count > 1 ? true : false;

            if (_turnBaseMode)
            {
                TurnHandler();

                float distanceCost = Vector3.Distance(player.transform.position, originDistance);
                distance = distanceCost;
                float walkCost = (distance / 100) * combatTurnBase._maxAction;//walking cost ap decrease

                combatTurnBase._action = combatTurnBase._action - (int)walkCost;//calculation of walking
            }
            else
            {
                player.enabled = true;
            }
        }

        void TurnHandler()
        {
            queue = queue % charQueue.Count; //queue can be loop
            for (int i = 0; i < charQueue.Count; i++)
            {
                charQueue[i].enabled = i == queue ? true : false;//enabled only the character turn 

                if (charQueue[i].enabled)
                {
                    combatTurnBase = charQueue[i].GetComponent<CombatHandler>();
                }
            }
        }
        
    }
}
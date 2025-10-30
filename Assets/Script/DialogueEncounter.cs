using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class DialogueEncounter : MonoBehaviour
    {
        public DialogueBox.DialogueLines[] dialougeRoute;
        private void OnTriggerEnter(Collider other)
        {
            PlayerController _targetData = other.GetComponent<PlayerController>();
            if (_targetData != null)
            {
                TurnBaseManager.turnBaseData.dialogue.dialogueLines = dialougeRoute;
                TurnBaseManager.turnBaseData.dialogue.lineCount = 0;
                Destroy(gameObject);
            }
        }
    }
    public class DialogueEnding : DialogueEncounter
    {
        public DialogueBox.DialogueLines[] dialougeGoodEnd;
        void Update()
        {
            if(TurnBaseManager.turnBaseData.goodEnding)  dialougeRoute = dialougeGoodEnd;
        }
    }
}
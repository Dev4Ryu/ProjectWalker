using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class DialogueEncounter : MonoBehaviour
    {
        public DialogueBox.DialogueLines[] dialougeRoute;
        public DialogueBox.DialogueLines[] dialougeGoodEnd;
        private void OnTriggerEnter(Collider other)
        {
            if(TurnBaseManager.turnBaseData.dialogue.goodEnding)  dialougeRoute = dialougeGoodEnd;
            PlayerController _targetData = other.GetComponent<PlayerController>();
            if (_targetData != null)
            {
                TurnBaseManager.turnBaseData.dialogue.lineCount = 0;
                TurnBaseManager.turnBaseData.dialogue.dialogueLines = dialougeRoute;
                Destroy(gameObject);
            }
        }
    }
}
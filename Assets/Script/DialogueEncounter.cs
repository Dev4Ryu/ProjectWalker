using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace StarterAssets
{
    public class DialogueEncounter : MonoBehaviour
    {
        public DialogueBox.DialogueLines[] dialougeRoute;
        public DialogueBox.DialogueLines[] dialougeGoodEnd;
        public bool deleteEnd;
        private void OnTriggerEnter(Collider other)
        {
            if (TurnBaseManager.turnBaseData.dialogue.goodEnding && dialougeGoodEnd !=null) dialougeRoute = dialougeGoodEnd;
            PlayerController _targetData = other.GetComponent<PlayerController>();
            if (_targetData != null)
            {
                TurnBaseManager.turnBaseData.dialogue.lineCount = 0;
                TurnBaseManager.turnBaseData.dialogue.dialogueLines = dialougeRoute;
                if (!deleteEnd)
                    Destroy(gameObject);
            }
        }
        void OnTriggerStay()
        {
            if (TurnBaseManager.turnBaseData.dialogue.lineCount >= TurnBaseManager.turnBaseData.dialogue.dialogueLines.Length - 1 &&
            !TurnBaseManager.turnBaseData.dialogue._popUp)
            {
                if (deleteEnd)
                Destroy(gameObject);
            }
        }
    }
}
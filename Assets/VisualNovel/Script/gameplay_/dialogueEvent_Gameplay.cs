using UnityEngine;

namespace dialogueUI
{
    public class dialogueEvent_Gameplay : MonoBehaviour
    {
        public DialogueBox dialogueBox;

        void Start()
        {
            dialogueBox = GetComponent<DialogueBox>();   
        }
    }
}


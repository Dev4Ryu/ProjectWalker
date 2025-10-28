using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

namespace dialogueUI{
    public class DialogueBox : MonoBehaviour
    {
        [System.Serializable]
        public class DialogueLines {
            public string name;
            public string lines;
            public bool mirror;
            public GameObject[] eventObject;
            
        }
        public DialogueLines[] dialogueLines;
        public TextMeshProUGUI textComponent;
        public TextMeshProUGUI nameComponent;
        public float textSpeed;
        public GameObject target1;
        public GameObject target2;
        public float smoothPopup = 0.125f;
        private bool _popUp;
        private int lineCount;
        void Start()
        {
            textComponent.text = string.Empty;
            transform.position = target2.transform.position;
            StartDialogue();
        }

        void Update()
        {
            PopUp();
            if (Input.GetKeyDown("tab")){
                NextLines();
            }
            if (dialogueLines[lineCount].mirror){
                nameComponent.alignment = TextAlignmentOptions.Right;
            }else{
                nameComponent.alignment = TextAlignmentOptions.Left;
            }
            nameComponent.text = dialogueLines[lineCount].name;
        }
        void StartDialogue(){
            StartCoroutine(TypeLine());
            _popUp = true;
        }
        IEnumerator TypeLine(){
            foreach (char c in dialogueLines[lineCount].lines.ToCharArray())
            {
                textComponent.text += c;
                yield return new WaitForSeconds(textSpeed);
            }
        }
        void NextLines(){
            StopAllCoroutines();
            if(dialogueLines.Length -1 > lineCount){
                lineCount++;
                textComponent.text = string.Empty;
                StartCoroutine(TypeLine());
            }else{
                _popUp = false;
            }
        }
        void PopUp(){
            Vector3 target = _popUp ? target1.transform.position : target2.transform.position;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, target, smoothPopup);
            transform.position = smoothedPosition;
        }
    }
}
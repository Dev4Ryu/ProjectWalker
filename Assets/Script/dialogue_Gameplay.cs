using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace StarterAssets{
    public class DialogueBox : MonoBehaviour
    {
        [System.Serializable]
        [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
        public class DialogueRoute : ScriptableObject
        {
            public DialogueLines[] dialogueLines;
        }
        [System.Serializable]
        public class DialogueLines
        {
            public string name;
            public string lines;
            public bool mirror;
            public Texture backGround;
            public GameObject eventSpawn;
            public string effect;

        }
        private Animator cutSceneAnimator;
        public RawImage backGround;
        public AudioSource bgm;
        public DialogueLines[] dialogueLines;
        public DialogueRoute startDialogueRoute;
        public TextMeshProUGUI textComponent;
        public TextMeshProUGUI nameComponent;
        public float textSpeed;
        [Header("TextBox")]
        public GameObject textBox;
        public GameObject target1;
        public GameObject target2;
        public float smoothPopup = 0.125f;
        public bool _popUp;
        private int lineCount;
        void Start()
        {
            cutSceneAnimator = GetComponent<Animator>();
            bgm = GetComponent<AudioSource>();
            textComponent.text = string.Empty;
            textBox.transform.position = target2.transform.position;
            if (startDialogueRoute != null)
            {
                dialogueLines =  startDialogueRoute.dialogueLines;
            }
        }

        void Update()
        {
            if (!_popUp && lineCount == 0)
            {
                StartDialogue();
                PlayEffect("PopUp");
            }
            PopUp();
            if (dialogueLines[lineCount].mirror) {
                nameComponent.alignment = TextAlignmentOptions.Right;
            } else {
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
        public void NextLines(){
            StopAllCoroutines();
            if(dialogueLines.Length -1 > lineCount){
                lineCount++;
                textComponent.text = string.Empty;
                StartCoroutine(TypeLine());
                if (dialogueLines[lineCount].effect != "")
                {
                    PlayEffect(dialogueLines[lineCount].effect);
                }
                else if(dialogueLines[lineCount].backGround != null)
                {
                    PlayEffect("NextLine");
                }
            }else{
                _popUp = false;
                PlayEffect("PopDown");
            }
        }
        void PopUp()
        {
            Vector3 target = _popUp ? target1.transform.position : target2.transform.position;
            Vector3 smoothedPosition = Vector3.Lerp(textBox.transform.position, target, smoothPopup);
            textBox.transform.position = smoothedPosition;
        }
        public void DialogueShake(float time)
        {
            TurnBaseManager.turnBaseData.StartCoroutine(TurnBaseManager.turnBaseData.Shake(1f, time));
        }
        public void ChangeBGM(AudioSource audio)
        {
            bgm = audio;
        }
        public void PlayEffect(string effect)
        {
            cutSceneAnimator.CrossFadeInFixedTime(effect, 0);
            Debug.Log(effect);
        }
        public void ChangeBG()
        {
            if (dialogueLines[lineCount].backGround != null)
            {
                backGround.texture = dialogueLines[lineCount].backGround;
            }
        }
    }
}
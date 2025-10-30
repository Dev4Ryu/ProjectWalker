using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace StarterAssets{
    public class DialogueBox : MonoBehaviour
    {
        [System.Serializable]
        public class DialogueLines
        {
            public string name;
            public string lines;
            public bool mirror;
            public Texture backGround;
            public GameObject eventSpawn;
            public AudioClip music;
            public AudioClip sfx;
            public string effect;

        }
        private Animator cutSceneAnimator;
        public RawImage backGround;
        public AudioSource musicManager;
        public AudioSource sfxManager;
        public DialogueLines[] dialogueLines;
        public TextMeshProUGUI textComponent;
        public TextMeshProUGUI nameComponent;
        public float textSpeed;
        [Header("TextBox")]
        public GameObject textBox;
        public GameObject target1;
        public GameObject target2;
        public float smoothPopup = 0.125f;
        public bool _popUp;
        public int lineCount;
        private bool skip;
        void Start()
        {
            cutSceneAnimator = GetComponent<Animator>();
            textComponent.text = string.Empty;
            textBox.transform.position = target2.transform.position;
        }

        void Update()
        {
            if (dialogueLines[0].lines != "")
            {
                if (!_popUp && lineCount == 0)
                {
                    StartDialogue();
                    PlayEffect("PopUp");
                }
                PopUp();
                if (dialogueLines[lineCount].mirror)
                {
                    nameComponent.alignment = TextAlignmentOptions.Right;
                }
                else
                {
                    nameComponent.alignment = TextAlignmentOptions.Left;
                }
                nameComponent.text = dialogueLines[lineCount].name;
            }
            musicManager.enabled = _popUp ? true : false;
            sfxManager.enabled = _popUp ? true : false;
        }
        void StartDialogue()
        {
            StartCoroutine(TypeLine());
            EventManager();
            _popUp = true;
        }
        void EventManager()
        {
            if (dialogueLines[lineCount].music != null)
                ChangeSound(musicManager, dialogueLines[lineCount].music);
            if (dialogueLines[lineCount].sfx != null)
                ChangeSound(sfxManager, dialogueLines[lineCount].sfx);
            if (dialogueLines[lineCount].effect != "")
            {
                PlayEffect(dialogueLines[lineCount].effect);
            }
            else if(dialogueLines[lineCount].backGround != null)
            {
                PlayEffect("NextLine");
            }
        }
        IEnumerator TypeLine(){
            foreach (char c in dialogueLines[lineCount].lines.ToCharArray())
            {
                textComponent.text += c;
                yield return new WaitForSeconds(textSpeed);
            }
        }
        public void NextLines(){
            if (!skip) return;

            StopAllCoroutines();
            if(dialogueLines.Length -1 > lineCount){
                lineCount++;
                textComponent.text = string.Empty;
                StartCoroutine(TypeLine());
                EventManager();
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
        public void ChangeSound(AudioSource audioSource,AudioClip audio)
        {
            audioSource.clip = audio;
            audioSource.Play();
        }
        public void PlayEffect(string effect)
        {
            cutSceneAnimator.CrossFadeInFixedTime(effect, 0);
            StartCoroutine(Skip());
            Debug.Log(effect);
        }
        public void ChangeBG()
        {
            if (dialogueLines[lineCount].backGround != null)
            {
                backGround.texture = dialogueLines[lineCount].backGround;
            }
        }
        public void NewDialouge(DialogueEncounter newDialogue)
        {
            dialogueLines = newDialogue.dialougeRoute;
            lineCount = 0;
        }
        IEnumerator Skip()
        {
            skip = false;
            AnimatorStateInfo stateInfo = cutSceneAnimator.GetCurrentAnimatorStateInfo(0);
            float realLength = stateInfo.length / stateInfo.speed;

            yield return new WaitForSeconds(stateInfo.length);
            skip = true;
        }
        
    }
}
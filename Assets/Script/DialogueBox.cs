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
        [Header("Story")]
        public bool goodEnding;
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
            _popUp = true;
            sfxManager.enabled = true;
            sfxManager.clip = null;
            EventManager();
            TurnBaseManager.turnBaseData.bgm.enabled = false;
        }
        void EventManager()
        {
            if (dialogueLines[lineCount].effect != "")
            {
                PlayEffect(dialogueLines[lineCount].effect);
            }
            if (dialogueLines[lineCount].music != null)
                ChangeSound(musicManager, dialogueLines[lineCount].music);
            if (dialogueLines[lineCount].sfx != null)
                ChangeSound(sfxManager, dialogueLines[lineCount].sfx);
            else if (dialogueLines[lineCount].backGround != null)
            {
                PlayEffect("NextLine");
            }
            else if (lineCount == 0)
            {
                PlayEffect("PopUp");
            }
            if (dialogueLines[lineCount].eventSpawn != null)
            {
                Instantiate(dialogueLines[lineCount].eventSpawn);
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
                TurnBaseManager.turnBaseData.bgm.enabled = true;
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
            _popUp = false;
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
        public void endingRoute(bool good)
        {
            goodEnding = good;
        }
        
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Cinemachine;
using System.Runtime.InteropServices;

namespace StarterAssets
{
    public class TurnBaseManager : MonoBehaviour
    {
        [Header("CombatManager")]
        [Tooltip("comabt properties and turn base queue")]
        public static TurnBaseManager turnBaseData;
        public int level;
        public AudioSource bgm;
        public AudioClip[] bgmClip;
        public PlayerController player;
        public List<ControllerHandler> charQueue = new List<ControllerHandler>();
        public DialogueBox dialogue;
        public CombatHandler combatTurnBase;
        public Transform originPos;
        public bool savedOriginal;
        public bool _turnBaseMode = false;
        public int queue;
        public int currentAction;
        public ControllerHandler charSelect;
        [Header("ZoomCamera")]
        [Tooltip("camera")]
        public float zoomMax = 20f;
        public float zoomMin = 10f;
        public float zoomSpeed = 10f;
        public float zoomFactor = 200f;
        public float zoom = 20f;
        private float cameraDistance = 20f;
        public CinemachinePositionComposer cinemachineVirtualCamera;

        void Awake()
        {
            turnBaseData = this;
            if (cinemachineVirtualCamera == null)
            {
                cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("CinemachineTarget").GetComponent<CinemachinePositionComposer>();
            }
        }

        void Start()
        {
            if (player != null)
            {
                charQueue.Add(FindObjectOfType<PlayerController>());
            }
            if(dialogue != null)
            {
                dialogue = FindObjectOfType<DialogueBox>();
            }
        }

        void Update()
        {

            _turnBaseMode = charQueue.Count > 1 ? true : false;
            if (charQueue.Count == 1 && !dialogue._popUp)
            {
                player.enabled = true;
                bgm.enabled = true;
                queue = 0;
                if (bgm.clip == bgmClip[0]) return;
                bgm.clip = bgmClip[0];
                bgm.Play();
                originPos.gameObject.SetActive(false);
            }
            else if (_turnBaseMode)
            {
                TurnHandler();
                if (bgm.clip == bgmClip[1]) return;
                bgm.clip = bgmClip[1];
                bgm.Play();
                originPos.gameObject.SetActive(player.enabled ? true : false);
            }
        }
        void LateUpdate()
        {
             CameraZooming();
        }

        void TurnHandler()
        {
            charQueue.RemoveAll(item => item == null);

            if (charQueue.Count == 0) return;

            queue = queue % charQueue.Count; // queue can loop

            SaveOrigin(charQueue[queue].transform.position);

            for (int i = 0; i < charQueue.Count; i++)
            {
                charQueue[i].enabled = i == queue; // enable only the active turn

                if (charQueue[i].enabled)
                {
                    combatTurnBase = charQueue[i].GetComponent<CombatHandler>();
                    float distance = Vector3.Distance(charQueue[i].transform.position, originPos.position);
                    float walkCost = (distance / 10) * currentAction;//walking cost ap decrease

                    currentAction = (int)((int)currentAction - walkCost) + 1;//calculation of walking
                    if (walkCost >= currentAction)
                    {
                        if (player.enabled == true)
                        {
                            Vector3 direction = (originPos.position - player.transform.position).normalized;
                            player._controller.Move(direction * combatTurnBase._speedMove * Time.deltaTime);
                        }
                    }
                }
            }
        }

        void CameraZooming()
        {
            print("work");
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            zoom -= scrollInput * zoomFactor;
            zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
            cameraDistance = Mathf.Lerp(cameraDistance, this.zoom, Time.deltaTime * this.zoomSpeed);

            cinemachineVirtualCamera.CameraDistance = cameraDistance;
        }
        public IEnumerator Shake(float intensity, float duration)
        {
            CinemachineBasicMultiChannelPerlin perlin = cinemachineVirtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();


            float originalIntensity = 0;
            perlin.AmplitudeGain = intensity;

            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            perlin.AmplitudeGain = originalIntensity;
        }
        public void SaveOrigin(Vector3 origin)
        {
            if (savedOriginal == true)
                originPos.position = origin;
            savedOriginal = false;
        }
        public void NextQueue()
        {
            queue++;
            savedOriginal = true;
            currentAction = charQueue[queue]._combat._maxAction;
        }
        
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Cinemachine;

namespace StarterAssets
{
    public class TurnBaseManager : MonoBehaviour
    {
        [Header("CombatManager")]
        [Tooltip("comabt properties and turn base queue")]
        public static TurnBaseManager turnBaseData;
        public PlayerController player;
        public List<ControllerHandler> charQueue = new List<ControllerHandler>();
        public CombatHandler combatTurnBase;
        public Vector3 originDistance;

        public float distance;

        public bool _turnBaseMode = false;
        public int queue;
        public AIController charSelect;
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
                queue = 0;
            }
            CameraZooming();
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
        void CameraZooming()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            zoom -= scrollInput * zoomFactor;
            zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
            cameraDistance = Mathf.Lerp(cameraDistance, this.zoom, Time.deltaTime * this.zoomSpeed);

            cinemachineVirtualCamera.CameraDistance = cameraDistance;
        }
        public IEnumerator Shake(float intensity, float duration)
        {
            CinemachineBasicMultiChannelPerlin perlin = cinemachineVirtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

            if (perlin != null)
            {
                float originalIntensity = perlin.AmplitudeGain;
                perlin.AmplitudeGain = intensity;

                float timer = 0f;
                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    yield return null;
                }

                perlin.AmplitudeGain = originalIntensity;
            }
        }
        
    }
}
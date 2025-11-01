using System.Net;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace StarterAssets
{
    public class ActionHud : MonoBehaviour
    {
        public GameObject targetAction;
        public Image characterAction;
        public Image playerAction;

        public float baseScale = 1f;     // Normal size when close
        public float scaleDistance = 5f; // Distance where scale starts to shrink
        public float minScale = 0.3f;    // Minimum scale when far

        void Update()
        {
            if (TurnBaseManager.turnBaseData.charSelect != null)
            {
                targetAction = TurnBaseManager.turnBaseData.charSelect.gameObject;

                if (targetAction.TryGetComponent<PlayerController>(out PlayerController _player))
                {
                    LockOnTarget(playerAction);

                    playerAction.gameObject.SetActive(true);
                    characterAction.gameObject.SetActive(false);
                    print("player");

                }
                else if (targetAction.TryGetComponent<AIController>(out AIController _enemy))
                {
                    LockOnTarget(characterAction);

                    playerAction.gameObject.SetActive(false);
                    characterAction.gameObject.SetActive(true);
                    print("enemy");
                }
            }
            if(!TurnBaseManager.turnBaseData.player.enabled || TurnBaseManager.turnBaseData.charSelect == null)
            {
                playerAction.gameObject.SetActive(false);
                characterAction.gameObject.SetActive(false);
            }
        }
        void LockOnTarget(Image actionMenu)
        {
            if (targetAction == null || actionMenu == null)
                return;

            Transform target = FindChildWithTag(targetAction.transform, "CinemachineTarget");

            // Use that target’s position if found, otherwise use targetAction’s own position
            Vector3 worldPos = target != null ? target.position : targetAction.transform.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            // Move the UI element to that screen position
            actionMenu.transform.position = new Vector3(screenPos.x, screenPos.y, 0);

            // Calculate distance for scaling
            float distance = Vector3.Distance(Camera.main.transform.position, targetAction.transform.position);

            // Scale inversely by distance (closer = bigger)
            float scale = baseScale / (distance / scaleDistance);
            scale = Mathf.Clamp(scale, minScale, baseScale);

            actionMenu.rectTransform.localScale = Vector3.one * scale;
        }

        Transform FindChildWithTag(Transform parent, string tag)
        {
            foreach (Transform child in parent)
            {
                if (child.CompareTag(tag))
                    return child;

                // Recursively search deeper children
                Transform result = FindChildWithTag(child, tag);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}

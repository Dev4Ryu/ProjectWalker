using UnityEngine;
using UnityEngine.UI;

namespace StarterAssets
{
    public class ActionHud : MonoBehaviour
    {
        public GameObject targetAction;
        public Image characterAction;

        public float baseScale = 1f;     // Normal size when close
        public float scaleDistance = 5f; // Distance where scale starts to shrink
        public float minScale = 0.3f;    // Minimum scale when far

        void Update()
        {
            if (targetAction == null || characterAction == null)
                return;

            Transform target = FindChildWithTag(targetAction.transform, "CinemachineTarget");

            // Use that target’s position if found, otherwise use targetAction’s own position
            Vector3 worldPos = target != null ? target.position : targetAction.transform.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            // Move the UI element to that screen position
            characterAction.transform.position = new Vector3(screenPos.x, screenPos.y, 0);

            // Calculate distance for scaling
            float distance = Vector3.Distance(Camera.main.transform.position, targetAction.transform.position);

            // Scale inversely by distance (closer = bigger)
            float scale = baseScale / (distance / scaleDistance);
            scale = Mathf.Clamp(scale, minScale, baseScale);

            characterAction.rectTransform.localScale = Vector3.one * scale;
        }

        void LateUpdate()
        {
            if (TurnBaseManager.turnBaseData.charSelect != null)
            {
                characterAction.gameObject.SetActive(true);
                targetAction = TurnBaseManager.turnBaseData.charSelect.gameObject;
            }
            else
            {
                characterAction.gameObject.SetActive(false);
            }
        }

        // ✅ Helper function to find a child (or grandchild) with a specific tag
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

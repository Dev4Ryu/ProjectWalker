using UnityEngine;
using UnityEngine.UI;

public class ActionHud : MonoBehaviour
{
    public GameObject someGameObject;
    public Image uiIcon;

    public float baseScale = 1f;     // Normal size when close
    public float scaleDistance = 5f; // Distance where scale starts to shrink
    public float minScale = 0.3f;    // Minimum scale when far

    void Update()
    {
        if (someGameObject == null || uiIcon == null)
            return;

        // Get world and screen position
        Vector3 worldPos = someGameObject.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // If object is behind the camera, hide icon
        // if (screenPos.z < 0)
        // {
        //     uiIcon.enabled = false;
        //     return;
        // }

        uiIcon.enabled = true;
        uiIcon.transform.position = new Vector3(screenPos.x, screenPos.y, 0);

        // Calculate distance
        float distance = Vector3.Distance(Camera.main.transform.position, someGameObject.transform.position);

        // Scale inversely by distance (closer = bigger)
        float scale = baseScale / (distance / scaleDistance);
        scale = Mathf.Clamp(scale, minScale, baseScale);

        uiIcon.rectTransform.localScale = Vector3.one * scale;
    }
}

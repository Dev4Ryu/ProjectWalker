using UnityEngine;

public class UIBackgroundMouseMove : MonoBehaviour
{
    [SerializeField] private RectTransform background; // The UI element to move
    [SerializeField] private float moveAmount = 50f;    // How far it moves relative to mouse
    [SerializeField] private float smoothSpeed = 5f;    // Smoothing speed

    private Vector3 targetPos;

    void Update()
    {
        // Get mouse position normalized to -1 ~ 1 (center of screen is 0)
        float x = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float y = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Calculate target position
        targetPos = new Vector3(-x * moveAmount, -y * moveAmount, 0);

        // Smoothly move background
        background.anchoredPosition = Vector3.Lerp(background.anchoredPosition, targetPos, Time.deltaTime * smoothSpeed);
    }
}

using UnityEngine;

[ExecuteInEditMode]  // 👈 Makes this script run in Edit Mode too
public class LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null)
        {
            // Try to find camera again if it was null (useful in editor)
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        // Make the sprite face the camera direction
        transform.forward = mainCamera.transform.forward;

        // OR use this line for classic billboard behavior (facing camera position)
        // transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
        //                  mainCamera.transform.rotation * Vector3.up);
    }
}

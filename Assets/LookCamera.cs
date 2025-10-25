using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // Make the sprite face the camera
        transform.forward = mainCamera.transform.forward;

        // OR use this line if you want it to rotate to face camera position (classic billboard)
        // transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
        //                  mainCamera.transform.rotation * Vector3.up);
    }
}

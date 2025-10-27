using UnityEngine;

[ExecuteInEditMode]
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
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        // Copy camera rotation without affecting parent transforms
        transform.rotation = Quaternion.LookRotation(
            mainCamera.transform.forward,
            mainCamera.transform.up
        );
    }
}

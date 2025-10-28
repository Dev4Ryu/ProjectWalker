using UnityEngine;
using System.Collections.Generic;

public class SpriteVisibilityByCamera : MonoBehaviour
{
    [Header("Culling Settings")]
    public float buffer = 1f; // extra margin outside the screen before hiding

    private Camera cam;
    private List<SpriteRenderer> allSprites = new List<SpriteRenderer>();
    private Plane[] camPlanes;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("This script must be attached to a Camera!");
            enabled = false;
            return;
        }

        // Find all sprites in the scene
        SpriteRenderer[] foundSprites = FindObjectsOfType<SpriteRenderer>();
        allSprites.AddRange(foundSprites);
    }

    void LateUpdate()
    {
        // Get the camera’s frustum planes
        camPlanes = GeometryUtility.CalculateFrustumPlanes(cam);

        foreach (var sr in allSprites)
        {
            if (sr == null) continue;

            Bounds bounds = sr.bounds;
            bounds.Expand(buffer); // small margin to prevent flickering

            // Check if sprite is within camera frustum
            bool isVisible = GeometryUtility.TestPlanesAABB(camPlanes, bounds);
            sr.enabled = isVisible;
        }
    }
}

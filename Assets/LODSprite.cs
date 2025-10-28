using UnityEngine;
using System.Collections.Generic;

public class SpriteVisibilityByCamera : MonoBehaviour
{
    [Header("Culling Settings")]
    public float buffer = 1f; // extra margin outside the screen before hiding
    public float refreshRate = 0.5f; // how often to rescan for new sprites

    private Camera cam;
    private readonly List<SpriteRenderer> allSprites = new List<SpriteRenderer>();
    private Plane[] camPlanes;
    private float refreshTimer;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("This script must be attached to a Camera!");
            enabled = false;
            return;
        }

        RefreshSpriteList();
    }

    void Update()
    {
        // Regularly refresh sprite list for newly created or destroyed sprites
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= refreshRate)
        {
            RefreshSpriteList();
            refreshTimer = 0f;
        }
    }

    void LateUpdate()
    {
        camPlanes = GeometryUtility.CalculateFrustumPlanes(cam);

        for (int i = 0; i < allSprites.Count; i++)
        {
            var sr = allSprites[i];
            if (sr == null)
                continue; // skip destroyed sprites

            Bounds bounds = sr.bounds;
            bounds.Expand(buffer);

            bool isVisible = GeometryUtility.TestPlanesAABB(camPlanes, bounds);
            sr.enabled = isVisible;
        }
    }

    private void RefreshSpriteList()
    {
        allSprites.Clear();
        allSprites.AddRange(FindObjectsOfType<SpriteRenderer>());
    }
}

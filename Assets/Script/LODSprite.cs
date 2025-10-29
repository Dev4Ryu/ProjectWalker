using UnityEngine;
using System.Collections.Generic;

namespace StarterAssets
{
    public class LODSPrite : MonoBehaviour
    {
        [Header("Culling Settings")]
    public float buffer = 1f;
    public float refreshRate = 0.5f;
    [Range(0f, 1f)] public float seeThroughAlpha = 0.5f;

    [Header("Debug Info")]
    public List<SpriteRenderer> blockedRenderers = new List<SpriteRenderer>(); // environment objects blocking any character

    private Camera cam;
    private float refreshTimer;

    private int characterLayer;
    private int environmentLayer;
    private LayerMask environmentMask;

    private readonly HashSet<SpriteRenderer> fadedSprites = new HashSet<SpriteRenderer>();

    // 🔧 For Gizmo drawing
    private readonly List<(Vector3 start, Vector3 end, bool hit)> debugRays = new List<(Vector3, Vector3, bool)>();

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("This script must be attached to a Camera!");
            enabled = false;
            return;
        }

        characterLayer = LayerMask.NameToLayer("Character");
        environmentLayer = LayerMask.NameToLayer("Environment");
        environmentMask = 1 << environmentLayer;
    }

    void Update()
    {
            RefreshObstruction();
    }

    void RefreshObstruction()
    {
        ResetFadedSprites();
        blockedRenderers.Clear();
        debugRays.Clear(); // 🔧 reset gizmo rays each refresh

        var queue = TurnBaseManager.turnBaseData.charQueue;
        if (queue == null || queue.Count == 0)
            return;

        foreach (var controller in queue)
        {
            if (controller == null) continue;

            var sr = controller.GetComponentInChildren<SpriteRenderer>();
            if (sr == null) continue;

            Vector3 charPos = sr.bounds.center;
            Vector3 camPos = cam.transform.position;
            Vector3 direction = camPos - charPos;
            float distance = direction.magnitude;

            if (distance <= Mathf.Epsilon) continue;
            direction.Normalize();

            bool hitEnvironment = false;

            RaycastHit[] hits = Physics.RaycastAll(charPos, direction, distance, environmentMask, QueryTriggerInteraction.Collide);

            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;

                SpriteRenderer hitSR = hit.collider.GetComponent<SpriteRenderer>() ?? hit.collider.GetComponentInParent<SpriteRenderer>();
                if (hitSR == null) continue;

                // fade the environment
                SetAlphaIfNeeded(hitSR, seeThroughAlpha);
                fadedSprites.Add(hitSR);
                if (!blockedRenderers.Contains(hitSR))
                    blockedRenderers.Add(hitSR);

                hitEnvironment = true;
            }

            // 🔧 record for gizmos
            debugRays.Add((charPos, camPos, hitEnvironment));
        }
    }

    private void ResetFadedSprites()
    {
        if (fadedSprites.Count == 0) return;
        foreach (var sr in fadedSprites)
        {
            if (sr != null)
                SetAlphaIfNeeded(sr, 1f);
        }
        fadedSprites.Clear();
    }

    private void SetAlphaIfNeeded(SpriteRenderer sr, float alpha)
    {
        if (sr == null) return;
        Color c = sr.color;
        if (!Mathf.Approximately(c.a, alpha))
        {
            c.a = alpha;
            sr.color = c;
        }
    }

    // 🔧 Gizmos visualization
    private void OnDrawGizmos()
    {
        if (debugRays == null) return;

        foreach (var (start, end, hit) in debugRays)
        {
            Gizmos.color = hit ? Color.red : Color.green;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(start, 0.05f);
        }
    }
    }
}

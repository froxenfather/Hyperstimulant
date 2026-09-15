using UnityEngine;

// Learning Objective: build the habit of visualizing invisible math,
// gated behind Inspector toggles so only what you need is drawn.
public class Exp14_VisualDebuggingLab : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] private bool showVelocity;
    [SerializeField] private bool showGroundRay;
    [SerializeField] private bool showSurfaceNormal;

    [Header("Settings")]
    [SerializeField] private float groundRayDistance = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (showVelocity)
        {
            // TODO: draw the current velocity vector.
            // Pseudocode:
            //   Debug.DrawRay(transform.position, rb.linearVelocity, Color.red);
        }

        if (showGroundRay || showSurfaceNormal)
        {
            // TODO: raycast down once, then draw the ray and/or the hit normal
            // depending on which toggles are enabled.
            // Pseudocode:
            //   bool hitSomething = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundRayDistance);
            //   if (showGroundRay && hitSomething) Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.yellow);
            //   if (showSurfaceNormal && hitSomething) Debug.DrawRay(hit.point, hit.normal * 2f, Color.green);
        }
    }
}

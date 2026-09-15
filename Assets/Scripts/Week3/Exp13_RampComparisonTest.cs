using UnityEngine;

// Learning Objective: bring raycasts, surface normals, slope angle, and projection
// together. Put one of these on (or above) each of Ramp_15 / Ramp_30 / Ramp_45.
public class Exp13_RampComparisonTest : MonoBehaviour
{
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private Vector3 testMovementDirection = Vector3.forward;
    [SerializeField] private float maxDistance = 5f;

    private void Update()
    {
        Vector3 origin = rayOrigin != null ? rayOrigin.position : transform.position;

        // TODO: raycast down onto this ramp, get its normal + slope angle,
        // and project a test movement vector onto its surface.
        // Pseudocode:
        //   if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, maxDistance))
        //   {
        //       float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        //       Vector3 projectedMovement = Vector3.ProjectOnPlane(testMovementDirection, hit.normal).normalized;
        //
        //       Debug.DrawRay(origin, Vector3.down * hit.distance, Color.yellow);
        //       Debug.DrawRay(hit.point, hit.normal * 2f, Color.green);
        //       Debug.DrawRay(hit.point, projectedMovement * 2f, Color.magenta);
        //       Debug.Log($"{name}: slope={slopeAngle:F1} deg");
        //   }
    }
}

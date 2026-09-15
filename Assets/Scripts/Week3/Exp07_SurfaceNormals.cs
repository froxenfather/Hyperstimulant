using UnityEngine;

// Learning Objective: a surface normal points perpendicular away from a surface.
// Place this on a tester above Ramp_15 / Ramp_30 / Ramp_45 and compare results.
public class Exp07_SurfaceNormals : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float normalDrawLength = 2f;

    private void Update()
    {
        // TODO: raycast downward onto the current surface and draw its normal.
        // Pseudocode:
        //   if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxDistance))
        //   {
        //       Debug.DrawRay(hit.point, hit.normal * normalDrawLength, Color.green);
        //       Debug.Log($"surface normal: {hit.normal}");
        //   }
    }
}

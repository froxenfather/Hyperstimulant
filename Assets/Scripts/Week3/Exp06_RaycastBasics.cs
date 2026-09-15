using UnityEngine;

// Learning Objective: understand how a raycast detects geometry
// (origin, direction, max distance, layer mask, RaycastHit).
public class Exp06_RaycastBasics : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private LayerMask hitMask = ~0; // ~0 = "everything"

    private void Update()
    {
        // TODO: cast a ray downward from this object and report what it hits.
        // Pseudocode:
        //   if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxDistance, hitMask))
        //   {
        //       Debug.Log($"Hit {hit.collider.name} at {hit.point}, distance {hit.distance}, normal {hit.normal}");
        //       Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.green);
        //   }
        //   else
        //   {
        //       Debug.DrawRay(transform.position, Vector3.down * maxDistance, Color.red);
        //   }
    }
}

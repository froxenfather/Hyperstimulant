using UnityEngine;

// Learning Objective: Vector3.ProjectOnPlane removes the portion of a vector
// that points into a surface, leaving a vector that lies along it.
public class Exp08_VectorProjection : MonoBehaviour
{
    [SerializeField] private Vector3 movementDirection = Vector3.forward;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float rayLength = 2f;

    private void Update()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxDistance))
            return;

        // TODO: project movementDirection onto the surface plane defined by hit.normal.
        // Pseudocode:
        //   Vector3 projectedDirection = Vector3.ProjectOnPlane(movementDirection, hit.normal);

        // TODO: draw the original movement vector, the surface normal, and the projected vector.
        // Pseudocode:
        //   Debug.DrawRay(transform.position, movementDirection * rayLength, Color.white);
        //   Debug.DrawRay(hit.point, hit.normal * rayLength, Color.green);
        //   Debug.DrawRay(transform.position, projectedDirection * rayLength, Color.magenta);
    }
}

using UnityEngine;

// Learning Objective: the cross product of two vectors returns a vector
// perpendicular to both — useful for finding a direction along a surface.
public class Exp05_CrossProduct : MonoBehaviour
{
    [SerializeField] private float rayLength = 3f;

    private void Update()
    {
        // TODO: compute a "right" direction using the cross product of world up and forward.
        // Pseudocode:
        //   Vector3 rightDirection = Vector3.Cross(Vector3.up, transform.forward);

        // TODO: draw forward, up, and the cross product result with different colors.
        // Pseudocode:
        //   Debug.DrawRay(transform.position, transform.forward * rayLength, Color.blue);
        //   Debug.DrawRay(transform.position, Vector3.up * rayLength, Color.green);
        //   Debug.DrawRay(transform.position, rightDirection * rayLength, Color.red);
    }
}

using UnityEngine;

// Learning Objective: the dot product of two normalized vectors measures alignment.
// 1 = same direction, 0 = perpendicular, -1 = opposite direction.
public class Exp04_DotProduct : MonoBehaviour
{
    [SerializeField] private Transform target;

    private enum Facing { InFront, ToTheSide, Behind }

    private void Update()
    {
        if (target == null) return;

        // TODO: compute the dot product between this object's forward and the
        // normalized direction to the target.
        // Pseudocode:
        //   - get the normalized direction from this object to the target
        //   - take the dot product of this object's forward vector and that
        //     normalized direction - result is a single float between -1 and 1
        //   - log the result
        Vector3 diffVector = target.position - transform.position;
        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 direction = diffVector.normalized;
        
        float dot = Vector3.Dot(transform.forward, direction);
        Debug.Log("DotProduct: " + dot);

        // TODO (Homework Task): classify the target using dot thresholds you choose.
        // Pseudocode:
        //   - pick two threshold values between -1 and 1 (you decide what feels right)
        //   - if the dot is above the high threshold, the target counts as in front
        //   - if it's below the low threshold, the target counts as behind
        //   - otherwise, it counts as to the side
        //   - log which category it landed in
    }
}

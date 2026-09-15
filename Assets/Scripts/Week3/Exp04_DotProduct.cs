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
        //   Vector3 directionToTarget = (target.position - transform.position).normalized;
        //   float dot = Vector3.Dot(transform.forward, directionToTarget);
        //   Debug.Log($"dot: {dot}");

        // TODO (Homework Task): classify the target using dot thresholds you choose.
        // Pseudocode:
        //   Facing facing;
        //   if (dot > /* your threshold */ 0.5f) facing = Facing.InFront;
        //   else if (dot < /* your threshold */ -0.5f) facing = Facing.Behind;
        //   else facing = Facing.ToTheSide;
        //   Debug.Log(facing);
    }
}

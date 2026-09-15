using UnityEngine;

// Learning Objective: understand what normalizing a vector does —
// direction keeps distance baked in, normalizedDirection has magnitude 1.
public class Exp03_VectorNormalization : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void Update()
    {
        if (target == null) return;

        // TODO: compute the direction to target, and its normalized form.
        // Pseudocode:
        //   Vector3 direction = target.position - transform.position;
        //   Vector3 normalizedDirection = direction.normalized;
        //   Debug.Log($"direction: {direction} (mag {direction.magnitude})  normalized: {normalizedDirection} (mag {normalizedDirection.magnitude})");

        // TODO: draw both vectors with different colors so the length difference is visible.
        // Pseudocode:
        //   Debug.DrawRay(transform.position, direction, Color.yellow);
        //   Debug.DrawRay(transform.position, normalizedDirection * 3f, Color.cyan);
    }
}

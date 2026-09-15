using UnityEngine;

// Learning Objective: a raycast is an infinitely thin line; a spherecast
// sweeps a volume through space and can catch edges a raycast would miss.
public class Exp10_SphereCast : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float sphereRadius = 0.4f;

    private void Update()
    {
        // TODO: cast both a raycast and a spherecast downward and compare results.
        // Pseudocode:
        //   bool rayHit = Physics.Raycast(transform.position, Vector3.down, out RaycastHit rHit, maxDistance);
        //   bool sphereHit = Physics.SphereCast(transform.position, sphereRadius, Vector3.down, out RaycastHit sHit, maxDistance);
        //   Debug.Log($"raycast hit: {rayHit}   spherecast hit: {sphereHit}");

        // TODO: visualize both casts.
        // Pseudocode:
        //   Debug.DrawRay(transform.position, Vector3.down * maxDistance, Color.red);
        //   // (spherecasts don't have a built-in debug draw; drawing the ray is enough to compare hit results in the Console)
    }
}

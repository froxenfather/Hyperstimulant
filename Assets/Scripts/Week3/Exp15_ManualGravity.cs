using UnityEngine;
using UnityEngine.InputSystem;

// Learning Objective: model gravity as acceleration applied directly to a
// Transform - no Rigidbody, no physics engine. You own every step of
// acceleration -> velocity -> position yourself (Euler integration).
// Requires: just a Transform (no Rigidbody, no Collider needed).
public class Exp15_ManualGravity : MonoBehaviour
{
    [SerializeField] private float gravity = 9.81f;

    private Vector3 velocity;
    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            transform.position = startPosition;
            velocity = Vector3.zero;
        }

        // TODO: integrate gravity into velocity, then velocity into position.
        // This is the same idea a Rigidbody does internally under the hood -
        // here you're doing it by hand, one frame at a time.
        // Pseudocode:
        //   - add a downward amount to velocity, proportional to gravity and how
        //     much time passed this frame (this is acceleration becoming velocity)
        //   - move the transform's position by velocity, scaled by how much time
        //     passed this frame (this is velocity becoming position)

        Debug.DrawRay(transform.position, velocity, Color.red);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

// Learning Objective: without a Rigidbody, nothing stops the object from
// falling through the floor - moving a Transform never triggers physics
// collision resolution. YOU have to detect the ground and cancel velocity.
// Requires: a Collider on the ground below (no Rigidbody on this object).
public class Exp16_ManualGroundCollision : MonoBehaviour
{
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float skinWidth = 0.05f; // small buffer above the surface to prevent jitter/penetration
    [SerializeField] private float groundCheckDistance = 0.2f;

    private Vector3 velocity;
    private Vector3 startPosition;
    private bool grounded;

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

        // TODO: apply gravity to velocity, same as Exp15.
        // Pseudocode:
        //   - add a downward amount to velocity, proportional to gravity and deltaTime

        // TODO: raycast downward. If the ground is within reach, snap the
        // object to the surface (plus skinWidth) and zero out downward
        // velocity instead of letting it pass through.
        // Pseudocode:
        //   - calculate where the object WOULD end up this frame if it just moved by
        //     velocity * deltaTime (don't apply it yet - check it first)
        //   - raycast straight down from the current position, out to groundCheckDistance
        //     plus skinWidth
        //   - if it hits: mark grounded true, override the calculated position's height to
        //     sit just above the hit point (using skinWidth as the buffer), and zero out
        //     the downward component of velocity so it stops accumulating
        //   - if it doesn't hit: mark grounded false and leave the calculated position alone
        //   - finally, apply whichever position you ended up with to the transform

        Debug.DrawRay(transform.position, Vector3.down * (groundCheckDistance + skinWidth), grounded ? Color.green : Color.red);
    }
}

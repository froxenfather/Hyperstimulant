using UnityEngine;
using UnityEngine.InputSystem;

// Learning Objective: revisit Exp01's Force vs Impulse idea, but with no
// Rigidbody. "Force" becomes acceleration added every frame it's held;
// "impulse" becomes a one-time jump added to velocity on the frame it's pressed.
// Requires: just a Transform.
public class Exp17_ManualForceAndImpulse : MonoBehaviour
{
    [SerializeField] private float upwardAcceleration = 15f;
    [SerializeField] private float jumpImpulseSpeed = 6f;
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

        // TODO: "continuous force" - held key adds acceleration * deltaTime every frame.
        // Pseudocode:
        //   - while the space key is held down, add an upward amount to velocity each
        //     frame, proportional to upwardAcceleration and deltaTime (it builds up the
        //     longer it's held)

        // TODO: "impulse" - one tap adds a fixed velocity change instantly, once.
        // Pseudocode:
        //   - on the single frame the G key is first pressed, add a fixed upward amount
        //     to velocity all at once, based on jumpImpulseSpeed (no deltaTime here -
        //     it's an instant change, not something that builds up)

        // TODO: gravity still needs to be integrated every frame, same as Exp15/16.
        // Pseudocode:
        //   - add a downward amount to velocity, proportional to gravity and deltaTime

        // TODO: apply the accumulated velocity to position.
        // Pseudocode:
        //   - move the transform's position by velocity, scaled by deltaTime

        Debug.DrawRay(transform.position, velocity, Color.red);
    }
}

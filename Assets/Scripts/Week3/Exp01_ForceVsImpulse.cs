using UnityEngine;
using UnityEngine.InputSystem;

// Learning Objective: understand the difference between a continuous Force,
// an instant Impulse, and directly setting velocity.
// Controls: Space = up force, F = forward force, G = forward impulse, R = reset.
public class Exp01_ForceVsImpulse : MonoBehaviour
{
    [Header("Force Settings")]
    [SerializeField] private float upwardForce = 10f;
    [SerializeField] private float forwardForce = 10f;
    [SerializeField] private float forwardImpulse = 5f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // TODO: apply an upward CONTINUOUS force (ForceMode.Force).
            // Pseudocode:
            //   rb.AddForce(Vector3.up * upwardForce, ForceMode.Force);
        }

        if (Keyboard.current.fKey.isPressed)
        {
            // TODO: apply a forward CONTINUOUS force, every frame it's held.
            // Pseudocode:
            //   rb.AddForce(transform.forward * forwardForce, ForceMode.Force);
        }

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            // TODO: apply a forward INSTANT impulse (ForceMode.Impulse).
            // Pseudocode:
            //   rb.AddForce(transform.forward * forwardImpulse, ForceMode.Impulse);
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetCube();
        }
    }

    private void ResetCube()
    {
        // TODO: reset position, rotation, and velocity so the test can repeat cleanly.
        // Pseudocode:
        //   transform.position = startPosition;
        //   transform.rotation = startRotation;
        //   rb.linearVelocity = Vector3.zero;
        //   rb.angularVelocity = Vector3.zero;
    }
}

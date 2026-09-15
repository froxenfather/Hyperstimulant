using UnityEngine;
using UnityEngine.InputSystem;

// Learning Objective: compare three ways of losing speed —
// 1 = Rigidbody linearDamping (set in Inspector, no code), 2 = opposing force, 3 = direct velocity change.
public class Exp12_DragAndDeceleration : MonoBehaviour
{
    [SerializeField] private float manualDecelForce = 10f;
    [SerializeField] private float directVelocityDecelRate = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Key 1: rely entirely on Rigidbody.linearDamping in the Inspector — no code needed.

        if (Keyboard.current.digit2Key.isPressed)
        {
            // TODO: apply a force opposing current velocity to slow down manually.
            // Pseudocode:
            //   Vector3 opposingForce = -rb.linearVelocity.normalized * manualDecelForce;
            //   rb.AddForce(opposingForce, ForceMode.Force);
        }

        if (Keyboard.current.digit3Key.isPressed)
        {
            // TODO: directly manipulate velocity toward zero (no forces involved).
            // Pseudocode:
            //   rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, Vector3.zero, directVelocityDecelRate * Time.fixedDeltaTime);
        }
    }
}

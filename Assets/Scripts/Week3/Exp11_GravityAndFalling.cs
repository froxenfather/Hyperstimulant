using UnityEngine;

// Learning Objective: gravity is acceleration — velocity changes continuously over time,
// it isn't an instantly-applied speed.
public class Exp11_GravityAndFalling : MonoBehaviour
{
    private Rigidbody rb;
    private float fallStartTime;
    private bool wasFalling;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // TODO: track vertical velocity and how long the object has been falling.
        // Pseudocode:
        //   float verticalVelocity = rb.linearVelocity.y;
        //   bool isFalling = verticalVelocity < -0.01f;
        //   if (isFalling && !wasFalling) fallStartTime = Time.time;
        //   if (isFalling) Debug.Log($"falling. vY={verticalVelocity}, elapsed={Time.time - fallStartTime}");
        //   wasFalling = isFalling;
    }
}

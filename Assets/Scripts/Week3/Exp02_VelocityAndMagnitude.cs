using UnityEngine;

// Learning Objective: velocity is a vector (direction + speed);
// magnitude collapses it down to a single speed number.
public class Exp02_VelocityAndMagnitude : MonoBehaviour
{
    [SerializeField] private float rayScale = 1f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // TODO: read the rigidbody's velocity and its magnitude, then log them.
        // Pseudocode:
        //   Vector3 velocity = rb.linearVelocity;
        //   float speed = velocity.magnitude;
        //   Debug.Log($"velocity: {velocity}  speed: {speed}");

        // TODO: draw the velocity vector as a ray from this object's position.
        // Pseudocode:
        //   Debug.DrawRay(transform.position, velocity * rayScale, Color.red);
    }
}

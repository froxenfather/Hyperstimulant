using UnityEngine;
using UnityEngine.InputSystem;

// Learning Objective: convert the velocity that would be lost hitting a ramp
// (the part driving INTO the surface) into extra flat speed down the ramp.
// Steeper ramp -> bigger boost. Head-on hit -> bigger boost than a glancing one.
// Requires: a ramp Collider below (no Rigidbody on this object). Drop it
// straight down, then try again from an angle (tilt startVelocity).
public class Exp07_5_SlopeSpeedBoost : MonoBehaviour
{
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private Vector3 startVelocity = Vector3.zero;
    [SerializeField] private float boostMultiplier = 1f; // "k" in the math
    [SerializeField] private AnimationCurve slopeBoostCurve = AnimationCurve.Linear(0f, 0f, 90f, 1f); // x = slope degrees, y = multiplier
    [SerializeField] private float skinWidth = 0.05f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    private Vector3 velocity;
    private Vector3 startPosition;
    private bool grounded;

    private void Awake()
    {
        startPosition = transform.position;
        velocity = startVelocity;
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            transform.position = startPosition;
            velocity = startVelocity;
            grounded = false;
        }

        // TODO: apply gravity to velocity and predict the next position (same as Exp16).
        // Pseudocode:
        //   - add a downward amount to velocity, proportional to gravity and deltaTime
        //   - calculate where the object would end up if it moved by velocity * deltaTime

        // TODO: raycast down. On the frame it hits, convert impact speed into flat boost.
        // Pseudocode:
        //   - raycast straight down out to groundCheckDistance plus skinWidth
        //   - if it hits:
        //       1. impact speed: take the dot product of velocity and the hit normal.
        //          It's negative when moving into the surface, so flip its sign
        //          (ignore the hit if it isn't moving into the surface at all)
        //       2. tangential velocity: subtract (dot result * normal) from velocity.
        //          This is the momentum already running along the ramp
        //       3. slope angle: the angle between the normal and world up, in degrees
        //       4. steepness factor: sample slopeBoostCurve at the slope angle
        //       5. flat downhill direction: project world down onto the surface plane,
        //          zero out its y, then normalize
        //       6. bonus speed = impact speed * steepness factor * boostMultiplier
        //       7. new velocity = the tangential velocity with its y zeroed, plus the
        //          flat downhill direction scaled by the bonus
        //       - snap the predicted position's height to just above the hit point
        //         (plus skinWidth), and mark grounded
        //       - log impact speed, slope angle, and bonus so you can compare ramps
        //   - if it doesn't hit: mark grounded false
        //   - apply the predicted position to the transform

        Debug.DrawRay(transform.position, velocity, grounded ? Color.cyan : Color.red);
    }
}

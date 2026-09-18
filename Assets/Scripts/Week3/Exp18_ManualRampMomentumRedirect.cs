using UnityEngine;
using UnityEngine.InputSystem;

// Learning Objective: this is Exp16's manual ground collision, but instead of
// just zeroing out downward velocity on impact, the FULL incoming velocity
// gets projected onto the surface plane - so a fast fall onto a steep ramp
// gets redirected down-slope instead of just stopping dead.
// Requires: a Collider on the ramp below (no Rigidbody on this object).
public class Exp18_ManualRampMomentumRedirect : MonoBehaviour
{
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float skinWidth = 0.05f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [Tooltip("If true, the redirected velocity keeps the same speed it had on impact. If false, it keeps whatever magnitude ProjectOnPlane naturally produces.")]
    [SerializeField] private bool preserveImpactSpeed = true;

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

        // TODO: apply gravity, same as Exp15/16.
        // Pseudocode:
        //   - add a downward amount to velocity, proportional to gravity and deltaTime

        // TODO: raycast downward. On hit, instead of just zeroing velocity.y,
        // project the WHOLE velocity vector onto the surface plane - this is
        // what redirects a steep fall into down-slope momentum. Compare
        // preserveImpactSpeed true vs false to see the difference.
        // Pseudocode:
        //   - calculate where the object would end up this frame if it moved by
        //     velocity * deltaTime (check before applying, same as Exp16)
        //   - raycast straight down out to groundCheckDistance plus skinWidth
        //   - if it hits:
        //       - mark grounded true, and set the calculated position's height to sit
        //         just above the hit point (plus skinWidth)
        //       - record the velocity's magnitude before you change it - that's the
        //         impact speed
        //       - project the FULL velocity vector onto the surface plane defined by
        //         the hit normal (not just the y component this time)
        //       - if preserveImpactSpeed is on, rescale that projected vector back up
        //         to the recorded impact speed; otherwise just use the projected
        //         vector as-is
        //       - find the angle between the hit normal and world up for the slope
        //         angle, and log impact speed, slope angle, and the resulting speed
        //         so you can compare steep vs shallow ramps
        //   - if it doesn't hit: mark grounded false and leave the calculated
        //     position alone
        //   - apply whichever position you ended up with to the transform

        Debug.DrawRay(transform.position, velocity, grounded ? Color.cyan : Color.red);
    }
}

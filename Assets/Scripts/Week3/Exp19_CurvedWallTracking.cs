using UnityEngine;
using UnityEngine.InputSystem;

// Learning Objective: follow a curved wall (cylinder) using only the hit normal.
// A non-Rigidbody object travels along the wall's tangent, re-casting toward the
// wall every frame. Toggle the stick force to see why straight-line motion drifts
// off a curve, and toggle speed preservation to see reprojection slow you down.
// Requires: a vertical Cylinder with a Collider. Start this object near it,
// at torso height, a bit away from the surface. No Rigidbody on this object.
public class Exp19_CurvedWallTracking : MonoBehaviour
{
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float castRadius = 0.3f;
    [SerializeField] private float castDistance = 2f;
    [SerializeField] private float wallOffset = 0.5f; // how far from the surface to hover
    [SerializeField] private float stickStrength = 1f; // scales the inward pull
    [SerializeField] private bool useStickForce = true;
    [SerializeField] private bool preserveSpeed = true;

    private Vector3 velocity;
    private Vector3 lastNormal;
    private bool attached;
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
            attached = false;
        }

        // Attach: press Space to cast toward the wall. Set your start facing so
        // transform.forward points at the cylinder. Detach: press E.
        if (!attached && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // TODO: find the wall to attach to.
            // Pseudocode:
            //   - spherecast from this object's position along transform.forward,
            //     using castRadius and castDistance
            //   - if it hits: store the hit normal as lastNormal and mark attached
            //   - pick the initial run direction: cross of world up and lastNormal,
            //     then flip it if needed so it points the way this object is
            //     roughly facing (hint: dot it against transform.forward)
            //   - set velocity to that direction scaled by runSpeed
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            attached = false;
            velocity = Vector3.zero;
        }

        if (!attached) return;

        // TODO: re-cast toward the wall every frame to get the CURRENT normal.
        // Pseudocode:
        //   - spherecast from the current position along the NEGATIVE of lastNormal
        //     (that is: into the wall), using castRadius and castDistance
        //   - if nothing is hit: mark detached and stop this frame's update
        //   - if it hits: overwrite lastNormal with the new hit normal
        //   - also verify the wall is roughly vertical (angle between the normal and
        //     world up close to 90 degrees) - detach if it isn't

        // TODO: keep the run direction along the wall as the normal rotates.
        // Pseudocode:
        //   - remember the current speed (velocity magnitude) BEFORE changing velocity
        //   - project velocity onto the plane defined by the new normal
        //   - normalize the projected result to get a pure direction
        //   - if preserveSpeed is on, scale it back up to the remembered speed;
        //     if it's off, scale by whatever length the projection left you with
        //   - assign that back to velocity

        // TODO: optional stick force that keeps you on the curve.
        // Pseudocode:
        //   - if useStickForce is on: add an inward pull to velocity along the
        //     negative normal, proportional to speed squared and stickStrength,
        //     multiplied by delta time (tuning tip: start small, since larger
        //     speeds make speed-squared grow fast)
        //   - if it is off, skip this and watch what happens on a tight cylinder

        // TODO: move, and hold the correct distance from the wall.
        // Pseudocode:
        //   - move the transform's position by velocity scaled by delta time
        //   - optionally correct the position so its distance from the surface stays
        //     at wallOffset (use the hit distance from the recast to work out how far
        //     to nudge along the normal)

        Debug.DrawRay(transform.position, lastNormal * 2f, Color.green);   // wall normal
        Debug.DrawRay(transform.position, velocity, Color.red);            // velocity
        Debug.DrawRay(transform.position, Vector3.Cross(Vector3.up, lastNormal) * 2f, Color.cyan); // tangent
    }
}

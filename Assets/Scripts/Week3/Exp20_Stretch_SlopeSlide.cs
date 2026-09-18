using UnityEngine;
using UnityEngine.InputSystem;

// STRETCH EXPERIMENT (do only after the core experiments are done).
// Learning Objective: on a ramp, gravity's pull ALONG the surface is g * sin(slope).
// Steeper ramp -> faster acceleration down; sliding up a ramp slows you down.
// Requires: a ramp Collider below (no Rigidbody on this object). Place it on
// Ramp_15 / Ramp_30 / Ramp_45 and compare. This is NOT the final slide system.
public class Exp20_Stretch_SlopeSlide : MonoBehaviour
{
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float skinWidth = 0.05f;
    [SerializeField] private float groundCheckDistance = 0.3f;

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

        // TODO: raycast down to find the ramp under this object.
        // Pseudocode:
        //   - raycast straight down out to groundCheckDistance plus skinWidth
        //   - if nothing is hit, leave this experiment for now (return)

        // TODO: find the downhill direction and the slope angle.
        // Pseudocode:
        //   - downhill direction: project world down onto the plane defined by the hit
        //     normal, then normalize it
        //   - slope angle: the angle between the hit normal and world up, in degrees
        //   - if the slope angle is basically zero (flat ground), there's no downhill,
        //     so skip the acceleration step

        // TODO: accelerate along the surface.
        // Pseudocode:
        //   - acceleration amount = gravity * sine of the slope angle
        //     (convert degrees to radians before taking the sine)
        //   - add the downhill direction scaled by (acceleration amount * deltaTime)
        //     to velocity. The sign takes care of itself: if velocity points uphill,
        //     this slows it; if downhill, it speeds it up

        // TODO: keep velocity ON the surface, then move.
        // Pseudocode:
        //   - remove any component of velocity pointing into the surface
        //     (project velocity onto the surface plane, same as Exp8)
        //   - move the transform's position by velocity * deltaTime
        //   - snap the height so it sits just above the hit point (plus skinWidth),
        //     same idea as Exp16
        //   - log the slope angle and current speed so you can compare ramps

        Debug.DrawRay(transform.position, velocity, Color.cyan);
    }
}

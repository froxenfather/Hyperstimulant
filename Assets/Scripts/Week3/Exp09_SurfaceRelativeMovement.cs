using UnityEngine;
using UnityEngine.InputSystem;

// Learning Objective: combine input + surface normal to move along angled geometry
// (flat ground, 15/30/45 degree ramps) without the projected vector losing speed.
public class Exp09_SurfaceRelativeMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float maxDistance = 5f;

    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) moveInput.y += 1f;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1f;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1f;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1f;
    }

    private void FixedUpdate()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxDistance))
            return;

        Vector3 rawDirection =
            transform.forward * moveInput.y +
            transform.right * moveInput.x;

        Debug.DrawRay(hit.point, hit.normal * 2f, Color.green);

        // TODO: project rawDirection onto the surface, normalize it, then set velocity.
        // Pseudocode:
        //   Vector3 surfaceDirection = Vector3.ProjectOnPlane(rawDirection, hit.normal).normalized;
        //   rb.linearVelocity = surfaceDirection * moveSpeed;
    }
}

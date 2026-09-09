using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpVelocity = 5f;
    [SerializeField] private float extraGravity = 9.81f;
    [SerializeField] private float airControl = 2f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private BoxCollider boxCollider;

    private Vector2 moveInput;
    private bool jumpPressed;
    private bool grounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        CheckGround();
        Move();
        ApplyExtraGravity();
        Jump();
    }

    private void ReadInput()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            moveInput.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            moveInput.y -= 1f;

        if (Keyboard.current.dKey.isPressed)
            moveInput.x += 1f;

        if (Keyboard.current.aKey.isPressed)
            moveInput.x -= 1f;

        if (Keyboard.current.spaceKey.isPressed)
            jumpPressed = true;

        moveInput = moveInput.normalized;
    }

    private void CheckGround()
    {
        float rayDistance = boxCollider.bounds.extents.y + 0.1f;

        grounded = Physics.Raycast(
            boxCollider.bounds.center,
            Vector3.down,
            rayDistance
        );
    }

    private void Move()
    {
        Vector3 moveDirection =
            transform.forward * moveInput.y +
            transform.right * moveInput.x;

        if (grounded)
        {
            // grounded: we have full authority over horizontal velocity
            Vector3 targetVelocity = moveDirection * moveSpeed;

            rb.linearVelocity = new Vector3(
                targetVelocity.x,
                rb.linearVelocity.y,
                targetVelocity.z
            );
        }
        else
        {
            // airborne: preserve launch / jump momentum, allow only light steering
            rb.AddForce(moveDirection * airControl, ForceMode.Acceleration);
        }
    }

    private void Jump()
    {
        if (jumpPressed && grounded)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                jumpVelocity,
                rb.linearVelocity.z
            );
        }

        jumpPressed = false;
    }

    private void ApplyExtraGravity()
    {
        if (!grounded)
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
    }
}
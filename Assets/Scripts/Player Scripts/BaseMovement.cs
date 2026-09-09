using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpVelocity = 5f;
    [SerializeField] private float groundCheckDistance = 1.1f;

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
        float distanceToFeet = boxCollider.bounds.extents.y + 0.1f;

        grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            distanceToFeet
        );
    }

    private void Move()
    {
        Vector3 moveDirection =
            transform.forward * moveInput.y +
            transform.right * moveInput.x;

        Vector3 targetVelocity = moveDirection * moveSpeed;

        rb.linearVelocity = new Vector3(
            targetVelocity.x,
            rb.linearVelocity.y,
            targetVelocity.z
        );
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
}
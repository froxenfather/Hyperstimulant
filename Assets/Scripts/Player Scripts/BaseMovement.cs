using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpVelocity = 5f;
    [SerializeField] private float extraGravity = 9.81f;
    [SerializeField] private float airControl = 2f;
    [SerializeField] private float maxAirSpeed = 8f;

    private Rigidbody rb;

    private Vector2 moveInput;
    private bool jumpPressed;
    private bool grounded;

    private Rigidbody supportBody;
    private Vector3 supportVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
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

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpPressed = true;

        moveInput = moveInput.normalized;
    }

    private void Move()
    {
        Vector3 moveDirection =
            transform.forward * moveInput.y +
            transform.right * moveInput.x;

        if (grounded)
        {
            Vector3 playerVelocity = moveDirection * moveSpeed;

            Vector3 finalVelocity =
                playerVelocity +
                supportVelocity;

            rb.linearVelocity = new Vector3(
                finalVelocity.x,
                rb.linearVelocity.y,
                finalVelocity.z
            );
        }
        else
        {
            rb.AddForce(
                moveDirection * airControl,
                ForceMode.Acceleration
            );

            LimitAirSpeed();
        }
    }

    private void LimitAirSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        if (horizontalVelocity.magnitude > maxAirSpeed)
        {
            horizontalVelocity =
                horizontalVelocity.normalized * maxAirSpeed;

            rb.linearVelocity = new Vector3(
                horizontalVelocity.x,
                rb.linearVelocity.y,
                horizontalVelocity.z
            );
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

            grounded = false;
            supportBody = null;
            supportVelocity = Vector3.zero;
        }

        jumpPressed = false;
    }

    private void ApplyExtraGravity()
    {
        if (!grounded)
        {
            rb.AddForce(
                Vector3.down * extraGravity,
                ForceMode.Acceleration
            );
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        grounded = false;
        supportBody = null;
        supportVelocity = Vector3.zero;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                grounded = true;

                supportBody = collision.rigidbody;

                if (supportBody != null)
                {
                    supportVelocity = supportBody.GetPointVelocity(contact.point);
                }

                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody == supportBody)
        {
            supportBody = null;
            supportVelocity = Vector3.zero;
            grounded = false;
        }
    }
}
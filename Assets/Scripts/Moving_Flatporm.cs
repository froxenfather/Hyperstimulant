using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float leftDistance = 5f;
    [SerializeField] private float upDistance = 5f;
    [SerializeField] private float rightDistance = 5f;
    [SerializeField] private float downDistance = 5f;
    [SerializeField] private float phaseTime = 5f;

    private Rigidbody rb;

    // current phase in the movement cycle
    // 0 = left
    // 1 = pause
    // 2 = up
    // 3 = pause
    // 4 = right
    // 5 = pause
    // 6 = down
    // 7 = pause
    private int phase = 0;
    private float timer;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        startPosition = rb.position;
        SetTarget();
    }

    private void FixedUpdate()
    {
        // Increase the timer using Unity's physics timestep
        timer += Time.fixedDeltaTime;

        // Only move during movement phases
        if (IsMovingPhase())
            MovePlatform();

        // When the phase time ends, move to the next phase
        if (timer >= phaseTime)
            NextPhase();
    }

    private bool IsMovingPhase()
    {
        // Even-numbered phases move
        // Odd-numbered phases pause
        return phase % 2 == 0;
    }

    private void NextPhase()
    {
        // Move to the next phase and loop back to 0 after phase 7
        phase = (phase + 1) % 8;

        // Reset the phase timer
        timer = 0f;

        // Set a new destination if the next phase involves movement
        SetTarget();
    }

    private void SetTarget()
    {
        // Pause phases do not need a new target
        if (!IsMovingPhase())
            return;

        // Save where this movement phase begins
        startPosition = rb.position;

        // Choose the new target based on the current phase
        targetPosition = phase switch
        {
            0 => startPosition + Vector3.left * leftDistance,
            2 => startPosition + Vector3.up * upDistance,
            4 => startPosition + Vector3.right * rightDistance,
            6 => startPosition + Vector3.down * downDistance,
            _ => startPosition
        };
    }

    private void MovePlatform()
    {
        // Calculate how far this phase needs to travel
        float distance = Vector3.Distance(
            startPosition,
            targetPosition
        );

        // Calculate the speed needed to finish exactly within phaseTime
        float speed = distance / phaseTime;

        // Calculate the next position for this physics frame
        Vector3 nextPosition = Vector3.MoveTowards(
            rb.position,
            targetPosition,
            speed * Time.fixedDeltaTime
        );

        // Move the kinematic Rigidbody through Unity's physics system
        rb.MovePosition(nextPosition);
    }
}
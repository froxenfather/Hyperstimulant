using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float leftDistance = 5f;
    [SerializeField] private float upDistance = 5f;
    [SerializeField] private float rightDistance = 5f;
    [SerializeField] private float downDistance = 5f;

    [SerializeField] private float phaseTime = 5f;

    private Vector3 targetPosition;
    private int direction = 1;
    private float nextDirectionChange;

    private void Start()
    {
        targetPosition = transform.position + Vector3.left * leftDistance;
        nextDirectionChange = phaseTime;
    }

    private void Update()
    {
        Move();

        if (Time.time >= nextDirectionChange)
        {
            UpdateDirection();
            nextDirectionChange += phaseTime;
        }
    }

    private void UpdateDirection()
    {
        direction++;

        if (direction > 7)
            direction = 0;

        if (direction == 1)
            targetPosition = transform.position + Vector3.left * leftDistance;

        if (direction == 3)
            targetPosition = transform.position + Vector3.up * upDistance;

        if (direction == 5)
            targetPosition = transform.position + Vector3.right * rightDistance;

        if (direction == 7)
            targetPosition = transform.position + Vector3.down * downDistance;
    }

    private void Move()
    {
        if (direction % 2 == 0)
            return;

        float distance;

        if (direction == 1)
            distance = leftDistance;
        else if (direction == 3)
            distance = upDistance;
        else if (direction == 5)
            distance = rightDistance;
        else
            distance = downDistance;

        float speed = distance / phaseTime;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }
}
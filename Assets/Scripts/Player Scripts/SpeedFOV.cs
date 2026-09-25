using UnityEngine;

// Widens the camera FOV as the player speeds up. Put it on the Camera.
[RequireComponent(typeof(Camera))]
public class SpeedFOV : MonoBehaviour
{
    [SerializeField] private CustomPlayerController player;

    [Header("Speed range")]
    [Range(0f, 60f)]
    [SerializeField] private float startSpeed = 12f;   // at or below this, no FOV change (walk speed)

    [Range(0f, 60f)]
    [SerializeField] private float fullSpeed = 20f;    // max speed the FOV reacts to, at or above this = full boost (sprint speed)

    [Header("FOV")]
    [Range(0f, 40f)]
    [SerializeField] private float maxFovBoost = 12f;

    [Range(1f, 200f)]
    [SerializeField] private float fovChangeSpeed = 60f;   // degrees per second

    private Camera cam;
    private float baseFov;

    private void Start()
    {
        cam = GetComponent<Camera>();
        baseFov = cam.fieldOfView;
    }

    // Camera work runs per frame, not in FixedUpdate.
    private void Update()
    {
        // 0 at walk speed, 1 at sprint speed, clamped so launch pads don't blow the FOV out.
        float speedFraction = Mathf.InverseLerp(startSpeed, fullSpeed, player.HorizontalSpeed);

        float targetFov = baseFov + maxFovBoost * speedFraction;

        cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, targetFov, fovChangeSpeed * Time.deltaTime);
    }
}

using IObjects;
using UnityEngine;

public class LaunchBehavior : MonoBehaviour, IPlayerContact
{
    [SerializeField] private float launchForward = 5f;
    [SerializeField] private float launchUp = 5f;

    void Start()
    {
        Debug.Log("Im a fat fucking launch pad");
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        if (rb == null)
            return;

        bool isPlayer = rb.GetComponent<PlayerMovement>() != null;

        Vector3 launchDirection = transform.up * launchUp;

        if (isPlayer)
            launchDirection += transform.forward * launchForward;

        rb.linearVelocity = launchDirection;
    }

    // The custom controller has no dynamic Rigidbody, so it reports contact itself instead of OnCollisionEnter.
    public void OnPlayerContact(CustomPlayerController player)
    {
        player.SetVelocity(transform.up * launchUp + transform.forward * launchForward);
    }
}

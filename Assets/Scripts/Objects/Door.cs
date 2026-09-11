using IObjects;
using UnityEngine;

public class Door : MonoBehaviour, IPowered
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float rotationSpeed = 90f;

    private bool isOpen;

    private void Update()
    {
        float targetAngle = isOpen ? openAngle : 0f;

        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void SwitchOn()
    {
        isOpen = true;
    }

    public void SwitchOff()
    {
        isOpen = false;
    }
}

using IObjects;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private MonoBehaviour target;

    private IPowered poweredObject;

    private void Awake()
    {
        poweredObject = target as IPowered;
    }

    private void OnTriggerEnter(Collider other)
    {
        poweredObject.SwitchOn();
    }

    private void OnTriggerExit(Collider other)
    {
        poweredObject.SwitchOff();
        
    }
}
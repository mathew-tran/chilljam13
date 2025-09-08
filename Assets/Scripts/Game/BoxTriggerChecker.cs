using UnityEngine;
using UnityEngine.Events;

public class BoxTriggerChecker : MonoBehaviour
{
    public UnityEvent<GameObject> OnBoxEntered;

    private void OnTriggerEnter(Collider other)
    {
        Box boxComponent = other.gameObject.GetComponent<Box>();
        if (boxComponent)
        {
            OnBoxEntered?.Invoke(other.gameObject);
        }
    }
}

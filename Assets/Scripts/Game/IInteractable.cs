using UnityEngine;

public abstract class IInteractable : MonoBehaviour
{
    public abstract string GetInteractableName();

    public abstract bool CanInteract();

    public abstract void Interact(Player playerRef);

    public abstract void Drop();

    public float DistanceAwayFromPlayer = 1.0f;


}

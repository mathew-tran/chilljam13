using UnityEngine;
using UnityEngine.Events;

public class Box : IInteractable
{

    [SerializeField]
    private BoxData Data;

    private bool bCanInteract = true;

    public UnityEvent OnBoxDeath;
    public override bool CanInteract()
    {
        return bCanInteract;
    }

    public override void Drop()
    {
        bCanInteract = true;
    }

    public string GetBoxName()
    {
        return Data.BoxName;
    }

    public override string GetInteractableName()
    {
        return GetBoxName();
    }

    public override void Interact(Player playerRef)
    {
        bCanInteract = false;
        playerRef.PickupItem(gameObject);

    }

    private void OnDestroy()
    {
        OnBoxDeath?.Invoke();
    }

}

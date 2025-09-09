using UnityEngine;

public class Box : IInteractable
{

    [SerializeField]
    private BoxData Data;

    private bool bCanInteract = true;

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

}

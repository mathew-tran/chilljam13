using UnityEngine;

public class Box : IInteractable
{

    [SerializeField]
    private string BoxName = "";

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
        return BoxName;
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

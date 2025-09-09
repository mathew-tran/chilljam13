using UnityEngine;
using UnityEngine.Events;

public class TriggerButton : IInteractable
{
    public string InteractableText;

    public UnityEvent OnTriggerPressed;
    public override bool CanInteract()
    {
        return true;
    }

    public override void Drop()
    {
        
    }

    public override string GetInteractableName()
    {
       return InteractableText;
    }

    public override void Interact(Player playerRef)
    {
        OnTriggerPressed?.Invoke();
        Debug.Log("Trigger Pressed");
    }

}

using UnityEngine;

public class Box : IInteractable
{

    [SerializeField]
    private string BoxName = "";

    public string GetBoxName()
    {
        return BoxName;
    }

    public override string GetInteractableName()
    {
        return GetBoxName();
    }
}

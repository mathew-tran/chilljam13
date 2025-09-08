using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableLabel : MonoBehaviour
{
    [SerializeField]

    private TMP_Text Text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        OnInteractChange(null);

        Player.GetInstance().OnInteractChange += OnInteractChange;
    }
    private void OnDestroy()
    {
        Player.GetInstance().OnInteractChange -= OnInteractChange;
    }

    private void OnInteractChange(IInteractable interactable)
    {
        if (interactable == null)
        {
            Text.text = "";
        }
        else
        {
            Text.text = interactable.GetInteractableName();
        }
    }

  
}

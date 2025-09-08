using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    public InputSystem_Actions InputActions;

    private InputAction MoveAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        MoveAction = InputActions.Player.Move;
        MoveAction.performed += MoveAction_performed;
    }
    private void OnEnable()
    {
        MoveAction.Enable();
    }
    private void OnDisable()
    {
        MoveAction.Disable();
    }
    void Start()
    {
        
    }

    private void MoveAction_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Performed");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

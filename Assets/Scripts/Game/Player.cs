using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    [SerializeField]
    public InputSystem_Actions InputActions;

    private InputAction MoveAction;

    private InputAction LookAction;

    [SerializeField]
    private CharacterController Controller;

    [SerializeField]
    private Camera CameraRef;

    [SerializeField]
    private float MoveSpeed = 5;

    private float Gravity = 9.8f;

    private Vector3 PlayerVelocity;

    [SerializeField]
    private float ySensitivity = .01f;

    private float Pitch = 0.0f;

    [SerializeField]
    private LayerMask InteractableLayer;

    private IInteractable CurrentInteractable;

    public Action<IInteractable> OnInteractChange;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static Player mInstance;

    public static Player GetInstance()
    {
        return mInstance;
    }


    public IInteractable GetInteractable()
    {
        return CurrentInteractable;
    }
    private void Awake()
    {
        if (mInstance != null)
        {
            Debug.Log("Player::AWAKE More than one player!");
        }
        mInstance = this;

        InputActions = new InputSystem_Actions();
        MoveAction = InputActions.Player.Move;
        LookAction = InputActions.Player.Look;
    }
    private void OnEnable()
    {
        MoveAction.Enable();
        LookAction.Enable();

        InputActions.Enable();
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;                   
    }
    private void OnDisable()
    {
        MoveAction.Disable();
        LookAction.Disable();

        InputActions.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        Move(MoveAction.ReadValue<Vector2>());
        Look(LookAction.ReadValue<Vector2>());
        DetectObjects();
    }

    private void DetectObjects()
    {
        RaycastHit hit;
        Ray ray = CameraRef.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        Debug.DrawRay(transform.position, CameraRef.transform.forward, Color.rebeccaPurple);
        if (Physics.Raycast(ray, out hit, 30.0f, InteractableLayer))
        {
            Debug.Log("HIT" + hit.collider.name);
            CurrentInteractable = hit.collider.gameObject.GetComponent<IInteractable>();


        }
        else
        {
            CurrentInteractable = null;
        }
        OnInteractChange?.Invoke(CurrentInteractable);
    }

    private void Look(Vector2 value)
    {
        float yaw = value.x * ySensitivity;
        float pitchDelta = -value.y * ySensitivity;

        Pitch = Mathf.Clamp(Pitch + pitchDelta, -45f, 45f);

        CameraRef.transform.localRotation = Quaternion.Euler(Pitch, 0f, 0f);
        transform.Rotate(Vector3.up, yaw);
    }

    private void Move(Vector2 vector)
    {
        bool bIsGrounded = Controller.isGrounded;

        var moveVector = new Vector3(vector.x, 0, vector.y);
        moveVector = transform.TransformDirection(moveVector);

       

        if (bIsGrounded == false && PlayerVelocity.y > 0)
        {
            PlayerVelocity.y = Gravity;
        }
        else
        {
            PlayerVelocity.y = 0;
        }

        PlayerVelocity.y -= Gravity;

        Vector3 finalMove = moveVector * MoveSpeed + PlayerVelocity.y * Vector3.up;
        Controller.Move(finalMove * Time.deltaTime);


    }
}

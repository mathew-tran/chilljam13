using System;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    public InputSystem_Actions InputActions;

    private InputAction MoveAction;

    private InputAction LookAction;

    private InputAction InteractAction;

    private InputAction ShootAction;

    private InputAction JumpAction;

    private InputAction SprintAction;

    private InputAction RotateAction;

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

    private float ObjectPitch = 0.0f;
    private float ObjectYaw = 0.0f;
    private float ObjectOffset = 0.0f;
    private Vector3 LocalObjectStartPosition;

    private float Pitch = 0.0f;

    [SerializeField]
    private LayerMask InteractableLayer;

    [SerializeField]
    private LayerMask NonInteractableLayer;

    private IInteractable CurrentInteractable;

    public Action<IInteractable> OnInteractChange;

    public Action OnThrowStart;
    public Action OnThrowEnd;

    public static Player mInstance;

    public GameObject ItemHolder;
    private GameObject HeldItem;



    private float ThrowPower = 1.0f;

    [SerializeField]
    private float MaxThrowPower = 500.0f;

    private bool bIsThrowing = false;


    [SerializeField]
    private float JumpStrength = 100.0f;


    
    private float RunningModifier = 0.0f;

    [SerializeField]
    private float MaxRunningModifier = 2.5f;
    private bool bIsRunning = false;

    private enum MODE
    {
        MOVEMENT_MODE,
        OBJECT_MODE
    }

    private MODE CurrentMode = MODE.MOVEMENT_MODE;

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

        InteractAction = InputActions.Player.Interact;
        InteractAction.performed += OnInteractPerformed;

        ShootAction = InputActions.Player.Shoot;
        ShootAction.canceled += OnShootCancelled;
        ShootAction.started += OnShootStarted;

        JumpAction = InputActions.Player.Jump;
        JumpAction.performed += OnJumpPerformed;

        SprintAction = InputActions.Player.Sprint;
        SprintAction.started += OnSprintStarted;
        SprintAction.canceled += OnSprintCancelled;

        RotateAction = InputActions.Player.Rotate;
        RotateAction.started += OnRotateStarted;
        RotateAction.canceled += OnRotateEnded;
    }

    private void OnRotateEnded(InputAction.CallbackContext context)
    {
        CurrentMode = MODE.MOVEMENT_MODE;
    }

    private void OnRotateStarted(InputAction.CallbackContext context)
    {
        if (HeldItem)
        {
            CurrentMode = MODE.OBJECT_MODE;
        }
    }

    private void OnSprintCancelled(InputAction.CallbackContext context)
    {
        bIsRunning = false;
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        bIsRunning = true;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (Controller.isGrounded)
        {

            PlayerVelocity.y = Mathf.Sqrt(JumpStrength  * Gravity);
            Debug.Log("JUMP");
        }
        
    }

    public float GetThrowStrengthPercentage()
    {
        float result = ThrowPower / MaxThrowPower;

        return result;
    }
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Attempt Interact");
        if (CurrentInteractable == null)
        {
            if (HeldItem != null)
            {
                ReleaseHeldItem();
            }
            return;
        }
        if (CurrentInteractable.CanInteract())
        {
            CurrentInteractable.Interact(this);
        }
    }

    private void OnShootCancelled(InputAction.CallbackContext context)
    {
        if (HeldItem)
        {
            GameObject obj = HeldItem;
            ReleaseHeldItem();

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if(rb)
            {
                rb.AddForce(CameraRef.transform.forward * ThrowPower);
            }
            Debug.Log("Threw " + obj.name + " with force of " + ThrowPower);
            bIsThrowing = false;
            OnThrowEnd?.Invoke();
        }

    }

    private void OnShootStarted(InputAction.CallbackContext context)
    {
        if (HeldItem)
        {
            bIsThrowing = true;
            ThrowPower = 0.0f;
            OnThrowStart?.Invoke();
        }
    }

    public void PickupItem(GameObject obj)
    {
        if (HeldItem)
        {
            ReleaseHeldItem();
            return;
        }

        EquipHeldItem(obj);
       
    }

    private void EquipHeldItem(GameObject obj)
    {
        Debug.Log("Equipping item: " + obj.name);
        HeldItem = obj;
        HeldItem.transform.SetParent(ItemHolder.transform);

        Rigidbody rb = HeldItem.GetComponent<Rigidbody>();
        HeldItem.gameObject.layer = LayerMask.NameToLayer("Non-Interactable");
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            LocalObjectStartPosition = HeldItem.transform.localPosition;
            ObjectOffset = obj.GetComponent<IInteractable>().DistanceAwayFromPlayer;
        }

    }
    private void ReleaseHeldItem()
    {
        Debug.Log("Releasing item: " + HeldItem.name);
        HeldItem.transform.parent = null;
        Rigidbody rb = HeldItem.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            ObjectPitch = 0.0f;

        }
        HeldItem.gameObject.layer = LayerMask.NameToLayer("Interactable");
        HeldItem.GetComponent<IInteractable>().Drop();
        HeldItem = null;
        OnThrowEnd?.Invoke();
    }

    private void OnEnable()
    {
        MoveAction.Enable();
        LookAction.Enable();
        ShootAction.Enable();
        JumpAction.Enable();
        InteractAction.Enable();
        SprintAction.Enable();
        RotateAction.Enable();

        InputActions.Enable();
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;                   
    }
    private void OnDisable()
    {
        MoveAction.Disable();
        LookAction.Disable();
        ShootAction.Disable();
        JumpAction.Disable();
        InteractAction.Disable();
        SprintAction.Disable();
        RotateAction.Disable();

        InputActions.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void DoMoveModeControls()
    {
        Move(MoveAction.ReadValue<Vector2>());
        Look(LookAction.ReadValue<Vector2>());
        DetectObjects();

        if (bIsThrowing)
        {
            ThrowPower += Time.deltaTime * 800.0f;
            if (ThrowPower > MaxThrowPower)
            {
                ThrowPower = MaxThrowPower;
            }
        }
        if (HeldItem)
        {
            Rigidbody rb = HeldItem.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void DoObjectModeControls()
    {
        if (HeldItem)
        {
            RotateObject(LookAction.ReadValue<Vector2>());
            MoveObject(MoveAction.ReadValue<Vector2>());
        }
        else
        {
            CurrentMode = MODE.MOVEMENT_MODE;
        }
    }
    private void Update()
    {
        switch(CurrentMode)
        {
            case MODE.MOVEMENT_MODE:
                DoMoveModeControls();
                break;
            case MODE.OBJECT_MODE:
                DoObjectModeControls();
                break;
        }
      


        CameraRef.fieldOfView = Mathf.Lerp(60, 80, GetMaxSpeedPercent());
    }

    private float GetMaxSpeedPercent()
    {
        return (MoveSpeed + RunningModifier) / (MoveSpeed + MaxRunningModifier);
    }
    private void DetectObjects()
    {
        RaycastHit hit;
        Ray ray = CameraRef.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        Debug.DrawRay(transform.position, CameraRef.transform.forward, Color.rebeccaPurple);
        if (Physics.Raycast(ray, out hit, 3.0f, InteractableLayer))
        {
            CurrentInteractable = hit.collider.gameObject.GetComponent<IInteractable>();

        }
        else
        {
            CurrentInteractable = null;
        }
        OnInteractChange?.Invoke(CurrentInteractable);
    }

    private void MoveObject(Vector2 value)
    {
        ObjectOffset += value.y;
        ObjectOffset = Mathf.Clamp(ObjectOffset, 1f, 3f);
    }
    private void RotateObject(Vector2 value)
    {
        float yaw = value.x * ySensitivity;
        float yawDelta = -value.y * ySensitivity;

        float pitch = value.y * ySensitivity;
        float pitchDelta = -value.x * ySensitivity;



        ObjectPitch = Mathf.Clamp(ObjectPitch + yawDelta, -90f, 90f);

        ObjectYaw = Mathf.Clamp(ObjectYaw + pitchDelta, -1800f, 180f);

        HeldItem.transform.localRotation = Quaternion.Euler(ObjectPitch, ObjectYaw, 0f);

        HeldItem.transform.localPosition = Vector3.MoveTowards(HeldItem.transform.localPosition, LocalObjectStartPosition, Time.deltaTime);
        HeldItem.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }


    private void Look(Vector2 value)
    {
        float yaw = value.x * ySensitivity;
        float pitchDelta = -value.y * ySensitivity;

        Pitch = Mathf.Clamp(Pitch + pitchDelta, -90f, 45f);

        CameraRef.transform.localRotation = Quaternion.Euler(Pitch, 0f, 0f);
        transform.Rotate(Vector3.up, yaw);
    }

    private void Move(Vector2 vector)
    {
        bool bIsGrounded = Controller.isGrounded;

        if (bIsRunning == false || vector == Vector2.zero)
        {
            RunningModifier -= 10.5f * Time.deltaTime;
            if (RunningModifier <= 0f)
            {
                RunningModifier = 0f;
            }
        }
        else if (bIsRunning && vector != Vector2.zero)
        {
            RunningModifier += Time.deltaTime * 2.25f;
            if (RunningModifier > MaxRunningModifier)
            {
                RunningModifier = MaxRunningModifier;
            }
        }

        var moveVector = new Vector3(vector.x, 0, vector.y);
        moveVector = transform.TransformDirection(moveVector);

        if (bIsGrounded && PlayerVelocity.y < 0)
        {
            PlayerVelocity.y = -2f;
        }

        PlayerVelocity.y -= Gravity * Time.deltaTime;

        Vector3 finalMove = moveVector * GetCurrentMoveSpeed() + PlayerVelocity.y * Vector3.up;
        Controller.Move(finalMove * Time.deltaTime);


    }

    public float GetCurrentMoveSpeed()
    {
        return MoveSpeed + RunningModifier;
    }
}

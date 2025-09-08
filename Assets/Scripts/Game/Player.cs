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

    private InputAction InteractAction;

    private InputAction ShootAction;

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

    [SerializeField]
    private LayerMask NonInteractableLayer;

    private IInteractable CurrentInteractable;

    public Action<IInteractable> OnInteractChange;

    public Action OnThrowStart;
    public Action OnThrowEnd;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static Player mInstance;

    public GameObject ItemHolder;
    private GameObject HeldItem;



    private float ThrowPower = 1.0f;

    [SerializeField]
    private float MaxThrowPower = 500.0f;

    private bool bIsThrowing = false;

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
            rb.isKinematic = true;
        }
        HeldItem.transform.position = ItemHolder.transform.position;

    }
    private void ReleaseHeldItem()
    {
        Debug.Log("Releasing item: " + HeldItem.name);
        HeldItem.transform.parent = null;
        Rigidbody rb = HeldItem.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            
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
        InteractAction.Enable();

        InputActions.Enable();
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;                   
    }
    private void OnDisable()
    {
        MoveAction.Disable();
        LookAction.Disable();
        InteractAction.Disable();

        InputActions.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        Move(MoveAction.ReadValue<Vector2>());
        Look(LookAction.ReadValue<Vector2>());
        DetectObjects();

        if(bIsThrowing)
        {
            ThrowPower += Time.deltaTime * 500.0f;
            if (ThrowPower > MaxThrowPower)
            {
                ThrowPower = MaxThrowPower;
            }
        }
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

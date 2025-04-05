using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // 2D up view character controller.
    // Has collisions
    [SerializeField] private float      speed = 2f;
    [SerializeField] private float      sprintMult = 2.5f;
    private Rigidbody2D                 rb;
    private InputSystem_Actions         inputActions;
    private Vector2                     movementInput;
    private bool                        isSprinting = false;

    //================================//
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
    }

    //================================//
    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;
        inputActions.Player.Disable();
    }

    //================================//
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    //================================//
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //================================//
    private void FixedUpdate()
    {
        rb.linearVelocity = movementInput.normalized * speed;
    }

    //================================//
    private void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    //================================//
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
    }

    //================================//
    private void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprinting = true;
            speed *= sprintMult;
        }
        else if (context.canceled)
        {
            isSprinting = false;
            speed /= sprintMult;
        }
    }
}

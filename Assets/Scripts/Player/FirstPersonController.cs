using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerHealth))]
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float jumpHeight = 1.2f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;

    private CharacterController characterController;
    private InputActionMap playerActionMap;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float verticalRotation;
    private float currentHorizontalSpeed;
    private bool isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();

        var asset = inputActions != null ? inputActions : Resources.Load<InputActionAsset>("InputSystem_Actions");
        playerActionMap = asset.FindActionMap("Player");
        moveAction = playerActionMap.FindAction("Move");
        lookAction = playerActionMap.FindAction("Look");
        jumpAction = playerActionMap.FindAction("Jump");
        sprintAction = playerActionMap.FindAction("Sprint");
    }

    private void OnEnable()
    {
        playerActionMap?.Enable();
    }

    private void OnDisable()
    {
        playerActionMap?.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        UpdateGrounded();
        HandleInput();
        HandleMovement();
        HandleRotation();
    }

    private void UpdateGrounded()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
    }

    private void HandleInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 direction = transform.TransformDirection(input);
        float targetSpeed = (sprintAction != null && sprintAction.IsPressed()) ? runSpeed : walkSpeed;

        if (direction.sqrMagnitude > 0.001f)
        {
            currentHorizontalSpeed = Mathf.MoveTowards(
                currentHorizontalSpeed, targetSpeed, acceleration * Time.deltaTime);
            direction = direction.normalized * currentHorizontalSpeed;
        }
        else
        {
            currentHorizontalSpeed = Mathf.MoveTowards(
                currentHorizontalSpeed, 0f, acceleration * Time.deltaTime);
            direction = Vector3.zero;
        }

        if (!isGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        else if (jumpAction.triggered)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }

        direction.y = velocity.y;
        characterController.Move(direction * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

        if (playerCamera != null)
            playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        characterController.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        characterController.enabled = true;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8.5f;
    [SerializeField] private float jumpHeight = 2.2f;
    [SerializeField] private float gravity = -20f;

    [Header("Look")]
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float pitchLimit = 80f;
    [SerializeField] private Transform playerCamera;

    private CharacterController controller;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private Vector3 velocity;
    private float pitch;
    private float yaw;
    private bool inputInitialized;

    public void SetCameraTransform(Transform cameraTransform)
    {
        playerCamera = cameraTransform;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
        {
            playerCamera = Camera.main != null ? Camera.main.transform : null;
        }

        InitializeInput();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!inputInitialized)
        {
            return;
        }

        HandleLook();
        HandleMovement();
    }

    private void InitializeInput()
    {
        var actions = InputSystem.actions;
        if (actions == null)
        {
            Debug.LogError("Input System actions are not assigned. Set the project-wide actions asset to InputSystem_Actions.");
            enabled = false;
            return;
        }

        var playerMap = actions.FindActionMap("Player");
        if (playerMap == null)
        {
            Debug.LogError("Player action map was not found in the current Input Actions asset.");
            enabled = false;
            return;
        }

        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");
        jumpAction = playerMap.FindAction("Jump");
        sprintAction = playerMap.FindAction("Sprint");

        if (moveAction == null || lookAction == null || jumpAction == null || sprintAction == null)
        {
            Debug.LogError("One or more required Player actions are missing from the Input Actions asset.");
            enabled = false;
            return;
        }

        actions.Enable();
        inputInitialized = true;
    }

    private void HandleLook()
    {
        var lookInput = lookAction.ReadValue<Vector2>();
        if (lookInput.sqrMagnitude <= 0.0001f || playerCamera == null)
        {
            return;
        }

        yaw += lookInput.x * lookSensitivity;
        pitch -= lookInput.y * lookSensitivity;
        pitch = Mathf.Clamp(pitch, -pitchLimit, pitchLimit);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        var moveInput = moveAction.ReadValue<Vector2>();
        float currentSpeed = sprintAction.IsPressed() ? sprintSpeed : walkSpeed;

        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        moveDirection.y = 0f;

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        if (jumpAction.triggered && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

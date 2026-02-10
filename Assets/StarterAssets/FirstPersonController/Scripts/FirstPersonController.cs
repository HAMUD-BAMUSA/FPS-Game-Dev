using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;            // Normal walking speed
    public float sprintSpeed = 8f;          // Sprint speed
    public float crouchSpeed = 2.5f;        // Movement speed while crouching
    public float slideSpeed = 10f;          // Speed during a slide
    public float airControl = 0.5f;         // Multiplier for moving in air

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.5f;         // Jump height
    public float gravity = -9.81f;          // Gravity
    private float verticalVelocity;         // Y velocity

    [Header("Crouch & Slide")]
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode slideKey = KeyCode.LeftControl;
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSlideSmooth = 8f;    // Smooth interpolation speed

    [Header("Mouse & Camera")]
    public Transform cameraTarget;          // Assign your main camera here
    public float lookSpeed = 2f;            // Mouse sensitivity
    public float topClamp = 90f;            // Max upward angle
    public float bottomClamp = -90f;        // Max downward angle

    private CharacterController controller;
    private Vector3 moveInput;
    private float currentSpeed;
    private float targetSpeed;
    private bool isCrouching = false;
    private bool isSliding = false;
    private float cameraPitch = 0f;
    private Vector3 originalCameraPos;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTarget == null)
        {
            Debug.LogWarning("CameraTarget not assigned! Please assign your main camera.");
        }
        else
        {
            originalCameraPos = cameraTarget.localPosition;
        }

        
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleCrouchSlide();
        HandleGravityJump();
    }

    // ---------------------------
    // MOUSE LOOK
    // ---------------------------
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        // Vertical rotation (pitch)
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, bottomClamp, topClamp);

        if (cameraTarget != null)
        {
            cameraTarget.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }

        // Horizontal rotation (yaw)
        transform.Rotate(Vector3.up * mouseX);
    }

    // ---------------------------
    // PLAYER MOVEMENT
    // ---------------------------
    private void HandleMovement()
    {
        // Read WASD/Arrow keys
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(inputX, 0f, inputZ).normalized;

        // Determine target speed
        if (isSliding)
            targetSpeed = slideSpeed;
        else if (isCrouching)
            targetSpeed = crouchSpeed;
        else if (Input.GetKey(KeyCode.LeftShift))
            targetSpeed = sprintSpeed;
        else
            targetSpeed = walkSpeed;

        // Smooth acceleration/deceleration
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed * inputDir.magnitude, Time.deltaTime * 10f);

        // Calculate movement direction relative to player
        Vector3 move = transform.right * inputDir.x + transform.forward * inputDir.z;

        // Apply air control if not grounded
        if (!controller.isGrounded)
            move *= airControl;

        // Final movement including vertical velocity
        Vector3 velocity = move * currentSpeed + Vector3.up * verticalVelocity;

        // Move character
        if (controller.enabled)
            controller.Move(velocity * Time.deltaTime);
    }

    // ---------------------------
    // CROUCH & SLIDE
    // ---------------------------
    private void HandleCrouchSlide()
    {
        // Toggle crouch
        if (Input.GetKeyDown(crouchKey))
            isCrouching = !isCrouching;

        // Start slide if key pressed while moving and grounded
        if (Input.GetKeyDown(slideKey) && controller.isGrounded)
            isSliding = true;

        // Stop slide if key released
        if (isSliding && !Input.GetKey(slideKey))
            isSliding = false;

        // Set target height
        float targetHeight = standingHeight;
        if (isCrouching || isSliding)
            targetHeight = crouchHeight;

        // Smoothly adjust height and center
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchSlideSmooth);
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        // Adjust camera
        if (cameraTarget != null)
        {
            Vector3 camPos = cameraTarget.localPosition;
            float camTargetY = originalCameraPos.y - (standingHeight - controller.height);
            camPos.y = Mathf.Lerp(camPos.y, camTargetY, Time.deltaTime * crouchSlideSmooth);
            cameraTarget.localPosition = camPos;
        }
    }

    // ---------------------------
    // GRAVITY & JUMP
    // ---------------------------
    private void HandleGravityJump()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f; // small downward force to stay grounded

            // Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            // Apply gravity
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void OnEnable()
{
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}

    private void OnDisable()
{
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}

}

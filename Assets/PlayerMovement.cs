using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool sprintInput;
    
    [Header("Components")]
    [SerializeField] private CinemachineCamera fpCamera;
    private CharacterController charController;

    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float jumpHeight;
    float currentSpeed => sprintInput ? sprintSpeed : speed;
    bool isSprinting { get { return sprintInput && charController.velocity.sqrMagnitude > 0.1f; } }
    
    [Header("Physics Parameters")]
    [SerializeField] private float gravity;
    [SerializeField] private float characterWeight;
    private float verticalVelocity;
    private Vector3 currentVelocity;
    
    [Header("Look Parameters")]
    [SerializeField] private float lookSensitivity;
    [SerializeField] private float pitchLimit;
    float currentPitch;

    [Header("Camera Parameters")]
    [SerializeField] private float camSprintFOVIncrease;
    [SerializeField] private float sprintFOVSmooth;
    private float camNormalFOV;
    float camTargetFOV { get { return isSprinting ? camNormalFOV + camSprintFOVIncrease : camNormalFOV; } }

    void Start()
    {
        if (charController == null) charController = GetComponent<CharacterController>();
        camNormalFOV = fpCamera.Lens.FieldOfView;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MoveUpdate();
        LookUpdate();
        CameraUpdate();
    }

    void MoveUpdate()
    {
        Vector3 move = fpCamera.transform.forward * moveInput.y + fpCamera.transform.right * moveInput.x;
        move.y = 0;
        move.Normalize();

        if (move.sqrMagnitude >= 0.01f)
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, move * currentSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, acceleration * Time.deltaTime);
        }

        if (charController.isGrounded && verticalVelocity <= 0.01f)
        {
            verticalVelocity = -3f;
        }
        
        verticalVelocity += gravity * characterWeight * Time.deltaTime;
        Vector3 velocity = new Vector3(currentVelocity.x, verticalVelocity, currentVelocity.z);

        charController.Move(velocity * Time.deltaTime);
    }

    public void TryJump()
    {
        if (charController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(Mathf.Abs(jumpHeight * gravity * characterWeight));
        }
    }

    void LookUpdate()
    {
        Vector2 input = new Vector2(lookInput.x * lookSensitivity, lookInput.y * lookSensitivity);
        currentPitch = Mathf.Clamp(currentPitch - input.y, -pitchLimit, pitchLimit);
        fpCamera.transform.localRotation = Quaternion.Euler(currentPitch, 0, 0);
        transform.Rotate(Vector3.up * input.x);
    }

    void CameraUpdate()
    {
        fpCamera.Lens.FieldOfView = Mathf.Lerp(fpCamera.Lens.FieldOfView, camTargetFOV, sprintFOVSmooth * Time.deltaTime);
    }

}

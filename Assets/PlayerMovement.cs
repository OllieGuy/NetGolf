using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CinemachineCamera fpCamera;
    [SerializeField] private CharacterController charController;
    [SerializeField] private PlayerController pc;

    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float jumpHeight;
    float currentSpeed => pc.sprintInput ? sprintSpeed : speed;
    bool isSprinting { get { return pc.sprintInput && charController.velocity.sqrMagnitude > 0.1f; } }
    
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
        //if (pc == null) pc = GetComponent<PlayerController>();
        //if (charController == null) charController = GetComponent<CharacterController>();
        //if (fpCamera == null) fpCamera = GetComponentInChildren<CinemachineCamera>();
        camNormalFOV = fpCamera.Lens.FieldOfView;

        //Cursor.visible = false;
    }

    void Update()
    {
        MoveUpdate();
        JumpUpdate();
        LookUpdate();
        CameraUpdate();
    }

    void MoveUpdate()
    {
        Vector3 move = fpCamera.transform.forward * pc.moveInput.y + fpCamera.transform.right * pc.moveInput.x;
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
        Debug.Log(velocity);
        charController.Move(velocity * Time.deltaTime);
    }

    public void JumpUpdate()
    {
        if (charController.isGrounded && pc.jumpInput)
        {
            verticalVelocity = Mathf.Sqrt(Mathf.Abs(jumpHeight * gravity * characterWeight));
            pc.jumpInput = false;
        }
    }

    void LookUpdate()
    {
        Vector2 input = new Vector2(pc.lookInput.x * lookSensitivity, pc.lookInput.y * lookSensitivity);
        currentPitch = Mathf.Clamp(currentPitch - input.y, -pitchLimit, pitchLimit);
        fpCamera.transform.localRotation = Quaternion.Euler(currentPitch, 0, 0);
        transform.Rotate(Vector3.up * input.x);
    }

    void CameraUpdate()
    {
        fpCamera.Lens.FieldOfView = Mathf.Lerp(fpCamera.Lens.FieldOfView, camTargetFOV, sprintFOVSmooth * Time.deltaTime);
    }

}

using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(CharacterController))]
public class PlayerBaseMovement : PlayerBaseState
{
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

    [Header("Camera Parameters")]
    [SerializeField] private float camSprintFOVIncrease;
    [SerializeField] private float sprintFOVSmooth;
    float camTargetFOV { get { return isSprinting ? camNormalFOV + camSprintFOVIncrease : camNormalFOV; } }

    public override void StartState() { }
    public override void UpdateState()
    {
        MoveUpdate();
        JumpUpdate();
        LookUpdate(); 
        InputUpdate();
        CameraUpdate();
    }
    public override void ExitState() { }

    protected void MoveUpdate()
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
        charController.Move(velocity * Time.deltaTime);
    }

    protected void JumpUpdate()
    {
        if (charController.isGrounded && pc.jumpInput)
        {
            verticalVelocity = Mathf.Sqrt(Mathf.Abs(jumpHeight * gravity * characterWeight));
            playerAnimator.SetTrigger("Jump");
        }
    }

    protected void LookUpdate()
    {
        Vector2 input = new Vector2(pc.lookInput.x * lookSensitivity, pc.lookInput.y * lookSensitivity);
        currentPitch = Mathf.Clamp(currentPitch - input.y, -pitchLimit, pitchLimit);
        fpCamera.transform.localRotation = Quaternion.Euler(currentPitch, 0, 0);
        transform.Rotate(Vector3.up * input.x);
    }
    
    protected void InputUpdate()
    {
        if (pc.interactInput)
        {
            RaycastHit hit;

            Vector3 origin = fpCamera.transform.position;
            Vector3 direction = fpCamera.transform.forward;
            Vector3 halfExtents = new Vector3(1f, 1f, 1f);
            float maxDistance = 5f;

            int ballLayerMask = 1 << LayerMask.NameToLayer("Ball");

            bool hasHit = Physics.BoxCast(
                origin,
                halfExtents,
                direction,
                out hit,
                fpCamera.transform.rotation,
                maxDistance,
                ballLayerMask
            );

            if (hasHit && hit.collider.CompareTag("Ball"))
            {
                BallNetworked networkBall = hit.collider.GetComponent<BallNetworked>();
                if (networkBall != null && networkBall.IsOwner)
                {
                    PlayerBaseState state = GetState(PlayerStates.AimBall);
                    PlayerAimBall aimState = (PlayerAimBall)state;
                    aimState.SetBall(hit.collider.gameObject);
                    ChangeState(PlayerStates.AimBall);
                }
            }
        }
        else if (pc.attack2Input)
        {
            ChangeState(PlayerStates.PlaceBall);
        }
        
    }

    protected void CameraUpdate()
    {
        fpCamera.Lens.FieldOfView = Mathf.Lerp(fpCamera.Lens.FieldOfView, camTargetFOV, sprintFOVSmooth * Time.deltaTime);
    }

}

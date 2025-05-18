using System.Collections.Generic;
using System.Globalization;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerState : NetworkBehaviour
{
    [Header("State References")]
    [SerializeField] PlayerBaseMovement playerBaseMovement;
    [SerializeField] PlayerPlaceBall playerPlaceBall;
    [SerializeField] PlayerAimBall playerAimBall;
    [SerializeField] PlayerRagdoll playerRagdoll;

    [Header("Components")]
    [SerializeField] Animator playerAnimator;
    [SerializeField] SkinnedMeshRenderer playerMesh;
    [SerializeField] CinemachineCamera fpCamera;
    [SerializeField] CharacterController charController;
    [SerializeField] PlayerController pc;

    [Header("Camera Parameters")]
    [System.NonSerialized] public float cameraPitch;
    [System.NonSerialized] public float camNormalFOV;
    public float lookSensitivity;
    public float pitchLimit;

    private PlayerBaseState currentState;
    private Dictionary<PlayerStates, PlayerBaseState> stateRefs;

    void Awake()
    {
        stateRefs = new Dictionary<PlayerStates, PlayerBaseState>
        {
            { PlayerStates.BaseMovement, playerBaseMovement },
            { PlayerStates.PlaceBall, playerPlaceBall },
            { PlayerStates.AimBall, playerAimBall },
            { PlayerStates.Ragdoll, playerRagdoll }
        };

        foreach (PlayerBaseState state in stateRefs.Values)
        {
            state.SetValues(playerAnimator, playerMesh, fpCamera, charController, pc, gameObject, this);
        }

        ChangeState(PlayerStates.BaseMovement);
    }

    void Update()
    {
        currentState?.UpdateState();
        playerAnimator.SetFloat("PlayerSpeed", currentState.currentVelocity.magnitude);
        playerAnimator.SetBool("Grounded", charController.isGrounded);
    }

    public void ChangeState(PlayerStates newState)
    {
        currentState?.ExitState();
        currentState = stateRefs[newState];
        currentState.StartState();
    }

    [Rpc(SendTo.Server)]
    public void TriggerRagdollServerRpc(Vector3 force, Vector3 hitPoint)
    {
        TriggerRagdollClientRpc(force, hitPoint);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerRagdollClientRpc(Vector3 force, Vector3 hitPoint)
    {
        PlayerRagdoll ragdollState = (PlayerRagdoll)GetState(PlayerStates.Ragdoll);
        ragdollState.SetHitValues(force, hitPoint);
        ChangeState(PlayerStates.Ragdoll);
    }

    public PlayerBaseState GetState(PlayerStates newState)
    {
        return stateRefs[newState];
    }
}

public abstract class PlayerBaseState : MonoBehaviour
{
    protected Animator playerAnimator;
    protected SkinnedMeshRenderer playerMesh;
    protected CinemachineCamera fpCamera;
    protected CharacterController charController;
    protected PlayerController pc;
    protected GameObject playerObject;
    protected PlayerState stateMachine;
    protected float currentPitch
    {
        get => stateMachine.cameraPitch;
        set => stateMachine.cameraPitch = value;
    }

    protected float camNormalFOV
    {
        get => stateMachine.camNormalFOV;
        set => stateMachine.camNormalFOV = value;
    }

    protected float lookSensitivity
    {
        get => stateMachine.lookSensitivity;
    }
    
    protected float pitchLimit
    {
        get => stateMachine.pitchLimit;
    }

    public Vector3 currentVelocity;

    public void SetValues(Animator anim, SkinnedMeshRenderer mesh, CinemachineCamera cam, CharacterController controller, PlayerController input, GameObject obj, PlayerState _stateMachine)
    {
        stateMachine = _stateMachine;

        playerAnimator = anim;
        playerMesh = mesh;
        fpCamera = cam;
        camNormalFOV = fpCamera.Lens.FieldOfView;
        charController = controller;
        pc = input;
        playerObject = obj;
    }

    public abstract void StartState();
    public abstract void UpdateState();
    public abstract void ExitState();
    protected void ChangeState(PlayerStates newState) => stateMachine.ChangeState(newState);
    protected PlayerBaseState GetState(PlayerStates newState) => stateMachine.GetState(newState);
}

public enum PlayerStates
{
    BaseMovement,
    PlaceBall,
    AimBall,
    Ragdoll
}

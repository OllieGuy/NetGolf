using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [Header("State References")]
    [SerializeField] PlayerBaseMovement playerBaseMovement;
    [SerializeField] PlayerPlaceBall playerPlaceBall;
    [SerializeField] PlayerAimBall playerAimBall;

    [Header("Components")]
    [SerializeField] CinemachineCamera fpCamera;
    [SerializeField] CharacterController charController;
    [SerializeField] PlayerController pc;

    private PlayerBaseState currentState;
    private Dictionary<PlayerStates, PlayerBaseState> stateRefs;

    void Awake()
    {
        stateRefs = new Dictionary<PlayerStates, PlayerBaseState>
        {
            { PlayerStates.BaseMovement, playerBaseMovement },
            { PlayerStates.PlaceBall, playerPlaceBall },
            { PlayerStates.AimBall, playerAimBall }
        };

        foreach (PlayerBaseState state in stateRefs.Values)
        {
            state.SetValues(fpCamera, charController, pc, gameObject, this);
        }

        ChangeState(PlayerStates.BaseMovement);
    }

    void Update()
    {
        currentState?.UpdateState();
    }

    public void ChangeState(PlayerStates newState)
    {
        currentState?.ExitState();
        currentState = stateRefs[newState];
        currentState.StartState();
    }
}

public abstract class PlayerBaseState : MonoBehaviour
{
    protected CinemachineCamera fpCamera;
    protected CharacterController charController;
    protected PlayerController pc;
    protected GameObject playerObject;
    protected PlayerState stateMachine;
    protected float currentPitch;

    public void SetValues(CinemachineCamera cam, CharacterController controller, PlayerController input, GameObject obj, PlayerState _stateMachine)
    {
        fpCamera = cam;
        charController = controller;
        pc = input;
        playerObject = obj;
        stateMachine = _stateMachine;
    }

    public abstract void StartState();
    public abstract void UpdateState();
    public abstract void ExitState();
    protected void ChangeState(PlayerStates newState) => stateMachine.ChangeState(newState);
}

public enum PlayerStates
{
    BaseMovement,
    PlaceBall,
    AimBall
}

using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    PlayerState currentState;
    PlayerBaseMovement playerBaseMovement;
    PlayerPlaceBall playerPlaceBall;
    PlayerAimBall playerAimBall;

    [Header("Components")]
    protected CinemachineCamera fpCamera;
    protected CharacterController charController;
    protected PlayerController pc;

    Dictionary<PlayerStates, PlayerState> stateRefs;

    void Awake()
    {
        playerBaseMovement = GetComponent<PlayerBaseMovement>();
        playerPlaceBall = GetComponent<PlayerPlaceBall>();
        playerAimBall = GetComponent<PlayerAimBall>();

        fpCamera = GetComponentInChildren<CinemachineCamera>();
        charController = GetComponent<CharacterController>();
        pc = GetComponent<PlayerController>();

        currentState = playerBaseMovement;
        stateRefs = new Dictionary<PlayerStates, PlayerState>()
        {
            { PlayerStates.BaseMovement, playerBaseMovement },
            { PlayerStates.PlaceBall, playerPlaceBall },
            { PlayerStates.AimBall, playerAimBall },
        };
    }

    void Update()
    {
        if (pc.interactInput)
        {
            Debug.Log("int");
            ChangeState(PlayerStates.PlaceBall);
            pc.interactInput = false;
        }
        currentState.UpdateState();
    }

    public void ChangeState(PlayerStates newState)
    {
        currentState.ExitState();
        currentState = stateRefs[newState];
        currentState.StartState();
    }

    public virtual void StartState() { }
    public virtual void UpdateState() { }
    public virtual void ExitState() { }
}

public enum PlayerStates
{
    BaseMovement,
    PlaceBall,
    AimBall
}

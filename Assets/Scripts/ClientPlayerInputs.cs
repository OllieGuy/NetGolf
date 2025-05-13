using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientPlayerInputs : NetworkBehaviour
{
    private PlayerInput inputs;
    private PlayerController playerController;
    private PlayerState playerState;
    private CinemachineCamera playerCamera;

    void Awake()
    {
        inputs = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        playerState = GetComponent<PlayerState>();
        playerCamera = GetComponentInChildren<CinemachineCamera>();

        inputs.enabled = false;
        playerController.enabled = false;
        playerState.enabled = false;
        playerCamera.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            inputs.enabled = true;
            playerController.enabled = true;
            playerCamera.enabled = true;
            playerState.enabled = true;
        }
    }

    //[Rpc(target:SendTo.Server)]
    //private void UpdateInputServerRpc(Vector2 move, Vector2 look, bool sprint, bool jump)
    //{
    //    playerController.MoveInput(move);
    //    playerController.LookInput(look);
    //    playerController.SprintInput(sprint);
    //    playerController.JumpInput(jump);
    //}

    //private void LateUpdate()
    //{
    //    if (!IsOwner) 
    //        return;

    //    UpdateInputServerRpc(playerController.moveInput, playerController.lookInput, playerController.sprintInput, playerController.jumpInput);
    //}
}

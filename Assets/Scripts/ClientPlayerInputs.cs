using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientPlayerInputs : NetworkBehaviour
{
    private PlayerInput inputs;
    private PlayerController playerController;
    private PlayerBaseState playerState;
    private CinemachineCamera playerCamera;

    void Awake()
    {
        inputs = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        playerState = GetComponent<PlayerBaseState>();
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
}

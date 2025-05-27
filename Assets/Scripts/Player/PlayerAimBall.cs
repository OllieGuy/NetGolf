using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class PlayerAimBall : PlayerBaseState
{
    [SerializeField] private float horizontalAimSensitivity = 20f;
    [SerializeField] private float verticalAimSensitivity = 2f;
    [SerializeField] private float powerAimSensitivity = 5f;
    [SerializeField] private float maxPower = 30f;
    [SerializeField] private float defaultAimPower = 5f;
    [SerializeField] private float defaultYAim = 1f;

    private GameObject ballGameObj;
    private Rigidbody ballRb;
    private BallAimPreview ballAp;
    private float aimPower;
    private float yAim;

    BallNetworked networkBall;

    public void SetBall(GameObject ball)
    {
        aimPower = defaultAimPower;
        yAim = defaultYAim;

        ballGameObj = ball;
        ballRb = ballGameObj.GetComponent<Rigidbody>();
        ballRb.isKinematic = false;
        networkBall = ballGameObj.GetComponent<BallNetworked>();
        ballAp = networkBall.BallAimPreview;
        ballAp.gameObject.SetActive(true);
        ballAp.Initialise(maxPower);
        Vector3 startDirection = fpCamera.transform.forward;
        startDirection.y = 0;
        startDirection.Normalize();
        networkBall.Stopball();
        ballGameObj.transform.rotation = Quaternion.LookRotation(startDirection);
    }

    public override void StartState()
    {
        if (ballGameObj == null || ballRb == null)
        {
            ChangeState(PlayerStates.BaseMovement);
            return;
        }
        charController.enabled = false;
    }

    public override void UpdateState()
    {
        LookUpdate();
        AimUpdate();
        PowerUpdate();
        ShowPreview();

        if (pc.attackInput)
        {
            HitBall();
            ChangeState(PlayerStates.BaseMovement);
        }
    }

    public override void ExitState()
    {
        charController.enabled = true;
        networkBall = null;
    }

    private void LookUpdate()
    {
        Vector2 input = new Vector2(pc.lookInput.x * lookSensitivity, pc.lookInput.y * lookSensitivity);
        currentPitch = Mathf.Clamp(currentPitch - input.y, -pitchLimit, pitchLimit);
        fpCamera.transform.localRotation = Quaternion.Euler(currentPitch, 0, 0);
        transform.Rotate(Vector3.up * input.x);
    }

    private void AimUpdate()
    {
        if (networkBall != null && networkBall.IsOwner)
        {
            networkBall.RotateBallServerRpc(Vector3.up * pc.moveInput.x * horizontalAimSensitivity * Time.deltaTime);
        }
        yAim += pc.moveInput.y * verticalAimSensitivity * Time.deltaTime;

        Vector3 aim = new Vector3(ballGameObj.transform.forward.x, yAim, ballGameObj.transform.forward.z);
        aim.Normalize();
        Debug.DrawRay(ballGameObj.transform.position, aim * aimPower, Color.green);
        
    }
    
    private void PowerUpdate()
    {
        if (pc.sprintInput)
            aimPower = Mathf.Min(maxPower, aimPower + powerAimSensitivity * Time.deltaTime);
        if (pc.crouchInput)
            aimPower = Mathf.Max(0f, aimPower - powerAimSensitivity * Time.deltaTime);
    }

    private void ShowPreview()
    {
        ballAp.UpdatePreview(ballGameObj.transform.position, new Vector3(ballGameObj.transform.forward.x, yAim, ballGameObj.transform.forward.z), aimPower);
    }

    private void HitBall()
    {
        Vector3 direction = new Vector3(ballGameObj.transform.forward.x, yAim, ballGameObj.transform.forward.z);

        if (networkBall != null && networkBall.IsOwner)
        {
            networkBall.HitBallServerRpc(direction, aimPower);
            ballAp.gameObject.SetActive(false);
        }
    }
}

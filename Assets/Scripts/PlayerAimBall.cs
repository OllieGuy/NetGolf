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

    private GameObject currentBall;
    private Rigidbody ballRb;
    private BallAimPreview ballAp;
    private float aimPower;
    private float yAim;

    public void SetBall(GameObject ball)
    {
        aimPower = defaultAimPower;
        yAim = defaultYAim;

        currentBall = ball;
        ballRb = currentBall.GetComponent<Rigidbody>();
        ballRb.isKinematic = false;
        ballAp = currentBall.GetComponentInChildren<BallAimPreview>();
        ballAp.gameObject.SetActive(true);
        ballAp.Initialise(maxPower);
        Vector3 startDirection = fpCamera.transform.forward;
        startDirection.y = 0;
        startDirection.Normalize();
        currentBall.transform.rotation = Quaternion.LookRotation(startDirection);
    }

    public override void StartState()
    {
        if (currentBall == null || ballRb == null)
        {
            Debug.LogError("No ball");
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
        currentBall.transform.Rotate(Vector3.up * pc.moveInput.x * horizontalAimSensitivity * Time.deltaTime);
        yAim += pc.moveInput.y * verticalAimSensitivity * Time.deltaTime;

        Vector3 aim = new Vector3(currentBall.transform.forward.x, yAim, currentBall.transform.forward.z);
        aim.Normalize();
        Debug.DrawRay(currentBall.transform.position, aim * aimPower, Color.green);
        
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
        ballAp.UpdatePreview(currentBall.transform.position, new Vector3(currentBall.transform.forward.x, yAim, currentBall.transform.forward.z), aimPower);
    }

    private void HitBall()
    {
        ballRb.isKinematic = false;
        ballRb.AddForce(new Vector3(currentBall.transform.forward.x, yAim, currentBall.transform.forward.z) * aimPower, ForceMode.Impulse);
        ballAp.gameObject.SetActive(false);
    }
}

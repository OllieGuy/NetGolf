using Unity.VisualScripting;
using UnityEngine;

public class PlayerPlaceBall : PlayerBaseMovement
{
    [SerializeField] private GameObject ballPreviewPrefab;
    private GameObject ballPreviewObject;
    private BallPlacement ballPlacement;

    public override void StartState()
    {
        base.StartState();
        if (ballPreviewObject != null) ballPreviewObject.SetActive(true);
        else ballPreviewObject = Instantiate(ballPreviewPrefab);
        ballPlacement = ballPreviewObject.GetComponent<BallPlacement>();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        BallPreviewUpdate();
        PlaceBallInput();
    }

    public override void ExitState()
    {
        ballPreviewObject.SetActive(false);
    }

    private void BallPreviewUpdate()
    {
        ballPreviewObject.transform.position = fpCamera.transform.position + fpCamera.transform.forward * 1f;
        ballPreviewObject.transform.rotation = Quaternion.LookRotation(fpCamera.transform.forward);
    }

    private void PlaceBallInput()
    {
        if (pc.attackInput)
        {
            ballPlacement.PlaceBall();
            ChangeState(PlayerStates.BaseMovement);
        }
    }
}

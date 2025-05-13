using Unity.VisualScripting;
using UnityEngine;

public class PlayerPlaceBall : PlayerBaseMovement
{
    [SerializeField] GameObject ballPreviewObject;
    
    BallPlacement ballPlacement;

    public override void StartState()
    {
        //base.StartState();
        ballPlacement = ballPreviewObject.GetComponent<BallPlacement>();
    }
    public override void UpdateState()
    {
        base.UpdateState();
        PlaceUpdate();
        BallPreviewUpdate();
    }

    void BallPreviewUpdate()
    {
        ballPreviewObject.transform.position = fpCamera.transform.position + fpCamera.transform.forward.normalized * 1f;
        ballPreviewObject.transform.rotation = Quaternion.LookRotation(fpCamera.transform.forward);
    }
    
    void PlaceUpdate()
    {
        if (pc.attackInput)
        {
            ballPlacement.PlaceBall();
            pc.attackInput = false;
            ChangeState(PlayerStates.BaseMovement);
        }
    }
}

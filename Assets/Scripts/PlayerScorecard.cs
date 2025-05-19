using Unity.Netcode;
using UnityEngine;

public class PlayerScorecard : PlayerBaseMovement
{
    [SerializeField] Transform scorecardDisplayPosition;
    [SerializeField] Transform scorecardStoredPosition;

    Scorecard scorecard;
    Transform scorecardTransform;

    Vector2Int currentCursorPosition;
    public override void StartState()
    {
        base.StartState();
        scorecard = inventory.scorecard;
        scorecardTransform = inventory.scorecard.gameObject.transform;
        OpenScorecard();
    }

    void OpenScorecard()
    {
        scorecardTransform.parent = scorecardDisplayPosition;
        scorecardTransform.localPosition = Vector3.zero;
        scorecardTransform.localRotation = Quaternion.identity;
    }
    
    void CloseScorecard()
    {
        scorecardTransform.parent = scorecardStoredPosition;
        scorecardTransform.localPosition = Vector3.zero;
        scorecardTransform.localRotation = Quaternion.identity;
    }

    public override void UpdateState()
    {
        if (charController.enabled) MoveUpdate();
        JumpUpdate();
        ScInputUpdate();
        CameraUpdate();
    }

    void ScInputUpdate()
    {
        Vector2Int input = new Vector2Int((int)pc.lookInput.x, (int)pc.lookInput.y);
        currentCursorPosition.x = Mathf.Clamp(currentCursorPosition.x + input.x, 0, scorecard.Width);
        currentCursorPosition.y = Mathf.Clamp(currentCursorPosition.y + input.y, 0, scorecard.Height);
        scorecard.UpdateBrushPosition(currentCursorPosition);

        if (pc.attackInput)
        {
            scorecard.DrawAt(currentCursorPosition, Color.black);
        }
        
        if (pc.attack2Input)
        {
            pc.attack2Input = false;
            scorecard.DropCard(scorecard.gameObject.transform.position);
            inventory.scorecard = null;
            ChangeState(PlayerStates.BaseMovement);
        }

        if (pc.scorecardInput)
        {
            pc.scorecardInput = false;
            CloseScorecard();
            ChangeState(PlayerStates.BaseMovement);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        scorecard = null;
    }
}

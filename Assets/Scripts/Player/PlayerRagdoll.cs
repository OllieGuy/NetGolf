using Unity.Netcode;
using UnityEngine;

public class PlayerRagdoll : PlayerBaseState
{
    [SerializeField] RagdollController ragdollController;
    [SerializeField] Transform normalCamParent;
    [SerializeField] Transform ragdollCamParent;
    Vector3 force;
    Vector3 hitPoint;
    float timer;

    [SerializeField] float ragdollDuration;

    public void SetHitValues(Vector3 _force, Vector3 _hitPoint)
    {
        force = _force;
        hitPoint = _hitPoint;
    }

    public override void StartState()
    {
        if (force == Vector3.zero) ChangeState(PlayerStates.BaseMovement);
        ragdollController.SetRagdoll(true);
        ragdollController.RagdollBase.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
        fpCamera.gameObject.transform.parent = ragdollCamParent;
    }
    
    public override void UpdateState()
    {
        timer += Time.deltaTime;

        Vector3 ragdollPos = ragdollController.RagdollBase.position;
        //playerMesh.transform.position = ragdollPos;

        if (timer > ragdollDuration) ChangeState(PlayerStates.BaseMovement);
    }

    public void OnGetUpFinished()
    {
        fpCamera.gameObject.transform.parent = normalCamParent;
        fpCamera.transform.localPosition = new Vector3(0, 2, 0);
        charController.enabled = true;
    }

    public override void ExitState()
    {
        timer = 0;
        force = Vector3.zero;
        hitPoint = Vector3.zero;

        ragdollController.SetRagdoll(false);

        charController.enabled = false;
        charController.transform.position = new Vector3(
            ragdollController.RagdollBase.position.x,
            ragdollController.RagdollBase.position.y - 0.25f,
            ragdollController.RagdollBase.position.z
        );
        charController.transform.rotation = Quaternion.Euler(0, ragdollController.RagdollBase.rotation.y, 0);

        playerAnimator.SetTrigger("GetUp");
    }
}

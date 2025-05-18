using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public Rigidbody[] ragdollBodies;
    public Animator animator;
    Rigidbody ragdollBase;

    public Rigidbody RagdollBase { get { return ragdollBase; } }

    void Awake()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollBase = ragdollBodies[0];
        SetRagdoll(false);
    }

    public void SetRagdoll(bool enabled)
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = !enabled;
            rb.detectCollisions = enabled;
        }
        animator.enabled = !enabled;
    }
}

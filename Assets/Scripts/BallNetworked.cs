using Unity.Netcode;
using UnityEngine;

public class BallNetworked : NetworkBehaviour
{
    private Rigidbody rb;

    public bool playerCollision;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void LaunchBall(Vector3 direction, float power)
    {
        rb.isKinematic = false;
        rb.AddForce(direction * power, ForceMode.Impulse);
    }
    
    public void RotateBall(Vector3 eulers)
    {
        transform.Rotate(eulers);
    }

    [Rpc(SendTo.Server)]
    public void RotateBallServerRpc(Vector3 eulers)
    {
        RotateBall(eulers);
    }
    
    [Rpc(SendTo.Server)]
    public void HitBallServerRpc(Vector3 direction, float power)
    {
        LaunchBall(direction, power);
    }

    void Update()
    {
        playerCollision = rb.linearVelocity.sqrMagnitude > 1f;
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerState playerState = collision.gameObject.GetComponent<PlayerState>();
        if (playerState != null && playerCollision)
        {
            playerState.TriggerRagdollServerRpc(rb.linearVelocity, collision.contacts[0].point);
        }
    }
}

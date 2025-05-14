using Unity.Netcode;
using UnityEngine;

public class BallNetworked : NetworkBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void LaunchBall(Vector3 direction, float power)
    {
        rb.isKinematic = false;
        rb.AddForce(direction * power, ForceMode.Impulse);
    }

    [Rpc(SendTo.Server)]
    public void HitBallServerRpc(Vector3 direction, float power)
    {
        LaunchBall(direction, power);
    }
}

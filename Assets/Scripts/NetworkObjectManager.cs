using Unity.Netcode;
using UnityEngine;

public class NetworkObjectManager : NetworkBehaviour
{
    [SerializeField] private GameObject ballPrefab;

    private static NetworkObjectManager instance;

    public static NetworkObjectManager Instance => instance;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public void AddBall(Vector3 pos, Quaternion rot, ulong ownerClientId)
    {
        GameObject ball = Instantiate(ballPrefab, pos, rot);
        var netObj = ball.GetComponent<NetworkObject>();
        netObj.SpawnWithOwnership(ownerClientId);
        ball.transform.parent = transform;
    }

    public static void RequestBall(Vector3 pos, Quaternion rot)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            instance.AddBall(pos, rot, NetworkManager.Singleton.LocalClientId);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            instance.RequestBallServerRpc(pos, rot, NetworkManager.Singleton.LocalClientId);
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestBallServerRpc(Vector3 pos, Quaternion rot, ulong senderId)
    {
        AddBall(pos, rot, senderId);
    }
}
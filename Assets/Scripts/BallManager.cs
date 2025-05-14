using Unity.Netcode;
using UnityEngine;

public class BallManager : NetworkBehaviour
{
    [SerializeField] GameObject ballPrefab;

    static BallManager instance = null;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    static public void AddBall(Vector3 pos, Quaternion rot, RpcParams rpcParams = default)
    {
        GameObject instantiatedBall = Instantiate(instance.ballPrefab, pos, rot, instance.gameObject.transform);
        var netObj = instantiatedBall.GetComponent<NetworkObject>();
        netObj.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
    }
    
    static public void AddBallFromClient(Vector3 pos, Quaternion rot, RpcParams rpcParams = default)
    {
        instance.AddBallRpc(pos, rot, rpcParams);
    }

    [Rpc(SendTo.Server)]
    void AddBallRpc(Vector3 pos, Quaternion rot, RpcParams rpcParams = default)
    {
        AddBall(pos, rot, rpcParams);
    }
}

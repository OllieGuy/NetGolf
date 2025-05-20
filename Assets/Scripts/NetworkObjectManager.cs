using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkObjectManager : NetworkBehaviour
{
    [SerializeField] GameObject ballPrefab;

    static NetworkObjectManager instance = null;
    
    public static NetworkObjectManager Instance { get { return instance; } }

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

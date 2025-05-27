using MoreMountains.Feedbacks;
using Unity.Netcode;
using UnityEngine;

public class HoleNetworked : NetworkBehaviour
{
    [SerializeField] MMF_Player HoleFeedback;

    public void OnBallEntered(BallNetworked ball)
    {
        BallInHoleRpc(ball.OwnerClientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void BallInHoleRpc(ulong ballOwnerId)
    {
        if (ballOwnerId == NetworkManager.Singleton.LocalClientId) HoleFeedback.PlayFeedbacks();
    }
}

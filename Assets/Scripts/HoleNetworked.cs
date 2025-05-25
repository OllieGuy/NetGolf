using MoreMountains.Feedbacks;
using Unity.Netcode;
using UnityEngine;

public class HoleNetworked : NetworkBehaviour
{
    [SerializeField] MMF_Player HoleFeedback;

    public void OnBallEntered()
    {
        HoleFeedback.PlayFeedbacks();
    }
}

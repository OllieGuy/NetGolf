using Unity.Netcode;
using UnityEngine;

public class NetworkScorecard : NetworkBehaviour
{
    [SerializeField] GameObject attachedCard;
    [SerializeField] Rigidbody rb;

    public Scorecard TakeCard()
    {
        rb.isKinematic = true;
        return attachedCard.GetComponent<Scorecard>();
    }
    
    public void DropCard(Vector3 position)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            transform.position = position;
            rb.isKinematic = false;
            attachedCard.transform.parent = transform;
            attachedCard.transform.localPosition = Vector3.zero;
            transform.localRotation = attachedCard.transform.localRotation;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            DroppedScorecardRpc(position);
        }
    }

    [Rpc(SendTo.Server)]
    void DroppedScorecardRpc(Vector3 pos)
    {
        DropCard(pos);
    }
}

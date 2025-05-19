using Unity.Netcode;
using UnityEngine;

public class NetworkScorecard : NetworkBehaviour
{
    [SerializeField] GameObject attachedCard;
    [SerializeField] Rigidbody rb;

    byte[] drawnOnCard;

    public Scorecard TakeCard()
    {
        rb.isKinematic = true;
        Scorecard cardData = attachedCard.GetComponent<Scorecard>();
        if (drawnOnCard != null) cardData.LoadDrawingFromBytes(drawnOnCard);
        return cardData;
    }

    public void DropCard(Vector3 position, byte[] drawnBytes)
    {
        if (IsServer)
        {
            drawnOnCard = drawnBytes;
            InternalDrop(position);
            NotifyClientsCardDroppedClientRpc(position, drawnBytes);
        }
        else
        {
            DroppedScorecardRpc(position, drawnBytes);
        }
    }

    void InternalDrop(Vector3 position)
    {
        transform.position = position;
        rb.isKinematic = false;
        attachedCard.transform.parent = transform;
        attachedCard.transform.localPosition = Vector3.zero;
        transform.rotation = attachedCard.transform.rotation;
        attachedCard.transform.localRotation = Quaternion.identity;
    }

    [Rpc(SendTo.Server)]
    void DroppedScorecardRpc(Vector3 pos, byte[] drawnBytes)
    {
        DropCard(pos, drawnBytes);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void NotifyClientsCardDroppedClientRpc(Vector3 position, byte[] drawnBytes)
    {
        if (!IsServer)
        {
            drawnOnCard = drawnBytes;
            InternalDrop(position);
            attachedCard.GetComponent<Scorecard>().LoadDrawingFromBytes(drawnBytes);
        }
    }
}

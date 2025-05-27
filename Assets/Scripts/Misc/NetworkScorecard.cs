using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class NetworkScorecard : NetworkBehaviour
{
    [SerializeField] GameObject attachedCard;
    [SerializeField] BoxCollider scorecardCollider;
    [SerializeField] Rigidbody rb;

    byte[] drawnOnCard;
    ulong holdingClientId = ulong.MaxValue;

    public Scorecard TakeCard(NetworkObject playerObject)
    {
        if (holdingClientId != ulong.MaxValue && holdingClientId != playerObject.OwnerClientId) return null;

        holdingClientId = playerObject.OwnerClientId;

        rb.isKinematic = true;
        scorecardCollider.enabled = false;

        DisableCardForOthersClientRpc(holdingClientId);

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
        scorecardCollider.enabled = true;

        attachedCard.transform.parent = transform;
        attachedCard.transform.localPosition = Vector3.zero;
        attachedCard.transform.localRotation = Quaternion.identity;
        transform.rotation = attachedCard.transform.rotation;

        attachedCard.SetActive(true);
        holdingClientId = ulong.MaxValue;
    }

    [Rpc(SendTo.ClientsAndHost)]
    void DisableCardForOthersClientRpc(ulong holderId)
    {
        if (NetworkManager.Singleton.LocalClientId != holderId)
        {
            attachedCard.SetActive(false);
        }
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

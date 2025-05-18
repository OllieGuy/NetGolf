using Unity.Netcode;
using UnityEngine;

public class BallPlacement : MonoBehaviour
{
    [SerializeField] Material placeableMaterial;
    [SerializeField] Material unplaceableMaterial;
    [SerializeField] MeshRenderer ballRenderer;

    bool isPlaceable = false;

    public void PlaceBall()
    {
        if (isPlaceable && NetworkManager.Singleton.IsServer)
        {
            NetworkObjectManager.AddBall(transform.position, transform.rotation);
        }
        else if (isPlaceable && NetworkManager.Singleton.IsClient)
        {
            NetworkObjectManager.AddBallFromClient(transform.position, transform.rotation);
        }
    }

    private void OnEnable()
    {
        isPlaceable = false;
        ballRenderer.material = unplaceableMaterial;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tee"))
        {
            isPlaceable = true;
            ballRenderer.material = placeableMaterial;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Tee"))
        {
            isPlaceable = false;
            ballRenderer.material = unplaceableMaterial;
        }
    }
}

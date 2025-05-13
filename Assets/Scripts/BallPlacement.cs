using UnityEngine;

public class BallPlacement : MonoBehaviour
{
    [SerializeField] Material placeableMaterial;
    [SerializeField] Material unplaceableMaterial;
    [SerializeField] GameObject ball;
    [SerializeField] MeshRenderer ballRenderer;

    bool isPlaceable = false;

    public void PlaceBall()
    {
        if (isPlaceable)
        {
            Instantiate(ball, transform.position, Quaternion.identity);
            Destroy(this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tee"))
        {
            isPlaceable = true;
            ballRenderer.material = placeableMaterial;
            Debug.Log("in the box");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Tee"))
        {
            isPlaceable = false;
            ballRenderer.material = unplaceableMaterial;
            Debug.Log("out the box");
        }
    }
}

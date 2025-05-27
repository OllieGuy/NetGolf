using Unity.Netcode;
using UnityEngine;

public class BallNetworked : NetworkBehaviour
{
    private Rigidbody rb;
    private float baseLinearDrag;
    private float baseAngularDrag;
    private GroundMaterial currentGroundMaterial;

    public bool playerCollision;
    [SerializeField] BallAimPreview ballAimPreview;
    
    public BallAimPreview BallAimPreview { get { return ballAimPreview;} }
    public bool Hittable { get { return rb.linearVelocity.sqrMagnitude < 0.1f; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        baseLinearDrag = rb.linearDamping;
        baseAngularDrag = rb.angularDamping;
        currentGroundMaterial = null;
    }

    public void Stopball()
    {
        transform.rotation = Quaternion.identity;
        rb.isKinematic = true;
    }
    
    public void LaunchBall(Vector3 direction, float power)
    {
        rb.isKinematic = false;
        rb.AddForce(direction * power, ForceMode.Impulse);
    }
    
    public void RotateBall(Vector3 eulers)
    {
        transform.Rotate(eulers);
    }

    [Rpc(SendTo.Server)]
    public void RotateBallServerRpc(Vector3 eulers)
    {
        RotateBall(eulers);
    }
    
    [Rpc(SendTo.Server)]
    public void HitBallServerRpc(Vector3 direction, float power)
    {
        LaunchBall(direction, power);
    }

    void Update()
    {
        playerCollision = rb.linearVelocity.sqrMagnitude > 1f;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ENTER: " + collision.gameObject.name);

        if (collision.gameObject.TryGetComponent(out GroundMaterialApplier applier) && applier.groundMaterial != null)
        {
            ApplyGroundMaterial(applier.groundMaterial);
        }

        PlayerState playerState = collision.gameObject.GetComponent<PlayerState>();
        if (playerState != null && playerCollision)
        {
            playerState.TriggerRagdollServerRpc(rb.linearVelocity, collision.contacts[0].point);
        }
    }


    void OnCollisionExit(Collision collision)
    {
        Debug.Log("EXIT: " + collision.gameObject.name);

        if (collision.gameObject.TryGetComponent(out GroundMaterialApplier applier) && applier.groundMaterial == currentGroundMaterial)
        {
            RemoveGroundMaterial();
        }
    }

    void ApplyGroundMaterial(GroundMaterial material)
    {
        currentGroundMaterial = material;
        rb.linearDamping = material.linearDrag;
        rb.angularDamping = material.angularDrag;
    }

    void RemoveGroundMaterial()
    {
        rb.linearDamping = baseLinearDrag;
        rb.angularDamping = baseAngularDrag;
        currentGroundMaterial = null;
    }

    void OnTriggerEnter(Collider collider)
    {
        HoleNetworked hole = collider.gameObject.GetComponent<HoleNetworked>();
        if (hole != null)
        {
            hole.OnBallEntered(this);
            Destroy(gameObject);
        }
    }
}

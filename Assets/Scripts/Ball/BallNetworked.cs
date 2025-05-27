using Unity.Burst.Intrinsics;
using Unity.Netcode;
using UnityEngine;

public class BallNetworked : NetworkBehaviour
{
    private Rigidbody rb;
    private float baseLinearDrag;
    private float baseAngularDrag;
    private GroundMaterial currentGroundMaterial;

    public bool playerCollision;
    [SerializeField] private BallAimPreview ballAimPreview;
    [SerializeField] private Transform aimRotation;
    public BallAimPreview BallAimPreview => ballAimPreview;
    public Vector3 BallAimDirection => aimRotation.forward;
    public bool Hittable => rb.linearVelocity.sqrMagnitude < 0.1f;

    // Prediction and reconciliation
    private CircularBuffer<BallState> stateBuffer;
    private NetworkTimer networkTimer;
    private const float reconciliationThreshold = 0.01f;
    private const float rotationThresholdDegrees = 1f;

    [SerializeField] private int bufferSize = 1024;
    [SerializeField] private float tickRate = 60f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        baseLinearDrag = rb.linearDamping;
        baseAngularDrag = rb.angularDamping;
        currentGroundMaterial = null;

        stateBuffer = new CircularBuffer<BallState>(bufferSize);
        networkTimer = new NetworkTimer(tickRate);
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            networkTimer.Update(Time.fixedDeltaTime);
            if (networkTimer.ShouldTick())
            {
                int tick = networkTimer.CurrentTick;

                // Predict
                BallState predictedState = new BallState
                {
                    position = transform.position,
                    rotation = transform.rotation,
                    velocity = rb.linearVelocity,
                    angularVelocity = rb.angularVelocity
                };

                stateBuffer.Add(predictedState, tick);
                SendStateToServerServerRpc(predictedState.position, predictedState.rotation, tick);
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void SendStateToServerServerRpc(Vector3 position, Quaternion rotation, int tick)
    {
        if (IsServer)
        {
            SendAuthoritativeStateClientRpc(transform.position, transform.rotation, tick);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SendAuthoritativeStateClientRpc(Vector3 serverPos, Quaternion serverRot, int tick)
    {
        if (!IsOwner) return;

        BallState predicted = stateBuffer.Get(tick);
        float positionError = (predicted.position - serverPos).sqrMagnitude;
        float rotationError = Quaternion.Angle(predicted.rotation, serverRot);

        if (positionError > reconciliationThreshold || rotationError > rotationThresholdDegrees)
        {
            // Optionally use interpolation here for smoothing
            rb.position = serverPos;
            rb.rotation = serverRot;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void Stopball()
    {
        transform.rotation = Quaternion.identity;
        rb.isKinematic = true;
    }

    public void HitBall(Vector3 direction, float power)
    {
        rb.isKinematic = false;
        rb.AddForce(direction * power, ForceMode.Impulse);
    }

    public void RotateAim(Vector3 eulers)
    {
        aimRotation.rotation = Quaternion.Euler(aimRotation.rotation.eulerAngles + eulers);
        Debug.DrawRay(transform.position, aimRotation.transform.forward, Color.green);
    }

    [Rpc(SendTo.Server)]
    public void HitBallServerRpc(float power)
    {
        HitBall(aimRotation.forward, power);
    }

    void Update()
    {
        playerCollision = rb.linearVelocity.sqrMagnitude > 1f;
    }

    void OnCollisionEnter(Collision collision)
    {
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

public struct BallState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
}
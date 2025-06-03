using Unity.Burst.Intrinsics;
using Unity.Netcode;
using Unity.Netcode.Components;
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

    [SerializeField] NetworkTransform networkTransform;
    private NetworkTimer networkTimer;
    private CircularBuffer<BallState> clientStateBuffer;
    private CircularBuffer<BallState> serverStateBuffer;
    private const float reconciliationThreshold = 0.01f;

    [SerializeField] private int bufferSize = 512;
    [SerializeField] private float tickRate = 30f;

    bool initialised = false;

    private void Initialise()
    {
        rb = GetComponent<Rigidbody>();
        baseLinearDrag = rb.linearDamping;
        baseAngularDrag = rb.angularDamping;
        currentGroundMaterial = null;

        clientStateBuffer = new(bufferSize);
        serverStateBuffer = new(bufferSize);
        networkTimer = new(tickRate);
        if (!IsOwner || IsServer)
        {
            networkTransform.enabled = true;
        }
    }

    private void Update()
    {
        if (!initialised)
        {
            Initialise();
            initialised = true;
        }
        networkTimer.Update(Time.deltaTime);
        playerCollision = rb.linearVelocity.sqrMagnitude > 1f;
    }

    private void FixedUpdate()
    {
        if (networkTimer != null && networkTimer.ShouldTick())
        {
            int tick = networkTimer.CurrentTick;
            if (IsServer)
            {
                BallState state = new BallState
                {
                    position = transform.position,
                    velocity = rb.linearVelocity,
                    angularVelocity = rb.angularVelocity
                };

                serverStateBuffer.Add(state, tick);
            }
            
            if (IsOwner)
            {
                BallState clientState = new BallState
                {
                    position = transform.position,
                    velocity = rb.linearVelocity,
                    angularVelocity = rb.angularVelocity
                };

                clientStateBuffer.Add(clientState, tick);
                SendStateToServerServerRpc(clientState, tick);
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void SendStateToServerServerRpc(BallState clientState, int tick)
    {
        if (!serverStateBuffer.TryGet(tick, out var serverState)) return;

        float positionError = (serverState.position - clientState.position).sqrMagnitude;

        if (positionError > reconciliationThreshold)
        {
            SendToReconcileClientRpc(serverState);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SendToReconcileClientRpc(BallState serverState)
    {
        if (!IsOwner) return;

        rb.position = serverState.position;
        rb.linearVelocity = serverState.velocity;
        rb.angularVelocity = serverState.angularVelocity;
        rb.isKinematic = false;
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

    public void SetAim(Vector3 eulers)
    {
        aimRotation.rotation = Quaternion.Euler(eulers);
    }
    
    public void RotateAim(Vector3 eulers)
    {
        aimRotation.rotation = Quaternion.Euler(aimRotation.rotation.eulerAngles + eulers);
    }

    [Rpc(SendTo.Server)]
    public void HitBallServerRpc(Vector3 direction, float power)
    {
        HitBall(direction, power);
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

public struct BallState : INetworkSerializable
{
    public int tick;
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref velocity);
        serializer.SerializeValue(ref angularVelocity);
    }
}
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkAnimator : NetworkAnimator
{
    public NetworkVariable<float> NetSpeed = new NetworkVariable<float>(
        writePerm: NetworkVariableWritePermission.Owner);
    
    public NetworkVariable<bool> NetGrounded = new NetworkVariable<bool>(
        writePerm: NetworkVariableWritePermission.Owner);

    void Update()
    {
        if (IsOwner)
        {
            NetSpeed.Value = Animator.GetFloat("PlayerSpeed");
            NetGrounded.Value = Animator.GetBool("Grounded");
        }

        Animator.SetFloat("PlayerSpeed", NetSpeed.Value);
        Animator.SetBool("Grounded", NetGrounded.Value);
    }

    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}

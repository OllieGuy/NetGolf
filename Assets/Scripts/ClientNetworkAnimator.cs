using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkAnimator : NetworkAnimator
{
    public NetworkVariable<float> NetSpeed = new NetworkVariable<float>(
        writePerm: NetworkVariableWritePermission.Owner);

    void Update()
    {
        if (IsOwner)
        {
            NetSpeed.Value = Animator.GetFloat("PlayerSpeed");
        }

        Animator.SetFloat("PlayerSpeed", NetSpeed.Value);
    }

    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}

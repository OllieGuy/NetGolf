using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerCosmetics : NetworkBehaviour
{
    [SerializeField] SkinnedMeshRenderer playerRenderer;

    [SerializeField] Material[] skins;
    [SerializeField] Material[] accessoryColors;

    [SerializeField] Accessory[] faceAccessories;
    [SerializeField] Transform faceAccessoryTransform;

    [SerializeField] Accessory[] hairstyles;
    [SerializeField] Accessory[] headAccessories;
    [SerializeField] Transform headAccessoryTransform;
    List<GameObject> ownCosmetics = new();

    NetworkVariable<CosmeticData> syncedCosmetics = new();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            CosmeticData data = GenerateRandomCosmetics();
            syncedCosmetics.Value = data;
        }

        syncedCosmetics.OnValueChanged += (_, newValue) => ApplyCosmetics(newValue);
        ApplyCosmetics(syncedCosmetics.Value);
    }

    CosmeticData GenerateRandomCosmetics()
    {
        CosmeticData data = new CosmeticData
        {
            skinIndex = UnityEngine.Random.Range(0, skins.Length),

            faceAccessoryIndex = UnityEngine.Random.Range(0, 100) < 10 ?
                UnityEngine.Random.Range(0, faceAccessories.Length) : -1,

            hairIndex = -1,
            hatIndex = -1
        };

        int headSeed = UnityEngine.Random.Range(0, 100);
        if (headSeed < 75)
            data.hairIndex = UnityEngine.Random.Range(0, hairstyles.Length);
        else if (headSeed >= 95)
            data.hatIndex = UnityEngine.Random.Range(0, headAccessories.Length);

        data.faceColorIndex = UnityEngine.Random.Range(0, accessoryColors.Length);
        data.hairColorIndex = UnityEngine.Random.Range(0, accessoryColors.Length);
        data.hatColorIndex = UnityEngine.Random.Range(0, accessoryColors.Length);

        return data;
    }

    void ApplyCosmetics(CosmeticData data)
    {
        playerRenderer.material = skins[data.skinIndex];

        foreach (var obj in ownCosmetics)
            Destroy(obj);
        ownCosmetics.Clear();

        // Face Accessory
        if (data.faceAccessoryIndex >= 0)
        {
            var acc = Instantiate(faceAccessories[data.faceAccessoryIndex].gameObj, faceAccessoryTransform);
            ownCosmetics.Add(acc);
            acc.transform.localPosition = faceAccessories[data.faceAccessoryIndex].offset;
            acc.GetComponent<MeshRenderer>().material = 
                faceAccessories[data.faceAccessoryIndex].texture != null ? 
                faceAccessories[data.faceAccessoryIndex].texture : 
                accessoryColors[data.faceColorIndex];
        }

        // Hair
        if (data.hairIndex >= 0)
        {
            var acc = Instantiate(hairstyles[data.hairIndex].gameObj, headAccessoryTransform);
            ownCosmetics.Add(acc);
            acc.transform.localPosition = hairstyles[data.hairIndex].offset;
            acc.GetComponent<MeshRenderer>().material = 
                hairstyles[data.hairIndex].texture != null ? 
                hairstyles[data.hairIndex].texture : 
                accessoryColors[data.hairColorIndex];
        }

        // Hat
        if (data.hatIndex >= 0)
        {
            var acc = Instantiate(headAccessories[data.hatIndex].gameObj, headAccessoryTransform);
            ownCosmetics.Add(acc);
            acc.transform.localPosition = headAccessories[data.hatIndex].offset;
            acc.GetComponent<MeshRenderer>().material = 
                headAccessories[data.hatIndex].texture != null ? 
                headAccessories[data.hatIndex].texture : 
                accessoryColors[data.hatColorIndex];
        }

        // Change layer so camera doesn't render
        if (IsOwner)
        {
            foreach (GameObject obj in ownCosmetics)
                obj.layer = LayerMask.NameToLayer("PlayerSelf");
        }
    }
}

[Serializable]
public class Accessory
{
    public GameObject gameObj;
    public Material texture;
    public Vector3 offset;
}

public struct CosmeticData : INetworkSerializable
{
    public int skinIndex;
    public int faceAccessoryIndex;
    public int hairIndex;
    public int hatIndex;
    public int faceColorIndex;
    public int hairColorIndex;
    public int hatColorIndex;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref skinIndex);
        serializer.SerializeValue(ref faceAccessoryIndex);
        serializer.SerializeValue(ref hairIndex);
        serializer.SerializeValue(ref hatIndex);
        serializer.SerializeValue(ref faceColorIndex);
        serializer.SerializeValue(ref hairColorIndex);
        serializer.SerializeValue(ref hatColorIndex);
    }
}
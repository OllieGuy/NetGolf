using System;
using UnityEngine;

public class PlayerCosmetics : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer playerRenderer;

    [SerializeField] Material[] skins;
    [SerializeField] Material[] accessoryColors;

    [SerializeField] Accessory[] faceAccessories;
    [SerializeField] Transform faceAccessoryTransform;

    [SerializeField] Accessory[] hairstyles;
    [SerializeField] Accessory[] headAccessories;
    [SerializeField] Transform headAccessoryTransform;

    void Start()
    {
        SetTexture();
        AddAccessories();
    }

    void SetTexture()
    {
        int skinIndex = UnityEngine.Random.Range(0, skins.Length - 1);
        playerRenderer.material = skins[skinIndex];
    }

    void AddAccessories()
    {
        if (UnityEngine.Random.Range(0, 100) < 10) // 10% face accessory chance
        {
            int accessoryIndex = UnityEngine.Random.Range(0, faceAccessories.Length - 1);
            GameObject accessory = Instantiate(faceAccessories[accessoryIndex].gameObj, faceAccessoryTransform);
            accessory.transform.localPosition = Vector3.zero + faceAccessories[accessoryIndex].offset;
            accessory.GetComponent<MeshRenderer>().material = faceAccessories[accessoryIndex].texture == null ? AccessoryColour() : faceAccessories[accessoryIndex].texture;
        }

        int headSeed = UnityEngine.Random.Range(0, 100);
        if (headSeed < 75) // 75% chance for hair
        {
            int hairIndex = UnityEngine.Random.Range(0, hairstyles.Length - 1);
            GameObject accessory = Instantiate(hairstyles[hairIndex].gameObj, headAccessoryTransform);
            accessory.transform.localPosition = Vector3.zero + hairstyles[hairIndex].offset;
            accessory.GetComponent<MeshRenderer>().material = hairstyles[hairIndex].texture == null ? AccessoryColour() : hairstyles[hairIndex].texture;
        }
        else if (headSeed >= 95) // 5% chance for hat
        {
            int headAccessoryIndex = UnityEngine.Random.Range(0, headAccessories.Length - 1);
            GameObject accessory = Instantiate(headAccessories[headAccessoryIndex].gameObj, headAccessoryTransform);
            accessory.transform.localPosition = Vector3.zero + headAccessories[headAccessoryIndex].offset;
            accessory.GetComponent<MeshRenderer>().material = headAccessories[headAccessoryIndex].texture == null ? AccessoryColour() : headAccessories[headAccessoryIndex].texture;

        }

        Material AccessoryColour()
        {
            int colorInList = UnityEngine.Random.Range(0, accessoryColors.Length - 1);
            return accessoryColors[colorInList];
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
using System;
using UnityEngine;

public class PlayerCosmetics : MonoBehaviour
{
    [SerializeField] Material[] skins;
    [SerializeField] Material[] accessoryColors;

    [SerializeField] Accessory[] faceAccessories;
    [SerializeField] Transform faceAccessoryTransform;

    [SerializeField] Accessory[] hairstyles;
    [SerializeField] Accessory[] headAccessories;
    [SerializeField] Transform headAccessoryTransform;

    void Start()
    {
        AddAccessories();
    }

    void AddAccessories()
    {
        if (UnityEngine.Random.Range(0, 100) < 10) // 10% face accessory chance
        {
            int accessoryInList = UnityEngine.Random.Range(0, faceAccessories.Length);
            GameObject accessory = Instantiate(faceAccessories[accessoryInList].gameObj, faceAccessoryTransform);
            accessory.GetComponent<MeshRenderer>().material = faceAccessories[accessoryInList].texture == null ? AccessoryColour() : faceAccessories[0].texture;
        }

        int headSeed = UnityEngine.Random.Range(0, 100);
        if (headSeed < 75) // 75% chance for hair
        {
            int hairInList = UnityEngine.Random.Range(0, hairstyles.Length);
            GameObject accessory = Instantiate(hairstyles[hairInList].gameObj, headAccessoryTransform);
            accessory.GetComponent<MeshRenderer>().material = hairstyles[hairInList].texture == null ? AccessoryColour() : hairstyles[hairInList].texture;
        }
        else if (headSeed >= 95) // 5% chance for hat
        {
            int headAccessoryInList = UnityEngine.Random.Range(0, headAccessories.Length);
            GameObject accessory = Instantiate(headAccessories[headAccessoryInList].gameObj, headAccessoryTransform);
            accessory.GetComponent<MeshRenderer>().material = headAccessories[headAccessoryInList].texture == null ? AccessoryColour() : headAccessories[headAccessoryInList].texture;

        }

        Material AccessoryColour()
        {
            int colorInList = UnityEngine.Random.Range(0, accessoryColors.Length);
            return accessoryColors[colorInList];
        }
    }
}

[Serializable]
public class Accessory
{
    public GameObject gameObj;
    public Material texture;
}
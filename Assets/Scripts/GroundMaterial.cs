using UnityEngine;

[CreateAssetMenu(fileName = "GroundMaterial", menuName = "Scriptable Objects/GroundMaterial")]
public class GroundMaterial : ScriptableObject
{
    public float linearDrag = 0.5f;
    public float angularDrag = 0.5f;
    public float bounce = 0.5f;
    public PhysicsMaterialCombine bounceCombine = PhysicsMaterialCombine.Average;
}
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GroundMaterialApplier : MonoBehaviour
{
    public GroundMaterial groundMaterial;

    private void Reset()
    {
        if (TryGetComponent(out Collider col))
        {
            col.material = new PhysicsMaterial();
        }
    }

    private void Start()
    {
        if (groundMaterial && TryGetComponent(out Collider col))
        {
            PhysicsMaterial mat = new PhysicsMaterial();
            mat.bounciness = groundMaterial.bounce;
            mat.bounceCombine = groundMaterial.bounceCombine;
            col.material = mat;
        }
    }
}
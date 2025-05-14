using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(LineRenderer))]
public class BallAimPreview : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Gradient powerGradient;
    [SerializeField] private int resolution = 30;
    [SerializeField] private float lineWidth = 0.05f;

    float maxPower;
    private Gradient dynamicGradient;

    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        dynamicGradient = new Gradient();
    }

    public void Initialise(float _maxPower)
    {
        maxPower = _maxPower;
    }

    public void UpdatePreview(Vector3 startPosition, Vector3 direction, float power)
    {
        lineRenderer.positionCount = resolution;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        float normalizedPower = Mathf.Clamp01(power / maxPower);
        Color baseColor = powerGradient.Evaluate(normalizedPower);

        GradientColorKey[] colorKeys = new GradientColorKey[]
        {
        new GradientColorKey(baseColor, 0f),
        new GradientColorKey(baseColor, 1f)
        };

        float fadeEnd = 10f / maxPower;
        fadeEnd = Mathf.Clamp01(fadeEnd);

        GradientAlphaKey[] alphaKeys;

        alphaKeys = new GradientAlphaKey[]
        {
            new GradientAlphaKey(0.3f, 0f),
            new GradientAlphaKey(baseColor.a, fadeEnd),
            new GradientAlphaKey(baseColor.a, 1f)
        };

        dynamicGradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = dynamicGradient;

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            Vector3 point = startPosition + direction * power * t + Physics.gravity * t * t * 0.5f;
            lineRenderer.SetPosition(i, point);
        }
    }
}

using UnityEngine;

public class Scorecard : MonoBehaviour
{
    [SerializeField] Renderer cardRenderer;
    [SerializeField] Texture2D baseTexture;
    [SerializeField] GameObject brushIcon;
    private Texture2D drawingTexture;
    private Texture2D combinedTexture;

    public int Height { get { return baseTexture.height;} }
    public int Width { get { return baseTexture.width; } }

    void Start()
    {
        drawingTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.RGBA32, false);
        ClearTexture(drawingTexture);

        combinedTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.RGBA32, false);
        CombineTextures();

        cardRenderer.material.mainTexture = combinedTexture;
    }

    void ClearTexture(Texture2D tex)
    {
        Color[] pixels = new Color[tex.width * tex.height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
        tex.SetPixels(pixels);
        tex.Apply();
    }
    
    public void UpdateBrushPosition(Vector2Int coords)
    {
        Vector3 normalisedBrushPosition = new Vector3(
            ((float)coords.x / (float)Width) - 0.5f, 
            ((float)coords.y / (float)Height) - 0.5f, 
            -0.001f);
        brushIcon.transform.localPosition = normalisedBrushPosition;
    }
    
    public void UpdateBrushColor(Color color)
    {
        brushIcon.GetComponent<SpriteRenderer>().color = color;
    }

    public void DrawAt(Vector2Int coords, Color color)
    {
        int brushSize = 2;
        for (int i = -brushSize; i < brushSize; i++)
        {
            for (int j = -brushSize; j < brushSize; j++)
            {
                int px = coords.x + i;
                int py = coords.y + j;
                if (px >= 0 && py >= 0 && px < drawingTexture.width && py < drawingTexture.height)
                    drawingTexture.SetPixel(px, py, color);
            }
        }
        drawingTexture.Apply();
        CombineTextures();
    }

    void CombineTextures()
    {
        Color[] basePixels = baseTexture.GetPixels();
        Color[] drawingPixels = drawingTexture.GetPixels();
        for (int i = 0; i < basePixels.Length; i++)
        {
            basePixels[i] = Color.Lerp(basePixels[i], drawingPixels[i], drawingPixels[i].a);
        }
        combinedTexture.SetPixels(basePixels);
        combinedTexture.Apply();
    }

    public byte[] GetDrawingBytes() => drawingTexture.EncodeToPNG();

    public void LoadDrawingFromBytes(byte[] data)
    {
        Texture2D loaded = new Texture2D(baseTexture.width, baseTexture.height);
        loaded.LoadImage(data);
        drawingTexture = loaded;
        CombineTextures();
    }
}

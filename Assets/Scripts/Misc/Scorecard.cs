using UnityEngine;

public class Scorecard : MonoBehaviour
{
    [SerializeField] GameObject networkedParentObject;
    NetworkScorecard networkedParent;
    [SerializeField] Renderer cardRenderer;
    [SerializeField] Texture2D baseTexture;
    [SerializeField] GameObject brushIcon;
    private Texture2D drawingTexture;
    private Texture2D combinedTexture;

    Vector2Int previousDrawLocation;
    bool wasDrawring;

    public int Height { get { return baseTexture.height;} }
    public int Width { get { return baseTexture.width; } }
    public bool WasDrawring { get { return wasDrawring; } }

    void Start()
    {
        networkedParent = networkedParentObject.GetComponent<NetworkScorecard>();
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

    public void DrawAt(Vector2Int coords, Color color, int brushSize)
    {
        if (wasDrawring)
        {
            foreach (Vector2Int point in InterpolateLine(previousDrawLocation, coords))
            {
                DrawBrush(point, color, brushSize);
            }
        }
        else
        {
            DrawBrush(coords, color, brushSize);
        }

        drawingTexture.Apply();
        CombineTextures();

        previousDrawLocation = coords;
        wasDrawring = true;
    }

    void DrawBrush(Vector2Int coords, Color color, int brushSize)
    {
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
    }

    System.Collections.Generic.IEnumerable<Vector2Int> InterpolateLine(Vector2Int p0, Vector2Int p1)
    {
        int dx = Mathf.Abs(p1.x - p0.x);
        int dy = Mathf.Abs(p1.y - p0.y);

        int sx = p0.x < p1.x ? 1 : -1;
        int sy = p0.y < p1.y ? 1 : -1;

        int err = dx - dy;

        Vector2Int current = p0;
        while (true)
        {
            yield return current;
            if (current == p1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                current.x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                current.y += sy;
            }
        }
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

    public void StopDrawing()
    {
        wasDrawring = false;
    }

    public void DropCard(Vector3 position)
    {
        networkedParent.DropCard(position, GetDrawingBytes());
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

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Painting : MonoBehaviour
{
    [Header("画笔设置")]
    public Color paintColor = new Color(1f, 0.654902f, 0.6196079f, 1f);
    public float brushSize = 5f;

    private SpriteRenderer spriteRenderer;
    private Collider2D paintCollider;
    private Texture2D canvasTexture;
    private Texture2D backgroundTexture; // 保存原始背景
    private Vector2 lastPixelPos;
    private bool isDrawing = false;
    private int textureWidth;
    private int textureHeight;
    private Vector2 spriteSize;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        paintCollider = GetComponent<Collider2D>();

        Sprite originalSprite = spriteRenderer.sprite;
        if (originalSprite != null)
        {
            textureWidth = (int)originalSprite.rect.width;
            textureHeight = (int)originalSprite.rect.height;
            spriteSize = originalSprite.bounds.size;

            // 复制原始纹理
            Texture2D bgTex = SpriteToTexture2D(originalSprite);
            backgroundTexture = new Texture2D(textureWidth, textureHeight, bgTex.format, false);
            backgroundTexture.SetPixels(bgTex.GetPixels());
            backgroundTexture.Apply();

            canvasTexture = new Texture2D(textureWidth, textureHeight, bgTex.format, false);
            canvasTexture.SetPixels(bgTex.GetPixels());
            canvasTexture.Apply();
            Destroy(bgTex);
        }
        else
        {
            textureWidth = 512;
            textureHeight = 512;
            spriteSize = Vector2.one;
            canvasTexture = new Texture2D(textureWidth, textureHeight);
            FillCanvas(Color.white);
            backgroundTexture = null;
        }

        ApplyTextureToSprite();
    }

    Texture2D SpriteToTexture2D(Sprite sprite)
    {
        Rect rect = sprite.rect;
        int w = (int)rect.width;
        int h = (int)rect.height;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        RenderTexture rt = RenderTexture.GetTemporary(w, h, 0);
        Graphics.Blit(sprite.texture, rt);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();
        RenderTexture.ReleaseTemporary(rt);
        RenderTexture.active = null;
        return tex;
    }

    void FillCanvas(Color color)
    {
        Color[] colors = new Color[textureWidth * textureHeight];
        for (int i = 0; i < colors.Length; i++) colors[i] = color;
        canvasTexture.SetPixels(colors);
        canvasTexture.Apply();
    }

    void Update()
    {
        if (paintCollider == null || spriteRenderer == null || canvasTexture == null) return;

        // 按下左键
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (paintCollider.OverlapPoint(worldPoint))
            {
                isDrawing = true;
                Vector2 pixelPos = WorldToPixel(worldPoint);
                DrawPoint(pixelPos);
                lastPixelPos = pixelPos;
                ApplyTextureToSprite();
            }
        }

        // 按住左键拖动
        if (Input.GetMouseButton(0) && isDrawing)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (paintCollider.OverlapPoint(worldPoint))
            {
                Vector2 currentPixelPos = WorldToPixel(worldPoint);
                DrawLine(lastPixelPos, currentPixelPos);
                lastPixelPos = currentPixelPos;
                ApplyTextureToSprite();
            }
            else
            {
                isDrawing = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }
    }

    Vector2 WorldToPixel(Vector2 worldPos)
    {
        Vector2 localPos = transform.InverseTransformPoint(worldPos);
        float u = (localPos.x / spriteSize.x) + 0.5f;
        float v = (localPos.y / spriteSize.y) + 0.5f;
        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);
        int px = Mathf.RoundToInt(u * (textureWidth - 1));
        int py = Mathf.RoundToInt(v * (textureHeight - 1));
        return new Vector2(px, py);
    }

    void DrawPoint(Vector2 pixelPos)
    {
        int cx = (int)pixelPos.x;
        int cy = (int)pixelPos.y;
        int radius = Mathf.RoundToInt(brushSize);
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    int px = cx + x;
                    int py = cy + y;
                    if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                        canvasTexture.SetPixel(px, py, paintColor);
                }
            }
        }
    }

    void DrawLine(Vector2 from, Vector2 to)
    {
        float dist = Vector2.Distance(from, to);
        if (dist < 0.5f) { DrawPoint(to); return; }
        int steps = Mathf.CeilToInt(dist / 2f);
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 pos = Vector2.Lerp(from, to, t);
            DrawPoint(pos);
        }
    }

    void ApplyTextureToSprite()
    {
        if (canvasTexture == null) return;
        canvasTexture.Apply();
        Sprite newSprite = Sprite.Create(canvasTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f), 100f);
        spriteRenderer.sprite = newSprite;
    }

    public void ClearCanvas()
    {
        if (backgroundTexture != null)
        {
            canvasTexture.SetPixels(backgroundTexture.GetPixels());
            canvasTexture.Apply();
            ApplyTextureToSprite();
        }
        else
        {
            FillCanvas(Color.white);
            ApplyTextureToSprite();
        }
    }

    public void ClearButton() => ClearCanvas();
    public void SetPaintColor(Color newColor) => paintColor = newColor;
    public void SetBrushSize(float newSize) => brushSize = newSize;
}
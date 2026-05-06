using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptToCamera_SpriteRenderer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sprite currentSprite;

    public void AdaptToCamera()
    {
        Camera camera = Camera.main;
        float screenY = camera.orthographicSize * 2f;
        float screenX = screenY * camera.aspect;

        float spriteY=spriteRenderer.sprite.bounds.size.y;
        float spriteX =spriteRenderer.sprite.bounds.size.x;

        float scaleX = screenX / spriteX;
        float scaleY = screenY / spriteY;

        float uniformScale = Mathf.Min(scaleX, scaleY);

        transform.localScale = new Vector3(uniformScale, uniformScale, 1f);
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSprite = spriteRenderer.sprite;
    }

    private void Update()
    {
        // 检测当前图片是否发生变化
        if (spriteRenderer.sprite != currentSprite)
        {
            currentSprite = spriteRenderer.sprite;
            AdaptToCamera();
        }
    }

}

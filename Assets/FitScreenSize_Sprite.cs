using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitScreenSize_Sprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;


    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        FitScreenSize();
    }

    public void FitScreenSize()
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
}

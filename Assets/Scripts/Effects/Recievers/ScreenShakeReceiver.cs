using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ScreenShakeReceiver : MonoBehaviour
{
    public Shader shakeShader;
    private Material material;
    private float currentOffsetX = 0f;
    private float currentOffsetY = 0f;

    void Start()
    {
        if (shakeShader != null)
            material = new Material(shakeShader);
    }

    /// <summary>
    /// 外部调用，开始晃动
    /// </summary>
    public IEnumerator Shake(float duration, float magnitude)
    {
        yield return StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private System.Collections.IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float strength = magnitude * (1f - t);
            currentOffsetX = Random.Range(-strength, strength);
            currentOffsetY = Random.Range(-strength, strength);
            elapsed += Time.deltaTime;
            yield return null;
        }
        currentOffsetX = 0f;
        currentOffsetY = 0f;
        
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null && (currentOffsetX != 0f || currentOffsetY != 0f))
        {
            material.SetFloat("_OffsetX", currentOffsetX);
            material.SetFloat("_OffsetY", currentOffsetY);
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
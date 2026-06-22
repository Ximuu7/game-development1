using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOutline : MonoBehaviour
{
    [Header("描边参数")]
    public Color outlineColor = Color.white;
    [Range(1, 20)] public float outlineWidth = 5f;

    private Material material;
    private SpriteRenderer spriteRenderer;
    private Coroutine currentCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Shader shader = Shader.Find("Custom/SpriteOutline");
        if (shader != null)
        {
            material = new Material(shader);
            spriteRenderer.material = material;
        }
        else
        {
            Debug.LogWarning("SpriteOutline: Shader 'Custom/SpriteOutline' not found. Using default material.");
            material = null;
        }
        SetOutlineImmediate(false);
    }

    public void SetOutlineImmediate(bool show)
    {
        if (material == null) return;
        if (show)
        {
            material.SetColor("_OutlineColor", outlineColor);
            material.SetFloat("_OutlineWidth", outlineWidth);
        }
        else
        {
            material.SetColor("_OutlineColor", Color.clear);
            material.SetFloat("_OutlineWidth", 0f);
        }
    }

    public void StartAnimate(bool show, float duration = 0.5f)
    {
        if (material == null) return;
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(AnimateCoroutine(show, duration));
    }

    public IEnumerator AnimateCoroutine(bool show, float duration)
    {
        if (material == null) yield break;
        Color targetColor = show ? outlineColor : Color.clear;
        float targetWidth = show ? outlineWidth : 0f;

        Color currentColor = material.GetColor("_OutlineColor");
        float currentWidth = material.GetFloat("_OutlineWidth");

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            material.SetColor("_OutlineColor", Color.Lerp(currentColor, targetColor, t));
            material.SetFloat("_OutlineWidth", Mathf.Lerp(currentWidth, targetWidth, t));
            yield return null;
        }
        material.SetColor("_OutlineColor", targetColor);
        material.SetFloat("_OutlineWidth", targetWidth);
        currentCoroutine = null;
    }

    public bool IsAnimating => currentCoroutine != null;

    void OnDestroy()
    {
        if (material != null) DestroyImmediate(material);
    }
}
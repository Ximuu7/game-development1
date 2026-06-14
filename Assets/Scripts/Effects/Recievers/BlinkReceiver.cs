using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class BlinkReceiver : MonoBehaviour
{
    public Shader effectShader;
    [Range(0f, 1f)]
    public float openEyesValue = 1f;   // 0=全闭，1=全开

    private Material material;

    void Start()
    {
        if (effectShader == null)
        {
            effectShader = Shader.Find("Custom/EyeShader");
        }
        if (effectShader != null)
        {
            material = new Material(effectShader);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            material.SetFloat("_OpenValue", openEyesValue);
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
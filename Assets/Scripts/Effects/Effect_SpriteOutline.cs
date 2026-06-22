using System.Collections;
using UnityEngine;

public class Effect_SpriteOutline : Effect
{
    public SpriteOutline receiver;          // 目标描边组件
    public bool show = true;                // true=显示，false=隐藏
    public float duration = 0.5f;           // 渐变时长

    public override IEnumerator Effects()
    {
        if (receiver == null) yield break;
        // 启动动画并等待完成（会与外部调用冲突？此处独立协程，不与 StartAnimate 冲突）
        yield return receiver.AnimateCoroutine(show, duration);
    }
}
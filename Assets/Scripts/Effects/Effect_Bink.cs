using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlinkController : Effect
{
    public BlinkReceiver blinkReceiver;   // 拖拽相机上的接收器组件
    public float duration = 3f;       // 睁眼所需时间（秒）

    // 外部调用：开始睁眼动画（从当前值渐变到1）
    public override IEnumerator Effects()
    {
        if (blinkReceiver != null)
            yield return StartCoroutine(FadeOpenEyes(blinkReceiver.openEyesValue, 1f));
    }

    private IEnumerator FadeOpenEyes(float start, float end)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float value = Mathf.Lerp(start, end, t);
            blinkReceiver.openEyesValue = value;
            yield return null;
        }
        blinkReceiver.openEyesValue = end;
    }
}

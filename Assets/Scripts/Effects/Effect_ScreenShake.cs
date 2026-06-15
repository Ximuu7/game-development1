using System.Collections;
using UnityEngine;

public class Effect_ScreenShake: Effect
{
    public ScreenShakeReceiver receiver;
    public float duration=0f; // 晃动持续时间
    public float magnitude=0f; // 晃动幅度
    public override IEnumerator Effects()
    {
        yield return receiver.Shake(duration, magnitude);
       

    }
}
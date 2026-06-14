using System.Collections;
using UnityEngine;

public class Effect_ScreenShake: Effect
{
    public ScreenShakeReceiver receiver;
    public float duration=0; // 晃动持续时间
    public float magnitude=0; // 晃动幅度
    public IEnumerator Effects(float duration, float magnitude)
    {
            yield return receiver.Shake(duration, magnitude);
    }
}
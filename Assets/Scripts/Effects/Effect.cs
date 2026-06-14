using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public virtual IEnumerator Effects()
    {
        Debug.LogWarning("基类 Effect 未实现具体效果");
        yield break;
    }
}

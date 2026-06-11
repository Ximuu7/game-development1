using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public virtual IEnumerator Interactions()
    {
        Debug.LogWarning("基类 Interaction 未实现具体交互");
        yield break;
    }
}

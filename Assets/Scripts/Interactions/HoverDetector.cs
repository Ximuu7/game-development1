using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverDetector : MonoBehaviour
{
    void OnMouseEnter()
    {
        Debug.Log("鼠标进入物体");
    }
    void OnMouseExit()
    {
        Debug.Log("鼠标离开物体");
    }
}

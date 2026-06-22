using UnityEngine;

public class ColliderEvents : MonoBehaviour
{
    private SpriteOutline outlineController;
    private Coroutine currentOutlineCoroutine;

    void Awake()
    {
        outlineController = GetComponent<SpriteOutline>();
        if (outlineController == null)
            Debug.LogWarning("ColliderEvents: 未找到 SpriteOutline 组件");
    }

    void OnMouseDown()
    {
        Debug.Log("鼠标左键点击了书本");
        
    }

    void OnMouseEnter()
    {
        Debug.Log("鼠标悬停进入书本");
        // 取消之前的动画，启动新的显示动画
        if (currentOutlineCoroutine != null)
            StopCoroutine(currentOutlineCoroutine);
        currentOutlineCoroutine = StartCoroutine(outlineController.AnimateCoroutine(true, 0.5f));
    }

    void OnMouseExit()
    {
        Debug.Log("鼠标离开书本");
        if (currentOutlineCoroutine != null)
            StopCoroutine(currentOutlineCoroutine);
        currentOutlineCoroutine = StartCoroutine(outlineController.AnimateCoroutine(false, 0.5f));
    }
}
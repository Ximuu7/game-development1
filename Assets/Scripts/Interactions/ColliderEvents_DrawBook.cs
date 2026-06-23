using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEvents_DrawBook : MonoBehaviour
{
    private SpriteOutline outlineController;
    private Coroutine currentOutlineCoroutine;
    private ProcessController processController;
    private Interaction_DrawBook book;

    void Awake()
    {
        outlineController = GetComponent<SpriteOutline>();
        GameObject obj = GameObject.Find("Interaction_DrawBook");
        book = obj.GetComponent<Interaction_DrawBook>();
        if (outlineController == null)
            Debug.LogWarning("ColliderEvents: 未找到 SpriteOutline 组件");
    }

    void OnMouseDown()
    {
        Debug.Log("鼠标左键点击了书本");
        processController.Processor();
        book.clicked = true;

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

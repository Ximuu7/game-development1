using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Interaction_LimitedTimeToChoose : Interaction
{
    public GameObject countdownbar;
    public GameObject canvas_main;
    private Button[] buttons;
    private GameObject obj;
    
    public override IEnumerator Interactions()
    {
        obj = Instantiate(countdownbar, canvas_main.transform);
        Transform parent = GameObject.Find("Options").transform;
        buttons = parent.GetComponentsInChildren<Button>(false);
        for (int i = 0;i < buttons.Length;i++)
            buttons[i].onClick.AddListener(DestroyBar);
        yield return new WaitUntil(() =>obj == null);
        ClickButtonWithEffect(0);
    }

    private void DestroyBar()
    {
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].onClick.RemoveListener(DestroyBar);
        Debug.Log("销毁倒计时UI");
        Destroy(obj);
    }

    public void ClickButtonWithEffect(int buttonIndex)
    {
        if (buttons[buttonIndex] != null)
        {
            Button targetButton = buttons[buttonIndex];
            // 模拟鼠标点击：先按下去，再弹起
            ExecuteEvents.Execute(targetButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(targetButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(targetButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
    }
}

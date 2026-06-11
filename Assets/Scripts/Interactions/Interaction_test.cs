using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Interaction_test:Interaction
{
    public GameObject countdownbar;
    public GameObject canvas_main;
    private Button[] buttons;
    public override IEnumerator Interactions()
    {
        GameObject obj = Instantiate(countdownbar, canvas_main.transform);
        yield return new WaitUntil(()=>obj==null);
        Transform parent = GameObject.Find("Options").transform;
        buttons = parent.GetComponentsInChildren<Button>(false);
        ClickButtonWithEffect(0);
    }

    public void ClickButtonWithEffect(int buttonIndex)
    {
        Button targetButton = buttons[buttonIndex];
        // 模拟鼠标点击：先按下去，再弹起
        ExecuteEvents.Execute(targetButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(targetButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(targetButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction_End : Interaction
{
    public GameObject endButton;
    private Button button;
    public GameObject canvas_main;
    public ButtonManager manager;
    public ProcessController pc;
    private bool clicked=false;
    public override IEnumerator Interactions()
    {
        endButton=Instantiate(endButton,canvas_main.transform);
        button=endButton.GetComponent<Button>();
        button.onClick.AddListener(FunctionAndDestroy);
        yield return new WaitUntil(()=>clicked);

    }

    private void FunctionAndDestroy()
    {
        manager.OpenStart();
        manager.CloseGame();
        manager.CloseSettings();
        button.onClick.RemoveListener(FunctionAndDestroy);
        Destroy(button.gameObject);
        clicked = true;
        pc.processID = 0;

    }

}

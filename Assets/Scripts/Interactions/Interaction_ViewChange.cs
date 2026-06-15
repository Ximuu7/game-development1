using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interaction_Classroom : Interaction
{
    public Button arrow_up;
    public Button arrow_down;
    private Button up;
    private Button down;
    public Sprite desk;
    public Sprite classroom;
    public ProcessController pc;
    

    public override IEnumerator Interactions()
    {
        pc.allowuichange = false;
        pc.backgroundfade = true;
        pc.imagefadetime = 0.5f;
        ChangeBackGrounds();
        ShowArrows();
        yield return StartCoroutine(ShowInstruction());
        yield return new WaitForSeconds(5);
        pc.ShowUI();
        pc.allowuichange = true;
        Destroy(up.gameObject);
        Destroy(down.gameObject);
    }

    private void ChangeBackGrounds()
    {
        pc.backgrounds[0].sprite = desk;
        StartCoroutine(pc.ClearBackground(1));
    }
    private void ShowArrows()
    {
        up=Instantiate(arrow_up,pc.canvas_main.transform);
        up.transform.localPosition =new Vector3(0,100,0);
        up.onClick.AddListener(ButtonUp);
        down=Instantiate(arrow_down,pc.canvas_main.transform);
        down.gameObject.SetActive(false);
        down.transform.localPosition = new Vector3(0, -100, 0);
        
    }
    
    private IEnumerator ViewUp()
    {
        up.gameObject.SetActive(false);
        yield return StartCoroutine(pc.ClearBackground(0));
        yield return StartCoroutine(pc.ShowBackground("classroom,0"));
        down.gameObject.SetActive(true);
    }
    private IEnumerator ViewDown()
    {
        down.gameObject.SetActive(false);
        yield return StartCoroutine(pc.ClearBackground(0));
        yield return StartCoroutine(pc.ShowBackground("desk,0"));
        up.gameObject.SetActive(true);
    }

    public void ButtonUp()
    {
        down.onClick.AddListener(ButtonDown);
        up.onClick.RemoveListener(ButtonUp);
        StartCoroutine(ViewUp());
    }
    public void ButtonDown()
    {
        up.onClick.AddListener(ButtonUp);
        down.onClick.RemoveListener(ButtonDown);
        StartCoroutine(ViewDown());
    }

    private IEnumerator ShowInstruction()
    {
        GameObject obj = new GameObject("instruction");
        obj.transform.SetParent(pc.canvas_main.transform);
        obj.AddComponent<TextMeshPro>();
        yield return null;
    }



}

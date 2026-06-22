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
    private GameObject instruction1;
    private GameObject instruction2;
    private GameObject book;
    public float fontsize=12f;
    public Effect_SpriteOutline Outline;


    public override IEnumerator Interactions()
    {
        pc.allowuichange = false;
        pc.backgroundfade = true;
        pc.textfade = true;
        pc.imagefadetime = 0.5f;
        ChangeBackGrounds();
        ShowArrows();
        StartCoroutine(ShowBook());
        StartCoroutine(ShowInstruction_ChangeView());
        StartCoroutine(ShowInstruction_OpenBook());
        instruction1 = pc.gameobjects.Find(obj => obj.name == "instruction1");
        instruction2 = pc.gameobjects.Find(obj => obj.name == "instruction2");
        TMP_Text tmp1=instruction1.GetComponent<TMP_Text>();
        TMP_Text tmp2=instruction2.GetComponent<TMP_Text>();
        tmp1.fontSize=fontsize;
        tmp2.fontSize=fontsize;
        instruction2.SetActive(false);
        yield return new WaitForSeconds(5);
        pc.ShowUI();
        pc.allowuichange = true;
        Destroy(up.gameObject);
        Destroy(down.gameObject);
        Destroy(instruction1);
        Destroy(instruction2);
    }

    private void ChangeBackGrounds()
    {
        pc.backgrounds[0].sprite = desk;
        StartCoroutine(pc.ClearBackground(1)) ;
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
        instruction2.SetActive(false);
        up.gameObject.SetActive(false);
        yield return StartCoroutine(pc.ClearBackground(0));
        yield return StartCoroutine(pc.ShowBackground("classroom,0"));
        down.gameObject.SetActive(true);
        instruction1.SetActive(true);
    }
    private IEnumerator ViewDown()
    {
        instruction1.SetActive(false);
        down.gameObject.SetActive(false);
        yield return StartCoroutine(pc.ClearBackground(0));
        yield return StartCoroutine(pc.ShowBackground("desk,0"));
        up.gameObject.SetActive(true);
        instruction2.SetActive(true);
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

    private IEnumerator ShowInstruction_ChangeView()
    {
        StartCoroutine(pc.ShowText("instruction1", "点击按键切换视角",5.77f,-3.74f));
        yield return null;
    }

    private IEnumerator ShowInstruction_OpenBook()
    {
        StartCoroutine(pc.ShowText("instruction2", "点击课本开始早读", 5.77f, -3.74f));
        yield return null;
    }

    private IEnumerator ShowBook()
    {
        yield return StartCoroutine(pc.ShowSprite("father", 1f, 1f, 1f));
        book = pc.gameobjects.Find(obj => obj.name == "father");
        PolygonCollider2D collider = book.AddComponent<PolygonCollider2D>();
        collider.isTrigger = true;
        book.AddComponent<SpriteOutline>(); // 添加描边
        book.AddComponent<ColliderEvents>();
        Outline.receiver = book.GetComponent<SpriteOutline>();

        yield return null;
    }

}

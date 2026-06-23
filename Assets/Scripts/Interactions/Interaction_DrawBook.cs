using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction_DrawBook : Interaction
{
    public ProcessController pc;
    private GameObject book;
    public Effect_SpriteOutline Outline;
    public bool clicked;
    public override IEnumerator Interactions()
    {
        StartCoroutine(pc.HideUI());
        pc.allowuichange = false;
        yield return StartCoroutine(ShowBook());
        yield return new WaitUntil(()=>clicked);
        pc.Processor();
        pc.allowuichange = true;
       
    }

    private IEnumerator ShowBook()
    {
        yield return StartCoroutine(pc.ShowSprite("father", 1f, 1f, 1f));
        book=pc.gameobjects.Find(obj => obj.name == "father");
        PolygonCollider2D collider = book.AddComponent<PolygonCollider2D>();
        collider.isTrigger = true;
        book.AddComponent<SpriteOutline>(); // 添加描边
        book.AddComponent<ColliderEvents_DrawBook>();
        Outline.receiver = book.GetComponent<SpriteOutline>();

        yield return null;
    }








}

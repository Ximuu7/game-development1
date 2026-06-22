using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction_DrawBook : Interaction
{
    public ProcessController pc;
    private GameObject book;
    public Effect_SpriteOutline Outline;

    public override IEnumerator Interactions()
    {
        
        yield return StartCoroutine(ShowBook());
        yield return new WaitForSeconds(1);
       
    }

    private IEnumerator ShowBook()
    {
        yield return StartCoroutine(pc.ShowSprite("father", 1f, 1f, 1f));
        book=pc.gameobjects.Find(obj => obj.name == "father");
        PolygonCollider2D collider = book.AddComponent<PolygonCollider2D>();
        collider.isTrigger = true;
        book.AddComponent<SpriteOutline>(); // 添加描边
        book.AddComponent<ColliderEvents>();
        Outline.receiver = book.GetComponent<SpriteOutline>();

        yield return null;
    }








}

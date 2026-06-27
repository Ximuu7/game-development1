using UnityEngine;

public class Note : MonoBehaviour
{
    public float speed = 3f;
    public Transform parentObject;
    public RhythmGame rhythmGame;   // 由生成时赋值
    private bool isDestroyed = false;  // 防重复

    private void Awake()
    {
        parentObject = transform.parent.transform;
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (parentObject != null && !isDestroyed)
        {
            // 如果低于判定线超过1单位，视为 Miss
            if (transform.position.y < parentObject.position.y - 1.5f)
            {
                if (rhythmGame != null)
                    rhythmGame.NoteMissed();
                isDestroyed = true;
                Destroy(gameObject);
            }
        }
    }
}

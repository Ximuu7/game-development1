using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmGame : MonoBehaviour
{
    [Header("轨道设置")]
    public float noteSpeed = 3f;         // 下落速度（单位/秒）
    public float notedowntime = 3f;
    private float noteGeneratePlace;  // 音符在判定线上方多少单位生成

    [Header("判定阈值")]
    public float clickarea = 0.5f;

    [Header("预制体")]
    public List<GameObject> notePrefabs=new List<GameObject>();
    public Sprite clickeffect;  // 打击特效（播放后自动销毁）

    public List<GameObject> lines=new List<GameObject>();
    public float duration = 0.3f;

    public class NoteData
    {
        public int notetype;
        public float time;
        public int track;
    }

    public List<NoteData> notes = new List<NoteData>();

    public bool finished = false;
    public int count;
    public float effectScale = 0.12f;
    public AudioClip click;

    public void StartGame()
    {
        GameObject obj1 = GameObject.Find("Interaction2(Clone)");
        GameObject obj = obj1.transform.Find("lines").gameObject;
        lines.Add(obj.transform.Find("line1").gameObject);
        lines.Add(obj.transform.Find("line2").gameObject);
        lines.Add(obj.transform.Find("line3").gameObject);
        lines.Add(obj.transform.Find("line4").gameObject);
        notes.Add(new NoteData { notetype = 1, time = 0.8f, track = 1 });
        notes.Add(new NoteData { notetype = 2, time = 1.2f, track = 2 });
        notes.Add(new NoteData { notetype = 1, time = 1.6f, track = 3 });
        notes.Add(new NoteData { notetype = 2, time = 2.0f, track = 4 });
        notes.Add(new NoteData { notetype = 1, time = 2.4f, track = 1 });
        notes.Add(new NoteData { notetype = 2, time = 2.6f, track = 2 });
        notes.Add(new NoteData { notetype = 1, time = 3.0f, track = 3 });
        notes.Add(new NoteData { notetype = 2, time = 3.2f, track = 4 });
        notes.Add(new NoteData { notetype = 1, time = 3.6f, track = 1 });
        notes.Add(new NoteData { notetype = 2, time = 4.0f, track = 2 });
        notes.Add(new NoteData { notetype = 1, time = 4.4f, track = 3 });
        notes.Add(new NoteData { notetype = 2, time = 4.8f, track = 4 });
        notes.Add(new NoteData { notetype = 1, time = 5.2f, track = 1 });
        notes.Add(new NoteData { notetype = 2, time = 5.6f, track = 2 });
        notes.Add(new NoteData { notetype = 1, time = 6.0f, track = 3 });
        notes.Add(new NoteData { notetype = 2, time = 6.4f, track = 4 });
        notes.Add(new NoteData { notetype = 1, time = 6.8f, track = 1 });
        notes.Add(new NoteData { notetype = 2, time = 7.2f, track = 2 });
        notes.Add(new NoteData { notetype = 1, time = 7.6f, track = 3 });
        notes.Add(new NoteData { notetype = 2, time = 8.0f, track = 4 });
        noteGeneratePlace = 15f;
        count=notes.Count;
    }

    private IEnumerator GenerateNotes(int notetype, int track, float time)
    {
        yield return new WaitForSeconds(time);
        GameObject obj = Instantiate(notePrefabs[notetype - 1],
            new Vector3(lines[track - 1].transform.position.x,
                        lines[track - 1].transform.position.y + noteGeneratePlace,
                        lines[track - 1].transform.position.z - 1),
            Quaternion.identity,
            lines[track - 1].transform);

        Note note = obj.GetComponent<Note>();
        if (note != null)
        {
            note.rhythmGame = this;   // 传递引用
        }
        yield return null;
    }

    public IEnumerator Generator()
    {
        int index = 0;
        while (index < notes.Count)
        {
            StartCoroutine(GenerateNotes(notes[index].notetype, notes[index].track, notes[index].time)); 
            index++;
        }
        yield return null;
    }

    private void ClickCheck(int index)
    {
        float dist;
        Transform nearest = GetClosestChildByY(index, out dist);
        if (nearest != null && dist <= clickarea)
        {
            AudioSource.PlayClipAtPoint(click, Vector3.zero , 1f);
            Vector3 position = nearest.position;
            StartCoroutine(SpawnAndFadeCoroutine(position));
            Destroy(nearest.gameObject);
            count--;
        }
    }

    Transform GetClosestChildByY(int index, out float closestDist)
    {
        Transform parent = lines[index - 1].transform;
        Transform closest = null;
        float minDist = float.MaxValue;
        float parentY = parent.position.y;

        foreach (Transform child in parent)
        {
            float dist = Mathf.Abs(child.position.y - parentY);
            if (dist < minDist)
            {
                minDist = dist;
                closest = child;
            }
        }
        closestDist = minDist;
        return closest;
    }

    IEnumerator SpawnAndFadeCoroutine(Vector3 position)
    {
        GameObject obj = new GameObject("FadingSprite");
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * effectScale;  // 控制大小
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = clickeffect;
        sr.color = new Color(1f, 1f, 1f, 1f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            sr.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        sr.color = new Color(1f, 1f, 1f, 0f);
        Destroy(obj);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) ClickCheck(1);
        if (Input.GetKeyDown(KeyCode.S)) ClickCheck(2);
        if (Input.GetKeyDown(KeyCode.D)) ClickCheck(3);
        if (Input.GetKeyDown(KeyCode.F)) ClickCheck(4);
    }

    public void NoteMissed()
    {
        count--;
        // 可选：Debug.Log("Miss!");
    }
}

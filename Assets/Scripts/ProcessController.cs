using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class ProcessController : MonoBehaviour
{
    public int processID=0;
    public TextAsset jsonfile;
    public TMP_FontAsset fontAsset;
    public Canvas canvas_main;
    public Button button_continue;
    public Camera mainCamera;
    public Button option; // 选项按钮预制体
    public Transform optiongroup; // 选项按钮的父物体
    public GameObject roles;
    public bool allowuichange = true;
    private ButtonManager buttonmanager;
    [System.Serializable]
    public class Dialogrows
    {
        public int process_ID; // 进程序号，从0开始递增
        public string command_before; // 进程开始前的命令
        public string dialog; // 对话文本
        public string image;// 角色图像
        public string background;//背景图像
        public string audio;// 音频
        public string command_after; // 命令
        public int process_next; // 下一个进程ID
    }//数据结构
    #region 状态检测
    public bool allowtoskip=false;
    private bool isoption = false;//是否在选项环节
    private bool ininteraction = false;
    private bool inoption = false;
    [HideInInspector] public bool imagefade = false;//是否在进行图像渐变
    [HideInInspector] public bool textfade = false;//tmp渐变
    [HideInInspector] public bool backgroundfade = false;//是否在进行背景渐变
    [HideInInspector] public bool audiofade = false;//是否在进行音频渐变
    [HideInInspector] public bool uifade = false;//是否在进行UI渐变
    private bool imagefadefinished = true;
    private bool backgroundfadefinished = true;
    private bool audiofadefinished = true;
    private bool textfadefinished=true;

    public float imagefadetime=1f;
    public float backgroundfadetime=1f;
    public float audiofadetime=1f;
    public float textfadetime=0.1f;
    #endregion
    #region Dictionaries
    public Dictionary<string,Sprite> Dic_Name_Image = new Dictionary<string,Sprite>();
    public Dictionary<string,Sprite> Dic_Name_Background = new Dictionary<string,Sprite>();
    public Dictionary<string,AudioSource> Dic_Name_Audio = new Dictionary<string, AudioSource>();
    public Dictionary<string,AnimationClip> Dic_Name_Animation = new Dictionary<string, AnimationClip>();
    public Dictionary<string,Interaction> Dic_Name_Interaction = new Dictionary<string,Interaction>();
    public Dictionary<string,Effect> Dic_Name_Effect = new Dictionary<string, Effect>();
    #endregion

    public TMP_Text dialogtext;//对话文本组件
    public List<Dialogrows> dialogrows=new List<Dialogrows>();//对话数据
    public List<Sprite> sprite_roles;//角色图像
    public List<GameObject> gameobjects=new List<GameObject>();//临时生成的物体
    public List<Image> backgrounds=new List<Image>();//背景
    public List<Sprite> sprite_backgrounds;//背景图像
    public List<SpriteRenderer> spriterenderers = new List<SpriteRenderer>();  // 用于显示图像的SpriteRenderer组件
    public List<AudioSource> audiosources = new List<AudioSource>();// 音频
    public List<AnimationClip> animationclips = new List<AnimationClip>();// 动画
    public List<Interaction> interactions = new List<Interaction>();// 交互
    public List<Effect> effects = new List<Effect>();// 效果



    private void ReadDialog(string str)
    {
        jsonfile = Resources.Load<TextAsset>(str);
        dialogrows= JsonConvert.DeserializeObject<List<Dialogrows>>(jsonfile.text);
    }//读取文件
    #region 文本方法
    public IEnumerator ShowText(string dialog)
    {
        if (dialog == "")
        {
            yield break;
        }
        dialogtext.text = dialog;
        if (textfade)
        {
            StartCoroutine(FadeInText(dialogtext.gameObject.GetComponent<TextMeshProUGUI>(),textfadetime));
        }
        yield return null;
    }//显示对话文本
    public IEnumerator ShowText(string text_name,string text,float x,float y)
    {
        if (text == "" || text_name == "")
        {
            yield break;
        }
        GameObject textt = new GameObject(text_name);
        gameobjects.Add(textt);
        textt.transform.SetParent(roles.transform, false);
        textt.transform.position=new Vector3(x,y,100);
        TextMeshPro tmp= textt.AddComponent<TextMeshPro>();
        tmp.font = fontAsset;
        tmp.text = text;
        if (textfade)
        {
            StartCoroutine(FadeInText((TextMeshPro)tmp, textfadetime)); 
        }
        yield return null;
    }//创建物体显示文本
    public IEnumerator ChangeTextColor(TMP_Text text, Color from, Color to, float duration)
    {
        textfadefinished = false;
        text.color = from;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            text.color = Color.Lerp(from, to, t);
            yield return null;
        }
        text.color = to;
        textfadefinished = true;
    }//改变文本颜色

    public IEnumerator FadeInText(TMP_Text text, float duration)
    {
        Color from = new Color(text.color.r, text.color.g, text.color.b, 0f);
        Color to = new Color(text.color.r, text.color.g, text.color.b, 1f);
        yield return StartCoroutine(ChangeTextColor(text, from, to, duration));
    }//淡入文本
    public IEnumerator FadeOutText(TMP_Text text, float duration)
    {
        Color from = new Color(text.color.r, text.color.g, text.color.b, 1f);
        Color to = new Color(text.color.r, text.color.g, text.color.b, 0f);
        yield return StartCoroutine(ChangeTextColor(text, from, to, duration));
    }//淡出文本
    public IEnumerator ClearText(string text)
    {
        if (text == "")
        {
            yield break;
        }
        if (textfade)
        {
            yield return StartCoroutine(FadeOutText(dialogtext, textfadetime));
        }
        dialogtext.text = "";
    }//清除对话文本
    public IEnumerator ClearText(TMP_Text text_name,string text)
    {
        if(text == "" || text_name == null)
        {
            yield break;
        }
        if (textfade)
        {
            yield return StartCoroutine(FadeOutText(dialogtext, textfadetime));
        }
        Destroy(text_name.gameObject); 

    }//清除程序创建的文本
    #endregion
    #region Sprite方法
    public IEnumerator ShowSprite(string sprite_name,int sprite_position,float sprite_size)
    {
        Sprite sprite = Dic_Name_Image[sprite_name];
        spriterenderers[sprite_position].sprite = sprite;
        Camera camera = Camera.main;
        float screenY = camera.orthographicSize * 2f;
        float screenX = screenY * camera.aspect;
        float spriteY = spriterenderers[sprite_position].sprite.bounds.size.y;
        float spriteX = spriterenderers[sprite_position].sprite.bounds.size.x;
        float scaleX = screenX / spriteX;
        float scaleY = screenY / spriteY;
        float uniformScale = sprite_size * Mathf.Min(scaleX, scaleY);

        spriterenderers[sprite_position].transform.localScale = new Vector3(uniformScale, uniformScale, 1f);
        yield return null;

    }//用位置显示图像
    public IEnumerator ShowSprite(string sprite_name,float spite_positionX,float sprite_positionY,float sprite_size)
    {
        Sprite sprite = Dic_Name_Image[sprite_name];
        GameObject spritee=new GameObject(sprite.name);
        gameobjects.Add(spritee);
        spritee.transform.SetParent(roles.transform);
        spritee.transform.position=new Vector3(spite_positionX,sprite_positionY,100);
        SpriteRenderer spriterenderer=spritee.AddComponent<SpriteRenderer>();
        spriterenderer.sprite = sprite;
        Camera camera = Camera.main;
        float screenY = camera.orthographicSize * 2f;
        float screenX = screenY * camera.aspect;
        float spriteY = spriterenderer.sprite.bounds.size.y;
        float spriteX = spriterenderer.sprite.bounds.size.x;
        float scaleX = screenX / spriteX;
        float scaleY = screenY / spriteY;
        float uniformScale = sprite_size * Mathf.Min(scaleX, scaleY);

        spriterenderer.transform.localScale = new Vector3(uniformScale, uniformScale, 1f);
        yield return null;

    }//用坐标显示图像
    public IEnumerator ShowSprite(int index)
    {
        string image = dialogrows[index].image;
        if (image == "")
        {
            yield break;
        }
        string[] commands = image.Split(';');
        for (int i = 0; i < commands.Length; i++)
        {
            string[] singlecommand = commands[i].Split(',');
            if (singlecommand.Length == 4)
            {
                StartCoroutine(ShowSprite(singlecommand[0], float.Parse(singlecommand[1]), float.Parse(singlecommand[2]), float.Parse(singlecommand[3])));
                if (imagefade)
                {
                    GameObject target = gameobjects.Find(obj => obj.name == singlecommand[0]);
                    SpriteRenderer spriterenderer = target.GetComponent<SpriteRenderer>();
                    StartCoroutine(FadeInSprite(spriterenderer, imagefadetime));
                    
                }
            }
            if (singlecommand.Length == 3)
            {
                StartCoroutine(ShowSprite(singlecommand[0], int.Parse(singlecommand[1]), float.Parse(singlecommand[2])));
                if (imagefade)
                {
                    StartCoroutine(FadeInSprite(spriterenderers[int.Parse(singlecommand[1])], imagefadetime));
                }
            }
        }
        yield return null;
    }//根据文件显示图像
    private IEnumerator ChangeSpriteColor(SpriteRenderer spriteRenderer, Color from, Color to, float duration)
    {
        imagefadefinished = false;
        spriteRenderer.color = from;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            spriteRenderer.color = Color.Lerp(from, to, t);
            yield return null;
        }
        spriteRenderer.color = to;
        imagefadefinished = true;
    }//改变sprite颜色
    public IEnumerator FadeInSprite(SpriteRenderer spriteRenderer, float duration)
    {
        Color from = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        Color to = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        yield return StartCoroutine(ChangeSpriteColor(spriteRenderer, from, to, duration));
    }//Sprite淡入
    public IEnumerator FadeOutSprite(SpriteRenderer spriteRenderer, float duration)
    {
        Color from = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        Color to = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        yield return StartCoroutine(ChangeSpriteColor(spriteRenderer, from, to, duration));
    }//Sprite淡出
    public IEnumerator ClearSprite(int sprite_position)
    {
        if (imagefade)
        {
            SpriteRenderer spriterenderer = spriterenderers[sprite_position];
            yield return StartCoroutine(FadeOutSprite(spriterenderer, imagefadetime));
        }
        spriterenderers[sprite_position].sprite = null;
        Debug.Log("Cleared image at position: " + sprite_position);
        yield return null;
    }//清除图像
    public IEnumerator ClearSprite(string sprite_name)
    {
        GameObject target = gameobjects.Find(obj => obj.name == sprite_name);
        if (target == null)
        {
            Debug.Log("需要清除的图像不存在" + sprite_name);
            yield break;
        }
        SpriteRenderer spriterenderer = target.GetComponent<SpriteRenderer>();
        if (imagefade)
        {
            yield return StartCoroutine(FadeOutSprite(spriterenderer, imagefadetime));
        }
        Destroy(target);
        yield return null;
    }//清除用坐标显示的图像
    #endregion
    #region Image方法
    public IEnumerator ShowBackground(string background)
    {
        
        if(background=="")
        {
            yield break;
        }
        string[] commands = background.Split(';');
        for (int i = 0; i < commands.Length; i++)
        {
            string[] singlecommand = commands[i].Split(',');
            Sprite sprite = Dic_Name_Background[singlecommand[0]];
            int index=int.Parse(singlecommand[1]);
            backgrounds[index].sprite = sprite;
            if (backgroundfade)
            {
                StartCoroutine(FadeInImage(backgrounds[index], backgroundfadetime));
            }
            yield return null;
        }
        
    }//显示背景
    private IEnumerator ChangeImageColor(Image image, Color from, Color to, float duration)
    {
        backgroundfadefinished = false;
        image.color = from;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            image.color = Color.Lerp(from, to, t);
            yield return null;

        }
        image.color = to;
        backgroundfadefinished = true;
    }//改变Image颜色   
    public IEnumerator FadeInImage(Image image, float duration)
    {
        Color from = new Color(image.color.r, image.color.g, image.color.b, 0f);
        Color to = new Color(image.color.r, image.color.g, image.color.b, 1f);
        yield return StartCoroutine(ChangeImageColor(image, from, to, duration));
    }//图像淡入
    public IEnumerator FadeOutImage(Image image, float duration)
    {
        Color from = new Color(image.color.r, image.color.g, image.color.b, 1f);
        Color to = new Color(image.color.r, image.color.g, image.color.b, 0f);
        yield return StartCoroutine(ChangeImageColor(image, from, to, duration));
    }//图像淡出
    public IEnumerator ClearBackground(int index)
    {
        if (backgroundfade)
        {
            yield return StartCoroutine(FadeOutImage(backgrounds[index], imagefadetime));    
        }
        backgrounds[index].sprite = null;
        yield return null;



    }//清除背景
    #endregion
    #region 音频方法
    public IEnumerator PlayAudio(string audio_name)
    {
        if (audio_name == "")
        {
            yield break;
        }
        string[] commands = audio_name.Split(';');
        for (int i = 0; i < commands.Length; i++)
        {
            AudioSource source = Dic_Name_Audio[audio_name];
            if (source != null)
            {
                source.Play();
            }
            else
            {
                Debug.LogWarning("音频不存在" + audio_name);
            }
            if (audiofade)
            {
                StartCoroutine(FadeInAudio(audio_name, audiofadetime));
            }
        }
        yield return null;
    }//播放音频
    public IEnumerator StopPlayAudio(string audio_name)
    {
        AudioSource source = Dic_Name_Audio[audio_name];

        if (source != null)
        {
            if (audiofade)
            {
                StartCoroutine(FadeOutAudio(audio_name, audiofadetime));
            }
            source.Stop();
        }
        else
        {
            Debug.LogWarning("音频不存在" + audio_name);
        }//停止播放音频
        yield return null;
    }//停止播放音频
    private IEnumerator FadeVolume(string audio_name, float startVolume, float targetVolume, float duration)
    {
        audiofadefinished = false;
        AudioSource source = Dic_Name_Audio[audio_name];
        float startVol = source.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }
        source.volume = targetVolume;
        audiofadefinished=true;

    }//音频响度渐变
    private IEnumerator FadeInAudio(string audio_name, float duration)
    {
        AudioSource source = Dic_Name_Audio[audio_name];
        source.volume = 0f;
        source.Play();
        yield return StartCoroutine(FadeVolume(audio_name, 0f, 1f, duration));
    }//音频淡入
    private IEnumerator FadeOutAudio(string audio_name, float duration)
    {
        AudioSource source = Dic_Name_Audio[audio_name];
        yield return StartCoroutine(FadeVolume(audio_name, source.volume, 0f, duration));
    }//音频淡出
    #endregion
    private void ShowOptions(int process_ID)
    {
        string[] commands = dialogrows[process_ID].command_before.Split(';');
        for (int i = 0; i < commands.Length; i++)
        {
            string[] singlecommand = commands[i].Split(',');
            if (singlecommand[0] == "option")
            {
                isoption = true;
            }
        }
        if (isoption)
        {
            GameObject button = Instantiate(option.gameObject, optiongroup);
            Debug.Log("generated option");
            button.GetComponentInChildren<TMP_Text>().text = dialogrows[process_ID].dialog;

            button.GetComponent<Button>().onClick.AddListener
            (delegate
            {
                OnOptionClick(dialogrows[process_ID].process_next);
            });
            isoption = false;
            ShowOptions(process_ID + 1);
        }
    }//显示选项
    private void ShowAnimation(string animation_name)
    {
        Debug.Log("Playing animation: " + animation_name);
    }//显示动画
    private void StartInteraction(string interaction_name)
    {
        Interaction interaction = Dic_Name_Interaction[interaction_name];
        StartCoroutine(Interaction(interaction));

    }//开始交互环节
    IEnumerator Interaction(Interaction interaction)
    {
        ininteraction = true;
        Debug.Log("interaction");
        interaction.gameObject.SetActive(true);
        yield return interaction.Interactions();
        interaction.gameObject.SetActive(false);
        ininteraction = false;
        button_continue.gameObject.SetActive(true);
    }//交互协程
    private void ShowEffect(string effect_name)
    {
        Effect effect = Dic_Name_Effect[effect_name];
        StartCoroutine(Effect(effect));
    }//显示效果
    private IEnumerator Effect(Effect effect)
    {
        effect.gameObject.SetActive(true);
        yield return effect.Effects();
        effect.gameObject.SetActive(false);
    }
    public IEnumerator HideUI()
    {
        Transform a = canvas_main.transform.Find("dialogwindow");
        Transform b = canvas_main.transform.Find("Button_Setting");
        Transform c = canvas_main.transform.Find("Button_Continue");
        if(uifade)
        {
            StartCoroutine(FadeOutImage(a.GetComponent<Image>(), imagefadetime));
            StartCoroutine(FadeOutImage(b.GetComponent<Image>(), imagefadetime));
            StartCoroutine(FadeOutText(b.gameObject.GetComponentInChildren<TextMeshProUGUI>(), imagefadetime));
            StartCoroutine(FadeOutImage(c.GetComponent<Image>(), imagefadetime));
        }
        a.gameObject.SetActive(false);
        b.gameObject.SetActive(false);
        c.gameObject.SetActive(false);
        yield return null;


    }//隐藏UI
    public IEnumerator ShowUI()
    {
        Transform a = canvas_main.transform.Find("dialogwindow");
        Transform b = canvas_main.transform.Find("Button_Setting");
        Transform c = canvas_main.transform.Find("Button_Continue");
        a.gameObject.SetActive(true);
        b.gameObject.SetActive(true);
        c.gameObject.SetActive(true);
        if (uifade)
        {
            StartCoroutine(FadeInImage(a.GetComponent<Image>(), imagefadetime));
            StartCoroutine(FadeInImage(b.GetComponent<Image>(), imagefadetime));
            StartCoroutine(FadeInImage(c.GetComponent<Image>(), imagefadetime));
        }
        else
        {
            Image aa = a.GetComponent<Image>();
            Image bb = b.GetComponent<Image>();
            Image cc = c.GetComponent<Image>();
            aa.color = new Color(aa.color.r, aa.color.g, aa.color.b, 1f);
            bb.color = new Color(bb.color.r, bb.color.g, bb.color.b, 1f);
            cc.color = new Color(cc.color.r, cc.color.g, cc.color.b, 1f);
        }
            yield return null;
    }//显示UI
    public void HideContinueButton()
    {
        button_continue.gameObject.SetActive(false);
        
    }//隐藏继续按钮
    public void ShowContinueButton()
    {
        button_continue.gameObject.SetActive(true);
    }//显示继续按钮
    private IEnumerator processfinishedchecker()
    {
       yield return new WaitUntil(() => textfadefinished && backgroundfadefinished && audiofadefinished && imagefadefinished);
    }//进程完成检查
    private void CommandReader(string command)
    {
        if (command != "")
        {
            Debug.Log("Reading command: " + command);
            string[] commands = command.Split(';');
            for (int i = 0; i < commands.Length; i++)
            {
                string[] singlecommand = commands[i].Split(',');
                if (singlecommand[0] == "imagefade")
                {
                    imagefade = true;
                }
                if (singlecommand[0] == "imageflash")
                {
                    imagefade = false;
                }
                if (singlecommand[0] == "backgroundfade")
                {
                    backgroundfade = true;
                }
                if (singlecommand[0] == "backgroundflash")
                {
                    backgroundfade = false;
                }
                if (singlecommand[0] == "audiofade")
                {
                    audiofade = true;
                }
                if (singlecommand[0] == "audiofalsh")
                {
                    audiofade = false;
                }
                if (singlecommand[0] == "uifade")
                {
                    uifade = true;
                }
                if (singlecommand[0] == "uiflash")
                {
                    uifade = false;
                }
                if (singlecommand[0] == "imagefadetime")
                {
                    imagefadetime = float.Parse(singlecommand[1]);
                }
                if (singlecommand[0] == "backgroundfadetime")
                {
                    backgroundfadetime = float.Parse(singlecommand[1]);
                }
                if (singlecommand[0] == "audiofadetime")
                {
                    audiofadetime = float.Parse(singlecommand[1]);
                }
                if (singlecommand[0] == "textfadetime")
                {
                    textfadetime = float.Parse(singlecommand[1]);
                }
                if (singlecommand[0] == "clearimage")
                {
                    if (int.TryParse(singlecommand[1], out int value))
                    {
                        StartCoroutine(ClearSprite(value));
                    }
                    else
                    {
                        StartCoroutine(ClearSprite(singlecommand[1]));
                    }
                }
                if (singlecommand[0] == "stopaudio")
                {
                    StartCoroutine(StopPlayAudio(singlecommand[1]));
                }
                if (singlecommand[0] == "hideui")
                {
                    StartCoroutine(HideUI());
                }
                if (singlecommand[0] == "showui")
                {
                    StartCoroutine(ShowUI());
                }
                if (singlecommand[0] == "skip")
                {
                    allowtoskip = true;
                }
                if (singlecommand[0] == "noskip")
                {
                    allowtoskip=false;
                }
                if (singlecommand[0] == "option")
                {
                    isoption = true;
                }
                if (singlecommand[0] == "effect")
                {
                    ShowEffect(singlecommand[1]);
                    
                }
                if (singlecommand[0] == "interaction")
                {
                    StartInteraction(singlecommand[1]);
                }
                if (singlecommand[0] == "continue")
                {
                    Processor(processID);
                }
                
            }
        }
    }//命令读取
    public void Processor()
    {
        StartCoroutine(Processor(processID));
    }//进程控制器
    private IEnumerator Processor(int process_ID)//进程控制器
    {
        Debug.Log("processID=" + processID);
        gameobjects.RemoveAll(obj => obj == null);
        processID = dialogrows[process_ID].process_next;
        inoption = false;
        ininteraction = false;
        if (!allowtoskip)
        {
            HideContinueButton();
        }
        CommandReader(dialogrows[process_ID].command_before);    
        if(isoption)
        {
            ShowOptions(process_ID);
            inoption=true;
        }
        else
        {
            StartCoroutine(Text(process_ID));
            StartCoroutine(Image(process_ID));
            StartCoroutine(Background(process_ID));
            StartCoroutine(Audio(process_ID));
        }
        yield return StartCoroutine(processfinishedchecker());
        if (!inoption&&!ininteraction)
        {
            ShowContinueButton();
        }
        Debug.Log("over");
        CommandReader(dialogrows[process_ID].command_after);
        isoption = false;

    }

    private IEnumerator Text(int index)
    {
        if (dialogrows[index].dialog != "")
        {
            
            ShowText(dialogrows[index].dialog);
            yield return null;
            
        }
    }
    private IEnumerator Image(int index)
    {
        if (dialogrows[index].image != null)
        {

            StartCoroutine(ShowSprite(index));
            if (!allowtoskip)
            {
                yield return new WaitWhile(() => imagefadefinished);
            }
            yield return null;
            
        }
    }
    private IEnumerator Background(int index)
    {
        if (dialogrows[index].background != null)
        {

            StartCoroutine(ShowBackground(dialogrows[index].background));
            if (!allowtoskip)
            {
                yield return new WaitWhile(() => backgroundfadefinished);
            }
            yield return null;
            
        }
    }
    private IEnumerator Audio(int index)
    {
        if (dialogrows[index].audio != "")
        {

            StartCoroutine(PlayAudio(dialogrows[index].audio));
            if (!allowtoskip)
            {
                yield return new WaitWhile(() => audiofadefinished);
            }
            yield return null;
            
        }
    }

    //*************************以下是按钮功能***************************//
    private void OnOptionClick(int index)
    {
        processID = index;
        Processor(processID);
        PlayAudio("click1");
        for (int i = 0; i < optiongroup.childCount; i++)
        {
            Destroy(optiongroup.GetChild(i).gameObject);
            Debug.Log("Destroyed option button");
        }
        button_continue.gameObject.SetActive(true);
    }//点击选项按钮
    public void ChangeState_MainUI()
    {
        if ((UnityEngine.Input.GetMouseButtonDown(1) || UnityEngine.Input.GetKeyDown(KeyCode.Escape)) && canvas_main.gameObject.activeSelf)
        {
            canvas_main.gameObject.SetActive(false);
        }
        else if ((UnityEngine.Input.GetMouseButtonDown(1) || UnityEngine.Input.GetMouseButtonDown(0) || UnityEngine.Input.GetKeyDown(KeyCode.Escape)) && !canvas_main.gameObject.activeSelf)
        {
            canvas_main.gameObject.SetActive(true);
        }
    }//隐藏文本框和UI


    private void Update()
    {
        if (buttonmanager.isgaming && allowuichange)
        {
            ChangeState_MainUI();
        }
    }

    private void Awake()
    {
        ReadDialog("storyline");
        #region 初始化图像字典
        Dic_Name_Image["classmate1"]= sprite_roles[0];
        Dic_Name_Image["classmate2"] = sprite_roles[1];
        Dic_Name_Image["classmates"] = sprite_roles[2];
        Dic_Name_Image["father"] = sprite_roles[3];
        Dic_Name_Image["friend"] = sprite_roles[4];
        Dic_Name_Image["houseparent"] = sprite_roles[5];
        Dic_Name_Image["mother"] = sprite_roles[6];
        Dic_Name_Image["relative1"] = sprite_roles[7];
        Dic_Name_Image["relative2"] = sprite_roles[8];
        Dic_Name_Image["relative3"] = sprite_roles[9];
        Dic_Name_Image["teacher_mouth_close"] = sprite_roles[10];
        Dic_Name_Image["teacher_mouth_open"] = sprite_roles[11];
        Dic_Name_Image["blackboard"] = sprite_roles[12];
        Dic_Name_Image["drawbook"] = sprite_roles[13];
        Dic_Name_Image["phone"] = sprite_roles[11];
        #endregion
        #region 初始化背景字典
        Dic_Name_Background["classroom"] = sprite_backgrounds[0];
        Dic_Name_Background["desk"] = sprite_backgrounds[1];
        Dic_Name_Background["dormitory"] = sprite_backgrounds[2];
        Dic_Name_Background["bookshelf"] = sprite_backgrounds[3];
        Dic_Name_Background["livingroom"] = sprite_backgrounds[4];
        Dic_Name_Background["office"] = sprite_backgrounds[5];
        #endregion
        #region 初始化音频字典
        Dic_Name_Audio["click1"]= audiosources[0];
        Dic_Name_Audio["bgm1"] = audiosources[1];
        #endregion
        #region 初始化交互
        Dic_Name_Interaction["LimitedTimeToChoose"] = interactions[0];
        Dic_Name_Interaction["viewchange"] = interactions[1];
        Dic_Name_Interaction["DrawBook"]=interactions[2];
        #endregion
        #region 初始化效果
        Dic_Name_Effect["ScreenShake"] = effects[0];
        Dic_Name_Effect["Blink"]=effects[1];
        Dic_Name_Effect["Outline"] = effects[2];
        #endregion
        Debug.Log("Awake finished");
    }

    private void Start()
    {
        buttonmanager = FindObjectOfType<ButtonManager>();
    }



}

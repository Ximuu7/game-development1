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
using static Unity.VisualScripting.Member;

public class ProcessController : MonoBehaviour
{
    public int processID;
    public TextAsset jsonfile;
    public Canvas canvas_main;
    public Button button_continue;
    public Camera mainCamera;
    public Button option; // 选项按钮预制体
    public Transform optiongroup; // 选项按钮的父物体
    public string command_type;
    public string command_content;
    public Image background;
    public GameObject roles;

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

    private bool isoption = false;//是否在选项环节
    private bool isinteraction = false;//是否在交互环节
    private bool imagefade = false;//是否在进行图像渐变
    private bool backgroundfade = false;//是否在进行背景渐变
    private bool audiofade = false;//是否在进行音频渐变
    private bool uifade = false;//是否在进行UI渐变

    public Dictionary<string,Sprite> Dic_Name_Image = new Dictionary<string,Sprite>();
    public Dictionary<string,Sprite> Dic_Name_Background = new Dictionary<string,Sprite>();
    public Dictionary<string,AudioSource> Dic_Name_Audio = new Dictionary<string, AudioSource>();
    public Dictionary<string,AnimationClip> Dic_Name_Animation = new Dictionary<string, AnimationClip>();
    public Dictionary<string,Interaction> Dic_Name_Interaction = new Dictionary<string,Interaction>();
    public Dictionary<string,Effect> Dic_Name_Effect = new Dictionary<string, Effect>();

    public TMP_Text dialogtext;//对话文本组件
    public List<Dialogrows> dialogrows=new List<Dialogrows>();//对话数据
    public List<Sprite> sprite_roles;//角色图像
    public List<GameObject> gameobjects=new List<GameObject>();//临时生成的物体
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
    private void ShowText(string dialog)
    {
        if (dialog == "")
        {
            return;
        }
        dialogtext.text = dialog;
    }//显示文本
    #region 图像的函数
    private void ShowImage(string image_name,int image_position,float image_size)
    {
        Sprite sprite = Dic_Name_Image[image_name];
        spriterenderers[image_position].sprite = sprite;
        Camera camera = Camera.main;
        float screenY = camera.orthographicSize * 2f;
        float screenX = screenY * camera.aspect;
        float spriteY = spriterenderers[image_position].sprite.bounds.size.y;
        float spriteX = spriterenderers[image_position].sprite.bounds.size.x;
        float scaleX = screenX / spriteX;
        float scaleY = screenY / spriteY;
        float uniformScale = image_size * Mathf.Min(scaleX, scaleY);

        spriterenderers[image_position].transform.localScale = new Vector3(uniformScale, uniformScale, 1f);

    }//用位置显示图像
    private void ShowImage(string image_name,float image_positionX,float image_positionY,float image_size)
    {
        Sprite sprite = Dic_Name_Image[image_name];
        GameObject obj=new GameObject(sprite.name);
        gameobjects.Add(obj);
        obj.transform.SetParent(roles.transform);
        obj.transform.position=new Vector3(image_positionX,image_positionY,100);
        SpriteRenderer spriterenderer=obj.AddComponent<SpriteRenderer>();
        spriterenderer.sprite = sprite;
        Camera camera = Camera.main;
        float screenY = camera.orthographicSize * 2f;
        float screenX = screenY * camera.aspect;
        float spriteY = spriterenderer.sprite.bounds.size.y;
        float spriteX = spriterenderer.sprite.bounds.size.x;
        float scaleX = screenX / spriteX;
        float scaleY = screenY / spriteY;
        float uniformScale = image_size * Mathf.Min(scaleX, scaleY);

        spriterenderer.transform.localScale = new Vector3(uniformScale, uniformScale, 1f);

    }//用坐标显示图像
    private void ShowImage(int index)
    {
        string image = dialogrows[index].image;
        if (image == "")
        {
            return;
        }
        string[] commands = image.Split(';');
        for (int i = 0; i < commands.Length; i++)
        {
            string[] singlecommand = commands[i].Split(',');
            if (singlecommand.Length == 4)
            {
                ShowImage(singlecommand[0], float.Parse(singlecommand[1]), float.Parse(singlecommand[2]), float.Parse(singlecommand[3]));
            }
            if (singlecommand.Length == 3)
            {
                ShowImage(singlecommand[0], int.Parse(singlecommand[1]), float.Parse(singlecommand[2]));
            }
            if (imagefade)
            {
                StartCoroutine(FadeInSprite(spriterenderers[int.Parse(singlecommand[1])], 1f));
            }
        }
    }//显示图像

    private void ShowBackground(string image_background)
    {
        if(image_background=="")
        {
            return;
        }
        Sprite sprite = Dic_Name_Background[image_background];
        background.sprite = sprite;
    }//显示背景

    private void ChangeImageColor(Image image, Color from, Color to, float duration)
    {
        image.color = from;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            image.color = Color.Lerp(from, to, t);
            
        }
        image.color = to;
    }//改变Image颜色
    private IEnumerator ChangeSpriteColor(SpriteRenderer spriteRenderer, Color from, Color to, float duration)
    {
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
    }
    private void FadeInImage(string image_name, float duration)
    {
        Image image = Dic_Name_Image[image_name].GetComponent<Image>();
        Color from = new Color(image.color.r, image.color.g, image.color.b, 0f);
        Color to = new Color(image.color.r, image.color.g, image.color.b, 1f);
        ChangeImageColor(image, from, to, duration);
    }//图像淡入
    private void FadeOutImage(string image_name, float duration)
    {
        Image image = Dic_Name_Image[image_name].GetComponent<Image>();
        Color from = new Color(image.color.r, image.color.g, image.color.b, 1f);
        Color to = new Color(image.color.r, image.color.g, image.color.b, 0f);
        ChangeImageColor(image, from, to, duration);
    }//图像淡出
    private IEnumerator FadeInSprite(SpriteRenderer spriteRenderer, float duration)
    {
        Color from = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        Color to = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        yield return StartCoroutine(ChangeSpriteColor(spriteRenderer, from, to, duration));
    }//Sprite淡入
    private IEnumerator FadeOutSprite(SpriteRenderer spriteRenderer, float duration)
    {
        Color from = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        Color to = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        yield return StartCoroutine(ChangeSpriteColor(spriteRenderer, from, to, duration));
    }//Sprite淡出

    private void ClearImage(int image_position)
    {
        spriterenderers[image_position].sprite = null;
        Debug.Log("Cleared image at position: " + image_position);
    }//清除图像
    private void ClearImage(string image_name)
    {
        GameObject target = gameobjects.Find(obj => obj.name == image_name);
        Destroy(target);
    }//清除用坐标显示的图像
    #endregion
    #region 音频的函数
    private void PlayAudio(string audio_name)
    {
        if (audio_name == "")
        {
            return;
        }
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
            StartCoroutine(FadeInAudio(audio_name, 1f));
        }
    }//播放音频
    private void StopPlayAudio(string audio_name)
    {
        AudioSource source = Dic_Name_Audio[audio_name];
        if (source != null)
        {
            source.Stop();
        }
        else
        {
            Debug.LogWarning("音频不存在" + audio_name);
        }//停止播放音频
    }//停止播放音频
    private IEnumerator FadeVolume(string audio_name, float startVolume, float targetVolume, float duration)
    {
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
        source.Stop();
    }//音频淡出
    #endregion
    /*private void ShowDialog(int process_ID)
    {
        if(dialogrows[process_ID].dialog!="")
            ShowText(dialogrows[process_ID].dialog);
        if (dialogrows[process_ID].image_name != "")
            ShowImage(dialogrows[process_ID].image_name, dialogrows[process_ID].image_position, dialogrows[process_ID].image_size);
    }//显示“对话”*/
    private void ShowOptions(int process_ID)
    {
        CommandReader(dialogrows[process_ID].command_before);
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
        button_continue.gameObject.SetActive(false);
        Interaction interaction = Dic_Name_Interaction[interaction_name];
        StartCoroutine(Interaction(interaction));

    }//开始交互环节
    IEnumerator Interaction(Interaction interaction)
    {
        yield return interaction.Interactions();
        button_continue.gameObject.SetActive(true);
    }//交互协程
    private void ShowEffect(string effect_name)
    {
        Effect effect = Dic_Name_Effect[effect_name];
        effect.Effects();
    }//显示效果
    private void HideUI()
    {
        Transform a = canvas_main.transform.GetChild(0);
        Transform b = canvas_main.transform.GetChild(1);
        Transform c = canvas_main.transform.GetChild(2);
        a.gameObject.SetActive(false);
        b.gameObject.SetActive(false);
        c.gameObject.SetActive(false);

    }//隐藏UI
    private void ShowUI()
    {
        Transform a = canvas_main.transform.GetChild(0);
        Transform b = canvas_main.transform.GetChild(1);
        Transform c = canvas_main.transform.GetChild(2);
        a.gameObject.SetActive(true);
        b.gameObject.SetActive(true);
        c.gameObject.SetActive(true);
    }//显示UI
    private void HideContinueButton()
    {
        button_continue.gameObject.SetActive(false);
        
    }//隐藏继续按钮
    private void ShowContinueButton()
    {
        button_continue.gameObject.SetActive(true);
    }//显示继续按钮
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
                if (singlecommand[0] == "clearimage")
                {
                    if (int.TryParse(singlecommand[1], out int value))
                        ClearImage(value);
                    else
                        ClearImage(singlecommand[1]);
                    if (imagefade)
                    {
                        FadeOutSprite(spriterenderers[value], 1f);
                    }
                }
                if (singlecommand[0] == "stopaudio")
                {
                    StopPlayAudio(singlecommand[1]);
                    if (audiofade)
                    {
                        FadeOutAudio(singlecommand[1], 1f);
                    }
                }
                if (singlecommand[0] == "option")
                {
                    isoption = true;
                }
                if (singlecommand[0] == "interaction")
                {
                    isinteraction = true;
                }
                if (singlecommand[0] == "continue")
                {
                    Processor(processID);
                }
            }
        }
    }//命令读取
    private void Processor(int process_ID)//进程控制器
    {
        processID = dialogrows[process_ID].process_next;
        CommandReader(dialogrows[process_ID].command_before);    
        if(isoption)
        {
            ShowOptions(process_ID);
        }
        else if(isinteraction)
        {
            StartInteraction(dialogrows[process_ID].command_after);
        }
        else
        {
            StartCoroutine(Text(process_ID));
            StartCoroutine(Image(process_ID));
            StartCoroutine(Background(process_ID));
            StartCoroutine(Audio(process_ID));
        }

        CommandReader(dialogrows[process_ID].command_after);

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
            ShowImage(index);
                yield return null;
        }
    }
    private IEnumerator Background(int index)
    {
        if (dialogrows[index].background != null)
        {
            ShowBackground(dialogrows[index].background);
            yield return null;
        }
    }
    private IEnumerator Audio(int index)
    {
        if (dialogrows[index].audio != "")
        {
            PlayAudio(dialogrows[index].audio);
            yield return null;
        }
    }

    //*************************以下是按钮功能***************************//
    private void ButtonContinueClick()
    {
        Processor(processID);


    }//继续（隐藏按钮）
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
        #endregion
        #region 初始化背景字典
        Dic_Name_Background["classroom"] = sprite_backgrounds[0];
        Dic_Name_Background["desk"] = sprite_backgrounds[1];
        Dic_Name_Background["dormitory"] = sprite_backgrounds[2];
        #endregion
        #region 初始化音频字典
        Dic_Name_Audio["click1"]= audiosources[0];
        Dic_Name_Audio["bgm1"] = audiosources[1];
        #endregion
        #region 初始化交互
        Dic_Name_Interaction["Interaction_LimitedTimeToChoose"] = interactions[0];
        #endregion
        Debug.Log("Awake finished");
    }

    private void Start()
    {
        processID = 0;
    }

}

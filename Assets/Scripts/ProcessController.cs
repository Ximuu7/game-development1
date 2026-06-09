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

public class ProcessController : MonoBehaviour
{
    public int processID;
    public TextAsset jsonfile;
    public Canvas canvas_main;
    public Button button_continue;

    [System.Serializable]
    public class Dialogrows
    {
        public int process_ID; // 进程序号，从0开始递增
        public string process_type; // 对话、选择、交互
        public string dialog; // 对话文本
        public string image_name; // 图像名称
        public float image_size; // 图像大小,1表示与屏幕等高
        public int image_position; // 图像位置
        public string command; // 命令
        public int process_next; // 下一个进程ID
    }

    public Dictionary<string,Sprite> Dic_Name_Image = new Dictionary<string,Sprite>();
    public Dictionary<string,Sprite> Dic_Name_Background = new Dictionary<string,Sprite>();
    public Dictionary<string,AudioClip> Dic_Name_Audio = new Dictionary<string, AudioClip>();
    public Dictionary<string,AnimationClip> Dic_Name_Animation = new Dictionary<string, AnimationClip>();

    public TMP_Text dialogtext;
    public List<Dialogrows> dialogrows=new List<Dialogrows>();

    public List<Sprite> sprite_roles;//角色图像
    public List<Sprite> sprite_backgrounds;//背景图像
    public List<SpriteRenderer> spriterenderers = new List<SpriteRenderer>();  // 用于显示图像的SpriteRenderer组件
    public List<AudioClip> audioclips = new List<AudioClip>();// 音频
    public List<AnimationClip> animationclips = new List<AnimationClip>();// 动画

    public Button option; // 选项按钮预制体
    public Transform optiongroup; // 选项按钮的父物体
    public string command_type;
    public string command_content;
    public Image background;


    public void ReadDialog(string str)
    {
        jsonfile = Resources.Load<TextAsset>(str);
        dialogrows= JsonConvert.DeserializeObject<List<Dialogrows>>(jsonfile.text);
    }//读取文件
    public void ShowText(string dialog)
    {
        dialogtext.text = dialog;
    }//显示文本
    public void ShowImage(string image_name,int image_position,float image_size)
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

    }//显示图像
    
    public void ShowDialog(int process_ID)
    {
        if(dialogrows[process_ID].dialog!="")
            ShowText(dialogrows[process_ID].dialog);
        if (dialogrows[process_ID].image_name != "")
            ShowImage(dialogrows[process_ID].image_name, dialogrows[process_ID].image_position, dialogrows[process_ID].image_size);
    }//显示“对话”
    public void ShowOptions(int process_ID)
    {
        if (dialogrows[process_ID].process_type == "b")
        {
            GameObject button = Instantiate(option.gameObject, optiongroup);
            Debug.Log("generated option");
            button.GetComponentInChildren<TMP_Text>().text = dialogrows[process_ID].dialog;

            button.GetComponent<Button>().onClick.AddListener
            (delegate
            {
                OnOptionClick(dialogrows[process_ID].process_next);
            });
            ShowOptions(process_ID + 1);
        }
    }//显示选项
    public void ShowBackground(string image_background)
    {
        Sprite sprite= Dic_Name_Background[image_background];
        background.sprite = sprite;
    }//显示背景
    public void ShowAnimation(string animation_name)
    {
        Debug.Log("Playing animation: " + animation_name);
    }//显示动画
    public void PlayAudio(string audio_name)
    {
        AudioClip clip = Dic_Name_Audio[audio_name];
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Vector3.zero);
            Debug.Log("Playing audio: " + audio_name);
        }
        else
        {
            Debug.LogWarning("音频不存在" + audio_name);
        }
    }//播放音频


    public void ClearImage(int image_position)
    {
        spriterenderers[image_position].sprite = null;
        Debug.Log("Cleared image at position: " + image_position);
    }//清除图像
    public void StopPlayAudio(string audio_name)
    {

    }//停止播放音频

    public void ReadCommand(string command)
    {
        Debug.Log("Reading command: " + command);
        string[] commands = command.Split(',');
        for (int i = 0; i < commands.Length; i++)
        {
            string[] singlecommand= commands[i].Split('=');
            if (singlecommand[0] == "audio")
            {
                command_content = singlecommand[1];
                PlayAudio(command_content);
            }
            else if (singlecommand[0] == "animation")
            {
                command_content = singlecommand[1];
                ShowAnimation(command_content);
                
            }
            else if (singlecommand[0] == "background")
            {
                command_content = singlecommand[1];
                ShowBackground(command_content);
            }
            else if (singlecommand[0] == "clearimage")
            {
                int image_position = int.Parse(singlecommand[1]);
                ClearImage(image_position);
            }
            else if (singlecommand[0]=="continue")
            {
            
                Debug.Log("Command: continue to process ID " + processID);
                Processor(processID);
            }
            else if (singlecommand[0] == "interaction")
            {
            }
        }

    }//读取命令
    public void Processor(int process_ID)
    {
        Debug.Log("Processing ID: " + process_ID);
        string process_type = dialogrows[process_ID].process_type;
        if (process_type=="a") //只播放音乐、动画，无对话
        {            
            processID = dialogrows[process_ID].process_next;
        }
        else if(process_type=="b")//选择
        {
            button_continue.gameObject.SetActive(false);
            ShowOptions(process_ID);
        }
        else if(process_type=="c")//对话
        {
            ShowDialog(process_ID);
            processID = dialogrows[process_ID].process_next;
        }else if (process_type=="d")//交互
        {
            
        }
        ReadCommand(dialogrows[process_ID].command);
    }//进程控制器

    public void ButtonContinueClick()
    {
        Processor(processID);
    }//继续（隐藏按钮）
    public void OnOptionClick(int index)
    {
        processID = index;
        Processor(processID);
        for (int i = 0; i < optiongroup.childCount; i++)
        {
            Destroy(optiongroup.GetChild(i).gameObject);
            Debug.Log("Destroyed option button");
        }
        button_continue.gameObject.SetActive(true);
    }//点击选项按钮

    public void NotAllowSkip() 
    {         
        button_continue.gameObject.SetActive(false);
    }//不允许跳过


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
        Dic_Name_Audio["click1"]= audioclips[0];
        Dic_Name_Audio["bgm1"] = audioclips[1];
        #endregion
        Debug.Log("Awake finished");
    }


    private void Start()
    {
        processID = 0;
    }

}

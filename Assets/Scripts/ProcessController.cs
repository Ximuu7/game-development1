using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProcessController : MonoBehaviour
{
    public int process_ID;
    public TextAsset jsonfile;

    [System.Serializable]
    public class Dialogrows
    {
        public int process_ID; // 进程序号，从0开始递增
        public string process_type; // 对话、选择、交互
        public string dialog; // 对话文本
        public string image_name; // 图像名称
        public int image_size; // 图像大小,1表示与屏幕等高
        public int image_position; // 图像位置
        public string command; // 命令
        public int process_next; // 下一个进程ID
    }

    public Dictionary<string,Sprite> Dic_Name_Image = new Dictionary<string,Sprite>();

    public TMP_Text dialogtext;
    public List<Dialogrows> dialogrows=new List<Dialogrows>();

    public List<Sprite> sprite_roles;//角色图像
    public List<Sprite> sprites_backgrounds;//背景图像
    public List<SpriteRenderer> spriterenderers = new List<SpriteRenderer>();  // 用于显示图像的SpriteRenderer组件
    public List<AudioClip> audioclips = new List<AudioClip>();// 音频
    public List<AnimationClip> animationclips = new List<AnimationClip>();// 动画
    public Button option; // 选项按钮预制体
    public Transform optiongroup; // 选项按钮的父物体
    public string command_type;
    public string command_content;

    public void ReadDialog(string str)
    {
        jsonfile = Resources.Load<TextAsset>(str);
        dialogrows= JsonConvert.DeserializeObject<List<Dialogrows>>(jsonfile.text);
    }

    public void ShowText(string dialog)
    {
        dialogtext.text = dialog;
    }//显示文本

    public void ShowImage(string image_name,int image_position)
    {
        Sprite sprite = Dic_Name_Image[image_name];
        spriterenderers[image_position].sprite = sprite;

    }//显示对应图像

    public void ShowDialog(int program_ID)
    {
        if(dialogrows[program_ID].dialog!="")
            ShowText(dialogrows[program_ID].dialog);
        if (dialogrows[program_ID].image_name != "")
            ShowImage(dialogrows[program_ID].image_name, dialogrows[program_ID].image_position);
    }//显示“对话”

    public void ShowOptions(int program_ID)
    {
        GameObject button = Instantiate(option.gameObject, optiongroup);
        Debug.Log("generated option");
        button.GetComponentInChildren<TMP_Text>().text = dialogrows[program_ID].dialog;

        button.GetComponent<Button>().onClick.AddListener
        (delegate
        {
            OnOptionClick(dialogrows[program_ID].process_next);
        });
        ShowOptions(program_ID + 1);

    }

    public void ShowBackground(string image_background)
    {

    }

    public void ReadCommand(string command)
    {
        string[] commands = command.Split(',');
        for (int i = 0; i < commands.Length; i++)
        {
            string[] singlecommand= commands[i].Split('=');
            if (singlecommand[0] == "sound")
            {
                command_type = "sound";
                command_content = singlecommand[1];
                for(int j = 0; j < audioclips.Count; j++)
                {
                    if(audioclips[j].name == command_content)
                    {
                        AudioSource.PlayClipAtPoint(audioclips[j], Vector3.zero);
                        break;
                    }
                }
            }
            else if (singlecommand[0] == "animation")
            {
                command_type = "animation";
                command_content = singlecommand[1];
                for (int j = 0; j < animationclips.Count; j++)
                {
                    if (animationclips[j].name == command_content)
                    {
                        // Play the animation clip
                        // Assuming you have an Animator component attached to the same GameObject
                        Animator animator = GetComponent<Animator>();
                        if (animator != null)
                        {
                            animator.Play(animationclips[j].name);
                        }
                        break;
                    }
                }
            }
            else if (singlecommand[0] == "background")
            {
                command_type = "background";
                command_content = singlecommand[1];
                ShowBackground(command_content);
            }
        }

    }//读取命令

    public void Processor(int program_ID)
    {
        string program_type = dialogrows[program_ID].process_type;
        ReadCommand(dialogrows[program_ID].command);
        if (program_type=="#") //只播放音乐、动画，无对话
        {

        }
        else if(program_type=="&")//选择
        {
            ShowOptions(program_ID);
        }
        else if(program_type=="@")//对话
        {
            ShowDialog(program_ID);
        }
    }

    public void ButtonContinueClick()
    {
        Processor(process_ID);
    }//继续按钮（隐藏）
    public void OnOptionClick(int index)
    {
        process_ID = index;
        Processor(process_ID);
        for (int i = 0; i < optiongroup.childCount; i++)
        {
            Destroy(optiongroup.GetChild(i).gameObject);
        }
    }//点击选项按钮
 

    private void Awake()
    {
        ReadDialog("storyline");

    }


    private void Start()
    {
        process_ID = 0;
    }
}

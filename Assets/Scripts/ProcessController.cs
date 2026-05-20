using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProcessController : MonoBehaviour
{
    public int current_process_ID;
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

    public List<Sprite> sprite_roles;
    public List<Sprite> sprites_backgrounds;
    public List<SpriteRenderer> SpriteRenderers = new List<SpriteRenderer>();
    public List<Image> images = new List<Image>();

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
        images[image_position].sprite = sprite;

    }//显示对应图像

    public void ShowDialog(int current_program_ID)
    {
        if(dialogrows[current_program_ID].dialog!="")
            ShowText(dialogrows[current_program_ID].dialog);
        if (dialogrows[current_program_ID].image_name != "")
            ShowImage(dialogrows[current_program_ID].image_name, dialogrows[current_program_ID].image_position);
    }//显示“对话”

    public void ShowChoices()
    {

    }

    public void ShowBackground(string image_background)
    {

    }

    public void CommandReader(string command)
    {
        string[] command_parts = command.Split(':');
        command_type = command_parts[0];
        command_content = command_parts[1];

    }

    public void Processor(int current_program_ID)
    {
        string program_type = dialogrows[current_program_ID].process_type;
        if(program_type=="对话")
        {
            ShowDialog(current_program_ID);
        }
        else if(program_type=="选择")
        {
            ShowChoices();
        }
        else if(program_type=="交互")
        {

        }
        else if(program_type=="动画")
        {

        }

    }

    public void CloseDialogWindow(string dialogwindow)
    {

    }

    public void ReadCommand(string command)
    {

    }
    private void Awake()
    {
        ReadDialog("storyline");

    }


    private void Start()
    {
        current_process_ID = 0;
    }
}

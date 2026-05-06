using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    public TextAsset jsonfile;
    public List<Sprite> sprites;

    [System.Serializable]
    public class Dialogrows
    {
        public int dialog_ID; // 0
        public string dialog_type; // #
        public string name; // a
        public string dialog; // a1
        public string dialog_position; // down
        public string image_name; // a1
        public string image_position; // left
        public int dialog_next; // 1
        public string command; // 
    }

    public Dictionary<string,Sprite> Dic_Name_Image = new Dictionary<string,Sprite>();

    public TMP_Text dialogtext;

    public List<Dialogrows> dialogrow=new List<Dialogrows>();

    public void ReadDialog(string str)
    {
        jsonfile = Resources.Load<TextAsset>(str);
        dialogrow= JsonConvert.DeserializeObject<List<Dialogrows>>(jsonfile.text);
    }

    public void ShowText(string text,string text_position)
    {

    }

    public void ShowImage(string image_name,string image_position)
    {

    }

    public void ShowBackground(string image_background)
    {

    }

    public void ShowDialog()
    {

    }

    public void Awake()
    {
        ReadDialog("storyline");

    }

}

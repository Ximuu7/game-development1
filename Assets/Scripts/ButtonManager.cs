using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{

    public List<Button> buttons_start;
    public List<Button> buttons_main;
    public List<Button> buttons_settings;

    public List<ToggleGroup> togglegroups_settings;

    public Slider volumeSlider;
    public GameObject canvas_settings;
    public GameObject canvas_main;
    public GameObject canvas_start;
    public GameObject canvas_background;
    public GameObject roles;
    public AudioClip click;
    Vector3 button_position_backtostart;
    public bool isgaming = false;
    Resolution currentResolution;


    public void Display_FullScreen(bool isOn)
    {
        if (isOn)
        {
            Screen.SetResolution(currentResolution.width, currentResolution.height, true);
        }
    }//全屏
    public void Display_Windowed(bool isOn)
    {
        if (isOn)
        {
            Screen.SetResolution(currentResolution.width, currentResolution.height, false);
        }
    }//窗口化
    public void SetVolume(float value)//主音量
    {
        AudioListener.volume = value;
    }
    public void VolumeController_Effect()
    {
    }// 音效控制
    
    public void CountDown() 
    {      
        
    }//显示倒计时条

    public void OpenStart()
    {
        canvas_start.SetActive(true);
    }//打开主界面
    private void CloseStart()
    {
        canvas_start.SetActive(false);
    }//关闭主界面

    private void OpenGame() 
    {
        canvas_background.SetActive(true);
        canvas_main.SetActive(true);
        isgaming = true;
    }//打开游戏界面
    public void CloseGame()
    {
        canvas_background.SetActive(false);
        canvas_main.SetActive(false);
        isgaming = false;
    }//关闭游戏界面

    private void OpenSettings()
    {
        canvas_settings.SetActive(true);
    }//打开设置界面
    public void CloseSettings()
    {
        canvas_settings.SetActive(false);
    }//关闭设置界面


    public void ToGame()
    {
        CloseStart();
        OpenGame();
        CloseSettings();
    }//游戏
    public void ToStart()
    {
        CloseGame();
        CloseSettings();
        OpenStart();
    }//主界面
    public void ToSettings()
    {
        CloseGame();
        CloseStart();
        OpenSettings();
    }//设置界面
    public void ExitGame()
    {
        Application.Quit();
    }//退出游戏
    

    public void ChangeButtonPositionInSettings()
    {
        buttons_settings[0].gameObject.SetActive(false);
        buttons_settings[1].transform.localPosition = buttons_settings[0].transform.localPosition;
    }
    public void RecoverButtonPositionInSettings()
    {
        buttons_settings[0].gameObject.SetActive(true);
        buttons_settings[1].transform.localPosition = button_position_backtostart;
    }
    public void Continue()
    {

    }

    public void ClickSound()
    {
        AudioSource.PlayClipAtPoint(click, Vector3.zero, 1f);
    }

    private void Update()
    {
        
    }

    private void Start()
    {
        OpenStart();
        CloseGame();
        CloseSettings();
        button_position_backtostart = buttons_settings[1].transform.localPosition;
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = AudioListener.volume; // 初始同步
        volumeSlider.onValueChanged.AddListener(SetVolume);
        // 获取当前屏幕的推荐分辨率
        currentResolution = Screen.currentResolution;
        // 设置为当前设备的分辨率，使用全屏窗口模式
        Screen.SetResolution(currentResolution.width, currentResolution.height, FullScreenMode.FullScreenWindow);
    }

}



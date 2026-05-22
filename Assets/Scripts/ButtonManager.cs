using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager_Settings : MonoBehaviour
{

    public List<Button> buttons_start;
    public List<Button> buttons_main;
    public List<Button> buttons_settings;

    public List<ToggleGroup> togglegroups_settings;

    public List<Slider> sliders_settings;
    public GameObject canvas_settings;
    public GameObject canvas_main;
    public GameObject canvas_start;
    public GameObject canvas_background;
    public GameObject roles;

    Vector3 button_position_backtostart;
    bool isgaming = false;

    public void Display_FullScreen(bool isOn)
    {
        if (isOn)
        {
            Screen.SetResolution(1920, 1080, true);
        }
    }//全屏
    public void Display_Windowed(bool isOn)
    {
        if (isOn)
        {
            Screen.SetResolution(1920, 1080, false);
        }
    }//窗口化
    public void VolumeController_Background(float value)
    {

    }// 背景音控制
    public void VolumeController_Effect()
    {
    }// 音效控制
    public void ChangeState_MainUI()
    {
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) && canvas_main.activeSelf)
        {
            canvas_main.SetActive(false);
        }
        else if ((Input.GetMouseButtonDown(1)||Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape)) && !canvas_main.activeSelf)
        {
            canvas_main.SetActive(true);
        }
    }//隐藏文本框和UI

    private void OpenStart()
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
    private void CloseGame()
    {
        canvas_background.SetActive(false);
        canvas_main.SetActive(false);
        isgaming = false;
    }//关闭游戏界面

    private void OpenSettings()
    {
        canvas_settings.SetActive(true);
    }//打开设置界面
    private void CloseSettings()
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

    private void Update()
    {
        if (isgaming)
        {
            ChangeState_MainUI();
        }
    }

    private void Start()
    {
        OpenStart();
        CloseGame();
        CloseSettings();
        button_position_backtostart = buttons_settings[1].transform.localPosition;
    }

}

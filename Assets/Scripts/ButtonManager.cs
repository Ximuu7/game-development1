using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{

    public List<Button> buttons;
    public List<Toggle> toggles;
    public List<Slider> sliders;
    public GameObject canvas_settings;
    public GameObject canvas_main;
    public GameObject canvas_start;


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
    }

    public void CloseSettings()
    {
        canvas_settings.SetActive(false);
    }//关闭设置界面
    public void OpenSettings()
    {
        canvas_settings.SetActive(true);
    }//打开设置界面

    public void Continue()
    {

    }

    private void Update()
    {
        ChangeState_MainUI();
    }
}

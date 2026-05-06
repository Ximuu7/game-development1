using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetResolution1920x1440 : MonoBehaviour
{
    private int x = 1920;
    private int y = 1440;
    private Toggle toggle;
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(ChangeResolution);
    }

    void ChangeResolution(bool isOn)
    {
        if (isOn)
        {
            Screen.SetResolution(x, y, true);
        }
    }
}

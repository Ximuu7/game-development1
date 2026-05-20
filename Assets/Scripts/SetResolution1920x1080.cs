using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SetResolution1920x1080 : MonoBehaviour
{   // Update is called once per frame
    private int x=1920;
    private int y=1080;
    private Toggle toggle;
    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(ChangeResolution);
    }

    void ChangeResolution(bool isOn)
    {
        if(isOn)
        {
            Screen.SetResolution(x, y, true);
        }
    }
}

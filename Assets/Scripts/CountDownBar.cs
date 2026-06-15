using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownBar : MonoBehaviour
{
    public float time;
    private GameObject fillarea;
    private float fillamount;
    public void CountDown()
    {
        if (fillarea.GetComponent<Image>().fillAmount > 0)
        {
            fillarea.GetComponent<Image>().fillAmount -= Time.deltaTime / time;
        }
        if (fillarea.GetComponent<Image>().fillAmount <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        fillarea = GameObject.Find("Fill Area");
        fillamount = fillarea.GetComponent<Image>().fillAmount;
        
    }


    void Update()
    {
        CountDown();
    }

}

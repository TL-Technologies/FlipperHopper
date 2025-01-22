using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SineScript : MonoBehaviour
{
    public float magnitude = 5f;
    public float spd = 5f;
    public float startValue;
    public bool fontSize;

    public void Update(){
        // making "tap too shoot" text big and small like a sine animation
        
        float value =  startValue + Mathf.Sin(Time.time * spd) * magnitude;
        
        if (fontSize)
        {
            GetComponent<TextMeshProUGUI>().fontSize = value;
        }
        
    }

    
}

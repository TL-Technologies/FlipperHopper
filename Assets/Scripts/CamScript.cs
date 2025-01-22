using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CamScript : MonoBehaviour
{
    public float horizontalResolution = 1920;
    public float distNum = 5f;
 
    void OnGUI ()
    {
        float currentAspect = (float) Screen.width / (float) Screen.height;
        Camera.main.orthographicSize = horizontalResolution / currentAspect / 200;
        
    }
}

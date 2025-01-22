using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class buttonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        // checks if button is being pressed
        isPressed = true;
    }
 
    public void OnPointerUp(PointerEventData eventData)
    {
        // checks if button is being not pressed
        isPressed = false;
    }
}

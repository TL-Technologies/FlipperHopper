using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeScript : MonoBehaviour
{
    public GameObject scrollbar;
    private float scroll_pos = 0;
    float[] pos;
    private float swipeT = 1f;
    public int curPage;
    public bool tabChanged;

    private StoreScript storeScript;

    private void Start()
    {
        tabChanged = false;
        storeScript = FindObjectOfType<StoreScript>();

        LeanTween.delayedCall(0.1f, () => { GotoCurPage(); });

    }

    void Update()
    {
        // controlling swipe in store
        
        pos = new float[transform.childCount];
        
        float distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
            
        // when we're touching, just change the scroll
        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
            swipeT = 0.03f;
        }
        // when we're not touching, go to nearest page
        else
        {
            if (!tabChanged)
            {
                for (int i = 0; i < pos.Length; i++)
                {
                    if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                    {
                        scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], swipeT * Time.deltaTime*500);
                        curPage = i;
                    }
                }
            }
            else
            {
                tabChanged = false;
                GotoCurPage();
            }
        }
        
    }
    
    void GotoCurPage()
    {
        // go to selected item page 
        
        int selectedItemPage = 0;
                
        if (storeScript.curTab == 0)
        {
            selectedItemPage = int.Parse(storeScript.balls.Split('|')[0])/9;
        }
        else if (storeScript.curTab == 1)
        {
            selectedItemPage = int.Parse(storeScript.flippers.Split('|')[0])/9;
        }
        
        scroll_pos = selectedItemPage;
        scrollbar.GetComponent<Scrollbar>().value = selectedItemPage;

    }
}
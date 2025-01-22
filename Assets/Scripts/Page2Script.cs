using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Page2Script : MonoBehaviour
{
    private GameController gameController;
    
    public GameObject txt1;
    public GameObject txt2;
    public GameObject diamonds;
    public GameObject collectedDiamonds;
    public GameObject diamonds_txt;
    public GameObject collectedDiamonds_txt;
    public GameObject btnAction;
    public GameObject btnWatchAd;
    public GameObject waitForAdPanel;
    public GameObject panel;
    public GameObject progress;
    public GameObject adProgressBar;
    


    void Start()
    {
        gameController = FindObjectOfType<GameController>();

        if (btnAction)
        {
            btnAction.GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("Game"); });
        }
        
        if (btnWatchAd)
        {
            btnWatchAd.GetComponent<Button>().onClick.AddListener(gameController.BtnWatchAdClicked);
        }
        
        
    }
    
}

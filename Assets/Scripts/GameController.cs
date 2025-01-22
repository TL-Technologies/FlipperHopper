using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class GameController : MonoBehaviour
{
    public int levelScore;
    public int diamonds;
    public int level;
    public int tempLevel;
    public string levelState;

    public GameObject startFader;
    public GameObject txtLevel;
    public GameObject tickArea;

    public GameObject txtTap;
    public GameObject btnStore;
    public GameObject btnSound;
    public Page2Script page2;

    public GameObject tickPrefab;
    public GameObject diamondsPrefab;
    public GameObject ballPrefab;
    public GameObject flipperPrefab;
    public GameObject basketParticles;
    
    public List<GameObject> ticksList = null;
    public LevelsList[] levelsList;
    
    private BasketScript basketScript = null;
    private ItemsControllerScript itemsControllerScript;
    
    [Space]
    [SerializeField] internal SpriteRenderer flipper;
    [SerializeField] internal SpriteRenderer ball;
    [SerializeField] internal List<Sprite> ballSprites = new List<Sprite>();
    [SerializeField] internal List<Sprite> flipperList = new List<Sprite>();

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        basketScript = FindObjectOfType<BasketScript>();
        itemsControllerScript = FindObjectOfType<ItemsControllerScript>();
        SetFlipperAndBall();
        // getting needed data from memory
        GetPlayerPrefsAndData();
        
        // deactivating page2 at start
        // page2 shows the game result (win / lose)
        if (page2)
        {
            if (page2.gameObject.activeSelf)
            {
                page2.gameObject.SetActive(false);
            }
            
            // deactivating waitForAdPanel(in page2) at start
            page2.waitForAdPanel.SetActive(false);
        }
        
        //starting startFader(a black layer on gameplay at start) and then fading it out 
        startFader.SetActive(true);
        LeanTween.value(startFader, 1, 0, 0.5f).setEaseOutQuint().setOnUpdate( (float val) => { SetAlpha(startFader, val); });
        
        //adding listeners/actions for Store and Sound buttons
        btnStore.GetComponent<Button>().onClick.AddListener(()=> { SceneManager.LoadScene("Store"); });
        btnSound.GetComponent<Button>().onClick.AddListener(()=> { GetComponent<AudioController>().SoundTrigger(); });

        //setting ball and flipper sprites
        SetSprites();
    }

    private void Update()
    {
        // controlling 'tap to start' part
        TapToStart();
    }
    
    void SetSprites()
    {
        if (PlayerPrefs.HasKey("balls"))
        {
            int curBall = int.Parse(PlayerPrefs.GetString("balls").Split('|')[0]);
            Sprite[] ballSprites = Resources.LoadAll<Sprite>("Sprites/Store/balls").OfType<Sprite>().ToArray();
            
            ballPrefab.GetComponent<SpriteRenderer>().sprite = ballSprites[curBall];
        }

        if (PlayerPrefs.HasKey("flippers"))
        {
            int curFlipper = int.Parse(PlayerPrefs.GetString("flippers").Split('|')[0]);
            Sprite[] flipperSprites = Resources.LoadAll<Sprite>("Sprites/Store/flippers").OfType<Sprite>().ToArray();
            flipperPrefab.GetComponent<SpriteRenderer>().sprite = flipperSprites[curFlipper];
        }
    }
    
    void GetPlayerPrefsAndData()
    {

        level = PlayerPrefs.GetInt("level") != 0 ? PlayerPrefs.GetInt("level") : 1;
        diamonds = PlayerPrefs.GetInt("diamonds");

        if (level % (levelsList.Length - 1) == 0)
        {
            tempLevel = levelsList.Length - 1;
        }
        else
        {
            tempLevel = level % (levelsList.Length - 1);
        }

        
        txtLevel.GetComponent<TextMeshProUGUI>().text = "LEVEL " + level;
        CreateTicks();

    }
    
    void TapToStart()
    {
        // controlling 'tap to start' part
        
        // if we clicked anywhere
        if (txtTap && Input.GetMouseButtonDown(0))
        {
            // if we didn't click on btnStore or btnSound
            if (!btnStore.GetComponent<buttonScript>().isPressed  && !btnSound.GetComponent<buttonScript>().isPressed)
            {
                // destroying txtTap & btnStore & btnSound
                LeanTween.value(txtTap, 1, 0, 0.3f).setEaseOutQuint().setOnUpdate(
                    (float val) => { SetAlpha(txtTap,val); }
                ).setOnComplete(()=> { Destroy(txtTap); });

                LeanTween.value(btnStore, 1, 0, 0.1f).setEaseOutQuint().setOnUpdate(
                    (float val) => { SetAlpha(btnStore,val); }
                ).setOnComplete(()=> { Destroy(btnStore); });
            
                LeanTween.value(btnSound, 1, 0, 0.1f).setEaseOutQuint().setOnUpdate(
                    (float val) => { SetAlpha(btnSound,val); }
                ).setOnComplete(()=> { Destroy(btnSound); });
            
                // creating a new basket
                basketScript.CreateNew();
            
                // creating a new item (big basket / 2x score / teleport) after given random number 
                itemsControllerScript.Invoke("TryToCreateItem", 
                    Random.Range(itemsControllerScript.createItemStartTime,itemsControllerScript.createItemEndTime));
            }
            
        }
    }
    
    void CreateTicks()
    {
        // deleting initial ticks
        for (int i = 0; i < tickArea.transform.childCount; i++)
        {
            Destroy(tickArea.transform.GetChild(i).gameObject);
        }
        
        // creating new ticks based on list of levels
        for (int i = 1; i <= levelsList[tempLevel].ticksCount; i++)
        {
            GameObject newTick = Instantiate(tickPrefab, transform.position, transform.rotation);
            
            newTick.transform.SetParent(tickArea.transform);
            newTick.transform.localScale = Vector3.one;
            
            ticksList.Add(newTick);
            
        }
        
    }

    public void AddScore()
    {
        // if item 2x is enable, we add two ticks
        if (itemsControllerScript.curAvailableItem == ItemsControllerScript.Items.TwoX)
        {
            AddTick();
            AddTick();
            
            itemsControllerScript.ItemActivation(false ,ItemsControllerScript.Items.None);
        }
        // else we just add one tick
        else
        {
            AddTick();
        }
        
        // moving current basket out (and then create new one)
        basketScript = FindObjectOfType<BasketScript>();
        basketScript.MoveOut();
        
    }
    
    void AddTick()
    {
        // adding to ticks | level scores 
        if (ticksList.Count >= levelScore+1) 
        {
            levelScore++;
            PlayerPrefs.SetInt("levelScore" , levelScore);
            GameObject theTick = ticksList[levelScore - 1].gameObject.transform.GetChild(0).gameObject;
            theTick.SetActive(true);
            theTick.GetComponent<RectTransform>().localScale = Vector3.zero;
            LeanTween.scale(theTick, Vector3.one, 0.2f).setEaseOutBack();
        }
    }
[ContextMenu("Eleve")]
    public void LevelDone()
    {
        // we reached to the level target, now we show level done message via 'page2' object
        if (levelState == "")
        {
            // activating page2
            page2.gameObject.SetActive(true);
            
            // playing "LevelDone" sound
            GetComponent<AudioController>().PlaySound("LevelDone" , 1);
            
            // setting levelState 
            levelState = "complete";
            
            // these objects are for level failed situation, so we deactivate / destroy them
            ballPrefab.SetActive(false);
            
            // saving current level in the memory to get it in the next gameplay
            PlayerPrefs.SetInt("level", level + 1);

            // showing "CONTINUE" for the btnAction
            page2.btnAction.GetComponent<TextMeshProUGUI>().text = "CONTINUE";
            
            // activating objects
            page2.txt2.SetActive(true);
            
            // showing diamonds
           // page2.diamonds_txt.GetComponent<TextMeshProUGUI>().text = diamonds.ToString();
            
            // showing level
            page2.txt1.GetComponent<TextMeshProUGUI>().text = "LEVEL " + level;
            
            // showing random diamonds as level gift
            var randomDiamonds = Convert.ToInt32(Random.Range(7,35));
            page2.collectedDiamonds_txt.GetComponent<TextMeshProUGUI>().text = randomDiamonds.ToString();

            // adding collected diamonds (the random diamonds) to game diamonds
            page2.panel.transform.localScale = Vector3.zero;
            LeanTween.scale(page2.panel, Vector3.one, 0.5f).setEaseOutQuint();
        }
    }

    public void LevelFailed()
    {
        if (levelState == "")
        {
            // activating page2
            page2.gameObject.SetActive(true);
            
            // playing "LevelDone" sound
            GetComponent<AudioController>().PlaySound("LevelFailed" , 1);
            
            // setting levelState 
            levelState = "failed";
            
            // this object is for level done situation, so we deactivate it
            ballPrefab.SetActive(false);

            // activating objects
            page2.diamonds.SetActive(false);
            page2.collectedDiamonds.SetActive(false);
            page2.txt2.SetActive(false);

            // setting texts text
            page2.txt1.GetComponent<TextMeshProUGUI>().text = "SECOND CHANCE";
            page2.btnAction.GetComponent<TextMeshProUGUI>().text = "TRY AGAIN";
            
            // page2 scale out to normal size
            page2.panel.transform.localScale = Vector3.zero;
            LeanTween.scale(page2.panel, Vector3.one, 0.5f).setEaseOutQuint();

            // resetting ad progress bar
            LeanTween.pause(page2.progress);
            page2.progress.GetComponent<Image>().fillAmount = 1;
            
            // starting ad progress bar. when it completes, we deactivate adProgressBar
            LeanTween.value(page2.progress, 1f, 0f, 5f).setEaseLinear().setDelay(0.5f)
                .setOnUpdate((float val) =>
                    { page2.progress.GetComponent<Image>().fillAmount = val;})
                .setOnComplete(() =>
                {
                    page2.adProgressBar.SetActive(false);
                });
        }
    }
    
    void AddCollectedDiamonds(int howMany)
    {
        // adding diamond objects and moving toward diamond text
        var diamondsValue = howMany;
        var num = Convert.ToInt32(Mathf.Ceil(howMany / 5f));
        for (int i = 1; i <= num; i++)
        {
            GameObject diamond = Instantiate(diamondsPrefab, transform.position, transform.rotation);
            diamond.transform.SetParent(page2.panel.transform);
            diamond.transform.localScale = Vector3.one;
            diamond.transform.position = page2.collectedDiamonds_txt.transform.position;
            
            LeanTween.move(diamond, page2.diamonds_txt.transform, 0.5f).setDelay(i / 15f)
                .setOnComplete( () => {
                    diamondsValue -= 5;
                    if (diamondsValue >= 0) { diamonds += 5; } else { diamonds += howMany%5; PlayerPrefs.SetInt("diamonds",diamonds); }
                    
                    Destroy(diamond);
                    page2.diamonds_txt.GetComponent<TextMeshProUGUI>().text = diamonds.ToString();
                });
        }
    }

    public void SetAlpha(GameObject go , float value)
    {
        // setting objects alpha / opacity
        if (go)
        {
            if (go.gameObject.GetComponent<Image>())
            {
                var color = go.gameObject.GetComponent<Image>().color;
                color.a = value;
                go.gameObject.GetComponent<Image>().color = color;
            }
            else if (go.gameObject.GetComponent<TextMeshProUGUI>())
            {
                var color = go.gameObject.GetComponent<TextMeshProUGUI>().color;
                color.a = value;
                go.gameObject.GetComponent<TextMeshProUGUI>().color = color;
            }
            else if (go.gameObject.GetComponent<SpriteRenderer>())
            {
                var color = go.gameObject.GetComponent<SpriteRenderer>().color;
                color.a = value;
                go.gameObject.GetComponent<SpriteRenderer>().color = color;
            } 
        }
        

    }

    public void CheckLose()
    {
        // if no score earned, we just restart the game
        if (levelScore == 0)
        {
            SceneManager.LoadScene("Game");
        }
        // else we show level failed page
        else
        {
            LevelFailed();
        }
        
    }
    
    public void BtnWatchAdClicked()
    {
        // showing "Please Wait..." before showing ad
        page2.waitForAdPanel.SetActive(true);
        page2.waitForAdPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Please Wait...";
        
        // starting a 7 seconds timers for recognizing does ad shown or not
        LeanTween.delayedCall(7f, CheckForShowingAd);
        LeanTween.pause(page2.progress);
        
    }

    public void CheckForShowingAd()
    {
        // if ad didn't show after 7 seconds (this time starts when we click on BtnWatchAd), we show the message
        if (page2)
        {
            if (page2.waitForAdPanel.activeSelf)
            {
                AdFailed("It Seems Something's Wrong");
            }
        }
        
    }
    
    public void AdFailed(string msg)
    {
        // if ad failed to show/load or closed while it was playing, we call this function
        // then we show a proper message as result
        page2.waitForAdPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = msg;
        LeanTween.delayedCall(2f, () =>
        {
            // after 2 seconds, we deactivate adProgressBar & waitForAdPanel
            page2.waitForAdPanel.SetActive(false);
            page2.adProgressBar.SetActive(false);
        });
        
    }
    
    public void GiveMeReward()
    {
        // deactivating page2 & waitForAdPanel
        page2.waitForAdPanel.SetActive(false);
        page2.gameObject.SetActive(false);
        levelState = "";
        
        LeanTween.delayedCall(0.5f, () =>
        {
            ballPrefab.SetActive(true);
            ballPrefab.transform.position = new Vector3(-1.691f , -2f , ballPrefab.transform.position.z);
        });
    }




    void SetFlipperAndBall()
    {
        ball.sprite = ballSprites[PlayerPrefs.GetInt("BallIndex")];
        flipper.sprite = flipperList[PlayerPrefs.GetInt("FlipperIndex")];
    }
}
// class of levels (which we add needed score to pass each level)
[Serializable]
public class LevelsList 
{
    [Range(1, 12)] public int ticksCount;
}


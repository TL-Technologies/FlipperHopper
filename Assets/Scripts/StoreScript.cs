using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;



using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StoreScript : MonoBehaviour
{
    public GameObject content;
    public GameObject storeGroup;
    public GameObject storeItem;
    public GameObject storePageCircle;
    public GameObject btnBack;
    
    private int diamonds;
    private GameObject diamondsObject;
    private GameObject diamondsTextObject;

    private GameObject btnUnlock;
    private GameObject btnUnlockPriceTextObject;
    private int ballsPrice = 100;
    private int flippersPrice = 500;
    
    public string balls = "0|0,";
    public List<Sprite> ballItems;
    public List<GameObject> ballItemsPrefabs;
    
    public string flippers = "0|0,";
    public List<Sprite> flipperItems;
    public List<GameObject> flippersItemsPrefabs;
    
    
    public List<GameObject> storePageCircles;
    public GameObject pageCirclesGO;
    
    private int curPageTemp;
    
    public GameObject[] tabButtons;
    public GameObject tabLine;
    public int curTab = 0;

    void Start()
    {
        // getting needed data from memory
        GetStorePlayerPrefs();
        
        // getting number of diamonds and showing it
        diamonds = PlayerPrefs.GetInt("diamonds");
        diamondsObject = GameObject.Find("Diamonds");
//        diamondsTextObject = diamondsObject.transform.GetChild(1).gameObject;
       // diamondsTextObject.GetComponent<TextMeshProUGUI>().text = diamonds.ToString();

        // finding and adding listener of BtnUnlock
       // btnUnlock = GameObject.Find("BtnUnlock");
//        btnUnlock.GetComponent<Button>().onClick.AddListener(BtnUnlockClicked);
//        btnUnlockPriceTextObject = btnUnlock.transform.GetChild(1).transform.GetChild(0).gameObject;
        
        // go to Game scene when btnBack clicked
        btnBack.GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("Game");});

        CreateItems();
        SetStoreItems();

        // adding listener of store tabs
        for (int i = 0; i < tabButtons.Length; i++)
        {
            var num = i;
            tabButtons[num].GetComponent<Button>().onClick.AddListener(() => {TabClicked(num);});
        }
        
    }

    void GetStorePlayerPrefs()
    {
        // getting needed data from memory
        
        diamonds = PlayerPrefs.GetInt("diamonds");
        
        if (PlayerPrefs.HasKey("balls"))
        {
            balls = PlayerPrefs.GetString("balls");
        }

        if (PlayerPrefs.HasKey("flippers"))
        {
            flippers = PlayerPrefs.GetString("flippers");
        }
        
    }
    
    private void Update()
    {
        // setting the color of PageCircles when page changed
        if (curPageTemp != content.GetComponent<SwipeScript>().curPage)
        {
            curPageTemp = content.GetComponent<SwipeScript>().curPage;
            SetStorePageCircleColors();
        }
    }

    void BtnUnlockClicked()
    {
        // if it's balls tab
        if (curTab == 0)
        {
            // if all balls didn't unlock
            if (balls.Split(',').Length < ballItemsPrefabs.Count+1)
            {
                // if we have enough diamonds to unlock 
                if (diamonds >= ballsPrice)
                {
                    // go and find a random locked ball in the current page
                    var newItem = GetRandomItem(balls , ballItemsPrefabs);
                    
                    // if you found the ball
                    if (newItem != "not found")
                    {
                        // reduce from diamonds
                        diamonds -= ballsPrice;
                        
                        // save dimaonds number in the memory
                        PlayerPrefs.SetInt("diamonds",diamonds);
                        
                        // showing new value of diamonds
                        //diamondsTextObject.GetComponent<TextMeshProUGUI>().text = diamonds.ToString();
                        
                        // adding new unlocked ball to the list of balls  
                        balls += newItem;
                        
                        // saving new value of balls list
                        PlayerPrefs.SetString("balls" , balls);
                        
                        //showing new ball as unlocked (basically this function works from list of balls, flippers)
                        SetStoreItems();

                        // play "Buy" sound
                        FindObjectOfType<AudioController>().PlaySound("Buy" , 0.5f);
                    }
                }
            }
        }
        // if it's flippers tab
        else
        {
            // if all flippers didn't unlock
            if (flippers.Split(',').Length < flippersItemsPrefabs.Count+1)
            {
                // if we have enough diamonds to unlock 
                if (diamonds >= flippersPrice)
                {
                    // go and find a random locked flipper in the current page
                    var newItem = GetRandomItem(flippers , flippersItemsPrefabs);
                    
                    // if you found the flipper
                    if (newItem != "not found")
                    {
                        // reduce from diamonds
                        diamonds -= flippersPrice;
                        
                        // save dimaonds number in the memory
                        PlayerPrefs.SetInt("diamonds",diamonds);
                        
                        // showing new value of diamonds
                        //diamondsTextObject.GetComponent<TextMeshProUGUI>().text = diamonds.ToString();
                        
                        // adding new unlocked flipper to the list of flippers  
                        flippers += newItem;
                        
                        // saving new value of flippers list
                        PlayerPrefs.SetString("flippers" , flippers);
                        
                        //showing new flipper as unlocked (basically this function works from list of balls, flippers)
                        SetStoreItems();
                        
                        // play "Buy" sound
                        FindObjectOfType<AudioController>().PlaySound("Buy" , 0.5f);
                    }
                }
            }
        }
    }
    
    string GetRandomItem(string data , List<GameObject> itemsPrefabs)
    {
        var firstIndex = Mathf.Max(1,content.GetComponent<SwipeScript>().curPage * 9);
        var secondIndex = content.GetComponent<SwipeScript>().curPage*9 +9;

        // getting a random item (ball / flipper) in current page
        var rnd = int.Parse(Random.Range(firstIndex,secondIndex).ToString());
        
        // if the random item wasn't in the item list, 
        if (!data.Contains("," + rnd + ","))
        {
            // we add it to the list
            return rnd + ",";
        }
        else
        {
            for (int i = firstIndex; i < secondIndex; i++)
            {
                // if the random item found in current page, we call this function again to find another
                if (!data.Contains("," + i + ","))
                {
                    return GetRandomItem(data , itemsPrefabs);
                }
            }
            
            // all itens in current page found, so we return "not found" 
            return "not found";
        }
    }
    
    void DelItems()
    {
        // we call this function when tab change.
        // deleting all items (balls / flippers) 
        
        foreach (Transform child in content.transform) {
            Destroy(child.gameObject);
        }
        
        foreach (Transform child in pageCirclesGO.transform) {
            Destroy(child.gameObject);
        }
        
        ballItemsPrefabs.Clear();
        flippersItemsPrefabs.Clear();
        storePageCircles.Clear();
    }
    
    void CreateItems()
    {
        List<Sprite> curItemsType;
        if (curTab == 0)
        {
            curItemsType =  ballItems;
        }
        else
        {
            curItemsType = flipperItems;
        }
        
        GameObject curStoreGroup = null;
        for (int i = 0; i < curItemsType.Count ; i++)
        {
            // creating StoreGroups for adding items in them (every 9 items will go in one StoreGroups)
            if (i % 9 == 0 )
            {
                curStoreGroup = Instantiate(storeGroup, content.transform, true);
                curStoreGroup.transform.localScale = Vector3.one;
                
                // creating a StorePageCircle
                CreateStorePageCircle(pageCirclesGO);
            }
            
            // creating all items in current item tab 
            if (curStoreGroup != null)
            {
                GameObject newStoreItem = Instantiate(storeItem, curStoreGroup.transform, true);
                newStoreItem.transform.localScale = Vector3.one;
                
                // adding button listener for items 
                var num = i;
                newStoreItem.GetComponent<Button>().onClick.AddListener(() => { BtnStoreItemClicked(num); });

                // setting sprite of items
                var imageComponent = newStoreItem.transform.GetChild(0).GetComponent<Image>();
                imageComponent.sprite = curItemsType[i];
                imageComponent.SetNativeSize();
                
                // setting size of items
                var rectTransform = newStoreItem.transform.GetChild(0).GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x / 2, rectTransform.sizeDelta.y / 2);
                
                // adding created items (the prefab) to their list 
                if (curTab == 0)
                {
                    ballItemsPrefabs.Add(newStoreItem);
                }
                else
                {
                    flippersItemsPrefabs.Add(newStoreItem);
                }

            }
        }

    }

    void CreateStorePageCircle(GameObject pageCircles)
    {
        // creating store points based on page count 
        GameObject newStorePageCircle = Instantiate(storePageCircle, pageCircles.transform, true);
        newStorePageCircle.transform.localScale = Vector3.one;
        
        storePageCircles.Add(newStorePageCircle);

        SetStorePageCircleColors();
    }

    public void SetStorePageCircleColors()
    {
        // here we set the color of StorePageCircles based on current page
        foreach (var circle in storePageCircles)
        {
            var tempColor = circle.GetComponent<Image>().color;
            tempColor = new Color(255, 255, 255, 0.2f);
            circle.GetComponent<Image>().color = tempColor;
        }
        
        storePageCircles[Mathf.Min(content.GetComponent<SwipeScript>().curPage , storePageCircles.Count-1)].GetComponent<Image>().color = new Color(254,217,0);
    }
    
    private int selectedIndex = -1; // To store the index of the selected button

    private void SetStoreItems()
    {
        // Here we show all items as unlocked
        List<GameObject> itemsPrefabs;

        if (curTab == 0)
        {
            itemsPrefabs = ballItemsPrefabs;
        }
        else
        {
            itemsPrefabs = flippersItemsPrefabs;
        }

        // Loop through all itemPrefabs and mark them as unlocked
        for (int i = 0; i < itemsPrefabs.Count; i++)
        {
            var index = i; // Capture the current index for the lambda
            var itemsPrefab = itemsPrefabs[i];

            // Set unlocked appearance
            itemsPrefab.GetComponent<Image>().sprite = GetStoreItemsBg("unlocked");
            itemsPrefab.GetComponent<Button>().interactable = true;

            // Reset transparency or appearance for unlocked state
            var color = itemsPrefab.transform.GetChild(0).GetComponent<Image>().color;
            color.a = 1f; // Fully opaque
            itemsPrefab.transform.GetChild(0).GetComponent<Image>().color = color;

            // Add a click event to set the "selected" sprite when clicked
            itemsPrefab.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnPrefabClicked(index, itemsPrefabs);
                if (curTab == 0)
                {
                    SaveBall(index);
                }
                else
                {
                    SaveFlipper(index);
                }
            });
        }

        // Optionally, set a default "selected" item
        if (itemsPrefabs.Count > 0)
        {
            selectedIndex = 0;
            itemsPrefabs[0].GetComponent<Image>().sprite = GetStoreItemsBg("selected");
        }
    }

    private void OnPrefabClicked(int index, List<GameObject> itemsPrefabs)
    {
        // Deselect all items
        foreach (var itemsPrefab in itemsPrefabs)
        {
            itemsPrefab.GetComponent<Image>().sprite = GetStoreItemsBg("unlocked");
        }

        // Select the clicked item
        itemsPrefabs[index].GetComponent<Image>().sprite = GetStoreItemsBg("selected");

        // Save the selected index
        selectedIndex = index;

        // Debugging (optional)
        Debug.Log("Selected Index: " + selectedIndex);
    }


    public void SaveBall(int index)
    {
        PlayerPrefs.SetInt("BallIndex", index);
    }

    public int GetBallIndex()
    {
       return PlayerPrefs.GetInt("BallIndex");
    }


    public void SaveFlipper(int index)
    {
        PlayerPrefs.SetInt("FlipperIndex", index);
    }

    public int GetFlipperIndex()
    {
        return PlayerPrefs.GetInt("FlipperIndex");
    }

    private Sprite GetStoreItemsBg(string type)
    {
        // returning sprite of requested StoreItemsBg
        return Resources.Load<Sprite>("Sprites/Store/StoreItemsBg/" + type);
    }

    public void BtnStoreItemClicked(int num)
    {
        // if it's balls tab
        if (curTab == 0)
        {
            // if the button is interactable (unlocked)
            if (ballItemsPrefabs[num].GetComponent<Button>().interactable)
            {
                // change selected ball in balls list
                balls = balls.Replace(balls.Split('|')[0] + "|", num + "|");
                
                // save new value of balls in memory
                PlayerPrefs.SetString("balls" , balls);
                
                //showing the ball as selected
                SetStoreItems();
            }
        }
        else
        {
            // if the button is interactable (unlocked)
            if (flippersItemsPrefabs[num].GetComponent<Button>().interactable)
            {
                // change selected ball in flippers list
                flippers = flippers.Replace(flippers.Split('|')[0] + "|", num + "|");
                
                // save new value of flippers in memory
                PlayerPrefs.SetString("flippers" , flippers);
                
                //showing the flipper as selected
                SetStoreItems();
            }
        }
        
        
    }

    public void TabClicked(int num)
    {
        if (curTab != num)
        {
            // deleting and creating new items for the clicked tab
            curTab = num;
            DelItems();
            CreateItems();
            
            content.GetComponent<SwipeScript>().tabChanged = true;
            
            // show which items are locked / unlocked / selected
            SetStoreItems();
        
            tabLine.GetComponent<Image>().color = tabButtons[num].GetComponent<Image>().color;
        
            // animation of tab change
            for (int i = 0; i < tabButtons.Length; i++)
            {
                LeanTween.moveLocalY(tabButtons[i], 15f, 0.2f).setEaseOutBack();
            }

            LeanTween.moveLocalY(tabButtons[num], -2f, 0.2f).setEaseOutBack();

            // setting price value of btnUnlocked
            if (curTab == 0)
            {
                //btnUnlockPriceTextObject.GetComponent<TextMeshProUGUI>().text = ballsPrice.ToString();
            }
            else
            {
                //btnUnlockPriceTextObject.GetComponent<TextMeshProUGUI>().text = flippersPrice.ToString();
            }
        }
        
    }
}

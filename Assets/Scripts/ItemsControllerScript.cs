using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ItemsControllerScript : MonoBehaviour
{
    public Dictionary<string, Sprite> spritesDic = new Dictionary<string, Sprite>();
    public GameObject teleportPrefab;
    public float itemTimer = 10f;
    public float createItemStartTime = 3f;
    public float createItemEndTime = 3f;
    
    private GameController gameController;
    
    public enum Items
    {
        // list of items 
        None,
        BigBasket,
        Teleport,
        TwoX
    }
    
    public Items curAvailableItem;

    public GameObject itemPrefab;
    
    void Awake()
    {
        // getting items sprite and adding them to spritesDic
        
        spritesDic.Add("itemUI_2X", Resources.Load<Sprite>("Sprites/ItemUI/itemUI_2X"));
        spritesDic.Add( "itemUI_BigBasket" ,Resources.Load<Sprite>("Sprites/ItemUI/itemUI_BigBasket"));
        spritesDic.Add( "itemUI_Teleport" ,Resources.Load<Sprite>("Sprites/ItemUI/itemUI_Teleport"));
        
        spritesDic.Add( "item2X" ,Resources.Load<Sprite>("Sprites/Item/item2X"));
        spritesDic.Add( "itemBigBasket" ,Resources.Load<Sprite>("Sprites/Item/itemBigBasket"));
        spritesDic.Add( "itemTeleport" ,Resources.Load<Sprite>("Sprites/Item/itemTeleport"));
    }

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();

        ItemActivation(false , Items.None);
        
    }

    public void TryToCreateItem()
    {
        // here we determine which level we should add which item 
        
        // in levels 3 & 4, we just create item BigBasket
        if (gameController.level >= 3 && gameController.level <= 4)
        {
            CreateItem(Items.BigBasket);
        }
        // between levels 5-7 we just create items BigBasket & Teleport
        else if (gameController.level >= 5 && gameController.level <= 7)
        {
            if (Random.Range(0, 2) == 0)
            {
                CreateItem(Items.BigBasket);
            }
            else
            {
                CreateItem(Items.Teleport);
            }
        }
        // in levels bigger than 7, we create all kind of items (BigBasket / Teleport / 2X score)
        else if (gameController.level > 7)
        {
            CreateItem((Items) Random.Range(1, 4));
        }
    }

    public void CreateItem(Items itemType)
    {
        // if game started
        if (!gameController.txtTap && !FindObjectOfType<ItemScript>() && !transform.GetChild(0).gameObject.activeSelf)
        {
            // create a new item on a random position
            GameObject game = GameObject.Find("Game");
            var x = Random.Range(-1f, 1f);
            var y = Random.Range(3f, -0.5f);
            GameObject item = Instantiate(itemPrefab , new Vector3(x,y,0f),transform.rotation);
            item.transform.SetParent(game.transform);
            item.GetComponent<CircleCollider2D>().enabled = false;
            
            var itemSprite = item.GetComponent<SpriteRenderer>().sprite;
        
            // here we get sprite of the item based on its type (itemType parameter)
            switch (itemType)
            {
                case Items.BigBasket:
                    itemSprite = spritesDic["itemBigBasket"];
                    break;
                case Items.Teleport:
                    itemSprite = spritesDic["itemTeleport"];
                    break;
                case Items.TwoX:
                    itemSprite = spritesDic["item2X"];
                    break;
            }

            item.GetComponent<SpriteRenderer>().sprite = itemSprite;
        
            // alpha tween animation
            gameController.SetAlpha(item,0);
            LeanTween.value(item, 0, 1, 0.3f).setEaseOutQuint().setOnUpdate((float val) => {
                gameController.SetAlpha(item,val);
            });
        
            // scale tween animation
            item.transform.localScale = Vector3.zero;
            LeanTween.scale(item, Vector3.one, 0.3f).setEaseOutBack().setOnComplete(() =>
            {
                item.GetComponent<CircleCollider2D>().enabled = true;
            });
        }
        
    }

    public void ItemActivation(bool active , Items item)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }

        if (active)
        {
            var spritesDictionary = GetComponent<ItemsControllerScript>().spritesDic;
            var itemUIImage = transform.GetChild(0).gameObject.GetComponent<Image>();

            if (itemUIImage != null && spritesDictionary != null)
            {
                // setting sprite of item (it appears at top-left of the screen)
                switch (item)
                {
                    case Items.TwoX:
                        itemUIImage.sprite = spritesDictionary["itemUI_2X"];
                        break;
                    case Items.BigBasket:
                        itemUIImage.sprite = spritesDictionary["itemUI_BigBasket"];
                        break;
                    case Items.Teleport:
                        itemUIImage.sprite = spritesDictionary["itemUI_Teleport"];
                        break;
                }
            
                // setting size of the item to native size
                itemUIImage.SetNativeSize();

                curAvailableItem = item;
                
                // resetting item's progress bar 
                transform.GetChild(1).GetComponent<Image>().fillAmount = 1;

                // count down of item's progress bar 
                LeanTween.value(gameObject, 1, 0, itemTimer).setEaseLinear()
                    .setOnUpdate((val) => {
                        if (transform.GetChild(1).gameObject.activeInHierarchy)
                        {
                            transform.GetChild(1).GetComponent<Image>().fillAmount = val;
                        }
                        
                    })
                    // when the progress bar end, we set current item to None
                    .setOnComplete(() => {
                        if (transform.GetChild(1).gameObject.activeInHierarchy)
                        {
                            ItemActivation(false, Items.None);
                        }
                        
                    });
                
                // creating teleports if item is Teleport
                if (item == Items.Teleport)
                {
                    CreateTeleoports();
                }
                // activing Hoop2x object on top of the basket (it's just for show)
                else if (item == Items.TwoX)
                {
                    BasketScript basket = FindObjectOfType<BasketScript>();
                    var hoop2XGameObject = basket.Hoop2x.gameObject;
                    hoop2XGameObject.SetActive(true);
                    
                    gameController.SetAlpha(hoop2XGameObject,0);
                    LeanTween.value(hoop2XGameObject, 0, 1, 0.5f).setEaseOutQuint().setOnUpdate(
                        (float val) => { gameController.SetAlpha(hoop2XGameObject, val); });

                }
            }
        }
        // deactivating current item
        else
        {
            transform.GetChild(1).GetComponent<Image>().fillAmount = 0;
            curAvailableItem = Items.None;
            DeleteTeleports();
            
            // call TryToCreateItem in a random time
            Invoke("TryToCreateItem", Random.Range(createItemStartTime,createItemEndTime));
            
            BasketScript basket = FindObjectOfType<BasketScript>();
            basket.Hoop2x.gameObject.SetActive(false);
            
        }
    }

    void CreateTeleoports()
    {
        // creating teleports when item Teleport is activated
        GameObject teleports = new GameObject();
        teleports.name = "Teleports";
        
        GameObject game = GameObject.Find("Game");
        teleports.transform.SetParent(game.gameObject.transform);
        
        GameObject tele = Instantiate(teleportPrefab,new Vector3(-1.4f,-4,0f),transform.rotation);
        tele.name = "TeleportBottom";
        tele.transform.SetParent(teleports.gameObject.transform);
        gameController.SetAlpha(tele,0);
        LeanTween.value(tele, 0, 1, 0.5f).setEaseOutQuint().setOnUpdate(
            (float val) => { gameController.SetAlpha(tele, val); });

        GameObject tele2 = Instantiate(teleportPrefab,new Vector3(-2.63f,-3.36f,0f),transform.rotation);
        tele2.name = "TeleportBottom";
        tele2.transform.SetParent(teleports.gameObject.transform);
        tele2.transform.localScale = new Vector3(0.5f,1,1);
        Quaternion initialRot = tele2.transform.rotation;
        tele2.transform.localRotation = initialRot * Quaternion.Euler(0, 0, -90f);
        gameController.SetAlpha(tele2,0);
        LeanTween.value(tele2, 0, 1, 0.5f).setEaseOutQuint().setOnUpdate(
            (float val) => { gameController.SetAlpha(tele2, val); });
    }

    void DeleteTeleports()
    {
        // deleting teleports (when item timer finished)
        GameObject teleports = GameObject.Find("Teleports");
        if (teleports)
        {
            Destroy(teleports);
        }
        
    }
}

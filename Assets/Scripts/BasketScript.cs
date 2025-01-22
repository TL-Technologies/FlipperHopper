using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BasketScript : MonoBehaviour
{
    public bool rightSide;
    public GameObject net2skin;
    public GameObject handle;
    public GameObject basketTop;
    public GameObject Hoop2x;
    public bool bigSize;
    
    GameObject newBasket;
    private GameController gameController;
    private ItemsControllerScript itemsControllerScript;
     
    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        gameObject.GetComponent<BasketScript>().net2skin.gameObject.SetActive(true);
        bigSize = false;

        itemsControllerScript = FindObjectOfType<ItemsControllerScript>();
    }

    public void MoveOut()
    {
        // setting z of basket
        SetZ(gameObject.GetComponent<BasketScript>().net2skin.transform, 0f);
        SetZ(gameObject.GetComponent<BasketScript>().basketTop.transform, -1f);
        
        // moving basket out then calling "CreateNew" function
        if (!rightSide)
        {
            LeanTween.moveX(gameObject, transform.position.x - 3, 0.2f).setOnComplete(CreateNew);
        }
        else
        {
            LeanTween.moveX(gameObject, transform.position.x + 3, 0.2f).setOnComplete(CreateNew);
        }
    }

    public void CreateNew()
    {
        gameController = FindObjectOfType<GameController>();
        if (gameController)
        {
            // if we haven't reach to the level target
            if (gameController.levelScore < gameController.levelsList[gameController.tempLevel].ticksCount)
            {
                // here we create a new basket (at a random y position between 0.3f, 3f). then we move it in screen.
                Vector3 newPos;

                float randomY = Random.Range(0.3f, 3f);
                if (!rightSide)
                {
                    newPos = new Vector3(4,randomY,transform.position.z);
                    newBasket = Instantiate(this.gameObject, newPos, transform.rotation);
                    newBasket.gameObject.GetComponent<BasketScript>().rightSide = true;

                    LeanTween.moveX(newBasket, 1.66f, 0.4f).setDelay(0.01f)
                        .setOnComplete(() =>
                        {
                            SetZ(newBasket.gameObject.GetComponent<BasketScript>().net2skin.transform, -7f);
                            SetZ(newBasket.gameObject.GetComponent<BasketScript>().basketTop.transform, -8f);

                        });
                }
                else
                {
                    newPos = new Vector3(-4,randomY,transform.position.z); 
                    newBasket = Instantiate(this.gameObject, newPos, transform.rotation);
                    newBasket.gameObject.GetComponent<BasketScript>().rightSide = false;
            
                    LeanTween.moveX(newBasket, -1.78f, 0.4f).setDelay(0.01f)
                        .setOnComplete(() =>
                        {
                            SetZ(newBasket.gameObject.GetComponent<BasketScript>().net2skin.transform, -7f);
                            SetZ(newBasket.gameObject.GetComponent<BasketScript>().basketTop.transform, -8f);

                        });
                }

                newBasket.gameObject.name = "Basket";
        
                // setting position of basket handle based on which side it created
                GameObject handle = newBasket.gameObject.GetComponent<BasketScript>().handle;
                var transformPosition = handle.transform.localPosition;
                transformPosition.x = -handle.transform.localPosition.x;
                handle.transform.localPosition = transformPosition;
        
                Destroy(gameObject);
            }
            else
            {
                gameController.LevelDone();
            }
        }
        
    }

    void SetZ(Transform curTransform , float z)
    {
        // setting z transform 
        
        var transformPosition = curTransform.position;
        transformPosition.z = z;
        curTransform.position  = transformPosition;
        
    }

    private void Update()
    {
        // always check for big size item
        BigSize();
        
    }

    void BigSize()
    {
        // if item big size is enable
        if ((itemsControllerScript.curAvailableItem == ItemsControllerScript.Items.BigBasket) && (!bigSize))
        {
            // do the big size animation for the basket
            
            bigSize = true;

            float localPosX;
            if (rightSide)
                localPosX = -0.107f;
            else
                localPosX = 0.107f;

            LeanTween.scaleX(transform.GetChild(1).gameObject, 1.16f, 0.5f).setEaseOutBack();
            LeanTween.moveLocalX(transform.GetChild(1).gameObject, localPosX, 0.5f).setEaseOutBack();
        }
        else if ((itemsControllerScript.curAvailableItem != ItemsControllerScript.Items.BigBasket && bigSize) || itemsControllerScript.curAvailableItem == ItemsControllerScript.Items.None)
        {
            // do the normal size animation for the basket
            
            if (bigSize)
            {
                bigSize = false;
            
                LeanTween.scaleX(transform.GetChild(1).gameObject, 1f, 0.5f).setEaseOutBack();
                LeanTween.moveLocalX(transform.GetChild(1).gameObject, 0f, 0.5f).setEaseOutBack();
            }
            
        }
    }
}

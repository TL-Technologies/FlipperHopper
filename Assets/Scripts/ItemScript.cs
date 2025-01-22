using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemScript : MonoBehaviour
{
    private ItemsControllerScript itemsControllerScript;
    private GameController gameController;

    private void Start()
    {
        itemsControllerScript = FindObjectOfType<ItemsControllerScript>();
        gameController = FindObjectOfType<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if item icon collide with the ball, we activate the item
        var spriteString = GetComponent<SpriteRenderer>().sprite.ToString();
        if (spriteString.Contains("2X"))
        {
            itemsControllerScript.ItemActivation(true,ItemsControllerScript.Items.TwoX);
        }
        else if (spriteString.Contains("BigBasket"))
        {
            itemsControllerScript.ItemActivation(true,ItemsControllerScript.Items.BigBasket);
        }
        else if (spriteString.Contains("Teleport"))
        {
            itemsControllerScript.ItemActivation(true,ItemsControllerScript.Items.Teleport);
        }

        // destroying item icon
        Destroy(gameObject);
        
        //calling TryToCreateItem after a random time 
        itemsControllerScript.Invoke("TryToCreateItem", 
            Random.Range(itemsControllerScript.createItemStartTime + itemsControllerScript.itemTimer,itemsControllerScript.createItemEndTime + itemsControllerScript.itemTimer));
        
        // playing "Item" sound
        gameController.GetComponent<AudioController>().PlaySound("Item" , 0.1f);
    }
}

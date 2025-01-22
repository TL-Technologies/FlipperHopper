using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private bool firstTriggerPassed;
    
    private GameController gameController;
    private ItemsControllerScript itemsControllerScript;
    
    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        itemsControllerScript = FindObjectOfType<ItemsControllerScript>();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        // if the ball passed the top trigger of basket, we set firstTriggerPassed to true
        if ( (other.gameObject.name == "Trigger") && (!firstTriggerPassed))
        {
            if (gameObject.transform.position.y < other.gameObject.transform.position.y)
            {
                firstTriggerPassed = true;
            }
        }
        
        // if the ball passed the TriggerBottom and already passed top trigger
        if ( (other.gameObject.name == "TriggerBottom") && (firstTriggerPassed) )
        {
            if (this.gameObject.transform.position.y < other.gameObject.transform.position.y)
            {
                Vector3 basketPos = Vector3.zero;
                
                if (basketPos == Vector3.zero)
                {
                    basketPos = other.transform.position;
                    basketPos.y += 0.7f;
                }
                
                // add to score 
                gameController.AddScore();
                
                // reset firstTriggerPassed
                firstTriggerPassed = false;
                
                // add a particle then destroy it in 1 sec
                var newParticles = Instantiate(gameController.basketParticles , gameController.ballPrefab.transform.position , transform.rotation);
                Destroy(newParticles,1f);

                // play "Score" sound
                gameController.GetComponent<AudioController>().PlaySound("Score" , 1);
            }
        }
        
        // if the ball passed the TeleportBottom
        if ( other.gameObject.name == "TeleportBottom" )
        {
            // reset the ball position to the top and deactive it
            transform.position = new Vector3(0,4.40f , transform.position.z);
            gameObject.SetActive(false);

            LeanTween.delayedCall(0.3f, () =>
            {
                // activating the ball 
                gameObject.SetActive(true);
                transform.GetChild(0).gameObject.SetActive(true);
                gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.down,0);
                
                // creating a new teleport at top (it's just for show)
                CreateTeleTop();
            });
        }
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        // if the ball hit a solid object and if its y position is >= 1.5 
        if (transform.position.y >= -1.5f )
        {
            // play "Hit" sound
            gameController.GetComponent<AudioController>().PlaySound("Hit" , 0.3f);
        }
        
    }
    
    void CreateTeleTop()
    {
        // creating a new teleport object at top (when the ball falls at top) and animate it via lean tween 
        if (!GameObject.Find("TeleportTop"))
        {
            transform.GetChild(0).gameObject.SetActive(false);
                
            GameObject tele = Instantiate(itemsControllerScript.teleportPrefab,new Vector3(0f,4.5f,0f),gameController.transform.rotation);
            tele.name = "TeleportTop";
            
            GameObject teleports = GameObject.Find("Teleports");
            tele.transform.SetParent(teleports.gameObject.transform);

            var localScale = tele.transform.localScale;
            localScale = new Vector3(0.7f,1,1);
            tele.transform.localScale = localScale;
            Quaternion initialRot = tele.transform.rotation;
            tele.transform.localRotation = initialRot * Quaternion.Euler(0, 0, 180f);

            LeanTween.scale(tele, new Vector3(localScale.x * 1.7f,localScale.y * 1.7f), 0.8f).setEaseOutQuint();

            LeanTween.value(tele, 1, 0, 0.5f).setDelay(0.3f).setEaseOutQuint().setOnUpdate(
                (float val) => { gameController.SetAlpha(tele, val); }).setDestroyOnComplete(true);
        }
    }
    
}

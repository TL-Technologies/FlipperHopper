using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipperScript : MonoBehaviour
{
    private HingeJoint2D hingeJoint2D;
    public float rotSpeed = 0;
    public int maxRotSpeed = 0;
    public int rotAmount = 0;
    public int maxAngle = 0;
    JointMotor2D jointMotor2D;
    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        hingeJoint2D = GetComponent<HingeJoint2D>();
        jointMotor2D = hingeJoint2D.motor;
        var jointAngleLimits2D = hingeJoint2D.limits;
        jointAngleLimits2D.max = maxAngle;
        GetComponent<HingeJoint2D>().limits = jointAngleLimits2D;
    }
    
    void Update()
    {
        // changing motorSpeed of flipper while we are touching or not 
        
        if (gameController && gameController.ballPrefab && gameController.ballPrefab.activeSelf)
        {
            if (!gameController.btnStore || (!gameController.btnStore.GetComponent<buttonScript>().isPressed  && !gameController.btnSound.GetComponent<buttonScript>().isPressed))
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
                {
                    rotSpeed = 0;
                }

                if (Input.GetMouseButton(0))
                {
                    rotSpeed = Mathf.Min(maxRotSpeed, rotSpeed + (rotAmount*Time.deltaTime*60));
            
                }
                else if(rotSpeed > -maxRotSpeed)
                {
                    rotSpeed = Mathf.Max(-maxRotSpeed, rotSpeed - (rotAmount*Time.deltaTime*60));
                }

                jointMotor2D.motorSpeed = rotSpeed;
            }
        }
    }

    private void FixedUpdate()
    {
        GetComponent<HingeJoint2D>().motor = jointMotor2D;
    }
    
    
}

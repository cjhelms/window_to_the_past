using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject activePlayer;

    [Header ("Movement Variables")]
    //Movement
    bool moving = true;
    Vector3 moveVector;
    bool rightInput;
    bool leftInput;
    bool upInput;
    bool downInput;
    int xVelocity;
    int yVelocity;
    Vector3 velocityVector;
    float accTimer;
    [SerializeField] float acceleration;
    [SerializeField] float speed;

    //Dash
    bool dashing = false;
    float dashTimer;
    float dashStartX;
    float dashStartY;
    float dashX;
    float dashY;
    Vector3 dashEnd;
    [SerializeField] float dashDis;
    [SerializeField] float dashTime;

    void Update()
    {
        //Moving around
        if (moving)
        {
            //Listen for inputs
            if (Input.GetKey(KeyCode.RightArrow)) {rightInput = true;} else {rightInput = false;}
            if (Input.GetKey(KeyCode.LeftArrow))  {leftInput = true;}  else {leftInput = false;}
            if (Input.GetKey(KeyCode.UpArrow))    {upInput = true;}    else {upInput = false;}
            if (Input.GetKey(KeyCode.DownArrow))  {downInput = true;}  else {downInput = false;}

            //Set velocity targets
            if (rightInput) {xVelocity = 1;}
            if (leftInput)  {xVelocity = -1;}
            if ((rightInput && leftInput) || (!rightInput && !leftInput)) {xVelocity = 0;}
            if (upInput)    {yVelocity = 1;}
            if (downInput)  {yVelocity = -1;}
            if ((upInput && downInput) || (!upInput && !downInput))       {yVelocity = 0;}
            //Create vector and normalize
            velocityVector = new Vector3(xVelocity, yVelocity, 0).normalized;

            //Adjust velocity x
            if (moveVector.x < velocityVector.x) 
            {
                moveVector.x += acceleration;
                if (moveVector.x > 1) {moveVector.x = 1;}
            }
            if (moveVector.x > velocityVector.x)
            {
                moveVector.x -= acceleration;
                if (moveVector.x < -1) {moveVector.x = -1;}
            }

            //Adjust velocity y
            if (moveVector.y < velocityVector.y) 
            {
                moveVector.y += acceleration;
                if (moveVector.y > 1) {moveVector.y = 1;}
            }
            if (moveVector.y > velocityVector.y)
            {
                moveVector.y -= acceleration;
                if (moveVector.y < -1) {moveVector.y = -1;}
            }

            //Round near zero
            if (Mathf.Abs(moveVector.x) < acceleration) {moveVector.x = 0;}
            if (Mathf.Abs(moveVector.y) < acceleration) {moveVector.y = 0;}

            //Move active player
            activePlayer.transform.Translate(moveVector * speed * Time.deltaTime);
        }
        
        //Dash input
        if (Input.GetKeyDown(KeyCode.Space)) { DashInitiated(); }

        //Dashing
        if (dashing) { Dashing(); }
    }

    void DashInitiated ()
    {
        moving = false;
        dashing = true;

        //Setting numbers for dash
        dashStartX = activePlayer.transform.position.x;
        dashEnd.x = dashStartX + (moveVector.normalized.x * dashDis);
        dashStartY = activePlayer.transform.position.y;
        dashEnd.y = dashStartY + (moveVector.normalized.y * dashDis);
        dashTimer = 0;
    }

    void Dashing ()
    {
        if (dashTimer < dashTime)
        {
            dashX = Mathf.Lerp(dashStartX, dashEnd.x, dashTimer/dashTime);
            dashY = Mathf.Lerp(dashStartY, dashEnd.y, dashTimer/dashTime);
            dashTimer += Time.deltaTime;

            activePlayer.transform.position = new Vector3(dashX, dashY, 0);
        }
        else {dashing = false; moving = true;}
    }
}

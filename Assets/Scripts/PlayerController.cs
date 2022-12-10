using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject activePlayer;

    [Header ("Movement Variables")]
    [SerializeField] float speed;
    [SerializeField] float dashDis;
    [SerializeField] float dashTime;

    bool dash = false;

    Vector3 moveVector;

    //Dash
    float dashTimer;
    float dashStartX;
    float dashStartY;
    float dashX;
    float dashY;
    Vector3 dashEnd;

    void Update()
    {
        //Moving around
        if (!dash)
        {
            //Set move vector
            moveVector.x = 0; moveVector.y = 0;
            if (Input.GetKey(KeyCode.RightArrow))   {moveVector.x = 1f;}
            if (Input.GetKey(KeyCode.LeftArrow))    {moveVector.x = -1f;}
            if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow)) {moveVector.x = 0;}
            if (Input.GetKey(KeyCode.UpArrow))      {moveVector.y = 1f;}
            if (Input.GetKey(KeyCode.DownArrow))    {moveVector.y = -1f;}
            if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow))    {moveVector.y = 0;}

            //Move active player
            activePlayer.transform.Translate(moveVector.normalized * speed * Time.deltaTime);
        }
        
        //Dash input
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            dash = true;
            
            dashStartX = activePlayer.transform.position.x;
            dashEnd.x = dashStartX + (moveVector.normalized.x * dashDis);
            dashStartY = activePlayer.transform.position.y;
            dashEnd.y = dashStartY + (moveVector.normalized.y * dashDis);
            dashTimer = 0;
        }

        //Dashing
        if (dash)
        {
            if (dashTimer < dashTime)
            {
                dashX = Mathf.Lerp(dashStartX, dashEnd.x, dashTimer/dashTime);
                dashY = Mathf.Lerp(dashStartY, dashEnd.y, dashTimer/dashTime);
                dashTimer += Time.deltaTime;

                activePlayer.transform.position = new Vector3(dashX, dashY, 0);
            }
            else {dash = false;}
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float FlashbackTime = 10.0f;

    [Header ("Movement Variables")]
    //Movement
    //bool moving = true;
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
    //bool dashing = false;
    float dashTimer;
    Vector3 dashStart;
    float dashX;
    float dashY;
    Vector3 dashEnd;
    [SerializeField] float dashDis;
    [SerializeField] float dashTime;
    [SerializeField] GameObject dashTrailPrefab;
    GameObject dashTrail;

    enum State
    {
        Active,
        Dashing,
        Inactive,
        Rewind,
        Replay
    }

    State state;    

    void Start() {}

    // TODO: Known bug, if you flashback or collapse while enemy is charging or cooling down, that
    // enemy will forever be stuck in that state
    void Update()
    {
        switch (state)
        {
            case State.Active:
                InputListening();
                break;
            case State.Dashing:
                Dashing();
                break;
        }
    }

    void InputListening ()
    {
        Movement();

        //Dash input
        if (Input.GetKeyDown(KeyCode.D) && moveVector.magnitude != 0) { DashInitiated(); }

        // Handle flashback and collapse commands
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Flashback requested!");
            //lm.Flashback(FlashbackTime);
            SendMessageUpwards("HandleFlashbackRequest");
        }
        else if(Input.GetKeyDown(KeyCode.Backspace))
        {
            Debug.Log("Collapse requested!");
            //lm.Collapse();
            SendMessageUpwards("Collapse");
        }
    }

    void Movement ()
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
            moveVector.x += acceleration * Time.deltaTime;
            if (moveVector.x > 1) {moveVector.x = 1;}
        }
        else if (moveVector.x > velocityVector.x)
        {
            moveVector.x -= acceleration * Time.deltaTime;
            if (moveVector.x < -1) {moveVector.x = -1;}
        }

        //Adjust velocity y
        if (moveVector.y < velocityVector.y) 
        {
            moveVector.y += acceleration * Time.deltaTime;
            if (moveVector.y > 1) {moveVector.y = 1;}
        }
        else if (moveVector.y > velocityVector.y)
        {
            moveVector.y -= acceleration * Time.deltaTime;
            if (moveVector.y < -1) {moveVector.y = -1;}
        }

        //Round near zero (only do this is no key pressed?)
        if (!rightInput && !leftInput && !upInput && !downInput)
        {
            if (Mathf.Abs(moveVector.x) < 0.2f) {moveVector.x = 0;}
            if (Mathf.Abs(moveVector.y) < 0.2f) {moveVector.y = 0;}
        }
        
        //Move active player
        gameObject.transform.Translate(moveVector * speed * Time.deltaTime);
    }

    void DashInitiated ()
    {
        //moving = false;

        //Setting numbers for dash
        dashStart.x = gameObject.transform.position.x;
        dashEnd.x = dashStart.x + (moveVector.normalized.x * dashDis);
        dashStart.y = gameObject.transform.position.y;
        dashEnd.y = dashStart.y + (moveVector.normalized.y * dashDis);
        dashTimer = 0;

        InstantiateDashTrail();

        //dashing = true;
        state = State.Dashing;
    }

    void Dashing ()
    {
        if (dashTimer < dashTime)
        {
            dashX = Mathf.Lerp(dashStart.x, dashEnd.x, dashTimer/dashTime);
            dashY = Mathf.Lerp(dashStart.y, dashEnd.y, dashTimer/dashTime);
            dashTimer += Time.deltaTime;

            gameObject.transform.position = new Vector3(dashX, dashY, 0);

            //Update dash trail
            Vector3 halfway = (dashStart + gameObject.transform.position) / 2;
            dashTrail.transform.position = halfway;
            Vector3 difference = gameObject.transform.position - dashStart;
            float distanceTraveled = Mathf.Abs(difference.magnitude);
            dashTrail.transform.localScale = new Vector3(distanceTraveled, 0.25f, 0);
        }
        else 
        {
            //dashing = false;
            //moving = true;
            state = State.Active;

            //When the time is right, will this function be ready?
            SendMessageUpwards("HandleDash");
        }
    }

    void InstantiateDashTrail ()
    {
        dashTrail = Instantiate(dashTrailPrefab, gameObject.transform.position, Quaternion.identity);

        Vector3 rot = new Vector3();
        if (xVelocity == 1 && yVelocity == 1)           {rot.z = 45;}
        else if (xVelocity == -1 && yVelocity == 1)     {rot.z = 135;}
        else if (xVelocity == -1 && yVelocity == -1)    {rot.z = 225;}
        else if (xVelocity == 1 && yVelocity == -1)     {rot.z = 315;}
        else if (yVelocity == 1)                        {rot.z = 90;}
        else if (yVelocity == -1)                       {rot.z = 270;}

        dashTrail.transform.Rotate(rot);
    }
}

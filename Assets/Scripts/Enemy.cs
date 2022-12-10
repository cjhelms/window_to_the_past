using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameObject player;
    SpriteRenderer spriteRend;

    bool chasing = true;
    Vector3 distanceToPlayer;
    [SerializeField] float attackDistance;

    Vector3 targetVector;
    Vector3 movementVector;
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
    [SerializeField] float dashCooldown;

    void Start ()
    {
        player = GameObject.Find("Player");
        spriteRend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //Vector3 pointing to player
        targetVector = player.transform.position - gameObject.transform.position;

        //The chase is on!
        if (chasing)
        {
            movementVector = targetVector.normalized * speed * Time.deltaTime;
            gameObject.transform.position += movementVector;

            //Are we in range to attack?
            if (targetVector.magnitude < attackDistance) 
            { 
                chasing = false; 
                StartCoroutine("DashPrep"); 
            } 
        }

        if (dashing) { Dashing(); }
    }

    IEnumerator DashPrep ()
    {
        //Set numbers for dash
        dashStartX = gameObject.transform.position.x;
        dashEnd.x = dashStartX + (targetVector.normalized.x * dashDis);
        dashStartY = gameObject.transform.position.y;
        dashEnd.y = dashStartY + (targetVector.normalized.y * dashDis);
        dashTimer = 0;

        //Blink animation
        spriteRend.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        spriteRend.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRend.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        spriteRend.color = Color.red;
        yield return new WaitForSeconds(0.2f);

        dashing = true;
    }

    void Dashing ()
    {
        if (dashTimer < dashTime)
        {
            dashX = Mathf.Lerp(dashStartX, dashEnd.x, dashTimer/dashTime);
            dashY = Mathf.Lerp(dashStartY, dashEnd.y, dashTimer/dashTime);
            dashTimer += Time.deltaTime;

            gameObject.transform.position = new Vector3(dashX, dashY, 0);
        }
        else { dashing = false; StartCoroutine("DashCD"); }
    }

    //Cooldown after dashing
    IEnumerator DashCD ()
    {
        spriteRend.color = Color.gray;
        yield return new WaitForSeconds(dashCooldown);
        spriteRend.color = Color.red;
        chasing = true;
    }
}

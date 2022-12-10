using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject activePlayer;

    Vector3 moveVector;
    float speed = .01f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))   {moveVector.x += 1f;}
        if (Input.GetKeyDown(KeyCode.LeftArrow))    {moveVector.x -= 1f;}
        if (Input.GetKeyDown(KeyCode.UpArrow))      {moveVector.y += 1f;}
        if (Input.GetKeyDown(KeyCode.DownArrow))    {moveVector.y -= 1f;}

        if (Input.GetKeyUp(KeyCode.RightArrow))   {moveVector.x -= 1f;}
        if (Input.GetKeyUp(KeyCode.LeftArrow))    {moveVector.x += 1f;}
        if (Input.GetKeyUp(KeyCode.UpArrow))      {moveVector.y -= 1f;}
        if (Input.GetKeyUp(KeyCode.DownArrow))    {moveVector.y += 1f;}

        activePlayer.transform.Translate(moveVector.normalized * speed);
    }
}

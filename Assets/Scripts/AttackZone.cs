using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackZone : MonoBehaviour
{
    SpriteRenderer spriteRend;
    [SerializeField] float fadeSpeed;

    void Start ()
    {
        spriteRend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (spriteRend.color.a > 0)
        {
            float temp = spriteRend.color.a;
            temp -= fadeSpeed;
            spriteRend.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, temp);
        }

        if (spriteRend.color.a <= 0) {Destroy(gameObject);}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoController : MonoBehaviour
{
    //PERSONAJE
    public float speed; 
    float velX, velY;
    Rigidbody2D rb;

    Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {   
        FlipCharacter();
    }
    private void FixedUpdate()
    {
        Movement();
    }
    public void Movement()
    {
        velX = Input.GetAxisRaw("Horizontal");
        velY = rb.velocity.y;

        rb.velocity = new Vector2(velX * speed, velY);

        if(rb.velocity.x != 0)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }
    }

    public void FlipCharacter()
    {
        if(rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(1,1,1);
        }
    }
}

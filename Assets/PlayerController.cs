using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    //Start() Variables
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    //FSM
    private enum State { idle,running,jumping,falling,hurt}
    private State state = State.idle;

    //Inseptor variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int cherries = 0;
    [SerializeField] private Text CherryText;
    [SerializeField] private float hurtForce = 10f;
    private void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (state != State.hurt)
        {
            Movement();
        }
       
        AnimationState();
        anim.SetInteger("state", (int)state);   //sets animation based on enumerator state
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
      if(other.tag=="collectable")
        {
            Destroy(other.gameObject);
            cherries += 1;
            CherryText.text = cherries.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (state == State.falling)
            {
                Destroy(collision.gameObject);
                Jump();
            }
           
        }
        else
        {
            state = State.hurt;
           // print("this is happining ");
            if (collision.gameObject.transform.position.x > transform.position.x)
            {
                rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
               // print("to my right");
            }
            else
            {
                rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                //print("to my left");
            }
        }
        
    }

    private void Movement()
    {
        float _h = Input.GetAxis("Horizontal");

        if (_h < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
            //anim.SetBool("running", true);
        }
        else if (_h > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
            //anim.SetBool("running", true);
        }
        else
        {
            //anim.SetBool("running", false);

        }
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();
        }
    }
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }

    private void AnimationState()
    {
        if (state == State.jumping)
        {
            if (rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }
        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        
        }
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
         else if (Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
        }
    }
}

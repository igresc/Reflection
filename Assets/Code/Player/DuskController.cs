using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuskController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;

    //MOVEMENT
    private float horizontal;
    public float speed;

    //JUMP
    private float jumpTimer;
    private bool isJumping;
    bool isGrounded;

    public Transform feetPos;

    public float checkRadius;
    public float jumpForce;
    public float jumpTime;

    public LayerMask whatIsGround;


    //TWIST
    bool isFacingRight;

    //ANIMATIONS
    public enum PlayerStates{
        IDLE, 
        RUN,
        JUMP,
        FALL,
        INTERACT
    }
    PlayerStates States 
    {
        set
        {
            if (stateLock) {
                currentState = value;
                switch (currentState)
                {
                    case PlayerStates.IDLE:
                        playerAnimator.Play("DuskIdle");
                        break;
                    case PlayerStates.RUN:
                        playerAnimator.Play("DuskRun");
                        break;
                    case PlayerStates.JUMP:
                        playerAnimator.Play("DuskJump");
                        stateLock = true;
                        break;
                    case PlayerStates.FALL:
                        playerAnimator.Play("DuskFall");
                        stateLock = true;
                        break;
                    case PlayerStates.INTERACT:
                        playerAnimator.Play("DuskInteract");
                        stateLock = true;
                        break;
                }
            }
            
        }
    }
    Animator playerAnimator;
    PlayerStates currentState;
    bool stateLock = true;
    bool isFalling;

    void Start()
    {
        //dashingTime = dashCooldown;
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        //if (isDashing) { return; }
        
        horizontal = Input.GetAxisRaw("Horizontal");
        Jump();
        Interact();
        
    }

    void FixedUpdate()
    {
        if (horizontal != 0) 
        {
            Movement();
        }
        if (rb.velocity.x == 0 && rb.velocity.y == 0 && isGrounded) 
        {
            States = PlayerStates.IDLE;
        }
    }

    void Movement() 
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        if (rb.velocity.y == 0) 
        {
            States = PlayerStates.RUN;
        }
        
        if (horizontal > 0 && isFacingRight)
        {
            TwistSprite();
        }
        if (horizontal < 0 && !isFacingRight)
        {
            TwistSprite();
        }
        
    }

    void Jump() 
    {
        
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        if (isGrounded == true && Input.GetKeyDown(KeyCode.Space))
        {
            
            //stateLock = true;
            isJumping = true;
            jumpTimer = jumpTime;
            rb.velocity = Vector2.up * jumpForce;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimer > 0)
            {
                States = PlayerStates.JUMP;
                rb.velocity = Vector2.up * jumpForce;
                jumpTimer -= Time.deltaTime;
            }
            else { 
                isJumping = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space) || rb.velocity.y < 0)
        {
            isJumping = false;
            States = PlayerStates.FALL;
        }
    }

    void Interact() 
    {
        if (Input.GetKeyDown(KeyCode.E) && isGrounded) 
        {
            States = PlayerStates.INTERACT;
        }
    }

    void TwistSprite() 
    {
        isFacingRight = !isFacingRight;
        Vector2 scaleFactor = transform.localScale;
        scaleFactor.x *= -1;
        transform.localScale = scaleFactor;
    }
}

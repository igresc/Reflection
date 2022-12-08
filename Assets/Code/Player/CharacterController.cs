using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
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

    //DASH
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    private bool canMove = true;
    private bool canDash = true;
    private bool isDashing = false;
    float cooldownDash = 3;

    //TWIST
    bool isFacingRight;

    //ANIMATIONS
    public enum PlayerStates{
        IDLE, 
        RUN,
        JUMP,
        FALL,
        DASH
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
                        playerAnimator.Play("Idle");
                        break;
                    case PlayerStates.RUN:
                        playerAnimator.Play("Run");
                        break;
                    case PlayerStates.JUMP:
                        playerAnimator.Play("Jump");
                        stateLock = true;
                        break;
                    case PlayerStates.FALL:
                        playerAnimator.Play("Falling");
                        stateLock = true;
                        break;
                    case PlayerStates.DASH:
                        playerAnimator.Play("Dash");
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

        //DASH
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
            //cooldownDash = 3;
        }
        Debug.Log(rb.velocity);
    }

    void FixedUpdate()
    {
        if (canMove && horizontal != 0) 
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
        if (isDashing) { return; }
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        if (isGrounded == true && Input.GetKeyDown(KeyCode.Space))
        {
            
            //stateLock = true;
            isJumping = true;
            jumpTimer = jumpTime;
            rb.velocity = Vector2.up * jumpForce;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping && !isDashing)
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
        if (Input.GetKeyUp(KeyCode.Space) || rb.velocity.y < 0 && !isDashing)
        {
            isJumping = false;
            States = PlayerStates.FALL;
        }
    }

    //DASH
    private IEnumerator Dash()
    {
        canMove = false;
        canDash = false;
        isDashing = true;
        States = PlayerStates.DASH;
        stateLock = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0);
        
        yield return new WaitForSeconds(dashingTime);
        
        canDash = true;
        canMove = true;
        isDashing = false;
        rb.gravityScale = originalGravity;
        stateLock = true;
    }

    void TwistSprite() 
    {
        isFacingRight = !isFacingRight;
        Vector2 scaleFactor = transform.localScale;
        scaleFactor.x *= -1;
        transform.localScale = scaleFactor;
    }
}

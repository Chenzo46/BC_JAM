using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using TMPro;
using Unity.Collections;
using System;
using DG.Tweening;

public class CatController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private SpriteRenderer sprend;
    [SerializeField] private Animator anim;
    [Header("Other")]
    [SerializeField] private Vector2 maxVelocity = new Vector2(20, 20);
    [SerializeField] private GameObject dustParticles;
    [SerializeField] private GameObject wallDustParticles;
    [SerializeField] private GameObject wallDustLeft;
    [SerializeField] private ParticleSystem clingDust;
    [SerializeField] private float deathStrength = 15f;
    [Header("Run")]
    [SerializeField] private float speed = 500;
    [SerializeField] private float counterMovement = 10;
    [Header("Jump")]
    [SerializeField] private float jumpStrength;
    [SerializeField] private float jumpSlowdown = 1;
    [SerializeField] private float cyoteTime = 0.1f;
    [SerializeField] private float jumpQueueTime = 0.2f;
    [SerializeField] private float fallSpeedUp = 1f;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(1,1);
    [SerializeField] private LayerMask groundMask;
    
    [Header("Wall Climb")]
    [SerializeField] [Range(0,1)] private float fallSlowdown;
    [SerializeField] private float wallPushback = 100;
    [SerializeField] private Vector2 wallCheckSize =  new Vector2(1,1);
    [SerializeField] private float wallJumpMult = 1.3f;
    [SerializeField] private float wallJumpTime = 1f;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallCyoteTime = 0.05f;
    [SerializeField] private float wallJumpChainTime = 0.5f;
    [SerializeField] private float wallJumpBufferTime = 0.1f;

    private float cyoteTimeRef = 0;
    private float wallCyoteTimeRef;
    private float wallJumpTimeRef = 0;
    private float moveX = 0;
    private float maxYRef;

    private float wallJumpChainTimeRef = 0f;
    private float wallJumpBufferTimeRef = 0f;

    private float jumpQueueTimeRef = 0f;
    private bool jumpQueued = false;
    private float orgGravity;
    private bool isGrounded => Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundMask);
    private Vector2 wallDir = Vector2.zero;

    private bool wallClimbIntended = false;

    //private ParticleSystem.VelocityOverLifetimeModule partVel;
    private bool hasPlayedFallDust = true;
    private bool clingDustPlaying = false;

    private Vector2 orgDustPos;

    private void Awake()
    {
        cyoteTimeRef = cyoteTime;
        cyoteTime = 0;
        maxYRef = maxVelocity.y;
        orgGravity = rb.gravityScale;
        wallCyoteTimeRef = 0f;
        //partVel = wallDustParticles.velocityOverLifetime;
        orgDustPos = dustParticles.transform.localPosition;
    }

    private void Update()
    {
        //stuff that happens all the time
        animations();
        //Stuff that happens while in play
        if (GameStateManager.Singleton.getCurrentGameState() != GameStateManager.GameState.Play || PauseMenuManager.Singleton.isPlayingCutscene()) { return; }
        jump();
        cyoteTimeBehavior();
        wallMovement();
    }

    private void FixedUpdate()
    {
        movement();
        limitVelocity();
        counterMovementBehavior();
    }

    private bool isTouchingWall()
    {
        RaycastHit2D rHit = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0f, Vector2.zero, 0f, wallMask);

        bool isHitting = rHit.collider != null;

        if (rHit.point.x > transform.position.x && isHitting)
        {
            wallDir = Vector2.right;
        }
        else if (rHit.point.x < transform.position.x && isHitting)
        {
            wallDir = Vector2.left;
        }
        

        return isHitting && !isGrounded;
    }

    private void wallMovement()
    {
        if (isTouchingWall() && wallClimbIntended && !clingDustPlaying)
        {
            maxVelocity.y = maxYRef - (maxYRef * fallSlowdown);
            wallCyoteTimeRef = wallCyoteTime;
            clingDustPlaying = true;
            clingDust.transform.localPosition = new Vector2(0.55f, clingDust.transform.localPosition.y);
            clingDust.Play();
        }
        else if(clingDustPlaying && !isTouchingWall())
        {
            maxVelocity.y = maxYRef;
            wallCyoteTimeRef -= Time.deltaTime;
            clingDustPlaying = false;
            clingDust.Stop();
        }

        if(isTouchingWall() && moveX != 0 && !wallClimbIntended){
            wallClimbIntended = true;
        }
        else if(!isTouchingWall() && wallJumpChainTimeRef < 0){
            wallClimbIntended = false;
        }

        if(wallJumpChainTimeRef > 0){
            wallCyoteTimeRef -= Time.deltaTime;
        }

        
    }

    private void animations()
    {
        anim.SetBool("isRunning", moveX != 0);
        anim.SetBool("isJumping", !isGrounded);
        anim.SetBool("isOnWall", isTouchingWall() && wallClimbIntended && rb.velocity.y <= 0);
        anim.SetBool("falling", rb.velocity.y <= 0.001f);
    }

    private void movement()
    {
        moveX = Input.GetAxisRaw("Horizontal");

        if(Mathf.Abs(moveX) < 0.2f)
        {
            moveX = 0;
        }
        else
        {
            moveX = 1 * Mathf.Sign(moveX);
        }
        
        if (GameStateManager.Singleton.getCurrentGameState() != GameStateManager.GameState.Play || wallJumpTimeRef > 0 || PauseMenuManager.Singleton.isPlayingCutscene()) { moveX = 0; }

        rb.AddForce(Vector2.right * moveX * speed);
        flipSprite();
    }

    private void counterMovementBehavior()
    {
        if(wallJumpTimeRef > 0){return;}
        rb.AddForce(new Vector2(-rb.velocity.x, 0) * counterMovement, ForceMode2D.Force); // Counter movement
    }

    private void flipSprite()
    {
        if(wallJumpTimeRef > 0)
        {
            if(rb.velocity.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if(rb.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            return;
        }

        if(moveX > 0.2f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if(moveX < -0.2f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void limitVelocity()
    {
        float clampX = Mathf.Clamp(rb.velocity.x, -maxVelocity.x, maxVelocity.x);
        float clampY = Mathf.Clamp(rb.velocity.y, -maxVelocity.y, maxVelocity.y + 1000);

        rb.velocity = new Vector2(clampX, clampY);
    }

    private void jump()
    {

        // Ground Jump
        if (Input.GetButtonDown("Jump") && (isGrounded || (cyoteTime > 0 && rb.velocity.y <= 0)) && !isTouchingWall())
        {
            jumpBehavior();
            cyoteTime = 0;
        }
        else if (Input.GetButtonUp("Jump") && rb.velocity.y > 0 && wallJumpTimeRef <= 0) 
        {
            rb.AddForce(Vector2.down * rb.velocity.y * jumpSlowdown, ForceMode2D.Impulse);
        }
        // Wall jump
        if(Input.GetButtonDown("Jump") && (isTouchingWall() || wallCyoteTimeRef > 0) && !isGrounded && wallClimbIntended)
        {
            wallJumpBehavior();
            wallCyoteTimeRef = 0f;
        }
        else if(Input.GetButtonUp("Jump") && wallJumpTimeRef > 0)
        {
            wallJumpTimeRef = 0;
            rb.velocity = new Vector2(rb.velocity.x/2,rb.velocity.y/2);
            //Debug.Log("Wall Jump Cut");
        }

        if(Input.GetButton("Jump") && !Input.GetButtonUp("Jump")){
            cyoteTime = 0;
        }

        if(wallJumpTimeRef > 0)
        {
            wallJumpTimeRef -= Time.deltaTime;
        }

        if(rb.velocity.y <= 12)
        {
            rb.gravityScale = orgGravity + fallSpeedUp;
        }
        else
        {
            rb.gravityScale = orgGravity;
        }

        jumpQueuer();

        //VFX

        if(!hasPlayedFallDust && isGrounded){
            //dustParticles.Play();
            Instantiate(dustParticles, groundCheck.position, Quaternion.identity);
            hasPlayedFallDust = true;
            dustParticles.transform.position = groundCheck.position;
        }

        if(rb.velocity.y < -0.01f  && hasPlayedFallDust){
            hasPlayedFallDust = false;
        }
    }

    private void jumpQueuer()
    {
        //Normal Jump Buffer

        if (Input.GetButtonDown("Jump") && !isGrounded && rb.velocity.y < 0)
        {
            jumpQueued = true;

            jumpQueueTimeRef = jumpQueueTime;
        }

        if(isGrounded && jumpQueued && Input.GetButton("Jump"))
        {
            jumpBehavior();

            jumpQueued = false;
            jumpQueueTimeRef = 0f;
        }

        if(isGrounded)
        {
            jumpQueued = false;
            jumpQueueTimeRef = 0f;
        }

        if(jumpQueueTimeRef <= 0)
        {
            jumpQueueTimeRef = 0f;
            jumpQueued = false;
        }
        else
        {
            jumpQueueTimeRef -= Time.deltaTime;
        }

        // Wall Jump Buffer

        if(Input.GetButtonDown("Jump") && !isTouchingWall() && !isGrounded){
            wallJumpBufferTimeRef = wallJumpBufferTime;
        }

        if(wallJumpBufferTimeRef > 0){
            wallJumpBufferTimeRef -= Time.deltaTime;
        }

        if(wallJumpBufferTimeRef > 0 && isTouchingWall() && Input.GetButton("Jump")){
            wallJumpBehavior();
            wallJumpBufferTimeRef = 0;

            //Debug.Log("wall jump buffer");
        }

        //Debug.Log($"WJUMP BUFFER TIME: {wallJumpBufferTimeRef}");
    }

    private void jumpBehavior(){
        //dustParticles.transform.localPosition = orgDustPos;
        //dustParticles.Play();
        Instantiate(dustParticles, groundCheck.position, Quaternion.identity);
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
    }

    private void wallJumpBehavior(){
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(-wallDir * wallPushback, ForceMode2D.Impulse);
        rb.AddForce(Vector2.up * (jumpStrength/wallJumpMult), ForceMode2D.Impulse);
        wallJumpTimeRef = wallJumpTime;
        wallJumpChainTimeRef = wallJumpChainTime;

        //var dustVel = wallDustParticles.velocityOverLifetime;
        //dustVel.xMultiplier = transform.localScale.x;

        //wallDustParticles.Play();
        if(transform.localScale.x > 0){
            Instantiate(wallDustParticles, transform.position + Vector3.right*0.5f, wallDustParticles.transform.rotation);
        }
        else{
            Instantiate(wallDustLeft, transform.position - Vector3.right*0.5f, wallDustParticles.transform.rotation);
        }
       
    }

    private void cyoteTimeBehavior()
    {
        if (cyoteTime > 0 && !isGrounded)
        {
            cyoteTime -= Time.deltaTime;
        }

        if (isGrounded)
        {
            cyoteTime = cyoteTimeRef;
        }
    }

    public void die(Vector2 source)
    {
        //Put death stuff here
        GameStateManager.Singleton.setState(GameStateManager.GameState.Cutscene);
        SceneTransitioner.Singleton.restartScene();
        anim.SetTrigger("die");
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        Vector2 dir = source - (Vector2)transform.position;
        rb.AddForce(dir.normalized * deathStrength, ForceMode2D.Impulse);

        rb.velocity = new Vector2();

        //Destroy(gameObject);
    }

    public void pushCat(Vector2 dir)
    {
        rb.AddForce(dir, ForceMode2D.Impulse);
    }

    public void zeroOutYVel()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }

}

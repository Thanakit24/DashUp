using System;
using UnityEngine;
using Cinemachine; 

public class PlayerController : StateMachine, IPlayerController
{
    public LayerMask playerLayer;
    public Rigidbody2D rb;
    private CapsuleCollider2D col;

    [Header("MOVEMENT")]
    public float maxSpeed;
    public float acceleration;
    public float groundDeceleration;
    public float airDeceleration;
    public float maxFallSpeed;
    public float fallAcceleration;
    public float groundingForce;

    //---------------------------------------------------------------------

    [Header("GROUND & COLLISION CHECKS")]
    public LayerMask groundLayerMask;
    [SerializeField] private float grounderDistance;
    public float JumpEndEarlyGravityModifier;
    [SerializeField] private bool _cachedQueryStartInColliders;

    //---------------------------------------------------------------------

    [Header("JUMPING")]
    public int amountOfJumps;
    public float jumpPower;
    [SerializeField] private int maxAmountOfJumps;
    [SerializeField] private float jumpBuffer;
    [SerializeField] private float coyoteTime;
    [HideInInspector] public bool bufferedJumpUsable;
    [HideInInspector] public bool coyoteUsable;
    [HideInInspector] public bool jumpToConsume;
    [HideInInspector] public float timeJumpWasPressed;
    public bool endedJumpEarly;

    //---------------------------------------------------------------------

    [Header("GLIDING")]
    public float maxGlideSpeed;
    public float glideAcceleration;
    public float glideFallSpeed;
    public float glideFallAcceleration;
    public float glideGravityResistance;

    //---------------------------------------------------------------------

    [Header("PIDGEYPOOP")]
    [SerializeField] private float amountOfPoop;
    public Transform poopDropPos;
    public GameObject poopPrefab;

    //---------------------------------------------------------------------

    [HideInInspector] public Vector2 frameVelocity;
    [HideInInspector] public FrameInput frameInput;
    public bool isFacingRight = true;
    private float time;

    [HideInInspector] public Animator anim;

    //Interface
    public Vector2 FrameInput => frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    public override BaseState DefaultState()
    {
        return new GroundMoveState(this);
    }
    protected override void Awake()
    {
        base.Awake();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    protected override void Start()
    {
        base.Start();
        amountOfJumps = maxAmountOfJumps;
    }
    protected override void Update()
    {
        base.Update();
        print(currentState);
        time += Time.deltaTime;
        GatherInput();

        if (frameInput.JumpDown && amountOfJumps > 0) //For Handling Jump conditions and changing States
        {
            jumpToConsume = true;
            timeJumpWasPressed = time;
            ChangeState(new JumpState(this));
        }

        if (frameInput.Move.x > 0 && !isFacingRight)
            FlipSprite();
      
        if (frameInput.Move.x < 0 && isFacingRight)
            FlipSprite();
           
    }
    private void GatherInput()
    {
        frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump") && amountOfJumps > 0,
            JumpHeld = Input.GetButton("Jump"),
            isGliding = Input.GetKey(KeyCode.W) && currentState is AirborneMoveState && rb.velocity.y <= 0
            && time > frameLeftGrounded + coyoteTime && time > timeJumpWasPressed + jumpBuffer,  //currently in air and is falling to ground
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        };

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            PidgeyPoop();
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckCollisions();
        HandleJump();
        //HandleDirection();
        //HandleGravity();
       // ApplyMovement();
        AnimationsHandler();
    }

    private void PidgeyPoop()
    {
        print("function called"); //refine later

        GameObject poop = Instantiate(poopPrefab, poopDropPos.position, Quaternion.identity);
    }
    #region Horizontal Movement
    //private void HandleDirection()
    //{
    //    if (frameInput.isGliding)
    //    {
    //        frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, frameInput.Move.x * maxGlideSpeed, glideAcceleration * Time.fixedDeltaTime);
    //    }

    //    else if (frameInput.Move.x == 0)
    //    {
    //        var deceleration = grounded ? groundDeceleration : airDeceleration;
    //        // this line of code is setting the deceleration variable to either groundDeceleration or airDeceleration
    //        // based on whether the character is currently on the ground (grounded).
    //        // If grounded is true, it will use groundDeceleration; otherwise, it will use airDeceleration
    //        frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
    //    }
    //    else
    //    {
    //        frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, frameInput.Move.x * maxSpeed, acceleration * Time.fixedDeltaTime);
    //    }
    //}

    //private void HandleGravity()
    //{
    //    if (grounded && frameVelocity.y <= 0f)
    //    {
    //        frameVelocity.y = groundingForce;
    //    }
    //    else if (frameInput.isGliding)
    //    {
    //        frameVelocity.y /= glideGravityResistance;
    //        frameVelocity.y = Mathf.MoveTowards(frameVelocity.y, -glideFallSpeed, glideFallAcceleration * Time.fixedDeltaTime);
    //    }
    //    else
    //    {
    //        var inAirGravity = fallAcceleration;
    //        if (endedJumpEarly && frameVelocity.y > 0) inAirGravity *= JumpEndEarlyGravityModifier;
    //        frameVelocity.y = Mathf.MoveTowards(frameVelocity.y, -maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
    //    }

    //}
    //private void ApplyMovement() => rb.velocity = frameVelocity;
    #endregion //Moved to PlayerStates 

    #region Collisions

    private float frameLeftGrounded = float.MinValue;
    public bool grounded;

    private void CheckCollisions() //confusing on the GroundChanged?
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.down, grounderDistance, ~playerLayer);
        bool ceilingHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.up, grounderDistance, ~playerLayer);

        // Hit a Ceiling
        if (ceilingHit) frameVelocity.y = Mathf.Min(0, frameVelocity.y);

        // Landed on the Ground
        if (!grounded && groundHit)
        {
            grounded = true;
            coyoteUsable = true;
            bufferedJumpUsable = true;
            endedJumpEarly = false;
            amountOfJumps = maxAmountOfJumps;
            GroundedChanged?.Invoke(true, Mathf.Abs(frameVelocity.y)); //idk wtf this is
            anim.SetBool("isJumping", false);
        }
        // Left the Ground
        else if (grounded && !groundHit)
        {
            grounded = false;
            frameLeftGrounded = time;
            GroundedChanged?.Invoke(false, 0);
            anim.SetBool("isJumping", true);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;

    }
    #endregion

    #region Jumping

    [HideInInspector] public bool HasBufferedJump => bufferedJumpUsable && time < timeJumpWasPressed + jumpBuffer; //buffering jump before performing next jump
    [HideInInspector] public bool CanUseCoyote => coyoteUsable && !grounded && time < frameLeftGrounded + coyoteTime;

    private void HandleJump() //Jumping conditions 
    {
        if (!endedJumpEarly && !grounded && !frameInput.JumpHeld && rb.velocity.y > 0)
            endedJumpEarly = true;

        if (!jumpToConsume && !HasBufferedJump)
            return;

        
        if (jumpToConsume && amountOfJumps > 0 || CanUseCoyote)
            amountOfJumps--; //placed this here because in JumpState it was decrementing more than once
            ChangeState(new JumpState(this));

        jumpToConsume = false;
    }

    //private void ExecuteJump() //Where jump happens //Moved to player States
    //{
    //    amountOfJumps--;
    //    endedJumpEarly = false;
    //    timeJumpWasPressed = 0;
    //    bufferedJumpUsable = false;
    //    coyoteUsable = false;
    //    frameInput.isGliding = false;
    //    frameVelocity.y = jumpPower;
    //    Jumped?.Invoke();
    //}

    #endregion 
    private void FlipSprite()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

    private void AnimationsHandler()
    {
        anim.SetFloat("xVelocity", Mathf.Abs(frameInput.Move.x));
        anim.SetFloat("yVelocity", rb.velocity.y);
    }

    public void Dead()
    {
        print("kill player");
    }

}
public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public bool isGliding;
    public bool isPoop;
    public Vector2 Move;
}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;

    public event Action Jumped;
    public Vector2 FrameInput { get; }
}


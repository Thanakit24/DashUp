using System;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.UI;


public class PlayerController : StateMachine, IPlayerController
{
    public LayerMask playerLayer;
    [HideInInspector] public Rigidbody2D rb;
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

    [Header("JUMPING")]
    public float jumpPower;
    public int amountOfJumps;
    public int maxAmountOfJumps;
    [SerializeField] private float jumpBuffer;
    [HideInInspector] public bool bufferedJumpUsable;
    [HideInInspector] public bool jumpToConsume = false;
    [HideInInspector] public float timeJumpWasPressed;
    [HideInInspector] public bool endedJumpEarly;
    public float coyoteTime;
    public bool coyoteUsable;
    public bool blackJump;


    //---------------------------------------------------------------------

    [Header("GLIDING")]
    public float maxGlideSpeed;
    public float glideAcceleration;
    public float glideFallSpeed;
    public float glideFallAcceleration;
    public float glideGravityResistance;

    //---------------------------------------------------------------------

    [Header("FLYING")]
    public float maxFlySpeed;
    public float flyAcceleration;
    public float flyUpwardSpeed;
    public float flyUpwardAcceleration;
    public float airDownwardForce;
    public bool firstLaunch = false;
    public float launchDeplete;
    //public float launchPower = 15f;
    //public float testLaunch;

    //---------------------------------------------------------------------

    [Header("PIDGEYPOOP")]
    public int amountOfPoop;
    public int maxPoop;
    public float poopPower;
    public float poopRegen;
    public float poopRegenIncrement;
    public float poopMaxRegen;
    public Transform poopDropPos;
    public GameObject poopPrefab;

    //---------------------------------------------------------------------

    [Header("GROUND & COLLISION CHECKS")]
    public LayerMask groundLayerMask;
    [SerializeField] private float grounderDistance;
    public float JumpEndEarlyGravityModifier;
    [SerializeField] private bool _cachedQueryStartInColliders;

    //---------------------------------------------------------------------

    [Header("ENERGY BAR")]
    public Transform barDisplay;
    public Slider energyBar;
    public float currentEnergy;
    public float increaseEnergy;
    public float depleteEnergy;
    public float maxEnergy;

    //---------------------------------------------------------------------

    [Header("PARTICLES")]
    public ParticleSystem blackAura;
    public TextMeshProUGUI poopCounter;
    public SpriteRenderer sprite;
    
   
    //Animation Keys 
    [HideInInspector] public Animator anim;
    public static readonly int AnimSpeedParameter = Animator.StringToHash("animSpeed");
    public static readonly int IdleKey = Animator.StringToHash("Idle");
    public static readonly int MoveKey = Animator.StringToHash("Move");
    public static readonly int JumpKey = Animator.StringToHash("Jump");
    public static readonly int BlackJumpkey = Animator.StringToHash("BlackJump");
    public static readonly int FallKey = Animator.StringToHash("Fall");
    public static readonly int GlideKey = Animator.StringToHash("Glide");
    public static readonly int FlyKey = Animator.StringToHash("Fly");
    public static readonly int DeathKey = Animator.StringToHash("Death");
    public static readonly int DeathKey2 = Animator.StringToHash("Death2");

    //--------------------------------------------------------------------

    [HideInInspector] public Vector2 frameVelocity;
    [HideInInspector] public FrameInput frameInput;
    public bool isFacingRight = true;
    private float time;
    [HideInInspector] public TrailRenderer trail;

    //--------------------------------------------------------------------
    //Interface
    public Vector2 FrameInput => frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    //public event Action Jumped;
  

    public override BaseState DefaultState()
    {
        return new GroundMoveState(this);
    }
    protected override void Awake()
    {
        base.Awake();
        trail = GetComponentInChildren<TrailRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        //blackAura = GetComponentInChildren<ParticleSystem>();
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;

    }

    protected override void Start()
    {
        base.Start();
        rb.velocity = Vector2.zero;
        amountOfJumps = maxAmountOfJumps;
        currentEnergy = maxEnergy;
        energyBar.value = maxEnergy;
        energyBar.maxValue = maxEnergy;
        trail.emitting = true;
        blackAura.Play(false);
        energyBar.gameObject.SetActive(false);
        //blackLaunch.Play(false);
    }
    protected override void Update()
    {
        
        base.Update();
        //print(currentState);
        SetUpEnergyBar();
        SetUpPoop();
        time += Time.deltaTime;

        GatherInput();
        //if (frameInput.isTest)
        //{
        //    frameVelocity.y = 0f;
        //    frameVelocity.y = testLaunch;
        //    //frameVelocity = new Vector2(frameInput.Move.x * testLaunch, frameVelocity.y * testLaunch);
        //}
        if (frameInput.isLaunch && !firstLaunch) //this prevents spamming "fly input" to gain speed and minimize energy consumption
        {
            firstLaunch = true;

            if (firstLaunch)
            {
                currentEnergy -= launchDeplete;
                firstLaunch = false;
            }
        }


        if (frameInput.Move.x > 0 && !isFacingRight)
            FlipSprite();

        if (frameInput.Move.x < 0 && isFacingRight)
            FlipSprite();

    }

    private void SetUpEnergyBar() //set up energy bar position and particle effects
    {
        energyBar.gameObject.transform.position = barDisplay.position;
        energyBar.value = currentEnergy;
        
        #region EnergyBar
        if (currentState is GlideState || currentState is FlyState)
        {
            energyBar.gameObject.SetActive(true);
        }
        else if (grounded && currentEnergy <= maxEnergy)
        {
            currentEnergy += increaseEnergy * Time.deltaTime;
            if (currentEnergy >= maxEnergy)
            {
                currentEnergy = maxEnergy;
                
            }
        }

        if (currentEnergy == maxEnergy)
        {
            energyBar.gameObject.SetActive(false);
        }

        if (currentEnergy <= maxEnergy / 2.05f)
        {
            float deductThreshold = maxEnergy / 2.05f;
            float energyRatio = currentEnergy / deductThreshold;
            // Set the red component directly, keeping green and blue at 0
            float decreaseChannels = Mathf.Clamp01(energyRatio); // Ensure redness is between 0 and 1
            sprite.color = new Color(255, Mathf.Max(decreaseChannels, 0.4f), Mathf.Max(decreaseChannels, 0.4f), 255f);
        } 
        else
        {
            sprite.color = new Color(255, 255, 255);
        }
        #endregion
    }

    private void SetUpPoop()
    {
        if (amountOfPoop > 0)
        {
            if (!blackAura.isPlaying) //needs to check or else it wont play/cause inconsistency
            {
                blackAura.Play();
            }
            poopCounter.gameObject.SetActive(true);
            poopCounter.text = $" {amountOfPoop}"; //might remove this

        }
        else
        {
            
            poopCounter.gameObject.SetActive(false);
            if (blackAura.isPlaying)
            {
                blackAura.Stop();
            }
        }

        if (amountOfPoop >= maxPoop)
        {
            amountOfPoop = maxPoop;
        }
    }
    private void GatherInput()
    {
        frameInput = new FrameInput
        {
            JumpDown = (Input.GetButtonDown("Jump") || Input.GetKey(KeyCode.C)) && (amountOfJumps > 0 || amountOfPoop > 0),
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
            isGliding = (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftShift)) && !grounded && rb.velocity.y <= 0f && currentEnergy > 0,
            isFlying = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && currentEnergy > 0,
            //isPoop = (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.P)) && amountOfPoop > 0,
            isLaunch = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow),
            //isTest = Input.GetKeyDown(KeyCode.J),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        };

        if (frameInput.JumpDown) //For Handling Jump conditions
        {
            jumpToConsume = true;
            timeJumpWasPressed = time;
        }

        if (currentState is not JumpState)  //JumpState is set in HandleJump method
        {
            if (frameInput.isFlying && currentState is not GlideState)
            {
                ChangeState(new FlyState(this));
            }

            else if (frameInput.isGliding && currentState is not FlyState)
            {
                ChangeState(new GlideState(this));
            }
        }

    }
    protected override void FixedUpdate()
    {
        HandleJump();
        CheckCollisions();
        base.FixedUpdate(); // player states relies on the first two functions, handle jump and check collision

        //HandleDirection();
        //HandleGravity();
        ApplyMovement();
        //AnimationsHandler();
    }

    #region Horizontal Movement (Moved To Player States)
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
    private void ApplyMovement() => rb.velocity = frameVelocity;
    #endregion //Moved to PlayerStates 

    #region Collisions

    private float frameLeftGrounded = float.MinValue;
    public bool grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.down, grounderDistance, groundLayerMask) && (currentState is not FlyState || currentState is not GlideState);
        //added is not flying check because when the player collides with the ground while flying, it resets the energy bar to full 
        bool ceilingHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.up, grounderDistance, groundLayerMask);

        // Hit a Ceiling
        if (ceilingHit) frameVelocity.y = Mathf.Min(0, frameVelocity.y);

        // Landed on the Ground

        if (!grounded && groundHit) /// adding the check here lead to animation bugs when flying
        {
            grounded = true;
            coyoteUsable = true;
            bufferedJumpUsable = true;
            endedJumpEarly = false;
            amountOfJumps = maxAmountOfJumps;
            //currentEnergy = maxEnergy;
            sprite.color = new Color(255, 255, 255);
            GroundedChanged?.Invoke(true, Mathf.Abs(frameVelocity.y));

        }
        // Left the Ground
        else if (grounded && !groundHit)
        {
            grounded = false;
            frameLeftGrounded = time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;

    }
    #endregion

    #region Jumping (ExecuteJump Moved to PlayerStates)

    [HideInInspector] public bool HasBufferedJump => bufferedJumpUsable && time < timeJumpWasPressed + jumpBuffer; //buffering jump before performing next jump
    [HideInInspector] public bool CanUseCoyote => coyoteUsable && !grounded && time < frameLeftGrounded + coyoteTime;

    private void HandleJump() //Jumping conditions 
    {
        if (!endedJumpEarly && !grounded && !frameInput.JumpHeld && rb.velocity.y > 0)
            endedJumpEarly = true;

        if (!jumpToConsume && !HasBufferedJump)
            return;

        if (jumpToConsume)
        {
            ChangeState(new JumpState(this));
        }

        jumpToConsume = false;

    }
    #endregion 
    private void FlipSprite()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

    public void Dead()
    {
        print("kill player");
        ChangeState(new DeathState(this));
    }

}
public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public bool isGliding;
    public bool isFlying;
    public bool isLaunch;
    public bool isPoop;
    //public bool isTest;
    public Vector2 Move;
}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;

    //public event Action Jumped;
    public Vector2 FrameInput { get; }
}



//split animations using anim keys, 


//DebugCode for Coyotetime, finding a good balance between when the player can perform a jump from coyote and when the player can glide when not on ground =
//if (currentState is JumpState && time < frameLeftGrounded + coyoteTime)
//{
//    //print("used coyote");
//    coyoteUsedThisFrame = true;
//    print(coyoteUsedThisFrame);

//}
//else if (coyoteUsedThisFrame)
//{
//    coyoteUsedThisFrame = false;
//}

//void ReadDoubleTapInput()
//{
//    switch (inputState)
//    {
//        case InputState.off:
//            if (Input.GetKeyDown(KeyCode.W))
//            {
//                inputState = InputState.reading;
//                inputTime = Time.time;
//            }
//            break;
//        case InputState.reading:
//            if (Input.GetKeyDown(KeyCode.W) && Time.time - inputTime < doubleTapThreshold)
//            {
//                // purely for input reading for the duration of threshold
//                inputState = InputState.held;
//                doubleTapped = true;
//                print("Double Tap");
//            }
//            if (Time.time - inputTime >= doubleTapThreshold)
//            {
//                inputState = Input.GetKey(KeyCode.W) ? InputState.held : InputState.off;
//            }
//            break;
//        case InputState.held:
//            if (!Input.GetKey(KeyCode.W))
//            {
//                inputState = InputState.off;
//                doubleTapped = false;
//                // if doubletapped true, fly
//                // if false, glide
//            }

//            //hold W to glide 
//            //tap W and hold W, fly
//            break;
//    }
//}


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